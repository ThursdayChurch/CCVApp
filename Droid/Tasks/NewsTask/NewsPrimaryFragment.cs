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
using App.Shared.Network;
using Android.Graphics;
using App.Shared;
using System.IO;
using Rock.Mobile.PlatformSpecific.Android.Graphics;
using App.Shared.Config;
using Rock.Mobile.PlatformSpecific.Android.UI;
using App.Shared.PrivateConfig;
using Rock.Mobile.IO;

namespace Droid
{
    namespace Tasks
    {
        namespace News
        {
            public class PortraitNewsArrayAdapter : ListAdapter
            {
                NewsPrimaryFragment ParentFragment { get; set; }

                public PortraitNewsArrayAdapter( NewsPrimaryFragment parentFragment )
                {
                    ParentFragment = parentFragment;
                }

                public override int Count 
                {
                    get { return ParentFragment.News.Count; }
                }

                public override View GetView(int position, View convertView, ViewGroup parent)
                {
                    SingleNewsListItem listItem = convertView as SingleNewsListItem;
                    if ( listItem == null )
                    {
                        listItem = new SingleNewsListItem( Rock.Mobile.PlatformSpecific.Android.Core.Context );
                    }

                    if ( ParentFragment.News[ position ].Image != null )
                    {
                        listItem.Billboard.SetImageBitmap( ParentFragment.News[ position ].Image );
                    }
                    else
                    {
                        listItem.Billboard.SetImageBitmap( ParentFragment.Placeholder );
                    }

                    return base.AddView( listItem );
                }
            }

            public class LandscapeNewsArrayAdapter : ListAdapter
            {
                NewsPrimaryFragment ParentFragment { get; set; }
                public LandscapeNewsArrayAdapter( NewsPrimaryFragment parentFragment )
                {
                    ParentFragment = parentFragment;
                }

                public override int Count 
                {
                    get 
                    { 
                        // start with a top row
                        int numItems = 1;

                        // each row after will show two items
                        double remainingItems = ParentFragment.News.Count - 1;
                        if ( remainingItems > 0 )
                        {
                            // take the rows we'll need and round up
                            double rowsNeeded = remainingItems / 2.0f;

                            rowsNeeded = Math.Ceiling( rowsNeeded );

                            numItems += (int)rowsNeeded;
                        }
                        return numItems;
                    }
                }

                public void RowItemClicked( int itemIndex )
                {
                    ParentFragment.OnClick( itemIndex );
                }

                public override View GetView(int position, View convertView, ViewGroup parent)
                {
                    ListAdapter.ListItemView item = null;

                    if ( position == 0 )
                    {
                        item = GetPrimaryView( position, convertView, parent );
                    }
                    else
                    {
                        // for standard views, subtract one from the position so we
                        // can more easily convert from row index to correct left index image.
                        item = GetStandardView( position - 1, convertView, parent );
                    }

                    return AddView( item );
                }

                ListAdapter.ListItemView GetPrimaryView( int position, View convertView, ViewGroup parent )
                {
                    SingleNewsListItem listItem = convertView as SingleNewsListItem;
                    if ( listItem == null )
                    {
                        listItem = new SingleNewsListItem( Rock.Mobile.PlatformSpecific.Android.Core.Context );
                    }

                    if ( ParentFragment.News[ position ].Image != null )
                    {
                        listItem.Billboard.SetImageBitmap( ParentFragment.News[ position ].Image );
                    }
                    else
                    {
                        listItem.Billboard.SetImageBitmap( ParentFragment.Placeholder );
                    }

                    return listItem;
                }

                ListAdapter.ListItemView GetStandardView( int rowIndex, View convertView, ViewGroup parent )
                {
                    // convert the position to the appropriate image index.
                    int leftImageIndex = 1 + ( rowIndex * 2 );

                    // create the item if needed
                    DoubleNewsListItem seriesItem = convertView as DoubleNewsListItem;
                    if ( seriesItem == null )
                    {
                        seriesItem = new DoubleNewsListItem( Rock.Mobile.PlatformSpecific.Android.Core.Context );
                        seriesItem.ParentAdapter = this;
                    }   

                    seriesItem.LeftImageIndex = leftImageIndex;

                    // first set the left item
                    if ( leftImageIndex < ParentFragment.News.Count )
                    {
                        if ( ParentFragment.News[ leftImageIndex ].Image != null )
                        {
                            seriesItem.LeftImage.SetImageBitmap( ParentFragment.News[ leftImageIndex ].Image );
                        }
                        else
                        {
                            seriesItem.LeftImage.SetImageBitmap( ParentFragment.Placeholder );
                        }
                    }
                    else
                    {
                        seriesItem.LeftImage.SetImageBitmap( ParentFragment.Placeholder );
                    }

                    // now if there's a right item, set it
                    int rightImageIndex = leftImageIndex + 1;
                    if ( rightImageIndex < ParentFragment.News.Count )
                    {
                        if ( ParentFragment.News[ rightImageIndex ].Image != null )
                        {
                            seriesItem.RightImage.SetImageBitmap( ParentFragment.News[ rightImageIndex ].Image );
                        }
                        else
                        {
                            seriesItem.RightImage.SetImageBitmap( ParentFragment.Placeholder );
                        }    
                    }
                    else
                    {
                        seriesItem.RightImage.SetImageBitmap( ParentFragment.Placeholder );
                    }

                    return seriesItem;
                }
            }

            /// <summary>
            /// Implementation of the news row that has a single image
            /// </summary>
            class SingleNewsListItem : Rock.Mobile.PlatformSpecific.Android.UI.ListAdapter.ListItemView
            {
                public AspectScaledImageView Billboard { get; set; }

                public SingleNewsListItem( Context context ) : base( context )
                {
                    Billboard = new AspectScaledImageView( context );
                    Billboard.LayoutParameters = new AbsListView.LayoutParams( ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent );
                    Billboard.SetScaleType( ImageView.ScaleType.CenterCrop );

                    AddView( Billboard );
                }

                public override void Destroy()
                {
                    Billboard.SetImageBitmap( null );
                }
            }

            /// <summary>
            /// Implementation of the news row that has two images side by side
            /// </summary>
            class DoubleNewsListItem : Rock.Mobile.PlatformSpecific.Android.UI.ListAdapter.ListItemView
            {
                public LandscapeNewsArrayAdapter ParentAdapter { get; set; }

                public int LeftImageIndex { get; set; }

                public RelativeLayout LeftLayout { get; set; }
                public Button LeftButton { get; set; }
                public Rock.Mobile.PlatformSpecific.Android.Graphics.AspectScaledImageView LeftImage { get; set; }

                public RelativeLayout RightLayout { get; set; }
                public Button RightButton { get; set; }
                public Rock.Mobile.PlatformSpecific.Android.Graphics.AspectScaledImageView RightImage { get; set; }

                public DoubleNewsListItem( Context context ) : base( context )
                {
                    Orientation = Orientation.Horizontal;

                    SetBackgroundColor( Rock.Mobile.UI.Util.GetUIColor( ControlStylingConfig.BackgroundColor ) );

                    LeftLayout = new RelativeLayout( Rock.Mobile.PlatformSpecific.Android.Core.Context );
                    LeftLayout.LayoutParameters = new LinearLayout.LayoutParams( LayoutParams.WrapContent, LayoutParams.WrapContent );
                    AddView( LeftLayout );

                    LeftImage = new Rock.Mobile.PlatformSpecific.Android.Graphics.AspectScaledImageView( Rock.Mobile.PlatformSpecific.Android.Core.Context );
                    LeftImage.LayoutParameters = new LinearLayout.LayoutParams( NavbarFragment.GetContainerDisplayWidth( ) / 2, LayoutParams.WrapContent );
                    ( (LinearLayout.LayoutParams)LeftImage.LayoutParameters ).Gravity = GravityFlags.CenterVertical;
                    LeftImage.SetScaleType( ImageView.ScaleType.CenterCrop );
                    LeftLayout.AddView( LeftImage );

                    LeftButton = new Button( Rock.Mobile.PlatformSpecific.Android.Core.Context );
                    LeftButton.LayoutParameters = LeftImage.LayoutParameters;
                    LeftButton.Background = null;
                    LeftButton.Click += (object sender, EventArgs e ) =>
                        {
                            // notify our parent that the image index was clicked
                            ParentAdapter.RowItemClicked( LeftImageIndex );
                        };
                    LeftLayout.AddView( LeftButton );


                    RightLayout = new RelativeLayout( Rock.Mobile.PlatformSpecific.Android.Core.Context );
                    RightLayout.LayoutParameters = new LinearLayout.LayoutParams( LayoutParams.WrapContent, LayoutParams.WrapContent );
                    AddView( RightLayout );

                    RightImage = new Rock.Mobile.PlatformSpecific.Android.Graphics.AspectScaledImageView( Rock.Mobile.PlatformSpecific.Android.Core.Context );
                    RightImage.LayoutParameters = new LinearLayout.LayoutParams( NavbarFragment.GetContainerDisplayWidth( ) / 2, LayoutParams.WrapContent );
                    ( (LinearLayout.LayoutParams)RightImage.LayoutParameters ).Gravity = GravityFlags.CenterVertical;
                    RightImage.SetScaleType( ImageView.ScaleType.CenterCrop );
                    RightLayout.AddView( RightImage );

                    RightButton = new Button( Rock.Mobile.PlatformSpecific.Android.Core.Context );
                    RightButton.LayoutParameters = RightImage.LayoutParameters;
                    RightButton.Background = null;
                    RightButton.Click += (object sender, EventArgs e ) =>
                        {
                            // notify our parent that the image index was clicked
                            ParentAdapter.RowItemClicked( LeftImageIndex + 1 );
                        };
                    RightLayout.AddView( RightButton );
                }

                public override void Destroy()
                {
                    LeftImage.SetImageBitmap( null );
                    RightImage.SetImageBitmap( null );
                }
            }

            public class NewsEntry
            {
                public RockNews News { get; set; }
                public Bitmap Image { get; set; }
            }

            public class NewsPrimaryFragment : TaskFragment
            {
                public List<NewsEntry> News { get; set; }
                public List<RockNews> SourceNews { get; set; }

                ListView ListView { get; set; }

                public Bitmap Placeholder { get; set; }

                bool FragmentActive { get; set; }

                public NewsPrimaryFragment( ) : base( )
                {
                    News = new List<NewsEntry>();
                }

                public void OnClick( int position )
                {
                    ParentTask.OnClick( this, position );
                }

                public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
                {
                    if (container == null)
                    {
                        // Currently in a layout without a container, so no reason to create our view.
                        return null;
                    }

                    System.IO.Stream thumbnailStream = Activity.BaseContext.Assets.Open( PrivateGeneralConfig.NewsMainPlaceholder );
                    Placeholder = BitmapFactory.DecodeStream( thumbnailStream );
                    thumbnailStream.Dispose( );

					View view = inflater.Inflate(Resource.Layout.News_Primary, container, false);
                    view.SetOnTouchListener( this );

                    ListView = view.FindViewById<ListView>( Resource.Id.news_primary_list );
                    ListView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => 
                        {
                            // in landscape wide, we only want the first item. All the rest
                            // are handled by the rows themselves
                            if ( MainActivity.SupportsLandscapeWide( ) == true )
                            {
                                if( e.Position == 0 )
                                {
                                    OnClick( e.Position );
                                }
                            }
                            else
                            {
                                // in portrait mode, it's fine to use the list
                                // for click detection
                                OnClick( e.Position );
                            }
                        };
                    ListView.SetOnTouchListener( this );

                    if ( MainActivity.SupportsLandscapeWide( ) == true )
                    {
                        ListView.Adapter = new LandscapeNewsArrayAdapter( this );
                    }
                    else
                    {
                        ListView.Adapter = new PortraitNewsArrayAdapter( this );
                    }

                    ReloadNews( );

                    return view;
                }

                public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
                {
                    base.OnConfigurationChanged(newConfig);

                    if ( MainActivity.SupportsLandscapeWide( ) == true )
                    {
                        ListView.Adapter = new LandscapeNewsArrayAdapter( this );
                    }
                    else
                    {
                        ListView.Adapter = new PortraitNewsArrayAdapter( this );
                    }
                }

                public override void OnResume()
                {
                    base.OnResume();

                    ParentTask.NavbarFragment.NavToolbar.SetBackButtonEnabled( false );
                    ParentTask.NavbarFragment.NavToolbar.SetCreateButtonEnabled( false, null );
                    ParentTask.NavbarFragment.NavToolbar.SetShareButtonEnabled( false, null );
                    ParentTask.NavbarFragment.NavToolbar.Reveal( false );

                    FragmentActive = true;
                }

                void DumpNews( )
                {
                    if ( ListView != null && ListView.Adapter != null )
                    {
                        ( (ListAdapter)ListView.Adapter ).Destroy( );
                    }

                    // be sure to dump the existing news images so
                    // Dalvik knows it can use the memory
                    foreach ( NewsEntry newsEntry in News )
                    {
                        if ( newsEntry.Image != null )
                        {
                            newsEntry.Image.Dispose( );
                            newsEntry.Image = null;
                        }
                    }

                    News.Clear( );
                }

                public void ReloadNews( )
                {
                    DumpNews( );

                    foreach ( RockNews rockEntry in SourceNews )
                    {
                        NewsEntry newsEntry = new NewsEntry();
                        News.Add( newsEntry );

                        newsEntry.News = rockEntry;

                        TryLoadCachedImage( newsEntry );
                    }

                    // if we've already created the list source, refresh it
                    if ( ListView != null && ListView.Adapter != null )
                    {
                        if ( MainActivity.SupportsLandscapeWide( ) == true )
                        {
                            ( ListView.Adapter as LandscapeNewsArrayAdapter ).NotifyDataSetChanged( );
                        }
                        else
                        {
                            ( ListView.Adapter as PortraitNewsArrayAdapter ).NotifyDataSetChanged( );
                        }
                    }
                }

                bool TryLoadCachedImage( NewsEntry entry )
                {
                    bool needImage = false;

                    // check the billboard
                    if ( entry.Image == null )
                    {
                        MemoryStream imageStream = (MemoryStream)FileCache.Instance.LoadFile( entry.News.ImageName );
                        if ( imageStream != null )
                        {
                            try
                            {
                                entry.Image = BitmapFactory.DecodeStream( imageStream );
                            }
                            catch( Exception )
                            {
                                FileCache.Instance.RemoveFile( entry.News.ImageName );
                                Console.WriteLine( "Image {0} was corrupt. Removing.", entry.News.ImageName );
                            }

                            imageStream.Dispose( );
                            imageStream = null;
                        }
                        else
                        {
                            needImage = true;
                        }
                    }

                    return needImage;
                }

                public void DownloadImages( )
                {
                    foreach ( NewsEntry newsEntry in News )
                    {
                        if ( newsEntry.Image == null )
                        {
                            FileCache.Instance.DownloadFileToCache( newsEntry.News.ImageURL, newsEntry.News.ImageName, delegate { SeriesImageDownloaded( ); } );
                            FileCache.Instance.DownloadFileToCache( newsEntry.News.HeaderImageURL, newsEntry.News.HeaderImageName, null );
                        }
                    }
                }

                void SeriesImageDownloaded( )
                {
                    Rock.Mobile.Threading.Util.PerformOnUIThread( delegate
                        {
                            // using only the cache, try to load any image that isn't loaded
                            foreach ( NewsEntry entry in News )
                            {
                                TryLoadCachedImage( entry );
                            }

                            if ( FragmentActive == true )
                            {
                                if ( MainActivity.SupportsLandscapeWide( ) == true )
                                {
                                    ( ListView.Adapter as LandscapeNewsArrayAdapter ).NotifyDataSetChanged( );
                                }
                                else
                                {
                                    ( ListView.Adapter as PortraitNewsArrayAdapter ).NotifyDataSetChanged( );
                                }
                            }
                        } );
                }

                public override void OnPause()
                {
                    base.OnPause();

                    FragmentActive = false;
                }

                public override void OnDestroyView()
                {
                    base.OnDestroyView();

                    DumpNews( );

                    Placeholder.Dispose( );
                    Placeholder = null;
                }
            }
        }
    }
}

