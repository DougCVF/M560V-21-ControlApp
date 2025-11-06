using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using M560V_21_ControlApp.Data;
using M560V_21_ControlApp.Models;

namespace M560V_21_ControlApp.Controls
{
    public partial class RackRowControl : UserControl
    {
        public RackRowControl()
        {
            InitializeComponent();
            Loaded += RackRowControl_Loaded;
        }

        private void RackRowControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var parts = Repository.GetAllParts();
                cmbPartSelect.ItemsSource = new ObservableCollection<Part>(parts);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Part load failed: " + ex.Message);
            }
        }

        private void cmbPartSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = cmbPartSelect.SelectedItem as Part;
            if (selected == null) return;

            // Pull stock dimensions
            double width = GetDouble(selected, "StockWidth", "StockWidthX");
            double depth = GetDouble(selected, "StockDepth", "StockDepthY");
            double height = GetDouble(selected, "StockHeight", "StockHeightZ");

            // Update dependency properties for tray geometry
            if (width > 0) StockWidthX = width;
            if (depth > 0) StockDepthY = depth;

            // Determine tray type by width
            if (StockWidthX <= 3.2)
            {
                TrayType = "Small";
                Slots = 10;
            }
            else if (StockWidthX <= 6.2)
            {
                TrayType = "Medium";
                Slots = 5;
            }
            else
            {
                TrayType = "Large";
                Slots = 4;
            }

            // Apply changes to tray control
            TrayRowControl.TrayType = TrayType;
            TrayRowControl.Slots = Slots;
            TrayRowControl.StockWidthX = StockWidthX;
            TrayRowControl.StockDepthY = StockDepthY;
            TrayRowControl.UpdateLayout();
            TrayRowControl.InvalidateVisual();

            // Update the label text for user visibility
            lblStockInfo.Text = $"Stock = {width:0.###}\" x {depth:0.###}\" x {height:0.###}\"";

            Console.WriteLine($"[{Title}] Updated Tray → {TrayType}, {Slots} slots (W={width}, D={depth}, H={height})");
        }

        private static double GetDouble(object obj, params string[] propertyNames)
        {
            foreach (string name in propertyNames)
            {
                PropertyInfo prop = obj.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
                if (prop != null)
                {
                    try
                    {
                        var val = prop.GetValue(obj, null);
                        if (val != null) return Convert.ToDouble(val);
                    }
                    catch { }
                }
            }
            return 0;
        }

        private void SelectAll_Click(object sender, RoutedEventArgs e) => TrayRowControl.SelectAll(true);
        private void SelectNone_Click(object sender, RoutedEventArgs e) => TrayRowControl.SelectAll(false);

        // dependency properties
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(RackRowControl), new PropertyMetadata("Row"));

        public static readonly DependencyProperty TrayTypeProperty =
            DependencyProperty.Register("TrayType", typeof(string), typeof(RackRowControl), new PropertyMetadata("Small"));

        public static readonly DependencyProperty SlotsProperty =
            DependencyProperty.Register("Slots", typeof(int), typeof(RackRowControl), new PropertyMetadata(10));

        public static readonly DependencyProperty StockWidthXProperty =
            DependencyProperty.Register("StockWidthX", typeof(double), typeof(RackRowControl), new PropertyMetadata(3.0));

        public static readonly DependencyProperty StockDepthYProperty =
            DependencyProperty.Register("StockDepthY", typeof(double), typeof(RackRowControl), new PropertyMetadata(10.0));

        public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
        public string TrayType { get => (string)GetValue(TrayTypeProperty); set => SetValue(TrayTypeProperty, value); }
        public int Slots { get => (int)GetValue(SlotsProperty); set => SetValue(SlotsProperty, value); }
        public double StockWidthX { get => (double)GetValue(StockWidthXProperty); set => SetValue(StockWidthXProperty, value); }
        public double StockDepthY { get => (double)GetValue(StockDepthYProperty); set => SetValue(StockDepthYProperty, value); }
    }
}
