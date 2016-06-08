﻿using System.Windows.Controls;

namespace ENBOrganizer.App.Views.Presets
{
    /// <summary>
    /// Interaction logic for PresetsOverviewView.xaml
    /// </summary>
    public partial class PresetsView : UserControl
    {
        public PresetsView()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AddPopup.IsOpen = true;
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AddPopup.IsOpen = false;
        }
    }
}