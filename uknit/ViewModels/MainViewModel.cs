using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using uknit.Models;

namespace uknit.ViewModels
{
	public class MainViewModel : INotifyPropertyChanged
	{
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
			List<KnittingProject> projects = ConfigurationModel.LoadKnittingProjects();

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