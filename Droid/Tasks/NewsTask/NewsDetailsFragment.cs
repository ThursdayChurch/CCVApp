﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Rock.Mobile.PlatformUI;
using App.Shared.Config;
using App.Shared.Strings;
using Android.Text.Method;
using App.Shared;
using System.IO;
using App.Shared.PrivateConfig;

namespace Droid
{
    namespace Tasks
    {
        namespace News
        {
            public class NewsDetailsFragment : TaskFragment
            {
                bool IsFragmentActive { get; set; }

                public App.Shared.Network.RockNews NewsItem { get; set; }

                Rock.Mobile.PlatformSpecific.Android.Graphics.AspectScaledImageView ImageBanner { get; set; }
                Bitmap HeaderImage { get; set; }

                public override void OnCreate( Bundle savedInstanceState )
                {
                    base.OnCreate( savedInstanceState );
                }

                public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
                {
                    if (container == null)
                    {
                        // Currently in a layout without a container, so no reason to create our view.
                        return null;
                    }

                    View view = inflater.Inflate(Resource.Layout.News_Details, container, false);
                    view.SetOnTouchListener( this );
                    view.SetBackgroundColor( Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.BackgroundColor ) );

                    // set the banner
                    ImageBanner = new Rock.Mobile.PlatformSpecific.Android.Graphics.AspectScaledImageView( Activity );
                    ( (LinearLayout)view ).AddView( ImageBanner, 0 );

                    TextView title = view.FindViewById<TextView>( Resource.Id.news_details_title );
                    title.Text = NewsItem.Title;
                    title.SetSingleLine( );
                    title.Ellipsize = Android.Text.TextUtils.TruncateAt.End;
                    title.SetMaxLines( 1 );
                    title.SetHorizontallyScrolling( true );
                    ControlStyling.StyleUILabel( title, ControlStylingConfig.Font_Bold, ControlStylingConfig.Large_FontSize );

                    // set the description
                    TextView description = view.FindViewById<TextView>( Resource.Id.news_details_details );
                    description.Text = NewsItem.Description;
                    description.MovementMethod = new ScrollingMovementMethod();
                    ControlStyling.StyleUILabel( description, ControlStylingConfig.Font_Light, ControlStylingConfig.Small_FontSize );

                    Button launchUrlButton = view.FindViewById<Button>(Resource.Id.news_details_launch_url);
                    launchUrlButton.Click += (object sender, EventArgs e) => 
                        {
                            // move to the next page..somehow.
                            ParentTask.OnClick( this, launchUrlButton.Id );
                        };
                    ControlStyling.StyleButton( launchUrlButton, NewsStrings.LearnMore, ControlStylingConfig.Font_Regular, ControlStylingConfig.Small_FontSize );

                    // hide the button if there's no reference URL.
                    if ( string.IsNullOrEmpty( NewsItem.ReferenceURL ) == true )
                    {
                        launchUrlButton.Visibility = ViewStates.Invisible;
                    }

                    return view;
                }

                public override void OnResume()
                {
                    base.OnResume();

                    ParentTask.NavbarFragment.NavToolbar.SetBackButtonEnabled( true );
                    ParentTask.NavbarFragment.NavToolbar.SetCreateButtonEnabled( false, null );
                    ParentTask.NavbarFragment.NavToolbar.SetShareButtonEnabled( false, null );
                    ParentTask.NavbarFragment.NavToolbar.Reveal( false );

                    IsFragmentActive = true;


                    // attempt to load the image from cache. If that doesn't work, use a placeholder
                    HeaderImage = null;

                    System.IO.MemoryStream assetStream = (System.IO.MemoryStream)FileCache.Instance.LoadFile( NewsItem.HeaderImageName );
                    if ( assetStream!= null )
                    {
                        try
                        {
                            HeaderImage = BitmapFactory.DecodeStream( assetStream );
                        }
                        catch( Exception )
                        {
                            FileCache.Instance.RemoveFile( NewsItem.HeaderImageName );
                            Console.WriteLine( "Image {0} was corrupt. Removing.", NewsItem.HeaderImageName );
                        }
                        assetStream.Dispose( );
                    }
                    else
                    {
                        // use the placeholder and request the image download
                        System.IO.Stream thumbnailStream = Activity.BaseContext.Assets.Open( PrivateGeneralConfig.NewsDetailsPlaceholder );
                        HeaderImage = BitmapFactory.DecodeStream( thumbnailStream );

                        FileCache.Instance.DownloadFileToCache( NewsItem.HeaderImageURL, NewsItem.HeaderImageName, delegate
                            {
                                NewsHeaderDownloaded( );
                            } );
                    }
                    ImageBanner.SetImageBitmap( HeaderImage );
                }

                void NewsHeaderDownloaded( )
                {
                    // if they're still viewing this article
                    if ( IsFragmentActive == true )
                    {
                        Rock.Mobile.Threading.Util.PerformOnUIThread( delegate
                            {
                                MemoryStream imageStream = (System.IO.MemoryStream)FileCache.Instance.LoadFile( NewsItem.HeaderImageName );
                                if ( imageStream != null )
                                {
                                    try
                                    {
                                        HeaderImage.Dispose( );
                                        HeaderImage = BitmapFactory.DecodeStream( imageStream );

                                        ImageBanner.Drawable.Dispose( );
                                        ImageBanner.SetImageBitmap( HeaderImage );
                                    }
                                    catch( Exception )
                                    {
                                        FileCache.Instance.RemoveFile( NewsItem.HeaderImageName );
                                        Console.WriteLine( "Image {0} was corrupt. Removing.", NewsItem.HeaderImageName );

                                        ImageBanner.Drawable.Dispose( );
                                        ImageBanner.SetImageBitmap( null );
                                    }

                                    imageStream.Dispose( );
                                }
                            });

                    }
                }

                public override void OnPause()
                {
                    base.OnPause();

                    IsFragmentActive = false;

                    HeaderImage.Dispose( );
                    HeaderImage = null;

                    ImageBanner.Drawable.Dispose( );
                    ImageBanner.SetImageBitmap( null );
                }
            }
        }
    }
}
