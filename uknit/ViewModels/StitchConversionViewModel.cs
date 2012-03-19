using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using uknit.Models;

namespace uknit.ViewModels
{
	public class StitchConversionViewModel : INotifyPropertyChanged
	{
		private const string GaugeMatched = "Gauge matched!";
		private const string GaugeSmall = "Gauge is smaller than the pattern. Consider an increased needle size.";
		private const string GaugeLarge = "Gauge is larger than the pattern. Consider a decreased needle size.";
		private const string SwatchMeasurementImperial = "4 inches";
		private const string SwatchMeasurementMetric = "10 cm";

		private string UnitOfMeasure;
		private ApplicationSettingsManager AppSettings = new ApplicationSettingsManager();

		public ObservableCollection<string> ActualGaugeMeasurementOptions
		{
			get;
			private set;
		}

		public string SwatchMeasurement
		{
			get
			{
				if(this.UnitOfMeasure == "Imperial")
				{
					return StitchConversionViewModel.SwatchMeasurementImperial;
				}
				else
				{
					return StitchConversionViewModel.SwatchMeasurementMetric;
				}
			}
		}

		private double ActualStitchesProperty;
		public double ActualStitches
		{
			get
			{
				return this.ActualStitchesProperty;
			}
			private set
			{
				this.ActualStitchesProperty = value;
				NotifyPropertyChanged("ActualStitches");
			}
		}

		private string GaugeGuidanceProperty;
		public string GaugeGuidance
		{
			get
			{
				return this.GaugeGuidanceProperty;
			}
			set
			{
				this.GaugeGuidanceProperty = value;
				NotifyPropertyChanged("GaugeGuidance");
			}
		}

		public StitchConversionViewModel()
		{
			this.ActualStitchesProperty = 0;
		}

		public bool DetectSettingsChange()
		{
			return this.UnitOfMeasure != AppSettings.GetUnitOfMeasure();
		}

		public void Reinitialize()
		{
			if(AppSettings.GetUnitOfMeasure() != this.UnitOfMeasure)
			{
				this.UnitOfMeasure = AppSettings.GetUnitOfMeasure();
				NotifyPropertyChanged("SwatchMeasurement");
			}
		}

		public void CalculateActualStitches(double patternGauge, double patternStitches, double measuredGauge)
		{
			double lengthKnit = patternStitches / patternGauge;
			double actualStitches = lengthKnit * measuredGauge;

			this.ActualStitches = actualStitches;
		}

		public void MeasureGaugeGuidance(double patternGauge)
		{
			//if(this.ActualStitches == 0)
			//{
			//    this.GaugeGuidance = String.Empty;
			//}
			//else if(this.ActualStitches == patternGauge)
			//{
			//    this.GaugeGuidance = StitchConversionViewModel.GaugeMatched;
			//}
			//else if(this.ActualStitches > patternGauge)
			//{
			//    this.GaugeGuidance = StitchConversionViewModel.GaugeLarge;
			//}
			//else if(this.ActualStitches < patternGauge)
			//{
			//    this.GaugeGuidance = StitchConversionViewModel.GaugeSmall;
			//}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(String propertyName)
		{
			if(null != PropertyChanged)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
