using System;
using System.Linq;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Platform.iOS;
using MapKit;
using UIKit;

namespace MapExtensions.Forms.Plugin
{
	partial class MapClickEffect : PlatformEffect, IMapClickExtension
	{
		public event EventHandler<MapClickedEventArgs> MapClicked;
		public event EventHandler<MapClickedEventArgs> MapLongClicked;

		private MKMapView mapView;
		private ClickBehavior behavior;
		private UITapGestureRecognizer tapGesture;
		private UILongPressGestureRecognizer longPressGesture;

		#region implemented abstract members of Effect

		protected override void OnAttached()
		{
			mapView = Control as MKMapView;
			if (mapView == null)
			{
				throw new NotSupportedException(Control.GetType() + " is not supported.");
			}

			behavior = (Element as Map)?.Behaviors?.OfType<ClickBehavior>()?.FirstOrDefault();
			if (behavior == null)
				return;

			tapGesture = new UITapGestureRecognizer(recognizer =>
				MapClicked?.Invoke(this, new MapClickedEventArgs(
					ConvertLocationToPosition(recognizer, mapView)))
			)
			{
				NumberOfTapsRequired = 1,
			};

			longPressGesture = new UILongPressGestureRecognizer(recognizer =>
			{
				if (recognizer.State != UIGestureRecognizerState.Began)
					return;

				MapLongClicked?.Invoke(this, new MapClickedEventArgs(
					ConvertLocationToPosition(recognizer, mapView)));
			});

			tapGesture.RequireGestureRecognizerToFail(longPressGesture);
			mapView.AddGestureRecognizer(longPressGesture);
			mapView.AddGestureRecognizer(tapGesture);
		}

		protected override void OnDetached()
		{
			mapView.RemoveGestureRecognizer(tapGesture);
			tapGesture.Dispose();
			tapGesture = null;
			longPressGesture.Dispose();
			longPressGesture = null;
			mapView = null;
			behavior = null;
		}

		#endregion

		private static Position ConvertLocationToPosition(UIGestureRecognizer recognizer, MKMapView map)
		{
			var point = recognizer.LocationInView(map);
			var coordinate = map.ConvertPoint(point, map);
			return new Position(coordinate.Latitude, coordinate.Longitude);
		}
	}
}


