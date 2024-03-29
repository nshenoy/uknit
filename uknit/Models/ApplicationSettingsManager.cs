﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using ImageTools;
using Microsoft.Phone.Shell;
using uknit.Helpers.Converters;

namespace uknit.Models
{
	public class ApplicationSettingsManager
	{
		private const string DATA_PATH = "Data";
		private const string IMAGES_PATH = "Images";
		private const string SHAREDSHELLCONTENT_PATH = "Shared/ShellContent";
		private const string SAVEDPROJECTS_FILENAME = "SavedProjects.xml";
		private const string BACKGROUNDIMAGE_FILENAME = "Background.jpg";
		private const string DEFAULT_PANORAMA_IMAGE = "Content/Images/KnitBlueBackground.jpg";
		private const string DEFAULT_PAGE_IMAGE = "/Content/Images/KnitBackground480x800.jpg";

		private const string EnableRowCounterTensDigitKeyName = "EnableRowCounterTensDigit";
		private const string EnableBackgroundImageKeyName = "EnableBackgroundImage";
		private const string DevicePixelsPerInchKeyName = "DevicePixelsPerInch";
		private const string UnitOfMeasureKeyName = "UnitOfMeasure";
		private const string RulerCalibratedKeyName = "RulerCalibrated";

		private static IsolatedStorageSettings IsolatedStorage = IsolatedStorageSettings.ApplicationSettings;
		private static IsolatedStorageFile UserStoreForApplication = IsolatedStorageFile.GetUserStoreForApplication();

		public void SaveBackgroundImage(Stream stream)
		{
			BitmapImage chosenPhoto = new BitmapImage();
			chosenPhoto.SetSource(stream);
			WriteableBitmap backgroundImage = new WriteableBitmap(chosenPhoto);

			string backgroundImageFile = System.IO.Path.Combine(ApplicationSettingsManager.IMAGES_PATH, ApplicationSettingsManager.BACKGROUNDIMAGE_FILENAME);

			if(!ApplicationSettingsManager.UserStoreForApplication.DirectoryExists(ApplicationSettingsManager.IMAGES_PATH))
			{
				ApplicationSettingsManager.UserStoreForApplication.CreateDirectory(ApplicationSettingsManager.IMAGES_PATH);
			}
			else if(ApplicationSettingsManager.UserStoreForApplication.FileExists(backgroundImageFile))
			{
				ApplicationSettingsManager.UserStoreForApplication.DeleteFile(backgroundImageFile);
			}

			using(IsolatedStorageFileStream ifs = ApplicationSettingsManager.UserStoreForApplication.OpenFile(backgroundImageFile, FileMode.Create, FileAccess.ReadWrite))
			{
				System.Windows.Media.Imaging.Extensions.SaveJpeg(backgroundImage, ifs, backgroundImage.PixelWidth, backgroundImage.PixelHeight, 0, 85);
			}
		}

		public void CreateTile(WriteableBitmap tileImage, string tileName, Uri pageUri)
		{
			string tileImageFile = System.IO.Path.Combine(ApplicationSettingsManager.SHAREDSHELLCONTENT_PATH, tileName + ".png");

			using(IsolatedStorageFileStream ifs = ApplicationSettingsManager.UserStoreForApplication.OpenFile(tileImageFile, FileMode.Create, FileAccess.ReadWrite))
			{
				ExtendedImage image = tileImage.ToImage();
				image.WriteToStream(ifs, tileImageFile);
			}

			StandardTileData tileData = new StandardTileData
			{
				Title = String.Format("uknit - {0}", tileName),
				BackgroundImage = new Uri("isostore:/" + tileImageFile, UriKind.Absolute)
			};

			ShellTile.Create(pageUri, tileData);
		}

		public void DeleteTile(string tileName)
		{
			ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains(tileName));

			if(tile != null)
			{
				string tileImageFile = System.IO.Path.Combine(ApplicationSettingsManager.SHAREDSHELLCONTENT_PATH, tileName + ".png");
				ApplicationSettingsManager.UserStoreForApplication.DeleteFile(tileImageFile);
				tile.Delete();
			}
		}

		public void UpdateTile(WriteableBitmap tileImage, string tileName)
		{
			ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains(tileName));
			
			if(tile != null)
			{
				string tileImageFile = System.IO.Path.Combine(ApplicationSettingsManager.SHAREDSHELLCONTENT_PATH, tileName + ".png");

				ApplicationSettingsManager.UserStoreForApplication.DeleteFile(tileImageFile);

				using(IsolatedStorageFileStream ifs = ApplicationSettingsManager.UserStoreForApplication.OpenFile(tileImageFile, FileMode.Create, FileAccess.ReadWrite))
				{
					ExtendedImage image = tileImage.ToImage();
					image.WriteToStream(ifs, tileImageFile);
				}

				StandardTileData tileData = new StandardTileData
				{
					Title = String.Format("uknit - {0}", tileName),
					BackgroundImage = new Uri("isostore:/" + tileImageFile, UriKind.Absolute)
				};

				tile.Update(tileData);
			}
		}

		public void EnableRowCounterTensDigit(bool isEnabled)
		{
			IsolatedStorage[EnableRowCounterTensDigitKeyName] = isEnabled;
		}

		public bool IsRowCounterTensDigitEnabled()
		{
			bool? isRowCounterTensDigitEnabled;

			if(!IsolatedStorage.TryGetValue(EnableRowCounterTensDigitKeyName, out isRowCounterTensDigitEnabled))
			{
				isRowCounterTensDigitEnabled = false;
				IsolatedStorage[EnableRowCounterTensDigitKeyName] = false;
			}

			return (bool)isRowCounterTensDigitEnabled;
		}

		public bool IsBackgroundEnabled()
		{
			bool? isBackgroundEnabled;

			if(!IsolatedStorage.TryGetValue(EnableBackgroundImageKeyName, out isBackgroundEnabled))
			{
				isBackgroundEnabled = true;
			}

			return (bool)isBackgroundEnabled;
		}

		public bool IsLightThemeEnabled()
		{
			Visibility isLightTheme = (Visibility)Application.Current.Resources["PhoneLightThemeVisibility"];

			return isLightTheme == Visibility.Visible;
		}

		public Brush GetPanoramaBackgroundBrush()
		{
			Brush backgroundBrush;
			BitmapImage backgroundImage = new BitmapImage();
			bool? isBackgroundEnabled;

			if(!IsolatedStorage.TryGetValue(EnableBackgroundImageKeyName, out isBackgroundEnabled))
			{
				isBackgroundEnabled = true;
			}

			if(isBackgroundEnabled == true)
			{
				string backgroundImageFile = System.IO.Path.Combine(ApplicationSettingsManager.IMAGES_PATH, ApplicationSettingsManager.BACKGROUNDIMAGE_FILENAME);

				if(ApplicationSettingsManager.UserStoreForApplication.FileExists(backgroundImageFile))
				{
					using(IsolatedStorageFileStream ifs = ApplicationSettingsManager.UserStoreForApplication.OpenFile(backgroundImageFile, FileMode.Open, FileAccess.Read))
					{
						backgroundImage.SetSource(ifs);
					}
				}
				else
				{
					backgroundImage.UriSource = new Uri(ApplicationSettingsManager.DEFAULT_PANORAMA_IMAGE, UriKind.Relative);
				}

				ImageBrush img = new ImageBrush();
				img.ImageSource = backgroundImage;
				backgroundBrush = img;
			}
			else
			{
				backgroundBrush = Application.Current.Resources["PhoneBackgroundBrush"] as SolidColorBrush;
			}

			return backgroundBrush;
		}

		public Brush GetPageBackgroundBrush()
		{
			Brush backgroundBrush;
			BitmapImage backgroundImage = new BitmapImage();
			bool? isBackgroundEnabled;

			if(!IsolatedStorage.TryGetValue(EnableBackgroundImageKeyName, out isBackgroundEnabled))
			{
				isBackgroundEnabled = true;
			}

			if(isBackgroundEnabled == true)
			{
				backgroundImage.UriSource = new Uri(ApplicationSettingsManager.DEFAULT_PAGE_IMAGE, UriKind.Relative);

				ImageBrush img = new ImageBrush();
				img.ImageSource = backgroundImage;
				backgroundBrush = img;
			}
			else
			{
				backgroundBrush = Application.Current.Resources["PhoneBackgroundBrush"] as SolidColorBrush;
			}

			return backgroundBrush;
		}

		public KnittingProject GetKnittingProjectByName(string projectName)
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
						RowCounterColor = StringToColorConverter.HexString2Color(project.Element("RowCounterColorRGB").Value),
						IsPinnedToStart = project.Element("IsPinnedToStart") == null ? false : bool.Parse(project.Element("IsPinnedToStart").Value)
					};
			}

			return knittingProject;
		}

		public void RemoveKnittingProjectByName(string projectName)
		{
			XElement projectXml = GetKnittingProjects();
			XElement projectToRemove = projectXml.Elements("Project").Where(p => p.Element("Name").Value == projectName).First();
			projectToRemove.Remove();

			string projectPath = System.IO.Path.Combine(ApplicationSettingsManager.DATA_PATH, ApplicationSettingsManager.SAVEDPROJECTS_FILENAME);
			using(IsolatedStorageFileStream stream = ApplicationSettingsManager.UserStoreForApplication.OpenFile(projectPath, FileMode.Create, FileAccess.ReadWrite))
			{
				using(StreamWriter writer = new StreamWriter(stream))
				{
					projectXml.Save(writer);
				}
			}

			// If the project is pinned, then remove the tile
			// TODO

		}

		public void AddKnittingProject(string projectName, KnittingProject project)
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

				string projectPath = System.IO.Path.Combine(ApplicationSettingsManager.DATA_PATH, ApplicationSettingsManager.SAVEDPROJECTS_FILENAME);
				using(IsolatedStorageFileStream stream = ApplicationSettingsManager.UserStoreForApplication.OpenFile(projectPath, FileMode.Create, FileAccess.ReadWrite))
				{
					using(StreamWriter writer = new StreamWriter(stream))
					{
						projectXml.Save(writer);
					}
				}
			}
		}

		public void ModifyKnittingProjectByName(string projectName, KnittingProject project)
		{
			XElement projectXml = GetKnittingProjects();
			XElement projectToModify = projectXml.Elements("Project").Where(p => p.Element("Name").Value == projectName).First();

			projectToModify.Element("Name").Value = project.ProjectName;
			projectToModify.Element("Description").Value = project.ProjectDescription;
			projectToModify.Element("CurrentRowCount").Value = project.CurrentRowCount.ToString();
			projectToModify.Element("RowCounterColorRGB").Value = project.RowCounterColorRGB;

			if(projectToModify.Element("IsPinnedToStart") == null)
			{
				projectToModify.Add(new XElement("IsPinnedToStart", project.IsPinnedToStart.ToString()));
			}
			else
			{
				projectToModify.Element("IsPinnedToStart").Value = project.IsPinnedToStart.ToString();
			}

			string projectPath = System.IO.Path.Combine(ApplicationSettingsManager.DATA_PATH, ApplicationSettingsManager.SAVEDPROJECTS_FILENAME);
			using(IsolatedStorageFileStream stream = ApplicationSettingsManager.UserStoreForApplication.OpenFile(projectPath, FileMode.Create, FileAccess.ReadWrite))
			{
				using(StreamWriter writer = new StreamWriter(stream))
				{
					projectXml.Save(writer);
				}
			}
		}

		public List<KnittingProject> LoadKnittingProjects()
		{
			List<KnittingProject> projects = new List<KnittingProject>();
			XElement projectsXml = GetKnittingProjects();

			if(projectsXml != null)
			{
				projects = projectsXml.Elements("Project").Select(p => new KnittingProject
				{
					ProjectName = p.Element("Name").Value,
					ProjectDescription = p.Element("Description").Value,
					RowCounterColor = StringToColorConverter.HexString2Color(p.Element("RowCounterColorRGB").Value),
					CurrentRowCount = int.Parse(p.Element("CurrentRowCount").Value),
					IsPinnedToStart = p.Element("IsPinnedToStart") == null ? false : bool.Parse(p.Element("IsPinnedToStart").Value)
				}).ToList();
			}

			return projects;
		}

		private XElement GetKnittingProjects()
		{
			XElement projects = null;
			string projectPath = System.IO.Path.Combine(ApplicationSettingsManager.DATA_PATH, ApplicationSettingsManager.SAVEDPROJECTS_FILENAME);

			if(ApplicationSettingsManager.UserStoreForApplication.FileExists(projectPath))
			{
				using(IsolatedStorageFileStream stream = ApplicationSettingsManager.UserStoreForApplication.OpenFile(projectPath, FileMode.Open, FileAccess.Read))
				{
					using(StreamReader reader = new StreamReader(stream))
					{
						projects = XElement.Load(reader);
					}
				}
			}
			else
			{
				if(!ApplicationSettingsManager.UserStoreForApplication.DirectoryExists(ApplicationSettingsManager.DATA_PATH))
				{
					ApplicationSettingsManager.UserStoreForApplication.CreateDirectory(ApplicationSettingsManager.DATA_PATH);
				}

				using(IsolatedStorageFileStream stream = ApplicationSettingsManager.UserStoreForApplication.CreateFile(projectPath))
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

		public bool IsRulerCalibrated()
		{
			return IsolatedStorage.Contains(RulerCalibratedKeyName);
		}

		public void SetRulerCalibrated()
		{
			if(!this.IsRulerCalibrated())
			{
				IsolatedStorage[RulerCalibratedKeyName] = true;
			}
		}

		public string GetUnitOfMeasure()
		{
			string unitOfMeasure = "Inches";
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

		public void SetUnitOfMeasure(string unitOfMeasure)
		{
			IsolatedStorage[UnitOfMeasureKeyName] = unitOfMeasure;
		}

		public double GetDevicePixelDensity()
		{
			double ppd;

			if(this.GetUnitOfMeasure() == "Inches")
			{
				ppd = this.GetDevicePixelsPerInch();
			}
			else
			{
				ppd = this.GetDevicePixelsPerCentimeter();
			}

			return ppd;
		}

		public void SetDevicePixelDensity(double pixelsPerInch)
		{
			if(this.GetUnitOfMeasure() == "Inches")
			{
				this.SetDevicePixelsPerInch(pixelsPerInch);
			}
			else
			{
				this.SetDevicePixelsPerCentimeter(pixelsPerInch);
			}
		}

		public double GetDevicePixelsPerInch()
		{
			double ppi;
			if(!IsolatedStorage.TryGetValue(DevicePixelsPerInchKeyName, out ppi))
			{
				ppi = 262;
				IsolatedStorage[DevicePixelsPerInchKeyName] = ppi;
			}

			return ppi;
		}

		public void SetDevicePixelsPerInch(double pixelsPerInch)
		{
			IsolatedStorage[DevicePixelsPerInchKeyName] = pixelsPerInch;
		}

		public double GetDevicePixelsPerCentimeter()
		{
			double ppi = this.GetDevicePixelsPerInch();

			return (ppi / 2.54);
		}

		public void SetDevicePixelsPerCentimeter(double pixelsPerCentimeter)
		{
			IsolatedStorage[DevicePixelsPerInchKeyName] = pixelsPerCentimeter * 2.54;
		}

		public void RestoreDefaults()
		{
			IsolatedStorage.Remove(DevicePixelsPerInchKeyName);
			IsolatedStorage.Remove(UnitOfMeasureKeyName);
			IsolatedStorage.Remove(RulerCalibratedKeyName);
			IsolatedStorage.Remove(EnableRowCounterTensDigitKeyName);
			IsolatedStorage.Remove(EnableBackgroundImageKeyName);
		}
	}
}
