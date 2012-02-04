HSV Colour Picker Components for C#
===================================

Using this [MSDN article](http://msdn.microsoft.com/en-us/magazine/cc164113.aspx) (by Ken Getz) as a starting point, these components allow for a [HSV](http://en.wikipedia.org/wiki/HSL_and_HSV) (Hue, Saturation, Value) colour picker to be built. The colour wheel sets the Hue and Saturation, the value slider sets the Value and the Swatch can be used to display the product.

Components
----------
  1. **ColourWheel** - Draws a colour wheel. Clicking raises a "NewColour" event. Wheel can be rotated using "Rotate(int angle)".
  2. **ValueSlider** - Takes a colour and generates a gradient from values of 0 (black) to 255 (full colour). Clicking raises a "NewColour" event.
  3. **Swatch** - Paints a colour. Allows the RGB colour code to be copied to clipboard in decimal or hex.

 Support files
 -------------
  1. **ColourHandler.cs** - Taken almost verbatim from Ken's article, this file provides all sorts of useful classes and structures related to HSV and RGB colour. Also includes an enum for colour string formatting and the function which uses this.
  2. **ColourEventArgs.cs** - Event argument class that contains a colour property. Used by the NewColour event.
  3. **BaseColourControl.cs** (+ associated files) - Base class inherited by the Colour Wheel and the Value Slider.

  License
  -------
  This was written by [Tom Wright](http://tdwright.co.uk) (tom@tdwright.co.uk) at the [University of Sussex](http://sussex.ac.uk). Whilst I intend to provide these components as open source, I don't have the patience to make an informed decision about licensing. Let me know if you want to use them for something other than academic work.