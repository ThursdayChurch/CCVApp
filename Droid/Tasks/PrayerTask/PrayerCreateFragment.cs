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
using System.Drawing;
using CCVApp.Shared.Strings;
using CCVApp.Shared.Config;
using CCVApp.Shared.Analytics;
using Rock.Mobile.PlatformSpecific.Android.Animation;

namespace Droid
{
    namespace Tasks
    {
        namespace Prayer
        {
            public class SpinnerArrayAdapter : ArrayAdapter
            {
                int ResourceId { get; set; }
                public SpinnerArrayAdapter( Context context, int resourceId ) : base( context, resourceId )
                {
                    ResourceId = resourceId;
                }

                public override View GetView(int position, View convertView, ViewGroup parent)
                {
                    if ( convertView as TextView == null )
                    {
                        convertView = ( Context as Activity ).LayoutInflater.Inflate( ResourceId, parent, false );
                        ControlStyling.StyleUILabel( (convertView as TextView), ControlStylingConfig.Medium_Font_Regular, ControlStylingConfig.Medium_FontSize );
                    }

                    ( convertView as TextView ).Text = this.GetItem( position ).ToString( );

                    return convertView;
                }
            }

            public class PrayerCreateFragment : TaskFragment
            {
                EditText FirstNameText { get; set; }
                RelativeLayout FirstNameBGLayer { get; set; }
                uint FirstNameBGColor { get; set; }

                EditText LastNameText { get; set; }

                EditText RequestText { get; set; }
                RelativeLayout RequestBGLayer { get; set; }
                uint RequestBGColor { get; set; }

                Switch AnonymousSwitch { get; set; }
                Switch PublicSwitch { get; set; }

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

                    View view = inflater.Inflate(Resource.Layout.Prayer_Create, container, false);
                    view.SetOnTouchListener( this );

                    view.SetBackgroundColor( Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.BackgroundColor ) );

                    // setup the first name background
                    FirstNameBGLayer = view.FindViewById<RelativeLayout>( Resource.Id.first_name_background );
                    ControlStyling.StyleBGLayer( FirstNameBGLayer );
                    //

                    RelativeLayout lastNameBGLayer = view.FindViewById<RelativeLayout>( Resource.Id.last_name_background );
                    ControlStyling.StyleBGLayer( lastNameBGLayer );

                    // setup the prayer request background
                    RequestBGLayer = view.FindViewById<RelativeLayout>( Resource.Id.prayerRequest_background );
                    ControlStyling.StyleBGLayer( RequestBGLayer );
                    //

                    // setup the switch background
                    RelativeLayout backgroundLayout = view.FindViewById<RelativeLayout>( Resource.Id.switch_background );
                    ControlStyling.StyleBGLayer( backgroundLayout );

                    // setup the category background
                    backgroundLayout = view.FindViewById<RelativeLayout>( Resource.Id.spinner_background );
                    ControlStyling.StyleBGLayer( backgroundLayout );

                    // setup the text views
                    FirstNameText = (EditText)view.FindViewById<EditText>( Resource.Id.prayer_create_firstNameText );
                    ControlStyling.StyleTextField( FirstNameText, PrayerStrings.CreatePrayer_FirstNamePlaceholderText, ControlStylingConfig.Medium_Font_Regular, ControlStylingConfig.Medium_FontSize );
                    FirstNameBGColor = ControlStylingConfig.BG_Layer_Color;

                    LastNameText = (EditText)view.FindViewById<EditText>( Resource.Id.prayer_create_lastNameText );
                    ControlStyling.StyleTextField( LastNameText, PrayerStrings.CreatePrayer_LastNamePlaceholderText, ControlStylingConfig.Medium_Font_Regular, ControlStylingConfig.Medium_FontSize );

                    RequestText = (EditText)view.FindViewById<EditText>( Resource.Id.prayer_create_requestText );
                    ControlStyling.StyleTextField( RequestText, PrayerStrings.CreatePrayer_PrayerRequest, ControlStylingConfig.Medium_Font_Regular, ControlStylingConfig.Medium_FontSize );
                    RequestBGColor = ControlStylingConfig.BG_Layer_Color;


                    AnonymousSwitch = (Switch)view.FindViewById<Switch>( Resource.Id.postAnonymousSwitch );
                    AnonymousSwitch.Checked = false;
                    AnonymousSwitch.CheckedChange += (object sender, CompoundButton.CheckedChangeEventArgs e ) =>
                    {
                            if( AnonymousSwitch.Checked == false )
                            {
                                FirstNameText.Enabled = true;
                                LastNameText.Enabled = true;

                                FirstNameText.SetTextColor( Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.TextField_ActiveTextColor ) );
                                LastNameText.SetTextColor( Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.TextField_ActiveTextColor ) );
                            }
                            else
                            {
                                FirstNameText.Enabled = false;
                                LastNameText.Enabled = false;

                                FirstNameText.SetTextColor( Android.Graphics.Color.DimGray );
                                LastNameText.SetTextColor( Android.Graphics.Color.DimGray );
                            }
                    };

                    PublicSwitch = (Switch)view.FindViewById<Switch>( Resource.Id.makePublicSwitch );
                    PublicSwitch.Checked = true;

                    TextView postAnonymousLabel = view.FindViewById<TextView>( Resource.Id.postAnonymous );
                    ControlStyling.StyleUILabel( postAnonymousLabel, ControlStylingConfig.Medium_Font_Regular, ControlStylingConfig.Medium_FontSize );

                    TextView publicLabel = view.FindViewById<TextView>( Resource.Id.makePublic );
                    ControlStyling.StyleUILabel( publicLabel, ControlStylingConfig.Medium_Font_Regular, ControlStylingConfig.Medium_FontSize );

                    // setup our category spinner
                    Spinner spinner = (Spinner)view.FindViewById<Spinner>( Resource.Id.categorySpinner );
                    ArrayAdapter adapter = new SpinnerArrayAdapter( Rock.Mobile.PlatformSpecific.Android.Core.Context, Android.Resource.Layout.SimpleListItem1 );
                    adapter.SetDropDownViewResource( Android.Resource.Layout.SimpleSpinnerDropDownItem );
                    spinner.Adapter = adapter;

                    // populate the category
                    foreach ( string category in CCVApp.Shared.Network.RockGeneralData.Instance.Data.PrayerCategories )
                    {
                        adapter.Add( category );
                    }

                    Button submitButton = (Button)view.FindViewById<Button>( Resource.Id.prayer_create_submitButton );
                    ControlStyling.StyleButton( submitButton, PrayerStrings.CreatePrayer_SubmitButtonText, ControlStylingConfig.Small_Font_Regular, ControlStylingConfig.Small_FontSize );
                    submitButton.Click += (object sender, EventArgs e ) =>
                    {
                        SubmitPrayerRequest( );
                    };

                    return view;
                }

                void SubmitPrayerRequest( )
                {
                    if ( ( string.IsNullOrEmpty( FirstNameText.Text ) == false || AnonymousSwitch.Checked == true ) &&
                        string.IsNullOrEmpty( RequestText.Text ) == false )
                    {
                        Rock.Client.PrayerRequest prayerRequest = new Rock.Client.PrayerRequest();

                        FirstNameText.Enabled = false;
                        LastNameText.Enabled = false;
                        RequestText.Enabled = false;

                        // respect their privacy settings
                        if ( AnonymousSwitch.Checked == true )
                        {
                            prayerRequest.FirstName = "Anonymous";
                            prayerRequest.LastName = "Anonymous";
                        }
                        else
                        {
                            prayerRequest.FirstName = FirstNameText.Text;
                            prayerRequest.LastName = LastNameText.Text;
                        }

                        prayerRequest.Text = RequestText.Text;
                        prayerRequest.EnteredDateTime = DateTime.Now;
                        prayerRequest.ExpirationDate = DateTime.Now.AddYears( 1 );
                        prayerRequest.CategoryId = 110; //todo: Let the end user set this.
                        prayerRequest.IsActive = true;
                        prayerRequest.IsPublic = PublicSwitch.Checked;
                        prayerRequest.IsApproved = false;


                        ParentTask.OnClick( this, 0, prayerRequest );
                    }
                    else
                    {
                        // they forgot to fill something in, so show them what it was.

                        // Update the name background color
                        uint currNameColor = FirstNameBGColor;

                        // if they left the name field blank and didn't turn on Anonymous, flag the field.
                        uint targetNameColor = ControlStylingConfig.BG_Layer_Color; 
                        if( string.IsNullOrEmpty( FirstNameText.Text ) && AnonymousSwitch.Checked == false )
                        {
                            targetNameColor = ControlStylingConfig.BadInput_BG_Layer_Color;
                        }

                        SimpleAnimator_Color nameAnimator = new SimpleAnimator_Color( currNameColor, targetNameColor, .15f, delegate(float percent, object value )
                            {
                                FirstNameBGLayer.SetBackgroundColor( Rock.Mobile.PlatformUI.Util.GetUIColor( (uint)value ) );
                            }
                            ,
                            delegate
                            {
                                FirstNameBGColor = targetNameColor;
                            } );
                        nameAnimator.Start( );


                        // Update the prayer background color
                        uint currPrayerColor = RequestBGColor;
                        uint targetPrayerColor = string.IsNullOrEmpty( RequestText.Text ) ? ControlStylingConfig.BadInput_BG_Layer_Color : ControlStylingConfig.BG_Layer_Color;

                        SimpleAnimator_Color prayerAnimator = new SimpleAnimator_Color( currPrayerColor, targetPrayerColor, .15f, delegate(float percent, object value )
                            {
                                RequestBGLayer.SetBackgroundColor( Rock.Mobile.PlatformUI.Util.GetUIColor( (uint)value ) );
                            }
                            ,
                            delegate
                            {
                                RequestBGColor = targetPrayerColor;
                            } );
                        prayerAnimator.Start( );
                    }
                }

                public override void OnResume()
                {
                    base.OnResume();

                    ParentTask.NavbarFragment.NavToolbar.SetBackButtonEnabled( true );
                    ParentTask.NavbarFragment.NavToolbar.SetShareButtonEnabled( false, null );
                    ParentTask.NavbarFragment.NavToolbar.SetCreateButtonEnabled( false, null );
                    ParentTask.NavbarFragment.NavToolbar.RevealForTime( 3.0f );
                }
            }
        }
    }
}
