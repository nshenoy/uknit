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

namespace uknit.Views.Tools
{
	public partial class Ruler : PhoneApplicationPage
	{
		public enum Tick
		{
			Eighth,
			Quarter,
			Half,
			Inch
		};

		public Ruler()
		{
			InitializeComponent();

			DrawRuler(App.Current.Host.Content.ActualHeight);
		}

		private void OnClick_Settings(object sender, EventArgs e)
		{
			NavigationService.Navigate(new Uri("/Views/Settings.xaml", UriKind.Absolute));
		}

		private void DrawRuler(double rulerLengthInPixels)
		{
			int ppi = 262;
			int pixelsBetweenLines = (262 - 9) / 9;
			Tick tick = Tick.Inch;
			int sum = 0;
			int offset = 10;

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
						tick = Tick.Inch;
					}
					else if(idx % 4 == 0)
					{
						tick = Tick.Half;
					}
					else if(idx % 2 == 0)
					{
						tick = Tick.Quarter;
					}
					else
					{
						tick = Tick.Eighth;
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

						Debug.WriteLine(String.Format("Drawing line {0} at {1}", tick.ToString(), line.Y1));
						this.RulerGrid.Children.Add(line);
					}
				}
			}
		}

		private void OnClick_Bigger(object sender, RoutedEventArgs e)
		{
			var offsetCalcLines = this.RulerGrid.Children.OfType<Line>().Take(2).ToArray();
			double spacing = offsetCalcLines[1].Y1 - offsetCalcLines[0].Y1 + 1;
			double sum = offsetCalcLines[0].Y1;

			foreach(Line line in this.RulerGrid.Children.OfType<Line>().Skip(1))
			{
				sum += spacing;
				line.Y1 = sum;
				line.Y2 = sum;
			}
		}

		private void OnClick_Smaller(object sender, RoutedEventArgs e)
		{
			var offsetCalcLines = this.RulerGrid.Children.OfType<Line>().Take(2).ToArray();
			double spacing = offsetCalcLines[1].Y1 - offsetCalcLines[0].Y1 - 1;
			double sum = offsetCalcLines[0].Y1;

			foreach(Line line in this.RulerGrid.Children.OfType<Line>().Skip(1))
			{
				sum += spacing;
				line.Y1 = sum;
				line.Y2 = sum;
			}
		}
	}
}