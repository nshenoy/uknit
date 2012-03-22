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
	public partial class StitchConversion : PhoneApplicationPage
	{
		private StitchConversionViewModel viewModel = new StitchConversionViewModel();

		public StitchConversion()
		{
			InitializeComponent();

			this.DataContext = viewModel;			
		}

		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
			if(viewModel.Update())
			{
				this.MeasuredLength.SelectedIndex = 0;
			}
			base.OnNavigatedTo(e);
		}

		private void Measurement_TextChanged(object sender, TextChangedEventArgs e)
		{
			CalculateStitches();
		}

		private void Measurement_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			CalculateStitches();
		}

		private void OnClick_Settings(object sender, EventArgs e)
		{
			NavigationService.Navigate(new Uri("/Views/Settings.xaml", UriKind.Relative));
		}

		private void CalculateStitches()
		{
			double patternGauge = 0;
			double patternStitches = 0;
			double measuredStitches = 0;
			double measuredLength = 0;
			double measuredGauge = 0;

			if(!String.IsNullOrWhiteSpace(this.PatternGauge.Text))
			{
				patternGauge = double.Parse(this.PatternGauge.Text);
			}

			if(!String.IsNullOrWhiteSpace(this.PatternStitches.Text))
			{
				patternStitches = double.Parse(this.PatternStitches.Text);
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

			if((patternGauge > 0) && (patternStitches > 0) && (measuredStitches > 0) && (measuredLength > 0))
			{
				measuredGauge = viewModel.CalculateActualGauge(measuredStitches, measuredLength);
				viewModel.CalculateActualStitches(patternGauge, patternStitches, measuredGauge);
			}
		}
	}
}