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
using System.ComponentModel;
using System.IO.IsolatedStorage;

namespace uknit.Views
{
	public partial class RowCounter : PhoneApplicationPage, INotifyPropertyChanged
	{
		private bool IsNew = false;
		private int CurrentRowCount = 0;
		private IsolatedStorageSettings IsolatedStorage = IsolatedStorageSettings.ApplicationSettings;
		private string ProjectName = String.Empty;
		KnittingProjectViewModel KnittingProject;

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
		}

		private void InitializePage(string projectName)
		{
			this.ProjectName = projectName;
			this.KnittingProject = this.IsolatedStorage[projectName] as KnittingProjectViewModel;
			this.CurrentRowCount = this.KnittingProject.CurrentRowCount;
			this.RowCounterClicker.Fill = new SolidColorBrush(this.KnittingProject.RowCounterColor);

			this.TensDigit = (this.CurrentRowCount / 10).ToString();
			this.OnesDigit = (this.CurrentRowCount % 10).ToString();
		}

		protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
		{
			this.State["ProjectName"] = this.ProjectName;
			this.IsolatedStorage[this.ProjectName] = this.KnittingProject;

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
			this.Tens.DoubleTap += (s, gestureEventArgs) =>
				{
					IncrementRow(10);
				};

			this.Ones.DoubleTap += (s, gestureEventArgs) =>
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

				Point location = e.GetPosition(this.RowCounterClicker);
				if(location.X >= (this.RowCounterClicker.RenderTransformOrigin.X + (this.RowCounterClicker.RenderSize.Width / 2)))
				{
					IncrementRow(isIncrement ? 1 : -1);
				}
				else
				{
					IncrementRow(isIncrement ? 10 : -10);
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

			this.KnittingProject.CurrentRowCount = this.CurrentRowCount;
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}