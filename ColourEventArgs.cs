using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ColourPickerComponents
{
    public class ColourEventArgs : EventArgs
    {
        private Color colour;

        public ColourEventArgs(Color colour)
        {
            this.colour = colour;
        }

        public Color Colour
        {
            get
            {
                return this.colour;
            }
        }
    }
}
