﻿#region Copyright
// Copyright 2015 Geoff Green.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;


namespace Potato.Tools.NetworkConsole.Controls {
    class CodRichTextBox : RichTextBox {

        private Dictionary<string, Color> m_dicChatTextColours;

        public CodRichTextBox() : base() {
            // Deviated a bit from cod4 because some of these colours suck.
            //^0 = Black
            //^1 = Maroon (Potato error message colour)
            //^2 = MediumSeaGreen
            //^3 = DarkOrange
            //^4 = RoyalBlue
            //^5 = Cornflower Blue
            //^6 = Dark Violet
            //^7 = Deep Pink (No one is judging you if you use this in your plugin =D )
            //^8 = Red
            //^9 = Grey
            // http://www.tayloredmktg.com/rgb/
            m_dicChatTextColours = new Dictionary<string, Color>();
            m_dicChatTextColours.Add("^0", Color.Black);
            m_dicChatTextColours.Add("^1", Color.Maroon);
            m_dicChatTextColours.Add("^2", Color.MediumSeaGreen);
            m_dicChatTextColours.Add("^3", Color.DarkOrange);
            m_dicChatTextColours.Add("^4", Color.RoyalBlue);
            m_dicChatTextColours.Add("^5", Color.CornflowerBlue);
            m_dicChatTextColours.Add("^6", Color.DarkViolet);
            m_dicChatTextColours.Add("^7", Color.DeepPink);
            m_dicChatTextColours.Add("^8", Color.Red);
            m_dicChatTextColours.Add("^9", Color.Gray);
        }

        public void SetColour(string strVariable, string strValue) {

            var strCaretNumber = strVariable.Replace("TEXT_COLOUR_", "^");
            
            if (m_dicChatTextColours.ContainsKey(strCaretNumber) == true) {
                m_dicChatTextColours[strCaretNumber] = Color.FromName(strValue);
            }
            else {
                m_dicChatTextColours.Add(strCaretNumber, Color.FromName(strValue));
            }

        }

        /* This is the older much slower code I had to add colour and font styles to the RTB.
        private int FindCaretCode(int iAppendedLength) {

            int iReturnIndex = -1;

            for (int i = this.Text.Length - iAppendedLength; i < this.Text.Length - 1; i++) {
                if (this.Text[i] == '^' && (char.IsDigit(this.Text[i + 1]) == true || this.Text[i + 1] == 'b' || this.Text[i + 1] == 'n' || this.Text[i + 1] == 'i')) {
                    if (i > 0 && this.Text[i - 1] != '^') {
                        iReturnIndex = i;
                        break;
                    }
                    else if (i == 0) { // Else if only the i > 0 failed in the above.
                        iReturnIndex = i;
                        break;
                    }
                }
            }

            return iReturnIndex;
        }
        

        // TO DO: Once format is established clean up this method.
        private void ColourizeAppendedText(int iAppendedTextLength, int iCarets) {

            Color clrColour = Color.Black;
            bool blFindingColourCodes = false;

            int i = -1;
            int iFoundCarets = 0;

            string strSelectedText = String.Empty;
            this.ReadOnly = false;
            do {
                i = -1;
                blFindingColourCodes = false;

                //if ((i = this.Find("^", this.Text.Length - iConsoleOutputLength - 1, this.Text.Length, RichTextBoxFinds.MatchCase)) > 0) {
                if ((i = this.FindCaretCode(iAppendedTextLength)) >= 0) {
                    if (i < this.Text.Length - 1 && char.IsDigit(this.Text[i + 1]) == true) {

                        this.Select(i, 2);

                        if (this.m_dicChatTextColours.ContainsKey((strSelectedText = this.SelectedText)) == true) {
                            clrColour = this.m_dicChatTextColours[strSelectedText];
                        }

                        //this.ReadOnly = false;
                        this.SelectedText = String.Empty;
                        iAppendedTextLength -= 2;
                        //this.ReadOnly = true;

                        this.Select(this.SelectionStart, iAppendedTextLength);
                        this.SelectionColor = clrColour;

                        blFindingColourCodes = true;
                    }
                    else if (i < this.Text.Length - 1 && this.Text[i + 1] == 'b') {

                        this.Select(i, 2);

                        //this.ReadOnly = false;
                        this.SelectedText = String.Empty;
                        iAppendedTextLength -= 2;
                        //this.ReadOnly = true;

                        this.Select(this.SelectionStart, iAppendedTextLength);
                        this.SelectionFont = new Font("Calibri", 10, FontStyle.Bold);

                        blFindingColourCodes = true;
                    }
                    else if (i < this.Text.Length - 1 && this.Text[i + 1] == 'i') {

                        this.Select(i, 2);

                        //this.ReadOnly = false;
                        this.SelectedText = String.Empty;
                        iAppendedTextLength -= 2;
                        //this.ReadOnly = true;

                        this.Select(this.SelectionStart, iAppendedTextLength);
                        this.SelectionFont = new Font("Calibri", 10, FontStyle.Italic);

                        blFindingColourCodes = true;
                    }
                    else if (i < this.Text.Length - 1 && this.Text[i + 1] == 'n') {

                        this.Select(i, 2);

                        //this.ReadOnly = false;
                        this.SelectedText = String.Empty;
                        iAppendedTextLength -= 2;
                        //this.ReadOnly = true;

                        this.Select(this.SelectionStart, iAppendedTextLength);
                        this.SelectionFont = new Font("Calibri", 10);

                        blFindingColourCodes = true;
                    }

                    iFoundCarets++;
                }
            } while (blFindingColourCodes == true && iFoundCarets < iCarets);

            while ((i = this.Find("^^", this.Text.Length - iAppendedTextLength, this.Text.Length, RichTextBoxFinds.MatchCase)) > 0) {
                this.Select(i, 2);

                //this.ReadOnly = false;
                this.SelectedText = "^";
                //this.ReadOnly = true;
            }

            this.ReadOnly = true;
        }

        public new void AppendText(string strText) {

            base.AppendText(strText);

            int iCarets = 0;

            if ((iCarets = this.GetCaretCount(strText)) > 0) {
                this.ColourizeAppendedText(strText.Length - 1, iCarets);
            }
        }
        */

        private int FindCaretCode(string strText, int iAppendedLength) {

            var iReturnIndex = -1;

            //string test = this.Text;

            for (var i = strText.Length - iAppendedLength; i < strText.Length - 1; i++) {
                if (strText[i] == '^' && (char.IsDigit(strText[i + 1]) == true || strText[i + 1] == 'b' || strText[i + 1] == 'n' || strText[i + 1] == 'i')) {
                    if (i > 0 && strText[i - 1] != '^') {
                        iReturnIndex = i;
                        break;
                    }
                    else if (i == 0) { // Else if only the i > 0 failed in the above.
                        iReturnIndex = i;
                        break;
                    }
                }
            }

            return iReturnIndex;
        }

        private int GetCaretCount(string strText) {
            var iCarets = 0;

            for (var i = 0; i < strText.Length; i++) {
                if (strText[i] == '^') {
                    iCarets++;
                }
            }

            return iCarets;
        }

        private struct STextChange {
            public int m_iPosition;
            public Color m_clTextColour;
            public Font m_fntTextFont;
        }

        public new void AppendText(string strText) {

            var lstChanges = new List<STextChange>();

            var iCarets = 0;
            var iAppendedStartPosition = -1;
            var iAppendedTextLength = -1;

            if ((iCarets = GetCaretCount(strText)) > 0) {

                iAppendedStartPosition = Text.Length;
                iAppendedTextLength = strText.Length;

                var i = -1;
                var iFoundCarets = 0;
                var blFindingColourCodes = false;

                var strColourCode = string.Empty;
                var cFontCode = 'n';

                do {
                    i = -1;
                    blFindingColourCodes = false;

                    //if ((i = this.Find("^", this.Text.Length - iConsoleOutputLength - 1, this.Text.Length, RichTextBoxFinds.MatchCase)) > 0) {
                    if ((i = FindCaretCode(strText, iAppendedTextLength)) >= 0) {

                        if (i < iAppendedTextLength - 1 && char.IsDigit(strText[i + 1]) == true) {

                            var stcChange = new STextChange();
                            stcChange.m_iPosition = i;

                            strColourCode = strText.Substring(i, 2);

                            // Remove the ^[0-9]
                            strText = strText.Substring(0, i) + strText.Substring(i + 2);
                            iAppendedTextLength -= 2;

                            if (m_dicChatTextColours.ContainsKey(strColourCode) == true) {
                                stcChange.m_clTextColour = m_dicChatTextColours[strColourCode];
                            }

                            lstChanges.Add(stcChange);

                            blFindingColourCodes = true;
                        }
                        else if (i < iAppendedTextLength - 1 && ((cFontCode = strText[i + 1]) == 'b' || strText[i + 1] == 'n' || strText[i + 1] == 'i')) {

                            var stcChange = new STextChange();
                            stcChange.m_iPosition = i;

                            switch (cFontCode) {
                                case 'n':
                                    stcChange.m_fntTextFont = Font;// new Font("Calibri", 10);
                                    break;
                                case 'b':
                                    stcChange.m_fntTextFont = new Font(Font, FontStyle.Bold);  //new Font("Calibri", 10, FontStyle.Bold);
                                    break;
                                case 'i':
                                    stcChange.m_fntTextFont = new Font(Font, FontStyle.Italic);  //new Font("Calibri", 10, FontStyle.Italic);
                                    break;
                                default:
                                    break;
                            }

                            // Remove the ^[b|n|i]
                            strText = strText.Substring(0, i) + strText.Substring(i + 2);
                            iAppendedTextLength -= 2;

                            lstChanges.Add(stcChange);

                            blFindingColourCodes = true;
                        }

                        // Just stops that last pass of the string when we know how many times
                        // it should pass anyway.
                        iFoundCarets++;
                    }
                } while (blFindingColourCodes == true && iFoundCarets < iCarets);

                while ((i = strText.IndexOf("^^")) > 0) {
                    strText = strText.Substring(0, i) + "^" + strText.Substring(i + 2);
                    iAppendedTextLength--;
                }
            }

            base.AppendText(strText);

            if (iAppendedStartPosition >= 0) { 
                foreach (var stcChange in lstChanges) {
                    Select(iAppendedStartPosition + stcChange.m_iPosition, iAppendedTextLength - stcChange.m_iPosition);

                    if (stcChange.m_fntTextFont != null) {
                        SelectionFont = stcChange.m_fntTextFont;
                    }
                    else {
                        SelectionColor = stcChange.m_clTextColour;
                    }
                }
            }
        }
        

    }
}
