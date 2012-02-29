using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Navigation;

namespace uknit
{
	public partial class MainPage : PhoneApplicationPage
	{
		private SolidColorBrush SteelBlue = new SolidColorBrush(Color.FromArgb(0xFF, 0x46, 0x82, 0xb4));
		private SolidColorBrush WhiteSmoke = new SolidColorBrush(Color.FromArgb(0xFF, 0xF5, 0xF5, 0xF5));
		private bool isNew = true;

		// Constructor
		public MainPage()
		{
			InitializeComponent();

			// Set the data context of the listbox control to the sample data
			DataContext = App.ViewModel;
			this.Loaded += new RoutedEventHandler(MainPage_Loaded);

			isNew = true;
		}

		// Load data for the ViewModel Items
		private void MainPage_Loaded(object sender, RoutedEventArgs e)
		{
			if(!App.ViewModel.IsDataLoaded)
			{
				App.ViewModel.LoadData();
				MetroGridHelper.IsVisible = true;
			}
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			State["Meters.Text"] = Meters.Text;
			State["Yards.Text"] = Yards.Text;

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
			
			if(isNew)
			{
				object val;

				if(State.TryGetValue("Meters.Text", out val))
				{
					Meters.Text = val as string;
				}

				if(State.TryGetValue("Yards.Text", out val))
				{
					Yards.Text = val as string;
				}
			}

			isNew = false;

			base.OnNavigatedTo(e);
		}

		private void OnClick_Settings(object sender, EventArgs e)
		{
			NavigationService.Navigate(new Uri("/Views/Settings.xaml", UriKind.Relative));
		}

		private void Convert_Click(object sender, RoutedEventArgs e)
		{
			// No need to do anything since the LostFocus handlers do the job.
		}

		private void Yards_GotFocus(object sender, RoutedEventArgs e)
		{
			MetersLabel.Foreground = WhiteSmoke;
			YardsLabel.Foreground = SteelBlue;
		}

		private void Yards_LostFocus(object sender, RoutedEventArgs e)
		{
			// yards * 0.9144
			double meters;
			double yards;

			if(double.TryParse(Yards.Text, out yards))
			{
				meters = Math.Round(yards * 0.9144, 2);
				Meters.Text = meters.ToString();
			}
		}

		private void Meters_GotFocus(object sender, RoutedEventArgs e)
		{
			YardsLabel.Foreground = WhiteSmoke;
			MetersLabel.Foreground = SteelBlue;

		}

		private void Meters_LostFocus(object sender, RoutedEventArgs e)
		{
			// meters * 1.0936133
			double meters;
			double yards;

			if(double.TryParse(Meters.Text, out meters))
			{
				yards = Math.Round(meters * 1.0936133, 2);
				Yards.Text = yards.ToString();
			}

		}

		private void OnContextMenuClick_EditProject(object sender, RoutedEventArgs e)
		{
			MenuItem menuItem = (MenuItem)sender;
			string editPageUri = String.Format("/Views/AddNewProject.xaml?ProjectName={0}", menuItem.Tag.ToString());

			this.NavigationService.Navigate(new Uri(editPageUri, UriKind.Relative));
		}
	}
}