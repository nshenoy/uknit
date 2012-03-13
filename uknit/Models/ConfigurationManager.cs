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
	public static class ConfigurationManager
	{
		private const string DATA_PATH = "Data";
		private const string IMAGES_PATH = "Images";
		private const string SAVEDPROJECTS_FILENAME = "SavedProjects.xml";
		private const string BACKGROUNDIMAGE_FILENAME = "Background.jpg";

		private const string EnableBackgroundImageKeyName = "EnableBackgroundImage";
		private const string DevicePixelsPerInchKeyName = "DevicePixelsPerInch";
		private const string UnitOfMeasureKeyName = "UnitOfMeasure";
		private const string RulerCalibratedKeyName = "RulerCalibrated";

		private static IsolatedStorageSettings IsolatedStorage = IsolatedStorageSettings.ApplicationSettings;
		private static IsolatedStorageFile UserStoreForApplication = IsolatedStorageFile.GetUserStoreForApplication();

		public static void SaveBackgroundImage(Stream stream)
		{
			BitmapImage chosenPhoto = new BitmapImage();
			chosenPhoto.SetSource(stream);
			WriteableBitmap backgroundImage = new WriteableBitmap(chosenPhoto);

			string backgroundImageFile = System.IO.Path.Combine(ConfigurationManager.IMAGES_PATH, ConfigurationManager.BACKGROUNDIMAGE_FILENAME);

			if(!ConfigurationManager.UserStoreForApplication.DirectoryExists(ConfigurationManager.IMAGES_PATH))
			{
				ConfigurationManager.UserStoreForApplication.CreateDirectory(ConfigurationManager.IMAGES_PATH);
			}
			else if(ConfigurationManager.UserStoreForApplication.FileExists(backgroundImageFile))
			{
				ConfigurationManager.UserStoreForApplication.DeleteFile(backgroundImageFile);
			}

			using(IsolatedStorageFileStream ifs = ConfigurationManager.UserStoreForApplication.OpenFile(backgroundImageFile, FileMode.Create, FileAccess.ReadWrite))
			{
				System.Windows.Media.Imaging.Extensions.SaveJpeg(backgroundImage, ifs, backgroundImage.PixelWidth, backgroundImage.PixelHeight, 0, 85);
			}
		}

		public static bool IsBackgroundEnabled()
		{
			bool? isBackgroundEnabled;

			if(!IsolatedStorage.TryGetValue(EnableBackgroundImageKeyName, out isBackgroundEnabled))
			{
				isBackgroundEnabled = true;
			}

			return (bool)isBackgroundEnabled;
		}

		public static BitmapImage GetBackgroundImage()
		{
			BitmapImage image = new BitmapImage();
			bool? isBackgroundEnabled;

			if(!IsolatedStorage.TryGetValue(EnableBackgroundImageKeyName, out isBackgroundEnabled))
			{
				isBackgroundEnabled = true;
			}

			if(isBackgroundEnabled == true)
			{
				string backgroundImageFile = System.IO.Path.Combine(ConfigurationManager.IMAGES_PATH, ConfigurationManager.BACKGROUNDIMAGE_FILENAME);

				if(ConfigurationManager.UserStoreForApplication.FileExists(backgroundImageFile))
				{
					using(IsolatedStorageFileStream ifs = ConfigurationManager.UserStoreForApplication.OpenFile(backgroundImageFile, FileMode.Open, FileAccess.Read))
					{
						image.SetSource(ifs);
					}
				}
				else
				{
					image.UriSource = new Uri("Content/Images/SwirveDark.jpg", UriKind.Relative);
				}
			}
			else
			{
				//image.UriSource = new Uri("Content/Images/PanoramaBackground.jpg", UriKind.Relative);
				image = null;
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

			string projectPath = System.IO.Path.Combine(ConfigurationManager.DATA_PATH, ConfigurationManager.SAVEDPROJECTS_FILENAME);
			using(IsolatedStorageFileStream stream = ConfigurationManager.UserStoreForApplication.OpenFile(projectPath, FileMode.Create, FileAccess.ReadWrite))
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

				string projectPath = System.IO.Path.Combine(ConfigurationManager.DATA_PATH, ConfigurationManager.SAVEDPROJECTS_FILENAME);
				using(IsolatedStorageFileStream stream = ConfigurationManager.UserStoreForApplication.OpenFile(projectPath, FileMode.Create, FileAccess.ReadWrite))
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


			string projectPath = System.IO.Path.Combine(ConfigurationManager.DATA_PATH, ConfigurationManager.SAVEDPROJECTS_FILENAME);
			using(IsolatedStorageFileStream stream = ConfigurationManager.UserStoreForApplication.OpenFile(projectPath, FileMode.Create, FileAccess.ReadWrite))
			{
				using(StreamWriter writer = new StreamWriter(stream))
				{
					projectXml.Save(writer);
				}
			}
		}

		public static List<KnittingProject> LoadKnittingProjects()
		{
			List<KnittingProject> projects = new List<KnittingProject>();
			XElement projectsXml = GetKnittingProjects();

			if(projectsXml != null)
			{
				projects = projectsXml.Elements("Project").Select(p => new KnittingProject
				{
					ProjectName = p.Element("Name").Value,
					ProjectDescription = p.Element("Description").Value,
					RowCounterColor = HexString2Color(p.Element("RowCounterColorRGB").Value),
					CurrentRowCount = int.Parse(p.Element("CurrentRowCount").Value)
				}).ToList();
			}

			return projects;
		}

		private static XElement GetKnittingProjects()
		{
			XElement projects = null;
			string projectPath = System.IO.Path.Combine(ConfigurationManager.DATA_PATH, ConfigurationManager.SAVEDPROJECTS_FILENAME);

			if(ConfigurationManager.UserStoreForApplication.FileExists(projectPath))
			{
				using(IsolatedStorageFileStream stream = ConfigurationManager.UserStoreForApplication.OpenFile(projectPath, FileMode.Open, FileAccess.Read))
				{
					using(StreamReader reader = new StreamReader(stream))
					{
						projects = XElement.Load(reader);
					}
				}
			}
			else
			{
				if(!ConfigurationManager.UserStoreForApplication.DirectoryExists(ConfigurationManager.DATA_PATH))
				{
					ConfigurationManager.UserStoreForApplication.CreateDirectory(ConfigurationManager.DATA_PATH);
				}

				using(IsolatedStorageFileStream stream = ConfigurationManager.UserStoreForApplication.CreateFile(projectPath))
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

		public static bool IsRulerCalibrated()
		{
			return IsolatedStorage.Contains(RulerCalibratedKeyName);
		}

		public static void SetRulerCalibrated()
		{
			if(!ConfigurationManager.IsRulerCalibrated())
			{
				IsolatedStorage[RulerCalibratedKeyName] = true;
			}
		}
	
		public static string GetUnitOfMeasure()
		{
			string unitOfMeasure = "Imperial";
			if(IsolatedStorage.Contains(UnitOfMeasureKeyName))
			{
				unitOfMeasure = IsolatedStorage[UnitOfMeasureKeyName] as string;
			}
			else
			{
				SetUnitOfMeasure(unitOfMeasure);
			}

			return unitOfMeasure;
		}

		public static void SetUnitOfMeasure(string unitOfMeasure)
		{
			IsolatedStorage[UnitOfMeasureKeyName] = unitOfMeasure;
		}

		public static double GetDevicePixelDensity()
		{
			double ppd;

			if(ConfigurationManager.GetUnitOfMeasure() == "Imperial")
			{
				ppd = ConfigurationManager.GetDevicePixelsPerInch();
			}
			else
			{
				ppd = ConfigurationManager.GetDevicePixelsPerCentimeter();
			}

			return ppd;
		}

		public static void SetDevicePixelDensity(double pixelsBetweenLines)
		{
			if(ConfigurationManager.GetUnitOfMeasure() == "Imperial")
			{
				ConfigurationManager.SetDevicePixelsPerInch(pixelsBetweenLines);
			}
			else
			{
				ConfigurationManager.SetDevicePixelsPerCentimeter(pixelsBetweenLines);
			}
		}

		public static double GetDevicePixelsPerInch()
		{
			double ppi;
			if(!IsolatedStorage.TryGetValue(DevicePixelsPerInchKeyName, out ppi))
			{
				ppi = 262;
				IsolatedStorage[DevicePixelsPerInchKeyName] = ppi;
			}

			return ppi;
		}

		public static void SetDevicePixelsPerInch(double pixelsBetweenLines)
		{
			double ppi = pixelsBetweenLines * 9 + 9;

			IsolatedStorage[DevicePixelsPerInchKeyName] = ppi;
		}

		public static double GetDevicePixelsPerCentimeter()
		{
			double ppi = ConfigurationManager.GetDevicePixelsPerInch();

			return (ppi / 2.54);
		}

		public static void SetDevicePixelsPerCentimeter(double pixelsBetweenLines)
		{
			double ppc = pixelsBetweenLines * 11 + 11;

			IsolatedStorage[DevicePixelsPerInchKeyName] = ppc * 2.54;
		}

		public static void RestoreDefaults()
		{
			IsolatedStorage.Remove(DevicePixelsPerInchKeyName);
			IsolatedStorage.Remove(UnitOfMeasureKeyName);
			IsolatedStorage.Remove(RulerCalibratedKeyName);
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
