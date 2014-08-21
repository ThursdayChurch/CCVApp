﻿using System;
using System.Drawing;

namespace CCVApp
{
    namespace Config
    {
        public class Springboard
        {
            /// <summary>
            /// Image to display when a user does not have a profile.
            /// </summary>
            public const string NoProfileFile = "noprofile.png";

            /// <summary>
            /// The X offset to place the CENTER of the element's logo.
            /// </summary>
            public const int Element_LogoOffsetX = 20;

            /// <summary>
            /// The X offset to place the LEFT EDGE of the element's text.
            /// </summary>
            public const int Element_LabelOffsetX = 40;
        }

        public class SubNavToolbar
        {
            /// <summary>
            /// The height of the subNavigation toolbar (the one at the bottom)
            /// </summary>
            public const float Height = 44;

            /// <summary>
            /// The color of the subNavigation toolbar (the one at the bottom)
            /// </summary>
            public const uint BackgroundColor = 0x1C1C1CFF;

            /// <summary>
            /// The amount of opacity (see throughyness) of the subNavigation toolbar (the one at the bottom)
            /// 1 = fully opaque, 0 = fully transparent.
            /// </summary>
            public const float Opacity = .75f;

            /// <summary>
            /// The amount of time (in seconds) it takes for the subNavigation toolbar (the one at the bottom)
            /// to slide up or down.
            /// </summary>
            public const float SlideRate = .50f;

            /// <summary>
            /// The text to display for the subNavigation toolbar's back button. (the one at the bottom)
            /// </summary>
            public const string BackButton_Text = "<";

            /// <summary>
            /// The size (in font points) of the sub nav toolbar back button. (the one at the bottom)
            /// </summary>
            public const int BackButton_Size = 36;
        }

        /// <summary>
        /// Settings for the primary nav bar (the one at the top)
        /// </summary>
        public class PrimaryNavBar
        {
            /// <summary>
            /// The logo to be displayed on the primary nav bar.
            /// </summary>
            public const string LogoFile = "ccvLogo.png";

            /// <summary>
            /// The image to be displayed representing the springboard reveal icon.
            /// </summary>
            public const string SpringboardRevealFile = "cheeseburger.png";

            /// <summary>
            /// The color of the primary nav bar text.
            /// </summary>
            public const uint TextColor = 0xFFFFFFFF;

            /// <summary>
            /// The color of the primary nav bar background.
            /// </summary>
            public const uint BackgroundColor = 0x1C1C1CFF;
        }

        /// <summary>
        /// Settings for the primary container that all activities lie within.
        /// </summary>
        public class PrimaryContainer
        {
            /// <summary>
            /// Time (in seconds) it takes for the primary container to slide in/out to reveal the springboard.
            /// </summary>
            public const float SlideRate = .50f;

            /// <summary>
            /// The amount to slide when revelaing the springboard.
            /// </summary>
            public const float SlideAmount = 230;

            /// <summary>
            /// The darkness of the shadow cast by the primary container on top of the springboard.
            /// 1 = fully opaque, 0 = fully transparent.
            /// </summary>
            public const float ShadowOpacity = .60f;

            /// <summary>
            /// The offset of the shadow cast by the primary container on top of the springboard.
            /// </summary>
            public static SizeF ShadowOffset = new SizeF( 0.0f, 5.0f );

            /// <summary>
            /// The color of the shadow cast by the primary container on top of the springboard.
            /// </summary>
            public const uint ShadowColor = 0x000000FF;
        }
    }
}