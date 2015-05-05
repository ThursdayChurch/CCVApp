using System;
using System.Drawing;
using Rock.Mobile.PlatformUI;

namespace App
{
    namespace Shared
    {
        /// <summary>
        /// Config contains values for customizing various aspects of App.
        /// </summary>
        namespace Config
        {
            public class GeneralConfig
            {
                /// <summary>
                /// The full name of your organization
                /// </summary>
                public const string OrganizationName = "Christ's Church of the Valley";

                /// <summary>
                /// The abbreviated name of your organization. (You can set this to OrganizationName if desired)
                /// </summary>
                public const string OrganizationShortName = "CCV";

                /// <summary>
                /// Used when creating new addresses that will be sent to Rock. If your app will run in another country,
                /// set CountryCode to the ISO country code.
                /// </summary>
                public const string CountryCode = "US";

                /// <summary>
                /// The Facebook app ID reprsenting the Facebook app that will get things on behalf of the user.
                /// </summary>
                public const string FBAppID = "495461873811179";

                /// <summary>
                /// iOS only, this controls what style of keyboard is used for PLATFORM textFields.
                /// Meaning the ones dynamically created in code via Rock.Mobile. 
                /// Any normal iOS text field needs to have its style explicitely set.
                /// Although this is an int, it should match the KeyboardAppearance enum.
                /// </summary>
                public const KeyboardAppearanceStyle iOSPlatformUIKeyboardAppearance = KeyboardAppearanceStyle.Dark;

                /// <summary>
                /// This should be the base URL for where your Rock instance is hosted.
                /// </summary>
                public const string RockBaseUrl = "http://rock.ccvonline.com/";
            }

            public class SpringboardConfig
            {
                /// <summary>
                /// The background color of the springboard area.
                /// </summary>
                public const uint BackgroundColor = 0x2D2D2DFF;

                /// <summary>
                /// The color of the active springboard element.
                /// </summary>
                public const uint ActiveElementTextColor = 0xFFFFFFFF;

                /// <summary>
                /// The color for inactive springboard elements.
                /// </summary>
                public const uint InActiveElementTextColor = 0xA7A7A7FF;

                /// <summary>
                /// The background color of a selected element.
                /// </summary>
                public const int Element_SelectedColor = 0x7a1315FF;

                /// <summary>
                /// The icon to use representing the News element
                /// </summary>
                public const string Element_News_Icon = "";

                /// <summary>
                /// The icon to use representing the Connect element
                /// </summary>
                public const string Element_Connect_Icon = "";

                /// <summary>
                /// The icon to use representing the Messages element
                /// </summary>
                public const string Element_Messages_Icon = "";

                /// <summary>
                /// The icon to use representing the Prayer element
                /// </summary>
                public const string Element_Prayer_Icon = "";

                /// <summary>
                /// The icon to use representing the Give element
                /// </summary>
                public const string Element_Give_Icon = "";

                /// <summary>
                /// The icon to use representing the More element
                /// </summary>
                public const string Element_More_Icon = "";
            }

            /// <summary>
            /// Settings for the primary nav bar (the one at the top)
            /// </summary>
            public class PrimaryNavBarConfig
            {
                /// <summary>
                /// The background color of the primary navigation bar up top.
                /// </summary>
                public const uint BackgroundColor = 0x191919FF;
            }

            public class NewsConfig
            {
                /// <summary>
                ///  These are default news items that will show anytime server news is unavailable.
                /// </summary>
                public static string [] DefaultNews_A =
                    {
                        //Title
                        "Baptisms", 

                        // Description
                        "Baptism is one of the most important events in the life of a Christian. If you've made a commitment to Christ, " + 
                        "it's time to take the next step and make your decision known. Baptism is the best way to express your faith and " + 
                        "reflect your life change. Find out more about baptism through Starting Point, register online or contact your neighborhood pastor.",

                        // Reference URL
                        "http://www.ccvonline.com/Arena/default.aspx?page=17655&campus=1",

                        // Image Names
                        "default_news_a_main",

                        "default_news_a_header"
                    };

                public static string [] DefaultNews_B =
                    {
                        //Title
                        "Starting Point", 

                        // Description
                        "If you’re asking yourself, “Where do I begin at CCV?” — the answer is Starting Point. " +
                        "In Starting Point you’ll find out what CCV is all about, take a deep look at the Christian " + 
                        "faith, and learn how you can get involved. Childcare is available for attending parents.",

                        // Reference URL
                        "http://www.ccvonline.com/Arena/default.aspx?page=17400&campus=1",

                        // Image Names
                        "default_news_b_main",

                        "default_news_b_header"
                    };

                public static string [] DefaultNews_C =
                    {
                        //Title
                        "Learn More", 

                        // Description
                        "Wondering what else CCV is about? Check out our website.",

                        // Reference URL
                        "http://ccv.church/Arena/default.aspx?page=17369&campus=1",

                        // Image Names
                        "default_news_c_main",

                        "default_news_c_header"
                    };
            }

            public class NoteConfig
            {
                /// <summary>
                /// The base URL to look for Notes. (this may be removed in a future version)
                /// </summary>
                public const string BaseURL = "http://ccv.church/ccvmobile/";

                /// <summary>
                /// The color of the font/icon when displaying the citation icon.
                /// </summary>
                public const uint CitationUrl_IconColor = 0x878686BB;

                /// <summary>
                /// The color of the user note anchor (which is what the user interacts with to move, open, close and delete the note)
                /// </summary>
                public const uint UserNote_AnchorColor = 0x77777777;

                /// <summary>
                /// The color of the font/icon when displaying the user note icon.
                /// </summary>
                public const uint UserNote_IconColor = 0x8c8c8cFF;

                /// <summary>
                /// The color of the icon in the details table row.
                /// </summary>
                public const uint Details_Table_IconColor = 0xc43535FF;
            }

            public class PrayerConfig
            {
                /// <summary>
                /// The color to fill the "Pray" circle when a user prays for a request.
                /// </summary>
                public const uint PrayedForColor = 0x26b8e2FF;
            }

            public class ConnectConfig
            {
                /// <summary>
                /// These are the "other" connect options. Each one is a label, a URL to visit, and an icon to display.
                /// </summary>
                public static string[] WebViews = 
                    {
                        //"Starting Point", "http://www.ccvonline.com/Arena/default.aspx?page=17400&campus=1", "starting_point_thumb.png",
                        //"Baptisms", "http://www.ccvonline.com/Arena/default.aspx?page=17655&campus=1", "baptism_thumb.png",
                        //"Serve", "http://www.ccvonline.com/Arena/default.aspx?page=17659&campus=1", "serve_thumb.png"
                        "Starting Point", "http://www.jeredmcferron.com/ccv/placeholder.html", "starting_point_thumb.png",
                        "Baptisms", "http://www.jeredmcferron.com/ccv/placeholder.html", "baptism_thumb.png",
                        "Serve", "http://www.jeredmcferron.com/ccv/placeholder.html", "serve_thumb.png"

                    };
                
                /// <summary>
                /// The default latitude the group finder map will be positioned to before a search is performed.
                /// </summary>
                public const double GroupFinder_DefaultLatitude = 33.6054149;

                /// <summary>
                /// The default longitude the group finder map will be positioned to before a search is performed. 
                /// </summary>
                public const double GroupFinder_DefaultLongitude = -112.125051;

                /// <summary>
                /// The default latitude scale (how far out) the map will be scaled to before a search is performed.
                /// </summary>
                public const int GroupFinder_DefaultScale_iOS = 100000;

                /// <summary>
                /// The default longitude scale (how far out) the map will be scaled to before a search is performed. 
                /// </summary>
                public const float GroupFinder_DefaultScale_Android = 9.25f;
            }

            public class GiveConfig
            {
                /// <summary>
                /// The url to take a user to for giving.
                /// </summary>
                //public const string GiveUrl = "https://www.ccvonline.com/Arena/default.aspx?page=18485&campus=1";
                public const string GiveUrl = "http://www.jeredmcferron.com/ccv/placeholder.html";
            }

            public class ControlStylingConfig
            {
                /// <summary>
                /// The background color for the pages (basically the darkest area)
                /// </summary>
                public const uint BackgroundColor = 0x212121FF;

                /// <summary>
                /// The color of text for buttons
                /// </summary>
                public const uint Button_TextColor = 0xCCCCCCFF;

                /// <summary>
                /// The background color for buttons
                /// </summary>
                public const uint Button_BGColor = 0x7E7E7EFF;

                /// <summary>
                /// The corner roundedness for buttons (0 is no curvature)
                /// </summary>
                public const uint Button_CornerRadius = 3;

                /// <summary>
                /// The background color for the layer that backs elements (like the strip behind First Name)
                /// </summary>
                public const uint BG_Layer_Color = 0x3E3E3EFF;

                /// <summary>
                /// The background color for a text layer that has invalid input. This lets the user know
                /// there is something wrong with the particular field.
                /// </summary>
                public const uint BadInput_BG_Layer_Color = 0x5B1013FF;

                /// <summary>
                /// The border color for the layer that backs elements (like the strip behind First Name)
                /// This can also be used to highlight certain elements, as it is a bright color.
                /// </summary>
                public const uint BG_Layer_BorderColor = 0x595959FF;

                /// <summary>
                /// The border thickness for the layer that backs elements (like the strip behind First Name)
                /// </summary>
                public const float BG_Layer_BorderWidth = .5f;

                /// <summary>
                /// The color for text that is not placeholder (what the user types in, control labels, etc.)
                /// </summary>
                public const uint TextField_ActiveTextColor = 0xCCCCCCFF;

                /// <summary>
                /// The color for placeholder text in fields the user can type into.
                /// </summary>
                public const uint TextField_PlaceholderTextColor = 0x878686FF;

                /// <summary>
                /// The color of text in standard labels
                /// </summary>
                public const uint Label_TextColor = 0xCCCCCCFF;

                /// <summary>
                /// The color of a UI Switch when turned 'on'
                /// </summary>
                public const uint Switch_OnColor = 0x7E7E7EFF;

                /// <summary>
                /// The primary (most commonly used) icon font
                /// </summary>
                public const string Icon_Font_Primary = "FontAwesome";

                /// <summary>
                /// The secondary (used in occasional spots) icon font
                /// </summary>
                public const string Icon_Font_Secondary = "Bh";

                /// <summary>
                /// The symbol to use for a result that was successful.
                /// </summary>
                public const string Result_Symbol_Success = "";

                /// <summary>
                /// The symbol to use for a result that failed.
                /// </summary>
                public const string Result_Symbol_Failed = "";

                /// <summary>
                /// The color for the footers of primary table cells. (Like the footer in the Messages->Series primary cell that says "Previous Messages")
                /// </summary>
                public const uint Table_Footer_Color = 0x262626FF;

                /// <summary>
                /// The font to use representing a bold font throughout the app.
                /// </summary>
                public const string Font_Bold = "DINOffc-CondBlack";

                /// <summary>
                /// The font to use representing a regular font throughout the app.
                /// </summary>
                public const string Font_Regular = "OpenSans-Regular";

                /// <summary>
                /// The font to use representing a light font throughout the app.
                /// </summary>
                public const string Font_Light = "OpenSans-Light";

                /// <summary>
                /// The size of to use for the large font throughout the app.
                /// </summary>
                public const uint Large_FontSize = 23;

                /// <summary>
                /// The size of to use for the medium font throughout the app.
                /// </summary>
                public const uint Medium_FontSize = 19;

                /// <summary>
                /// The size of to use for the small font throughout the app.
                /// </summary>
                public const uint Small_FontSize = 16;
            }

            public class AboutConfig
            {
                /// <summary>
                /// The page to navigate to in the About's embedded webview.
                /// </summary>
                //public const string Url = "http://www.ccvonline.com/Arena/default.aspx?page=17623&campus=1";
                public const string Url = "http://www.jeredmcferron.com/ccv/placeholder.html";
            }
        }
    }
}