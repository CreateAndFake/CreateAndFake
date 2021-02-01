using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CreateAndFake.Toolbox.DuplicatorTool.CopyHints
{
    /// <summary>Handles copying objects for the duplicator.</summary>
    public sealed class ObjectCopyHint : CopyHint
    {
        /// <summary>Flags used to identify members.</summary>
        private const BindingFlags _MemberFlags = BindingFlags.Public
            | BindingFlags.NonPublic | BindingFlags.Instance;

        /// <summary>Tries to deep clone an object.</summary>
        /// <param name="source">Object to clone.</param>
        /// <param name="duplicator">Handles callback behavior for child values.</param>
        /// <returns>If the type could be cloned and the cloned instance.</returns>
        protected internal sealed override (bool, object) TryCopy(object source, DuplicatorChainer duplicator)
        {
            if (duplicator == null) throw new ArgumentNullException(nameof(duplicator));
            if (source == null) return (true, null);

            object result = Copy(source, duplicator);
            return (result != null, result);
        }

        /// <summary>Deep clones an object.</summary>
        /// <param name="source">Object to clone.</param>
        /// <param name="duplicator">Handles callback behavior for child values.</param>
        /// <returns>Duplicate object.</returns>
        private static object Copy(object source, DuplicatorChainer duplicator)
        {
            object dupe = CreateNew(source, duplicator);
            if (dupe == null)
            {
                return dupe;
            }
            duplicator.AddToHistory(source, dupe);

            foreach (FieldInfo field in source.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public)
                .Where(f => !f.IsInitOnly && !f.IsLiteral))
            {
                field.SetValue(dupe, duplicator.Copy(field.GetValue(source)));
            }

            foreach (PropertyInfo property in source.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CanRead && p.CanWrite))
            {
                property.SetValue(dupe, duplicator.Copy(property.GetValue(source)));
            }

            return dupe;
        }

        /// <summary>Creates an instance of an object.</summary>
        /// <param name="source">Object to create.</param>
        /// <param name="duplicator">Handles callback behavior for child values.</param>
        /// <returns>Created instance.</returns>
        private static object CreateNew(object source, DuplicatorChainer duplicator)
        {
            Type type = source.GetType();

            if (type.GetConstructor(Type.EmptyTypes) != null)
            {
                return Activator.CreateInstance(type);
            }
            else
            {
                PropertyInfo[] props = type.GetProperties(_MemberFlags).Where(p => p.CanRead).ToArray();
                FieldInfo[] fields = type.GetFields(_MemberFlags).ToArray();

                return type.GetConstructors(_MemberFlags)
                    .Where(c => !c.IsPrivate)
                    .OrderByDescending(c => c.GetParameters().Length)
                    .Select(c => TryCreate(source, duplicator, c, props, fields))
                    .FirstOrDefault(o => o != null);
            }
        }

        /// <summary>Attempts to create and instance using a constructor.</summary>
        /// <param name="source">Object being cloned.</param>
        /// <param name="duplicator">Handles callback behavior for child values.</param>
        /// <param name="constructor">Constructor to use.</param>
        /// <param name="props">Properties on the object.</param>
        /// <param name="fields">Fields on the object.</param>
        /// <returns>Null if failed; created instance otherwise.</returns>
        private static object TryCreate(object source, DuplicatorChainer duplicator,
            ConstructorInfo constructor, IEnumerable<PropertyInfo> props, IEnumerable<FieldInfo> fields)
        {
            List<PropertyInfo> propList = props.ToList();
            List<FieldInfo> fieldList = fields.ToList();

            // Attempts to match members with parameters in the constructor.
            List<MemberInfo> matchedMembers = new();
            foreach (ParameterInfo param in constructor.GetParameters())
            {
                PropertyInfo[] potentialProps = propList
                    .Where(p => p.PropertyType.Inherits(param.ParameterType))
                    .ToArray();
                if (potentialProps.Any())
                {
                    PropertyInfo directPropMatch = potentialProps.FirstOrDefault(
                        p => p.Name.Equals(param.Name, StringComparison.OrdinalIgnoreCase));
                    if (directPropMatch != null)
                    {
                        _ = propList.Remove(directPropMatch);
                        matchedMembers.Add(directPropMatch);
                    }
                    else
                    {
                        _ = propList.Remove(potentialProps.First());
                        matchedMembers.Add(potentialProps.First());
                    }
                    continue;
                }

                FieldInfo[] potentialFields = fieldList
                    .Where(f => f.FieldType.Inherits(param.ParameterType))
                    .ToArray();
                if (potentialFields.Any())
                {
                    FieldInfo directFieldMatch = potentialFields.FirstOrDefault(
                        f => f.Name.Equals(param.Name, StringComparison.OrdinalIgnoreCase));
                    if (directFieldMatch != null)
                    {
                        _ = fieldList.Remove(directFieldMatch);
                        matchedMembers.Add(directFieldMatch);
                    }
                    else
                    {
                        _ = fieldList.Remove(potentialFields.First());
                        matchedMembers.Add(potentialFields.First());
                    }
                    continue;
                }

                return null;
            }

            return constructor.Invoke(matchedMembers.Select(m => CopyMember(m, source, duplicator)).ToArray());
        }

        /// <summary>Copies the value of the member on the source.</summary>
        /// <param name="member">Property or field to copy.</param>
        /// <param name="source">Instance containing the member.</param>
        /// <param name="duplicator">Duplicator handling the cloning.</param>
        /// <returns>Duplicate object.</returns>
        private static object CopyMember(MemberInfo member, object source, DuplicatorChainer duplicator)
        {
            if (member is PropertyInfo prop)
            {
                return duplicator.Copy(prop.GetValue(source));
            }
            else
            {
                return duplicator.Copy(((FieldInfo)member).GetValue(source));
            }
        }
    }
}
