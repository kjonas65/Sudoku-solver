/*
Jonas Klausen, January-April 2016
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

        /*
         * CONSTRUCTORS
         */

        public sudokuField(int i) { //We know the number in this field, so no candidates
            number = i;
            candidates = allFalse; //There's already a number here!
            noOfCandidates = 0;
        }

        public sudokuField() {//Blank space
            number = -1;
            candidates = new bool[9] { true, true, true, true, true, true, true, true, true}; //Until proven otherwise, everything works here
            noOfCandidates = 9;
        }

        /*
         * INTERACTION
         */

        public bool removeCandidate(int i) {
            //Removes the given candidate from this field. Returns true, if there's only one candidate left now
            //Will only return true once for each sudokuField, since true is triggered, when second-last candidate is /removed/
            //Should NEVER be called outside of Sudoku.removeCandidate
            bool madeChange = candidates[i];
            candidates[i] = false;
            if (madeChange) {
                noOfCandidates -= 1;
                if(noOfCandidates == 1) {
                    return true;
                }else if(noOfCandidates == 0) {
                    throw new InvalidOperationException("Something went wrong. Make sure you entered a valid sudoku");
                }
            }
            return false;
        }

        public void updateField() {
            //updateField is called, when there is only one candidate left
            //Should only be called from handleQueue()!
            for(int i = 0; i<9; i++) {
                if (candidates[i]) {
                    number = i;
                    candidates = allFalse;
                    noOfCandidates = -1;
                    return;
                }
            }
        }

        /*
         * GETTING INFORMATION
         */

        public int getNumber() {
            //Returns the number written in this field - returns -1 if no number is set yet
            return number;
        }

        public int getNumberOfCandidates() {
            //Returns the number of candidates for this field - returns -1 if number is set
            return noOfCandidates;
        }

        public int[] getCandidates() {
            //Returns an array consisting of the candidates for this field - returns an empty int[] if number is set
            if(noOfCandidates == -1) {
                return new int[0];
            }
            int[] candidates = new int[noOfCandidates];
            int index = 0;
            for(int i = 0; i<9; i++) {
                if (isCandidate(i)) {
                    candidates[index] = i;
                    index++;
                }
            }
            return candidates;
        }

        public bool isCandidate(int number) {
            return candidates[number];
        }
        
    }
}
