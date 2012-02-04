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
    public partial class ColourWheel : BaseColourControl
    {

        // COLOR_COUNT represents the number of distinct colors
        // used to create the circular gradient. Its value 
        // is somewhat arbitrary -- change this to 6, for 
        // example, to see what happens. 1536 (6 * 256) seems 
        // a good compromise -- it's enough to get a full 
        // range of colors, but it doesn't overwhelm the processor
        // attempting to generate the image. The color wheel
        // contains 6 sections, and each section displays 
        // 256 colors. Seems like a reasonable compromise.
        private const int COLOUR_COUNT = 3 * 256;

        private const double DEGREES_PER_RADIAN = 180.0 / Math.PI;

        private Graphics g;
	    private Point centerPoint;
	    private Point colorPoint;
        private Region colorRegion;
        private Bitmap colorImage;
        private Rectangle colorRectangle;
        private int Radius;
        private bool loaded = false;

        private int offset = 0;

        private Color colourBeforeClick;
        public Color SelectedColour = Color.White;

        public ColourWheel()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            //Utils.SetDoubleBuffered(this);
        }

        private void ColourWheel_Load(object sender, EventArgs e)
        {
            this.loaded = true;
            this.UpdateBitmap();
        }

        private void ColourWheel_Resize(object sender, EventArgs e)
        {
            if (this.loaded)
            {
                this.UpdateBitmap();
                this.Refresh();
            }
        }

        public void Rotate(int angle)
        {
            this.offset += angle;
            this.UpdateBitmap();
            this.Refresh();
        }

        private void UpdateBitmap()
        {
            // Calc the radius
            this.Radius = Math.Min(this.Width, this.Height) / 2;
            using (GraphicsPath path = new GraphicsPath())
            {
                // Store away locations for later use. 
                this.colorRectangle = new Rectangle(this.Left, this.Top, this.Radius*2, this.Radius*2);

                // Calculate the center of the circle.
                // Start with the location, then offset
                // the point by the radius.
                // Use the smaller of the width and height of
                // the colorRectangle value.
                this.centerPoint = colorRectangle.Location;
                this.centerPoint.Offset(this.Radius, this.Radius);

                // Start the pointer in the center.
                this.colorPoint = this.centerPoint;

                // Create a region corresponding to the color circle.
                // Code uses this later to determine if a specified
                // point is within the region, using the IsVisible 
                // method.
                path.AddEllipse(colorRectangle);
                colorRegion = new Region(path);

                // Create the bitmap that contains the circular gradient.
                this.CreateGradient();
            }
        }

        private void UpdateDisplay()
        {
            using (Matrix m = new Matrix())
            {
                m.RotateAt((float)this.offset, this.centerPoint);
                g.Transform = m;
                g.DrawImage(colorImage, colorRectangle);
                g.ResetTransform();
            }

            // Given a point, draw the color selector. 
            // The constant SIZE represents half
            // the width -- the square will be twice
            // this value in width and height.
            if (this.ShowMarker)
            {
                using (Pen markerPen = new Pen(Color.Black))
                {
                    markerPen.Width = (float)Math.Ceiling((double)this.MarkerWidth / 2d);
                    g.DrawRectangle(markerPen, this.colorPoint.X - this.MarkerWidth, this.colorPoint.Y - this.MarkerWidth, this.MarkerWidth * 2, this.MarkerWidth * 2);
                }
            }
        }

        private void CreateGradient() 
	    {
		    // Create a new PathGradientBrush, supplying
		    // an array of points created by calling
		    // the GetPoints method.
		    using (PathGradientBrush pgb = 
			    new PathGradientBrush(GetPoints(this.Radius, new Point(this.Radius, this.Radius))))
		    {
			    // Set the various properties. Note the SurroundColors
			    // property, which contains an array of points, 
			    // in a one-to-one relationship with the points
			    // that created the gradient.
			    pgb.CenterColor = Color.White;
			    pgb.CenterPoint = new PointF(this.Radius, this.Radius);
			    pgb.SurroundColors = GetColors();

			    // Create a new bitmap containing
			    // the color wheel gradient, so the 
			    // code only needs to do all this 
			    // work once. Later code uses the bitmap
			    // rather than recreating the gradient.
			    colorImage = new Bitmap(
				    colorRectangle.Width, colorRectangle.Height, 
				    PixelFormat.Format32bppArgb);
	
			    using (Graphics newGraphics = Graphics.FromImage(colorImage))
                {
				    newGraphics.FillEllipse(pgb, 0, 0, colorRectangle.Width, colorRectangle.Height);
			    }
		    } 
	    }

        private void ColourWheel_Paint(object sender, PaintEventArgs e)
        {
            this.g = e.Graphics;
            this.UpdateDisplay();
        }

        /////////////////////////////
        //
        // MOUSE EVENTS
        //
        /////////////////////////////

        private void ColourWheel_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.mouseInRegion)
            {
                this.mouseDown = true;
                this.colourBeforeClick = this.SelectedColour;
                this.UpdateColour(this.ColourFromMousePoint(e.Location));
            }
        }

        private void ColourWheel_MouseMove(object sender, MouseEventArgs e)
        {
            this.mouseInRegion = this.colorRegion.IsVisible(e.Location);
            if (this.mouseDown)
            {
                if (this.mouseInRegion)
                {
                    this.UpdateColour(this.ColourFromMousePoint(e.Location));
                }
                else
                {
                    this.ResetColorAfterDragout();
                }
            }
        }

        private void ColourWheel_MouseUp(object sender, MouseEventArgs e)
        {
            this.mouseDown = false;
        }

        private void ColourWheel_MouseLeave(object sender, EventArgs e)
        {
            this.mouseInRegion = false;
            if (this.mouseDown)
            {
                this.ResetColorAfterDragout();
            }
        }

        private void ResetColorAfterDragout()
        {
            this.UpdateColour(this.colourBeforeClick);
        }

        private void UpdateColour(Color colour)
        {
            this.SelectedColour = colour;
            ColourEventArgs args = new ColourEventArgs(colour);
            OnNewColour(args);
            this.Refresh();
        }

        private Color ColourFromMousePoint(Point mousePoint)
        {
            this.colorPoint = mousePoint;
            Point delta = new Point(mousePoint.X - centerPoint.X, mousePoint.Y - centerPoint.Y);
            int degrees = CalcDegrees(delta);
            int adjustedDegrees = degrees + this.offset;
            double distance = Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y) / this.Radius;
            this.HSV.Hue = (int)(adjustedDegrees * 255 / 360);
            this.HSV.Saturation = (int)(distance * 255);
            this.HSV.value = 255;
            return ColourHandler.HSVtoColour(this.HSV); 
        }

        /////////////////////////////
        //
        // STATIC FUNCTIONS
        //
        /////////////////////////////
        
        static private Color[] GetColors()
        {
            // Create an array of COLOR_COUNT
            // colors, looping through all the 
            // hues between 0 and 255, broken
            // into COLOR_COUNT intervals. HSV is
            // particularly well-suited for this, 
            // because the only value that changes
            // as you create colors is the Hue.
            Color[] Colors = new Color[COLOUR_COUNT];

            for (int i = 0; i <= COLOUR_COUNT - 1; i++)
                Colors[i] = ColourHandler.HSVtoColour((int)((double)(i * 255) / COLOUR_COUNT), 255, 255);
            return Colors;
        }

        static private Point[] GetPoints(double radius, Point centerPoint)
        {
            // Generate the array of points that describe
            // the locations of the COLOR_COUNT colors to be 
            // displayed on the color wheel.
            Point[] Points = new Point[COLOUR_COUNT];

            for (int i = 0; i <= COLOUR_COUNT - 1; i++)
                Points[i] = GetPoint((double)(i * 360) / COLOUR_COUNT, radius, centerPoint);
            return Points;
        }

        static private Point GetPoint(double degrees, double radius, Point centerPoint)
        {
            // Given the center of a circle and its radius, along
            // with the angle corresponding to the point, find the coordinates. 
            // In other words, conver  t from polar to rectangular coordinates.
            double radians = degrees / DEGREES_PER_RADIAN;

            return new Point((int)(centerPoint.X + Math.Floor(radius * Math.Cos(radians))),
                (int)(centerPoint.Y - Math.Floor(radius * Math.Sin(radians))));
        }

        static private int CalcDegrees(Point pt)
        {
            int degrees;

            if (pt.X == 0)
            {
                // The point is on the y-axis. Determine whether 
                // it's above or below the x-axis, and return the 
                // corresponding angle. Note that the orientation of the
                // y-coordinate is backwards. That is, A positive Y value 
                // indicates a point BELOW the x-axis.
                if (pt.Y > 0)
                {
                    degrees = 270;
                }
                else
                {
                    degrees = 90;
                }
            }
            else
            {
                // This value needs to be multiplied
                // by -1 because the y-coordinate
                // is opposite from the normal direction here.
                // That is, a y-coordinate that's "higher" on 
                // the form has a lower y-value, in this coordinate
                // system. So everything's off by a factor of -1 when
                // performing the ratio calculations.
                degrees = (int)(-Math.Atan((double)pt.Y / pt.X) * DEGREES_PER_RADIAN);

                // If the x-coordinate of the selected point
                // is to the left of the center of the circle, you 
                // need to add 180 degrees to the angle. ArcTan only
                // gives you a value on the right-hand side of the circle.
                if (pt.X < 0)
                {
                    degrees += 180;
                }

                // Ensure that the return value is 
                // between 0 and 360.
                degrees = (degrees + 360) % 360;
            }
            return degrees;
        }
    }
}
