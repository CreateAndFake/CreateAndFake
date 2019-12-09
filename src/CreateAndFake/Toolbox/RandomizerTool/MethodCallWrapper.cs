﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CreateAndFake.Toolbox.DuplicatorTool;

namespace CreateAndFake.Toolbox.RandomizerTool
{
    /// <summary>Holds parameter data for a method.</summary>
    public sealed class MethodCallWrapper : IDuplicatable
    {
        /// <summary>Associated method.</summary>
        private readonly MethodBase _method;

        /// <summary>Ordered parameter names.</summary>
        private readonly IEnumerable<string> _names;

        /// <summary>Parameter values.</summary>
        private readonly IDictionary<string, object> _args;

        /// <summary>Parameter data for the method.</summary>
        public IEnumerable<object> Args => _names.Select(n => _args[n]).ToArray();

        /// <summary>Initializer.</summary>
        /// <param name="method">Associated method.</param>
        /// <param name="args">Parameter data for the method.</param>
        public MethodCallWrapper(MethodBase method, IEnumerable<Tuple<string, object>> args)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));

            _method = method ?? throw new ArgumentNullException(nameof(method));
            _args = new Dictionary<string, object>();

            IList<string> names = new List<string>();
            foreach (Tuple<string, object> arg in args)
            {
                names.Add(arg.Item1);
                _args.Add(arg.Item1, arg.Item2);
            }
            _names = names;
        }

        /// <summary>Modifies a parameter value.</summary>
        /// <param name="name">Name for the parameter to modify.</param>
        /// <param name="value">New value to use.</param>
        public void ModifyArg(string name, object value)
        {
            if (_args.ContainsKey(name))
            {
                _args[name] = value;
            }
            else
            {
                throw new KeyNotFoundException($"Parameter '{name}' not on method '{_method.Name}'.");
            }
        }

        /// <summary>Invokes the method on the instance.</summary>
        /// <param name="instance">Instance to call the method with the data on.</param>
        /// <returns>Results from the call.</returns>
        public object InvokeOn(object instance)
        {
            return _method.Invoke(instance, Args.ToArray());
        }

        /// <summary>
        ///     Makes a clone such that any mutation to the source
        ///     or copy only affects that object and not the other.
        /// </summary>
        /// <param name="duplicator">Duplicator to clone child values.</param>
        /// <returns>Clone that is equal in value to the instance.</returns>
        public IDuplicatable DeepClone(IDuplicator duplicator)
        {
            if (duplicator == null) throw new ArgumentNullException(nameof(duplicator));

            return new MethodCallWrapper(duplicator.Copy(_method),
                duplicator.Copy(_names.Select(n => Tuple.Create(n, _args[n])).ToArray()));
        }
    }
}
