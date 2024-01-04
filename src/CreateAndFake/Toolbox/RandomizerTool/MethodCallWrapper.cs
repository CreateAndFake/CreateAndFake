using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CreateAndFake.Design;
using CreateAndFake.Toolbox.DuplicatorTool;

namespace CreateAndFake.Toolbox.RandomizerTool;

/// <summary>Holds parameter data for a method.</summary>
public sealed class MethodCallWrapper : IDuplicatable
{
    /// <summary>Associated method.</summary>
    private readonly MethodBase _method;

    /// <summary>Ordered parameter names.</summary>
    private readonly IEnumerable<string> _names;

    /// <summary>Parameter values.</summary>
    private readonly Dictionary<string, object> _args;

    /// <summary>Parameter data for the method.</summary>
    public IEnumerable<object> Args => _names.Select(n => _args[n]).ToArray();

    /// <summary>Initializes a new instance of the <see cref="MethodCallWrapper"/> class.</summary>
    /// <param name="method">Associated method.</param>
    /// <param name="args">Parameter data for the method.</param>
    public MethodCallWrapper(MethodBase method, IEnumerable<Tuple<string, object>> args)
    {
        ArgumentGuard.ThrowIfNull(args, nameof(args));

        _method = method ?? throw new ArgumentNullException(nameof(method));
        _args = [];

        List<string> names = [];
        foreach (Tuple<string, object> arg in args)
        {
            names.Add(arg.Item1);
            _args.Add(arg.Item1, arg.Item2);
        }
        _names = names;
    }

    /// <summary>Sets parameter named <paramref name="name"/> to <paramref name="value"/>.</summary>
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

    /// <summary>Invokes the method on <paramref name="instance"/>.</summary>
    /// <param name="instance">Instance to call the method with the data on.</param>
    /// <returns>Results from the call.</returns>
    public object InvokeOn(object instance)
    {
        return _method.Invoke(instance, Args.ToArray());
    }

    /// <inheritdoc/>
    public IDuplicatable DeepClone(IDuplicator duplicator)
    {
        ArgumentGuard.ThrowIfNull(duplicator, nameof(duplicator));

        return new MethodCallWrapper(duplicator.Copy(_method),
            duplicator.Copy(_names.Select(n => Tuple.Create(n, _args[n])).ToArray()));
    }
}
