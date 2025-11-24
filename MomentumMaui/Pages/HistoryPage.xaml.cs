using Microsoft.Maui.Controls;
using System;

namespace MomentumMaui
{
    public partial class HistoryPage : ContentPage
    {
        public HistoryPage()
        {
            InitializeComponent();

            // Set initial date in code to avoid x:Static / assembly mapping issues in XAML
            HistoryDatePicker.Date = DateTime.Now;
        }

        private async void OnOpenEntryClicked(object sender, EventArgs e)
        {
            // Placeholder action — hook this into your journal storage to open the selected date's entry.
            await DisplayAlert("History", $"Open entry for {HistoryDatePicker.Date:d}", "OK");
        }
    }
}