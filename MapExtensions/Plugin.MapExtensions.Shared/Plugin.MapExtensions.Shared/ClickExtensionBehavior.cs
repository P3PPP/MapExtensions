using System;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using System.Windows.Input;

namespace Plugin.MapExtensions
{
	public class ClickBehavior : Behavior<Map>
	{
		public event EventHandler<MapClickedEventArgs> MapClicked;
		public event EventHandler<MapClickedEventArgs> MapLongClicked;

		private Effect effect;
		private IMapClickExtension mapClickExtension;

		protected override void OnAttachedTo(Map bindable)
		{
			base.OnAttachedTo(bindable);

			if (bindable as Map == null)
			{
				throw new NotSupportedException(bindable.GetType() + " is not supported.");
			}

			effect = MapClickEffect.Create();
			if (effect == null)
			{
				throw new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
			}

			// PlatformEffectから地図のタップイベントを貰う
			mapClickExtension = effect as IMapClickExtension;
			mapClickExtension.MapClicked += OnMapClicked;
			mapClickExtension.MapLongClicked += OnMapLongClicked;

			bindable.Effects.Add(effect);
		}

		protected override void OnDetachingFrom(Map bindable)
		{
			bindable?.Effects?.Remove(effect);
			effect = null;
			mapClickExtension.MapClicked -= OnMapClicked;
			mapClickExtension.MapLongClicked -= OnMapLongClicked;
			mapClickExtension = null;

			base.OnDetachingFrom(bindable);
		}

		private void OnMapClicked(object sender, MapClickedEventArgs e)
		{
			MapClicked?.Invoke(this, new MapClickedEventArgs(e.Position));
			MapClidkedCommand?.ExecuteIfPossible(new MapClickedEventArgs(e.Position));
		}

		private void OnMapLongClicked(object sender, MapClickedEventArgs e)
		{
			MapLongClicked?.Invoke(this, new MapClickedEventArgs(e.Position));
			MapLongClidkedCommand?.ExecuteIfPossible(new MapClickedEventArgs(e.Position));
		}

		#region BindableProperties

		public static readonly BindableProperty MapClidkedCommandProperty =
			BindableProperty.Create(nameof(MapClidkedCommand),
				typeof(ICommand),
				typeof(ClickBehavior),
				null);

		public ICommand MapClidkedCommand
		{
			get { return (ICommand)GetValue(MapClidkedCommandProperty); }
			set { SetValue(MapClidkedCommandProperty, value); }
		}

		public static readonly BindableProperty MapLongClidkedCommandProperty =
			BindableProperty.Create(nameof(MapLongClidkedCommand),
				typeof(ICommand),
				typeof(ClickBehavior),
				null);

		public ICommand MapLongClidkedCommand
		{
			get { return (ICommand)GetValue(MapLongClidkedCommandProperty); }
			set { SetValue(MapLongClidkedCommandProperty, value); }
		}

		#endregion
	}

	// ネイティブのEffect実装からイベントを上げてもらうためのインターフェース
	internal interface IMapClickExtension
	{
		event EventHandler<MapClickedEventArgs> MapClicked;
		event EventHandler<MapClickedEventArgs> MapLongClicked;
	}

	// タップ座標を受け取るためのEventArgs
	public sealed class MapClickedEventArgs : EventArgs
	{
		public Position Position
		{
			get;
		}

		public MapClickedEventArgs(Position position)
		{
			Position = position;
		}
	}
}