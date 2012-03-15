using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using uknit.Models;

namespace uknit
{
	public partial class MainPage : PhoneApplicationPage
	{
		private ApplicationSettingsManager AppSettings = new ApplicationSettingsManager();
		private MarketplaceDetailTask MarketPlaceDetail = new MarketplaceDetailTask();

		// Constructor
		public MainPage()
		{
			InitializeComponent();

			DataContext = App.ViewModel;
			this.Loaded += new RoutedEventHandler(MainPage_Loaded);
		}

		// Load data for the ViewModel Items
		private void MainPage_Loaded(object sender, RoutedEventArgs e)
		{
			if(!App.ViewModel.IsDataLoaded)
			{
				App.ViewModel.LoadData();

				if(System.Diagnostics.Debugger.IsAttached)
				{
					MetroGridHelper.IsVisible = true;
				}
			}

			Visibility isLightTheme = (Visibility)Resources["PhoneLightThemeVisibility"];
			if(this.AppSettings.IsBackgroundEnabled() && (isLightTheme == Visibility.Visible))
			{
				this.MainPagePanorama.Foreground = new SolidColorBrush(Colors.White);
				this.AddIcon.Source = new BitmapImage(new Uri("Content/Images/dark/add.png", UriKind.Relative));
			}
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			base.OnNavigatedFrom(e);
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			IDictionary<string, string> queryString = this.NavigationContext.QueryString;

			if(queryString.ContainsKey("AddEditProject"))
			{
				this.NavigationService.RemoveBackEntry();
				this.NavigationService.RemoveBackEntry();
			}

			if(!this.AppSettings.IsBackgroundEnabled())
			{
				Visibility isLightTheme = (Visibility)Resources["PhoneLightThemeVisibility"];
				if(isLightTheme == Visibility.Visible)
				{
					this.MainPagePanorama.Foreground = new SolidColorBrush(Colors.Black);
					this.AddIcon.Source = new BitmapImage(new Uri("Content/Images/Light/add.png", UriKind.Relative));
				}
			}
			App.ViewModel.PanoramaBackgroundBrush = this.AppSettings.GetBackgroundBrush();
			base.OnNavigatedTo(e);
		}

		private void OnClick_Settings(object sender, EventArgs e)
		{
			NavigationService.Navigate(new Uri("/Views/Settings.xaml", UriKind.Relative));
		}

		private void OnClick_RowCounter(object sender, EventArgs e)
		{
			Button button = (Button)sender;
			string rowCounterPageUri = String.Format("/Views/RowCounter.xaml?ProjectName={0}", button.Tag.ToString());
			NavigationService.Navigate(new Uri(rowCounterPageUri, UriKind.Relative));
		}


		private void OnContextMenuClick_EditProject(object sender, RoutedEventArgs e)
		{
			MenuItem menuItem = (MenuItem)sender;
			string editPageUri = String.Format("/Views/AddNewProject.xaml?ProjectName={0}", menuItem.Tag.ToString());

			this.NavigationService.Navigate(new Uri(editPageUri, UriKind.Relative));
		}

		private void OnContextMenuClick_DeleteProject(object sender, RoutedEventArgs e)
		{
			MenuItem menuItem = (MenuItem)sender;
			string projectName = menuItem.Tag.ToString();

			Coding4Fun.Phone.Controls.MessagePrompt messagePrompt = new Coding4Fun.Phone.Controls.MessagePrompt();
			messagePrompt.IsCancelVisible = true;
			messagePrompt.Body = new TextBlock
			{
				Text = String.Format("You are about to delete \'{0}\'. Are you sure you want to do this?", projectName),
				FontSize = 30.0,
				TextWrapping = TextWrapping.Wrap
			};

			messagePrompt.Completed += (str, res) =>
				{
					if(res.PopUpResult == Coding4Fun.Phone.Controls.PopUpResult.Ok)
					{
						DeleteProject(projectName);
						MessageBox.Show(String.Format("Deleted \'{0}\'", projectName));
					}
				};

			messagePrompt.Show();
		}

		private void DeleteProject(string projectName)
		{
			KnittingProject project = this.AppSettings.GetKnittingProjectByName(projectName);
			int index = App.ViewModel.KnittingProjects.IndexOf(project);
			App.ViewModel.KnittingProjects.RemoveAt(index);
			this.AppSettings.RemoveKnittingProjectByName(projectName);
		}

		private void OnClick_Tools_YardsMeters(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new Uri("/Views/Tools/YardsMeters.xaml", UriKind.Relative));
		}

		private void OnClick_Tools_Gauge(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new Uri("/Views/Tools/GaugeCalculator.xaml", UriKind.Relative));
		}

		private void OnClick_Tools_Ruler(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new Uri("/Views/Tools/Ruler.xaml", UriKind.Relative));
		}

		private void OnClick_AddNewProject(object sender, RoutedEventArgs e)
		{
			bool isTrial = (Application.Current as App).IsTrial;
			if(!isTrial || (isTrial && this.ProjectItems.Items.Count < 1))
			{
				NavigationService.Navigate(new Uri("/Views/AddNewProject.xaml", UriKind.Relative));
			}
			else
			{
				Coding4Fun.Phone.Controls.MessagePrompt messagePrompt = new Coding4Fun.Phone.Controls.MessagePrompt();
				messagePrompt.IsCancelVisible = true;
				messagePrompt.Body = new TextBlock
				{
					Text = "Thanks for trying uknit! This trial lets you use only 1 project. Would you like to buy the app to add unlimited projects?",
					FontSize = 30.0,
					TextWrapping = TextWrapping.Wrap
				};

				messagePrompt.Completed += (str, res) =>
				{
					if(res.PopUpResult == Coding4Fun.Phone.Controls.PopUpResult.Ok)
					{
						this.MarketPlaceDetail.Show();
					}
				};

				messagePrompt.Show();

			}
		}
	}
}