using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ColourPickerComponents
{
    public partial class ValueSlider : BaseColourControl
    {
        // selectedColor is the actual value selected
        // by the user. fullColor is the same color, 
        // with its brightness set to 255.
        private Color SelectedColour = Color.White;
        private Color fullcolour = Color.White;
        public Color colourBeforeClick;

        private Rectangle paintArea;
        private Point brightnessPoint;
        private Bitmap gradientImage;
        
        public ValueSlider()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        private void BrightnessSlider_Load(object sender, EventArgs e)
        {
            this.CalcPositions();
            this.UpdateBitmap();
            this.Refresh();
        }

        private void UpdateBitmap()
        {
            Rectangle r = new Rectangle(0, 0, 50, 200);
            using (LinearGradientBrush lgb = new LinearGradientBrush(r,this.fullcolour,Color.Black,LinearGradientMode.Vertical))
            {

                // Create a new bitmap containing
                // the color wheel gradient, so the 
                // code only needs to do all this 
                // work once. Later code uses the bitmap
                // rather than recreating the gradient.
                gradientImage = new Bitmap(50, 200, PixelFormat.Format32bppArgb);

                using (Graphics newGraphics = Graphics.FromImage(gradientImage))
                {
                    newGraphics.FillRectangle(lgb, r);
                }
            } 
        }

        private void CalcPositions()
        {
            Rectangle old = this.paintArea;
            this.paintArea = new Rectangle(this.Left, this.Top, this.Width, this.Height);
            if (old.Height == 0) // first time round
            {
                this.brightnessPoint = new Point(0, (int)((double)this.paintArea.Height * 0.2d));
            }
            else
            {
                double prop = (double)this.brightnessPoint.Y / (double)old.Height;
                this.brightnessPoint.Y = (int)(prop * (double)this.paintArea.Height);
            }
        }

        public void SetColour(Color colour)
        {
            this.fullcolour = colour;
            this.UpdateBitmap();
            this.Refresh();
            this.UpdateColour(this.CalcColour());
        }

        private void ValueSlider_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(this.gradientImage, new Rectangle(0, 0, paintArea.Width, paintArea.Height));
            if (this.ShowMarker)
            {
                ColourHandler.HSV alt = ColourHandler.RGBtoHSV(new ColourHandler.RGB(this.SelectedColour.R, this.SelectedColour.G, this.SelectedColour.B));
                alt.Saturation = 0;
                alt.value = 255 - alt.value;
                using (Pen pen = new Pen(ColourHandler.HSVtoColour(alt)))
                {
                    pen.Width = this.MarkerPenWidth;
                    e.Graphics.DrawRectangle(pen, 0 - pen.Width, this.brightnessPoint.Y - pen.Width, this.paintArea.Width + (pen.Width * 2), this.MarkerWidth * 2);
                }
            }
        }

        private void ValueSlider_Resize(object sender, EventArgs e)
        {
            this.CalcPositions();
            this.UpdateBitmap();
            this.Refresh();
        }

        /////////////////////////////
        //
        // MOUSE EVENTS
        //
        /////////////////////////////

        private void ValueSlider_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.mouseInRegion)
            {
                this.mouseDown = true;
                this.colourBeforeClick = this.SelectedColour;
                this.UpdateBrightnessPoint(e.Location);
            }
        }

        private void ValueSlider_MouseMove(object sender, MouseEventArgs e)
        {
            this.mouseInRegion = true;
            if (this.mouseDown)
            {
                this.UpdateBrightnessPoint(e.Location);
            }
        }

        private void ValueSlider_MouseLeave(object sender, EventArgs e)
        {
            this.mouseInRegion = false;
            if (this.mouseDown)
            {
                this.ResetColorAfterDragout();
            }
        }

        private void ValueSlider_MouseUp(object sender, MouseEventArgs e)
        {
            this.mouseDown = false;
        }

        private void ResetColorAfterDragout()
        {
            this.UpdateColour(this.colourBeforeClick);
        }

        private void UpdateBrightnessPoint(Point point)
        {
            if (point.Y < 0)
            {
                this.brightnessPoint.Y = 0;
            }
            else if (point.Y > (this.Height-this.MarkerWidth))
            {
                this.brightnessPoint.Y = this.Height - this.MarkerWidth;
            }
            else
            {
                this.brightnessPoint.Y = point.Y;
            }
            this.UpdateColour(this.CalcColour());
        }

        private Color CalcColour()
        {
            this.HSV = ColourHandler.RGBtoHSV(new ColourHandler.RGB(this.fullcolour.R, this.fullcolour.G,this.fullcolour.B));
            double value = ((double)(this.brightnessPoint.Y) / (double)(this.Height-this.MarkerWidth)) * 255;
            this.HSV.value = 255 - (int)value;
            return ColourHandler.HSVtoColour(HSV);
        }

        private void UpdateColour(Color colour)
        {
            this.SelectedColour = colour;
            ColourEventArgs args = new ColourEventArgs(colour);
            OnNewColour(args);
            this.Refresh();
        }
    }
}
