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

namespace Droid
{
    namespace Tasks
    {
        namespace News
        {
            /// <summary>
            /// Subclass ImageView so we can override OnMeasure and scale up the image 
            /// maintaining aspect ratio
            /// </summary>
            class ScaledImageView : ImageView
            {
                public ScaledImageView( Context context ) : base( context )
                {
                }

                protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
                {
                    if ( Drawable != null )
                    {
                        int width = MeasureSpec.GetSize( widthMeasureSpec );
                        int height = (int)Math.Ceiling( width * ( (float)Drawable.IntrinsicHeight / (float)Drawable.IntrinsicWidth ) );

                        SetMeasuredDimension( width, height );
                    }
                    else
                    {
                        base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
                    }
                }
            }

            public class NewsArrayAdapter : BaseAdapter
            {
                List<Bitmap> NewsImage { get; set; }

                NewsPrimaryFragment ParentFragment { get; set; }

                public NewsArrayAdapter( NewsPrimaryFragment parentFragment, List<Bitmap> newsImage )
                {
                    ParentFragment = parentFragment;

                    NewsImage = newsImage;
                }

                public override int Count 
                {
                    get { return NewsImage.Count; }
                }

                public override Java.Lang.Object GetItem (int position) 
                {
                    // could wrap a Contact in a Java.Lang.Object
                    // to return it here if needed
                    return null;
                }

                public override long GetItemId (int position) 
                {
                    return 0;
                }

                public override View GetView(int position, View convertView, ViewGroup parent)
                {
                    ScaledImageView view = (ScaledImageView) convertView ?? new ScaledImageView( ParentFragment.Activity.BaseContext );
                    view.LayoutParameters = new AbsListView.LayoutParams( ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent );

                    view.SetImageBitmap( NewsImage[ position ] );
                    view.SetScaleType( ImageView.ScaleType.CenterCrop );

                    return view;
                }
            }

            public class NewsPrimaryFragment : TaskFragment
            {
                public List<RockNews> News { get; set; }
                List<Bitmap> NewsImage { get; set; }

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

					View view = inflater.Inflate(Resource.Layout.News_Primary, container, false);
                    view.SetOnTouchListener( this );

                    NewsImage = new List<Bitmap>( );
                    foreach( RockNews item in News )
                    {
                        // load the stream from assets
                        System.IO.Stream assetStream = Activity.BaseContext.Assets.Open( item.ImageName );
                        NewsImage.Add( BitmapFactory.DecodeStream( assetStream ) );
                    }

                    ListView listView = view.FindViewById<ListView>( Resource.Id.news_primary_list );
                    listView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => 
                        {
                            ParentTask.OnClick( this, e.Position );
                        };
                    listView.SetOnTouchListener( this );
                    listView.Adapter = new NewsArrayAdapter( this, NewsImage );

                    return view;
                }

                public override void OnResume()
                {
                    base.OnResume();

                    ParentTask.NavbarFragment.NavToolbar.SetBackButtonEnabled( false );
                    ParentTask.NavbarFragment.NavToolbar.Reveal( false );

                    ParentTask.NavbarFragment.NavToolbar.SetShareButtonEnabled( false );
                    ParentTask.NavbarFragment.NavToolbar.DisplayShareButton( false, null );
                }
            }
        }
    }
}

