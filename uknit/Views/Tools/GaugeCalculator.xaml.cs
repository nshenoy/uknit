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
using uknit.ViewModels;

namespace uknit.Views
{
	public partial class GaugeCalculator : PhoneApplicationPage
	{
		private GaugeViewModel viewModel = new GaugeViewModel();

		public GaugeCalculator()
		{
			InitializeComponent();

			this.DataContext = viewModel;

			if(this.viewModel.IsBackgroundEnabled() && this.viewModel.IsLightThemeEnabled())
			{
				this.UpdateTextForeground(Colors.White);
			}
		}

		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
			if(viewModel.Update())
			{
				this.MeasuredLength.SelectedIndex = 0;
			}

			if(!this.viewModel.IsBackgroundEnabled() && this.viewModel.IsLightThemeEnabled())
			{
				this.UpdateTextForeground(Colors.Black);
			}
			else
			{
				this.UpdateTextForeground(Colors.White);
			}

			base.OnNavigatedTo(e);
		}

		private void Measurement_TextChanged(object sender, TextChangedEventArgs e)
		{
			CalculateGauge();
		}

		private void Measurement_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			CalculateGauge();
		}

		private void OnClick_Settings(object sender, EventArgs e)
		{
			NavigationService.Navigate(new Uri("/Views/Settings.xaml", UriKind.Relative));
		}

		private void CalculateGauge()
		{
			double patternGauge = 0;
			double measuredStitches = 0;
			double measuredLength = 0;

			if(!String.IsNullOrWhiteSpace(this.PatternGauge.Text))
			{
				patternGauge = double.Parse(this.PatternGauge.Text);
			}

			if(!String.IsNullOrWhiteSpace(this.MeasuredStitches.Text))
			{
				measuredStitches = double.Parse(this.MeasuredStitches.Text);
			}

			string measuredLengthText = this.MeasuredLength.Items[this.MeasuredLength.SelectedIndex] as string;
			if(!String.IsNullOrWhiteSpace(measuredLengthText))
			{
				measuredLength = double.Parse(measuredLengthText.Substring(0, measuredLengthText.Length - 3));
			}

			if((patternGauge > 0) && (measuredStitches > 0) && (measuredLength > 0))
			{
				viewModel.CalculateActualGauge(measuredStitches, measuredLength);
				viewModel.MeasureGaugeGuidance(patternGauge);
			}
		}

		private void UpdateTextForeground(Color foreground)
		{
			this.ApplicationTitle.Foreground = new SolidColorBrush(foreground);
			this.PageTitle.Foreground = new SolidColorBrush(foreground);

			TextBlock[] contentTextBlocks = this.ContentStackPanel.Children.OfType<TextBlock>().ToArray();
			foreach(TextBlock tb in contentTextBlocks)
			{
				tb.Foreground = new SolidColorBrush(foreground);
			}

			TextBlock[] patternGaugeTextBlocks = this.PatternGaugePanel.Children.OfType<TextBlock>().ToArray();
			foreach(TextBlock tb in patternGaugeTextBlocks)
			{
				tb.Foreground = new SolidColorBrush(foreground);
			}

			TextBlock[] measuredStitchesTextBlocks = this.MeasuredStitchesPanel.Children.OfType<TextBlock>().ToArray();
			foreach(TextBlock tb in measuredStitchesTextBlocks)
			{
				tb.Foreground = new SolidColorBrush(foreground);
			}

			TextBlock[] actualGaugeTextBlocks = this.ActualGaugePanel.Children.OfType<TextBlock>().ToArray();
			foreach(TextBlock tb in actualGaugeTextBlocks)
			{
				if(tb.Name != "ActualCalculatedGauge")
				{
					tb.Foreground = new SolidColorBrush(foreground);
				}
			}
		}
	}
}