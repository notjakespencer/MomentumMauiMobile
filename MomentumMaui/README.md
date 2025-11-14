# Momentum - Daily Journaling App

A .NET MAUI mobile application for daily journaling with AI-powered prompts, mood tracking, and progress visualization.

## Current Features

- ✅ Custom circular timer control (2-minute countdown)
- ✅ Dark/Light theme support with automatic switching
- ✅ Theme-aware color system
- ✅ Smooth timer animations (3 degrees per second)

## Tomorrow's Tasks

### 1. Add Text Box Below the Timer
- [ ] Create a multi-line `Editor` control for journal entry
- [ ] Style it to match the app's theme (card background with border)
- [ ] Set minimum height (e.g., 200px)
- [ ] Add placeholder text: "Start writing your thoughts..."
- [ ] Make it auto-focus when timer starts

### 2. Add Submit Button Below the Text Box
- [ ] Create a primary styled button with "Complete Entry" text
- [ ] Only show when text has been entered
- [ ] Disable when timer hasn't started
- [ ] Wire up to handle journal entry submission
- [ ] Show mood selector after submission

### 3. Add PromptCard Above the Text Box
- [ ] Create a new `PromptCard` control (reusable component)
- [ ] Display the daily journal prompt
- [ ] Make it tappable (acts as a button)
- [ ] Style as a card with rounded corners and shadow
- [ ] On tap: provide hint or generate new prompt

### 4. Add Calendar Control
- [ ] Research .NET MAUI calendar options (Community Toolkit or custom)
- [ ] Create calendar view page/control
- [ ] Color-code days based on mood:
  - Green: Amazing/Good
  - Yellow: Okay
  - Orange: Tough
  - Red: Difficult
- [ ] Show completion indicator (dot) on days with entries
- [ ] Tap a day to view that day's entry (read-only)

### 5. Build Prompt Library
- [ ] Create `Prompts.cs` in `Momentum.Shared` project
- [ ] Add array of 365 journal prompts (one per day)
- [ ] Implement logic to select prompt based on day of year
- [ ] Categories to include:
  - Gratitude prompts
  - Self-reflection prompts
  - Goal-setting prompts
  - Mindfulness prompts
  - Creative prompts
- [ ] Ensure no duplicate prompts in a year
- [ ] Consider AI integration for personalized prompts (future)

## Design System

### Colors (Dark Theme - Default)
- Background: `#0C0A09`
- Foreground: `#FAFAF9`
- Card: `#1C1917`
- Primary: `#8B5CF6` (Purple)
- Border: `#292524`

### Typography
- Headline: 32px, Bold
- SubHeadline: 24px, Bold
- Body: 14px, Regular
- Small: 12px, Regular

## Tech Stack

- **.NET 9**
- **.NET MAUI** (iOS, Android, Windows)
- **Azure Storage** (for journal entries)
- **Microsoft Entra ID** (authentication)
- **AI Agent** (prompt generation - future)

## Getting Started

1. Clone the repository
2. Open `MomentumMaui.sln` in Visual Studio 2022
3. Set `MomentumMaui` as the startup project
4. Select your target platform (Android Emulator, iOS Simulator, or Windows)
5. Press F5 to run

## Notes

- Timer starts automatically when user begins typing
- One journal entry per day (enforced)
- Entries become immutable after submission
- Experience points awarded for daily streaks
