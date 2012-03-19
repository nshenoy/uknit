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
			if(viewModel.DetectSettingsChange())
			{
				viewModel.Reinitialize();
			}
			base.OnNavigatedTo(e);
		}

		private void Measurement_TextChanged(object sender, TextChangedEventArgs e)
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
			double measuredGauge = 0;

			if(!String.IsNullOrWhiteSpace(this.PatternGauge.Text))
			{
				patternGauge = double.Parse(this.PatternGauge.Text);
			}

			if(!String.IsNullOrWhiteSpace(this.PatternStitches.Text))
			{
				patternStitches = double.Parse(this.PatternStitches.Text);
			}

			if(!String.IsNullOrWhiteSpace(this.MeasuredGauge.Text))
			{
				measuredGauge = double.Parse(this.MeasuredGauge.Text);
			}

			if((patternGauge > 0) && (patternStitches > 0) && (measuredGauge > 0))
			{
				viewModel.CalculateActualStitches(patternGauge, patternStitches, measuredGauge);
			}
		}
	}
}