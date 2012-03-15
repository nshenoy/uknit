using System;
using System.Windows.Media;
using uknit.Models;

namespace uknit.ViewModels
{
	public class AddNewProjectViewModel
	{
		public ConfigurationManager AppSettings = new ConfigurationManager();

		public string ProjectName
		{
			get;
			set;
		}

		public string ProjectDescription
		{
			get;
			set;
		}

		public Color RowCounterColor
		{
			get;
			set;
		}

		public AddNewProjectViewModel()
		{
		}

		public AddNewProjectViewModel(string projectName)
		{
			this.ProjectName = projectName;

			KnittingProject knittingProject = this.AppSettings.GetKnittingProjectByName(projectName);

			this.ProjectDescription = knittingProject.ProjectDescription;
			this.RowCounterColor = knittingProject.RowCounterColor;
		}

		public void CreateProject()
		{
			KnittingProject knittingProject = new KnittingProject
			{
				ProjectName = this.ProjectName,
				ProjectDescription = this.ProjectDescription,
				RowCounterColor = this.RowCounterColor,
				CurrentRowCount = 0
			};

			if(this.AppSettings.GetKnittingProjectByName(this.ProjectName) == null)
			{
				App.ViewModel.KnittingProjects.Insert(0, knittingProject);
				this.AppSettings.AddKnittingProject(this.ProjectName, knittingProject);
			}
		}

		public void EditProject(string projectName, string description, Color color)
		{
			KnittingProject project = this.AppSettings.GetKnittingProjectByName(this.ProjectName);
			int index = App.ViewModel.KnittingProjects.IndexOf(project);

			project.ProjectDescription = description;
			project.RowCounterColor = color;

			if(String.Compare(this.ProjectName, projectName) != 0)
			{
				this.AppSettings.RemoveKnittingProjectByName(this.ProjectName);

				this.ProjectName = projectName;
				project.ProjectName = projectName;

				this.AppSettings.AddKnittingProject(projectName, project);
			}
			else
			{
				this.AppSettings.ModifyKnittingProjectByName(projectName, project);
			}

			App.ViewModel.KnittingProjects[index] = project;
		}
	}
}
