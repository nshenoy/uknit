using System;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using System.Windows.Media;

namespace uknit.Views
{
	public partial class Settings : PhoneApplicationPage
	{
		public PhotoChooserTask PhotoChooser;
		private IsolatedStorageSettings IsolatedStorage = IsolatedStorageSettings.ApplicationSettings;

		public Settings()
		{
			InitializeComponent();

			if(IsolatedStorage.Contains("EnableBackgroundImage"))
			{
				bool? isBackgroundEnabled = IsolatedStorage["EnableBackgroundImage"] as bool?;
				this.BackgroundToggle.IsChecked = isBackgroundEnabled;
			}

			if(IsolatedStorage.Contains("UnitOfMeasure"))
			{
				this.UnitOfMeasure.SelectedIndex = (int)IsolatedStorage["UnitOfMeasure"];
			}
			//this.PhotoChooser = new PhotoChooserTask();
			//this.PhotoChooser.Completed += new EventHandler<PhotoResult>(PhotoChooserTask_Completed);
			this.UnitOfMeasure.SelectionChanged +=new System.Windows.Controls.SelectionChangedEventHandler(UnitOfMeasure_SelectionChanged);
		}

		void PhotoChooserTask_Completed(object sender, PhotoResult e)
		{
			if(e.TaskResult == TaskResult.OK)
			{
				uknit.Models.ConfigurationModel.SaveBackgroundImage(e.ChosenPhoto);
			}
		}

		private void OnClick_About(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new Uri("/YourLastAboutDialog;component/AboutPage.xaml", UriKind.Relative));
		}

		private void OnClick_ChangeBackground(object sender, RoutedEventArgs e)
		{
			try
			{
				this.PhotoChooser.Show();
			}
			catch(System.InvalidOperationException ex)
			{
				MessageBox.Show(String.Format("Sorry, an error occurred while selecting a photo. ({0})", ex.Message));
			}
		}

		private void BackgroundToggle_Checked(object sender, RoutedEventArgs e)
		{
			//this.BackgroundPicker.Visibility = System.Windows.Visibility.Visible;
			IsolatedStorage["EnableBackgroundImage"] = true;
		}

		private void BackgroundToggle_Unchecked(object sender, RoutedEventArgs e)
		{
			//this.BackgroundPicker.Visibility = System.Windows.Visibility.Collapsed;
			IsolatedStorage["EnableBackgroundImage"] = false;
		}

		private void UnitOfMeasure_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			IsolatedStorage["UnitOfMeasure"] = this.UnitOfMeasure.SelectedIndex;
		}
	}
}