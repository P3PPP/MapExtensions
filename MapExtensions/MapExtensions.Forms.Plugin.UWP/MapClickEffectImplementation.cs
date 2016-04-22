using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls.Maps;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Platform.UWP;

namespace Plugin.MapExtensions
{
	partial class MapClickEffect : PlatformEffect, IMapClickExtension
	{
		public event EventHandler<MapClickedEventArgs> MapClicked;
		public event EventHandler<MapClickedEventArgs> MapLongClicked;

		private MapControl mapControl;
		private ClickBehavior behavior;

		#region implemented abstract members of Effect

		protected override void OnAttached()
		{
			mapControl = Control as MapControl;
			if (mapControl == null)
			{
				throw new NotSupportedException(Control.GetType() + " is not supported.");
			}

			behavior = (Element as Map)?.Behaviors?.OfType<ClickBehavior>()?.FirstOrDefault();
			if (behavior == null)
				return;

			mapControl.MapTapped += MapControl_MapTapped;
			mapControl.MapHolding += MapControl_MapHolding;
		}

		private void MapControl_MapTapped(MapControl sender, MapInputEventArgs args)
		{
			MapClicked?.Invoke(this, new MapClickedEventArgs(
				new Position(args.Location.Position.Latitude,
					args.Location.Position.Longitude)
				));
		}

		private void MapControl_MapHolding(MapControl sender, MapInputEventArgs args)
		{
			MapLongClicked?.Invoke(this, new MapClickedEventArgs(
				new Position(args.Location.Position.Latitude,
					args.Location.Position.Longitude)
				));
		}

		protected override void OnDetached()
		{
			mapControl.MapTapped -= MapControl_MapTapped;
			mapControl.MapTapped -= MapControl_MapHolding;
			mapControl = null;
		}

		#endregion

	}
}
