namespace EDDY.IS.MatchingEngine.TestSplitSimulator
{
    partial class Form1
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
            this.button1 = new System.Windows.Forms.Button();
            this.txtResults = new System.Windows.Forms.TextBox();
            this.txtIterations = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSubject1 = new System.Windows.Forms.TextBox();
            this.txtSubject2 = new System.Windows.Forms.TextBox();
            this.txtSubject3 = new System.Windows.Forms.TextBox();
            this.txtSubject4 = new System.Windows.Forms.TextBox();
            this.txtSubject5 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(142, 23);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 9;
            this.button1.Text = "Run Test";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtResults
            // 
            this.txtResults.Location = new System.Drawing.Point(24, 121);
            this.txtResults.Multiline = true;
            this.txtResults.Name = "txtResults";
            this.txtResults.ReadOnly = true;
            this.txtResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtResults.Size = new System.Drawing.Size(190, 184);
            this.txtResults.TabIndex = 1;
            // 
            // txtIterations
            // 
            this.txtIterations.Location = new System.Drawing.Point(24, 25);
            this.txtIterations.Name = "txtIterations";
            this.txtIterations.Size = new System.Drawing.Size(100, 20);
            this.txtIterations.TabIndex = 1;
            this.txtIterations.Text = "5000";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Iteration Count:";
            // 
            // txtSubject1
            // 
            this.txtSubject1.Location = new System.Drawing.Point(24, 72);
            this.txtSubject1.Name = "txtSubject1";
            this.txtSubject1.Size = new System.Drawing.Size(30, 20);
            this.txtSubject1.TabIndex = 2;
            this.txtSubject1.Text = "100";
            this.txtSubject1.Leave += new System.EventHandler(this.txtSubject1_Leave);
            // 
            // txtSubject2
            // 
            this.txtSubject2.Location = new System.Drawing.Point(64, 72);
            this.txtSubject2.Name = "txtSubject2";
            this.txtSubject2.Size = new System.Drawing.Size(30, 20);
            this.txtSubject2.TabIndex = 3;
            this.txtSubject2.Leave += new System.EventHandler(this.txtSubject2_Leave);
            // 
            // txtSubject3
            // 
            this.txtSubject3.Location = new System.Drawing.Point(103, 72);
            this.txtSubject3.Name = "txtSubject3";
            this.txtSubject3.Size = new System.Drawing.Size(30, 20);
            this.txtSubject3.TabIndex = 4;
            this.txtSubject3.Leave += new System.EventHandler(this.txtSubject3_Leave);
            // 
            // txtSubject4
            // 
            this.txtSubject4.Location = new System.Drawing.Point(142, 72);
            this.txtSubject4.Name = "txtSubject4";
            this.txtSubject4.Size = new System.Drawing.Size(30, 20);
            this.txtSubject4.TabIndex = 5;
            this.txtSubject4.Leave += new System.EventHandler(this.txtSubject4_Leave);
            // 
            // txtSubject5
            // 
            this.txtSubject5.Location = new System.Drawing.Point(184, 72);
            this.txtSubject5.Name = "txtSubject5";
            this.txtSubject5.Size = new System.Drawing.Size(30, 20);
            this.txtSubject5.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(32, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 17);
            this.label2.TabIndex = 9;
            this.label2.Text = "1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(73, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 17);
            this.label3.TabIndex = 10;
            this.label3.Text = "2";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(107, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 17);
            this.label4.TabIndex = 11;
            this.label4.Text = "3";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(150, 52);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 17);
            this.label5.TabIndex = 12;
            this.label5.Text = "4";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(190, 52);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 17);
            this.label6.TabIndex = 13;
            this.label6.Text = "5";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(24, 102);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(39, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Output";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(226, 319);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtSubject5);
            this.Controls.Add(this.txtSubject4);
            this.Controls.Add(this.txtSubject3);
            this.Controls.Add(this.txtSubject2);
            this.Controls.Add(this.txtSubject1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtIterations);
            this.Controls.Add(this.txtResults);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Test Split Simulator";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtResults;
        private System.Windows.Forms.TextBox txtIterations;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSubject1;
        private System.Windows.Forms.TextBox txtSubject2;
        private System.Windows.Forms.TextBox txtSubject3;
        private System.Windows.Forms.TextBox txtSubject4;
        private System.Windows.Forms.TextBox txtSubject5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
    }
}

