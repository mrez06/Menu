namespace WinFormsApp1
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components;

        protected override void Dispose(bool disposing)
        {
            if (disposing) components?.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(933, 462);
            Name = "Form1";
            Text = "Cafe system - MENU";
            Load += Form1_Load;
            ResumeLayout(false);
        }
    }
}
