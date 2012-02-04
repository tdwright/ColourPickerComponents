namespace ColourPickerComponents
{
    partial class ColourWheel
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            // Dispose of graphic resources
            if (colorImage != null)
                colorImage.Dispose();
            if (colorRegion != null)
                colorRegion.Dispose();
            if (g != null)
                g.Dispose();

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ColourWheel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "ColourWheel";
            this.Load += new System.EventHandler(this.ColourWheel_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ColourWheel_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ColourWheel_MouseDown);
            this.MouseLeave += new System.EventHandler(this.ColourWheel_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ColourWheel_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ColourWheel_MouseUp);
            this.Resize += new System.EventHandler(this.ColourWheel_Resize);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
