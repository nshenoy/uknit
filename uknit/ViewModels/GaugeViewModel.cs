using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using uknit.Models;

namespace uknit.ViewModels
{
	public class GaugeViewModel : ViewModelBase
	{
		private const string GaugeMatched = "Gauge matched!";
		private const string GaugeSmall = "Gauge is smaller than the pattern. Consider an increased needle size.";
		private const string GaugeLarge = "Gauge is larger than the pattern. Consider a decreased needle size.";
		private const string SwatchMeasurementInches = "4 inches";
		private const string SwatchMeasurementCentimeters = "10 cm";

		private string UnitOfMeasure;

		public ObservableCollection<string> ActualGaugeMeasurementOptions
		{
			get;
			private set;
		}

		public string SwatchMeasurement
		{
			get
			{
				if(this.UnitOfMeasure == "Inches")
				{
					return GaugeViewModel.SwatchMeasurementInches;
				}
				else
				{
					return GaugeViewModel.SwatchMeasurementCentimeters;
				}
			}
		}

		private double ActualGaugeProperty;
		public double ActualGauge
		{
			get
			{
				return this.ActualGaugeProperty;
			}
			private set
			{
				this.ActualGaugeProperty = value;
				NotifyPropertyChanged("ActualGauge");
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

		public GaugeViewModel()
		{
			this.ActualGaugeProperty = 0;
			this.LoadData();
		}

		public new bool Update()
		{
			bool isUpdated = false;

			if(AppSettings.GetUnitOfMeasure() != this.UnitOfMeasure)
			{
				this.LoadData();
				NotifyPropertyChanged("SwatchMeasurement");
				isUpdated = true;
			}

			base.Update();

			return isUpdated;
		}

		public bool IsDataLoaded
		{
			get;
			private set;
		}

		public void LoadData()
		{
			int max;
			string units;

			this.UnitOfMeasure = AppSettings.GetUnitOfMeasure();

			if(this.UnitOfMeasure == "Inches")
			{
				max = 4;
				units = "in";
			}
			else
			{
				max = 10;
				units = "cm";
			}

			ObservableCollection<string> measurementChoices = new ObservableCollection<string>();
			for(int i = max; i > 0; i--)
			{
				measurementChoices.Add(String.Format("{0} {1}", i, units));
			}

			measurementChoices.OrderBy(p => p);
			this.ActualGaugeMeasurementOptions = measurementChoices;
			NotifyPropertyChanged("ActualGaugeMeasurementOptions");
			this.IsDataLoaded = true;
		}

		public void CalculateActualGauge(double measuredStitches, double measuredLength)
		{
			double gauge = measuredStitches / measuredLength;

			if(this.UnitOfMeasure == "Inches")
			{
				gauge = gauge * 4;
			}
			else
			{
				gauge = gauge * 10;
			}

			this.ActualGauge = gauge;
		}

		public void MeasureGaugeGuidance(double patternGauge)
		{
			if(this.ActualGauge == 0)
			{
				this.GaugeGuidance = String.Empty;
			}
			else if(this.ActualGauge == patternGauge)
			{
				this.GaugeGuidance = GaugeViewModel.GaugeMatched;
			}
			else if(this.ActualGauge > patternGauge)
			{
				this.GaugeGuidance = GaugeViewModel.GaugeLarge;
			}
			else if(this.ActualGauge < patternGauge)
			{
				this.GaugeGuidance = GaugeViewModel.GaugeSmall;
			}
		}
	}
}
