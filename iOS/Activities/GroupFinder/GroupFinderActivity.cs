﻿using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace iOS
{
    public class GroupFinderActivity : Activity
    {
        public GroupFinderActivity( string storyboardName ) : base( storyboardName )
        {
        }

        public override void MakeActive( UIViewController parentViewController, NavToolbar navToolbar )
        {
            base.MakeActive( parentViewController, navToolbar );
        }

        public override void MakeInActive( )
        {
            base.MakeInActive( );
        }

        public override void AppOnResignActive( )
        {
            base.AppOnResignActive( );
        }
    }
}

