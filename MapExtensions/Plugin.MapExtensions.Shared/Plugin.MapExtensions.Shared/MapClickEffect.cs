using System;
using System.Collections.Generic;
using System.Text;

namespace Plugin.MapExtensions
{
	partial class MapClickEffect
	{
		internal static MapClickEffect Create()
		{
#if PORTABLE
			return null;
#else
			return new MapClickEffect();
#endif
		}
	}
}
