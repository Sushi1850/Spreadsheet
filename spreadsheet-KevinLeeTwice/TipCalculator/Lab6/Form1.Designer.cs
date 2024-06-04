
namespace Lab6
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
            this.ComputeTipButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.EnterValue = new System.Windows.Forms.TextBox();
            this.TotalValue = new System.Windows.Forms.TextBox();
            this.TipLabel = new System.Windows.Forms.Label();
            this.TipTextBox = new System.Windows.Forms.TextBox();
            this.TotalAmountLabel = new System.Windows.Forms.Label();
            this.TotalAmountTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // ComputeTipButton
            // 
            this.ComputeTipButton.Enabled = false;
            this.ComputeTipButton.Location = new System.Drawing.Point(149, 186);
            this.ComputeTipButton.Name = "ComputeTipButton";
            this.ComputeTipButton.Size = new System.Drawing.Size(105, 21);
            this.ComputeTipButton.TabIndex = 0;
            this.ComputeTipButton.Text = "Calculate Tip";
            this.ComputeTipButton.UseVisualStyleBackColor = true;
            this.ComputeTipButton.TextChanged += new System.EventHandler(this.CalculateTipButton);
            this.ComputeTipButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(149, 129);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Enter Total Bill";
            this.label1.TextChanged += new System.EventHandler(this.EnterTotalBill);
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // EnterValue
            // 
            this.EnterValue.Location = new System.Drawing.Point(314, 121);
            this.EnterValue.Name = "EnterValue";
            this.EnterValue.Size = new System.Drawing.Size(100, 20);
            this.EnterValue.TabIndex = 2;
            this.EnterValue.TextChanged += new System.EventHandler(this.EnterValue_TextChanged);
            // 
            // TotalValue
            // 
            this.TotalValue.Location = new System.Drawing.Point(314, 186);
            this.TotalValue.Name = "TotalValue";
            this.TotalValue.Size = new System.Drawing.Size(100, 20);
            this.TotalValue.TabIndex = 3;
            this.TotalValue.TextChanged += new System.EventHandler(this.TotalValue_TextChanged);
            // 
            // TipLabel
            // 
            this.TipLabel.AutoSize = true;
            this.TipLabel.Location = new System.Drawing.Point(168, 157);
            this.TipLabel.Name = "TipLabel";
            this.TipLabel.Size = new System.Drawing.Size(33, 13);
            this.TipLabel.TabIndex = 4;
            this.TipLabel.Text = "Tip %";
            // 
            // TipTextBox
            // 
            this.TipTextBox.Location = new System.Drawing.Point(314, 149);
            this.TipTextBox.Name = "TipTextBox";
            this.TipTextBox.Size = new System.Drawing.Size(100, 20);
            this.TipTextBox.TabIndex = 5;
            // 
            // TotalAmountLabel
            // 
            this.TotalAmountLabel.AutoSize = true;
            this.TotalAmountLabel.Location = new System.Drawing.Point(152, 279);
            this.TotalAmountLabel.Name = "TotalAmountLabel";
            this.TotalAmountLabel.Size = new System.Drawing.Size(70, 13);
            this.TotalAmountLabel.TabIndex = 6;
            this.TotalAmountLabel.Text = "Total Amount";
            // 
            // TotalAmountTextBox
            // 
            this.TotalAmountTextBox.Location = new System.Drawing.Point(314, 271);
            this.TotalAmountTextBox.Name = "TotalAmountTextBox";
            this.TotalAmountTextBox.Size = new System.Drawing.Size(100, 20);
            this.TotalAmountTextBox.TabIndex = 7;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.TotalAmountTextBox);
            this.Controls.Add(this.TotalAmountLabel);
            this.Controls.Add(this.TipTextBox);
            this.Controls.Add(this.TipLabel);
            this.Controls.Add(this.TotalValue);
            this.Controls.Add(this.EnterValue);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ComputeTipButton);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ComputeTipButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox EnterValue;
        private System.Windows.Forms.TextBox TotalValue;
        private System.Windows.Forms.Label TipLabel;
        private System.Windows.Forms.TextBox TipTextBox;
        private System.Windows.Forms.Label TotalAmountLabel;
        private System.Windows.Forms.TextBox TotalAmountTextBox;
    }
}

