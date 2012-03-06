using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using uknit.Models;

namespace uknit
{
	public partial class MainPage : PhoneApplicationPage
	{
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

			BitmapImage backgroundImage = ConfigurationModel.GetBackgroundImage();

			if(backgroundImage != null)
			{
				ImageBrush img = new ImageBrush();
				img.ImageSource = backgroundImage;
				this.MainPagePanorama.Background = img;
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
			KnittingProject project = ConfigurationModel.GetKnittingProjectByName(projectName);
			int index = App.ViewModel.KnittingProjects.IndexOf(project);
			App.ViewModel.KnittingProjects.RemoveAt(index);
			ConfigurationModel.RemoveKnittingProjectByName(projectName);
		}

		private void OnClick_Tools_YardsMeters(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new Uri("/Views/Tools_YardsMeters.xaml", UriKind.Relative));
		}

		private void OnClick_Tools_Gauge(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new Uri("/Views/Tools_GaugeCalculator.xaml", UriKind.Relative));
		}
	}
}