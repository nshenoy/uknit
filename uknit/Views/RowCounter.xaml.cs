using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using Microsoft.Phone.Controls;
using uknit.Models;
using System.Windows.Media;

namespace uknit.Views
{
	public partial class RowCounter : PhoneApplicationPage, INotifyPropertyChanged
	{
		private bool IsNew = false;
		private int CurrentRowCount = 0;
		private string ProjectName = String.Empty;
		private KnittingProject Project;
		private ApplicationSettingsManager AppSettings = new ApplicationSettingsManager();
		private bool IsTensDigitEnabled = false;

		public string TensDigit
		{
			get
			{
				return (this.CurrentRowCount / 10).ToString();
			}

			set
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs("TensDigit"));
			}
		}

		public string OnesDigit
		{
			get
			{
				return (this.CurrentRowCount % 10).ToString();
			}

			set
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs("OnesDigit"));
			}
		}

		public RowCounter()
		{
			InitializeComponent();

			this.DataContext = this;
			this.Loaded += new RoutedEventHandler(RowCounter_Loaded);
			IsNew = true;

			if(!this.AppSettings.IsBackgroundEnabled())
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

			this.IsTensDigitEnabled = this.AppSettings.IsRowCounterTensDigitEnabled();
		}

		private void InitializePage(string projectName)
		{
			this.ProjectName = projectName;
			this.PageTitle.Text = projectName;

			this.Project = this.AppSettings.GetKnittingProjectByName(projectName);
			this.CurrentRowCount = this.Project.CurrentRowCount;
			this.RowCounterControl.Fill = this.Project.RowCounterColor;

			this.TensDigit = (this.CurrentRowCount / 10).ToString();
			this.OnesDigit = (this.CurrentRowCount % 10).ToString();
		}

		protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
		{
			this.State["ProjectName"] = this.ProjectName;
			this.AppSettings.ModifyKnittingProjectByName(this.ProjectName, this.Project);

			int index = App.ViewModel.KnittingProjects.IndexOf(this.Project);
			App.ViewModel.KnittingProjects[index] = this.Project;

			base.OnNavigatedFrom(e);
		}

		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
			IDictionary<string, string> queryString = this.NavigationContext.QueryString;
			if(queryString.ContainsKey("ProjectName"))
			{
				InitializePage(queryString["ProjectName"]);
			}
			else if(IsNew)
			{
				object val;

				if(this.State.TryGetValue("ProjectName", out val))
				{
					InitializePage(val as string);
				}
			}

			IsNew = false;

			base.OnNavigatedTo(e);
		}

		private void RowCounter_Loaded(object sender, RoutedEventArgs e)
		{
			if(this.IsTensDigitEnabled)
			{
				this.RowCounterControl.Tens.Tap += (s, gestureEventArgs) =>
					{
						IncrementRow(10);
					};
			}

			this.RowCounterControl.Ones.Tap += (s, gestureEventArgs) =>
				{
					IncrementRow(1);
				};
		}


		public void RowCounterGestureListener_Flick(object sender, FlickGestureEventArgs e)
		{
			if(e.Direction == System.Windows.Controls.Orientation.Vertical)
			{
				// Determine if this was meant to increment (up) or decrement (down)
				bool isIncrement = e.VerticalVelocity < 0;

				Point location = e.GetPosition(this.RowCounterControl.RowCounterColor);
				if(location.X >= (this.RowCounterControl.RowCounterColor.RenderTransformOrigin.X + (this.RowCounterControl.RowCounterColor.RenderSize.Width / 2)))
				{
					IncrementRow(isIncrement ? 1 : -1);
				}
				else
				{
					if(this.IsTensDigitEnabled)
					{
						IncrementRow(isIncrement ? 10 : -10);
					}
				}
			}
		}

		private void IncrementRow(int increment)
		{
			if((increment == -10 && this.CurrentRowCount < 10) || (increment == 10 && this.CurrentRowCount >= 90))
			{
				// Do nothing
				return;
			}

			this.CurrentRowCount += increment;
			if(this.CurrentRowCount < 0 || this.CurrentRowCount >= 100)
			{
				this.CurrentRowCount = 0;
			}

			this.TensDigit = (this.CurrentRowCount / 10).ToString();
			this.OnesDigit = (this.CurrentRowCount % 10).ToString();

			this.Project.CurrentRowCount = this.CurrentRowCount;
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}