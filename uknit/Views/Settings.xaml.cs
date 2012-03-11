using System;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using System.Windows.Media;
using uknit.Models;

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

			int selectedIndex = 0;
			switch(ConfigurationManager.GetUnitOfMeasure())
			{
				case "Imperial":
					selectedIndex = 0;
					break;
				case "Metric":
					selectedIndex = 1;
					break;
			}

			Dispatcher.BeginInvoke(() =>
			{
				this.UnitOfMeasure.SelectedIndex = selectedIndex;
			});

			//this.PhotoChooser = new PhotoChooserTask();
			//this.PhotoChooser.Completed += new EventHandler<PhotoResult>(PhotoChooserTask_Completed);
			this.UnitOfMeasure.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(UnitOfMeasure_SelectionChanged);
		}

		void PhotoChooserTask_Completed(object sender, PhotoResult e)
		{
			if(e.TaskResult == TaskResult.OK)
			{
				uknit.Models.ConfigurationManager.SaveBackgroundImage(e.ChosenPhoto);
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
			if(e.AddedItems.Count == 0)
			{
				ConfigurationManager.SetUnitOfMeasure("Imperial");
			}
			else
			{
				ConfigurationManager.SetUnitOfMeasure(((ListPickerItem)(e.AddedItems[0])).Content as string);
			}
		}
	}
}