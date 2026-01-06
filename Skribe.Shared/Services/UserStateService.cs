using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Skribe.Shared.Models;
using System;
using System.Globalization;

namespace Skribe.Shared.Services
{
    public class UserStateService
    {
        private const string LastCompletionKey = "LastCompletionDate";

        public Streak Streak { get; private set; } = new();
        public int TotalXp { get; private set; }
        public int Level { get; private set; }
        public string Theme { get; private set; } = "light";

        public void LoadData()
        {
            Streak.Current = Preferences.Get("CurrentStreak", 0);
            Streak.Longest = Preferences.Get("LongestStreak", 0);    
            TotalXp = Preferences.Get("TotalXp", 0);
            Theme = Preferences.Get("Theme", "light");
            Level = CalculateLevel(TotalXp);
            Preferences.Set("Level", Level);

            var storedDate = Preferences.Get(LastCompletionKey, null);
            if (!string.IsNullOrEmpty(storedDate) && DateTime.TryParse(storedDate, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var parsed))
            {
                Streak.LastCompletedDate = parsed.Date;
            }
            else Streak.LastCompletedDate = null;
        }

        public void SaveData()
        {
            Preferences.Set("CurrentStreak", Streak.Current);
            Preferences.Set("LongestStreak", Streak.Longest);
            Preferences.Set("TotalXp", TotalXp);
            Preferences.Set("Theme", Theme);
            Preferences.Set("Level", Level);

            if (Streak.LastCompletedDate.HasValue)
            {
                Preferences.Set(LastCompletionKey, Streak.LastCompletedDate.Value.ToString("o", CultureInfo.InvariantCulture));
            }
            else Preferences.Remove(LastCompletionKey);
        }

        public void UpdateTheme(string newTheme)
        {
            Theme = newTheme;
            Preferences.Set("Theme", Theme);
        }

        public (int xpIntoLevel, int xpNeeded, double progress) GetProgress()
        {
            int xpAtStartOfLevel = 100 * (Level - 1) * Level / 2;
            int xpIntoCurrentLevel = TotalXp - xpAtStartOfLevel;
            int xpNeededForNextLevel = Level * 100;
            double progress = (double)xpIntoCurrentLevel / xpNeededForNextLevel;
            return (xpIntoCurrentLevel, xpNeededForNextLevel, Math.Clamp(progress, 0, 1));
        }

        private int CalculateLevel(int xp)
        {
            if (xp < 100) return 1;
            double level = 0.5 * (1 + Math.Sqrt(1 + (8.0 * xp) / 100));
            return (int)Math.Floor(level);
        }

        public void AddEntry()
        {
            int baseXP = 50;
            double multiplier = Math.Pow(1.1, Streak.Current);
            int earnedXP = (int)Math.Round(baseXP * multiplier);
            TotalXp += earnedXP;

            Streak.Update(DateTime.Now);

            Level = CalculateLevel(TotalXp);
            SaveData();
        }
    }
}
