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
using System.Windows.Data;

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

		private string currentRulerUnitOfMeasure;

		public Ruler()
		{
			InitializeComponent();

			if(!ConfigurationManager.IsBackgroundEnabled())
			{
				this.LayoutRoot.Background = null;
			}
			else
			{
				Visibility isLightTheme = (Visibility)Resources["PhoneLightThemeVisibility"];
				if(isLightTheme == Visibility.Visible)
				{
					this.ApplicationTitle.Foreground = new SolidColorBrush(Colors.White);
					this.PageTitle.Foreground = new SolidColorBrush(Colors.White);
				}
			}

			currentRulerUnitOfMeasure = ConfigurationManager.GetUnitOfMeasure();
		}

		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
			DrawRuler();

			base.OnNavigatedTo(e);

			if(!ConfigurationManager.IsRulerCalibrated())
			{
				AskForCalibration();
			}
		}

		private void OnClick_Settings(object sender, EventArgs e)
		{
			NavigationService.Navigate(new Uri("/Views/Settings.xaml", UriKind.Relative));
		}

		private void DrawRuler()
		{
			this.RulerGrid.Children.Clear();
			if(ConfigurationManager.GetUnitOfMeasure() == "Imperial")
			{
				this.Imperial.IsChecked = true;
				DrawRulerImperial(App.Current.Host.Content.ActualHeight);
			}
			else
			{
				this.Metric.IsChecked = true;
				DrawRulerMetric(App.Current.Host.Content.ActualHeight);
			}
		}

		private void DrawRulerImperial(double rulerLengthInPixels)
		{
			//int ppi = 262;
			double ppi = ConfigurationManager.GetDevicePixelsPerInch();
			double pixelsBetweenLines = (ppi - 9) / 9;

			Debug.WriteLine("pixelsBetweenLines is {0}", pixelsBetweenLines);

			ImperialTick tick = ImperialTick.Inch;
			double sum = 0;
			double offset = 10;
			int currentInch = 0;

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

						if(tick == ImperialTick.Inch)
						{
							line.Name = "tick_inch_" + currentInch;
							TextBlock textBlock = new TextBlock()
							{
								Name = "label_" + line.Name,
								Text = currentInch.ToString(),
								FontSize = 24,
								Foreground = new SolidColorBrush(Colors.Black),
								RenderTransform = new RotateTransform()
								{
									Angle = 90
								}
							};

							textBlock.Margin = new Thickness(line.X2 + 36, line.Y1 - 6, 0, 0);
							this.RulerGrid.Children.Add(textBlock);
						}

						Debug.WriteLine("Drawing line {0} at {1}", tick.ToString(), line.Y1);
						this.RulerGrid.Children.Add(line);
					}
				}

				currentInch++;
			}
		}

		private void DrawRulerMetric(double rulerLengthInPixels)
		{
			//int ppi = 262;
			double ppi = ConfigurationManager.GetDevicePixelsPerCentimeter();
			double pixelsBetweenLines = (ppi - 11) / 11;

			Debug.WriteLine("pixelsBetweenLines is {0}", pixelsBetweenLines);

			MetricTick tick = MetricTick.Centimeter;
			double sum = 0;
			double offset = 10;
			int currentCm = 0;

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

						if(tick == MetricTick.Centimeter)
						{
							line.Name = "tick_cm_" + currentCm;
							TextBlock textBlock = new TextBlock()
							{
								Name = "label_" + line.Name,
								Text = currentCm.ToString(),
								FontSize = 24,
								Foreground = new SolidColorBrush(Colors.Black),
								RenderTransform = new RotateTransform()
								{
									Angle = 90
								}
							};

							textBlock.Margin = new Thickness(line.X2 + 36, line.Y1 - 6, 0, 0);
							this.RulerGrid.Children.Add(textBlock);
						}

						Debug.WriteLine("Drawing line {0} at {1}", tick.ToString(), line.Y1);
						this.RulerGrid.Children.Add(line);
					}
				}

				currentCm++;
			}
		}

		private void OnClick_Bigger(object sender, RoutedEventArgs e)
		{
			var offsetCalcLines = this.RulerGrid.Children.OfType<Line>().OrderBy(l => l.Y1).Take(2).ToArray();
			double pixelsBetweenLines = Math.Abs(offsetCalcLines[1].Y1 - offsetCalcLines[0].Y1) + 1;
			double sum = offsetCalcLines[0].Y1;

			ConfigurationManager.SetDevicePixelDensity(pixelsBetweenLines - 1);
			Debug.WriteLine("Spacing is now {0}", pixelsBetweenLines - 1);
			Debug.WriteLine("PPI is now {0}", ConfigurationManager.GetDevicePixelDensity());

			var lines = this.RulerGrid.Children.OfType<Line>().OrderBy(l => l.Y1).Skip(1);
			foreach(Line line in lines)
			{
				sum += pixelsBetweenLines;
				line.Y1 = sum;
				line.Y2 = sum;

				if(line.Name.StartsWith("tick_"))
				{
					string label = "label_" + line.Name;
					TextBlock tb = this.RulerGrid.Children.OfType<TextBlock>().Where(t => t.Name == label).First();
					Thickness thickness = tb.Margin;
					thickness.Top = line.Y1 - 6;
					tb.Margin = thickness;
				}
			}

			DrawRuler();
		}

		private void OnClick_Smaller(object sender, RoutedEventArgs e)
		{
			var offsetCalcLines = this.RulerGrid.Children.OfType<Line>().OrderBy(l => l.Y1).Take(2).ToArray();
			double pixelsBetweenLines = Math.Abs(offsetCalcLines[1].Y1 - offsetCalcLines[0].Y1) - 1;

			if(pixelsBetweenLines < 2)
			{
				// Don't allow the ruler to be made any smaller. Cuz that's just silly.
				return;
			}

			double sum = offsetCalcLines[0].Y1;

			ConfigurationManager.SetDevicePixelDensity(pixelsBetweenLines - 1);
			Debug.WriteLine("Spacing is now {0}", pixelsBetweenLines - 1);
			Debug.WriteLine("PPI is now {0}", ConfigurationManager.GetDevicePixelDensity());

			var lines = this.RulerGrid.Children.OfType<Line>().OrderBy(l => l.Y1).Skip(1);
			foreach(Line line in lines)
			{
				sum += pixelsBetweenLines;
				line.Y1 = sum;
				line.Y2 = sum;

				if(line.Name.StartsWith("tick_"))
				{
					string label = "label_" + line.Name;
					TextBlock tb = this.RulerGrid.Children.OfType<TextBlock>().Where(t => t.Name == label).First();
					Thickness thickness = tb.Margin;
					thickness.Top = line.Y1 - 6;
					tb.Margin = thickness;
				}
			}

			DrawRuler();
		}

		private void UnitOfMeasure_Checked(object sender, RoutedEventArgs e)
		{
			RadioButton rb = sender as RadioButton;
			ConfigurationManager.SetUnitOfMeasure(rb.Name);
			this.RulerSettingsText.Text = rb.Name;
			if(rb.Name != currentRulerUnitOfMeasure)
			{
				DrawRuler();
				currentRulerUnitOfMeasure = rb.Name;
			}
		}

		private void AskForCalibration()
		{
			Coding4Fun.Phone.Controls.MessagePrompt messagePrompt = new Coding4Fun.Phone.Controls.MessagePrompt();
			messagePrompt.IsCancelVisible = true;
			messagePrompt.Body = new TextBlock
			{
				Text = "I noticed that you have calibrated the ruler yet. This is important to make sure the ruler is somewhat accurate. Afterall, you don't want to make a baby hat that fits Andre the Giant. Would you like to calibrate now?",
				FontSize = 20.0,
				TextWrapping = TextWrapping.Wrap
			};

			messagePrompt.Completed += (str, res) =>
			{
				if(res.PopUpResult == Coding4Fun.Phone.Controls.PopUpResult.Ok)
				{
					this.RulerSettingsSwitch.IsChecked = true;
					this.CalibrationSwitch.IsChecked = true;
					ConfigurationManager.SetRulerCalibrated();
				}
			};

			messagePrompt.Show();
		}
	}
}