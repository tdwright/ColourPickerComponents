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
    public partial class Swatch : UserControl
    {

        private Rectangle paintArea;

        private ContextMenu rclick;

        private Color colour = Color.White;

        private bool contextmenuenabled = false;
        [DefaultValue(false), Category("Behaviour"), Description("Enable context menu - for copying current values")]
        public bool ContextMenuEnabled
        {
            set
            {
                if (value != contextmenuenabled)
                {
                    if (value)
                    {
                        this.ContextMenu = this.rclick;
                    }
                    else
                    {
                        this.ContextMenu = null;
                    }
                    this.contextmenuenabled = value;
                }
            }
            get
            {
                return this.contextmenuenabled;
            }
        }

        public Swatch()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        public void SetColour(Color Colour)
        {
            this.colour = Colour;
            this.Refresh();
        }

        private void Swatch_Load(object sender, EventArgs e)
        {
            this.UpdatePaintArea();
            SetupContextMenu();
        }

        private void SetupContextMenu()
        {
            this.rclick = new System.Windows.Forms.ContextMenu();
            Menu.MenuItemCollection items = new Menu.MenuItemCollection(rclick);
            this.rclick.MenuItems.Add(new MenuItem("Copy color to clipboard"));
            this.rclick.MenuItems[0].Enabled = false;
            this.rclick.MenuItems.Add(new MenuItem("...as R,G,B", this.CopyRGB));
            this.rclick.MenuItems.Add(new MenuItem("...as #HEX", this.CopyHex));
        }

        private void CopyToClipboard(ColourHandler.ColourRepresentations format)
        {
            Clipboard.SetText(ColourHandler.FormatColourString(this.colour, format));
        }

        private void CopyRGB(object sender, EventArgs e)
        {
            this.CopyToClipboard(ColourHandler.ColourRepresentations.RGB);
        }

        private void CopyHex(object sender, EventArgs e)
        {
            this.CopyToClipboard(ColourHandler.ColourRepresentations.Hex);
        }

        private void Swatch_Resize(object sender, EventArgs e)
        {
            this.UpdatePaintArea();
        }

        private void UpdatePaintArea()
        {
            this.paintArea = new Rectangle(0, 0, this.Width, this.Height);
        }

        private void Swatch_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(this.colour), this.paintArea);
        }
    }
}
