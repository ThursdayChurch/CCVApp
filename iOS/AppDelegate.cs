﻿using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using iOS;

namespace CCVApp
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to
    // application events from iOS.
    [Register( "AppDelegate" )]
    public partial class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations
        UIWindow window;

        protected static UIStoryboard Storyboard = UIStoryboard.FromName ("MainStoryboard", null);
        private SpringboardViewController Springboard { get; set; }

        //
        // This method is invoked when the application has loaded and is ready to run. In this
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching( UIApplication app, NSDictionary options )
        {
            // create a new window instance based on the screen size
            window = new UIWindow( UIScreen.MainScreen.Bounds );
			
            // If you have defined a root view controller, set it here:
            Springboard = Storyboard.InstantiateInitialViewController() as SpringboardViewController;
            window.RootViewController = Springboard;
			
            // make the window visible
            window.MakeKeyAndVisible( );
			
            return true;
        }

        public override void OnActivated(UIApplication application)
        {
            Console.WriteLine("OnActivated called, App is active.");
            Springboard.OnActivated( );
        }
        public override void WillEnterForeground(UIApplication application)
        {
            Console.WriteLine("App will enter foreground");
            Springboard.WillEnterForeground( );
        }
        public override void OnResignActivation(UIApplication application)
        {
            Console.WriteLine("OnResignActivation called, App moving to inactive state.");
            Springboard.OnResignActive( );
        }
        public override void DidEnterBackground(UIApplication application)
        {
            Console.WriteLine("App entering background state.");
            Springboard.DidEnterBackground( );
        }
        // not guaranteed that this will run
        public override void WillTerminate(UIApplication application)
        {
            Console.WriteLine("App is terminating.");
            Springboard.WillTerminate( );
        }
    }
}

