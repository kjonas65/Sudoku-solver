/*
Jonas Klausen, January/February 2016
Class representing a field on a sudoku
A sudoku contains the numbers 1-9, but this class is operating on the numbers 0-8 instead.
Relations to a user-oriented (graphical) Sudoku should be adjusted with +- 1 before and after accessing this class
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku_solver {
    class sudokuField {
        static bool[] allFalse = new bool[9] { false, false, false, false, false, false, false, false, false }; //9  * false, used for sudokuFields where we know the number

        bool[] candidates; //would this number be valid here?
        int noOfCandidates; //How many candidates are leagl in this field? If 1, field should be updated
        int number; //The number in this field

        public sudokuField(int i) { //We know the number in this field, so no candidates
            number = i;
            candidates = allFalse; //There's already a number here!
            noOfCandidates = -1;
        }

        public sudokuField() {//Blank space
            number = -1;
            candidates = new bool[9] { true, true, true, true, true, true, true, true, true}; //Until proven otherwise, everything works here
            noOfCandidates = 9;
        }

        public bool removeCandidate(int i) {
            //Removes the given candidate from this field. Returns true, if there's only one candidate left now
            //Will only return true once for each sudokuField, since true is triggered, when second-last candidate is /removed/
            bool madeChange = candidates[i];
            candidates[i] = false;
            if (madeChange) {
                noOfCandidates -= 1;
                if(noOfCandidates == 1) {
                    return true;
                }
            }
            return false;
        }

        public int getNumber() {
            //Returns the number written in this field - returns -1 if no number is set yet
            return number;
        }

        public void updateField() {
            //updateField is called, when there is only one candidate left
            for(int i = 0; i<9; i++) {
                if (candidates[i]) {
                    number = i;
                    candidates = allFalse;
                    noOfCandidates = -1;
                }
            }
        }
    }
}
