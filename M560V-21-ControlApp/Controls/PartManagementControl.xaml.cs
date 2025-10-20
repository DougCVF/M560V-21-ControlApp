using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using M560V_21_ControlApp.Data;
using M560V_21_ControlApp.Models;

namespace M560V_21_ControlApp.Controls
{
    public partial class PartManagementControl : UserControl
    {
        public PartManagementControl()
        {
            InitializeComponent();
            LoadParts();
        }

        private void LoadParts()
        {
            try
            {
                List<Part> parts = Repository.GetAllParts();
                dgParts.ItemsSource = parts;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load parts: " + ex.Message,
                    "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var window = new PartEditWindow(null);
            if (window.ShowDialog() == true)
                LoadParts();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var selectedPart = dgParts.SelectedItem as Part;
            if (selectedPart == null)
            {
                MessageBox.Show("Select a part to edit.", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var window = new PartEditWindow(selectedPart);
            if (window.ShowDialog() == true)
                LoadParts();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var selectedPart = dgParts.SelectedItem as Part;
            if (selectedPart == null)
            {
                MessageBox.Show("Select a part to delete.", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (MessageBox.Show("Delete selected part?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    Repository.DeletePart(selectedPart.Id);
                    LoadParts();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Delete failed: " + ex.Message,
                        "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadParts();
        }
    }
}
