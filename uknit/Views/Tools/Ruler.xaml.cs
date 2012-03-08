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

namespace uknit.Views.Tools
{
	public partial class Ruler : PhoneApplicationPage
	{
		public Ruler()
		{
			InitializeComponent();
			
		}

		private void OnClick_Settings(object sender, EventArgs e)
		{
			NavigationService.Navigate(new Uri("/Views/Settings.xaml", UriKind.Absolute));
		}
	}
}