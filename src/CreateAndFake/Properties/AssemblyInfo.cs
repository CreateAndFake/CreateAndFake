using System;
using System.Runtime.CompilerServices;

[assembly: CLSCompliant(true)]
[assembly: InternalsVisibleTo("FakerTypes")]
[assembly: InternalsVisibleTo("CreateAndFakeTests")]

#if NEEDINIT // Init type only shipped with .NET 5.0+
namespace System.Runtime.CompilerServices;

using System.ComponentModel;

/// <summary>Tracks metadata.</summary>
[EditorBrowsable(EditorBrowsableState.Never)]
internal sealed class IsExternalInit { }
#endif
