using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using uknit.Models;

namespace uknit.ViewModels
{
	public class ProjectViewModel
	{
		private ApplicationSettingsManager AppSettings = new ApplicationSettingsManager();

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

		public ProjectViewModel()
		{
		}

		public ProjectViewModel(string projectName)
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

				if(project.IsPinnedToStart)
				{
					WriteableBitmap tileBitmap = this.CreateRowCounterBitmap(project);

					this.AppSettings.UpdateTile(tileBitmap, this.ProjectName);
				}
			}

			App.ViewModel.KnittingProjects[index] = project;
		}

		public WriteableBitmap CreateRowCounterBitmap(KnittingProject project)
		{
			uknit.Controls.RowCounterControl rowCounter = new uknit.Controls.RowCounterControl()
			{
				TensDigit = (project.CurrentRowCount / 10).ToString(),
				OnesDigit = (project.CurrentRowCount % 10).ToString(),
				Fill = project.RowCounterColor,
				NeedleWidth = 100,
				HorizontalAlignment = System.Windows.HorizontalAlignment.Center
			};

			rowCounter.Measure(new Size(400, 400));
			rowCounter.Arrange(new Rect(0, 0, 400, 400));

			WriteableBitmap tileBitmap = new WriteableBitmap(400, 400);
			tileBitmap.Render(rowCounter, null);
			tileBitmap.Invalidate();

			return tileBitmap;
		}
	}
}
