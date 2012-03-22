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
using uknit.ViewModels;

namespace uknit.Views
{
	public partial class YardsMeters : PhoneApplicationPage
	{
		private SolidColorBrush SteelBlue = new SolidColorBrush(Color.FromArgb(0xFF, 0x46, 0x82, 0xb4));
		private SolidColorBrush WhiteSmoke = new SolidColorBrush(Color.FromArgb(0xFF, 0xF5, 0xF5, 0xF5));

		private ViewModelBase viewModel = new ViewModelBase();

		private bool isNew = true;

		public YardsMeters()
		{
			InitializeComponent();
			this.DataContext = viewModel;
			isNew = true;
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			State["Meters.Text"] = Meters.Text;
			State["Yards.Text"] = Yards.Text;

			base.OnNavigatedFrom(e);
		}

		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
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

			viewModel.Update();
			base.OnNavigatedTo(e);
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
	}
}