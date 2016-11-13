/*
Jonas Klausen, January/February 2016
Class representing a Sudoku, with methods for solving, storing and extrascting field
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku_solver {
    static class Sudoku {

        static sudokuField[] fields;
        static Queue<sudokuField> waitingForUpdate = new Queue<sudokuField>();

        public static void Create() {
            fields = new sudokuField[81];
            //Creates our sudoku from the textboxes found in Form1
            for(int i = 0; i<81; i++) {
                String txt = Form1.sudokuTextBoxes[i].Text;
                //If there's a valid sudoku-number in the box, we use that. Else, we leave it blank
                String pattern = "[1-9]";
                if (System.Text.RegularExpressions.Regex.IsMatch(txt, pattern)){
                    fields[i] = new sudokuField(Int32.Parse(txt)-1); //Remember to convert the number to "machine-friendly" numbers (0-8)
                } else {
                    fields[i] = new sudokuField();
                }

            }
        }

        public static void Solve() {
            //TODO: Make solve-ladder - and their functions, of course
            //Simple solve - first step
            //Written here for testing backbone - will be moved later
            //TODO: Move simple solve
            for(int i = 0; i<81; i++) {
                sudokuField field = fields[i];
                sudokuField[] surroundings = getSurroundings(field);

                //Update candidates of field, according to surrounding numbers
                foreach (sudokuField f in surroundings) {
                    int number = f.getNumber();
                    if(number > -1) {
                        removeCandidate(field, number);
                    }
                }
                //Better empty the queue now
                handleQueue();
            }

        }

        public static sudokuField[] getColumn(sudokuField field) {
            //Returns the column (vertical) that the given field is part of
            int index = Array.IndexOf(fields, field);
            sudokuField[] column = new sudokuField[9];
            int columnNo = index % 9;
            for (int i = 0; i < 9; i++) {
                column[i] = fields[columnNo + (i * 9)];
            }
            return column;
        }

        public static sudokuField[] getRow(sudokuField field) {
            //Returns the row (horizontal) that the given field is part of
            int index = Array.IndexOf(fields, field);
            sudokuField[] row = new sudokuField[9];
            int rowNo = index / 9;
            for(int i = 0; i<9; i++) {
                row[i] = fields[i + (rowNo * 9)];
            }
            return row;
        }

        public static sudokuField[] getSegment(sudokuField field) {
            //Returns the 3x3 grid the given field is part of
            sudokuField[] segment = new sudokuField[9];
            int index = Array.IndexOf(fields, field);
            int x = (index % 9)/3; //0,0 is segment in top-right corner
            int y = (index / 9)/3;
            for(int i = 0; i<9; i++) {
                int innerX = i % 3;
                int innerY = i / 3;
                int finalX = x * 3 + innerX;
                int finalY = y * 3 + innerY;
                segment[i] = fields[finalX + 9 * finalY];
            }
            return segment;
        }

        static sudokuField[] getSurroundings(sudokuField field) {
            //Returns an array of alle the fields in the same row, column and segment

            sudokuField[] row = getRow(field);
            sudokuField[] column = getColumn(field);
            sudokuField[] segment = getSegment(field);
            //Combine into one array - surroundingFields. This array will have duplicates, but that shouldn't really matter ...
            sudokuField[] surroundingFields = new sudokuField[27];
            Array.Copy(row, 0, surroundingFields, 0, row.Length);
            Array.Copy(column, 0, surroundingFields, row.Length, column.Length);
            Array.Copy(segment, 0, surroundingFields, row.Length + column.Length, segment.Length);

            return surroundingFields;
        }

        static void removeCandidate(sudokuField field, int number) {
            //Removes the candidate "number" from the given field, and enqueues the field in waitingForUpdate, if neccessary
            if (field.removeCandidate(number)) {
                //Now there's only one candidate left - so f needs updating too
                waitingForUpdate.Enqueue(field);
            }
        }

        static void handleQueue() {
            //Method checks the waitingForUpdate Queue, and updates all sudokuFields stored within
            //Also prints the new fields to Form1
            while(waitingForUpdate.Count > 0) {
                sudokuField fieldInQueue = waitingForUpdate.Dequeue();
                fieldInQueue.updateField();
                Form1.sudokuTextBoxes[Array.IndexOf(fields, fieldInQueue)].Text = (fieldInQueue.getNumber() + 1).ToString();

                //Updates the candidates of surrounding fields
                sudokuField[] surroundingFields = getSurroundings(fieldInQueue);

                foreach (sudokuField f in surroundingFields) {
                    removeCandidate(f, fieldInQueue.getNumber());
                }

            }
        }

    }
}
