namespace ModulesGraphDesktopApp
{
  partial class ModulesGraphForm
  {
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.webBrowser = new System.Windows.Forms.WebBrowser();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitContainer
      // 
      this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer.Location = new System.Drawing.Point(0, 0);
      this.splitContainer.Name = "splitContainer";
      this.splitContainer.Size = new System.Drawing.Size(770, 450);
      this.splitContainer.SplitterDistance = 555;
      this.splitContainer.TabIndex = 0;
      // 
      // webBrowser
      // 
      this.webBrowser.Location = new System.Drawing.Point(1064, 24);
      this.webBrowser.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
      this.webBrowser.MinimumSize = new System.Drawing.Size(27, 27);
      this.webBrowser.Name = "webBrowser";
      this.webBrowser.Size = new System.Drawing.Size(405, 506);
      this.webBrowser.TabIndex = 27;
      this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer.Panel2.Controls.Add(this.webBrowser);
      // 
      // ModulesGraphForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.White;
      this.ClientSize = new System.Drawing.Size(770, 450);
      this.Controls.Add(this.splitContainer);
      this.Name = "ModulesGraphForm";
      this.Text = "Граф зависимостей между модулями";
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private SplitContainer splitContainer;

    private System.Windows.Forms.WebBrowser webBrowser;
  }
}