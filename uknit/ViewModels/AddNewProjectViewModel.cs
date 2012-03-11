using System;
using System.Windows.Media;
using uknit.Models;

namespace uknit.ViewModels
{
	public class AddNewProjectViewModel
	{
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

			KnittingProject knittingProject = uknit.Models.ConfigurationManager.GetKnittingProjectByName(projectName);

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

			if(uknit.Models.ConfigurationManager.GetKnittingProjectByName(this.ProjectName) == null)
			{
				App.ViewModel.KnittingProjects.Insert(0, knittingProject);
				uknit.Models.ConfigurationManager.AddKnittingProject(this.ProjectName, knittingProject);
			}
		}

		public void EditProject(string projectName, string description, Color color)
		{
			KnittingProject project = uknit.Models.ConfigurationManager.GetKnittingProjectByName(this.ProjectName);
			int index = App.ViewModel.KnittingProjects.IndexOf(project);

			project.ProjectDescription = description;
			project.RowCounterColor = color;

			if(String.Compare(this.ProjectName, projectName) != 0)
			{
				uknit.Models.ConfigurationManager.RemoveKnittingProjectByName(this.ProjectName);

				this.ProjectName = projectName;
				project.ProjectName = projectName;

				uknit.Models.ConfigurationManager.AddKnittingProject(projectName, project);
			}
			else
			{
				uknit.Models.ConfigurationManager.ModifyKnittingProjectByName(projectName, project);
			}

			App.ViewModel.KnittingProjects[index] = project;
		}
	}
}
