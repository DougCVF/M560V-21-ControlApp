using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace M560V_21_ControlApp.Controls
{
    public partial class CellOverviewControl : UserControl
    {
        public CellOverviewControl()
        {
            InitializeComponent();
            Loaded += (_, __) => UpdateImage();
        }

        public static readonly DependencyProperty ActiveRackProperty =
            DependencyProperty.Register(nameof(ActiveRack), typeof(int), typeof(CellOverviewControl),
                new PropertyMetadata(1, OnActiveRackChanged));

        public int ActiveRack
        {
            get => (int)GetValue(ActiveRackProperty);
            set => SetValue(ActiveRackProperty, value);
        }

        private static void OnActiveRackChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CellOverviewControl)d).UpdateImage();
        }

        private void UpdateImage()
        {
            if (!IsLoaded) return;

            try
            {
                // Choose image based on rack number
                string imageName = $"pack://application:,,,/Assets/Rack{ActiveRack}.png";
                var bitmap = new BitmapImage(new Uri(imageName, UriKind.Absolute));

                // Fade-out / fade-in transition
                var fadeOut = new DoubleAnimation(1.0, 0.0, TimeSpan.FromMilliseconds(200));
                var fadeIn = new DoubleAnimation(0.0, 1.0, TimeSpan.FromMilliseconds(250));

                fadeOut.Completed += (s, _) =>
                {
                    RackImage.Source = bitmap;
                    RackImage.BeginAnimation(OpacityProperty, fadeIn);
                };

                RackImage.BeginAnimation(OpacityProperty, fadeOut);
            }
            catch
            {
                // Fallback if image not found
                RackImage.Source = null;
            }
        }
    }
}
