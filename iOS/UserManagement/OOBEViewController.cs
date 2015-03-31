using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using CCVApp.Shared.UI;
using CCVApp.Shared.Config;
using Rock.Mobile.PlatformSpecific.Util;
using System.Drawing;
using CoreGraphics;
using Rock.Mobile.Animation;

namespace iOS
{
	class OOBEViewController : UIViewController
	{
        UIOOBE OOBEView { get; set; }

        public SpringboardViewController Springboard { get; set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.BackgroundColor );

            // this is totally a hack, but in order to seamlessly transition from the splash screen
            // to our logo, we need to use a PER-DEVICE image. Sigh.
            string imageName = GetSplashLogo( UIKit.UIScreen.MainScreen.ApplicationFrame.Size, UIKit.UIScreen.MainScreen.Scale );

            OOBEView = new UIOOBE();
            OOBEView.Create( View, "oobe_splash_bg.png", imageName, View.Frame.ToRectF( ), delegate(int index) 
                {
                    Springboard.OOBEOnClick( index );
                } );
        }

        string GetSplashLogo( CGSize screenSize, nfloat scalar )
        {
            nfloat nativeWidth = screenSize.Width * scalar;
            nfloat nativeHeight = screenSize.Height * scalar;

            // default to iphone4, cause..why not.
            string imageName = "oobe_ccv_logo_i4@2x.png";

            // compare the dimensions with the known iDevice sizes, and return the appropriate string.
            if ( nativeWidth == 640 && nativeHeight == 960 )
            {
                imageName = "oobe_ccv_logo_i4@2x.png";
            }
            else if ( nativeWidth == 640 && nativeHeight == 1136 )
            {
                imageName = "oobe_ccv_logo_i5@2x.png";
            }
            else if ( nativeWidth == 750 && nativeHeight == 1334 )
            {
                imageName = "oobe_ccv_logo_i6@2x.png";
            }
            else if ( nativeWidth == 1242 && nativeHeight == 2208 )
            {
                imageName = "oobe_ccv_logo_i6p@3x.png";
            }

            return imageName;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            OOBEView.PerformStartup( );
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            OOBEView.LayoutChanged( View.Bounds.ToRectF( ) );
        }
	}
}
