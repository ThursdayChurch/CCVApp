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

namespace Droid
{
    namespace Tasks
    {
        namespace News
        {
            public class NewsPrimaryFragment : TaskFragment
            {
                public NewsPrimaryFragment( ) : base( )
                {
                }

                public NewsPrimaryFragment( Task parentTask ) : base( parentTask )
                {
                }

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

                    View view = inflater.Inflate(Resource.Layout.PrimaryNews, container, false);
                    view.SetOnTouchListener( this );

                    Button detailsButton = view.FindViewById<Button>(Resource.Id.detailsButton);

                    detailsButton.Click += (object sender, EventArgs e) => 
                        {
                            // move to the next page..somehow.
                            ParentTask.OnClick( this, detailsButton.Id );
                        };

                    ParentTask.NavbarFragment.NavToolbar.SetBackButtonEnabled( false );
                    ParentTask.NavbarFragment.NavToolbar.Reveal( false );

                    return view;
                }

                protected override void TouchUpInside(View v)
                {
                    // reveal the nav bar temporarily
                    ParentTask.NavbarFragment.NavToolbar.RevealForTime( 3.00f );
                }
            }
        }
    }
}

