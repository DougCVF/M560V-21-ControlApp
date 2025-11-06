using System;
using System.Windows;
using M560V_21_ControlApp.Models;
using M560V_21_ControlApp.Data;

namespace M560V_21_ControlApp.Controls
{
    public partial class PartEditWindow : Window
    {
        private readonly Part _part;
        private readonly bool _isEdit;

        public PartEditWindow(Part part = null)
        {
            InitializeComponent();

            _isEdit = (part != null);
            _part = part ?? new Part();

            if (_isEdit)
            {
                // Header/title can stay the same; just populate fields
                txtPartNumber.Text = _part.PartNumber;
                txtDescription.Text = _part.Description;

                txtWidth.Text = _part.StockWidth.ToString();
                txtDepth.Text = _part.StockDepth.ToString();
                txtHeight.Text = _part.StockHeight.ToString();

                txtOp10X.Text = _part.Op10PickXOffset.ToString();
                txtOp10Y.Text = _part.Op10PickYOffset.ToString();
                txtOp10Z.Text = _part.Op10PickZOffset.ToString();

                txtOp20X.Text = _part.Op20PickXOffset.ToString();
                txtOp20Y.Text = _part.Op20PickYOffset.ToString();
                txtOp20Z.Text = _part.Op20PickZOffset.ToString();

                txtFinX.Text = _part.Op20FinXOffset.ToString();
                txtFinY.Text = _part.Op20FinYOffset.ToString();
                txtFinZ.Text = _part.Op20FinZOffset.ToString();

                txtOp10PSI.Text = _part.Op10VisePSI.ToString();
                txtOp20PSI.Text = _part.Op20VisePSI.ToString();

                txtOp10Prog.Text = _part.Op10ProgramName;
                txtOp20Prog.Text = _part.Op20ProgramName;

                txtOp10Time.Text = _part.Op10CycleTime.ToString();
                txtOp20Time.Text = _part.Op20CycleTime.ToString();
            }
        }

        private static double ParseDoubleOrZero(string s)
        {
            double v; return double.TryParse(s, out v) ? v : 0.0;
        }

        private static int ParseIntOrDefault(string s, int def)
        {
            int v; return int.TryParse(s, out v) ? v : def;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Basic validation
                var pn = (txtPartNumber.Text ?? string.Empty).Trim();
                if (pn.Length == 0)
                {
                    MessageBox.Show("Part Number is required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtPartNumber.Focus();
                    return;
                }

                _part.PartNumber = pn;
                _part.Description = (txtDescription.Text ?? string.Empty).Trim();

                // Stock sizes
                _part.StockWidth = ParseDoubleOrZero(txtWidth.Text);
                _part.StockDepth = ParseDoubleOrZero(txtDepth.Text);
                _part.StockHeight = ParseDoubleOrZero(txtHeight.Text);

                // Op10 Offsets
                _part.Op10PickXOffset = ParseDoubleOrZero(txtOp10X.Text);
                _part.Op10PickYOffset = ParseDoubleOrZero(txtOp10Y.Text);
                _part.Op10PickZOffset = ParseDoubleOrZero(txtOp10Z.Text);

                // Op20 Offsets
                _part.Op20PickXOffset = ParseDoubleOrZero(txtOp20X.Text);
                _part.Op20PickYOffset = ParseDoubleOrZero(txtOp20Y.Text);
                _part.Op20PickZOffset = ParseDoubleOrZero(txtOp20Z.Text);

                // Finish offsets
                _part.Op20FinXOffset = ParseDoubleOrZero(txtFinX.Text);
                _part.Op20FinYOffset = ParseDoubleOrZero(txtFinY.Text);
                _part.Op20FinZOffset = ParseDoubleOrZero(txtFinZ.Text);

                // Vise PSI / Programs / Times
                _part.Op10VisePSI = ParseIntOrDefault(txtOp10PSI.Text, 120);
                _part.Op20VisePSI = ParseIntOrDefault(txtOp20PSI.Text, 120);

                _part.Op10ProgramName = (txtOp10Prog.Text ?? string.Empty).Trim();
                _part.Op20ProgramName = (txtOp20Prog.Text ?? string.Empty).Trim();

                _part.Op10CycleTime = ParseDoubleOrZero(txtOp10Time.Text);
                _part.Op20CycleTime = ParseDoubleOrZero(txtOp20Time.Text);

                if (_isEdit)
                    Repository.UpdatePart(_part);
                else
                    Repository.InsertPart(_part);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving part:\n" + ex.Message, "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
