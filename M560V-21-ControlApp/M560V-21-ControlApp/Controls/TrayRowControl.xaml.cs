using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace M560V_21_ControlApp.Controls
{
    public partial class TrayRowControl : UserControl
    {
        // Geometry constants (unchanged)
        private const double PANEL_WIDTH_IN = 46.0;
        private const double PANEL_HEIGHT_IN = 12.0;
        private const double POCKET_DEPTH_IN = 10.1;
        private const double EDGE_MARGIN_IN = 1.0;
        private const double TOP_MARGIN_IN = 0.7;
        private const double BOTTOM_MARGIN_IN = 0.7;

        private const double SMALL_POCKET_W = 3.1;
        private const double SMALL_WEB_W = 1.0;
        private const double MEDIUM_POCKET_W = 6.1;
        private const double MEDIUM_WEB_W = 2.4;
        private const double LARGE_POCKET_W = 8.1;
        private const double LARGE_WEB_W = 2.53;

        private static readonly Brush AccentBlue = new SolidColorBrush(Color.FromRgb(0, 148, 255));
        private static readonly Brush TrayFaceBlue = new SolidColorBrush(Color.FromRgb(30, 45, 60));
        private static readonly Brush PocketFill = new SolidColorBrush(Color.FromRgb(28, 28, 30));
        private static readonly Brush UnselectedFill = new SolidColorBrush(Color.FromRgb(58, 58, 58));
        private static readonly Brush SelectedFill = Brushes.White;
        private static readonly Brush StockStroke = new SolidColorBrush(Color.FromRgb(160, 160, 160));

        private readonly List<Rectangle> _stockRects = new List<Rectangle>();
        private readonly List<bool> _selectedStates = new List<bool>();

        public TrayRowControl()
        {
            InitializeComponent();
            SizeChanged += (_, __) => Redraw();
            Loaded += (_, __) => Redraw();
        }

        // Dependency properties (unchanged)
        public static readonly DependencyProperty TrayTypeProperty =
            DependencyProperty.Register(nameof(TrayType), typeof(string), typeof(TrayRowControl),
                new FrameworkPropertyMetadata("Small", FrameworkPropertyMetadataOptions.AffectsRender, OnParamsChanged));

        public static readonly DependencyProperty SlotsProperty =
            DependencyProperty.Register(nameof(Slots), typeof(int), typeof(TrayRowControl),
                new FrameworkPropertyMetadata(10, FrameworkPropertyMetadataOptions.AffectsRender, OnParamsChanged));

        public static readonly DependencyProperty StockWidthXProperty =
            DependencyProperty.Register(nameof(StockWidthX), typeof(double), typeof(TrayRowControl),
                new FrameworkPropertyMetadata(3.0, FrameworkPropertyMetadataOptions.AffectsRender, OnParamsChanged));

        public static readonly DependencyProperty StockDepthYProperty =
            DependencyProperty.Register(nameof(StockDepthY), typeof(double), typeof(TrayRowControl),
                new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.AffectsRender, OnParamsChanged));

        public string TrayType
        {
            get => (string)GetValue(TrayTypeProperty);
            set => SetValue(TrayTypeProperty, value);
        }

        public int Slots
        {
            get => (int)GetValue(SlotsProperty);
            set => SetValue(SlotsProperty, value);
        }

        public double StockWidthX
        {
            get => (double)GetValue(StockWidthXProperty);
            set => SetValue(StockWidthXProperty, value);
        }

        public double StockDepthY
        {
            get => (double)GetValue(StockDepthYProperty);
            set => SetValue(StockDepthYProperty, value);
        }

        private static void OnParamsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TrayRowControl)d).Redraw();
        }

        private void Redraw()
        {
            if (!IsLoaded || CanvasRoot == null) return;

            CanvasRoot.Children.Clear();
            _stockRects.Clear();
            _selectedStates.Clear();

            if (Slots < 1) return;

            // same drawing math as before...
            double availW = Math.Max(ActualWidth - 16, 50);
            double availH = Math.Max(ActualHeight - 16, 50);

            double scaleX = availW / PANEL_WIDTH_IN;
            double scaleY = availH / PANEL_HEIGHT_IN;
            double scale = Math.Min(scaleX, scaleY);

            double panelPxW = PANEL_WIDTH_IN * scale;
            double panelPxH = PANEL_HEIGHT_IN * scale;
            double strokePx = Math.Max(0.6, 0.05 * scale);

            double offsetX = (CanvasRoot.ActualWidth - panelPxW) / 2.0;
            double offsetY = (CanvasRoot.ActualHeight - panelPxH) / 2.0;
            if (double.IsNaN(offsetX) || double.IsInfinity(offsetX)) offsetX = 0;
            if (double.IsNaN(offsetY) || double.IsInfinity(offsetY)) offsetY = 0;

            var frame = new Rectangle
            {
                Width = panelPxW,
                Height = panelPxH,
                Stroke = AccentBlue,
                StrokeThickness = strokePx,
                Fill = TrayFaceBlue
            };
            Canvas.SetLeft(frame, offsetX);
            Canvas.SetTop(frame, offsetY);
            CanvasRoot.Children.Add(frame);

            double innerLeftIn = EDGE_MARGIN_IN;
            double innerRightIn = PANEL_WIDTH_IN - EDGE_MARGIN_IN;
            double innerTopIn = TOP_MARGIN_IN;
            double pocketHeightIn = Math.Min(POCKET_DEPTH_IN, PANEL_HEIGHT_IN - TOP_MARGIN_IN - BOTTOM_MARGIN_IN);
            double innerWIn = innerRightIn - innerLeftIn;

            double pocketWIn, webWIn;
            switch ((TrayType ?? "Small").Trim().ToLowerInvariant())
            {
                case "medium": pocketWIn = MEDIUM_POCKET_W; webWIn = MEDIUM_WEB_W; break;
                case "large": pocketWIn = LARGE_POCKET_W; webWIn = LARGE_WEB_W; break;
                default: pocketWIn = SMALL_POCKET_W; webWIn = SMALL_WEB_W; break;
            }

            double bandWidthIn = Slots * pocketWIn + (Slots - 1) * webWIn;
            double leftStartIn = innerLeftIn + Math.Max(0, (innerWIn - bandWidthIn) / 2.0);

            for (int i = 0; i < Slots; i++)
            {
                double leftIn = leftStartIn + i * (pocketWIn + webWIn);
                double topIn = innerTopIn;

                double L = offsetX + leftIn * scale;
                double T = offsetY + topIn * scale;
                double W = pocketWIn * scale;
                double H = pocketHeightIn * scale;

                var pocket = new Rectangle
                {
                    Width = W,
                    Height = H,
                    Stroke = AccentBlue,
                    StrokeThickness = strokePx,
                    Fill = PocketFill
                };
                Canvas.SetLeft(pocket, L);
                Canvas.SetTop(pocket, T);
                CanvasRoot.Children.Add(pocket);

                double stockWIn = Math.Min(StockWidthX, pocketWIn);
                double stockHIn = Math.Min(StockDepthY, pocketHeightIn);
                double stockW = stockWIn * scale;
                double stockH = stockHIn * scale;

                double stockLeft = L + W - stockW;
                double stockTop = T;

                var stockRect = new Rectangle
                {
                    Width = stockW,
                    Height = stockH,
                    Stroke = StockStroke,
                    StrokeThickness = Math.Max(0.5, 0.0125 * scale),
                    Cursor = Cursors.Hand,
                    Tag = i,
                    Fill = SelectedFill
                };

                _selectedStates.Add(true);
                stockRect.MouseLeftButtonUp += StockRect_MouseLeftButtonUp;

                Canvas.SetLeft(stockRect, stockLeft);
                Canvas.SetTop(stockRect, stockTop);
                CanvasRoot.Children.Add(stockRect);
                _stockRects.Add(stockRect);
            }
        }

        private void StockRect_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Rectangle r && r.Tag is int idx && idx >= 0 && idx < _stockRects.Count)
            {
                bool isSelected = !_selectedStates[idx];
                _selectedStates[idx] = isSelected;
                r.Fill = isSelected ? SelectedFill : UnselectedFill;
                OnSelectionChanged?.Invoke(this, idx, isSelected);
            }
        }

        public bool[] GetSelectionMask() => _selectedStates.ToArray();

        public void SelectAll(bool selected)
        {
            for (int i = 0; i < _stockRects.Count; i++)
            {
                _selectedStates[i] = selected;
                _stockRects[i].Fill = selected ? SelectedFill : UnselectedFill;
            }
        }

        public event Action<TrayRowControl, int, bool> OnSelectionChanged;
    }
}
