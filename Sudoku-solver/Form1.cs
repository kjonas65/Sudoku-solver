/*
Jonas Klausen, January-April 2016
A program for solving a given Sudoku
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sudoku_solver {
    public partial class MainWindow : Form {
        public MainWindow() {
            InitializeComponent();
            setup();
        }

        const int margin = 12;
        const int sudokuTextBoxSize = 20;
        const int sudokuTextBoxSpacing = 2;
        static public TextBox[] sudokuTextBoxes;
        
        void setup() {
            //Setup the textboxes for showing the Sudoku
            sudokuTextBoxes = new TextBox[81];
            for(int i = 0; i<81; i++) {
                sudokuTextBoxes[i] = new TextBox();
                sudokuTextBoxes[i].Location = new Point((i % 9)* (sudokuTextBoxSize + sudokuTextBoxSpacing) + margin, (i / 9)* (sudokuTextBoxSize + sudokuTextBoxSpacing) + margin);
                sudokuTextBoxes[i].Size = new Size(sudokuTextBoxSize, sudokuTextBoxSize);
                sudokuTextBoxes[i].TabIndex = i;
                sudokuTextBoxes[i].MaxLength = 1;
                sudokuTextBoxes[i].TextAlign = HorizontalAlignment.Center;
                Controls.Add(sudokuTextBoxes[i]);
            }
        }
        
        private void buttonSolveSudoku_Click(object sender, EventArgs e) {
            try {
                Sudoku.create();
                Sudoku.solve();
            }catch(InvalidOperationException ex) {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

        }

        private void buttonClear_Click(object sender, EventArgs e) {
            for(int i = 0; i<81; i++) {
                sudokuTextBoxes[i].Text = "";
            }
        }

        private void buttonHint_Click(object sender, EventArgs e) {
            try {
                Sudoku.create();
                Sudoku.hint();
            }catch(InvalidOperationException ex) {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        private void buttonLoad_Click(object sender, EventArgs e) {
            //Lets the user choose a .txt-file, and reads it as a sudoku
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "txt files (*.txt)|*.txt"; //Only allow .txt-files

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                try {
                    System.IO.Stream fileStream = dialog.OpenFile();
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(fileStream)) {
                        for (int i = 0; i < 81; i++) { //Check for each of the 81 textBoxes
                            int tmp;
                            do {
                                tmp = reader.Read();
                            } while ((char)tmp == '\n' || (char)tmp == '\r'); //Skip linebreak and carriage return

                            if (tmp >= '1' && tmp <= '9') {
                                sudokuTextBoxes[i].Text = ((char)tmp).ToString();
                            } else {
                                sudokuTextBoxes[i].Text = "";
                            }

                        }

                    }
                    fileStream.Close();

                }catch(Exception ex) {
                    System.Windows.Forms.MessageBox.Show("Couldn't open file");
                    Console.WriteLine(ex);
                }
            }
        }

    }
}
