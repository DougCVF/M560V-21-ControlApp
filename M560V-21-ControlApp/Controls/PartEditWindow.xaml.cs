using System;
using System.Globalization;
using System.Windows;
using M560V_21_ControlApp.Data; // adjust namespace if your Part class lives elsewhere
using M560V_21_ControlApp.Models;

namespace M560V_21_ControlApp.Controls
{
    public partial class PartEditWindow : Window
    {
        public Part CurrentPart { get; private set; }

        public PartEditWindow()
        {
            InitializeComponent();
            CurrentPart = new Part();
        }

        public PartEditWindow(Part existing) : this()
        {
            if (existing == null) return;
            CurrentPart = new Part
            {
                Id = existing.Id
            };
            // preload UI from existing
            txtPartNumber.Text = existing.PartNumber;
            txtDescription.Text = existing.Description;
            txtStockWidth.Text = existing.StockWidth.ToString(CultureInfo.InvariantCulture);
            txtStockDepth.Text = existing.StockDepth.ToString(CultureInfo.InvariantCulture);
            txtStockHeight.Text = existing.StockHeight.ToString(CultureInfo.InvariantCulture);

            txtOp10PickXOffset.Text = existing.Op10PickXOffset.ToString(CultureInfo.InvariantCulture);
            txtOp10PickYOffset.Text = existing.Op10PickYOffset.ToString(CultureInfo.InvariantCulture);
            txtOp10PickZOffset.Text = existing.Op10PickZOffset.ToString(CultureInfo.InvariantCulture);
            txtOp10CycleTime.Text = existing.Op10CycleTime.ToString(CultureInfo.InvariantCulture);

            txtOp20PickXOffset.Text = existing.Op20PickXOffset.ToString(CultureInfo.InvariantCulture);
            txtOp20PickYOffset.Text = existing.Op20PickYOffset.ToString(CultureInfo.InvariantCulture);
            txtOp20PickZOffset.Text = existing.Op20PickZOffset.ToString(CultureInfo.InvariantCulture);

            txtOp20FinXOffset.Text = existing.Op20FinXOffset.ToString(CultureInfo.InvariantCulture);
            txtOp20FinYOffset.Text = existing.Op20FinYOffset.ToString(CultureInfo.InvariantCulture);
            txtOp20FinZOffset.Text = existing.Op20FinZOffset.ToString(CultureInfo.InvariantCulture);
            txtOp20CycleTime.Text = existing.Op20CycleTime.ToString(CultureInfo.InvariantCulture);

            txtOp10VisePSI.Text = existing.Op10VisePSI.ToString(CultureInfo.InvariantCulture);
            txtOp20VisePSI.Text = existing.Op20VisePSI.ToString(CultureInfo.InvariantCulture);

            txtOp10ProgramName.Text = existing.Op10ProgramName;
            txtOp20ProgramName.Text = existing.Op20ProgramName;
        }

        private static double ParseOrZero(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0.0;
            double d;
            if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out d))
                return d;
            if (double.TryParse(s, NumberStyles.Float, CultureInfo.CurrentCulture, out d))
                return d;
            return 0.0;
        }

        private bool CollectFromUIIntoPart()
        {
            if (string.IsNullOrWhiteSpace(txtPartNumber.Text))
            {
                MessageBox.Show(this, "Part Number is required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPartNumber.Focus();
                return false;
            }

            CurrentPart.PartNumber = txtPartNumber.Text.Trim();
            CurrentPart.Description = txtDescription.Text.Trim();

            CurrentPart.StockWidth = ParseOrZero(txtStockWidth.Text);
            CurrentPart.StockDepth = ParseOrZero(txtStockDepth.Text);
            CurrentPart.StockHeight = ParseOrZero(txtStockHeight.Text);

            CurrentPart.Op10PickXOffset = ParseOrZero(txtOp10PickXOffset.Text);
            CurrentPart.Op10PickYOffset = ParseOrZero(txtOp10PickYOffset.Text);
            CurrentPart.Op10PickZOffset = ParseOrZero(txtOp10PickZOffset.Text);
            CurrentPart.Op10CycleTime = ParseOrZero(txtOp10CycleTime.Text);

            CurrentPart.Op20PickXOffset = ParseOrZero(txtOp20PickXOffset.Text);
            CurrentPart.Op20PickYOffset = ParseOrZero(txtOp20PickYOffset.Text);
            CurrentPart.Op20PickZOffset = ParseOrZero(txtOp20PickZOffset.Text);

            CurrentPart.Op20FinXOffset = ParseOrZero(txtOp20FinXOffset.Text);
            CurrentPart.Op20FinYOffset = ParseOrZero(txtOp20FinYOffset.Text);
            CurrentPart.Op20FinZOffset = ParseOrZero(txtOp20FinZOffset.Text);
            CurrentPart.Op20CycleTime = ParseOrZero(txtOp20CycleTime.Text);

            CurrentPart.Op10VisePSI = ParseOrZero(txtOp10VisePSI.Text);
            CurrentPart.Op20VisePSI = ParseOrZero(txtOp20VisePSI.Text);

            CurrentPart.Op10ProgramName = (txtOp10ProgramName.Text ?? "").Trim();
            CurrentPart.Op20ProgramName = (txtOp20ProgramName.Text ?? "").Trim();

            return true;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!CollectFromUIIntoPart()) return;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
