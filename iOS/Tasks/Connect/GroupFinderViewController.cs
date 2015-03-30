using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;
using CoreLocation;
using MapKit;
using CCVApp.Shared.Config;
using CoreGraphics;
using System.Collections.Generic;
using CCVApp.Shared.Network;
using Rock.Mobile.Util.Strings;
using System.Collections;
using CCVApp.Shared;
using Rock.Mobile.PlatformSpecific.iOS.UI;
using CCVApp.Shared.Strings;
using CCVApp.Shared.Analytics;
using Rock.Mobile.Animation;

namespace iOS
{
	partial class GroupFinderViewController : TaskUIViewController
	{
        public GroupFinderViewController (IntPtr handle) : base (handle)
        {
        }

        public class TableSource : UITableViewSource 
        {
            /// <summary>
            /// Definition for each cell in this table
            /// </summary>
            class GroupCell : UITableViewCell
            {
                public static string Identifier = "GroupCell";

                public TableSource TableSource { get; set; }

                public UILabel Title { get; set; }
                public UILabel MeetingTime { get; set; }
                public UILabel Distance { get; set; }
                public UIButton JoinButton { get; set; }

                public UIView Seperator { get; set; }

                public int RowIndex { get; set; }

                public GroupCell( UITableViewCellStyle style, string cellIdentifier ) : base( style, cellIdentifier )
                {
                    Title = new UILabel( );
                    Title.Font = Rock.Mobile.PlatformSpecific.iOS.Graphics.FontManager.GetFont( ControlStylingConfig.Medium_Font_Bold, ControlStylingConfig.Medium_FontSize );
                    Title.Layer.AnchorPoint = CGPoint.Empty;
                    Title.TextColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.Label_TextColor );
                    Title.BackgroundColor = UIColor.Clear;
                    Title.LineBreakMode = UILineBreakMode.TailTruncation;
                    AddSubview( Title );

                    MeetingTime = new UILabel( );
                    MeetingTime.Font = Rock.Mobile.PlatformSpecific.iOS.Graphics.FontManager.GetFont( ControlStylingConfig.Small_Font_Light, ControlStylingConfig.Small_FontSize );
                    MeetingTime.Layer.AnchorPoint = CGPoint.Empty;
                    MeetingTime.TextColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.Label_TextColor );
                    MeetingTime.BackgroundColor = UIColor.Clear;
                    AddSubview( MeetingTime );

                    Distance = new UILabel( );
                    Distance.Font = Rock.Mobile.PlatformSpecific.iOS.Graphics.FontManager.GetFont( ControlStylingConfig.Small_Font_Light, ControlStylingConfig.Small_FontSize );
                    Distance.Layer.AnchorPoint = CGPoint.Empty;
                    Distance.TextColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.Label_TextColor );
                    Distance.BackgroundColor = UIColor.Clear;
                    AddSubview( Distance );

                    JoinButton = UIButton.FromType( UIButtonType.Custom );
                    JoinButton.TouchUpInside += (object sender, EventArgs e) => { TableSource.RowButtonClicked( RowIndex ); };
                    JoinButton.Font = Rock.Mobile.PlatformSpecific.iOS.Graphics.FontManager.GetFont( ControlStylingConfig.Icon_Font_Secondary, ConnectConfig.GroupFinder_Join_IconSize );
                    JoinButton.SetTitle( ConnectConfig.GroupFinder_JoinIcon, UIControlState.Normal );
                    JoinButton.SetTitleColor( Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.TextField_PlaceholderTextColor ), UIControlState.Normal );
                    JoinButton.Layer.AnchorPoint = CGPoint.Empty;
                    JoinButton.BackgroundColor = UIColor.Clear;
                    JoinButton.SizeToFit( );
                    AddSubview( JoinButton );

                    Seperator = new UIView( );
                    AddSubview( Seperator );
                    Seperator.Layer.BorderWidth = 1;
                    Seperator.Layer.BorderColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.BG_Layer_Color ).CGColor;
                }
            }

            GroupFinderViewController Parent { get; set; }

            nfloat PendingCellHeight { get; set; }

            public int SelectedIndex { get; set; }

            public TableSource (GroupFinderViewController parent )
            {
                Parent = parent;
                SelectedIndex = -1;
            }

            public override nint RowsInSection (UITableView tableview, nint section)
            {
                return Parent.GroupEntries.Count;
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
                if ( PendingCellHeight > 0 )
                {
                    return PendingCellHeight;
                }

                // If we don't have the cell's height yet (first render), return the table's height
                return tableView.Frame.Height;
            }

            public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
            {
                GroupCell cell = tableView.DequeueReusableCell( GroupCell.Identifier ) as GroupCell;

                // if there are no cells to reuse, create a new one
                if (cell == null)
                {
                    cell = new GroupCell( UITableViewCellStyle.Default, GroupCell.Identifier );
                    cell.TableSource = this;

                    // take the parent table's width so we inherit its width constraint
                    cell.Bounds = new CGRect( cell.Bounds.X, cell.Bounds.Y, tableView.Bounds.Width, cell.Bounds.Height );

                    // remove the selection highlight
                    cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                    cell.BackgroundColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.BackgroundColor );
                }

                // if it's the group nearest the user, color it different. (we always sort by distance)
                if ( indexPath.Row == 0 )
                {
                    cell.BackgroundColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ConnectConfig.GroupFinder_ClosestGroupColor );
                }
                // color the row based on whether it's selected or not
                else if ( SelectedIndex == indexPath.Row )
                {
                    cell.BackgroundColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.BG_Layer_Color );
                }
                else
                {
                    cell.BackgroundColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.BackgroundColor );
                }

                cell.RowIndex = indexPath.Row;

                // Create the title
                cell.Title.Text = Parent.GroupEntries[ indexPath.Row ].Title;
                cell.Title.SizeToFit( );

                // Meeting time - If it isn't set, just blank it out and we wont' show anything for that row.
                if ( string.IsNullOrEmpty( Parent.GroupEntries[ indexPath.Row ].MeetingTime ) == false )
                {
                    cell.MeetingTime.Text = Parent.GroupEntries[ indexPath.Row ].MeetingTime;
                }
                else
                {
                    cell.MeetingTime.Text = ConnectStrings.GroupFinder_ContactForTime;
                }
                cell.MeetingTime.SizeToFit( );

                // Distance
                cell.Distance.Text = string.Format( "{0:##.0} {1}", Parent.GroupEntries[ indexPath.Row ].Distance, ConnectStrings.GroupFinder_MilesSuffix );
                if ( indexPath.Row == 0 )
                {
                    cell.Distance.Text += " " + ConnectStrings.GroupFinder_ClosestTag;
                }
                cell.Distance.SizeToFit( );

                // Position the Title & Address in the center to the right of the image
                cell.Title.Frame = new CGRect( 10, 5, cell.Frame.Width - 5, cell.Title.Frame.Height );
                cell.MeetingTime.Frame = new CGRect( 10, cell.Title.Frame.Bottom + 2, cell.Frame.Width - 5, cell.MeetingTime.Frame.Height + 5 );
                cell.Distance.Frame = new CGRect( 10, cell.MeetingTime.Frame.Bottom - 6, cell.Frame.Width - 5, cell.Distance.Frame.Height + 5 );

                // add the seperator to the bottom
                cell.Seperator.Frame = new CGRect( 0, cell.Distance.Frame.Bottom + 5, cell.Bounds.Width, 1 );

                PendingCellHeight = cell.Seperator.Frame.Bottom;

                cell.JoinButton.Frame = new CGRect( cell.Bounds.Width - cell.JoinButton.Bounds.Width, 
                    (PendingCellHeight - cell.JoinButton.Bounds.Height) / 2, 
                    cell.JoinButton.Bounds.Width, 
                    cell.JoinButton.Bounds.Height );

                return cell;
            }

            public void RowButtonClicked( int row )
            {
                Parent.RowButtonClicked( row );
            }

            public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
            {
                SetSelectedRow( tableView, indexPath.Row );

                // let the parent know it should reveal the nav bar
                Parent.RowClicked( indexPath.Row );
            }

            public void SetSelectedRow( UITableView tableView, int row )
            {
                if ( row != SelectedIndex )
                {
                    tableView.BeginUpdates( );

                    // setup a list with the rows that need to be redrawn
                    List<NSIndexPath> rowIndices = new List<NSIndexPath>();

                    // if there was previously a row selected, add it to our list
                    // so it'll be deselected
                    if ( SelectedIndex > -1 )
                    {
                        rowIndices.Add( NSIndexPath.FromRowSection( SelectedIndex, 0 ) );
                    }


                    // setup the newly selected index
                    SelectedIndex = row;
                    NSIndexPath activeIndex = NSIndexPath.FromRowSection( SelectedIndex, 0 );
                    rowIndices.Add( activeIndex );

                    // force a redraw on the row(s) so their selection state is updated
                    tableView.ReloadRows( rowIndices.ToArray( ), UITableViewRowAnimation.Fade );

                    tableView.EndUpdates( );


                    // make sure the newly selected row comes fully into view
                    tableView.ScrollToRow( activeIndex, UITableViewScrollPosition.Top, true );
                }
            }
        }

        List<GroupFinder.GroupEntry> GroupEntries { get; set; }
        GroupFinder.GroupEntry SourceLocation { get; set; }

        public bool GroupListUpdated { get; set; }

        UITableView GroupFinderTableView { get; set; }
        GroupFinderViewController.TableSource GroupTableSource { get; set; }

        BlockerView BlockerView { get; set; }

        public UITextField Street { get; set; }
        public UITextField City { get; set; }
        public UITextField State { get; set; }
        public UITextField Zip { get; set; }
        public UIView ZipSpacer { get; set; }

        public MKMapView MapView { get; set; }

        public UILabel SearchResultsBanner { get; set; }
        public UILabel DetailsFooter { get; set; }
        public UILabel JoinFooter { get; set; }
        public UIView Seperator { get; set; }

        UIView StreetBorder { get; set; }
        UIView CityBorder { get; set; }
        UIView StateBorder { get; set; }

        public UIButton SearchButton { get; set; }

        bool Searching { get; set; }

        class MapViewDelegate : MKMapViewDelegate
        {
            public GroupFinderViewController Parent { get; set; }

            public override void DidSelectAnnotationView(MKMapView mapView, MKAnnotationView view)
            {
                Parent.AnnotationSelected( view );
            }

            static string AnnotationID = "pinID";
            public override MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
            {
                MKPinAnnotationView pinView = (MKPinAnnotationView) mapView.DequeueReusableAnnotation( AnnotationID );
                if ( pinView == null )
                {
                    pinView = new MKPinAnnotationView( annotation, AnnotationID );
                    pinView.CanShowCallout = true;
                }

                // are we rendering the source location?
                if ( annotation.Coordinate.Latitude == Parent.SourceLocation.Latitude &&
                     annotation.Coordinate.Longitude == Parent.SourceLocation.Longitude )
                {
                    pinView.PinColor = MKPinAnnotationColor.Green;
                }
                else
                {
                    pinView.PinColor = MKPinAnnotationColor.Red;
                }

                return pinView;
            }

            public override void DidAddAnnotationViews(MKMapView mapView, MKAnnotationView[] views)
            {
                Console.WriteLine( "Done" );
            }

            public override void DidFinishRenderingMap(MKMapView mapView, bool fullyRendered)
            {
                Console.WriteLine( "Done" );
            }

            public override void RegionChanged(MKMapView mapView, bool animated)
            {
                Parent.RegionChanged( );
            }
        }

        /// <summary>
        /// Delegate for our address field. When returning, notify the primary cell's parent that this was clicked.
        /// </summary>
        class AddressDelegate : UITextFieldDelegate
        {
            public GroupFinderViewController Parent { get; set; }

            public override bool ShouldReturn(UITextField textField)
            {
                Parent.GetGroups( Parent.Street.Text, Parent.City.Text, Parent.State.Text, Parent.Zip.Text );

                textField.ResignFirstResponder( );
                return true;
            }
        }

        /// <summary>
        /// Simple class to inset the text of our text fields.
        /// </summary>
        public class UIInsetTextField : UITextField
        {
            public override CGRect TextRect(CGRect forBounds)
            {
                return new CGRect( forBounds.X + 5, forBounds.Y, forBounds.Width - 5, forBounds.Height );
            }

            public override CGRect EditingRect(CGRect forBounds)
            {
                return new CGRect( forBounds.X + 5, forBounds.Y, forBounds.Width - 5, forBounds.Height );
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = Rock.Mobile.PlatformUI.Util.GetUIColor( CCVApp.Shared.Config.ControlStylingConfig.BG_Layer_Color );

            // setup everything except positioning, which will happen in LayoutChanged()
            SourceLocation = null;
            GroupEntries = new List<GroupFinder.GroupEntry>();

            SearchButton = UIButton.FromType( UIButtonType.System );
            View.AddSubview( SearchButton );
            SearchButton.Layer.AnchorPoint = new CGPoint( 0, 0 );
            ControlStyling.StyleButton( SearchButton, ConnectConfig.GroupFinder_SearchIcon, ControlStylingConfig.Icon_Font_Secondary, 32 );
            SearchButton.SizeToFit( );
            SearchButton.TouchUpInside += (object sender, EventArgs e) => 
                {
                    GetGroups( Street.Text, City.Text, State.Text, Zip.Text );
                };

            Street = new UIInsetTextField( );
            View.AddSubview( Street );
            Street.Layer.AnchorPoint = CGPoint.Empty;
            ControlStyling.StyleTextField( Street, ConnectStrings.GroupFinder_StreetPlaceholder, ControlStylingConfig.Small_Font_Regular, ControlStylingConfig.Small_FontSize );
            Street.ReturnKeyType = UIReturnKeyType.Search;
            Street.KeyboardAppearance = UIKeyboardAppearance.Dark;
            Street.AutocapitalizationType = UITextAutocapitalizationType.Words;
            Street.Delegate = new AddressDelegate( ) { Parent = this };
            Street.AutocorrectionType = UITextAutocorrectionType.No;

            StreetBorder = new UIView( );
            View.AddSubview( StreetBorder );
            StreetBorder.BackgroundColor = UIColor.DarkGray;
            StreetBorder.Layer.AnchorPoint = CGPoint.Empty;

            City = new UIInsetTextField( );
            View.AddSubview( City );
            City.Layer.AnchorPoint = CGPoint.Empty;
            ControlStyling.StyleTextField( City, ConnectStrings.GroupFinder_CityPlaceholder, ControlStylingConfig.Small_Font_Regular, ControlStylingConfig.Small_FontSize );
            City.ReturnKeyType = UIReturnKeyType.Search;
            City.KeyboardAppearance = UIKeyboardAppearance.Dark;
            City.AutocapitalizationType = UITextAutocapitalizationType.Words;
            City.AutocorrectionType = UITextAutocorrectionType.No;
            City.Delegate = new AddressDelegate( ) { Parent = this };


            CityBorder = new UIView( );
            View.AddSubview( CityBorder );
            CityBorder.BackgroundColor = UIColor.DarkGray;
            CityBorder.Layer.AnchorPoint = CGPoint.Empty;

            State = new UIInsetTextField( );
            View.AddSubview( State );
            State.Layer.AnchorPoint = CGPoint.Empty;
            ControlStyling.StyleTextField( State, ConnectStrings.GroupFinder_StatePlaceholder, ControlStylingConfig.Small_Font_Regular, ControlStylingConfig.Small_FontSize );
            State.Text = ConnectStrings.GroupFinder_DefaultState;
            State.ReturnKeyType = UIReturnKeyType.Search;
            State.Delegate = new AddressDelegate( ) { Parent = this };
            State.KeyboardAppearance = UIKeyboardAppearance.Dark;
            State.AutocapitalizationType = UITextAutocapitalizationType.Words;
            State.AutocorrectionType = UITextAutocorrectionType.No;


            StateBorder = new UIView( );
            StateBorder.BackgroundColor = UIColor.DarkGray;
            StateBorder.Layer.AnchorPoint = CGPoint.Empty;
            View.AddSubview( StateBorder );

            Zip = new UIInsetTextField( );
            View.AddSubview( Zip );
            Zip.Layer.AnchorPoint = CGPoint.Empty;
            ControlStyling.StyleTextField( Zip, ConnectStrings.GroupFinder_ZipPlaceholder, ControlStylingConfig.Small_Font_Regular, ControlStylingConfig.Small_FontSize );
            Zip.KeyboardAppearance = UIKeyboardAppearance.Dark;
            Zip.ReturnKeyType = UIReturnKeyType.Search;
            Zip.AutocorrectionType = UITextAutocorrectionType.No;
            Zip.Delegate = new AddressDelegate( ) { Parent = this };

            ZipSpacer = new UIView( );
            View.AddSubview( ZipSpacer );
            ZipSpacer.BackgroundColor = UIColor.Clear;
            ZipSpacer.Layer.AnchorPoint = CGPoint.Empty;

            MapView = new MKMapView( );
            View.AddSubview( MapView );

            // set the default position for the map to whatever specified area.
            MKCoordinateRegion region = MKCoordinateRegion.FromDistance( new CLLocationCoordinate2D( 
                ConnectConfig.GroupFinder_DefaultLatitude, 
                ConnectConfig.GroupFinder_DefaultLongitude ), 
                ConnectConfig.GroupFinder_DefaultScale_iOS, 
                ConnectConfig.GroupFinder_DefaultScale_iOS );
            MapView.SetRegion( region, true );

            MapView.Layer.AnchorPoint = new CGPoint( 0, 0 );
            MapView.Delegate = new MapViewDelegate() { Parent = this };


            SearchResultsBanner = new UILabel( );
            View.AddSubview( SearchResultsBanner );
            SearchResultsBanner.Layer.AnchorPoint = new CGPoint( 0, 0 );
            SearchResultsBanner.Font = Rock.Mobile.PlatformSpecific.iOS.Graphics.FontManager.GetFont( ControlStylingConfig.Small_Font_Regular, ControlStylingConfig.Small_FontSize );
            SearchResultsBanner.Text = ConnectStrings.GroupFinder_BeforeSearch;
            SearchResultsBanner.SizeToFit( );
            SearchResultsBanner.TextColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.TextField_PlaceholderTextColor );
            SearchResultsBanner.BackgroundColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.BG_Layer_Color );
            SearchResultsBanner.TextAlignment = UITextAlignment.Center;


            DetailsFooter = new UILabel( );
            View.AddSubview( DetailsFooter );
            DetailsFooter.Layer.AnchorPoint = new CGPoint( 0, 0 );
            DetailsFooter.Font = Rock.Mobile.PlatformSpecific.iOS.Graphics.FontManager.GetFont( ControlStylingConfig.Small_Font_Regular, ControlStylingConfig.Small_FontSize );
            DetailsFooter.Text = ConnectStrings.GroupFinder_DetailsLabel;
            DetailsFooter.SizeToFit( );
            DetailsFooter.TextColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.TextField_PlaceholderTextColor );
            DetailsFooter.BackgroundColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.BG_Layer_Color );
            DetailsFooter.TextAlignment = UITextAlignment.Left;


            JoinFooter = new UILabel( );
            View.AddSubview( JoinFooter );
            JoinFooter.Layer.AnchorPoint = new CGPoint( 0, 0 );
            JoinFooter.Font = Rock.Mobile.PlatformSpecific.iOS.Graphics.FontManager.GetFont( ControlStylingConfig.Small_Font_Regular, ControlStylingConfig.Small_FontSize );
            JoinFooter.Text = ConnectStrings.GroupFinder_JoinLabel;
            JoinFooter.SizeToFit( );
            JoinFooter.TextColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.TextField_PlaceholderTextColor );
            JoinFooter.BackgroundColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.BG_Layer_Color );
            JoinFooter.TextAlignment = UITextAlignment.Left;


            Seperator = new UIView( );
            View.AddSubview( Seperator );
            Seperator.Layer.BorderWidth = 1;
            Seperator.Layer.BorderColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.TextField_PlaceholderTextColor ).CGColor;


            GroupFinderTableView = new UITableView();
            View.AddSubview( GroupFinderTableView );
            GroupTableSource = new GroupFinderViewController.TableSource( this );

            // add the table view and source
            GroupFinderTableView.BackgroundColor = Rock.Mobile.PlatformUI.Util.GetUIColor( CCVApp.Shared.Config.ControlStylingConfig.Table_Footer_Color );
            GroupFinderTableView.Source = GroupTableSource;
            GroupFinderTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;

            BlockerView = new BlockerView( View.Frame );
            View.AddSubview( BlockerView );
        }

        public override void LayoutChanged( )
        {
            base.LayoutChanged( );

            // setup the top area with the map
            // define the search button
            SearchButton.Frame = new CGRect( View.Frame.Width - SearchButton.Frame.Width, 0, SearchButton.Frame.Width, 33 );

            // add all four address fields with a border between each
            nfloat widthPerField = (View.Frame.Width - SearchButton.Frame.Width - 20) / 4;

            // Street
            Street.Frame = new CGRect( 0, 0, widthPerField * 1.5f, SearchButton.Frame.Height );

            StreetBorder.Frame = new CGRect( 0, 0, 1, SearchButton.Frame.Height );
            StreetBorder.Layer.Position = new CGPoint( Street.Frame.Right, 0 );


            // City
            City.Frame = new CGRect( StreetBorder.Frame.Right, 0, widthPerField, SearchButton.Frame.Height );

            CityBorder.Frame = new CGRect( 0, 0, 1, SearchButton.Frame.Height );
            CityBorder.Layer.Position = new CGPoint( City.Frame.Right, 0 );


            // State
            State.Frame = new CGRect( CityBorder.Frame.Right, 0, widthPerField / 2, SearchButton.Frame.Height );


            StateBorder.Frame = new CGRect( 0, 0, 1, SearchButton.Frame.Height );
            StateBorder.Layer.Position = new CGPoint( State.Frame.Right, 0 );


            // Zip
            Zip.Frame = new CGRect( StateBorder.Frame.Right, 0, widthPerField, SearchButton.Frame.Height );

            ZipSpacer.Frame = new CGRect( 0, 0, SearchButton.Frame.Left - Zip.Frame.Right, SearchButton.Frame.Height );
            ZipSpacer.Layer.Position = new CGPoint( Zip.Frame.Right, 0 );


            // Map
            MapView.Frame = new CGRect( 0, Street.Frame.Bottom, View.Frame.Width, View.Frame.Height * .40f );

            // Search Results Banner
            SearchResultsBanner.Frame = new CGRect( 0, MapView.Frame.Bottom, View.Frame.Width, SearchResultsBanner.Frame.Height );


            // Details / Join Banners
            DetailsFooter.Frame = new CGRect( 10, SearchResultsBanner.Frame.Bottom, DetailsFooter.Frame.Width, SearchResultsBanner.Frame.Height );
            JoinFooter.Frame = new CGRect( View.Frame.Width - JoinFooter.Frame.Width - 10, SearchResultsBanner.Frame.Bottom, JoinFooter.Frame.Width, SearchResultsBanner.Frame.Height );


            // add the seperator to the bottom
            Seperator.Frame = new CGRect( 0, DetailsFooter.Frame.Bottom - 1, View.Bounds.Width, 1 );

            // wait to layout the table view until all subviews have been laid out. Fixes an issue where the table gets more height than it should,
            // and the last row doesn't fit on screen.
            GroupFinderTableView.Frame = new CGRect( 0, Seperator.Frame.Bottom, View.Bounds.Width, View.Bounds.Height - Seperator.Frame.Bottom );
            GroupFinderTableView.ReloadData( );
        }

        public void UpdateMap( )
        {
            GroupTableSource.SelectedIndex = -1;

            // remove existing annotations
            MapView.RemoveAnnotations( MapView.Annotations );

            // set the search results banner appropriately
            if ( GroupEntries.Count > 0 )
            {
                SearchResultsBanner.Text = string.Format( ConnectStrings.GroupFinder_Neighborhood, GroupEntries[ 0 ].NeighborhoodArea );

                // add an annotation for each position found in the group
                List<IMKAnnotation> annotations = new List<IMKAnnotation>();

                // add an annotation for the source
                MKPointAnnotation sourceAnnotation = new MKPointAnnotation();
                sourceAnnotation.SetCoordinate( new CLLocationCoordinate2D( SourceLocation.Latitude, SourceLocation.Longitude ) );
                sourceAnnotation.Title = "";
                sourceAnnotation.Subtitle = "";
                annotations.Add( sourceAnnotation );

                foreach ( GroupFinder.GroupEntry entry in GroupEntries )
                {
                    MKPointAnnotation annotation = new MKPointAnnotation();
                    annotation.SetCoordinate( new CLLocationCoordinate2D( entry.Latitude, entry.Longitude ) );
                    annotation.Title = entry.Title;
                    annotation.Subtitle = string.Format( "{0:##.0} {1}", entry.Distance, ConnectStrings.GroupFinder_MilesSuffix );
                    annotations.Add( annotation );
                }
                MapView.ShowAnnotations( annotations.ToArray( ), true );
            }
            else
            {
                SearchResultsBanner.Text = ConnectStrings.GroupFinder_NoGroupsFound;

                // since there were no groups, revert the map to whatever specified area
                MKCoordinateRegion region = MKCoordinateRegion.FromDistance( new CLLocationCoordinate2D( 
                    ConnectConfig.GroupFinder_DefaultLatitude, 
                    ConnectConfig.GroupFinder_DefaultLongitude ), 
                    ConnectConfig.GroupFinder_DefaultScale_iOS, 
                    ConnectConfig.GroupFinder_DefaultScale_iOS );
                MapView.SetRegion( region, true );
            }
        }

        public void UpdateAddress( string street, string city, string state, string zip )
        {
            Street.Text = street;
            City.Text = city;
            State.Text = state;
            Zip.Text = zip;
        }

        public void RowButtonClicked( int row )
        {
            // create the view controller
            GroupFinderJoinViewController joinController = new GroupFinderJoinViewController();

            // set the group info
            GroupFinder.GroupEntry currGroup = GroupEntries[ row ];
            joinController.GroupTitle = currGroup.Title;
            joinController.MeetingTime = string.IsNullOrEmpty( currGroup.MeetingTime ) == false ? currGroup.MeetingTime : ConnectStrings.GroupFinder_ContactForTime;
            joinController.GroupID = currGroup.Id;

            joinController.Distance = string.Format( "{0:##.0} {1}", currGroup.Distance, ConnectStrings.GroupFinder_MilesSuffix );
            if ( row == 0 )
            {
                joinController.Distance += " " + ConnectStrings.GroupFinder_ClosestTag;
            }

            // launch the view
            Task.PerformSegue( this, joinController );
        }

        public void RowClicked( int row )
        {
            // if they selected a group in the list, center it on the map.
            if ( MapView.Annotations.Length > 0 )
            {
                // use the row index to get the matching annotation in the map
                IMKAnnotation marker = TableRowToMapAnnotation( row );

                // select it and set it as the center coordinate
                MapView.SelectedAnnotations = new IMKAnnotation[1] { marker };
                MapView.SetCenterCoordinate( marker.Coordinate, true );
            }
        }

        IMKAnnotation TableRowToMapAnnotation( int row )
        {
            GroupFinder.GroupEntry selectedGroup = GroupEntries[ row ];
            CLLocationCoordinate2D selectedCoord = new CLLocationCoordinate2D( selectedGroup.Latitude, selectedGroup.Longitude );

            // given the row index of a group entry, return its associated annotation
            // select the matching marker
            foreach ( IMKAnnotation marker in MapView.Annotations )
            {
                if ( marker.Coordinate.Latitude == selectedCoord.Latitude || 
                     marker.Coordinate.Longitude == selectedCoord.Longitude )
                {
                    return marker;
                }
            }

            return null;
        }

        int MapAnnotationToTableRow( IMKAnnotation marker )
        {
            // given a map annotation, we'll go thru each group entry and compare the coordinates
            // to find the matching location.

            for ( int i = 0; i < GroupEntries.Count; i++ )
            {
                // find the row index by matching coordinates
                GroupFinder.GroupEntry currGroup = GroupEntries[ i ];
                CLLocationCoordinate2D currCoord = new CLLocationCoordinate2D( currGroup.Latitude, currGroup.Longitude );

                if ( marker.Coordinate.Latitude == currCoord.Latitude &&
                     marker.Coordinate.Longitude == currCoord.Longitude )
                {
                    return i;
                }
            }

            return -1;
        }

        public void AnnotationSelected( MKAnnotationView annotationView )
        {
            // first select (center) the annotation)
            MapView.SetCenterCoordinate( annotationView.Annotation.Coordinate, true );

            // now determine where it is in the group list
            int rowIndex = MapAnnotationToTableRow( annotationView.Annotation );

            // and select that row
            GroupTableSource.SetSelectedRow( GroupFinderTableView, rowIndex );
        }

        public void RegionChanged( )
        {
            // called when we're done focusing on a new area
            if ( GroupListUpdated == true )
            {
                GroupListUpdated = false;
                RowClicked( 0 );
            }
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            //base.TouchesEnded(touches, evt);
        } 

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            // see if there's an address for this person that we can automatically use.
            if ( RockMobileUser.Instance.HasFullAddress( ) == true )
            {
                UpdateAddress( RockMobileUser.Instance.Street1( ), RockMobileUser.Instance.City( ), RockMobileUser.Instance.State( ), RockMobileUser.Instance.Zip( ) );
                GetGroups( RockMobileUser.Instance.Street1( ), RockMobileUser.Instance.City( ), RockMobileUser.Instance.State( ), RockMobileUser.Instance.Zip( ) );
            }
        }

        void GetGroups( string street, string city, string state, string zip )
        {
            Street.ResignFirstResponder( );
            City.ResignFirstResponder( );
            State.ResignFirstResponder( );
            Zip.ResignFirstResponder( );

            // fun bonus!
            if ( street == CCVApp.Shared.ConnectLink.CheatException.CheatString )
            {
                throw new CCVApp.Shared.ConnectLink.CheatException( );
            }
            else
            {
                if ( string.IsNullOrEmpty( street ) == false &&
                     string.IsNullOrEmpty( city ) == false &&
                     string.IsNullOrEmpty( state ) == false &&
                     string.IsNullOrEmpty( zip ) == false )
                {
                    if ( Searching == false )
                    {
                        Searching = true;

                        BlockerView.FadeIn( delegate
                            {
                                GroupFinder.GetGroups( street, city, state, zip, 
                                    delegate( GroupFinder.GroupEntry sourceLocation, List<GroupFinder.GroupEntry> groupEntries )
                                    {
                                        BlockerView.FadeOut( delegate
                                            {
                                                Searching = false;

                                                groupEntries.Sort( delegate(GroupFinder.GroupEntry x, GroupFinder.GroupEntry y )
                                                    {
                                                        return x.Distance < y.Distance ? -1 : 1;
                                                    } );

                                                SourceLocation = sourceLocation;
                                                GroupEntries = groupEntries;
                                                UpdateMap( );
                                                GroupFinderTableView.ReloadData( );

                                                // flag that our group list was updated so that
                                                // on the region updated callback from the map, we 
                                                // can select the appropriate group
                                                GroupListUpdated = true;

                                                // and record an analytic for the neighborhood that this location was apart of. This helps us know
                                                // which neighborhoods get the most hits.
                                                string address = street + " " + city + ", " + state + ", " + zip;

                                                if ( groupEntries.Count > 0 )
                                                {
                                                    // record an analytic that they searched
                                                    GroupFinderAnalytic.Instance.Trigger( GroupFinderAnalytic.Location, address );

                                                    GroupFinderAnalytic.Instance.Trigger( GroupFinderAnalytic.Neighborhood, groupEntries[ 0 ].NeighborhoodArea );
                                                }
                                                else
                                                {
                                                    // record that this address failed
                                                    GroupFinderAnalytic.Instance.Trigger( GroupFinderAnalytic.OutOfBounds, address );
                                                }
                                            } );
                                    } );
                            } );
                    }
                }

                ValidateTextFields( );
            }
        }

        void ValidateTextFields( )
        {
            // this will color the invalid fields red so the user knows they need to fill them in.

            // Validate Street
            uint targetStreetColor = string.IsNullOrEmpty( Street.Text ) == true ? ControlStylingConfig.BadInput_BG_Layer_Color : ControlStylingConfig.BG_Layer_Color; 
            Rock.Mobile.PlatformSpecific.iOS.UI.Util.AnimateViewColor( targetStreetColor, Street );


            // Validate City
            uint targetCityColor = string.IsNullOrEmpty( City.Text ) == true ? ControlStylingConfig.BadInput_BG_Layer_Color : ControlStylingConfig.BG_Layer_Color; 
            Rock.Mobile.PlatformSpecific.iOS.UI.Util.AnimateViewColor( targetCityColor, City );

            // Validate State
            uint targetStateColor = string.IsNullOrEmpty( State.Text ) == true ? ControlStylingConfig.BadInput_BG_Layer_Color : ControlStylingConfig.BG_Layer_Color; 
            Rock.Mobile.PlatformSpecific.iOS.UI.Util.AnimateViewColor( targetStateColor, State );


            // Validate Zip
            uint targetZipColor = string.IsNullOrEmpty( Zip.Text ) == true ? ControlStylingConfig.BadInput_BG_Layer_Color : ControlStylingConfig.BG_Layer_Color; 
            Rock.Mobile.PlatformSpecific.iOS.UI.Util.AnimateViewColor( targetZipColor, Zip );
            Rock.Mobile.PlatformSpecific.iOS.UI.Util.AnimateViewColor( targetZipColor, ZipSpacer );
        }
	}
}
