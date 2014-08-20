﻿using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace iOS
{
    public class Activity
    {
        protected UIViewController ParentViewController { get; set; }
        protected UIStoryboard Storyboard { get; set; }

        public Activity( string storyboardName )
        {
            // activities don't HAVE to have a storyboard
            if( false == string.IsNullOrEmpty( storyboardName ) )
            {
                Storyboard = UIStoryboard.FromName( storyboardName, null );
            }
        }

        /// <summary>
        /// Called when the activity is going to be the forefront activity.
        /// Allows it to do any work necessary before being interacted with.
        /// Ex: Notes might disable the phone's sleep
        /// This is NOT called when the application comes into the foreground.
        /// </summary>
        /// <param name="parentViewController">Parent view controller.</param>
        public virtual void MakeActive( UIViewController parentViewController )
        {
            ParentViewController = parentViewController;
        }

        /// <summary>
        /// Called when the activity is going away so another activity can be interacted with.
        /// Allows it to undo any work done in MakeActive.
        /// Ex: Notes might RE-enable the phone's sleep.
        /// This is NOT called when the application goes into the background.
        /// </summary>
        public virtual void MakeInActive( )
        {
            // always clear our parent view controller when going inactive
            ParentViewController = null;
        }

        /// <summary>
        /// Called when the application will go into the background.
        /// This is NOT called when the activity goes into the background.
        /// </summary>
        public virtual void AppOnResignActive( )
        {
        }

        public virtual void AppDidEnterBackground( )
        {
        }

        public virtual void AppWillTerminate( )
        {
        }
    }
}

