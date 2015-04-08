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
using Android.Media;
using CCVApp.Shared.Strings;
using Rock.Mobile.PlatformUI;
using CCVApp.Shared;
using CCVApp.Shared.Analytics;

namespace Droid
{
    namespace Tasks
    {
        namespace Notes
        {
            // define the type of class the binder will be
            public class AudioServiceBinder : Binder
            {
                public AudioService Service { get; protected set; }
                public AudioServiceBinder( AudioService service )
                {
                    Console.WriteLine( "AudioServiceBinder created" );
                    Service = service;
                }
            }

            public class AudioServiceConnection : Java.Lang.Object, IServiceConnection
            {
                //IBinder ServiceBinder { get; set; }
                NotesListenFragment ConnectionFragment { get; set; }

                public AudioServiceConnection( NotesListenFragment connectionFragment )
                {
                    Console.WriteLine( "AudioServiceConnection created" );
                    ConnectionFragment = connectionFragment;
                }

                public void OnServiceConnected( ComponentName name, IBinder serviceBinder )
                {
                    Console.WriteLine( "OnServiceConnected - We are binding" );

                    AudioServiceBinder binder = serviceBinder as AudioServiceBinder;
                    if ( binder != null )
                    {
                        ConnectionFragment.ServiceConnected( binder );
                    }
                }

                public void OnServiceDisconnected( ComponentName name )
                {
                    Console.WriteLine( "OnServiceDisconnected - We are unbinding" );
                    ConnectionFragment.ServiceDisconnected( );
                }
            }

            [Service]
            public class AudioService : Service
            {
                public MediaPlayer MediaPlayer { get; set; }

                // create our binder object that will be used to pass an instance of ourselves around
                IBinder Binder;

                public AudioService( ) : base(  )
                {
                    Console.WriteLine( "AudioService::()" );
                }

                public override void OnCreate()
                {
                    base.OnCreate();

                    Console.WriteLine( "AudioService::OnCreate" );

                    Binder = new AudioServiceBinder( this );

                    // prepare our media player
                    MediaPlayer = new MediaPlayer();
                    MediaPlayer.SetAudioStreamType( Stream.Music );
                    MediaPlayer.Stop( );
                }

                public override void OnDestroy()
                {
                    base.OnDestroy();

                    MediaPlayer.Stop( );
                }

                // when bound, return the Binder object containing our instance
                public override IBinder OnBind(Intent intent)
                {
                    Console.WriteLine( "AudioService::OnBind" );
                    return Binder;
                }
            }



            public class NotesListenFragment : TaskFragment, Android.Media.MediaPlayer.IOnPreparedListener, Android.Media.MediaPlayer.IOnErrorListener, Android.Media.MediaPlayer.IOnSeekCompleteListener, Android.Widget.MediaController.IMediaPlayerControl
            {
                MediaController MediaController { get; set; }
                ProgressBar ProgressBar { get; set; }

                AudioServiceBinder AudioServiceBinder { get; set; }
                AudioServiceConnection AudioServiceConnection { get; set; }

                public string MediaUrl { get; set; }
                public string ShareUrl { get; set; }
                public string Name { get; set; }

                // used so that we know how to setup the UI / service
                // when being created or resumed.
                enum MediaPlayerState
                {
                    None,
                    WantsAutoplay,
                    Preparing,
                    Playing,
                    Stopped
                };
                MediaPlayerState PlayerState { get; set; }

                public void ServiceConnected( AudioServiceBinder binder )
                {
                    Console.WriteLine( "ServiceConnected." );
                    AudioServiceBinder = binder;

                    if ( PlayerState == MediaPlayerState.WantsAutoplay )
                    {
                        StartAudio( );

                        PlayerState = MediaPlayerState.Preparing;
                    }
                }

                public void ServiceDisconnected( )
                {
                    Console.WriteLine( "Service Disconnected." );
                    AudioServiceBinder = null;
                }

                void StartAudio( )
                {
                    if ( AudioServiceBinder != null )
                    {
                        AudioServiceBinder.Service.MediaPlayer.SetDataSource( Rock.Mobile.PlatformSpecific.Android.Core.Context, Android.Net.Uri.Parse( MediaUrl ) );
                        AudioServiceBinder.Service.MediaPlayer.SetOnPreparedListener( this );
                        AudioServiceBinder.Service.MediaPlayer.PrepareAsync( );
                    }
                }

                void StopAudio( )
                {
                    if( AudioServiceBinder != null )
                    {
                        AudioServiceBinder.Service.MediaPlayer.Stop( );
                    }
                }

                public override void OnCreate( Bundle savedInstanceState )
                {
                    base.OnCreate( savedInstanceState );

                    Console.WriteLine( "OnCreate - Starting Audio Service" );

                    // flag that we want to automatically start the audio.
                    // If this fragment is backgrounded and resumed, this will be false and we won't
                    // attempt to play a second time.
                    PlayerState = MediaPlayerState.WantsAutoplay;

                    // start our audio service
                    Activity.StartService( new Intent( Activity, typeof( AudioService ) ) );

                    // create our connector that manages the interface between us and the service
                    AudioServiceConnection = new AudioServiceConnection( this );
                }

                public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
                {
                    if (container == null)
                    {
                        // Currently in a layout without a container, so no reason to create our view.
                        return null;
                    }

                    RelativeLayout view = new RelativeLayout( Rock.Mobile.PlatformSpecific.Android.Core.Context );
                    view.LayoutParameters = new RelativeLayout.LayoutParams( ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent );
                    view.SetBackgroundColor( Android.Graphics.Color.Black );
                    view.SetOnTouchListener( this );

                    ProgressBar = new ProgressBar( Rock.Mobile.PlatformSpecific.Android.Core.Context );
                    ProgressBar.Indeterminate = true;
                    ProgressBar.SetBackgroundColor( Rock.Mobile.PlatformUI.Util.GetUIColor( 0 ) );
                    ProgressBar.LayoutParameters = new RelativeLayout.LayoutParams( ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent );
                    ( (RelativeLayout.LayoutParams)ProgressBar.LayoutParameters ).AddRule( LayoutRules.CenterInParent );
                    view.AddView( ProgressBar );

                    // setup our media controller for viewing the position of media
                    MediaController = new MediaController( Rock.Mobile.PlatformSpecific.Android.Core.Context );
                    MediaController.SetAnchorView( view );
                    MediaController.SetMediaPlayer( this );

                    return view;
                }

                public override void OnResume()
                {
                    base.OnResume();

                    ParentTask.NavbarFragment.NavToolbar.SetBackButtonEnabled( true );
                    ParentTask.NavbarFragment.NavToolbar.SetCreateButtonEnabled( false, null );
                    ParentTask.NavbarFragment.NavToolbar.Reveal( true );

                    ParentTask.NavbarFragment.NavToolbar.SetShareButtonEnabled( true, delegate 
                        {
                            // Generate an email advertising this video.
                            Intent sendIntent = new Intent();
                            sendIntent.SetAction( Intent.ActionSend );

                            sendIntent.PutExtra( Intent.ExtraSubject, MessagesStrings.Watch_Share_Subject );

                            string noteString = MessagesStrings.Watch_Share_Header_Html + string.Format( MessagesStrings.Watch_Share_Body_Html, ShareUrl );

                            // if they set a mobile app url, add that.
                            if( string.IsNullOrEmpty( MessagesStrings.Watch_Mobile_App_Url ) == false )
                            {
                                noteString += string.Format( MessagesStrings.Watch_Share_DownloadApp_Html, MessagesStrings.Watch_Mobile_App_Url );
                            }

                            sendIntent.PutExtra( Intent.ExtraText, Android.Text.Html.FromHtml( noteString ) );
                            sendIntent.SetType( "text/html" );
                            StartActivity( sendIntent );
                        });

                    if ( string.IsNullOrEmpty( MediaUrl ) == true )
                    {
                        throw new Exception( "MediaUrl must be valid." );
                    }

                    Console.WriteLine( "OnResume - Binding to audio service" );

                    // bind ourselves to the audio service
                    Activity.BindService( new Intent( Activity, typeof( AudioService )  ), AudioServiceConnection, Bind.AutoCreate );

                    SyncUI( );
                }

                public override void OnPause()
                {
                    base.OnPause();

                    Console.WriteLine( "OnPause - UNbinding audio service" );

                    ParentTask.NavbarFragment.EnableSpringboardRevealButton( true );
                    ParentTask.NavbarFragment.ToggleFullscreen( false );

                    // see if we should store the playback position for resuming
                    TrySaveMediaPosition( );

                    Activity.UnbindService( AudioServiceConnection );
                }

                public override void OnDestroy()
                {
                    base.OnDestroy();

                    Console.WriteLine( "OnDestroy - Stopping Audio Service" );

                    // attempt to store the media position. This works because unbinding the service doesn't
                    // necessarily disconnect us, so we might still have a valid reference to the service
                    // and thus the media position.
                    TrySaveMediaPosition( );

                    // if our activity is being destroyed, kill the audio service too
                    Activity.StopService( new Intent( Activity, typeof( AudioService ) ) );
                }

                void TrySaveMediaPosition( )
                {
                    if ( Duration > 0 )
                    {
                        // if we're within 10 and 90 percent, do it
                        float playbackPerc = (float)CurrentPosition / (float)Duration;
                        if ( playbackPerc > .10f && playbackPerc < .95f )
                        {
                            CCVApp.Shared.Network.RockMobileUser.Instance.LastStreamingMediaPos = CurrentPosition;
                        }
                        else
                        {
                            // otherwise plan on starting from the beginning
                            CCVApp.Shared.Network.RockMobileUser.Instance.LastStreamingMediaPos = 0;
                        }
                    }
                }

                public override bool OnTouch( View v, MotionEvent e )
                {
                    MediaController.Show( );

                    return false;
                }

                void SyncUI( )
                {
                    // based on the player state, configure our UI
                    switch ( PlayerState )
                    {
                        case MediaPlayerState.Preparing:
                        case MediaPlayerState.WantsAutoplay:
                        {
                            ProgressBar.Visibility = ViewStates.Visible;
                            ProgressBar.BringToFront( );

                            MediaController.Hide( );
                            break;
                        }

                        case MediaPlayerState.Stopped:
                        case MediaPlayerState.Playing:
                        {
                            // hide the progress bar
                            ProgressBar.Visibility = ViewStates.Gone;

                            //todo: bring up media controls
                            MediaController.Show( );
                            break;
                        }
                    }
                }

                public void OnPrepared( MediaPlayer mp )
                {
                    Console.WriteLine( "OnPrepared - Audio ready to play" );

                    // setup a seek listener
                    mp.SetOnSeekCompleteListener( this );

                    // log the series they tapped on.
                    MessageAnalytic.Instance.Trigger( MessageAnalytic.Listen, Name );

                    // if this is a new video, store the URL
                    if ( CCVApp.Shared.Network.RockMobileUser.Instance.LastStreamingMediaUrl != MediaUrl )
                    {
                        CCVApp.Shared.Network.RockMobileUser.Instance.LastStreamingMediaUrl = MediaUrl;

                        PlayerState = MediaPlayerState.Playing;
                        mp.Start( );

                        SyncUI( );
                    }
                    else
                    {
                        // otherwise, resume where we left off
                        mp.SeekTo( (int)CCVApp.Shared.Network.RockMobileUser.Instance.LastStreamingMediaPos );
                    }
                }

                public void OnSeekComplete( MediaPlayer mp )
                {
                    PlayerState = MediaPlayerState.Playing;
                    mp.Start( );

                    SyncUI( );
                }

                public bool OnError( MediaPlayer mp, MediaError error, int extra )
                {
                    ProgressBar.Visibility = ViewStates.Gone;
                    Springboard.DisplayError( MessagesStrings.Error_Title, MessagesStrings.Error_Watch_Playback );

                    PlayerState = MediaPlayerState.Stopped;

                    SyncUI( );

                    return true;
                }

                public bool CanPause( )
                {
                    if ( AudioServiceBinder != null )
                    {
                        return true;
                    }

                    return false;
                }

                public bool CanSeekBackward( )
                {
                    if ( AudioServiceBinder != null )
                    {
                        return true;
                    }

                    return false;
                }

                public bool CanSeekForward( )
                {
                    if ( AudioServiceBinder != null )
                    {
                        return true;
                    }

                    return false;
                }

                public int AudioSessionId
                {
                    get
                    {
                        if ( AudioServiceBinder != null )
                        {
                            return AudioServiceBinder.Service.MediaPlayer.AudioSessionId;
                        }

                        return 0;
                    }
                }

                public int BufferPercentage
                {
                    get
                    {
                        if ( AudioServiceBinder != null )
                        {
                            return 0;
                        }

                        return 0;
                    }
                }

                public int CurrentPosition
                {
                    get
                    {
                        if ( AudioServiceBinder != null )
                        {
                            return AudioServiceBinder.Service.MediaPlayer.CurrentPosition;
                        }

                        return 0;
                    }
                }

                public int Duration
                {
                    get
                    {
                        if ( AudioServiceBinder != null )
                        {
                            return AudioServiceBinder.Service.MediaPlayer.Duration;
                        }
                        return 0;
                    }
                }

                public bool IsPlaying
                {
                    get
                    {
                        if ( AudioServiceBinder != null )
                        {
                            return AudioServiceBinder.Service.MediaPlayer.IsPlaying;
                        }
                        return false;
                    }
                }

                public void Pause( )
                {
                    if ( AudioServiceBinder != null )
                    {
                        AudioServiceBinder.Service.MediaPlayer.Pause( );
                    }
                }

                public void SeekTo( int pos )
                {
                    if ( AudioServiceBinder != null )
                    {
                        AudioServiceBinder.Service.MediaPlayer.SeekTo( pos );
                    }
                }

                public void Start( )
                {
                    if ( AudioServiceBinder != null )
                    {
                        AudioServiceBinder.Service.MediaPlayer.Start( );
                    }
                }
            }
        }
    }
}

