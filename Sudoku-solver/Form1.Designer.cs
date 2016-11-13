namespace Sudoku_solver {
    partial class MainWindow {
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
            this.buttonSolveSudoku = new System.Windows.Forms.Button();
            this.buttonClear = new System.Windows.Forms.Button();
            this.buttonHint = new System.Windows.Forms.Button();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonSolveSudoku
            // 
            this.buttonSolveSudoku.Location = new System.Drawing.Point(239, 12);
            this.buttonSolveSudoku.Name = "buttonSolveSudoku";
            this.buttonSolveSudoku.Size = new System.Drawing.Size(94, 23);
            this.buttonSolveSudoku.TabIndex = 100;
            this.buttonSolveSudoku.Text = "Solve Sudoku";
            this.buttonSolveSudoku.UseVisualStyleBackColor = true;
            this.buttonSolveSudoku.Click += new System.EventHandler(this.buttonSolveSudoku_Click);
            // 
            // buttonClear
            // 
            this.buttonClear.Location = new System.Drawing.Point(239, 70);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(94, 23);
            this.buttonClear.TabIndex = 102;
            this.buttonClear.Text = "Clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // buttonHint
            // 
            this.buttonHint.Location = new System.Drawing.Point(239, 41);
            this.buttonHint.Name = "buttonHint";
            this.buttonHint.Size = new System.Drawing.Size(94, 23);
            this.buttonHint.TabIndex = 101;
            this.buttonHint.Text = "Get a hint";
            this.buttonHint.UseVisualStyleBackColor = true;
            this.buttonHint.Click += new System.EventHandler(this.buttonHint_Click);
            // 
            // buttonLoad
            // 
            this.buttonLoad.Location = new System.Drawing.Point(12, 226);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(94, 23);
            this.buttonLoad.TabIndex = 99;
            this.buttonLoad.Text = "Load from file";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(345, 261);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.buttonHint);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.buttonSolveSudoku);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainWindow";
            this.Text = "Sudoku-solver";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonSolveSudoku;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.Button buttonHint;
        private System.Windows.Forms.Button buttonLoad;
    }
}

