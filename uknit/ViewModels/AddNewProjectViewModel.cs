using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
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

			KnittingProject knittingProject = uknit.Models.ConfigurationModel.GetKnittingProjectByName(projectName);

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

			if(uknit.Models.ConfigurationModel.GetKnittingProjectByName(this.ProjectName) == null)
			{
				App.ViewModel.KnittingProjects.Insert(0, knittingProject);
				uknit.Models.ConfigurationModel.AddKnittingProject(this.ProjectName, knittingProject);
			}
		}

		public void EditProject(string projectName, string description, Color color)
		{
			KnittingProject project = uknit.Models.ConfigurationModel.GetKnittingProjectByName(this.ProjectName);
			int index = App.ViewModel.KnittingProjects.IndexOf(project);

			project.ProjectDescription = description;
			project.RowCounterColor = color;

			if(String.Compare(this.ProjectName, projectName) != 0)
			{
				uknit.Models.ConfigurationModel.RemoveKnittingProjectByName(this.ProjectName);

				this.ProjectName = projectName;
				project.ProjectName = projectName;

				uknit.Models.ConfigurationModel.AddKnittingProject(projectName, project);
			}
			else
			{
				uknit.Models.ConfigurationModel.ModifyKnittingProjectByName(projectName, project);
			}

			App.ViewModel.KnittingProjects[index] = project;
		}
	}
}
