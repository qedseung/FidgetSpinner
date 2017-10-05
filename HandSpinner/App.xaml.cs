using Xamarin.Forms;

namespace HandSpinner
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();
			MainPage = new HandSpinnerPage();
		}

		protected override void OnStart()
		{
			// Handle when your app starts
			if (!Application.Current.Properties.ContainsKey("HIGHSCORE"))
			{
				Application.Current.Properties.Add("HIGHSCORE", 0f);
			}
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
