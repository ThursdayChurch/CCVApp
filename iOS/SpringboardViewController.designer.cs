// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;

namespace iOS
{
	[Register ("SpringboardViewController")]
	partial class SpringboardViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton AboutCCVButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton NewsButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView ProfileImage { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton SermonNotesButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton ViewProfileButton { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (AboutCCVButton != null) {
				AboutCCVButton.Dispose ();
				AboutCCVButton = null;
			}
			if (NewsButton != null) {
				NewsButton.Dispose ();
				NewsButton = null;
			}
			if (ProfileImage != null) {
				ProfileImage.Dispose ();
				ProfileImage = null;
			}
			if (SermonNotesButton != null) {
				SermonNotesButton.Dispose ();
				SermonNotesButton = null;
			}
			if (ViewProfileButton != null) {
				ViewProfileButton.Dispose ();
				ViewProfileButton = null;
			}
		}
	}
}