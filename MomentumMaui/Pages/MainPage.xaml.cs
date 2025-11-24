using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Momentum.Shared.Data;
using Momentum.Shared.Services;
using System;
using System.Threading.Tasks;

namespace MomentumMaui
{
    public partial class MainPage : ContentPage
    {
        private readonly UserStateService _userStats = new();

        public MainPage()
        {
            InitializeComponent();

            // Subscribe to the timer completion event
            MyTimer?.TimerCompleted += OnTimerCompleted;
        }
        
        void UpdateThemeIcon()
        {
            bool isLight = Application.Current?.UserAppTheme == AppTheme.Light;
            ThemeButton.Source = isLight ? "moon.svg" : "sun.svg";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var prompt = PromptRepository.GetRandomPrompt();
            PromptLabel.Text = prompt.Text;

            _userStats.LoadData();
            UpdateUI();
            UpdateThemeIcon();
        }

        private void UpdateUI()
        {
            StreakLabel.Text = _userStats.Streak.Current.ToString();
            LevelLabel.Text = $"Level {_userStats.Level}";
            var (xpInto, xpNeeded, progress) = _userStats.GetProgress();
            XpProgressBar.Progress = progress;
            XpLabel.Text = $"{xpInto} / {xpNeeded} XP";
            var newTheme = Application.Current?.UserAppTheme == AppTheme.Light ? "moon.svg" : "sun.svg";
            Application.Current?.UserAppTheme = _userStats.Theme == "light" ? AppTheme.Light : AppTheme.Dark;
        }

        private void OnStartClicked(object sender, EventArgs e)
        {
            MyTimer.IsActive = true;
        }

        private void OnStopClicked(object sender, EventArgs e)
        {
            MyTimer.IsActive = false;
        }

        private void OnTimerCompleted(object sender, EventArgs e)
        {
            DisplayAlertAsync("Timer Complete", "The 2-minute timer has finished!", "OK");
        }

        private void OnThemeToggleClicked(object sender, EventArgs e)
        {
            var newTheme = Application.Current?.UserAppTheme == AppTheme.Light ? AppTheme.Dark : AppTheme.Light;
            Application.Current?.UserAppTheme = newTheme;
            UpdateThemeIcon();
        }

        void UpdateIconsForTheme()
        {
            bool isLight = Application.Current?.UserAppTheme == AppTheme.Light;
            StreakIcon.Source = isLight ? "flame.svg" : "flame_dark.svg";            
        }

        private void OnJournalClicked(object sender, EventArgs e)
        {
            // Start on the Journal Page -> Add a visual to the tab to show the page
        }

        private async Task OnHistoryClicked(object sender, EventArgs e)
        {
            // Try to push navigation stack.  If this page isn't wrapped in a NavigationPage,
            // Fall back to setting the MainPage to a NavigationPage containing HistoryPage

            try
            {
                if (this.Navigation != null && this.Navigation.NavigationStack != null)
                {
                    await Navigation.PushAsync(new HistoryPage());
                    return;
                }
            }
            catch
            {
                // Ignore and fall back
            } Application.Current.MainPage = new NavigationPage(new HistoryPage());
        }
    }
}