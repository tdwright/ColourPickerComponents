using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ColourPickerComponents
{
    public abstract partial class BaseColourControl : UserControl
    {


        // Event stuff
        public event ColourEventHandler NewColour;
        public delegate void ColourEventHandler(object sender, ColourEventArgs colourArgs);
        public void OnNewColour(ColourEventArgs colourArgs)
        {
            // Note the copy to a local variable, so that we don't risk a
            // NullReferenceException if another thread unsubscribes between the test and
            // the invocation.
            ColourEventHandler handler = NewColour;
            if (handler != null)
            {
                handler(this, colourArgs);
            }
        }

        // Mouse bools
        protected bool mouseInRegion = false;
        protected bool mouseDown = false;
        public bool HasBeenClicked = false;

        // Colour handler
        protected ColourHandler.HSV HSV;


        private bool showmarker = true;
        [DefaultValue(true), Category("Appearance"), Description("Show a marker at the current selection.")]
        public bool ShowMarker
        {
            get
            {
                return this.showmarker;
            }
            set
            {
                this.showmarker = value;
            }
        }

        private const int MAXMARKERWIDTH = 5;
        private int markerwidth = 2; // will be double 
        [DefaultValue(2), Category("Appearance"), Description("Size of position marker")]
        public int MarkerWidth
        {
            get
            {
                if (this.showmarker)
                {
                    return this.markerwidth;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (value == 0)
                {
                    this.markerwidth = 0;
                    this.showmarker = false;
                }
                else if (value > 0 && value <= MAXMARKERWIDTH)
                {
                    this.markerwidth = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Invalid value. Value must be between 0 and " + MAXMARKERWIDTH.ToString());
                }

            }
        }

        public float MarkerPenWidth
        {
            get
            {
                return (float)Math.Ceiling((double)this.MarkerWidth / 2d);
            }
        }

        public BaseColourControl()
        {
            InitializeComponent();
        }
    }
}
