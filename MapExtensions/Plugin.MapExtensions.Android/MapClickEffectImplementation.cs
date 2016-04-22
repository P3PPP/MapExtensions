using System;
using System.Linq;
using Xamarin.Forms.Platform.Android;
using Android.Gms.Maps;
using Xamarin.Forms.Maps;

namespace Plugin.MapExtensions
{
	partial class MapClickEffect : PlatformEffect, IMapClickExtension
	{
		public event EventHandler<MapClickedEventArgs> MapClicked;
		public event EventHandler<MapClickedEventArgs> MapLongClicked;

		private GoogleMap googleMap;
		private ClickBehavior behavior;

		#region implemented abstract members of Effect

		protected override void OnAttached()
		{
			var mapView = Control as MapView;
			if (mapView == null)
			{
				throw new NotSupportedException(Control.GetType() + " is not supported.");
			}

			behavior = (Element as Map)?.Behaviors?.OfType<ClickBehavior>()?.FirstOrDefault();
			if (behavior == null)
				return;

			// GoogleMapインスタンスを得る
			var callback = new OnMapReadyCallback();
			callback.OnMapReadyAction = (gMap) => {
				googleMap = gMap;
				googleMap.MapClick += GoogleMap_MapClick;
				googleMap.MapLongClick += GoogleMap_MapLongClick;
			};
			mapView.GetMapAsync(callback);
		}

		protected override void OnDetached()
		{
			if (googleMap != null)
			{
				googleMap.MapClick -= GoogleMap_MapClick;
				googleMap.MapLongClick -= GoogleMap_MapLongClick;
				googleMap = null;
			}
		}

		#endregion

		private void GoogleMap_MapClick(object sender, GoogleMap.MapClickEventArgs e)
		{
			MapClicked?.Invoke(this, new MapClickedEventArgs(
				new Position(e.Point.Latitude,
					e.Point.Longitude)));
		}

		private void GoogleMap_MapLongClick(object sender, GoogleMap.MapLongClickEventArgs e)
		{
			MapLongClicked?.Invoke(this, new MapClickedEventArgs(
				new Position(e.Point.Latitude,
					e.Point.Longitude)));
		}

		private class OnMapReadyCallback : Java.Lang.Object, IOnMapReadyCallback
		{
			public Action<GoogleMap> OnMapReadyAction;

			public void OnMapReady(GoogleMap googleMap)
			{
				OnMapReadyAction?.Invoke(googleMap);
			}
		}
	}
}