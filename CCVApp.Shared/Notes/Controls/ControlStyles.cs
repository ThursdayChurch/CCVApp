﻿using System;
using System.Xml;
using System.Drawing;
using System.IO;
using Rock.Mobile.Network;

namespace CCVApp
{
    namespace Shared
    {
        namespace Notes
        {
            namespace Styles
            {
                /// <summary>
                /// Alignment options for controls within parents.
                /// </summary>
                public enum Alignment
                {
                    Inherit,
                    Left,
                    Right,
                    Center
                }

                /// <summary>
                /// Casing options for text within controls.
                /// </summary>
                public enum TextCase
                {
                    Normal,
                    Upper,
                    Lower
                }

                /// <summary>
                /// Defines the font a control's text should use.
                /// Anything null uses values from the control's default style.
                /// </summary>
                public struct FontParams
                {
                    public string mName;
                    public float? mSize;
                    public uint? mColor;

                    public FontParams( string name, float size, uint color )
                    {
                        mName = name;
                        mSize = size;
                        mColor = color;
                    }
                }

                /// <summary>
                /// Defines common styling values for controls.
                /// </summary>
                public struct Style
                {
                    /// <summary>
                    /// The background color for a control. (Not used by all controls)
                    /// </summary>
                    public uint? mBackgroundColor;

                    /// <summary>
                    /// The alignment of the control within its parent.
                    /// </summary>
                    public Alignment? mAlignment;

                    /// <summary>
                    /// The font the control's text should use, as well as what will be passed to children.
                    /// </summary>
                    public FontParams mFont;

                    /// <summary>
                    /// Amount of padding between control's content and left edge.
                    /// </summary>
                    public float? mPaddingLeft;

                    /// <summary>
                    /// Amount of padding between control's content and top edge.
                    /// </summary>
                    public float? mPaddingTop;

                    /// <summary>
                    /// Amount of padding between control's content and right edge.
                    /// </summary>
                    public float? mPaddingRight;

                    /// <summary>
                    /// Amount of padding between control's content and bottom edge.
                    /// </summary>
                    public float? mPaddingBottom;

                    /// <summary>
                    /// The character to use for list bullet points
                    /// </summary>
                    public string mListBullet;

                    /// <summary>
                    /// The indention amount for lists
                    /// </summary>
                    public float? mListIndention;

                    /// <summary>
                    /// The casing for the text within a control. As is, Upper or Lower.
                    /// </summary>
                    public TextCase? mTextCase;

                    public void Initialize( )
                    {
                        mFont = new FontParams( );
                    }

                    // Utility function for retrieving a value without knowing if it's percent or not.
                    public static float GetStyleValue( float? value, float valueForPerc )
                    {
                        float styleValue = 0;

                        if( value.HasValue )
                        {
                            float realValue = value.Value;

                            // If it's a percent
                            if( realValue <= 1 )
                            {
                                // convert using valueForPerc as the source
                                styleValue = valueForPerc * realValue;
                            }
                            else
                            {
                                // otherwise take the value
                                styleValue = realValue;
                            }
                        }

                        return styleValue;
                    }

                    public static UInt32 ParseColor( string color )
                    {
                        if( color[0] != '#' ) throw new Exception( String.Format( "Colors must be in the format #RRGGBBAA. Color found: {0}", color ) );
                        return Convert.ToUInt32( color.Substring( 1 ), 16 ); //skip the first character
                    }

                    // Utility function for parsing common style attributes
                    public static void ParseStyleAttributes( XmlReader reader, ref Style style )
                    {
                        // This builds a style with the following conditions
                        // Values from XML come first. This means the control specifically asked for this.
                        // Values already set come second. This means the parent specifically asked for this.
                        // Padding is an exception AND DOES NOT INHERIT
                        // Unlike the WithDefaults version, this will allow style values to remain null. Important
                        // for container controls that don't want to force styles.
                        string result = reader.GetAttribute( "FontName" );
                        if( string.IsNullOrEmpty( result ) == false )
                        {
                            style.mFont.mName = result;
                        }

                        result = reader.GetAttribute( "FontSize" );
                        if( string.IsNullOrEmpty( result ) == false )
                        {
                            style.mFont.mSize = float.Parse( result );
                        }

                        result = reader.GetAttribute( "FontColor" );
                        if( string.IsNullOrEmpty( result ) == false )
                        {
                            style.mFont.mColor = ParseColor( result );
                        }

                        // check for alignment
                        result = reader.GetAttribute( "Alignment" );
                        if( string.IsNullOrEmpty( result ) == false )
                        {
                            switch( result )
                            {
                                case "Left": style.mAlignment = Styles.Alignment.Left; break;
                                case "Right": style.mAlignment = Styles.Alignment.Right; break;
                                case "Center": style.mAlignment = Styles.Alignment.Center; break;

                                default: throw new Exception( "Unknown alignment type specified." );
                            }
                        }

                        // check text casing
                        result = reader.GetAttribute( "TextCase" );
                        if( string.IsNullOrEmpty( result ) == false )
                        {
                            switch( result )
                            {
                                case "Normal": style.mTextCase = Styles.TextCase.Normal; break;
                                case "Upper": style.mTextCase = Styles.TextCase.Upper; break;
                                case "Lower": style.mTextCase = Styles.TextCase.Lower; break;

                                default: throw new Exception( "Unknown text case specified." );
                            }
                        }

                        // check for background color
                        result = reader.GetAttribute( "BackgroundColor" );
                        if( string.IsNullOrEmpty( result ) == false )
                        {
                            style.mBackgroundColor = ParseColor( result );
                        }

                        // check for list bullet point (lists only)
                        result = reader.GetAttribute( "BulletPoint" );
                        if( string.IsNullOrEmpty( result ) == false )
                        {
                            style.mListBullet = result;
                        }

                        // check for list indentation (lists only)
                        result = reader.GetAttribute( "Indentation" );
                        if( string.IsNullOrEmpty( result ) == false )
                        {
                            float denominator = 1.0f;
                            if( result.Contains( "%" ) )
                            {
                                result = result.Trim( '%' );
                                denominator = 100.0f;
                            }

                            style.mListIndention = float.Parse( result ) / denominator;
                        }


                        // check for padding; DOES NOT INHERIT
                        result = reader.GetAttribute( "Padding" );
                        if( string.IsNullOrEmpty( result ) == false )
                        {
                            float denominator = 1.0f;
                            if( result.Contains( "%" ) )
                            {
                                result = result.Trim( '%' );
                                denominator = 100.0f;
                            }

                            // set it for all padding values
                            style.mPaddingLeft = float.Parse( result ) / denominator;
                            style.mPaddingTop = style.mPaddingLeft;
                            style.mPaddingRight = style.mPaddingLeft;
                            style.mPaddingBottom = style.mPaddingLeft;
                        }
                        else
                        {
                            result = reader.GetAttribute( "PaddingLeft" );
                            if( string.IsNullOrEmpty( result ) == false )
                            {
                                float denominator = 1.0f;
                                if( result.Contains( "%" ) )
                                {
                                    result = result.Trim( '%' );
                                    denominator = 100.0f;
                                }

                                style.mPaddingLeft = float.Parse( result ) / denominator;
                            }
                            else
                            {
                                style.mPaddingLeft = null;
                            }

                            result = reader.GetAttribute( "PaddingTop" );
                            if( string.IsNullOrEmpty( result ) == false )
                            {
                                float denominator = 1.0f;
                                if( result.Contains( "%" ) )
                                {
                                    result = result.Trim( '%' );
                                    denominator = 100.0f;
                                }

                                style.mPaddingTop = float.Parse( result ) / denominator;
                            }
                            else
                            {
                                style.mPaddingTop = null;
                            }

                            result = reader.GetAttribute( "PaddingRight" );
                            if( string.IsNullOrEmpty( result ) == false )
                            {
                                float denominator = 1.0f;
                                if( result.Contains( "%" ) )
                                {
                                    result = result.Trim( '%' );
                                    denominator = 100.0f;
                                }

                                style.mPaddingRight = float.Parse( result ) / denominator;
                            }
                            else
                            {
                                style.mPaddingRight = null;
                            }

                            result = reader.GetAttribute( "PaddingBottom" );
                            if( string.IsNullOrEmpty( result ) == false )
                            {
                                float denominator = 1.0f;
                                if( result.Contains( "%" ) )
                                {
                                    result = result.Trim( '%' );
                                    denominator = 100.0f;
                                }

                                style.mPaddingBottom = float.Parse( result ) / denominator;
                            }
                            else
                            {
                                style.mPaddingBottom = null;
                            }
                        }
                    }

                    public static void ParseStyleAttributesWithDefaults( XmlReader reader, ref Style style, ref Style defaultStyle )
                    {
                        // This builds a style with the following conditions
                        // Values from XML come first. This means the control specifically asked for this.
                        // Values already set come second. This means the parent specifically asked for this.
                        // Values from defaultStyle come last. This means no one set the style, so it should use the control default.

                        // first use the normal parsing, which will result in a style with potential null values.
                        ParseStyleAttributes( reader, ref style );

                        // Lastly, merge defaultStyle values for anything null in style 
                        MergeStyleAttributesWithDefaults( ref style, ref defaultStyle );
                    }

                    public static void MergeStyleAttributesWithDefaults( ref Style style, ref Style defaultStyle )
                    {
                        // validate everything, and for anything null, use the default provided.
                        if( style.mFont.mName == null )
                        {
                            style.mFont.mName = defaultStyle.mFont.mName;
                        }

                        if( style.mFont.mSize.HasValue == false )
                        {
                            style.mFont.mSize = defaultStyle.mFont.mSize;
                        }

                        if( style.mFont.mColor.HasValue == false )
                        {
                            style.mFont.mColor = defaultStyle.mFont.mColor;
                        }

                        // check for alignment
                        if( style.mAlignment.HasValue == false )
                        {
                            style.mAlignment = defaultStyle.mAlignment;
                        }

                        // check for text case
                        if ( style.mTextCase.HasValue == false )
                        {
                            style.mTextCase = defaultStyle.mTextCase;
                        }

                        // check for list specifics
                        if ( style.mListBullet == null )
                        {
                            style.mListBullet = defaultStyle.mListBullet;
                        }

                        if ( style.mListIndention.HasValue == false )
                        {
                            style.mListIndention = defaultStyle.mListIndention;
                        }

                        // check for padding
                        if( style.mPaddingLeft.HasValue == false )
                        {
                            style.mPaddingLeft = defaultStyle.mPaddingLeft;
                        }

                        if( style.mPaddingTop.HasValue == false )
                        {
                            style.mPaddingTop = defaultStyle.mPaddingTop;
                        }

                        if( style.mPaddingRight.HasValue == false )
                        {
                            style.mPaddingRight = defaultStyle.mPaddingRight;
                        }

                        if( style.mPaddingBottom.HasValue == false )
                        {
                            style.mPaddingBottom = defaultStyle.mPaddingBottom;
                        }

                        // check for background color
                        if( style.mBackgroundColor.HasValue == false )
                        {
                            style.mBackgroundColor = defaultStyle.mBackgroundColor;
                        }
                    }
                }
            }

            //This is a static class containing all the default styles for controls
            public class ControlStyles
            {
                ///<summary>>
                /// Delegate used to notify the creator that styles have been downloaded and created.
                /// </summary>
                public delegate void StylesCreated( Exception e );

                /// <summary>
                /// Delegate object to invoke.
                /// </summary>
                static StylesCreated mStylesCreatedDelegate;

                /// <summary>
                /// URL of the style sheet to download. (For reference / debugging)
                /// </summary>
                static string mStyleSheetUrl;

                /// <summary>
                /// Actual XML of the style sheet. Used when needing to rebuild notes for an orientation change.
                /// </summary>
                /// <value>The style sheet xml.</value>
                public static string StyleSheetXml { get; protected set; }

                // These are to be referenced globally as needed
                /// <summary>
                /// Note control's default styles if none are specified by a parent or in NoteScript XML.
                /// </summary>
                public static Styles.Style mMainNote;

                /// <summary>
                /// Paragraph control's default styles if none are specified by a parent or in NoteScript XML.
                /// </summary>
                public static Styles.Style mParagraph;

                /// <summary>
                /// Stack control's default styles if none are specified by a parent or in NoteScript XML.
                /// </summary>
                public static Styles.Style mStackPanel;

                /// <summary>
                /// Canvas' default style if none are specified by a parent or in NoteScript XML.
                /// </summary>
                public static Styles.Style mCanvas;

                /// <summary>
                /// Revealbox's control's default styles if none are specified by a parent or in NoteScript XML.
                /// </summary>
                public static Styles.Style mRevealBox;

                /// <summary>
                /// TextInput control's default styles if none are specified by a parent or in NoteScript XML.
                /// </summary>
                public static Styles.Style mTextInput;

                /// <summary>
                /// Header Container's default styles if none are specified by a parent or in NoteScript XML.
                /// </summary>
                public static Styles.Style mHeaderContainer;

                /// <summary>
                /// Header Title control's default styles if none are specified by a parent or in NoteScript XML.
                /// </summary>
                public static Styles.Style mHeaderTitle;

                /// <summary>
                /// Header Date control's default styles if none are specified by a parent or in NoteScript XML.
                /// </summary>
                public static Styles.Style mHeaderDate;

                /// <summary>
                /// Header Speaker control's default styles if none are specified by a parent or in NoteScript XML.
                /// </summary>
                public static Styles.Style mHeaderSpeaker;

                /// <summary>
                /// Quote control's default styles if none are specified by a parent or in NoteScript XML.
                /// </summary>
                public static Styles.Style mQuote;

                /// <summary>
                /// Text's default styles if none are specified by a parent or in NoteScript XML.
                /// Note that text is not an actual control, rather it uses Labels.
                /// </summary>
                public static Styles.Style mText;

                /// <summary>
                /// List Item's default style if none are specified by a parent or in NoteScript XML.
                /// </summary>
                public static Styles.Style mListItem;

                /// <summary>
                /// List's default style if none are specified by a parent or in NoteScript XML.
                /// </summary>
                public static Styles.Style mList;

                static void CreateHeaderStyle()
                {
                    // Default the header container to no padding and left alignment.
                    mHeaderContainer = new Styles.Style( );
                    mHeaderContainer.Initialize();
                    mHeaderContainer.mAlignment = Styles.Alignment.Left;
                    mHeaderContainer.mPaddingLeft = 0;
                    mHeaderContainer.mPaddingTop = 0;
                    mHeaderContainer.mPaddingRight = 0;
                    mHeaderContainer.mPaddingBottom = 0;


                    //Title should be large, bevan, centered and white
                    // Also, prefer an upper case title.
                    mHeaderTitle = new Styles.Style( );
                    mHeaderTitle.Initialize();
                    mHeaderTitle.mAlignment = Styles.Alignment.Center;
                    mHeaderTitle.mTextCase = Styles.TextCase.Upper;
                    mHeaderTitle.mFont.mName = "Bevan";
                    mHeaderTitle.mFont.mSize = 18;
                    mHeaderTitle.mFont.mColor = 0xFFFFFFFF;

                    // Date should be medium, verdana, left and white
                    mHeaderDate = new Styles.Style( );
                    mHeaderDate.Initialize();
                    mHeaderDate.mAlignment = Styles.Alignment.Left;
                    mHeaderDate.mTextCase = Styles.TextCase.Normal;
                    mHeaderDate.mFont.mName = "Bevan";
                    mHeaderDate.mFont.mSize = 12;
                    mHeaderDate.mFont.mColor = 0xFFFFFFFF;

                    // Speaker should be mediu, verdana, right and white
                    mHeaderSpeaker = new Styles.Style( );
                    mHeaderSpeaker.Initialize();
                    mHeaderSpeaker.mAlignment = Styles.Alignment.Right;
                    mHeaderSpeaker.mTextCase = Styles.TextCase.Normal;
                    mHeaderSpeaker.mFont.mName = "Bevan";
                    mHeaderSpeaker.mFont.mSize = 12;
                    mHeaderSpeaker.mFont.mColor = 0xFFFFFFFF;
                }

                static void CreateMainNoteStyle()
                {
                    // note should have a black background and no alignment
                    mMainNote = new Styles.Style( );
                    mMainNote.Initialize();

                    mMainNote.mBackgroundColor = 0x000000FF;
                }

                static void CreateParagraphStyle()
                {
                    // paragraphs should have a preference for left alignment
                    mParagraph = new Styles.Style( );
                    mParagraph.Initialize();

                    mParagraph.mAlignment = Styles.Alignment.Left;
                }

                static void CreateStackPanelStyle()
                {
                    // like paragraphs, stack panels shouldn't care about anything but alignment
                    mStackPanel = new Styles.Style( );
                    mStackPanel.Initialize( );

                    mStackPanel.mAlignment = Styles.Alignment.Left;
                }

                static void CreateCanvasStyle()
                {
                    // like stacks and paragraphs, canvas shouldn't care about anything
                    // but alignment
                    mCanvas = new Styles.Style( );
                    mCanvas.Initialize( );

                    mCanvas.mAlignment = Styles.Alignment.Left;
                }

                static void CreateRevealBoxStyle()
                {
                    mRevealBox = new Styles.Style( );
                    mRevealBox.Initialize();

                    // make reveal boxes redish
                    mRevealBox.mAlignment = Styles.Alignment.Left;
                    mRevealBox.mFont.mName = "Bevan";
                    mRevealBox.mFont.mSize = 12;
                    mRevealBox.mFont.mColor = 0x6d1c1cFF;
                    mRevealBox.mTextCase = Styles.TextCase.Normal;
                }

                static void CreateTextInputStyle()
                {
                    mTextInput = new Styles.Style( );
                    mTextInput.Initialize();

                    // Make text input small by default
                    mTextInput.mAlignment = Styles.Alignment.Left;
                    mTextInput.mFont.mName = "Bevan";
                    mTextInput.mFont.mSize = 8;
                    mTextInput.mFont.mColor = 0xFFFFFFFF;
                    mTextInput.mTextCase = Styles.TextCase.Normal;
                }

                static void CreateQuoteStyle()
                {
                    mQuote = new Styles.Style( );
                    mQuote.Initialize();

                    // Make quotes redish
                    mQuote.mAlignment = Styles.Alignment.Left;
                    mQuote.mFont.mName = "Bevan";
                    mQuote.mFont.mSize = 12;
                    mQuote.mFont.mColor = 0x6d1c1cFF;
                    mQuote.mTextCase = Styles.TextCase.Normal;
                }

                static void CreateTextStyle()
                {
                    mText = new Styles.Style( );
                    mText.Initialize();

                    mText.mAlignment = Styles.Alignment.Left;
                    mText.mFont.mName = "Bevan";
                    mText.mFont.mSize = 12;
                    mText.mFont.mColor = 0xFFFFFFFF;
                    mText.mTextCase = Styles.TextCase.Normal;
                }

                static void CreateListStyle()
                {
                    mList = new Styles.Style( );
                    mList.Initialize( );

                    mList.mAlignment = Styles.Alignment.Left;
                    mList.mListBullet = "•"; //we want a nice looking bullet as default
                    mList.mListIndention = 25.0f;
                }

                static void CreateListItemStyle()
                {
                    mListItem = new Styles.Style( );
                    mListItem.Initialize( );
                }

                public static void Initialize( string styleSheetUrl, string styleSheetXml, StylesCreated stylesCreatedDelegate )
                {
                    // Create each control's default style. And put some defaults...for the defaults.
                    //(Seriously, that way if it doesn't exist in XML we still have a value.)
                    CreateMainNoteStyle();
                    CreateParagraphStyle();
                    CreateStackPanelStyle();
                    CreateCanvasStyle();
                    CreateRevealBoxStyle();
                    CreateTextInputStyle();
                    CreateQuoteStyle();
                    CreateTextStyle();
                    CreateHeaderStyle();
                    CreateListItemStyle();
                    CreateListStyle();

                    mStylesCreatedDelegate = stylesCreatedDelegate;


                    // store the styles URL so we can download it
                    mStyleSheetUrl = styleSheetUrl;

                    // if an existing XML stream wasn't provided, download via the URL
                    if( styleSheetXml == null )
                    {
                        HttpWebRequest.Instance.MakeAsyncRequest( mStyleSheetUrl, OnCompletion );
                    }
                    else
                    {
                        // otherwise we can directly parse the XML
                        ParseStyles( styleSheetXml );
                    }
                }

                static void OnCompletion( Exception ex, System.Collections.Generic.Dictionary<string, string> responseHeaders, string body )
                {
                    if( ex == null )
                    {
                        ParseStyles( body );
                    }
                    else
                    {
                        mStylesCreatedDelegate( ex );
                    }
                }

                protected static void ParseStyles( string styleSheetXml )
                {
                    Exception exception = null;
                    try
                    {
                        StyleSheetXml = styleSheetXml;

                        // now use a reader to get each element
                        XmlReader reader = XmlReader.Create( new StringReader( styleSheetXml ) );

                        bool finishedParsing = false;

                        // look at each element, as they all define styles for our controls
                        while( finishedParsing == false && reader.Read( ) )
                        {
                            switch( reader.NodeType )
                            {
                            //Find the control elements
                                case XmlNodeType.Element:
                                {
                                    //most controls don't care about anything other than the basic attributes,
                                    // so we can use a common Parse function. Certain styles may need to define more specific things,
                                    // for which we can add special parse classes
                                    switch( reader.Name )
                                    {
                                        case "Note": Notes.Styles.Style.ParseStyleAttributes( reader, ref mMainNote ); break;
                                        case "Paragraph": Notes.Styles.Style.ParseStyleAttributes( reader, ref mParagraph ); break;
                                        case "RevealBox": Notes.Styles.Style.ParseStyleAttributes( reader, ref mRevealBox ); break;
                                        case "TextInput": Notes.Styles.Style.ParseStyleAttributes( reader, ref mTextInput ); break;
                                        case "Quote": Notes.Styles.Style.ParseStyleAttributes( reader, ref mQuote ); break;
                                        case "Header.Container": Notes.Styles.Style.ParseStyleAttributes( reader, ref mHeaderContainer ); break;
                                        case "Header.Speaker": Notes.Styles.Style.ParseStyleAttributes( reader, ref mHeaderSpeaker ); break;
                                        case "Header.Date": Notes.Styles.Style.ParseStyleAttributes( reader, ref mHeaderDate ); break;
                                        case "Header.Title": Notes.Styles.Style.ParseStyleAttributes( reader, ref mHeaderTitle ); break;
                                        case "Text": Notes.Styles.Style.ParseStyleAttributes( reader, ref mText ); break;
                                        case "List": Notes.Styles.Style.ParseStyleAttributes( reader, ref mList ); break;
                                        case "ListItem": Notes.Styles.Style.ParseStyleAttributes( reader, ref mListItem ); break;
                                    }
                                    break;
                                }

                                case XmlNodeType.EndElement:
                                {
                                    if( reader.Name == "Styles" )
                                    {
                                        finishedParsing = true;
                                    }
                                    break;
                                }
                            }
                        }
                    } 
                    catch( Exception ex )
                    {
                        exception = ex;
                    }

                    mStylesCreatedDelegate( exception );
                }
            }
        }
    }
}
    