using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using CCVApp.Shared.Network;
using CoreGraphics;
using Rock.Mobile.Network;
using CCVApp.Shared.Notes.Model;
using System.Xml;
using System.IO;
using RestSharp;
using CCVApp.Shared.Config;
using CCVApp.Shared.Strings;
using Rock.Mobile.PlatformUI;
using System.Net;
using CCVApp.Shared;
using System.Threading.Tasks;
using System.Threading;

namespace iOS
{
    partial class NotesMainUIViewController : TaskUIViewController
    {
        public class TableSource : UITableViewSource 
        {
            /// <summary>
            /// Definition for the primary (top) cell, which advertises the current series
            /// more prominently
            /// </summary>
            class SeriesPrimaryCell : UITableViewCell
            {
                public static string Identifier = "SeriesPrimaryCell";

                public TableSource Parent { get; set; }

                public UIImageView Image { get; set; }


                public UILabel Title { get; set; }
                public UILabel Date { get; set; }
                public UILabel Speaker { get; set; }

                public UIButton WatchButton { get; set; }
                public UILabel WatchButtonIcon { get; set; }
                public UILabel WatchButtonLabel { get; set; }

                public UIButton TakeNotesButton { get; set; }
                public UILabel TakeNotesButtonIcon { get; set; }
                public UILabel TakeNotesButtonLabel { get; set; }

                public UILabel BottomBanner { get; set; }

                public SeriesPrimaryCell( UITableViewCellStyle style, string cellIdentifier ) : base( style, cellIdentifier )
                {
                    BackgroundColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.BG_Layer_Color );

                    Image = new UIImageView( );
                    Image.ContentMode = UIViewContentMode.ScaleAspectFit;
                    Image.Layer.AnchorPoint = CGPoint.Empty;
                    AddSubview( Image );

                    Title = new UILabel( );
                    Title.Font = Rock.Mobile.PlatformSpecific.iOS.Graphics.FontManager.GetFont( ControlStylingConfig.Large_Font_Bold, ControlStylingConfig.Large_FontSize );
                    Title.Layer.AnchorPoint = CGPoint.Empty;
                    Title.TextColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.TextField_ActiveTextColor );
                    Title.BackgroundColor = UIColor.Clear;
                    Title.LineBreakMode = UILineBreakMode.TailTruncation;
                    AddSubview( Title );

                    Date = new UILabel( );
                    Date.Font = Rock.Mobile.PlatformSpecific.iOS.Graphics.FontManager.GetFont( ControlStylingConfig.Small_Font_Regular, ControlStylingConfig.Small_FontSize );
                    Date.Layer.AnchorPoint = CGPoint.Empty;
                    Date.TextColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.TextField_PlaceholderTextColor );
                    Date.BackgroundColor = UIColor.Clear;
                    Date.LineBreakMode = UILineBreakMode.TailTruncation;
                    AddSubview( Date );

                    Speaker = new UILabel( );
                    Speaker.Font = Rock.Mobile.PlatformSpecific.iOS.Graphics.FontManager.GetFont( ControlStylingConfig.Small_Font_Regular, ControlStylingConfig.Small_FontSize );
                    Speaker.Layer.AnchorPoint = CGPoint.Empty;
                    Speaker.TextColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.TextField_PlaceholderTextColor );
                    Speaker.BackgroundColor = UIColor.Clear;
                    Speaker.LineBreakMode = UILineBreakMode.TailTruncation;
                    AddSubview( Speaker );

                    WatchButton = new UIButton( UIButtonType.Custom );
                    WatchButton.TouchUpInside += (object sender, EventArgs e) => { Parent.WatchButtonClicked( ); };
                    WatchButton.Layer.AnchorPoint = CGPoint.Empty;
                    WatchButton.BackgroundColor = UIColor.Clear;
                    WatchButton.Layer.BorderColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.BG_Layer_BorderColor ).CGColor;
                    WatchButton.Layer.BorderWidth = 1;
                    WatchButton.SizeToFit( );
                    AddSubview( WatchButton );

                    WatchButtonIcon = new UILabel( );
                    WatchButton.AddSubview( WatchButtonIcon );
                    WatchButtonIcon.Font = Rock.Mobile.PlatformSpecific.iOS.Graphics.FontManager.GetFont( ControlStylingConfig.Icon_Font_Secondary, NoteConfig.Series_Table_IconSize );
                    WatchButtonIcon.Text = NoteConfig.Series_Table_Watch_Icon;
                    WatchButtonIcon.SizeToFit( );

                    WatchButtonLabel = new UILabel( );
                    WatchButton.AddSubview( WatchButtonLabel );
                    WatchButtonLabel.Font = Rock.Mobile.PlatformSpecific.iOS.Graphics.FontManager.GetFont( ControlStylingConfig.Small_Font_Regular, ControlStylingConfig.Small_FontSize );
                    WatchButtonLabel.Text = MessagesStrings.Series_Table_Watch;
                    WatchButtonLabel.SizeToFit( );
                    

                    TakeNotesButton = new UIButton( UIButtonType.Custom );
                    TakeNotesButton.TouchUpInside += (object sender, EventArgs e) => { Parent.TakeNotesButtonClicked( ); };
                    TakeNotesButton.Layer.AnchorPoint = CGPoint.Empty;
                    TakeNotesButton.BackgroundColor = UIColor.Clear;
                    TakeNotesButton.Layer.BorderColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.BG_Layer_BorderColor ).CGColor;
                    TakeNotesButton.Layer.BorderWidth = 1;
                    TakeNotesButton.SizeToFit( );
                    AddSubview( TakeNotesButton );


                    TakeNotesButtonIcon = new UILabel( );
                    TakeNotesButton.AddSubview( TakeNotesButtonIcon );
                    TakeNotesButtonIcon.Font = Rock.Mobile.PlatformSpecific.iOS.Graphics.FontManager.GetFont( ControlStylingConfig.Icon_Font_Secondary, NoteConfig.Series_Table_IconSize );
                    TakeNotesButtonIcon.Text = NoteConfig.Series_Table_TakeNotes_Icon;
                    TakeNotesButtonIcon.SizeToFit( );

                    TakeNotesButtonLabel = new UILabel( );
                    TakeNotesButton.AddSubview( TakeNotesButtonLabel );
                    TakeNotesButtonLabel.Font = Rock.Mobile.PlatformSpecific.iOS.Graphics.FontManager.GetFont( ControlStylingConfig.Small_Font_Regular, ControlStylingConfig.Small_FontSize );
                    TakeNotesButtonLabel.Text = MessagesStrings.Series_Table_TakeNotes;
                    TakeNotesButtonLabel.SizeToFit( );


                    BottomBanner = new UILabel( );
                    BottomBanner.Font = Rock.Mobile.PlatformSpecific.iOS.Graphics.FontManager.GetFont( ControlStylingConfig.Small_Font_Regular, ControlStylingConfig.Small_FontSize );
                    BottomBanner.Layer.AnchorPoint = new CGPoint( 0, 0 );
                    BottomBanner.Text = MessagesStrings.Series_Table_PreviousMessages;
                    BottomBanner.TextColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.TextField_PlaceholderTextColor );
                    BottomBanner.BackgroundColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.Table_Footer_Color );
                    BottomBanner.TextAlignment = UITextAlignment.Center;
                    AddSubview( BottomBanner );
                }

                public void ToggleWatchButton( bool enabled )
                {
                    if ( enabled == true )
                    {
                        WatchButton.Enabled = true;
                        WatchButtonIcon.TextColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.TextField_ActiveTextColor );
                        WatchButtonLabel.TextColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.TextField_ActiveTextColor );
                    }
                    else
                    {
                        WatchButton.Enabled = false;
                        WatchButtonIcon.TextColor = UIColor.DarkGray;
                        WatchButtonLabel.TextColor = UIColor.DarkGray;
                    }
                }

                public void ToggleTakeNotesButton( bool enabled )
                {
                    if ( enabled == true )
                    {
                        TakeNotesButton.Enabled = true;
                        TakeNotesButtonIcon.TextColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.TextField_ActiveTextColor );
                        TakeNotesButtonLabel.TextColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.TextField_ActiveTextColor );
                    }
                    else
                    {
                        TakeNotesButton.Enabled = false;
                        TakeNotesButtonIcon.TextColor = UIColor.DarkGray;
                        TakeNotesButtonLabel.TextColor = UIColor.DarkGray;
                    }
                }
            }

            /// <summary>
            /// Definition for each cell in this table
            /// </summary>
            class SeriesCell : UITableViewCell
            {
                public static string Identifier = "SeriesCell";

                public TableSource Parent { get; set; }

                public UIImageView Image { get; set; }
                public UILabel Title { get; set; }
                public UILabel Date { get; set; }
                public UILabel Chevron { get; set; }

                public UIView Seperator { get; set; }

                public SeriesCell( UITableViewCellStyle style, string cellIdentifier ) : base( style, cellIdentifier )
                {
                    Image = new UIImageView( );
                    Image.ContentMode = UIViewContentMode.ScaleAspectFit;
                    Image.Layer.AnchorPoint = CGPoint.Empty;
                    AddSubview( Image );

                    Title = new UILabel( );
                    Title.Font = Rock.Mobile.PlatformSpecific.iOS.Graphics.FontManager.GetFont( ControlStylingConfig.Medium_Font_Regular, ControlStylingConfig.Medium_FontSize );
                    Title.Layer.AnchorPoint = CGPoint.Empty;
                    Title.TextColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.Label_TextColor );
                    Title.BackgroundColor = UIColor.Clear;
                    Title.LineBreakMode = UILineBreakMode.TailTruncation;
                    AddSubview( Title );

                    Date = new UILabel( );
                    Date.Font = Rock.Mobile.PlatformSpecific.iOS.Graphics.FontManager.GetFont( ControlStylingConfig.Small_Font_Regular, ControlStylingConfig.Small_FontSize );
                    Date.Layer.AnchorPoint = CGPoint.Empty;
                    Date.TextColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.TextField_PlaceholderTextColor );
                    Date.BackgroundColor = UIColor.Clear;
                    Date.LineBreakMode = UILineBreakMode.TailTruncation;
                    AddSubview( Date );

                    Chevron = new UILabel( );
                    AddSubview( Chevron );
                    Chevron.Font = Rock.Mobile.PlatformSpecific.iOS.Graphics.FontManager.GetFont( ControlStylingConfig.Icon_Font_Secondary, NoteConfig.Series_Table_IconSize );
                    Chevron.TextColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.TextField_PlaceholderTextColor );
                    Chevron.Text = NoteConfig.Series_Table_Navigate_Icon;
                    Chevron.SizeToFit( );

                    Seperator = new UIView( );
                    AddSubview( Seperator );
                    Seperator.Layer.BorderWidth = 1;
                    Seperator.Layer.BorderColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.BG_Layer_Color ).CGColor;
                }
            }

            NotesMainUIViewController Parent { get; set; }
            List<SeriesEntry> SeriesEntries { get; set; }
            UIImage ImagePlaceholder { get; set; }

            nfloat PendingPrimaryCellHeight { get; set; }
            nfloat PendingCellHeight { get; set; }

            public TableSource (NotesMainUIViewController parent, List<SeriesEntry> series, UIImage imagePlaceholder )
            {
                Parent = parent;
                SeriesEntries = series;
                ImagePlaceholder = imagePlaceholder;
            }

            public override nint RowsInSection (UITableView tableview, nint section)
            {
                return SeriesEntries.Count + 1;
            }

            public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
            {
                tableView.DeselectRow( indexPath, true );

                // notify our parent if it isn't the primary row.
                // The primary row only responds to its two buttons
                if ( indexPath.Row > 0 )
                {
                    Parent.RowClicked( indexPath.Row - 1 );
                }
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return GetCachedRowHeight( tableView, indexPath );
            }

            public override nfloat EstimatedHeight(UITableView tableView, NSIndexPath indexPath)
            {
                return GetCachedRowHeight( tableView, indexPath );
            }

            nfloat GetCachedRowHeight( UITableView tableView, NSIndexPath indexPath )
            {
                // Depending on the row, we either want the primary cell's height,
                // or a standard row's height.
                switch ( indexPath.Row )
                {
                    case 0:
                    {
                        if ( PendingPrimaryCellHeight > 0 )
                        {
                            return PendingPrimaryCellHeight;
                        }
                        break;
                    }

                    default:
                    {

                        if ( PendingCellHeight > 0 )
                        {
                            return PendingCellHeight;
                        }
                        break;
                    }
                }

                // If we don't have the cell's height yet (first render), return the table's height
                return tableView.Frame.Height;
            }

            public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
            {
                if ( indexPath.Row == 0 )
                {
                    return GetPrimaryCell( tableView );
                }
                else
                {
                    return GetStandardCell( tableView, indexPath.Row - 1 );
                }
            }

            UITableViewCell GetPrimaryCell( UITableView tableView )
            {
                SeriesPrimaryCell cell = tableView.DequeueReusableCell( SeriesPrimaryCell.Identifier ) as SeriesPrimaryCell;

                // if there are no cells to reuse, create a new one
                if (cell == null)
                {
                    cell = new SeriesPrimaryCell( UITableViewCellStyle.Default, SeriesCell.Identifier );
                    cell.Parent = this;

                    // take the parent table's width so we inherit its width constraint
                    cell.Bounds = new CGRect( cell.Bounds.X, cell.Bounds.Y, tableView.Bounds.Width, cell.Bounds.Height );

                    // configure the cell colors
                    cell.BackgroundColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.BG_Layer_Color );
                    cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                }

                // Banner Image
                cell.Image.Image = SeriesEntries[ 0 ].mBillboard != null ? SeriesEntries[ 0 ].mBillboard : ImagePlaceholder;
                cell.Image.SizeToFit( );

                // resize the image to fit the width of the device
                nfloat imageAspect = cell.Image.Bounds.Height / cell.Image.Bounds.Width;
                cell.Image.Frame = new CGRect( 0, 
                                               0, 
                                               cell.Bounds.Width, 
                                               cell.Bounds.Width * imageAspect );


                // Create the title
                cell.Title.Text = SeriesEntries[ 0 ].Series.Messages[ 0 ].Name;
                cell.Title.SizeToFit( );
                cell.Title.Frame = new CGRect( 5, cell.Image.Frame.Bottom + 5, cell.Frame.Width - 10, cell.Title.Frame.Height + 5 );


                // Date & Speaker
                cell.Date.Text = SeriesEntries[ 0 ].Series.Messages[ 0 ].Date;
                cell.Date.SizeToFit( );
                cell.Date.Frame = new CGRect( 5, cell.Title.Frame.Bottom - 5, cell.Frame.Width, cell.Date.Frame.Height + 5 );

                cell.Speaker.Text = SeriesEntries[ 0 ].Series.Messages[ 0 ].Speaker;
                cell.Speaker.SizeToFit( );
                cell.Speaker.Frame = new CGRect( cell.Frame.Width - cell.Speaker.Bounds.Width - 5, cell.Title.Frame.Bottom - 5, cell.Frame.Width, cell.Speaker.Frame.Height + 5 );


                // Watch Button & Labels
                cell.WatchButton.Bounds = new CGRect( 0, 0, cell.Frame.Width / 2 + 6, cell.WatchButton.Bounds.Height + 10 );
                cell.WatchButton.Layer.Position = new CGPoint( -5, cell.Speaker.Frame.Bottom + 15 );

                nfloat labelTotalWidth = cell.WatchButtonIcon.Bounds.Width + cell.WatchButtonLabel.Bounds.Width + 5;
                cell.WatchButtonIcon.Layer.Position = new CGPoint( (cell.WatchButton.Bounds.Width - labelTotalWidth) / 2 + (cell.WatchButtonIcon.Bounds.Width / 2), cell.WatchButton.Bounds.Height / 2 );
                cell.WatchButtonLabel.Layer.Position = new CGPoint( cell.WatchButtonIcon.Frame.Right + (cell.WatchButtonLabel.Bounds.Width / 2), cell.WatchButton.Bounds.Height / 2 );

                // disable the button if there's no watch URL
                if ( string.IsNullOrEmpty( SeriesEntries[ 0 ].Series.Messages[ 0 ].WatchUrl ) )
                {
                    cell.ToggleWatchButton( false );
                }
                else
                {
                    cell.ToggleWatchButton( true );
                }


                // Take Notes Button & Labels
                cell.TakeNotesButton.Bounds = new CGRect( 0, 0, cell.Frame.Width / 2 + 5, cell.TakeNotesButton.Bounds.Height + 10 );
                cell.TakeNotesButton.Layer.Position = new CGPoint( (cell.Frame.Width + 5) - cell.TakeNotesButton.Bounds.Width, cell.Speaker.Frame.Bottom + 15 );

                labelTotalWidth = cell.TakeNotesButtonIcon.Bounds.Width + cell.TakeNotesButtonLabel.Bounds.Width + 5;
                cell.TakeNotesButtonIcon.Layer.Position = new CGPoint( (cell.TakeNotesButton.Bounds.Width - labelTotalWidth) / 2 + (cell.TakeNotesButtonIcon.Bounds.Width / 2), cell.TakeNotesButton.Bounds.Height / 2 );
                cell.TakeNotesButtonLabel.Layer.Position = new CGPoint( cell.TakeNotesButtonIcon.Frame.Right + (cell.TakeNotesButtonLabel.Bounds.Width / 2), cell.TakeNotesButton.Bounds.Height / 2 );

                // disable the button if there's no note URL
                if ( string.IsNullOrEmpty( SeriesEntries[ 0 ].Series.Messages[ 0 ].NoteUrl ) )
                {
                    cell.ToggleTakeNotesButton( false );
                }
                else
                {
                    cell.ToggleTakeNotesButton( true );
                }


                // Bottom Banner
                cell.BottomBanner.SizeToFit( );
                cell.BottomBanner.Bounds = new CGRect( 0, 0, cell.Bounds.Width, cell.BottomBanner.Bounds.Height + 10 );
                cell.BottomBanner.Layer.Position = new CGPoint( 0, cell.TakeNotesButton.Frame.Bottom - 1 );

                PendingPrimaryCellHeight = cell.BottomBanner.Frame.Bottom;

                return cell;
            }

            UITableViewCell GetStandardCell( UITableView tableView, int row )
            {
                SeriesCell cell = tableView.DequeueReusableCell( SeriesCell.Identifier ) as SeriesCell;

                // if there are no cells to reuse, create a new one
                if (cell == null)
                {
                    cell = new SeriesCell( UITableViewCellStyle.Default, SeriesCell.Identifier );
                    cell.Parent = this;

                    // take the parent table's width so we inherit its width constraint
                    cell.Bounds = new CGRect( cell.Bounds.X, cell.Bounds.Y, tableView.Bounds.Width, cell.Bounds.Height );

                    // configure the cell colors
                    cell.BackgroundColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.BackgroundColor );
                    cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                }

                // Thumbnail Image
                cell.Image.Image = SeriesEntries[ row ].mThumbnail != null ? SeriesEntries[ row ].mThumbnail : ImagePlaceholder;
                cell.Image.SizeToFit( );

                // force the image to be sized according to the height of the cell
                cell.Image.Frame = new CGRect( 0, 
                                                   0, 
                                                   NoteConfig.Series_Main_CellImageWidth, 
                                                   NoteConfig.Series_Main_CellImageHeight );

                nfloat availableTextWidth = cell.Bounds.Width - cell.Chevron.Bounds.Width - cell.Image.Bounds.Width - 10;

                // Chevron
                cell.Chevron.Layer.Position = new CGPoint( cell.Bounds.Width - (cell.Chevron.Bounds.Width / 2) - 5, (NoteConfig.Series_Main_CellImageHeight) / 2 );

                // Create the title
                cell.Title.Text = SeriesEntries[ row ].Series.Name;
                cell.Title.SizeToFit( );

                // Date Range
                cell.Date.Text = SeriesEntries[ row ].Series.DateRanges;
                cell.Date.SizeToFit( );

                // Position the Title & Date in the center to the right of the image
                nfloat totalTextHeight = cell.Title.Bounds.Height + cell.Date.Bounds.Height - 1;
                cell.Title.Frame = new CGRect( cell.Image.Frame.Right + 10, (NoteConfig.Series_Main_CellImageHeight - totalTextHeight) / 2, availableTextWidth - 5, cell.Title.Frame.Height );
                cell.Date.Frame = new CGRect( cell.Title.Frame.Left, cell.Title.Frame.Bottom - 6, availableTextWidth - 5, cell.Date.Frame.Height + 5 );

                // add the seperator to the bottom
                cell.Seperator.Frame = new CGRect( 0, cell.Image.Frame.Bottom - 1, cell.Bounds.Width, 1 );

                PendingCellHeight = cell.Image.Frame.Bottom;

                return cell;
            }

            public void TakeNotesButtonClicked( )
            {
                Parent.TakeNotesClicked( );
            }

            public void WatchButtonClicked( )
            {
                Parent.WatchButtonClicked( );
            }
        }

        /// <summary>
        /// A wrapper class that consolidates the series and its image
        /// </summary>
        public class SeriesEntry
        {
            public Series Series { get; set; }
            public UIImage mBillboard;
            public UIImage mThumbnail;
        }
        List<SeriesEntry> SeriesEntries { get; set; }
        UIImage ImagePlaceholder{ get; set; }

        UIActivityIndicatorView ActivityIndicator { get; set; }

        NotesDetailsUIViewController DetailsViewController { get; set; }

        bool IsVisible { get; set; }

        public NotesMainUIViewController (IntPtr handle) : base (handle)
        {
            SeriesEntries = new List<SeriesEntry>();

            string imagePath = NSBundle.MainBundle.BundlePath + "/" + "podcastThumbnailPlaceholder.png";
            ImagePlaceholder = new UIImage( imagePath );
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // setup our table
            NotesTableView.BackgroundColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.BackgroundColor );
            NotesTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;

            ActivityIndicator = new UIActivityIndicatorView( new CGRect( View.Frame.Width / 2, View.Frame.Height / 2, 0, 0 ) );
            ActivityIndicator.StartAnimating( );
            ActivityIndicator.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.White;
            ActivityIndicator.SizeToFit( );
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            // adjust the table height for our navbar.
            // We MUST do it here, and we also have to set ContentType to Top, as opposed to ScaleToFill, on the view itself,
            // or our changes will be overwritten
            NotesTableView.Frame = new CGRect( 0, 0, View.Bounds.Width, View.Bounds.Height );
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            IsVisible = false;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            DetailsViewController = null;

            IsVisible = true;

            // what's the state of the series xml?
            if ( RockLaunchData.Instance.RequestingSeries == false )
            {
                // it's in the process of downloading, so wait and poll.
                View.AddSubview( ActivityIndicator );
                View.BringSubviewToFront( ActivityIndicator );
                ActivityIndicator.Hidden = false;

                // kick off a thread that will poll the download status and
                // call "SeriesReady()" when the download is finished.
                Thread waitThread = new Thread( WaitAsync );
                waitThread.Start( );

                while ( waitThread.IsAlive == false );
            }
            else if ( RockLaunchData.Instance.Data.Series.Count == 0 )
            {
                // it hasn't been downloaded, or failed, or something. Point is we
                // don't have anything, so request it.
                View.AddSubview( ActivityIndicator );
                View.BringSubviewToFront( ActivityIndicator );
                ActivityIndicator.Hidden = false;

                RockLaunchData.Instance.GetSeries( delegate
                    {
                        // don't worry about the result. The point is we tried,
                        // and now will either use downloaded data, saved data, or throw an error to the user.
                        SeriesReady( );
                    } );
            }
            else
            {
                // we have the series, so we can move forward.
                SeriesReady( );
            }
        }

        void WaitAsync( )
        {
            // while we're still requesting the series, simply wait
            while ( CCVApp.Shared.Network.RockLaunchData.Instance.RequestingSeries == true );

            // now that tis' finished, update the notes.
            SeriesReady( );
        }

        void SeriesReady( )
        {
            // only do image work on the main thread
            InvokeOnMainThread( delegate
                {
                    ActivityIndicator.Hidden = true;
                    ActivityIndicator.RemoveFromSuperview( );

                    // if there are now series entries, we're good
                    if ( RockLaunchData.Instance.Data.Series.Count > 0 )
                    {
                        // setup each series entry in our table
                        SetupSeriesEntries( RockLaunchData.Instance.Data.Series );

                        // only update the table if we're still visible
                        if ( IsVisible == true )
                        {
                            TableSource source = new TableSource( this, SeriesEntries, ImagePlaceholder );
                            NotesTableView.Source = source;
                            NotesTableView.ReloadData( );
                        }
                    }
                    else if ( IsVisible == true )
                    {
                        SpringboardViewController.DisplayError( MessagesStrings.Error_Title, MessagesStrings.Error_Message );
                    }
                });
        }

        void SetupSeriesEntries( List<Series> seriesList )
        {
            foreach ( Series series in seriesList )
            {
                // add the entry to our list
                SeriesEntry entry = new SeriesEntry();
                SeriesEntries.Add( entry );

                // copy over the series and give it a placeholder image
                entry.Series = series;

                // attempt to load both its images from cache
                bool needDownload = TryLoadCachedImage( entry );
                if ( needDownload )
                {
                    // something failed, so see what needs to be downloaded (could be both)
                    if ( entry.mBillboard == null )
                    {
                        DownloadImageToCache( entry.Series.BillboardUrl, entry.Series.Name + "_bb" );
                    }

                    if ( entry.mThumbnail == null )
                    {
                        DownloadImageToCache( entry.Series.ThumbnailUrl, entry.Series.Name + "_thumb" );
                    }
                }
            }
        }

        bool TryLoadCachedImage( SeriesEntry entry )
        {
            bool needImage = false;

            // check the billboard
            if ( entry.mBillboard == null )
            {
                MemoryStream imageStream = ImageCache.Instance.ReadImage( entry.Series.Name + "_bb" );
                if ( imageStream != null )
                {
                    NSData imageData = NSData.FromStream( imageStream );
                    entry.mBillboard = new UIImage( imageData, UIScreen.MainScreen.Scale );
                    imageStream.Dispose( );
                }
                else
                {
                    needImage = true;
                }
            }

            // check the thumbnail
            if ( entry.mThumbnail == null )
            {
                MemoryStream imageStream = ImageCache.Instance.ReadImage( entry.Series.Name + "_thumb" );
                if ( imageStream != null )
                {
                    NSData imageData = NSData.FromStream( imageStream );
                    entry.mThumbnail = new UIImage( imageData, UIScreen.MainScreen.Scale );
                    imageStream.Dispose( );
                }
                else
                {
                    needImage = true;
                }
            }

            return needImage;
        }

        void DownloadImageToCache( string downloadUrl, string cachedFilename )
        {
            if ( string.IsNullOrEmpty( downloadUrl ) == false )
            {
                // request the image for the series
                HttpRequest webRequest = new HttpRequest();
                RestRequest restRequest = new RestRequest( Method.GET );

                webRequest.ExecuteAsync( downloadUrl, restRequest, 
                    delegate(HttpStatusCode statusCode, string statusDescription, byte[] model )
                    {
                        if ( Rock.Mobile.Network.Util.StatusInSuccessRange( statusCode ) == true )
                        {
                            // write it to cache
                            MemoryStream imageBuffer = new MemoryStream( model );
                            ImageCache.Instance.WriteImage( imageBuffer, cachedFilename );
                            imageBuffer.Dispose( );

                            SeriesImageDownloaded( );
                        }
                    } );
            }
        }

        void SeriesImageDownloaded( )
        {
            if ( IsVisible == true )
            {
                InvokeOnMainThread( delegate
                    {
                        // using only the cache, try to load any image that isn't loaded
                        foreach ( SeriesEntry entry in SeriesEntries )
                        {
                            TryLoadCachedImage( entry );
                        }

                        NotesTableView.ReloadData( );
                    } );
            }
        }

        /// <summary>
        /// Called when the user pressed the 'Watch' button in the primary cell
        /// </summary>
        public void WatchButtonClicked( )
        {
            NotesWatchUIViewController viewController = Storyboard.InstantiateViewController( "NotesWatchUIViewController" ) as NotesWatchUIViewController;
            viewController.WatchUrl = SeriesEntries[ 0 ].Series.Messages[ 0 ].WatchUrl;
            viewController.ShareUrl = SeriesEntries[ 0 ].Series.Messages[ 0 ].ShareUrl;

            Task.PerformSegue( this, viewController );
        }

        /// <summary>
        /// Called when the user pressed the 'Take Notes' button in the primary cell
        /// </summary>
        public void TakeNotesClicked( )
        {
            // maybe technically a hack...we know our parent is a NoteTask,
            // so cast it so we can use the existing NotesViewController.
            NotesTask noteTask = Task as NotesTask;
            if ( noteTask != null )
            {
                noteTask.NoteController.NoteName = string.Format( "Message - {0}", SeriesEntries[ 0 ].Series.Messages[ 0 ].Name );
                noteTask.NoteController.NoteUrl = SeriesEntries[ 0 ].Series.Messages[ 0 ].NoteUrl;

                Task.PerformSegue( this, noteTask.NoteController );
            }
        }

        public void RowClicked( int row )
        {
            DetailsViewController = Storyboard.InstantiateViewController( "NotesDetailsUIViewController" ) as NotesDetailsUIViewController;
            DetailsViewController.Series = SeriesEntries[ row ].Series;
            DetailsViewController.SeriesBillboard = SeriesEntries[ row ].mBillboard != null ? SeriesEntries[ row ].mBillboard : ImagePlaceholder;

            // Note - if they are fast enough, they will end up going to the details of a series before
            // the series banner comes down, resulting in them seeing the generic image placeholder.
            // This isn't really a bug, more just a design issue. Ultimately it'll go away when we
            // start caching images
            //JHM 12-15-14: Don't set billboards, the latest design doesn't call for images on the entries.
            //DetailsViewController.ImagePlaceholder = SeriesEntries[ row ].Billboard;

            Task.PerformSegue( this, DetailsViewController );
        }
    }
}
