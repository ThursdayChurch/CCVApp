using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;
using Rock.Mobile.Network;
using CCVApp.Shared.Network;
using CoreAnimation;
using CoreGraphics;
using CCVApp.Shared.Config;
using CCVApp.Shared.Strings;
using Rock.Mobile.PlatformUI;
using System.Collections.Generic;
using Rock.Mobile.Util.Strings;
using Rock.Mobile.PlatformSpecific.iOS.UI;
using Rock.Mobile.PlatformSpecific.Util;
using Rock.Mobile.Animation;
using CCVApp.Shared.UI;

namespace iOS
{
    partial class RegisterViewController : UIViewController
    {
        /// <summary>
        /// Reference to the parent springboard for returning apon completion
        /// </summary>
        /// <value>The springboard.</value>
        public SpringboardViewController Springboard { get; set; }

        /// <summary>
        /// View for displaying the logo in the header
        /// </summary>
        /// <value>The logo view.</value>
        UIImageView LogoView { get; set; }

        enum RegisterState
        {
            None,
            Trying,
            Success,
            Fail
        }

        RegisterState State { get; set; }

        BlockerView BlockerView { get; set; }

        UIResultView ResultView { get; set; }

        StyledTextField UserNameText { get; set; }
        StyledTextField PasswordText { get; set; }
        StyledTextField ConfirmPasswordText { get; set; }

        StyledTextField NickNameText { get; set; }
        StyledTextField LastNameText { get; set; }

        StyledTextField EmailText { get; set; }
        StyledTextField CellPhoneText { get; set; }

        UIButton DoneButton { get; set; }
        UIButton CancelButton { get; set; }

        UIView HeaderView { get; set; }

        UIScrollViewWrapper ScrollView { get; set; }

        public RegisterViewController (IntPtr handle) : base (handle)
        {
        }

        public override bool ShouldAutorotate()
        {
            return Springboard.ShouldAutorotate();
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations( )
        {
            return Springboard.GetSupportedInterfaceOrientations( );
        }

        public override UIInterfaceOrientation PreferredInterfaceOrientationForPresentation( )
        {
            return Springboard.PreferredInterfaceOrientationForPresentation( );
        }

        public override bool PrefersStatusBarHidden()
        {
            return Springboard.PrefersStatusBarHidden();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();


            // setup the fake header
            HeaderView = new UIView( );
            View.AddSubview( HeaderView );
            HeaderView.Frame = new CGRect( View.Frame.Left, View.Frame.Top, View.Frame.Width, StyledTextField.StyledFieldHeight );
            HeaderView.BackgroundColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.BackgroundColor );

            string imagePath = NSBundle.MainBundle.BundlePath + "/" + PrimaryNavBarConfig.LogoFile;
            LogoView = new UIImageView( new UIImage( imagePath ) );
            HeaderView.AddSubview( LogoView );

            ScrollView = new UIScrollViewWrapper();
            ScrollView.Frame = new CGRect( View.Frame.Left, HeaderView.Frame.Bottom, View.Frame.Width, View.Frame.Height - HeaderView.Frame.Height );
            View.AddSubview( ScrollView );
            ScrollView.Parent = this;

            // logged in sanity check.
            if( RockMobileUser.Instance.LoggedIn == true ) throw new Exception("A user cannot be logged in when registering. How did you do this?" );

            BlockerView = new BlockerView( View.Frame );
            View.AddSubview( BlockerView );

            //setup styles
            View.BackgroundColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.BackgroundColor );

            UserNameText = new StyledTextField();
            ScrollView.AddSubview( UserNameText.Background );
            UserNameText.SetFrame( new CGRect( -10, View.Frame.Height * .05f, View.Frame.Width + 20, StyledTextField.StyledFieldHeight ) );
            ControlStyling.StyleTextField( UserNameText.Field, RegisterStrings.UsernamePlaceholder, ControlStylingConfig.Medium_Font_Regular, ControlStylingConfig.Medium_FontSize );
            ControlStyling.StyleBGLayer( UserNameText.Background );

            PasswordText = new StyledTextField();
            ScrollView.AddSubview( PasswordText.Background );
            PasswordText.SetFrame( new CGRect( -10, UserNameText.Background.Frame.Bottom, View.Frame.Width + 20, StyledTextField.StyledFieldHeight ) );
            PasswordText.Field.SecureTextEntry = true;
            ControlStyling.StyleTextField( PasswordText.Field, RegisterStrings.PasswordPlaceholder, ControlStylingConfig.Medium_Font_Regular, ControlStylingConfig.Medium_FontSize );
            ControlStyling.StyleBGLayer( PasswordText.Background );

            ConfirmPasswordText = new StyledTextField();
            ScrollView.AddSubview( ConfirmPasswordText.Background );
            ConfirmPasswordText.SetFrame( new CGRect( -10, PasswordText.Background.Frame.Bottom, View.Frame.Width + 20, StyledTextField.StyledFieldHeight ) );
            ConfirmPasswordText.Field.SecureTextEntry = true;
            ControlStyling.StyleTextField( ConfirmPasswordText.Field, RegisterStrings.ConfirmPasswordPlaceholder, ControlStylingConfig.Medium_Font_Regular, ControlStylingConfig.Medium_FontSize );
            ControlStyling.StyleBGLayer( ConfirmPasswordText.Background );

            NickNameText = new StyledTextField();
            ScrollView.AddSubview( NickNameText.Background );
            NickNameText.SetFrame( new CGRect( -10, ConfirmPasswordText.Background.Frame.Bottom + 40, View.Frame.Width + 20, StyledTextField.StyledFieldHeight ) );
            ControlStyling.StyleTextField( NickNameText.Field, RegisterStrings.NickNamePlaceholder, ControlStylingConfig.Medium_Font_Regular, ControlStylingConfig.Medium_FontSize );
            ControlStyling.StyleBGLayer( NickNameText.Background );

            LastNameText = new StyledTextField();
            ScrollView.AddSubview( LastNameText.Background );
            LastNameText.SetFrame( new CGRect( -10, NickNameText.Background.Frame.Bottom, View.Frame.Width + 20, StyledTextField.StyledFieldHeight ) );
            ControlStyling.StyleTextField( LastNameText.Field, RegisterStrings.LastNamePlaceholder, ControlStylingConfig.Medium_Font_Regular, ControlStylingConfig.Medium_FontSize );
            ControlStyling.StyleBGLayer( LastNameText.Background );

            EmailText = new StyledTextField();
            ScrollView.AddSubview( EmailText.Background );
            EmailText.SetFrame( new CGRect( -10, LastNameText.Background.Frame.Bottom + 40, View.Frame.Width + 20, StyledTextField.StyledFieldHeight ) );
            ControlStyling.StyleTextField( EmailText.Field, RegisterStrings.EmailPlaceholder, ControlStylingConfig.Medium_Font_Regular, ControlStylingConfig.Medium_FontSize );
            ControlStyling.StyleBGLayer( EmailText.Background );

            CellPhoneText = new StyledTextField();
            ScrollView.AddSubview( CellPhoneText.Background );
            CellPhoneText.SetFrame( new CGRect( -10, EmailText.Background.Frame.Bottom, View.Frame.Width + 20, StyledTextField.StyledFieldHeight ) );
            ControlStyling.StyleTextField( CellPhoneText.Field, RegisterStrings.CellPhonePlaceholder, ControlStylingConfig.Medium_Font_Regular, ControlStylingConfig.Medium_FontSize );
            ControlStyling.StyleBGLayer( CellPhoneText.Background );

            DoneButton = UIButton.FromType( UIButtonType.System );
            ScrollView.AddSubview( DoneButton );
            ControlStyling.StyleButton( DoneButton, RegisterStrings.RegisterButton, ControlStylingConfig.Medium_Font_Regular, ControlStylingConfig.Medium_FontSize );
            DoneButton.SizeToFit( );
            DoneButton.Frame = new CGRect( View.Frame.Left + 8, CellPhoneText.Background.Frame.Bottom + 20, ControlStyling.ButtonWidth, ControlStyling.ButtonHeight );


            CancelButton = UIButton.FromType( UIButtonType.System );
            ScrollView.AddSubview( CancelButton );
            ControlStyling.StyleButton( CancelButton, GeneralStrings.Cancel, ControlStylingConfig.Medium_Font_Regular, ControlStylingConfig.Medium_FontSize );
            CancelButton.SizeToFit( );
            CancelButton.Frame = new CGRect( ( View.Frame.Width - ControlStyling.ButtonWidth ) - 8, CellPhoneText.Background.Frame.Bottom + 20, ControlStyling.ButtonWidth, ControlStyling.ButtonHeight );


            // Allow the return on username and password to start
            // the login process
            NickNameText.Field.ShouldReturn += TextFieldShouldReturn;
            LastNameText.Field.ShouldReturn += TextFieldShouldReturn;

            EmailText.Field.ShouldReturn += TextFieldShouldReturn;

            // If submit is pressed with dirty changes, prompt the user to save them.
            DoneButton.TouchUpInside += (object sender, EventArgs e) => 
                {
                    RegisterUser( );
                };

            // On logout, make sure the user really wants to log out.
            CancelButton.TouchUpInside += (object sender, EventArgs e) => 
                {
                    // CONFIRM CANCEL
                    var actionSheet = new UIActionSheet( RegisterStrings.ConfirmCancelReg, null, GeneralStrings.Cancel, GeneralStrings.Yes, null );

                    actionSheet.ShowInView( View );

                    actionSheet.Clicked += (object s, UIButtonEventArgs ev) => 
                        {
                            if( ev.ButtonIndex == actionSheet.DestructiveButtonIndex )
                            {
                                Springboard.ResignModelViewController( this, null );
                            }
                        };
                };

            ResultView = new UIResultView( ScrollView, View.Frame.ToRectF( ), OnResultViewDone );

            ResultView.SetStyle( ControlStylingConfig.Medium_Font_Light, 
                ControlStylingConfig.Icon_Font_Secondary, 
                ControlStylingConfig.BackgroundColor,
                ControlStylingConfig.BG_Layer_Color, 
                ControlStylingConfig.BG_Layer_BorderColor, 
                ControlStylingConfig.TextField_PlaceholderTextColor,
                ControlStylingConfig.Button_BGColor, 
                ControlStylingConfig.Button_TextColor );
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            ScrollView.ContentSize = new CGSize( 0, View.Bounds.Height + ( View.Bounds.Height * .25f ) );

            // setup the header shadow
            UIBezierPath shadowPath = UIBezierPath.FromRect( HeaderView.Bounds );
            HeaderView.Layer.MasksToBounds = false;
            HeaderView.Layer.ShadowColor = UIColor.Black.CGColor;
            HeaderView.Layer.ShadowOffset = new CGSize( 0.0f, .0f );
            HeaderView.Layer.ShadowOpacity = .23f;
            HeaderView.Layer.ShadowPath = shadowPath.CGPath;

            LogoView.Layer.Position = new CGPoint( HeaderView.Bounds.Width / 2, HeaderView.Bounds.Height / 2 );
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            // reset the background colors
            UserNameText.Background.BackgroundColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.BG_Layer_Color );
            PasswordText.Background.BackgroundColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.BG_Layer_Color );
            ConfirmPasswordText.Background.BackgroundColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.BG_Layer_Color );

            NickNameText.Background.BackgroundColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.BG_Layer_Color );
            LastNameText.Background.BackgroundColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.BG_Layer_Color );
            EmailText.Background.BackgroundColor = Rock.Mobile.PlatformUI.Util.GetUIColor( ControlStylingConfig.BG_Layer_Color );

            ScrollView.ContentOffset = CGPoint.Empty;

            // set values
            UserNameText.Field.Text = string.Empty;

            PasswordText.Field.Text = string.Empty;
            ConfirmPasswordText.Field.Text = string.Empty;

            NickNameText.Field.Text = string.Empty;
            LastNameText.Field.Text = string.Empty;

            EmailText.Field.Text = string.Empty;

            // setup the phone number
            CellPhoneText.Field.Delegate = new Rock.Mobile.PlatformSpecific.iOS.UI.PhoneNumberFormatterDelegate();
            CellPhoneText.Field.Text = string.Empty;
            CellPhoneText.Field.Delegate.ShouldChangeCharacters( CellPhoneText.Field, new NSRange( CellPhoneText.Field.Text.Length, 0 ), "" );

            State = RegisterState.None;
            ResultView.Hide( );
        }

        public bool TextFieldShouldReturn( UITextField textField )
        {
            if( textField.IsFirstResponder == true )
            {
                textField.ResignFirstResponder();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Ensure all required fields have data
        /// </summary>
        public bool ValidateInput( )
        {
            bool result = true;

            // validate there's text in all required fields
            uint targetColor = ControlStylingConfig.BG_Layer_Color;
            if ( string.IsNullOrEmpty( UserNameText.Field.Text ) == true )
            {
                targetColor = ControlStylingConfig.BadInput_BG_Layer_Color;
                result = false;
            }
            Rock.Mobile.PlatformSpecific.iOS.UI.Util.AnimateViewColor( targetColor, UserNameText.Background );


            // for the password, if EITHER field is blank, that's not ok, OR if the passwords don't match, also not ok.
            targetColor = ControlStylingConfig.BG_Layer_Color;
            if ( (string.IsNullOrEmpty( PasswordText.Field.Text ) == true || string.IsNullOrEmpty( ConfirmPasswordText.Field.Text ) == true) ||
                ( PasswordText.Field.Text != ConfirmPasswordText.Field.Text ) )
            {
                targetColor = ControlStylingConfig.BadInput_BG_Layer_Color;
                result = false;
            }
            Rock.Mobile.PlatformSpecific.iOS.UI.Util.AnimateViewColor( targetColor, PasswordText.Background );
            Rock.Mobile.PlatformSpecific.iOS.UI.Util.AnimateViewColor( targetColor, ConfirmPasswordText.Background );


            targetColor = ControlStylingConfig.BG_Layer_Color;
            if ( string.IsNullOrEmpty( NickNameText.Field.Text ) == true )
            {
                targetColor = ControlStylingConfig.BadInput_BG_Layer_Color;
                result = false;
            }
            Rock.Mobile.PlatformSpecific.iOS.UI.Util.AnimateViewColor( targetColor, NickNameText.Background );


            targetColor = ControlStylingConfig.BG_Layer_Color;
            if ( string.IsNullOrEmpty( LastNameText.Field.Text ) == true )
            {
                targetColor = ControlStylingConfig.BadInput_BG_Layer_Color;
                result = false;
            }
            Rock.Mobile.PlatformSpecific.iOS.UI.Util.AnimateViewColor( targetColor, LastNameText.Background );


            // cell phone OR email is fine
            targetColor = ControlStylingConfig.BG_Layer_Color;
            if ( string.IsNullOrEmpty( EmailText.Field.Text ) == true && string.IsNullOrEmpty( CellPhoneText.Field.Text ) == true )
            {
                // if failure, only color email
                targetColor = ControlStylingConfig.BadInput_BG_Layer_Color;
                result = false;
            }
            Rock.Mobile.PlatformSpecific.iOS.UI.Util.AnimateViewColor( targetColor, EmailText.Background );

            return result;
        }

        void RegisterUser()
        {
            if ( State == RegisterState.None )
            {
                // make sure they entered all required fields
                if ( ValidateInput( ) )
                {
                    BlockerView.FadeIn( 
                        delegate
                        {
                            // force the UI to scroll back up
                            ScrollView.ContentOffset = CGPoint.Empty;
                            ScrollView.ScrollEnabled = false;

                            State = RegisterState.Trying;

                            // create a new user and submit them
                            Rock.Client.Person newPerson = new Rock.Client.Person();
                            Rock.Client.PhoneNumber newPhoneNumber = new Rock.Client.PhoneNumber();

                            // copy all the edited fields into the person object
                            newPerson.Email = EmailText.Field.Text;

                            newPerson.NickName = NickNameText.Field.Text;
                            newPerson.LastName = LastNameText.Field.Text;

                            // Update their cell phone. 
                            if ( string.IsNullOrEmpty( CellPhoneText.Field.Text ) == false )
                            {
                                // update the phone number
                                string digits = CellPhoneText.Field.Text.AsNumeric( );
                                newPhoneNumber.Number = digits;
                                newPhoneNumber.NumberFormatted = digits.AsPhoneNumber( );
                                newPhoneNumber.NumberTypeValueId = GeneralConfig.CellPhoneValueId;
                            }

                            RockApi.Instance.RegisterNewUser( newPerson, newPhoneNumber, UserNameText.Field.Text, PasswordText.Field.Text,
                                delegate(System.Net.HttpStatusCode statusCode, string statusDescription )
                                {
                                    if ( Rock.Mobile.Network.Util.StatusInSuccessRange( statusCode ) == true )
                                    {
                                        State = RegisterState.Success;
                                        ResultView.Display( RegisterStrings.RegisterStatus_Success, 
                                            ControlStylingConfig.Result_Symbol_Success, 
                                            RegisterStrings.RegisterResult_Success,
                                            GeneralStrings.Done );
                                    }
                                    else
                                    {
                                        State = RegisterState.Fail;
                                        ResultView.Display( RegisterStrings.RegisterStatus_Failed, 
                                            ControlStylingConfig.Result_Symbol_Failed, 
                                            RegisterStrings.RegisterResult_Failed,
                                            GeneralStrings.Done );
                                    }

                                    BlockerView.FadeOut( null );
                                } );
                        } );
                }
            }
        }

        void OnResultViewDone( )
        {
            switch ( State )
            {
                case RegisterState.Success:
                {
                    Springboard.ResignModelViewController( this, null );
                    ScrollView.ScrollEnabled = true;
                    State = RegisterState.None;
                    break;
                }

                case RegisterState.Fail:
                {
                    ResultView.Hide( );
                    ScrollView.ScrollEnabled = true;
                    State = RegisterState.None;
                    break;
                }
            }
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            ResultView.Hide( );
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded( touches, evt );

            // if they tap somewhere outside of the text fields, 
            // hide the keyboard
            TextFieldShouldReturn( NickNameText.Field );
            TextFieldShouldReturn( LastNameText.Field );

            TextFieldShouldReturn( CellPhoneText.Field );
            TextFieldShouldReturn( EmailText.Field );
        }
    }
}