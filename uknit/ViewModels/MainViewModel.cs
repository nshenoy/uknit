using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;

namespace uknit
{
	public class MainViewModel : INotifyPropertyChanged
	{
		public MainViewModel()
		{
			this.Items = new ObservableCollection<KnittingProjectViewModel>();
		}

		/// <summary>
		/// A collection for ItemViewModel objects.
		/// </summary>
		//public ObservableCollection<ItemViewModel> Items
		public ObservableCollection<KnittingProjectViewModel> Items
		{
			get;
			private set;
		}

		private string _sampleProperty = "Sample Runtime Property Value";
		/// <summary>
		/// Sample ViewModel property; this property is used in the view to display its value using a Binding
		/// </summary>
		/// <returns></returns>
		public string SampleProperty
		{
			get
			{
				return _sampleProperty;
			}
			set
			{
				_sampleProperty = value;
				NotifyPropertyChanged("SampleProperty");
			}
		}

		public bool IsDataLoaded
		{
			get;
			private set;
		}

		/// <summary>
		/// Creates and adds a few ItemViewModel objects into the Items collection.
		/// </summary>
		public void LoadData()
		{
			IsolatedStorageSettings isolatedStorage = IsolatedStorageSettings.ApplicationSettings;

			foreach(var oProj in isolatedStorage.Values)
			{
				KnittingProjectViewModel proj = oProj as KnittingProjectViewModel;
				if(proj != null)
				{
					this.Items.Add(proj);
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