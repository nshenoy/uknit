using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.IsolatedStorage;

namespace uknit
{
	public class MainViewModel : INotifyPropertyChanged
	{
		public MainViewModel()
		{
			this.KnittingProjects = new ObservableCollection<KnittingProjectViewModel>();
		}

		/// <summary>
		/// A collection for KnittingProjectViewModel objects.
		/// </summary>
		public ObservableCollection<KnittingProjectViewModel> KnittingProjects
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
			IsolatedStorageSettings isolatedStorage = IsolatedStorageSettings.ApplicationSettings;

			foreach(var oProj in isolatedStorage.Values)
			{
				KnittingProjectViewModel proj = oProj as KnittingProjectViewModel;
				if(proj != null)
				{
					this.KnittingProjects.Add(proj);
				}
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