using Plugin.MapExtensions.Abstractions;
using System;

namespace Plugin.MapExtensions
{
  /// <summary>
  /// Cross platform MapExtensions implemenations
  /// </summary>
  public class CrossMapExtensions
  {
    static Lazy<IMapExtensions> Implementation = new Lazy<IMapExtensions>(() => CreateMapExtensions(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

    /// <summary>
    /// Current settings to use
    /// </summary>
    public static IMapExtensions Current
    {
      get
      {
        var ret = Implementation.Value;
        if (ret == null)
        {
          throw NotImplementedInReferenceAssembly();
        }
        return ret;
      }
    }

    static IMapExtensions CreateMapExtensions()
    {
#if PORTABLE
        return null;
#else
        return new MapExtensionsImplementation();
#endif
    }

    internal static Exception NotImplementedInReferenceAssembly()
    {
      return new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
    }
  }
}
