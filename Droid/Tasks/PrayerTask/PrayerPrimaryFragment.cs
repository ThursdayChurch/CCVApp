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
using CCVApp.Shared.Network;
using Android.Graphics;
using Rock.Mobile.PlatformUI;
using Rock.Mobile.PlatformCommon;
using System.Drawing;

namespace Droid
{
    namespace Tasks
    {
        namespace Prayer
        {
            public class PrayerPrimaryFragment : TaskFragment
            {
                class PrayerCard
                {
                    PrayerLayoutRender CurrentPrayer { get; set; }
                    PlatformView View { get; set; }
                    Button Pray { get; set; }

                    public PrayerCard( ref PlatformView cardView, RectangleF bounds )
                    {
                        View = PlatformView.Create( );
                        View.Bounds = bounds;

                        // Pray Button
                        Pray = new Button( Rock.Mobile.PlatformCommon.Droid.Context );

                        RelativeLayout prayButtonLayout = new RelativeLayout( Rock.Mobile.PlatformCommon.Droid.Context );
                        prayButtonLayout.LayoutParameters = new RelativeLayout.LayoutParams( ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent );
                        ((RelativeLayout.LayoutParams)prayButtonLayout.LayoutParameters).AddRule( LayoutRules.AlignParentBottom );

                        Pray.Text = "Pray";
                        Pray.LayoutParameters = new RelativeLayout.LayoutParams( ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent );
                        ((RelativeLayout.LayoutParams)Pray.LayoutParameters).AddRule( LayoutRules.CenterHorizontal );
                        prayButtonLayout.AddView( Pray );

                        Pray.Enabled = false;
                        //

                       // set the outline for the card
                        View.BorderColor = 0x777777FF;
                        View.CornerRadius = 4;
                        View.BorderWidth = 1;

                        // add the controls
                        ViewGroup nativeView = View.PlatformNativeObject as ViewGroup;

                        nativeView.AddView( prayButtonLayout );

                        cardView = View;
                    }

                    public void SetPrayer( PrayerLayoutRender prayer )
                    {
                        ViewGroup nativeView = View.PlatformNativeObject as ViewGroup;

                        // if a valid prayer is given
                        if ( prayer != null )
                        {
                            // if it's a child, remove it from its current parent
                            if ( prayer.LinearLayout.Parent != null )
                            {
                                ( prayer.LinearLayout.Parent as ViewGroup ).RemoveView( prayer.LinearLayout );
                            }

                            // add it to this view
                            nativeView.AddView( prayer.LinearLayout );

                            Pray.Enabled = true;
                        }
                        else
                        {
                            // since null was passed, check check for a current prayer
                            if ( CurrentPrayer != null && CurrentPrayer.LinearLayout.Parent != null )
                            {
                                // remove it from ourselves
                                nativeView.RemoveView( CurrentPrayer.LinearLayout );
                            }

                            Pray.Enabled = false;
                        }
                    }
                }

                class PrayerLayoutRender
                {
                    public LinearLayout LinearLayout { get; set; }
                    TextView Name { get; set; }
                    TextView Date { get; set; }
                    TextView Prayer { get; set; }

                    public PrayerLayoutRender( RectangleF bounds, Rock.Client.PrayerRequest prayer )
                    {
                        Name = new TextView( Rock.Mobile.PlatformCommon.Droid.Context );
                        Name.LayoutParameters = new ViewGroup.LayoutParams( ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent );

                        Date = new TextView( Rock.Mobile.PlatformCommon.Droid.Context );
                        Date.LayoutParameters = new ViewGroup.LayoutParams( ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent );

                        Prayer = new TextView( Rock.Mobile.PlatformCommon.Droid.Context );
                        Prayer.LayoutParameters = new ViewGroup.LayoutParams( ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent );

                        // set the text color
                        Name.SetTextColor( Android.Graphics.Color.White );
                        Date.SetTextColor( Android.Graphics.Color.White );
                        Prayer.SetTextColor( Android.Graphics.Color.White );

                        LinearLayout = new LinearLayout( Rock.Mobile.PlatformCommon.Droid.Context );
                        LinearLayout.LayoutParameters = new ViewGroup.LayoutParams( ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent );
                        LinearLayout.LayoutParameters.Width = (int)bounds.Width;
                        LinearLayout.LayoutParameters.Height = (int)bounds.Height;
                        LinearLayout.Orientation = Orientation.Vertical;
                        LinearLayout.AddView( Name );
                        LinearLayout.AddView( Date );

                        LinearLayout.AddView( Prayer );

                        // set the contents
                        Name.Text = prayer.FirstName;
                        Date.Text = string.Format( "{0:dddd MMMM d}, {2:yyyy}", prayer.CreatedDateTime, prayer.CreatedDateTime, prayer.CreatedDateTime );
                        Prayer.Text = prayer.Text;
                    }
                }

                List<Rock.Client.PrayerRequest> PrayerRequests { get; set; }
                List<PrayerLayoutRender> PrayerLayouts { get; set; }

                PlatformCardCarousel Carousel { get; set; }

                PrayerCard SubLeftPrayer { get; set; }
                PrayerCard LeftPrayer { get; set; }
                PrayerCard CenterPrayer { get; set; }
                PrayerCard RightPrayer { get; set; }
                PrayerCard PostRightPrayer { get; set; }
                RectangleF PrayerCardBounds { get; set; }

                Button CreatePrayer { get; set; }
                ProgressBar ActivityIndicator { get; set; }

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

                    View view = inflater.Inflate(Resource.Layout.Prayer_Primary, container, false);
                    view.SetOnTouchListener( this );

                    ActivityIndicator = (ProgressBar)view.FindViewById<ProgressBar>( Resource.Id.prayer_primary_activityIndicator );
                    ActivityIndicator.Visibility = ViewStates.Gone;

                    float viewRealHeight = this.Resources.DisplayMetrics.HeightPixels;

                    float cardSizePerc = .80f;
                    float cardWidth = this.Resources.DisplayMetrics.WidthPixels * cardSizePerc;
                    float cardHeight = viewRealHeight * cardSizePerc;

                    // setup the card positions to be to the offscreen to the left, centered on screen, and offscreen to the right
                    float cardYOffset = ((viewRealHeight - cardHeight) / 2);

                    PrayerCardBounds = new RectangleF( 0, cardYOffset, this.Resources.DisplayMetrics.WidthPixels, viewRealHeight );

                    Carousel = PlatformCardCarousel.Create( cardWidth, cardHeight, PrayerCardBounds, UpdatePrayerCards );

                    // create our cards
                    SubLeftPrayer = new PrayerCard( ref Carousel.SubLeftCard, new RectangleF( 0, 0, cardWidth, cardHeight ) );
                    LeftPrayer = new PrayerCard( ref Carousel.LeftCard, new RectangleF( 0, 0, cardWidth, cardHeight ) );
                    CenterPrayer = new PrayerCard( ref Carousel.CenterCard, new RectangleF( 0, 0, cardWidth, cardHeight ) );
                    RightPrayer = new PrayerCard( ref Carousel.RightCard, new RectangleF( 0, 0, cardWidth, cardHeight ) );
                    PostRightPrayer = new PrayerCard( ref Carousel.PostRightCard, new RectangleF( 0, 0, cardWidth, cardHeight ) );

                    Carousel.Init( view );

                    // add the create prayer button
                    CreatePrayer = (Button) view.FindViewById<Button>( Resource.Id.prayer_primary_createPrayerButton );
                    CreatePrayer.Click += (object sender, EventArgs e ) =>
                        {
                            ParentTask.OnClick( this, Resource.Id.prayer_primary_createPrayerButton );
                        };

                    return view;
                }

                public override void OnResume()
                {
                    base.OnResume();

                    ParentTask.NavbarFragment.NavToolbar.SetBackButtonEnabled( false );
                    ParentTask.NavbarFragment.NavToolbar.Reveal( false );

                    ParentTask.NavbarFragment.NavToolbar.SetShareButtonEnabled( false );
                    ParentTask.NavbarFragment.NavToolbar.DisplayShareButton( false, null );

                    ActivityIndicator.Visibility = ViewStates.Visible;
                    CreatePrayer.Enabled = false;

                    ActivityIndicator.BringToFront( );
                    ResetPrayerCards( );

                    // request the prayers each time this appears
                    CCVApp.Shared.Network.RockApi.Instance.GetPrayers( delegate(System.Net.HttpStatusCode statusCode, string statusDescription, List<Rock.Client.PrayerRequest> prayerRequests) 
                        {
                            ActivityIndicator.Visibility = ViewStates.Gone;
                            CreatePrayer.Enabled = true;

                            if( Rock.Mobile.Network.Util.StatusInSuccessRange( statusCode ) == true )
                            {
                                if( prayerRequests.Count > 0 )
                                {
                                    PrayerRequests = prayerRequests;

                                    // create our prayer request layouts
                                    PrayerLayouts = new List<PrayerLayoutRender>( PrayerRequests.Count );
                                    foreach( Rock.Client.PrayerRequest request in PrayerRequests )
                                    {
                                        PrayerLayoutRender prayerLayout = new PrayerLayoutRender( PrayerCardBounds, request );
                                        PrayerLayouts.Add( prayerLayout );
                                    }

                                    Carousel.NumItems = PrayerRequests.Count;

                                    UpdatePrayerCards( 0 );
                                }
                            }
                            else
                            {
                                Springboard.DisplayError( "Prayer Requests", "There was a problem getting prayer requests. Please try again later." );
                            }
                        });
                }

                // forward these to the carousel
                public override bool OnFlingGesture( MotionEvent e1, MotionEvent e2, float velocityX, float velocityY )
                {
                    return ( (DroidCardCarousel)Carousel ).OnFling( e1, e2, velocityX, velocityY );
                }

                public override bool OnScrollGesture(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
                {
                    return ( (DroidCardCarousel)Carousel ).OnScroll( e1, e2, distanceX, distanceY );
                }

                public override bool OnDownGesture( MotionEvent e )
                {
                    Carousel.TouchesBegan( );
                    return false;
                }

                public override bool OnTouch( View v, MotionEvent e )
                {
                    if ( base.OnTouch( v, e ) == true )
                    {
                        return true;
                    }
                    else
                    {
                        switch ( e.Action )
                        {
                            case MotionEventActions.Up:
                            {
                                Carousel.TouchesEnded( );
                                break;
                            }
                        }

                        return false;
                    }
                }

                void UpdatePrayerCards( int prayerIndex )
                {
                    if( prayerIndex < PrayerRequests.Count )
                    {
                        CenterPrayer.SetPrayer( PrayerLayouts[ prayerIndex ] );

                        // set the left and right in case they want to swipe thru the prayers
                        if( prayerIndex - 2 >= 0 )
                        {
                            SubLeftPrayer.SetPrayer( PrayerLayouts[ prayerIndex - 2 ] );
                        }

                        if( prayerIndex - 1 >= 0 )
                        {
                            LeftPrayer.SetPrayer( PrayerLayouts[ prayerIndex - 1 ] );
                        }

                        if( prayerIndex + 1 < PrayerRequests.Count )
                        {
                            RightPrayer.SetPrayer( PrayerLayouts[ prayerIndex + 1 ] );
                        }

                        if( prayerIndex + 2 < PrayerRequests.Count )
                        {
                            PostRightPrayer.SetPrayer( PrayerLayouts[ prayerIndex + 2 ] );
                        }
                    }
                }

                public void SpringboardClosed( )
                {
                    // we need to know when the springboard closed so we can update the state of our
                    // buttons, which depends on whether we are downloading prayers or not, and
                    // whether we have prayers or not.

                    // if the activity indicator is visible, don't let any buttons work
                    if ( ActivityIndicator.Visibility == ViewStates.Visible )
                    {
                        CreatePrayer.Enabled = false;

                        ResetPrayerCards( );
                    }
                    else if ( PrayerRequests == null || PrayerRequests.Count == 0 )
                    {
                        ResetPrayerCards( );
                    }
                }

                void ResetPrayerCards( )
                {
                    SubLeftPrayer.SetPrayer( null );
                    LeftPrayer.SetPrayer( null );
                    CenterPrayer.SetPrayer( null );
                    RightPrayer.SetPrayer( null );
                    PostRightPrayer.SetPrayer( null );
                }
            }
        }
    }
}
