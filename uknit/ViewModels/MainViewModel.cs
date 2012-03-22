using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using uknit.Models;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;

namespace uknit.ViewModels
{
	public class MainViewModel : INotifyPropertyChanged
	{
		private ApplicationSettingsManager AppSettings = new ApplicationSettingsManager();

		public MainViewModel()
		{
			this.KnittingProjects = new ObservableCollection<KnittingProject>();
		}

		/// <summary>
		/// A collection for KnittingProjectViewModel objects.
		/// </summary>
		public ObservableCollection<KnittingProject> KnittingProjects
		{
			get;
			private set;
		}

		private Brush PanoramaBackgroundBrushProperty;
		public Brush PanoramaBackgroundBrush
		{
			get
			{
				this.PanoramaBackgroundBrushProperty = this.AppSettings.GetPanoramaBackgroundBrush();
				return PanoramaBackgroundBrushProperty;
			}
			set
			{
				this.PanoramaBackgroundBrushProperty = value;
				NotifyPropertyChanged("PanoramaBackgroundBrush");
			}
		}

		public bool IsDataLoaded
		{
			get;
			private set;
		}

		/// <summary>
		/// Populates main project list from isolated storage
		/// </summary>
		public void LoadData()
		{
			List<KnittingProject> projects = this.AppSettings.LoadKnittingProjects();

			foreach(KnittingProject project in projects)
			{
				this.KnittingProjects.Add(project);
			}

			this.IsDataLoaded = true;
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