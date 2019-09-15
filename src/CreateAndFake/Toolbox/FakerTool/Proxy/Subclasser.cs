using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace CreateAndFake.Toolbox.FakerTool.Proxy
{
    /// <summary>Creates dynamic subclasses.</summary>
    [SuppressMessage("Sonar", "S125:RemoveCommentedCode", Justification = "Documents emit code.")]
    public static class Subclasser
    {
        /// <summary>Assembly used to contain the dynamic types.</summary>
        public static AssemblyName AssemblyName { get; } = new AssemblyName("FakerTypes");

        /// <summary>Flags used to find members to implement.</summary>
        private const BindingFlags _MemberFinder = BindingFlags.FlattenHierarchy |
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        /// <summary>Underlying type managing implementation details.</summary>
        private static readonly Type
            _FakeType = typeof(IFaked),
            _MetaType = typeof(FakeMetaProvider);

        /// <summary>Methods called to chain fake calls.</summary>
        private static readonly MethodInfo
            _VoidChainer = _MetaType.GetMethod(
                nameof(FakeMetaProvider.CallVoid),
                BindingFlags.Instance | BindingFlags.NonPublic),
            _ResultChainer = _MetaType.GetMethod(
                nameof(FakeMetaProvider.CallRet),
                BindingFlags.Instance | BindingFlags.NonPublic),
            _TypeResolver = typeof(Type).GetMethod(
                nameof(Type.GetTypeFromHandle),
                BindingFlags.Static | BindingFlags.Public);

        /// <summary>Module storing the faked types.</summary>
        private static readonly ModuleBuilder _Module = AssemblyBuilder
            .DefineDynamicAssembly(AssemblyName, AssemblyBuilderAccess.RunAndCollect)
            .DefineDynamicModule("FakerTypesModule");

        /// <summary>Cache of already created types.</summary>
        private static readonly IDictionary<TypeInfo, Type[]> _TypeCache
            = new Dictionary<TypeInfo, Type[]>();

        /// <summary>Determines if the type can be faked.</summary>
        /// <typeparam name="T">Type to check.</typeparam>
        /// <returns>True if possible; false otherwise.</returns>
        public static bool Supports<T>()
        {
            return Supports(typeof(T));
        }

        /// <summary>Determines if the type can be faked.</summary>
        /// <param name="type">Type to check.</param>
        /// <returns>True if possible; false otherwise.</returns>
        public static bool Supports(Type type)
        {
            return CanBeSubclassed(type).Item1;
        }

        /// <summary>Creates a subclass of the given type.</summary>
        /// <typeparam name="T">Parent type to inherit from.</typeparam>
        /// <param name="interfaces">Extra interfaces to implement.</param>
        /// <returns>An instance of the created child type.</returns>
        public static T Create<T>(params Type[] interfaces)
        {
            return (T)Create(typeof(T), interfaces);
        }

        /// <summary>Creates a subclass of the given type.</summary>
        /// <param name="parent">Parent type to inherit from.</param>
        /// <param name="interfaces">Extra interfaces to implement.</param>
        /// <returns>An instance of the created child type.</returns>
        public static IFaked Create(Type parent, params Type[] interfaces)
        {
            return (IFaked)CreateInfo(parent, interfaces).AsType()
                .GetConstructor(new[] { _MetaType })
                .Invoke(new[] { new FakeMetaProvider() });
        }

        /// <summary>Creates a subclass of the given type.</summary>
        /// <param name="parent">Parent type to inherit from.</param>
        /// <param name="interfaces">Extra interfaces to implement.</param>
        /// <returns>The created child type.</returns>
        internal static TypeInfo CreateInfo(Type parent, params Type[] interfaces)
        {
            IList<Type> allInterfaces = interfaces?.ToList() ?? new List<Type>();
            allInterfaces.Add(_FakeType);

            Type realParent = parent ?? typeof(object);
            if (realParent.IsInterface)
            {
                allInterfaces.Add(realParent);
                realParent = typeof(object);
            }

            (bool, Exception) possible = CanBeSubclassed(realParent);
            if (!possible.Item1)
            {
                throw possible.Item2;
            }

            IEnumerable<Type> invalidInterfaces = allInterfaces.Where(t => !t.IsInterface);
            if (invalidInterfaces.Any())
            {
                throw new ArgumentException("Given interfaces not actually interfaces: '" +
                    string.Join("', '", invalidInterfaces.Select(t => t.Name)) + "'.");
            }

            return FindOrBuildType(realParent, allInterfaces.Distinct().ToArray());
        }

        /// <summary>Determines if type can be subclassed.</summary>
        /// <param name="parent">Type to check.</param>
        /// <returns>True if possible; false if not with exception to throw.</returns>
        private static (bool, Exception) CanBeSubclassed(Type parent)
        {
            if (parent == null)
            {
                return (true, null);
            }
            else if (parent.IsSealed)
            {
                return (false, new ArgumentException($"Cannot subclass the sealed type '{parent.Name}'."));
            }
            else if (parent.IsPointer)
            {
                return (false, new ArgumentException($"Cannot subclass the pointer '{parent.Name}'."));
            }
            else if (parent.ContainsGenericParameters)
            {
                return (false, new ArgumentException($"Cannot subclass with unspecified generics '{parent.Name}'."));
            }
            else if (!parent.IsVisibleTo(AssemblyName))
            {
                return (false, new ArgumentException($"Cannot subclass with nonpublic type '{parent.Name}'. "
                    + $"Think about adding 'InternalsVisibleTo(\"{AssemblyName.Name}\")' to the type's assembly."));
            }
            else if (parent == typeof(Array))
            {
                return (false, new ArgumentException("Cannot subclass system reserved 'Array' type."));
            }
            else
            {
                return (true, null);
            }
        }

        /// <summary>Finds or creates a type with the given hierarchy.</summary>
        /// <param name="parent">Parent type to inherit from.</param>
        /// <param name="interfaces">Extra interfaces to implement.</param>
        /// <returns>The cached or created child type.</returns>
        private static TypeInfo FindOrBuildType(Type parent, Type[] interfaces)
        {
            lock (_TypeCache)
            {
                TypeInfo cachedType = _TypeCache
                    .Where(t => t.Key.BaseType == parent)
                    .SingleOrDefault(t => !t.Value
                        .Except(interfaces)
                        .Union(interfaces
                            .Except(t.Value))
                        .Any()).Key;

                if (cachedType != null)
                {
                    return cachedType;
                }
                else
                {
                    TypeInfo newType = BuildType(parent, interfaces);
                    _TypeCache.Add(newType, interfaces);
                    return newType;
                }
            }
        }

        /// <summary>Creates a type with the given inheritence.</summary>
        /// <param name="parent">Base class inheriting from.</param>
        /// <param name="interfaces">Additional interfaces to inherit.</param>
        /// <returns>Dynamic type with behavior faked.</returns>
        private static TypeInfo BuildType(Type parent, Type[] interfaces)
        {
            TypeBuilder newType = _Module.DefineType(
                "Fake[" + string.Join("|", interfaces.Prepend(parent).Select(i => i.Name)) + "]-" + Guid.NewGuid(),
                TypeAttributes.NotPublic | TypeAttributes.Sealed, parent, interfaces);

            MethodInfo metaGetter = SetupFakeMetaProvider(newType, parent);

            (PropertyInfo, PropertyBuilder)[] props =
                FindImplementableProperties(interfaces.Prepend(parent))
                .Select(p => (p, newType.DefineProperty(
                    p.DeclaringType.Name + "." + p.Name,
                    p.Attributes,
                    p.PropertyType,
                    Type.EmptyTypes)))
                .ToArray();

            foreach (MethodInfo method in FindImplementableMethods(interfaces.Prepend(parent)))
            {
                MethodBuilder fakedMethod = newType.DefineMethod(
                    method.DeclaringType.Name + "." + method.Name,
                    method.Attributes & ~MethodAttributes.Abstract,
                    method.ReturnType,
                    method.GetParameters().Select(p => p.ParameterType).ToArray());

                if (method.IsGenericMethod)
                {
                    fakedMethod.DefineGenericParameters(method.GetGenericArguments().Select(a => a.Name).ToArray());
                }
                ImplementFakeBehavior(fakedMethod.GetILGenerator(), method, metaGetter);
                newType.DefineMethodOverride(fakedMethod, method);

                foreach ((PropertyInfo, PropertyBuilder) prop in props)
                {
                    if (prop.Item1.GetMethod == method)
                    {
                        prop.Item2.SetGetMethod(fakedMethod);
                    }
                    else if (prop.Item1.SetMethod == method)
                    {
                        prop.Item2.SetSetMethod(fakedMethod);
                    }
                }
            }

            return newType.CreateTypeInfo();
        }

        /// <summary>Finds all properties to generate for a type implementing the given interfaces.</summary>
        /// <param name="interfaces">Interfaces to inherit.</param>
        /// <returns>Properties to implement for the faked type.</returns>
        private static IEnumerable<PropertyInfo> FindImplementableProperties(IEnumerable<Type> interfaces)
        {
            return interfaces
                .Where(t => t != typeof(IFaked))
                .SelectMany(i => i
                    .GetProperties(_MemberFinder)
                    .Where(p => IsVisible(p.GetMethod))
                    .Concat(FindImplementableProperties(i.GetInterfaces())))
                .Distinct();
        }

        /// <summary>Finds all methods to generate for a type implementing the given interfaces.</summary>
        /// <param name="interfaces">Interfaces to inherit.</param>
        /// <returns>Methods to implement for the faked type.</returns>
        private static IEnumerable<MethodInfo> FindImplementableMethods(IEnumerable<Type> interfaces)
        {
            return interfaces
                .Where(t => t != typeof(IFaked))
                .SelectMany(i => i
                    .GetMethods(_MemberFinder)
                    .Where(m => m.IsAbstract || (m.IsVirtual && !m.IsFinal))
                    .Where(m => IsVisible(m))
                    .Where(m => m.Name != "Finalize")
                    .Concat(FindImplementableMethods(i.GetInterfaces())))
                .Distinct();
        }

        /// <summary>Determines if the method is visible for faking.</summary>
        /// <param name="method">Method to check.</param>
        /// <returns>True if visible; false otherwise.</returns>
        private static bool IsVisible(MethodInfo method)
        {
            if (method == null)
            {
                return true;
            }

            return !method.IsPrivate
                && (!(method.IsAssembly || method.IsFamilyAndAssembly)
                    || method.Module.Assembly.GetCustomAttributes<InternalsVisibleToAttribute>()
                        .Any(a => a.AssemblyName == AssemblyName.Name));
        }

        /// <summary>Chains the call for a method.</summary>
        /// <param name="gen">Generator for the fake method.</param>
        /// <param name="method">Method being implemented.</param>
        /// <param name="metaGetter">Hook for the meta provider to chain with.</param>
        private static void ImplementFakeBehavior(ILGenerator gen, MethodInfo method, MethodInfo metaGetter)
        {
            ParameterInfo[] argInfos = method.GetParameters();
            Type[] generics = method.GetGenericArguments();

            // object[] args = new object[params.Length];
            LocalBuilder args = gen.DeclareLocal(typeof(object[]));
            gen.Emit(OpCodes.Ldc_I4, argInfos.Length);
            gen.Emit(OpCodes.Newarr, typeof(object));
            gen.Emit(OpCodes.Stloc, args);

            // args[0..x] = params[0..x];
            for (int i = 0; i < argInfos.Length; i++)
            {
                gen.Emit(OpCodes.Ldloc, args);
                gen.Emit(OpCodes.Ldc_I4, i);
                if (!argInfos[i].ParameterType.IsByRef)
                {
                    gen.Emit(OpCodes.Ldarg, i + 1);
                    gen.Emit(OpCodes.Box, argInfos[i].ParameterType);
                    gen.Emit(OpCodes.Stelem_Ref);
                }
                else
                {
                    // args[i] = new OutRef<T>();
                    Type outRef = typeof(OutRef<>).MakeGenericType(argInfos[i].ParameterType.GetElementType());
                    gen.Emit(OpCodes.Newobj, outRef.GetConstructor(Type.EmptyTypes));
                    gen.Emit(OpCodes.Stelem_Ref);
                    if (!argInfos[i].IsOut)
                    {
                        // ((OutRef<T>)args[i].Var) = params[i];
                        gen.Emit(OpCodes.Ldloc, args);
                        gen.Emit(OpCodes.Ldc_I4, i);
                        gen.Emit(OpCodes.Ldelem_Ref);
                        gen.Emit(OpCodes.Castclass, outRef);
                        gen.Emit(OpCodes.Ldarg, i + 1);
                        gen.Emit(OpCodes.Ldind_Ref);
                        gen.Emit(OpCodes.Stfld, outRef.GetField(nameof(OutRef<Type>.Var)));
                    }
                }
            }

            // Type[] types = new Type[generics.Length];
            LocalBuilder types = gen.DeclareLocal(typeof(Type[]));
            gen.Emit(OpCodes.Ldc_I4, generics.Length);
            gen.Emit(OpCodes.Newarr, typeof(Type));
            gen.Emit(OpCodes.Stloc, types);

            // types[0..x] = typeof(generics[0..x]);
            for (int i = 0; i < generics.Length; i++)
            {
                gen.Emit(OpCodes.Ldloc, types);
                gen.Emit(OpCodes.Ldc_I4, i);
                gen.Emit(OpCodes.Ldtoken, generics[i]);
                gen.Emit(OpCodes.Call, _TypeResolver);
                gen.Emit(OpCodes.Stelem_Ref);
            }

            // this.FakeMeta.Call('method.Name', types, args);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Call, metaGetter);
            gen.Emit(OpCodes.Ldstr, method.Name);
            gen.Emit(OpCodes.Ldloc, types);
            gen.Emit(OpCodes.Ldloc, args);
            if (method.ReturnType != typeof(void))
            {
                gen.Emit(OpCodes.Call, _ResultChainer.MakeGenericMethod(method.ReturnType));
            }
            else
            {
                gen.Emit(OpCodes.Call, _VoidChainer);
            }

            // params[0..x] = ((OutRef)args[0..x]).Var;
            for (int i = 0; i < argInfos.Length; i++)
            {
                if (argInfos[i].ParameterType.IsByRef)
                {
                    Type outRef = typeof(OutRef<>).MakeGenericType(argInfos[i].ParameterType.GetElementType());

                    gen.Emit(OpCodes.Ldarg, i + 1);
                    gen.Emit(OpCodes.Ldloc, args);
                    gen.Emit(OpCodes.Ldc_I4, i);
                    gen.Emit(OpCodes.Ldelem_Ref);
                    gen.Emit(OpCodes.Castclass, outRef);
                    gen.Emit(OpCodes.Ldfld, outRef.GetField(nameof(OutRef<Type>.Var)));
                    gen.Emit(OpCodes.Stind_Ref);
                }
            }

            gen.Emit(OpCodes.Ret);
        }

        /// <summary>Hooks up the fake behavior mechanism for the new type.</summary>
        /// <param name="newType">Dynamic type being created.</param>
        /// <param name="parent">Base class inheriting from.</param>
        /// <returns>Info for the meta provider hook.</returns>
        private static MethodInfo SetupFakeMetaProvider(TypeBuilder newType, Type parent)
        {
            ConstructorInfo baseConstuctor = parent
                .GetConstructors(_MemberFinder)
                .SingleOrDefault(c => !c.GetParameters().Any());

            FieldBuilder backingField = newType.DefineField(
                newType.Name + ".m_" + nameof(IFaked.FakeMeta), _MetaType,
                FieldAttributes.Private | FieldAttributes.InitOnly);

            ConstructorBuilder constructor = newType.DefineConstructor(
                MethodAttributes.Public, CallingConventions.HasThis, new[] { _MetaType });
            {
                // base();
                ILGenerator newGenerator = constructor.GetILGenerator();
                if (baseConstuctor != null)
                {
                    newGenerator.Emit(OpCodes.Ldarg_0);
                    newGenerator.Emit(OpCodes.Call, baseConstuctor);
                }

                // this.m_FakeMeta = params[0];
                newGenerator.Emit(OpCodes.Ldarg_0);
                newGenerator.Emit(OpCodes.Ldarg_1);
                newGenerator.Emit(OpCodes.Stfld, backingField);
                newGenerator.Emit(OpCodes.Ret);
            }

            PropertyInfo propInfo = _FakeType.GetProperty(nameof(IFaked.FakeMeta));
            MethodInfo getterInfo = propInfo.GetGetMethod();

            MethodBuilder getMetaMethod = newType.DefineMethod(nameof(IFaked) + "." + getterInfo.Name,
                getterInfo.Attributes & ~MethodAttributes.Abstract, _MetaType, Type.EmptyTypes);
            {
                // return this.m_FakeMeta;
                ILGenerator getGenerator = getMetaMethod.GetILGenerator();
                getGenerator.Emit(OpCodes.Ldarg_0);
                getGenerator.Emit(OpCodes.Ldfld, backingField);
                getGenerator.Emit(OpCodes.Ret);
            }

            newType.DefineProperty(nameof(IFaked) + "." + propInfo.Name,
                propInfo.Attributes, _MetaType, Type.EmptyTypes).SetGetMethod(getMetaMethod);

            newType.DefineMethodOverride(getMetaMethod, getterInfo);
            return getterInfo;
        }
    }
}
