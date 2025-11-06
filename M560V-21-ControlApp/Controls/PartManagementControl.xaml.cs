using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using M560V_21_ControlApp.Data; // Part + Repository namespaces
using M560V_21_ControlApp.Controls;
using M560V_21_ControlApp.Models;

namespace M560V_21_ControlApp
{
    public partial class PartManagementControl : UserControl
    {
        private readonly Repository _repo = new Repository();

        public PartManagementControl()
        {
            InitializeComponent();
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            List<Part> parts = _repo.GetAllParts();
            PartsGrid.ItemsSource = parts;
        }

        private Part GetSelectedPart()
        {
            return PartsGrid?.SelectedItem as Part;
        }

        // ADD
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var win = new PartEditWindow { Owner = Window.GetWindow(this) };
            if (win.ShowDialog() == true)
            {
                _repo.InsertPart(win.CurrentPart);
                RefreshGrid();
            }
        }

        // EDIT
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedPart();
            if (selected == null)
            {
                MessageBox.Show("Select a part to edit.", "Edit", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var win = new PartEditWindow(selected) { Owner = Window.GetWindow(this) };
            if (win.ShowDialog() == true)
            {
                // keep original Id for update
                win.CurrentPart.Id = selected.Id;
                _repo.UpdatePart(win.CurrentPart);
                RefreshGrid();
            }
        }

        // DELETE
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedPart();
            if (selected == null)
            {
                MessageBox.Show("Select a part to delete.", "Delete", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var confirm = MessageBox.Show($"Delete part \"{selected.PartNumber}\"?", "Confirm Delete",
                                          MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm == MessageBoxResult.Yes)
            {
                _repo.DeletePart(selected.Id);
                RefreshGrid();
            }
        }
    }
}
