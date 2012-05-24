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

namespace uknit.Views.Tools
{
	public partial class YardsMeters : PhoneApplicationPage
	{
		private SolidColorBrush SteelBlue = new SolidColorBrush(Color.FromArgb(0xFF, 0x46, 0x82, 0xb4));
		private SolidColorBrush WhiteSmoke = new SolidColorBrush(Color.FromArgb(0xFF, 0xF5, 0xF5, 0xF5));
		private SolidColorBrush BlackForegroundBrush = new SolidColorBrush(Colors.Black);
		private SolidColorBrush WhiteForegroundBrush = new SolidColorBrush(Colors.White);
		private SolidColorBrush PhoneAccentBrush;
		private SolidColorBrush CurrentPageTextBrush;

		private ViewModelBase viewModel = new ViewModelBase();

		private bool isNew = true;

		public YardsMeters()
		{
			InitializeComponent();
			this.DataContext = viewModel;

			this.PhoneAccentBrush = Resources["PhoneAccentBrush"] as SolidColorBrush;
			if(this.viewModel.IsBackgroundEnabled() && this.viewModel.IsLightThemeEnabled())
			{
				this.CurrentPageTextBrush = this.WhiteForegroundBrush;
				this.UpdateTextForeground();
			}
			else
			{
				this.CurrentPageTextBrush = Resources["PhoneForegroundBrush"] as SolidColorBrush;
			}

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

			if(!this.viewModel.IsBackgroundEnabled() && this.viewModel.IsLightThemeEnabled())
			{
				this.CurrentPageTextBrush = this.BlackForegroundBrush;
				this.UpdateTextForeground();
			}
			else
			{
				this.CurrentPageTextBrush = this.WhiteForegroundBrush;
				this.UpdateTextForeground();
			}

			viewModel.Update();
			base.OnNavigatedTo(e);
		}

		private void Convert_Click(object sender, RoutedEventArgs e)
		{
			// No need to do anything since the LostFocus handlers do the job.
		}

		private void Yards_GotFocus(object sender, RoutedEventArgs e)
		{
			MetersLabel.Foreground = this.CurrentPageTextBrush;
			YardsLabel.Foreground = this.PhoneAccentBrush;
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
			YardsLabel.Foreground = this.CurrentPageTextBrush;
			MetersLabel.Foreground = this.PhoneAccentBrush;
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

		private void UpdateTextForeground()
		{
			this.ApplicationTitle.Foreground = this.CurrentPageTextBrush;
			this.PageTitle.Foreground = this.CurrentPageTextBrush;

			TextBlock[] contentTextBlocks = this.ContentStackPanel.Children.OfType<TextBlock>().ToArray();
			foreach(TextBlock tb in contentTextBlocks)
			{
				tb.Foreground = this.CurrentPageTextBrush;
			}

			TextBlock[] convertTextBlocks = this.ConvertGrid.Children.OfType<TextBlock>().ToArray();
			foreach(TextBlock tb in convertTextBlocks)
			{
				tb.Foreground = this.CurrentPageTextBrush;
			}

			this.Convert.Foreground = this.CurrentPageTextBrush;
			this.Convert.BorderBrush = this.CurrentPageTextBrush;
		}
	}
}