// Copyright 2009 Strom
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace CowKiller {
    partial class Form1 {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.textLog = new System.Windows.Forms.TextBox();
            this.lblExp = new System.Windows.Forms.Label();
            this.timerLog = new System.Windows.Forms.Timer(this.components);
            this.lblTime = new System.Windows.Forms.Label();
            this.comboScenario = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtExpLimit = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblRate = new System.Windows.Forms.Label();
            this.lblETA = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(134, 65);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(115, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Stop";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 65);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(116, 23);
            this.button2.TabIndex = 0;
            this.button2.Text = "Start";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textLog
            // 
            this.textLog.Location = new System.Drawing.Point(12, 170);
            this.textLog.Multiline = true;
            this.textLog.Name = "textLog";
            this.textLog.ReadOnly = true;
            this.textLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textLog.Size = new System.Drawing.Size(237, 455);
            this.textLog.TabIndex = 2;
            // 
            // lblExp
            // 
            this.lblExp.AutoSize = true;
            this.lblExp.Location = new System.Drawing.Point(12, 95);
            this.lblExp.Name = "lblExp";
            this.lblExp.Size = new System.Drawing.Size(108, 13);
            this.lblExp.TabIndex = 4;
            this.lblExp.Text = "Experience earned: 0";
            // 
            // timerLog
            // 
            this.timerLog.Interval = 200;
            this.timerLog.Tick += new System.EventHandler(this.timerLog_Tick);
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Location = new System.Drawing.Point(12, 113);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(93, 13);
            this.lblTime.TabIndex = 5;
            this.lblTime.Text = "Elapsed: 00:00:00";
            // 
            // comboScenario
            // 
            this.comboScenario.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboScenario.FormattingEnabled = true;
            this.comboScenario.Items.AddRange(new object[] {
            "Cow",
            "Outdoorsman - Navarro 2238",
            "Outdoorsman - Klamath",
            "Outdoorsman - Replication 3",
            "Outdoorsman - Mariposa"});
            this.comboScenario.Location = new System.Drawing.Point(98, 12);
            this.comboScenario.Name = "comboScenario";
            this.comboScenario.Size = new System.Drawing.Size(151, 21);
            this.comboScenario.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Scenario:";
            // 
            // txtExpLimit
            // 
            this.txtExpLimit.Location = new System.Drawing.Point(98, 39);
            this.txtExpLimit.Name = "txtExpLimit";
            this.txtExpLimit.Size = new System.Drawing.Size(151, 20);
            this.txtExpLimit.TabIndex = 8;
            this.txtExpLimit.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Experience limit:";
            // 
            // lblRate
            // 
            this.lblRate.AutoSize = true;
            this.lblRate.Location = new System.Drawing.Point(12, 131);
            this.lblRate.Name = "lblRate";
            this.lblRate.Size = new System.Drawing.Size(73, 13);
            this.lblRate.TabIndex = 10;
            this.lblRate.Text = "Rate: 0 exp/h";
            // 
            // lblETA
            // 
            this.lblETA.AutoSize = true;
            this.lblETA.Location = new System.Drawing.Point(12, 149);
            this.lblETA.Name = "lblETA";
            this.lblETA.Size = new System.Drawing.Size(76, 13);
            this.lblETA.TabIndex = 11;
            this.lblETA.Text = "ETA: 00:00:00";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(261, 637);
            this.Controls.Add(this.lblETA);
            this.Controls.Add(this.lblRate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtExpLimit);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboScenario);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.lblExp);
            this.Controls.Add(this.textLog);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "CowKiller 2.4 by Strom";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textLog;
        private System.Windows.Forms.Label lblExp;
        private System.Windows.Forms.Timer timerLog;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.ComboBox comboScenario;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtExpLimit;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblRate;
        private System.Windows.Forms.Label lblETA;
    }
}

