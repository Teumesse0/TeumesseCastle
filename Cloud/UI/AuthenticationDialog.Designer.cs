//
// Part of PingCastle 3.0.0.0 by Vincent Le Toux / Ping Castle SAS (https://www.pingcastle.com).
// Licensed under the Non-Profit OSL 3.0. See LICENSE.md in the project root for full license information.
//
// Modifications Copyright (c) 2026 Teumesse — TeumesseCastle project.
//
namespace PingCastle.Cloud.UI
{
    partial class AuthenticationDialog
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
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AuthenticationDialog));
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.timerForAutoSignAttempt = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // webBrowser
            // 
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.Location = new System.Drawing.Point(0, 0);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Size = new System.Drawing.Size(543, 887);
            this.webBrowser.TabIndex = 0;
            this.webBrowser.Url = new System.Uri("http://w", System.UriKind.Absolute);
            // 
            // timerForAutoSignAttempt
            // 
            this.timerForAutoSignAttempt.Tick += new System.EventHandler(this.timerForAutoSignAttempt_Tick);
            // 
            // AuthenticationDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(543, 887);
            this.Controls.Add(this.webBrowser);
            this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetEntryAssembly().Location);
            this.Name = "AuthenticationDialog";
            this.Text = "PingCastle Cloud";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser;
        private System.Windows.Forms.Timer timerForAutoSignAttempt;
    }
}