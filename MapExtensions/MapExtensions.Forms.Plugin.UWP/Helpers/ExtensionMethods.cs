using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Xamarin.Forms.Maps;

namespace MapExtensions.Forms.Plugin.UWP
{
	internal static class ExtensionMethods
	{
		internal static BasicGeoposition ToBasicGeoposition(this Position position)
		{
			return new BasicGeoposition { Latitude = position.Latitude, Longitude = position.Longitude };
		}
	}
}
