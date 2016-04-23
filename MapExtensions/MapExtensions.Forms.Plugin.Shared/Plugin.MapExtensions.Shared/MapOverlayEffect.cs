using System;
using System.Collections.Generic;
using System.Text;

namespace MapExtensions.Forms.Plugin
{
	partial class MapOverlayEffect
	{
		internal static MapOverlayEffect Create()
		{
#if PORTABLE
			return null;
#else
			return new MapOverlayEffect();
#endif
		}
	}
}
