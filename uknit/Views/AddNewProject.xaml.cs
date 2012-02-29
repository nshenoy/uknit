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
using Coding4Fun.Phone.Controls;
using System.IO.IsolatedStorage;

namespace uknit.Views
{
	public partial class AddNewProject : PhoneApplicationPage
	{
		private bool IsNew = false;
		private IsolatedStorageSettings IsolatedStorage = IsolatedStorageSettings.ApplicationSettings;

		public AddNewProject()
		{
			InitializeComponent();
			//SetPageValues(true);
			IsNew = true;
		}

		protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
		{
			State["AddProject_Name.Text"] = AddProject_Name.Text;
			State["AddProject_Description.Text"] = AddProject_Description.Text;
			State["AddProject_RowCounterColor.Color"] = AddProject_RowCounterColor.Color;
			base.OnNavigatedFrom(e);
		}

		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
			if(IsNew)
			{
				SetPageValues(true);
			}

			IsNew = false;

			base.OnNavigatedTo(e);
		}

		private void OnCancelProject(object sender, EventArgs e)
		{
			SetPageValues(false);
			NavigationService.GoBack();
		}

		private void OnSaveProject(object sender, EventArgs e)
		{
			// Persist values to isolated storage

			if(!IsolatedStorage.Contains(AddProject_Name.Text))
			{
				KnittingProjectViewModel proj = new KnittingProjectViewModel
				{
					ProjectName = AddProject_Name.Text,
					ProjectDescription = AddProject_Description.Text,
					RowCounterColor = AddProject_RowCounterColor.Color,
					CurrentRowCount = 0
				};

				App.ViewModel.Items.Add(proj);

				IsolatedStorage.Add(proj.ProjectName, new KnittingProjectViewModel
				{
					ProjectName = AddProject_Name.Text,
					ProjectDescription = AddProject_Description.Text,
					RowCounterColor = AddProject_RowCounterColor.Color,
					CurrentRowCount = 0
				});
			}

			NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
		}

		private void SetPageValues(bool restore)
		{
			if(restore)
			{
				object val;

				if(State.TryGetValue("AddProject_Name.Text", out val))
				{
					AddProject_Name.Text = val as string;
				}

				if(State.TryGetValue("AddProject_Description.Text", out val))
				{
					AddProject_Description.Text = val as string;
				}

				if(State.TryGetValue("AddProject_RowCounterColor.Color", out val))
				{
					AddProject_RowCounterColor.Color = (Color)val;
				}
			}
			else
			{
				AddProject_Name.Text = String.Empty;
				AddProject_Description.Text = String.Empty;
			}
		}
	}
}