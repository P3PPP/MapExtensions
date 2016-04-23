using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.UI.Xaml.Controls.Maps;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Platform.UWP;
using MapExtensions.Forms.Plugin.UWP;

namespace MapExtensions.Forms.Plugin
{
	partial class MapOverlayEffect : PlatformEffect, IRouteExtension
	{
		private MapControl mapControl;
		private OverlayBehavior behavior;
		private Dictionary<Route, MapRouteView> renderedRoute = new Dictionary<Route, MapRouteView>();

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
			mapControl = Control as MapControl;
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
			if(mapControl == null)
				return;

			var startLocation = route.StartLocation.ToBasicGeoposition();
			var endLocation = route.EndLocation.ToBasicGeoposition();

			MapRouteFinderResult routeResult =
				  await MapRouteFinder.GetWalkingRouteAsync(
				  new Geopoint(startLocation),
				  new Geopoint(endLocation));

			if (routeResult.Status == MapRouteFinderStatus.Success)
			{
				var routeView = new MapRouteView(routeResult.Route)
				{
					RouteColor = Windows.UI.Colors.Yellow,
					OutlineColor = Windows.UI.Colors.Black,
				};

				mapControl.Routes.Add(routeView);

				renderedRoute.Add(route, routeView);
			}
		}

		private void RemoveRouteView(Route route)
		{
			MapRouteView routeView;
			if(renderedRoute.TryGetValue(route, out routeView))
			{
				mapControl?.Routes.Remove(routeView);
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
