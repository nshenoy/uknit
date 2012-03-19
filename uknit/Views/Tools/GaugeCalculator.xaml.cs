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
		}

		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
			if(viewModel.DetectSettingsChange())
			{
				this.MeasuredLength.SelectedIndex = 0;
				viewModel.Reinitialize();
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
			int patternGauge = 0;
			int measuredStitches = 0;
			int measuredLength = 0;

			if(!String.IsNullOrWhiteSpace(this.PatternGauge.Text))
			{
				patternGauge = int.Parse(this.PatternGauge.Text);
			}

			if(!String.IsNullOrWhiteSpace(this.MeasuredStitches.Text))
			{
				measuredStitches = int.Parse(this.MeasuredStitches.Text);
			}

			string measuredLengthText = this.MeasuredLength.Items[this.MeasuredLength.SelectedIndex] as string;
			if(!String.IsNullOrWhiteSpace(measuredLengthText))
			{
				measuredLength = int.Parse(measuredLengthText.Substring(0, measuredLengthText.Length - 3));
			}

			if((patternGauge > 0) && (measuredStitches > 0) && (measuredLength > 0))
			{
				viewModel.CalculateActualGauge(measuredStitches, measuredLength);
				viewModel.MeasureGaugeGuidance(patternGauge);
			}
		}
	}
}