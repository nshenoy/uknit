using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using uknit.Models;
using System.Windows.Media;

namespace uknit.ViewModels
{
	public class StitchConversionViewModel : ViewModelBase
	{
		private const string GaugeMatched = "Gauge matched!";
		private const string GaugeSmall = "Gauge is smaller than the pattern. Consider an increased needle size.";
		private const string GaugeLarge = "Gauge is larger than the pattern. Consider a decreased needle size.";
		private const string SwatchMeasurementInches = "4 inches";
		private const string SwatchMeasurementCentimeters = "10 cm";

		private string UnitOfMeasure;

		public ObservableCollection<string> MeasurementOptions
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
					return StitchConversionViewModel.SwatchMeasurementInches;
				}
				else
				{
					return StitchConversionViewModel.SwatchMeasurementCentimeters;
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

		public bool IsDataLoaded
		{
			get;
			private set;
		}

		public StitchConversionViewModel()
		{
			this.ActualStitchesProperty = 0;
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
			this.MeasurementOptions = measurementChoices;
			NotifyPropertyChanged("MeasurementOptions");
			this.IsDataLoaded = true;
		}

		public void CalculateActualStitches(double patternGauge, double patternStitches, double measuredGauge)
		{
			double lengthKnit = patternStitches / patternGauge;
			double actualStitches = lengthKnit * measuredGauge;

			this.ActualStitches = actualStitches;
		}

		public double CalculateActualGauge(double measuredStitches, double measuredLength)
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

			return gauge;
		}
	}
}
