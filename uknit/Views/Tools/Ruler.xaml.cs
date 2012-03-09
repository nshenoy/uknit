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
using System.Diagnostics;
using uknit.Models;

namespace uknit.Views.Tools
{
	public partial class Ruler : PhoneApplicationPage
	{
		public enum ImperialTick
		{
			Eighth,
			Quarter,
			Half,
			Inch
		}

		public enum MetricTick
		{
			Millimeter,
			Half,
			Centimeter
		}

		public Ruler()
		{
			InitializeComponent();
		}

		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
			this.RulerGrid.Children.Clear();
			if(ConfigurationModel.GetUnitOfMeasure() == "Imperial")
			{
				DrawRulerImperial(App.Current.Host.Content.ActualHeight);
			}
			else
			{
				DrawRulerMetric(App.Current.Host.Content.ActualHeight);
			}

			base.OnNavigatedTo(e);
		}

		private void OnClick_Settings(object sender, EventArgs e)
		{
			NavigationService.Navigate(new Uri("/Views/Settings.xaml", UriKind.Relative));
		}

		private void DrawRulerImperial(double rulerLengthInPixels)
		{
			//int ppi = 262;
			double ppi = ConfigurationModel.GetDevicePixelsPerInch();
			double pixelsBetweenLines = (ppi - 9) / 9;

			Debug.WriteLine("pixelsBetweenLines is {0}", pixelsBetweenLines);

			ImperialTick tick = ImperialTick.Inch;
			double sum = 0;
			double offset = 10;

			while(sum < rulerLengthInPixels)
			{
				for(int idx = 0; idx < 9; idx++)
				{
					if(idx > 0)
					{
						sum += pixelsBetweenLines;
					}

					if(idx % 8 == 0)
					{
						tick = ImperialTick.Inch;
					}
					else if(idx % 4 == 0)
					{
						tick = ImperialTick.Half;
					}
					else if(idx % 2 == 0)
					{
						tick = ImperialTick.Quarter;
					}
					else
					{
						tick = ImperialTick.Eighth;
					}

					if(idx < 8)
					{
						sum += 1;

						Line line = new Line()
						{
							Style = (Style)this.LayoutRoot.Resources[tick.ToString()],
							Y1 = sum + offset,
							Y2 = sum + offset
						};

						Debug.WriteLine("Drawing line {0} at {1}", tick.ToString(), line.Y1);
						this.RulerGrid.Children.Add(line);
					}
				}
			}
		}

		private void DrawRulerMetric(double rulerLengthInPixels)
		{
			//int ppi = 262;
			double ppi = ConfigurationModel.GetDevicePixelsPerCentimeter();
			double pixelsBetweenLines = (ppi - 11) / 11;

			Debug.WriteLine("pixelsBetweenLines is {0}", pixelsBetweenLines);

			MetricTick tick = MetricTick.Centimeter;
			double sum = 0;
			double offset = 10;

			while(sum < rulerLengthInPixels)
			{
				for(int idx = 0; idx < 11; idx++)
				{
					if(idx > 0)
					{
						sum += pixelsBetweenLines;
					}

					if(idx % 10 == 0)
					{
						tick = MetricTick.Centimeter;
					}
					else if(idx % 5 == 0)
					{
						tick = MetricTick.Half;
					}
					else
					{
						tick = MetricTick.Millimeter;
					}

					if(idx < 10)
					{
						sum += 1;

						Line line = new Line()
						{
							Style = (Style)this.LayoutRoot.Resources[tick.ToString()],
							Y1 = sum + offset,
							Y2 = sum + offset
						};

						Debug.WriteLine("Drawing line {0} at {1}", tick.ToString(), line.Y1);
						this.RulerGrid.Children.Add(line);
					}
				}
			}
		}

		private void OnClick_Bigger(object sender, RoutedEventArgs e)
		{
			var offsetCalcLines = this.RulerGrid.Children.OfType<Line>().Take(2).ToArray();
			double pixelsBetweenLines = offsetCalcLines[1].Y1 - offsetCalcLines[0].Y1 + 1;
			double sum = offsetCalcLines[0].Y1;

			ConfigurationModel.SetDevicePixelDensity(pixelsBetweenLines - 1);
			Debug.WriteLine("Spacing is now {0}", pixelsBetweenLines - 1);
			Debug.WriteLine("PPI is now {0}", ConfigurationModel.GetDevicePixelDensity());

			foreach(Line line in this.RulerGrid.Children.OfType<Line>().Skip(1))
			{
				sum += pixelsBetweenLines;
				line.Y1 = sum;
				line.Y2 = sum;
			}
		}

		private void OnClick_Smaller(object sender, RoutedEventArgs e)
		{
			var offsetCalcLines = this.RulerGrid.Children.OfType<Line>().Take(2).ToArray();
			double pixelsBetweenLines = offsetCalcLines[1].Y1 - offsetCalcLines[0].Y1 - 1;
			double sum = offsetCalcLines[0].Y1;

			ConfigurationModel.SetDevicePixelDensity(pixelsBetweenLines - 1);
			Debug.WriteLine("Spacing is now {0}", pixelsBetweenLines - 1);
			Debug.WriteLine("PPI is now {0}", ConfigurationModel.GetDevicePixelDensity());

			foreach(Line line in this.RulerGrid.Children.OfType<Line>().Skip(1))
			{
				sum += pixelsBetweenLines;
				line.Y1 = sum;
				line.Y2 = sum;
			}
		}
	}
}