using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace MapExtensions.Forms.Plugin
{
    public class OverlayBehavior : Behavior<Map>
	{
		private Effect effect;
		private IRouteExtension routeExtension;

		protected override void OnAttachedTo(Map bindable)
		{
			base.OnAttachedTo(bindable);

			if (bindable as Map == null)
			{
				throw new NotSupportedException(bindable.GetType() + " is not supported.");
			}

			effect = MapOverlayEffect.Create();
			if (effect == null)
			{
				throw new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
			}

			routeExtension = effect as IRouteExtension;
			if(routeExtension != null)
			{
				routeExtension.Routes = this.Routes;
			}

			bindable.Effects.Add(effect);
		}

		protected override void OnDetachingFrom(Map bindable)
		{
			bindable?.Effects?.Remove(effect);
			effect = null;

			base.OnDetachingFrom(bindable);
		}


		#region Routes BindableProperty
		internal static readonly BindablePropertyKey RoutesPropertyKey =
			BindableProperty.CreateReadOnly(nameof(Routes), typeof(ObservableCollection<Route>), typeof(OverlayBehavior), null,
				defaultValueCreator: x => new ObservableCollection<Route>());

		public static readonly BindableProperty RoutesProperty = RoutesPropertyKey.BindableProperty;

		public ObservableCollection<Route> Routes
		{
			get { return (ObservableCollection<Route>)GetValue(RoutesProperty); }
			internal set { SetValue(RoutesPropertyKey, value); }
		}
		#endregion

	}
	public class Route
	{
		public Position StartLocation { get; private set; }
		public Position EndLocation { get; private set; }

		public Route(Position startLocation, Position endLocation)
		{
			StartLocation = startLocation;
			EndLocation = endLocation;
		}
	}

	internal interface IRouteExtension
	{
		ObservableCollection<Route> Routes { get; set; }
	}
}
