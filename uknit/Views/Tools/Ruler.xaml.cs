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
        private ApplicationSettingsManager AppSettings = new ApplicationSettingsManager();

        public enum ImperialTick
        {
            None = 0,
            Eighth = 1,
            Quarter = 2,
            Half = 3,
            Inch = 4
        }

        public enum MetricTick
        {
            None = 0,
            Millimeter = 1,
            Half = 2,
            Centimeter = 3
        }

        private string currentRulerUnitOfMeasure;

        public Ruler()
        {
            InitializeComponent();

            if (!this.AppSettings.IsBackgroundEnabled())
            {
                this.LayoutRoot.Background = null;
            }
            else
            {
                Visibility isLightTheme = (Visibility)Resources["PhoneLightThemeVisibility"];
                if (isLightTheme == Visibility.Visible)
                {
                    this.ApplicationTitle.Foreground = new SolidColorBrush(Colors.White);
                    this.PageTitle.Foreground = new SolidColorBrush(Colors.White);
                }
            }

            currentRulerUnitOfMeasure = this.AppSettings.GetUnitOfMeasure();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            DrawRuler();

            base.OnNavigatedTo(e);

            if (!this.AppSettings.IsRulerCalibrated())
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
            if (this.AppSettings.GetUnitOfMeasure() == "Inches")
            {
                this.Inches.IsChecked = true;
                DrawRulerImperial(App.Current.Host.Content.ActualHeight);
            }
            else
            {
                this.Centimeters.IsChecked = true;
                DrawRulerMetric(App.Current.Host.Content.ActualHeight);
            }
        }

        private void DrawRulerImperial(double rulerLengthInPixels)
        {
            int pixelDensity = (int)this.AppSettings.GetDevicePixelDensity();

            int rulerPixelPosition = 10;
            int currentInch = 0;

            while (rulerPixelPosition < rulerLengthInPixels)
            {
                DrawRulerImperialMark(rulerPixelPosition, ImperialTick.Inch, currentInch.ToString());
                DrawRulerImperialMeasure(rulerPixelPosition, rulerPixelPosition + pixelDensity, ImperialTick.Half);

                currentInch++;
                rulerPixelPosition += pixelDensity;
            }
        }

        private void DrawRulerImperialMeasure(int start, int end, ImperialTick tick)
        {
            if (tick == ImperialTick.None)
            {
                return;
            }

            int midPoint = (start + end) / 2;

            DrawRulerImperialMeasure(start, midPoint, tick - 1);
            DrawRulerImperialMark(midPoint, tick);
            DrawRulerImperialMeasure(midPoint, end, tick - 1);
        }

        private void DrawRulerImperialMark(int position, ImperialTick tick, string label = "")
        {
            Line line = new Line()
            {
                Style = (Style)this.LayoutRoot.Resources[tick.ToString()],
                Y1 = position,
                Y2 = position
            };

            if (tick == ImperialTick.Inch)
            {
                line.Name = "tick_inch_" + label;
                TextBlock textBlock = new TextBlock()
                {
                    Name = "label_" + line.Name,
                    Text = label,
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

        private void DrawRulerMetric(double rulerLengthInPixels)
        {
            int pixelDensity = (int)this.AppSettings.GetDevicePixelDensity();

            int rulerPixelPosition = 10;
            int currentCm = 0;

            while (rulerPixelPosition < rulerLengthInPixels)
            {
                DrawRulerMetricMark(rulerPixelPosition, MetricTick.Centimeter, currentCm.ToString());
                DrawRulerMetricMeasure(rulerPixelPosition, rulerPixelPosition + pixelDensity, MetricTick.Half);

                currentCm++;
                rulerPixelPosition += pixelDensity;
            }
        }

        private void DrawRulerMetricMeasure(int start, int end, MetricTick tick)
        {
            if (tick == MetricTick.Millimeter)
            {
                int mmDivision = (end - start) / 5;

                for (int i = 1; i < 5; i++)
                {
                    DrawRulerMetricMark(start + (mmDivision * i), tick);
                }

                return;
            }

            int midPoint = (start + end) / 2;

            DrawRulerMetricMeasure(start, midPoint, tick - 1);
            DrawRulerMetricMark(midPoint, tick);
            DrawRulerMetricMeasure(midPoint, end, tick - 1);
        }

        private void DrawRulerMetricMark(int position, MetricTick tick, string label = "")
        {
            Line line = new Line()
            {
                Style = (Style)this.LayoutRoot.Resources[tick.ToString()],
                Y1 = position,
                Y2 = position
            };

            if (tick == MetricTick.Centimeter)
            {
                line.Name = "tick_cm_" + label;
                TextBlock textBlock = new TextBlock()
                {
                    Name = "label_" + line.Name,
                    Text = label,
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

        private void OnClick_Bigger(object sender, RoutedEventArgs e)
        {
            double pixelsPerInch = this.AppSettings.GetDevicePixelDensity();
            this.AppSettings.SetDevicePixelDensity(pixelsPerInch + 1);
            Debug.WriteLine("PPI is now {0}", this.AppSettings.GetDevicePixelDensity());

            DrawRuler();
        }

        private void OnClick_Smaller(object sender, RoutedEventArgs e)
        {
            double pixelsPerInch = this.AppSettings.GetDevicePixelDensity();
            this.AppSettings.SetDevicePixelDensity(pixelsPerInch - 1);
            Debug.WriteLine("PPI is now {0}", this.AppSettings.GetDevicePixelDensity());

            DrawRuler();
        }

        private void UnitOfMeasure_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            this.AppSettings.SetUnitOfMeasure(rb.Name);
            this.RulerSettingsText.Text = rb.Name;
            if (rb.Name != currentRulerUnitOfMeasure)
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
                Text = "I noticed that you haven't calibrated the ruler yet. This is important to make sure the ruler is somewhat accurate. Afterall, you don't want to make a baby hat that fits Andre the Giant. Would you like to calibrate now?",
                FontSize = 20.0,
                TextWrapping = TextWrapping.Wrap
            };

            messagePrompt.Completed += (str, res) =>
            {
                if (res.PopUpResult == Coding4Fun.Phone.Controls.PopUpResult.Ok)
                {
                    this.RulerSettingsSwitch.IsChecked = true;
                    this.CalibrationSwitch.IsChecked = true;
                    this.AppSettings.SetRulerCalibrated();
                }
            };

            messagePrompt.Show();
        }
    }
}