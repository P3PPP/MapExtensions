using System;
using System.Linq;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Platform.iOS;
using MapKit;
using UIKit;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using CoreLocation;

namespace MapExtensions.Forms.Plugin
{
	partial class MapOverlayEffect : PlatformEffect, IRouteExtension
	{
		private MKMapView mapControl;
		private OverlayBehavior behavior;
		private Dictionary<Route, MKPolyline> renderedRoute = new Dictionary<Route, MKPolyline>();

		private ObservableCollection<Route> routes;
		public ObservableCollection<Route> Routes
		{
			get
			{
				return routes;
			}
			set
			{
				var oldValue = routes;
				routes = value;
				OnRoutesChanged(oldValue, value);
			}
		}

		#region implemented abstract members of Effect

		protected override void OnAttached()
		{
			mapControl = Control as MKMapView;
			if (mapControl == null)
			{
				throw new NotSupportedException(Control.GetType() + " is not supported.");
			}

			behavior = (Element as Map)?.Behaviors?.OfType<OverlayBehavior>()?.FirstOrDefault();
			if (behavior == null)
				return;

			mapControl.OverlayRenderer = GetOverlayRenderer;

			AddAllRouteView();
		}

		protected override void OnDetached()
		{
			RemoveAllRouteView();
			mapControl = null;
		}

		#endregion

		private MKPolylineRenderer GetOverlayRenderer(MKMapView mapView, IMKOverlay overlay)
		{
			if(overlay is MKPolyline)
			{
				return new MKPolylineRenderer((MKPolyline)overlay)
				{
					StrokeColor = UIColor.Yellow,
				};
			}

			return null;
		}


		private async void AddRouteView(Route route)
		{
			if (mapControl == null)
				return;

			var startPlace = new MKPlacemark(
				new CLLocationCoordinate2D(
					route.StartLocation.Latitude,
					route.StartLocation.Longitude),
				(MKPlacemarkAddress)null);

			var endPlace = new MKPlacemark(
				new CLLocationCoordinate2D(
					route.EndLocation.Latitude,
					route.EndLocation.Longitude),
				(MKPlacemarkAddress)null);

			var request = new MKDirectionsRequest
			{
				Source = new MKMapItem(startPlace),
				Destination = new MKMapItem(endPlace),
				RequestsAlternateRoutes = false,
			};

			var directions = new MKDirections(request);
			directions.CalculateDirections((response, error) =>
			{
				if(error == null)
				{
					foreach (var item in response.Routes)
					{
						mapControl.AddOverlay(item.Polyline);
						renderedRoute.Add(route, item.Polyline);
					}
				}
			});
		}

		private void RemoveRouteView(Route route)
		{
			MKPolyline polyline;
			if (renderedRoute.TryGetValue(route, out polyline))
			{
				mapControl?.RemoveOverlay(polyline);
			}
			renderedRoute.Remove(route);
		}

		private void AddAllRouteView()
		{
			foreach (var key in renderedRoute.Keys.ToList())
			{
				AddRouteView(key);
			}
		}

		private void RemoveAllRouteView()
		{
			foreach (var key in renderedRoute.Keys.ToList())
			{
				RemoveRouteView(key);
			}
		}

		private void OnRoutesChanged(ObservableCollection<Route> oldValue, ObservableCollection<Route> newValue)
		{
			if(oldValue != null)
			{
				oldValue.CollectionChanged -= OnRouteCollectionChanged;
				RemoveAllRouteView();
			}

			if(newValue != null)
			{
				newValue.CollectionChanged += OnRouteCollectionChanged;
				AddAllRouteView();
			}
		}

		private void OnRouteCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
					foreach (var item in e.NewItems)
					{
						AddRouteView((Route)item);
					}
					break;
				case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
					break;
				case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
					foreach (var item in e.OldItems)
					{
						RemoveRouteView((Route)item);
					}
					break;
				case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
					break;
				case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
					break;
				default:
					break;
			}
		}
	}
}
