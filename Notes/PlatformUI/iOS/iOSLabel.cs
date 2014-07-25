﻿#if __IOS__
using System;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace Notes
{
    namespace PlatformUI
    {
        public class iOSLabel : PlatformLabel
        {
            UILabel Label { get; set; }

            public iOSLabel( )
            {
                Label = new UILabel();
                Label.Layer.AnchorPoint = new PointF (0, 0);
                Label.TextAlignment = UITextAlignment.Left;
                Label.LineBreakMode = UILineBreakMode.WordWrap;
                Label.Lines = 0;
            }

            //TODO: Do not let outside entities call this (right now I am till I finish the port)
            public static UIColor GetUIColor(uint color)
            {
                return new UIColor(
                    (float)((color & 0xFF000000) >> 24) / 255, //TODO: obviously completely unacceptable.
                    (float)((color & 0x00FF0000) >> 16) / 255, 
                    (float)((color & 0x0000FF00) >> 8) / 255, 
                    (float)((color & 0x000000FF)) / 255);
            }

            // Properties
            public override void SetFont(string fontName, float fontSize)
            {
                Label.Font = UIFont.FromName(fontName, fontSize);
            }

            protected override void setBackgroundColor(uint backgroundColor)
            {
                Label.Layer.BackgroundColor = GetUIColor(backgroundColor).CGColor;
            }

            protected override float getOpacity()
            {
                return Label.Layer.Opacity;
            }

            protected override void setOpacity(float opacity)
            {
                Label.Layer.Opacity = opacity;
            }

            protected override float getZPosition()
            {
                return Label.Layer.ZPosition;
            }
            protected override void setZPosition(float zPosition)
            {
                Label.Layer.ZPosition = zPosition;
            }

            protected override RectangleF getBounds()
            {
                return Label.Bounds;
            }

            protected override void setBounds(RectangleF bounds)
            {
                Label.Bounds = bounds;
            }

            protected override RectangleF getFrame()
            {
                return Label.Frame;
            }

            protected override void setFrame(RectangleF frame)
            {
                Label.Frame = frame;
            }

            protected override  PointF getPosition()
            {
                return Label.Layer.Position;
            }
            protected override void setPosition(PointF position)
            {
                Label.Layer.Position = position;
            }

            protected override void setTextColor(uint color)
            {
                Label.TextColor = GetUIColor(color);
            }

            protected override string getText()
            {
                return Label.Text;
            }

            protected override void setText(string text)
            {
                Label.Text = text;
            }

            protected override TextAlignment getTextAlignment()
            {
                return (TextAlignment)Label.TextAlignment;
            }

            protected override void setTextAlignment(TextAlignment alignment)
            {
                Label.TextAlignment = (UITextAlignment)alignment;
            }


            public override void AddAsSubview(object masterView)
            {
                // we know that masterView will be an iOS View.
                UIView view = masterView as UIView;
                if(view == null)
                {
                    throw new InvalidCastException("Object passed to iOS AddAsSubview must be a UIView.");
                }

                view.AddSubview(Label);
            }

            public override void RemoveAsSubview()
            {
                Label.RemoveFromSuperview();
            }

            public override void SizeToFit()
            {
                Label.SizeToFit();
            }
        }
    }
}
#endif
