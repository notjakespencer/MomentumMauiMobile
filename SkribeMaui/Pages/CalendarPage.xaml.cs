using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using SkribeMaui.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices;
using System.Reflection;

// Alias to disambiguate the standard MAUI NavigationPage from the iOS-specific one
using MauiNavigation = Microsoft.Maui.Controls.NavigationPage;
// Alias to disambiguate the MAUI Application type from the platform-specific one
using MauiApplication = Microsoft.Maui.Controls.Application;
using Skribe.Shared.Services;
using Skribe.Shared.Models;

namespace SkribeMaui
{
    public partial class CalendarPage : ContentPage
    {
        private readonly UserStateService _userStats = new();
        private bool _swipeGesturesAttached;

        public CalendarPage()
        {
            InitializeComponent();
            Loaded += CalendarPage_Loaded;

            Shell.SetNavBarIsVisible(this, false);

            try
            {
                var pageType = typeof(Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific.Page);
                var prop = pageType.GetField("SafeAreaEdgesProperty", BindingFlags.Public | BindingFlags.Static);
                if (prop != null)
                {
                    var bp = prop.GetValue(null) as BindableProperty;
                    if (bp != null)
                    {
                        SetValue(bp, SafeAreaEdges.None);
                    }
                }
                else if (DeviceInfo.Platform == DevicePlatform.iOS)
                {
                    Padding = new Thickness(0);
                }
            }
            catch
            {
                if (DeviceInfo.Platform == DevicePlatform.iOS)
                {
                    Padding = new Thickness(0);
                }
            }
        }

        private void CalendarPage_Loaded(object? sender, EventArgs e)
        {
            Loaded -= CalendarPage_Loaded;
            var myCalendar = FindByName("MyCalendar") as Controls.CalendarGrid;
            if (myCalendar == null)
            {
                return;
            }

            myCalendar.CurrentMonth = DateTime.Now;

            LoadEntriesForMonth(myCalendar, myCalendar.CurrentMonth);
            UpdateMonthHeader(myCalendar.CurrentMonth);

            myCalendar.DateSelected -= OnCalendarDateSelected;
            myCalendar.DateSelected += OnCalendarDateSelected;

            RefreshStats();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            RefreshStats();

            try
            {
                NotificationService.EntrySaved += OnEntrySaved;
            }
            catch
            {
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            try
            {
                NotificationService.EntrySaved -= OnEntrySaved;
            }
            catch
            {
            }
        }

        private void OnEntrySaved(object? sender, DateTime when)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    var myCalendar = FindByName("MyCalendar") as Controls.CalendarGrid;
                    if (myCalendar != null)
                    {
                        LoadEntriesForMonth(myCalendar, myCalendar.CurrentMonth);
                        UpdateMonthHeader(myCalendar.CurrentMonth);
                    }
                }
                catch
                {
                }

                RefreshStats();
            });
        }

        private void RefreshStats()
        {
            try
            {
                _userStats.LoadData();

                var appDir = FileSystem.AppDataDirectory;
                int totalEntries = 0;
                try
                {
                    if (Directory.Exists(appDir))
                    {
                        totalEntries = Directory.GetFiles(appDir, "journal_*.json").Length;
                    }
                }
                catch
                {
                    totalEntries = 0;
                }

                var totalLabel = FindByName("TotalEntriesLabel") as Label;
                var currentStreakLabel = FindByName("CurrentStreakLabel") as Label;
                var longestStreakLabel = FindByName("LongestStreakLabel") as Label;
                var levelLabel = FindByName("CurrentLevelLabel") as Label;

                if (totalLabel != null)
                {
                    totalLabel.Text = totalEntries.ToString();
                }

                if (currentStreakLabel != null)
                {
                    currentStreakLabel.Text = _userStats.Streak.Current.ToString();
                }

                if (longestStreakLabel != null)
                {
                    longestStreakLabel.Text = _userStats.Streak.Longest.ToString();
                }

                if (levelLabel != null)
                {
                    levelLabel.Text = _userStats.Level.ToString();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RefreshStats error: {ex}");
            }
        }

        private async void OnOpenEntryClicked(object sender, EventArgs e)
        {
            await DisplayAlertAsync("History", "No date picker available. Select a date from the calendar to open an entry.", "OK");
        }

        // Navigate back to the Journal page when the journal image/button is tapped.
        private async void OnJournalClicked(object sender, EventArgs e)
        {
            var journal = new MainPage();
            var navPage = new MauiNavigation(journal);

            if (Window != null)
            {
                Window.Page = navPage;
                return;
            }

            var windows = MauiApplication.Current?.Windows;
            if (windows != null && windows.Count > 0)
            {
                windows[0].Page = navPage;
                return;
            }

            if (Navigation?.NavigationStack != null)
            {
                await Navigation.PushAsync(journal);
                return;
            }

            var newWindow = new Window(navPage);
            MauiApplication.Current?.OpenWindow(newWindow);
        }

        private void NavigateMonths(int monthOffset)
        {
            var calendar = FindByName("MyCalendar") as Controls.CalendarGrid;
            if (calendar == null)
            {
                return;
            }

            var newMonth = calendar.CurrentMonth.AddMonths(monthOffset);
            calendar.CurrentMonth = newMonth;

            LoadEntriesForMonth(calendar, newMonth);
            UpdateMonthHeader(newMonth);
        }

        private void UpdateMonthHeader(DateTime month)
        {
            if (FindByName("MonthLabel") is Label label)
            {
                label.Text = month.ToString("MMMM yyyy");
            }
        }

        private void LoadEntriesForMonth(Controls.CalendarGrid calendar, DateTime month)
        {
            try
            {
                var list = new List<Controls.CalendarGrid.CalendarGridEntry>();

                var start = new DateTime(month.Year, month.Month, 1);
                var end = start.AddMonths(1).AddDays(-1);

                for (var d = start; d <= end; d = d.AddDays(1))
                {
                    var fileName = Path.Combine(FileSystem.AppDataDirectory, $"journal_{d:yyyyMMdd}.json");
                    if (!File.Exists(fileName))
                    {
                        continue;
                    }

                    try
                    {
                        var json = File.ReadAllText(fileName);
                        var entry = System.Text.Json.JsonSerializer.Deserialize<JournalEntry>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (entry == null)
                        {
                            continue;
                        }

                        var moodKey = entry.Mood switch
                        {
                            MoodType.VeryHappy => "amazing",
                            MoodType.Happy => "good",
                            MoodType.Neutral => "okay",
                            MoodType.Sad => "tough",
                            MoodType.VerySad => "difficult",
                            _ => null
                        };

                        list.Add(new Controls.CalendarGrid.CalendarGridEntry
                        {
                            Date = entry.Date.Date,
                            Mood = moodKey,
                            Payload = entry
                        });
                    }
                    catch
                    {
                    }
                }

                calendar.Entries = list;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadEntriesForMonth error: {ex}");
            }
        }

        // Calendar date clicked -> show the journal details (prompt, response, mood)
        private async void OnCalendarDateSelected(object? sender, CalendarGrid.CalendarGridEntry? entry)
        {
            if (entry == null)
            {
                await DisplayAlertAsync("No Entry", "No journal entry exists for that date.", "OK");
                return;
            }

            if (entry.Payload is JournalEntry je)
            {
                await Navigation.PushModalAsync(new EntryDetailsPage(je));
                return;
            }

            var fallbackMsg = $"Date: {entry.Date:d}\nMood: {entry.Mood ?? "(unknown)"}\n\nNo further details available.";
            await DisplayAlertAsync("Entry Details", fallbackMsg, "OK");
        }

        private void OnCalendarSwipedLeft(object? sender, SwipedEventArgs e)
        {
            NavigateMonths(1);
        }

        private void OnCalendarSwipedRight(object? sender, SwipedEventArgs e)
        {
            NavigateMonths(-1);
        }
    }
}
