using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Xml.Serialization;
using uknit.Helpers.Converters;

namespace uknit.Models
{
	public class KnittingProject : INotifyPropertyChanged, IComparable
	{
		private string _projectName;

		[XmlElement("Name")]
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

		[XmlElement("Description")]
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
				_rowCounterColor = value;
				NotifyPropertyChanged("RowCounterColor");
			}
		}

		[XmlElement("RowCounterColorRGB")]
		public string RowCounterColorRGB
		{
			get
			{
				return _rowCounterColor.ToString();
			}
			set
			{
				RowCounterColor = StringToColorConverter.HexString2Color(value);
			}
		}

		private int _currentRowCount;

		[XmlElement("CurrentRowCount")]
		public int CurrentRowCount
		{
			get
			{
				return _currentRowCount;
			}
			set
			{
				_currentRowCount = value;
				NotifyPropertyChanged("CurrentRowCount");
			}
		}

		private bool _isPinnedToStart;

		[XmlElement("IsPinnedToStart")]
		public bool IsPinnedToStart
		{
			get
			{
				return _isPinnedToStart;
			}
			set
			{
				_isPinnedToStart = value;
				NotifyPropertyChanged("IsPinnedToStart");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(String propertyName)
		{
			if(null != PropertyChanged)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public int CompareTo(object obj)
		{
			KnittingProject compareAgainst = obj as KnittingProject;
			return this.ProjectName.CompareTo(compareAgainst.ProjectName);
		}

		public override bool Equals(object obj)
		{
			if(obj == null)
			{
				return base.Equals(obj);
			}
			KnittingProject compareAgainst = obj as KnittingProject;
			return String.Compare(this.ProjectName, compareAgainst.ProjectName) == 0;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}