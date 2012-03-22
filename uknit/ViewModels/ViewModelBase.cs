using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using uknit.Models;

namespace uknit.ViewModels
{
	public class ViewModelBase : INotifyPropertyChanged
	{
		#region Properties

		public ApplicationSettingsManager AppSettings = new ApplicationSettingsManager();

		private Brush BackgroundBrushProperty;
		public Brush BackgroundBrush
		{
			get
			{
				return this.BackgroundBrushProperty;
			}
			set
			{
				this.BackgroundBrushProperty = value;
				NotifyPropertyChanged("BackgroundBrush");
			}
		}

		#region Constructors

		public ViewModelBase()
		{
			this.BackgroundBrushProperty = AppSettings.GetPageBackgroundBrush();
		}

		#endregion

		#region Type specific methods

		public bool IsBackgroundEnabled()
		{
			return this.AppSettings.IsBackgroundEnabled();
		}

		public virtual void Update()
		{
			this.BackgroundBrush = AppSettings.GetPageBackgroundBrush();
		}

		#endregion

		#endregion

		#region INotifyPropertyChanged Implementation

		public event PropertyChangedEventHandler PropertyChanged;
		internal void NotifyPropertyChanged(String propertyName)
		{
			if(null != PropertyChanged)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion
	}
}
