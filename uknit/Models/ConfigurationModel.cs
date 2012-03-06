using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace uknit.Models
{
	public static class ConfigurationModel
	{
		private static readonly string DATA_PATH = "Data";
		private static readonly string IMAGES_PATH = "Images";
		private static readonly string SAVEDPROJECTS_FILENAME = "SavedProjects.xml";
		private static readonly string BACKGROUNDIMAGE_FILENAME = "Background.jpg";

		private static IsolatedStorageSettings IsolatedStorage = IsolatedStorageSettings.ApplicationSettings;
		private static IsolatedStorageFile UserStoreForApplication = IsolatedStorageFile.GetUserStoreForApplication();

		private static XElement ProjectsXml = null;

		public static void SaveBackgroundImage(Stream stream)
		{
			BitmapImage chosenPhoto = new BitmapImage();
			chosenPhoto.SetSource(stream);
			WriteableBitmap backgroundImage = new WriteableBitmap(chosenPhoto);

			string backgroundImageFile = System.IO.Path.Combine(ConfigurationModel.IMAGES_PATH, ConfigurationModel.BACKGROUNDIMAGE_FILENAME);

			if(!ConfigurationModel.UserStoreForApplication.DirectoryExists(ConfigurationModel.IMAGES_PATH))
			{
				ConfigurationModel.UserStoreForApplication.CreateDirectory(ConfigurationModel.IMAGES_PATH);
			}
			else if(ConfigurationModel.UserStoreForApplication.FileExists(backgroundImageFile))
			{
				ConfigurationModel.UserStoreForApplication.DeleteFile(backgroundImageFile);
			}

			using(IsolatedStorageFileStream ifs = ConfigurationModel.UserStoreForApplication.OpenFile(backgroundImageFile, FileMode.Create, FileAccess.ReadWrite))
			{
				System.Windows.Media.Imaging.Extensions.SaveJpeg(backgroundImage, ifs, backgroundImage.PixelWidth, backgroundImage.PixelHeight, 0, 85);
			}
		}

		public static BitmapImage GetBackgroundImage()
		{
			BitmapImage image = new BitmapImage();
			string backgroundImageFile = System.IO.Path.Combine(ConfigurationModel.IMAGES_PATH, ConfigurationModel.BACKGROUNDIMAGE_FILENAME);

			if(ConfigurationModel.UserStoreForApplication.FileExists(backgroundImageFile))
			{
				using(IsolatedStorageFileStream ifs = ConfigurationModel.UserStoreForApplication.OpenFile(backgroundImageFile, FileMode.Open, FileAccess.Read))
				{
					image.SetSource(ifs);
				}
			}
			else
			{
				image.UriSource = new Uri("Content/Images/SwirveDark.png", UriKind.Relative);
			}

			return image;
		}

		public static KnittingProject GetKnittingProjectByName(string projectName)
		{
			KnittingProject knittingProject = null;
			XElement projectXml = GetKnittingProjects();
			XElement project = projectXml.Elements("Project").Where(p => p.Element("Name").Value == projectName).FirstOrDefault();

			if(project != null)
			{
				knittingProject = new KnittingProject
					{
						ProjectName = project.Element("Name").Value,
						ProjectDescription = project.Element("Description").Value,
						CurrentRowCount = int.Parse(project.Element("CurrentRowCount").Value),
						RowCounterColor = HexString2Color(project.Element("RowCounterColorRGB").Value)
					};
			}

			return knittingProject;
		}

		public static void RemoveKnittingProjectByName(string projectName)
		{
			XElement projectXml = GetKnittingProjects();
			XElement projectToRemove = projectXml.Elements("Project").Where(p => p.Element("Name").Value == projectName).First();
			projectToRemove.Remove();

			string projectPath = System.IO.Path.Combine(ConfigurationModel.DATA_PATH, ConfigurationModel.SAVEDPROJECTS_FILENAME);
			using(IsolatedStorageFileStream stream = ConfigurationModel.UserStoreForApplication.OpenFile(projectPath, FileMode.Create, FileAccess.ReadWrite))
			{
				using(StreamWriter writer = new StreamWriter(stream))
				{
					projectXml.Save(writer);
				}
			}
		}

		public static void AddKnittingProject(string projectName, KnittingProject project)
		{
			XElement projectXml = GetKnittingProjects();
			List<XElement> projects = projectXml.Elements("Project").ToList();

			if(!projects.Any() || (projects.Elements("Name").Any(n => n.Value != projectName)))
			{
				XElement newProject = new XElement("Project",
					new XElement("Name", project.ProjectName),
					new XElement("Description", project.ProjectDescription),
					new XElement("CurrentRowCount", project.CurrentRowCount),
					new XElement("RowCounterColorRGB", project.RowCounterColorRGB));

				projectXml.Add(newProject);

				string projectPath = System.IO.Path.Combine(ConfigurationModel.DATA_PATH, ConfigurationModel.SAVEDPROJECTS_FILENAME);
				using(IsolatedStorageFileStream stream = ConfigurationModel.UserStoreForApplication.OpenFile(projectPath, FileMode.Create, FileAccess.ReadWrite))
				{
					using(StreamWriter writer = new StreamWriter(stream))
					{
						projectXml.Save(writer);
					}
				}
			}
		}

		public static void ModifyKnittingProjectByName(string projectName, KnittingProject project)
		{
			XElement projectXml = GetKnittingProjects();
			XElement projectToModify = projectXml.Elements("Project").Where(p => p.Element("Name").Value == projectName).First();

			projectToModify.Element("Name").Value = project.ProjectName;
			projectToModify.Element("Description").Value = project.ProjectDescription;
			projectToModify.Element("CurrentRowCount").Value = project.CurrentRowCount.ToString();
			projectToModify.Element("RowCounterColorRGB").Value = project.RowCounterColorRGB;


			string projectPath = System.IO.Path.Combine(ConfigurationModel.DATA_PATH, ConfigurationModel.SAVEDPROJECTS_FILENAME);
			using(IsolatedStorageFileStream stream = ConfigurationModel.UserStoreForApplication.OpenFile(projectPath, FileMode.Create, FileAccess.ReadWrite))
			{
				using(StreamWriter writer = new StreamWriter(stream))
				{
					projectXml.Save(writer);
				}
			}
		}

		private static void LoadKnittingProjects()
		{
			if(ConfigurationModel.ProjectsXml == null)
			{

			}
		}

		private static XElement GetKnittingProjects()
		{
			XElement projects = null;
			string projectPath = System.IO.Path.Combine(ConfigurationModel.DATA_PATH, ConfigurationModel.SAVEDPROJECTS_FILENAME);

			if(ConfigurationModel.UserStoreForApplication.FileExists(projectPath))
			{
				using(IsolatedStorageFileStream stream = ConfigurationModel.UserStoreForApplication.OpenFile(projectPath, FileMode.Open, FileAccess.Read))
				{
					using(StreamReader reader = new StreamReader(stream))
					{
						projects = XElement.Load(reader);
					}
				}
			}
			else
			{
				if(!ConfigurationModel.UserStoreForApplication.DirectoryExists(ConfigurationModel.DATA_PATH))
				{
					ConfigurationModel.UserStoreForApplication.CreateDirectory(ConfigurationModel.DATA_PATH);
				}

				using(IsolatedStorageFileStream stream = ConfigurationModel.UserStoreForApplication.CreateFile(projectPath))
				{
					using(StreamWriter writer = new StreamWriter(stream))
					{
						XElement root = new XElement("SavedProjects");
						root.Save(writer);
						projects = root;
					}
				}
			}

			return projects;
		}

		private static Color HexString2Color(string hex)
		{
			byte a;
			byte r;
			byte g;
			byte b;

			a = (byte)(int.Parse(hex.Substring(1, 2), System.Globalization.NumberStyles.AllowHexSpecifier));
			r = (byte)(int.Parse(hex.Substring(3, 2), System.Globalization.NumberStyles.AllowHexSpecifier));
			g = (byte)(int.Parse(hex.Substring(5, 2), System.Globalization.NumberStyles.AllowHexSpecifier));
			b = (byte)(int.Parse(hex.Substring(7, 2), System.Globalization.NumberStyles.AllowHexSpecifier));

			return Color.FromArgb(a, r, g, b);
		}
	}
}
