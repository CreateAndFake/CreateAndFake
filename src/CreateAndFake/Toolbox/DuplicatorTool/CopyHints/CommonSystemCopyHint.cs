﻿using System;
using System.Reflection;
using CreateAndFake.Design;

namespace CreateAndFake.Toolbox.DuplicatorTool.CopyHints;

/// <summary>Handles copying common types for the duplicator.</summary>
public sealed class CommonSystemCopyHint : CopyHint
{
    /// <inheritdoc/>
    protected internal sealed override (bool, object) TryCopy(object source, DuplicatorChainer duplicator)
    {
        ArgumentGuard.ThrowIfNull(duplicator, nameof(duplicator));

        if (source == null)
        {
            return (true, null);
        }
        else if (source is TimeSpan span)
        {
            return (true, new TimeSpan(duplicator.Copy(span.Ticks)));
        }
        else if (source is Uri link)
        {
            return (true, new Uri(link.OriginalString));
        }
        else if (source is WeakReference reference)
        {
            return (true, new WeakReference(reference.Target, reference.TrackResurrection));
        }
        else if (source is MemberInfo member)
        {
            return (true, member);
        }
        else
        {
            return (false, null);
        }
    }
}
