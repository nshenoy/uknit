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
using System.Windows;
using uknit.Models;

namespace uknit.ViewModels
{
	public class ThemeViewModel : INotifyPropertyChanged
	{
		public ConfigurationManager AppSettings = new ConfigurationManager();

		private Brush TextForegroundProperty;
		public Brush TextForeground
		{
			get
			{
				if(this.AppSettings.IsBackgroundEnabled())
				{
					Visibility isLightTheme = (Visibility)App.Current.Resources["PhoneLightThemeVisibility"];
					if(isLightTheme == Visibility.Visible)
					{
						this.TextForegroundProperty = new SolidColorBrush(Colors.White);
						return this.TextForegroundProperty;
					}
				}
			}
			set;
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
