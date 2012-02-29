using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace uknit
{
    public class KnittingProjectViewModel : INotifyPropertyChanged 
    {
        private string _projectName;
        public string ProjectName 
        {
            get 
            {
                return _projectName;
            }
            set 
            {
                _projectName = value;
                NotifyPropertyChanged("ProjectName");
            }
        }
        
        private string _projectDescription;
		public string ProjectDescription
        {
            get
            {
                return _projectDescription;
            }
            set
            {
                _projectDescription = value;
                NotifyPropertyChanged("ProjectDescription");
            }
        }

		private Color _rowCounterColor;
		public Color RowCounterColor
		{
			get
			{
				return _rowCounterColor;
			}
			set
			{
				NotifyPropertyChanged("RowCounterColor");
			}
		}

		private int _currentRowCount;
		public int CurrentRowCount
		{
			get
			{
				return _currentRowCount;
			}
			set
			{
				NotifyPropertyChanged("CurrentRowCount");
			}
		}
		
		public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName) 
        {
            if (null != PropertyChanged) 
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}