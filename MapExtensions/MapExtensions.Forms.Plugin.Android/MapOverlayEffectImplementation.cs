using System;
using System.Linq;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Platform.Android;
using Android.Gms.Maps;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Android.Gms.Maps.Model;

namespace MapExtensions.Forms.Plugin
{
	partial class MapOverlayEffect : PlatformEffect, IRouteExtension
	{
		private MapView mapControl;
		private OverlayBehavior behavior;
		private Dictionary<Route, Polyline> renderedRoute = new Dictionary<Route, Polyline>();

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
			mapControl = Control as MapView;
			if (mapControl == null)
			{
				throw new NotSupportedException(Control.GetType() + " is not supported.");
			}

			behavior = (Element as Map)?.Behaviors?.OfType<OverlayBehavior>()?.FirstOrDefault();
			if (behavior == null)
				return;

			AddAllRouteView();
		}

		protected override void OnDetached()
		{
			RemoveAllRouteView();
			mapControl = null;
		}

		#endregion


		private async void AddRouteView(Route route)
		{
			//if(mapControl == null)
			//	return;

			//var startLocation = route.StartLocation.ToBasicGeoposition();
			//var endLocation = route.EndLocation.ToBasicGeoposition();

			//MapRouteFinderResult routeResult =
			//	  await MapRouteFinder.GetWalkingRouteAsync(
			//	  new Geopoint(startLocation),
			//	  new Geopoint(endLocation));

			//if (routeResult.Status == MapRouteFinderStatus.Success)
			//{
			//	var routeView = new MapRouteView(routeResult.Route)
			//	{
			//		RouteColor = Windows.UI.Colors.Yellow,
			//		OutlineColor = Windows.UI.Colors.Black,
			//	};

			//	mapControl.Routes.Add(routeView);

			//	renderedRoute.Add(route, routeView);
			//}
		}

		private void RemoveRouteView(Route route)
		{
			//MapRouteView routeView;
			//if(renderedRoute.TryGetValue(route, out routeView))
			//{
			//	mapControl?.Routes.Remove(routeView);
			//}
			//renderedRoute.Remove(route);
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
