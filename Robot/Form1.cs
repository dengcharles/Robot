using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Robot
{
    public partial class Form1 : Form
    {
        private bool iniStart = false;

        public Form1()
        {
            InitializeComponent();
        }

        private int intOutputU1 = -1;
        private int intOutputU2 = -1;
        private string strOutputFacing = string.Empty;

        private void btnRun_Click(object sender, EventArgs e)
        {

            string strIn = txtInput.Text.ToUpper();
            lblReport.Text = string.Empty;

            if (!ValidateMessage(strIn))
            {
                lblReport.Text = "Input is invalid";
            }

            txtInput.Text = string.Empty;

        }

        private bool ValidateMessage(string strIn)
        {

            //bool result = true;

            if (!iniStart)
            {
                if (!strIn.StartsWith("PLACE"))
                {
                    return false;
                }
            }

            if (strIn.StartsWith("PLACE "))
            {
                //Match full PLACE statement
                string strMatchPattern = @"^PLACE [0-5][,][0-5][,](NORTH|SOUTH|EAST|WEST)$";

                Match match = Regex.Match(strIn, strMatchPattern, RegexOptions.IgnoreCase);

                //Match with PLACE coordinates only
                string strSubMathPattern = @"^PLACE [0-5][,][0-5]$";

                Match subPlaceMatch = Regex.Match(strIn, strSubMathPattern);

                //Full PLACE statement
                if (match.Success)
                {

                    return MatchPLACE(strIn, true);
                }
                // PLACE with coordinates only
                else if (iniStart && subPlaceMatch.Success)
                {
                    return MatchPLACE(strIn, false);
                }

            }
            else if (strIn == "LEFT" || strIn == "RIGHT")
            {
                if (strIn == "LEFT")
                {
                    if (strOutputFacing == "NORTH")
                        strOutputFacing = "WEST";
                    else if (strOutputFacing == "EAST")
                        strOutputFacing = "NORTH";
                    else if (strOutputFacing == "SOUTH")
                        strOutputFacing = "EAST";
                    else
                        strOutputFacing = "SOUTH";
                }
                else
                {
                    if (strOutputFacing == "NORTH")
                        strOutputFacing = "EAST";
                    else if (strOutputFacing == "EAST")
                        strOutputFacing = "SOUTH";
                    else if (strOutputFacing == "SOUTH")
                        strOutputFacing = "WEST";
                    else
                        strOutputFacing = "NORTH";
                }
            }
            else if (strIn == "MOVE")
            {
                if (strOutputFacing == "NORTH" && intOutputU2 <= 4)
                {
                    intOutputU2++;
                }
                else if (strOutputFacing == "EAST" && intOutputU1 <= 4)
                {
                    intOutputU1++;
                }
                else if (strOutputFacing == "SOUTH" && intOutputU2 >= 1)
                {
                    intOutputU2--;
                }
                else if (strOutputFacing == "WEST" && intOutputU1 >= 1)
                {
                    intOutputU1--;
                }
                else // All other combination is invalid
                {
                    return false;
                }
            }
            else if (strIn == "REPORT")
            {
                if (intOutputU1 == -1 || intOutputU2 == -1 || string.IsNullOrEmpty(strOutputFacing))
                    return false;
                lblReport.Text = "Output: " + intOutputU1.ToString() + "," + intOutputU2.ToString() + "," + strOutputFacing;
            }
            else
            {
                return false;
            }

            return true;
        }

        private bool MatchPLACE(string strIn, bool fullPLACE)
        {
            string[] validDirection = new string[] { "NORTH", "EAST", "SOUTH", "WEST" };
            //Get first int
            string strDirection = string.Empty;

            string str1stIntPattern = @"([0-5]+)";
            int i1 = -1;
            int i2 = -1;
            Match int1Match = Regex.Match(strIn, str1stIntPattern);

            if (int1Match.Success)
            {
                int.TryParse(int1Match.Groups[1].Value, out i1);

            }

            string str2ndIntPattern = @"[\D]*[\d]+[\D]+([\d]+)";

            Match int2Match = Regex.Match(strIn, str2ndIntPattern);
            if (int2Match.Success)
            {
                int.TryParse(int2Match.Groups[1].Value, out i2);
            }

            if (fullPLACE)
            {
                string directionPattern = @"[^,\d\s][^,\d]*$";
                strDirection = Regex.Match(strIn, directionPattern, RegexOptions.RightToLeft)?.Value;
            }


            if (!(i1 == -1 || i2 == -1))
            {
                if ((i1 < 0 || i1 > 5) || (i2 < 0 || i2 > 5))
                {
                    return false;
                }
                else if (fullPLACE && !validDirection.Contains(strDirection))
                {
                    return false;
                }

                intOutputU1 = i1;
                intOutputU2 = i2;

                if (fullPLACE)
                {
                    strOutputFacing = strDirection;
                }


                if (!iniStart)
                    iniStart = true;

                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
