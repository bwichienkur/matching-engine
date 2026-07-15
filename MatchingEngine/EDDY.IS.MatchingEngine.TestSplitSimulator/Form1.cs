using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EDDY.IS.MatchingEngine.TestSplitSimulator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Dictionary<int, TestSubject> SetupTestSubjects()
        {
            Dictionary<int, TestSubject> subjects = new Dictionary<int, TestSubject>();

            //TestSubject tSubject1 = new TestSubject(1, 20);
            //TestSubject tSubject2 = new TestSubject(21, 50);

            //subjects.Add(1, tSubject1);
            //subjects.Add(2, tSubject2);
            //subjects.Add(3, new TestSubject(51, 100));

            int i = 1;

            subjects.Add(1, new TestSubject(i, Convert.ToInt32(txtSubject1.Text)));

            if (txtSubject2.Text.Length > 0)
            {
                i += Convert.ToInt32(txtSubject1.Text);
                subjects.Add(2, new TestSubject(i,  i + Convert.ToInt32(txtSubject2.Text) - 1));
            }

            if (txtSubject3.Text.Length > 0)
            {
                i += Convert.ToInt32(txtSubject2.Text);
                subjects.Add(3, new TestSubject(i, i + Convert.ToInt32(txtSubject3.Text) - 1));
            }

            if (txtSubject4.Text.Length > 0)
            {
                i += Convert.ToInt32(txtSubject3.Text);
                subjects.Add(3, new TestSubject(i, i + Convert.ToInt32(txtSubject4.Text) - 1));
            }

            if (txtSubject5.Text.Length > 0)
            {
                i += Convert.ToInt32(txtSubject4.Text);
                subjects.Add(3, new TestSubject(i, i + Convert.ToInt32(txtSubject5.Text) - 1));
            }
            return subjects;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Dictionary<int, TestSubject> subjects = SetupTestSubjects();
            int iterationCount = 0;

            txtResults.Text = "Processing...";

            Random rand = new Random();
            for (int i = 0; i < Convert.ToInt32(txtIterations.Text); i++)
            {
                
                
                int diceRoll = rand.Next(1, 101);

                int choosenTestSubject = subjects.Where(p => p.Value.Range.Item1 <= diceRoll && p.Value.Range.Item2 >= diceRoll).First().Key;
                iterationCount++;
                subjects[choosenTestSubject].CountApplied++;
                //System.Threading.Thread.Sleep(10);
            }

            string s = "Iteration Count: " + iterationCount.ToString() + Environment.NewLine;
            foreach(int j in subjects.Keys)
            {
                int percentToShow = subjects[j].Range.Item2;

                if(subjects.ContainsKey(j - 1))
                    percentToShow -= subjects[j - 1].Range.Item2;

                s += "Subject " + j + ": " + Environment.NewLine;
                s += "    Percent To Show: " + percentToShow.ToString() + Environment.NewLine;
                s += "    Times Applied: " + subjects[j].CountApplied + Environment.NewLine;
                s += "    Percent Applied: " + (subjects[j].CountApplied / (float)iterationCount).ToString() + Environment.NewLine;
            }

            txtResults.Text = s;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtSubject2.Enabled = false;
            txtSubject3.Enabled = false;
            txtSubject4.Enabled = false;
            txtSubject5.Enabled = false;
        }

        private void SetSplits()
        {
            bool blnSubject1 = txtSubject1.Text.Length > 0;
            bool blnSubject2 = txtSubject2.Text.Length > 0;
            bool blnSubject3 = txtSubject3.Text.Length > 0;
            bool blnSubject4 = txtSubject4.Text.Length > 0;
            bool blnSubject5 = txtSubject5.Text.Length > 0;

            int totalPercent = Convert.ToInt32(txtSubject1.Text);

            if (totalPercent < 100)
            {
                txtSubject2.Enabled = true;
                if (blnSubject2)
                {
                    totalPercent += Convert.ToInt32(txtSubject2.Text);

                    if (totalPercent < 100)
                    {
                        txtSubject3.Enabled = true;
                        if (blnSubject3)
                        {
                            totalPercent += Convert.ToInt32(txtSubject3.Text);

                            if (totalPercent < 100)
                            {
                                txtSubject4.Enabled = true;
                                if (blnSubject4)
                                {
                                    totalPercent += Convert.ToInt32(txtSubject4.Text);

                                    if (totalPercent < 100)
                                    {
                                        txtSubject5.Enabled = true;
                                    }
                                    else if (totalPercent == 100)
                                    {
                                        txtSubject5.Enabled = false;
                                        txtSubject5.Text = "";
                                    }
                                    else
                                    {
                                        txtSubject4.Text = (100 - (totalPercent - Convert.ToInt32(txtSubject4.Text))).ToString();
                                        txtSubject5.Enabled = false;
                                        txtSubject5.Text = "";
                                    }
                                }
                            }
                            else if (totalPercent == 100)
                            {
                                txtSubject4.Enabled = false;
                                txtSubject4.Text = "";
                                txtSubject5.Enabled = false;
                                txtSubject5.Text = "";
                            }
                            else
                            {
                                txtSubject3.Text = (100 - (totalPercent - Convert.ToInt32(txtSubject3.Text))).ToString();
                                txtSubject4.Enabled = false;
                                txtSubject4.Text = "";
                                txtSubject5.Enabled = false;
                                txtSubject5.Text = "";
                            }
                        }
                    }
                    else if (totalPercent == 100)
                    {
                        txtSubject3.Enabled = false;
                        txtSubject3.Text = "";
                        txtSubject4.Enabled = false;
                        txtSubject4.Text = "";
                        txtSubject5.Enabled = false;
                        txtSubject5.Text = "";
                    }
                    else
                    {
                        txtSubject2.Text = (100 - (totalPercent - Convert.ToInt32(txtSubject2.Text))).ToString();
                        txtSubject3.Enabled = false;
                        txtSubject3.Text = "";
                        txtSubject4.Enabled = false;
                        txtSubject4.Text = "";
                        txtSubject5.Enabled = false;
                        txtSubject5.Text = "";
                    }
                }
            }
            else if (totalPercent == 100)
            {
                txtSubject2.Text = "";
                txtSubject2.Enabled = false;
                txtSubject3.Text = "";
                txtSubject3.Enabled = false;
                txtSubject4.Text = "";
                txtSubject4.Enabled = false;
                txtSubject5.Text = "";
                txtSubject5.Enabled = false;
            }
            else
            {
                txtSubject1.Text = "100";
                txtSubject2.Text = "";
                txtSubject2.Enabled = false;
                txtSubject3.Text = "";
                txtSubject3.Enabled = false;
                txtSubject4.Text = "";
                txtSubject4.Enabled = false;
                txtSubject5.Text = "";
                txtSubject5.Enabled = false;
            }

        }

        private void txtSubject1_Leave(object sender, EventArgs e)
        {
            SetSplits();
        }

        private void txtSubject2_Leave(object sender, EventArgs e)
        {
            SetSplits();
        }

        private void txtSubject3_Leave(object sender, EventArgs e)
        {
            SetSplits();
        }

        private void txtSubject4_Leave(object sender, EventArgs e)
        {
            SetSplits();
        }
    }

    public class TestSubject
    {
        public Tuple<int, int> Range { get; set; }
        public int CountApplied { get; set; }

        public TestSubject(int start, int end)
        {
            Range = new Tuple<int, int>(start, end);
        }
    }
}
