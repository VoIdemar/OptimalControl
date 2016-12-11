/*
 * Created by SharpDevelop.
 * User: Voldemar
 * Date: 07.02.2015
 * Time: 18:10
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace OptimalControl
{
    partial class MainForm
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        
        /// <summary>
        /// Disposes resources used by the form.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        
        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent()
        {
            this.plotTabControl = new System.Windows.Forms.TabControl();
            this.SuspendLayout();
            // 
            // plotTabControl
            // 
            this.plotTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotTabControl.Location = new System.Drawing.Point(0, 0);
            this.plotTabControl.Name = "plotTabControl";
            this.plotTabControl.SelectedIndex = 0;
            this.plotTabControl.Size = new System.Drawing.Size(739, 388);
            this.plotTabControl.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(739, 388);
            this.Controls.Add(this.plotTabControl);
            this.Name = "MainForm";
            this.Text = "OptimalControl";
            this.ResumeLayout(false);
        }
        private System.Windows.Forms.TabControl plotTabControl;
    }
}
