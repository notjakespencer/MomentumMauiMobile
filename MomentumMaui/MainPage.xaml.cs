namespace MomentumMaui
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            // Subscribe to the timer completion event
            MyTimer.TimerCompleted += OnTimerCompleted;
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
            DisplayAlert("Timer Complete", "The 2-minute timer has finished!", "OK");
        }
    }
}