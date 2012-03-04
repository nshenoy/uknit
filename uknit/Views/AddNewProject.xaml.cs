using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using uknit.Models;
using uknit.ViewModels;
using System.Windows;

namespace uknit.Views
{
	public partial class AddNewProject : PhoneApplicationPage
	{
		private bool IsNew = false;
		private bool IsEditProject = false;
		private IsolatedStorageSettings IsolatedStorage = IsolatedStorageSettings.ApplicationSettings;
		private string OriginalProjectName = String.Empty;
		private AddNewProjectViewModel Project = null;

		public AddNewProject()
		{
			InitializeComponent();
			IsNew = true;
		}

		protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
		{
			this.State["AddProject_Name.Text"] = this.AddProject_Name.Text;
			this.State["AddProject_Description.Text"] = this.AddProject_Description.Text;
			this.State["AddProject_RowCounterColor.Color"] = this.AddProject_RowCounterColor.Color;
			base.OnNavigatedFrom(e);
		}

		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
			IDictionary<string, string> queryString = this.NavigationContext.QueryString;
			if(queryString.ContainsKey("ProjectName"))
			{
				this.PageTitle.Text = "edit project";
				SetPageValues(queryString["ProjectName"]);
				this.IsEditProject = true;
			}
			else if(IsNew)
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
			bool successfullySaved = false;

			if(this.IsEditProject)
			{
				Project.EditProject(this.AddProject_Name.Text, this.AddProject_Description.Text, this.AddProject_RowCounterColor.Color);
				successfullySaved = true;
			}
			else
			{
				Project = new AddNewProjectViewModel()
				{
					ProjectName = this.AddProject_Name.Text,
					ProjectDescription = this.AddProject_Description.Text,
					RowCounterColor = this.AddProject_RowCounterColor.Color
				};

				try
				{
					Project.CreateProject();
					successfullySaved = true;
				}
				catch
				{
					MessageBox.Show(String.Format("Sorry, it looks like a project named \'{0}\' already exists. Please try a different name.", this.AddProject_Name.Text));
				}
			}

			if(successfullySaved)
			{
				NavigationService.Navigate(new Uri("/MainPage.xaml?AddEditProject=1", UriKind.Relative));
			}
		}

		private void SetPageValues(string projectName)
		{
			Project = new AddNewProjectViewModel(projectName);

			this.OriginalProjectName = projectName;

			this.AddProject_Name.Text = this.Project.ProjectName;
			this.AddProject_Description.Text = this.Project.ProjectDescription;
			this.AddProject_RowCounterColor.Color = this.Project.RowCounterColor;
		}

		private void SetPageValues(bool restore)
		{
			if(restore)
			{
				object val;

				if(this.State.TryGetValue("AddProject_Name.Text", out val))
				{
					this.AddProject_Name.Text = val as string;
				}

				if(this.State.TryGetValue("AddProject_Description.Text", out val))
				{
					this.AddProject_Description.Text = val as string;
				}

				if(this.State.TryGetValue("AddProject_RowCounterColor.Color", out val))
				{
					this.AddProject_RowCounterColor.Color = (Color)val;
				}
			}
			else
			{
				this.AddProject_Name.Text = String.Empty;
				this.AddProject_Description.Text = String.Empty;
			}
		}
	}
}