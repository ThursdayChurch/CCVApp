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
	[Register ("PrayerMainUIViewController")]
	partial class PrayerMainUIViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView ResultBackground { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel ResultLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView RetrievingPrayersView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton RetryButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView StatusBackground { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel StatusLabel { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (ResultBackground != null) {
				ResultBackground.Dispose ();
				ResultBackground = null;
			}
			if (ResultLabel != null) {
				ResultLabel.Dispose ();
				ResultLabel = null;
			}
			if (RetrievingPrayersView != null) {
				RetrievingPrayersView.Dispose ();
				RetrievingPrayersView = null;
			}
			if (RetryButton != null) {
				RetryButton.Dispose ();
				RetryButton = null;
			}
			if (StatusBackground != null) {
				StatusBackground.Dispose ();
				StatusBackground = null;
			}
			if (StatusLabel != null) {
				StatusLabel.Dispose ();
				StatusLabel = null;
			}
		}
	}
}
