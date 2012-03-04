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
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
using System.IO;

namespace uknit.Models
{
	public static class ConfigurationModel
	{
		private static IsolatedStorageSettings IsolatedStorage = IsolatedStorageSettings.ApplicationSettings;

		public static void SaveBackgroundImage(Stream stream)
		{
			BitmapImage chosenPhoto = new BitmapImage();
			chosenPhoto.SetSource(stream);
			ConfigurationModel.IsolatedStorage["MainPagePanoramaBackgroundImage"] = chosenPhoto;
		}

		public static KnittingProject GetKnittingProjectByName(string projectName)
		{
			KnittingProject knittingProject = null;

			if(ConfigurationModel.IsolatedStorage.Contains(projectName))
			{
				knittingProject = ConfigurationModel.IsolatedStorage[projectName] as KnittingProject;
			}

			return knittingProject;			
		}

		public static void RemoveKnittingProjectByName(string projectName)
		{
			ConfigurationModel.IsolatedStorage.Remove(projectName);
		}

		public static void AddKnittingProject(string projectName, KnittingProject project)
		{
			ConfigurationModel.IsolatedStorage.Add(projectName, project);
		}

		public static void ModifyKnittingProjectByName(string projectName, KnittingProject project)
		{
			ConfigurationModel.IsolatedStorage[projectName] = projectName;
		}
	}
}
