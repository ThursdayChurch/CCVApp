﻿using System;
using System.Reflection;
using System.Diagnostics;

namespace CCVApp.Shared
{
    namespace Strings
    {
        public class BuildStrings
        {
            public static string Version
            {
                get
                {
                    return "Alpha (Build 4)";
                }
            }
        }

        public class GeneralStrings
        {
            public const string Yes = "Yes";
            public const string No = "No";
            public const string Ok = "Ok";
            public const string Cancel = "Cancel";
        }

        public class SpringboardStrings
        {
            public const string ProfilePicture_SourceTitle = "Profile Picture";
            public const string ProfilePicture_SourceDescription = "Select a source for your profile picture.";
            public const string ProfilePicture_SourcePhotoLibrary = "Photo Library";
            public const string ProfilePicture_SourceCamera = "Take Photo";

            public const string ProfilePicture_Error_Title = "Profile Picture";
            public const string ProfilePicture_Error_Message = "There was a problem saving your profile picture. Please try again.";

            public const string Camera_Error_Title = "No Camera";
            public const string Camera_Error_Message = "This device does not have a camera.";

            public const string LoggedIn_Prefix = "Welcome";
            public const string LoggedOut_Promo = "Login to enable additional features.";
        }

        public class ProfileStrings
        {
            public const string SubmitChangesTitle = "Submit Changes?";
            public const string LogoutTitle = "Log Out?";
        }

        public class MessagesStrings
        {
            public const string Error_Title = "Messages";
            public const string Error_Message = "There was a problem downloading the messages. Please try again.";

            public const string Error_Watch_Playback = "There was a problem playing this video. Check your network settings and try again.";
        }

        public class PrayerStrings
        {
            public const string Error_Title = "Prayer";

            public const string Error_Retrieve_Message = "There was a problem getting prayer requests. Check your network settings and try again.";
            public const string Error_Submit_Message = "There was a problem submitting your prayer request. Check your network settings and try again.";

            public const string ViewPrayer_StatusText_Retrieving = "Getting Prayer Requests";
            public const string ViewPrayer_StatusText_Failed = "Darn :(";
            public const string Prayer_Confirm = "I Prayed";

            public const string CreatePrayer_FirstNamePlaceholderText = "First Name";
            public const string CreatePrayer_LastNamePlaceholderText = "Last Name";

            public const string PostPrayer_Status_Submitting = "Submitting";
            public const string PostPrayer_Status_SuccessText = "Submitted";
            public const string PostPrayer_Status_FailedText = "Darn :(";
            public const string PostPrayer_Result_SuccessText = "Prayer posted successfully. As soon as it is approved you'll see it in the Prayer Requests.";
            public const string PostPrayer_Result_FailedText = "Looks like there was a problem submitting your request. Tap below to try again, or go back to make changes.";
        }
    }
}
