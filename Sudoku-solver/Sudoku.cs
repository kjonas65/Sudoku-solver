/*
Jonas Klausen, January-April 2016
Class representing a Sudoku, with methods for solving, storing and extracting field
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku_solver {
    static class Sudoku {

        static sudokuField[] fields;
        static Queue<sudokuField> waitingForUpdate;
        static int solvedFields;
        static bool singleField; //Do we only want to solve one field?

        static int hintIndex = 0; //The location of last given hint (so we can fix the BG-color)
        
        delegate sudokuField[] delGetArrayFromSudokuField(sudokuField field);
        
        /*
         * VALIDATION OF SUDOKU
         */

        static bool sudokuIsInvalid() {
            //Marks the sudoku invalid if the same number is found more than once in a row/column/segment
            bool invalid = false;
            for (int i = 0; i < 9; i++) {
                sudokuField[] row = getRow(i);
                sudokuField[] column = getColumn(i);
                sudokuField[] segment = getSegment(i);

                if(hasDuplicates(row)) invalid = true;
                if(hasDuplicates(column)) invalid = true;
                if(hasDuplicates(segment)) invalid = true;
            }
            return invalid;
        }

        static bool hasDuplicates(sudokuField[] array) {
            //Checks if the same number occurs more than once in array
            int[] occurences = new int[9];
            for (int i = 0; i<9; i++) {
                if(array[i].getNumber() >= 0) {
                    occurences[array[i].getNumber()] += 1;
                }
            }
            for(int i = 0; i<9; i++) {
                if(occurences[i] > 1) { //More than one occurence of this number!
                    return true;
                }
            }
            //No duplicates found
            return false;
        }

        /*
         * INTERACTION WITH GUI
         */

        public static void create() {
            //Reads the textBoxes in MainWindow and translates them into data we can work with
            MainWindow.sudokuTextBoxes[hintIndex].BackColor = System.Drawing.Color.White;
            //re-initialize our variables
            waitingForUpdate = new Queue<sudokuField>();
            solvedFields = 0;
            singleField = false;
            fields = new sudokuField[81];

            //Creates our sudoku from the textboxes found in MainWindow
            for (int i = 0; i < 81; i++) {
                String txt = MainWindow.sudokuTextBoxes[i].Text;
                //If there's a valid sudoku-number in the box, we use that. Else, we leave it blank
                String pattern = "[1-9]";
                if (System.Text.RegularExpressions.Regex.IsMatch(txt, pattern)) {
                    fields[i] = new sudokuField(Int32.Parse(txt) - 1); //Remember to convert the number to "machine-friendly" numbers (0-8)
                    solvedFields += 1;
                } else {
                    fields[i] = new sudokuField();
                }

            }

            if(sudokuIsInvalid()){
                throw new InvalidOperationException("The entered sudoku isn't valid");
            }
        }

        public static void solve() {
            //Solves the given sudoku
            int startFields = solvedFields;

            loneCandidates();

            for (int i = 0; i < 500; i++) {
                solvingAlgorithms();
                
                //Clean up the queue
                handleQueue();
                if (solvedFields == 81) break;
            }

            if (solvedFields == 81) { //Sudoku is done
                System.Windows.Forms.MessageBox.Show("Solved!");
            }else if(solvedFields > startFields){ //We solved part of it
                System.Windows.Forms.MessageBox.Show(solvedFields + " out of 81 fields solved.");
            }else { //We solved nothing at all
                System.Windows.Forms.MessageBox.Show("Can't help you here, sorry");
            }

        }

        public static void hint() {
            //Solves a single field, thus giving the user a little help
            singleField = true;

            loneCandidates();

            for (int i = 0; i < 50; i++) { //Not the most efficient solution, but it should work
                solvingAlgorithms();  //Then we don't have to worry about edge-cases
            }

            if (waitingForUpdate.Count > 0) { //A field is ready for update!
                sudokuField tmpField = waitingForUpdate.Peek();
                handleQueue();

                hintIndex = Array.IndexOf(fields, tmpField); //Write down location where "hint" is given
                MainWindow.sudokuTextBoxes[hintIndex].BackColor = System.Drawing.Color.Beige;
            } else {
                System.Windows.Forms.MessageBox.Show("Can't help you here, sorry");
            }

        }

        /*
         * SOLVING LOGIC
         */

        static void solvingAlgorithms() {
            //Wrapper method for our algorithms. Could be converted into a list of delegates?
            canOnlyBePlacedInOneSpot();
            mustBeInThisRowOrColumn();
            pair();
        }

        static void pair() {
            //Wrapper for methods lookForPairsIn and lookForHiddenPairsIn
            for(int i = 0; i<9; i++) {
                sudokuField[] row = getRow(i);
                sudokuField[] column = getColumn(i);
                sudokuField[] segment = getSegment(i);

                lookForPairsIn(row);
                lookForPairsIn(column);
                lookForPairsIn(segment);

                lookForHiddenPairsIn(row);
                lookForHiddenPairsIn(column);
                lookForHiddenPairsIn(segment);
            }
        }

        static void lookForPairsIn(sudokuField[] array) {
            //Spots if two fields in array share the same two (and only two) candidates. If they do, these two numbers can't be placed anywhere else in arrat
            for(int index = 0; index<8; index++) {
                if(array[index].getNumberOfCandidates() == 2) { //First field has two candidates
                    sudokuField firstField = array[index];
                    int[] firstCandidates = firstField.getCandidates();

                    for (int index2 = index + 1; index2 < 9; index2++) {

                        if(array[index2].getNumberOfCandidates() == 2) {
                            sudokuField secondField = array[index2];
                            int[] secondCandidates = secondField.getCandidates();
                            
                            if(firstCandidates[0] == secondCandidates[0] && firstCandidates[1] == secondCandidates[1]) { //The candidates match?
                                removeCandidateFromArrayExcept(firstCandidates[0], array, firstField, secondField); //Remove the two candidates from the rest of array
                                removeCandidateFromArrayExcept(firstCandidates[1], array, firstField, secondField);
                            }

                        }
                    }
                }

            }
        }

        static void lookForHiddenPairsIn(sudokuField[] array) {
            //Two numbers only occur as candidates in the same two fields. No other number can be placed in these fields
            for (int number1 = 0; number1 < 8; number1++) {

                if(occurencesAsCandidate(number1, array) == 2) {
                    //Find locations
                    int firstOccurence = -1;
                    int secondOccurence = -1;
                    for (int i = 0; i < 9; i++) {
                        if (array[i].isCandidate(number1)) { //This is one of the occurences
                            if (firstOccurence < 0) {
                                firstOccurence = i;
                            } else {
                                secondOccurence = i;
                            }
                        }
                    }

                    for(int number2 = number1 + 1; number2 < 9; number2++) {
                        //Are these numbers candidates in the exact same fields?
                        sudokuField first = array[firstOccurence];
                        sudokuField second = array[secondOccurence];
                        if (occurencesAsCandidate(number2, array) == 2) {

                            bool pairFound = true;
                            for (int i = 0; i < 9; i++) {
                                if (array[i].isCandidate(number1) != array[i].isCandidate(number2)) {
                                    //They weren't candidates to the same fields - no pair here
                                    pairFound = false;
                                    break;
                                }

                            }

                            if (pairFound) {
                                removeAllCandidatesExcept(array[firstOccurence], number1, number2);
                                removeAllCandidatesExcept(array[secondOccurence], number1, number2);
                            }

                        }
                    }
                }

            }
        }
        
        static int occurencesAsCandidate(int number, sudokuField[] array) {
            //Counts the number of sudokuFields in array in which number is a candidate
            int occurences = 0;
            for (int i = 0; i < 9; i++) {
                if (array[i].isCandidate(number)) occurences += 1;
            }
            return occurences;
        }

        static void mustBeInThisRowOrColumn() {
            //If a number in a row/column can only be placed in a given segment, this number can't be placed anywhere else in that segment
            //Likewise, if a number can only be placed in a given row/column of a segment, this number can't be placed anywhere else in that row/column
            //Sort of a wrapper for knownSegmentOfArray()
            for (int i = 0; i < 9; i++) {
                sudokuField[] row = getRow(i);
                sudokuField[] column = getColumn(i);
                sudokuField[] segment = getSegment(i);

                //To test for columns (vertical) we need to mirror the segment, since knownSegmentOfArray groups index 0,1,2 , 3,4,5 and 6,7,8 together
                /*
                Original    Transformed
                0 1 2       0 3 6
                3 4 5       1 4 7
                6 7 8       2 5 8
                */
                sudokuField[] mirroredSegment = new sudokuField[9];
                for(int c = 0; c<8; c++) {
                    mirroredSegment[c] = segment[(3 * c) % 8];
                }
                mirroredSegment[8] = segment[8];


                knownSegmentOfArray(row, getSegment);
                knownSegmentOfArray(column, getSegment);
                knownSegmentOfArray(segment, getRow);
                knownSegmentOfArray(mirroredSegment, getColumn);
            }
        }

        static void knownSegmentOfArray(sudokuField[] array, delGetArrayFromSudokuField overlapFunction) {
            //This method determines if we know for sure which third of an array a given number is located in.
            //If the number must be placed in this third, then it can't be placed anywhere else in the overlapping segment/row
            bool[] numberIsPlaced = new bool[9];

            for (int c = 0; c < 9; c++) {
                int tmp = array[c].getNumber();
                if (tmp > -1) {
                    numberIsPlaced[tmp] = true;
                }
            }

            for (int number = 0; number < 9; number++) {
                if (!numberIsPlaced[number]) { //Number isn't placed yet
                    bool[] segments = new bool[3]; //Can number be placed in column?
                    for (int ii = 0; ii < 9; ii++) {
                        if (array[ii].isCandidate(number)) { //Number can be placed in this field
                            segments[ii / 3] = true; //Can be placed in this segment
                        }
                    }
                    int noOfSegments = 0; //The number of segments where this number can be placed

                    for (int ii = 0; ii < 3; ii++) {
                        if (segments[ii]) noOfSegments += 1;
                    }

                    int foundSegment = -1; //The found segment

                    sudokuField[] overlap = new sudokuField[0]; //Row and column initialized as empty
                                                                //This will hold the sudokuFields of found segment - if any

                    if (noOfSegments == 1) {
                        for (int ii = 0; ii < 3; ii++) {
                            if (segments[ii]) foundSegment = ii;
                        }
                        overlap = overlapFunction(array[3 * foundSegment]);
                    }
                    //Remove number as candidate from segment - except in the row/column (array)
                    removeCandidateFromArrayExcept(number, overlap, array);
                }
            }
        }


        

        static void loneCandidates() {
            //Simply checks on the candidates of all fields.
            //Should only be called once, as the very first solve-algo
            //After this, the candidates should be self-maintaining
            for (int i = 0; i < 81; i++) {
                sudokuField field = fields[i];
                sudokuField[] surroundings = getSurroundings(field);

                //Update candidates of this field, according to surrounding numbers
                foreach (sudokuField f in surroundings) {
                    int number = f.getNumber();
                    if (number > -1) {
                        removeCandidate(field, number);
                    }
                }

            }
        }

        static void canOnlyBePlacedInOneSpot() {
            //A number is only legal in one position
            for (int i = 0; i < 9; i++) {
                //i is the index of row/column/segment we are currently looking in
                sudokuField[] row = getRow(i);
                sudokuField[] column = getColumn(i);
                sudokuField[] segment = getSegment(i);
                for (int number = 0; number < 9; number++) {
                    //Check the three arrays for this number
                    onlyOnePositionInArray(row, number);
                    onlyOnePositionInArray(column, number);
                    onlyOnePositionInArray(segment, number);
                }

            }
        }

        static void onlyOnePositionInArray(sudokuField[] array, int number) {
            //Method used in canOnlyBePlacedInOneSpot()
            //Checks to see if number is only present once in the candidates of array
            int occurences = 0; //How many times do we find this number in the candidates?
            int placement = -1; //Where did we last see this as candidate?
                                //Check array
            for (int index = 0; index < 9; index++) {
                if (array[index].isCandidate(number)) {
                    occurences += 1;
                    placement = index;
                }
            }
            if (occurences == 1) { //The number is found exactly once, so it need to be placed here
                removeAllCandidatesExcept(array[placement], number);
            }

        }

        /*
         * NAVIGATION OF SUDOKU
         */

        static sudokuField[] getColumn(sudokuField field) {
            //Returns the column (vertical) that the given field is part of
            int index = Array.IndexOf(fields, field);
            sudokuField[] column = new sudokuField[9];
            int columnNo = index % 9;
            for (int i = 0; i < 9; i++) {
                column[i] = fields[columnNo + (i * 9)];
            }
            return column;
        }

        static sudokuField[] getColumn(int columnIndex) {
            //Get column from index instead
            return getColumn(fields[columnIndex]);
        }

        static sudokuField[] getRow(sudokuField field) {
            //Returns the row (horizontal) that the given field is part of
            int index = Array.IndexOf(fields, field);
            sudokuField[] row = new sudokuField[9];
            int rowNo = index / 9;
            for (int i = 0; i < 9; i++) {
                row[i] = fields[i + (rowNo * 9)];
            }
            return row;
        }

        static sudokuField[] getRow(int rowIndex) {
            //Get row from index instead
            return getRow(fields[9 * rowIndex]);
        }

        static sudokuField[] getSegment(sudokuField field) {
            //Returns the 3x3 grid the given field is part of
            sudokuField[] segment = new sudokuField[9];
            int index = Array.IndexOf(fields, field);
            int x = (index % 9) / 3; //0,0 is segment in top-left corner
            int y = (index / 9) / 3;
            for (int i = 0; i < 9; i++) {
                int innerX = i % 3;
                int innerY = i / 3;
                int finalX = x * 3 + innerX;
                int finalY = y * 3 + innerY;
                segment[i] = fields[finalX + 9 * finalY];
            }
            return segment;
        }

        static sudokuField[] getSegment(int segmentIndex) {
            //Get segment from a single index (0 to 8)
            int x = segmentIndex % 3; //x- and y-coordinate of desired segment
            int y = segmentIndex / 3;
            return getSegment(fields[(y * 9 + x) * 3]);
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

        /*
         * ALTERING SUDOKU-STATE
         */

        static void removeCandidate(sudokuField field, int number) {
            //Removes the candidate "number" from the given field, and enqueues the field in waitingForUpdate, if neccessary
            if (field.removeCandidate(number)) {
                //Now there's only one candidate left - so field needs updating too
                waitingForUpdate.Enqueue(field);
            }
        }

        static void removeAllCandidatesExcept(sudokuField field, params int[] numbers) {
            //Removes all candidates from field, except "numbers".
            for (int i = 0; i < 9; i++) {
                if (Array.IndexOf(numbers, i) >= 0) { //This number should be skipped
                    continue;
                } else {
                    removeCandidate(field, i);
                }
            }
        }

        static void removeCandidateFromArrayExcept(int number, sudokuField[] array, params sudokuField[] exceptions) {
            //Removes number as a candidate from all sudokuFields in array, except those sudokuFields which also appear in exceptions
            foreach (sudokuField field in array) {
                if (Array.IndexOf(exceptions, field) == -1) { //This sudokuField is not found in exceptions
                    removeCandidate(field, number);
                }
            }
        }

        static void handleQueue() {
            //Method checks the waitingForUpdate Queue, and updates all sudokuFields stored within
            //Also prints the new fields to MainWindow
            //This is the only method that should access field.updateField()!
            //If singleStep is set to true, only a single field will be fixed
            while (waitingForUpdate.Count > 0) {
                //Insert number in code and GUI
                sudokuField currentField = waitingForUpdate.Dequeue();
                currentField.updateField();

                if(sudokuIsInvalid()){
                    throw new InvalidOperationException("Something went wrong. Make sure you entered a valid sudoku");
                }

                MainWindow.sudokuTextBoxes[Array.IndexOf(fields, currentField)].Text = (currentField.getNumber() + 1).ToString();
                solvedFields += 1;

                //Updates the candidates of surrounding fields
                sudokuField[] surroundingFields = getSurroundings(currentField);

                foreach (sudokuField f in surroundingFields) {
                    removeCandidate(f, currentField.getNumber());
                }

                //Exit method if only one field is wanted
                if (singleField) {
                    return;
                }

            }
        }

    }
}
