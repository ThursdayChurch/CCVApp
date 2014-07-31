﻿#if __ANDROID__
using System;
using System.Drawing;
using Android.Widget;
using Android.Graphics;
using Android.Views;
using Android.Views.InputMethods;
using Android.App;


namespace Notes
{
    namespace PlatformUI
    {
        public class DroidTextField : PlatformTextField
        {
            EditText TextField { get; set; }

            View DummyView { get; set; }

            public DroidTextField( )
            {
                TextField = new EditText( DroidCommon.Context );
                TextField.LayoutParameters = new ViewGroup.LayoutParams( ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.WrapContent );

                // create a dummy view that can take focus to de-select the text field
                DummyView = new View( DroidCommon.Context );
                DummyView.Focusable = true;
                DummyView.FocusableInTouchMode = true;

                // let the dummy request focus so that the edit field doesn't get it and bring up the keyboard.
                DummyView.RequestFocus();
            }

            // Properties
            public override void SetFont( string fontName, float fontSize )
            {
                try
                {
                    Typeface fontFace = Typeface.CreateFromAsset( DroidCommon.Context.Assets, "Fonts/" + fontName + ".ttf" );
                    TextField.SetTypeface( fontFace, TypefaceStyle.Normal );
                    TextField.SetTextSize( Android.Util.ComplexUnitType.Pt, fontSize );
                } 
                catch
                {
                    throw new ArgumentException( string.Format( "Unable to load font: {0}", fontName ) );
                }
            }

            protected override void setBackgroundColor( uint backgroundColor )
            {
                TextField.SetBackgroundColor( GetUIColor( backgroundColor ) );
            }

            protected override float getOpacity( )
            {
                return TextField.Alpha;
            }

            protected override void setOpacity( float opacity )
            {
                TextField.Alpha = opacity;
            }

            protected override float getZPosition( )
            {
                // TODO: There's no current way I can find in Android to do this. But,
                // we only use it for debugging.
                return 0.0f;
            }

            protected override void setZPosition( float zPosition )
            {
                // TODO: There's no current way I can find in Android to do this.
            }

            protected override RectangleF getBounds( )
            {
                //Bounds is simply the localSpace coordinates of the edges.
                // NOTE: On android we're not supporting a non-0 left/top. I don't know why you'd EVER
                // want this, but it's possible to set on iOS.
                return new RectangleF( 0, 0, TextField.LayoutParameters.Width, TextField.LayoutParameters.Height );
            }

            protected override void setBounds( RectangleF bounds )
            {
                //Bounds is simply the localSpace coordinates of the edges.
                // NOTE: On android we're not supporting a non-0 left/top. I don't know why you'd EVER
                // want this, but it's possible to set on iOS.
                TextField.LayoutParameters.Width = ( int )bounds.Width;
                TextField.SetMaxWidth( TextField.LayoutParameters.Width );
                TextField.LayoutParameters.Height = ( int )bounds.Height;
            }

            protected override RectangleF getFrame( )
            {
                //Frame is the transformed bounds to include position, so the Right/Bottom will be absolute.
                RectangleF frame = new RectangleF( TextField.GetX( ), TextField.GetY( ), TextField.LayoutParameters.Width, TextField.LayoutParameters.Height );
                return frame;
            }

            protected override void setFrame( RectangleF frame )
            {
                //Frame is the transformed bounds to include position, so the Right/Bottom will be absolute.
                setPosition( new System.Drawing.PointF( frame.X, frame.Y ) );

                RectangleF bounds = new RectangleF( frame.Left, frame.Top, frame.Width, frame.Height );
                setBounds( bounds );
            }

            protected override System.Drawing.PointF getPosition( )
            {
                return new System.Drawing.PointF( TextField.GetX( ), TextField.GetY( ) );
            }

            protected override void setPosition( System.Drawing.PointF position )
            {
                TextField.SetX( position.X );
                TextField.SetY( position.Y );
            }

            protected override void setTextColor( uint color )
            {
                TextField.SetTextColor( GetUIColor( color ) );
            }

            protected override string getText( )
            {
                return TextField.Text;
            }

            protected override void setText( string text )
            {
                TextField.Text = text;
            }

            protected override void setPlaceholderTextColor( uint color )
            {
                TextField.SetHintTextColor( GetUIColor( color ) );
            }

            protected override string getPlaceholder( )
            {
                return TextField.Hint;
            }

            protected override void setPlaceholder( string placeholder )
            {
                TextField.Hint = placeholder;
            }

            protected override TextAlignment getTextAlignment( )
            {
                // gonna have to do a stupid transform
                switch( TextField.Gravity )
                {
                    case GravityFlags.Center:
                        return TextAlignment.Center;
                    case GravityFlags.Left:
                        return TextAlignment.Left;
                    case GravityFlags.Right:
                        return TextAlignment.Right;
                    default:
                        return TextAlignment.Left;
                }
            }

            protected override void setTextAlignment( TextAlignment alignment )
            {
                // gonna have to do a stupid transform
                switch( alignment )
                {
                    case TextAlignment.Center:
                        TextField.Gravity = GravityFlags.Center;
                        break;
                    case TextAlignment.Left:
                        TextField.Gravity = GravityFlags.Left;
                        break;
                    case TextAlignment.Right:
                        TextField.Gravity = GravityFlags.Right;
                        break;
                    default:
                        TextField.Gravity = GravityFlags.Left;
                        break;
                }
            }

            public override void AddAsSubview( object masterView )
            {
                // we know that masterView will be an iOS View.
                RelativeLayout view = masterView as RelativeLayout;
                if( view == null )
                {
                    throw new InvalidCastException( "Object passed to Android AddAsSubview must be a RelativeLayout." );
                }

                view.AddView( TextField );
                view.AddView( DummyView );
            }

            public override void RemoveAsSubview( object masterView )
            {
                // we know that masterView will be an iOS View.
                RelativeLayout view = masterView as RelativeLayout;
                if( view == null )
                {
                    throw new InvalidCastException( "Object passed to Android RemoveAsSubview must be a RelativeLayout." );
                }

                view.RemoveView( TextField );
                view.RemoveView( DummyView );
            }

            public override void SizeToFit( )
            {
                // create the specs we want for measurement
                int widthMeasureSpec = View.MeasureSpec.MakeMeasureSpec( TextField.LayoutParameters.Width, MeasureSpecMode.Unspecified );
                int heightMeasureSpec = View.MeasureSpec.MakeMeasureSpec( 0, MeasureSpecMode.Unspecified );

                // measure the label given the current width/height/text
                TextField.Measure( widthMeasureSpec, heightMeasureSpec );

                // update its width
                TextField.LayoutParameters.Width = TextField.MeasuredWidth;
                TextField.SetMaxWidth( TextField.LayoutParameters.Width );

                // set the height which will include the wrapped lines
                TextField.LayoutParameters.Height = TextField.MeasuredHeight;
            }

            public override void ResignFirstResponder( )
            {
                Activity activity = ( Activity )DroidCommon.Context;
                if( activity.CurrentFocus != null && ( activity.CurrentFocus as EditText ) != null )
                {
                    InputMethodManager imm = ( InputMethodManager )DroidCommon.Context.GetSystemService( Android.Content.Context.InputMethodService );

                    imm.HideSoftInputFromWindow( TextField.WindowToken, 0 );

                    // yeild focus to the dummy view so the text field clears it's caret and the selected outline
                    TextField.ClearFocus( );
                    DummyView.RequestFocus( );
                }
            }
        }
    }
}
#endif
