﻿using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Marketplace;
using Microsoft.Phone.Shell;
using uknit.ViewModels;

namespace uknit
{
	public partial class App : Application
	{
		private static MainViewModel viewModel = null;

		private static LicenseInformation LicenseInfo = new LicenseInformation();

		/// <summary>
		/// A static ViewModel used by the views to bind against.
		/// </summary>
		/// <returns>The MainViewModel object.</returns>
		public static MainViewModel ViewModel
		{
			get
			{
				// Delay creation of the view model until necessary
				if(viewModel == null)
					viewModel = new MainViewModel();

				return viewModel;
			}
		}

		private static bool _isTrial = true;
		public bool IsTrial
		{
			get
			{
				return _isTrial;
			}
		}

		/// <summary>
		/// Provides easy access to the root frame of the Phone Application.
		/// </summary>
		/// <returns>The root frame of the Phone Application.</returns>
		public PhoneApplicationFrame RootFrame
		{
			get;
			private set;
		}

		/// <summary>
		/// Constructor for the Application object.
		/// </summary>
		public App()
		{
			// Global handler for uncaught exceptions. 
			// Note that exceptions thrown by ApplicationBarItem.Click will not get caught here.
			UnhandledException += Application_UnhandledException;

			// Show graphics profiling information while debugging.
			if(System.Diagnostics.Debugger.IsAttached)
			{
				// Display the current frame rate counters.
				Application.Current.Host.Settings.EnableFrameRateCounter = true;

				// Show the areas of the app that are being redrawn in each frame.
				//Application.Current.Host.Settings.EnableRedrawRegions = true;

				// Enable non-production analysis visualization mode, 
				// which shows areas of a page that are being GPU accelerated with a colored overlay.
				//Application.Current.Host.Settings.EnableCacheVisualization = true;
			}

			// Standard Silverlight initialization
			InitializeComponent();

			// Phone-specific initialization
			InitializePhoneApplication();
		}

		// Code to execute when the application is launching (eg, from Start)
		// This code will not execute when the application is reactivated
		private void Application_Launching(object sender, LaunchingEventArgs e)
		{
			CheckLicense();
		}

		// Code to execute when the application is activated (brought to foreground)
		// This code will not execute when the application is first launched
		private void Application_Activated(object sender, ActivatedEventArgs e)
		{
			CheckLicense();

			if(!App.ViewModel.IsDataLoaded)
			{
				App.ViewModel.LoadData();
			}
		}

		// Code to execute when the application is deactivated (sent to background)
		// This code will not execute when the application is closing
		private void Application_Deactivated(object sender, DeactivatedEventArgs e)
		{
		}

		// Code to execute when the application is closing (eg, user hit Back)
		// This code will not execute when the application is deactivated
		private void Application_Closing(object sender, ClosingEventArgs e)
		{
		}

		// Code to execute if a navigation fails
		private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
		{
			if(System.Diagnostics.Debugger.IsAttached)
			{
				// A navigation has failed; break into the debugger
				System.Diagnostics.Debugger.Break();
			}
		}

		// Code to execute on Unhandled Exceptions
		private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
		{
			// Write the callstack to isolated storage
			System.IO.IsolatedStorage.IsolatedStorageSettings IsolatedStorage = System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings;
			System.IO.IsolatedStorage.IsolatedStorageFile UserStoreForApplication = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication();
			using(System.IO.IsolatedStorage.IsolatedStorageFileStream stream = UserStoreForApplication.OpenFile("callstack.txt", System.IO.FileMode.CreateNew, System.IO.FileAccess.ReadWrite))
			{
				using(System.IO.StreamWriter writer = new System.IO.StreamWriter(stream))
				{
					writer.WriteLine("Message: " + e.ExceptionObject.Message);
					writer.WriteLine("Callstack:");
					writer.WriteLine(e.ExceptionObject.StackTrace);
					writer.Flush();
				}
			}

			if(System.Diagnostics.Debugger.IsAttached)
			{
				// An unhandled exception has occurred; break into the debugger
				System.Diagnostics.Debugger.Break();
			}
		}

		#region Phone application initialization

		// Avoid double-initialization
		private bool phoneApplicationInitialized = false;

		// Do not add any additional code to this method
		private void InitializePhoneApplication()
		{
			if(phoneApplicationInitialized)
				return;

			// Create the frame but don't set it as RootVisual yet; this allows the splash
			// screen to remain active until the application is ready to render.
			//RootFrame = new PhoneApplicationFrame();
			RootFrame = new TransitionFrame();
			RootFrame.Navigated += CompleteInitializePhoneApplication;

			// Handle navigation failures
			RootFrame.NavigationFailed += RootFrame_NavigationFailed;

			// Ensure we don't initialize again
			phoneApplicationInitialized = true;
		}

		// Do not add any additional code to this method
		private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
		{
			// Set the root visual to allow the application to render
			if(RootVisual != RootFrame)
				RootVisual = RootFrame;

			// Remove this handler since it is no longer needed
			RootFrame.Navigated -= CompleteInitializePhoneApplication;
		}

		/// <summary>
		/// Check the current license information for this application
		/// </summary>
		private void CheckLicense()
		{
#if DEBUG
			string message = "This sample demonstrates the implementation of a trial mode in an application." +
							   "Press 'OK' to simulate trial mode. Press 'Cancel' to run the application in normal mode.";
			if(MessageBox.Show(message, "Debug Trial", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
			{
				_isTrial = true;
			}
			else
			{
				_isTrial = false;
			}
#else
			_isTrial = LicenseInfo.IsTrial();
#endif
		}

		#endregion
	}
}