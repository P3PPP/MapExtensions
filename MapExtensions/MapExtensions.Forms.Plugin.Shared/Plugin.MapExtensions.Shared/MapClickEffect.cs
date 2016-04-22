using System;
using System.Collections.Generic;
using System.Text;

namespace MapExtensions.Forms.Plugin
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
