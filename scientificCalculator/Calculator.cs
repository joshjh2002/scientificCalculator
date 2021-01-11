using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace scientificCalculator
{
    public partial class Simple_Calculator : Form
    {
        public Simple_Calculator()
        {
            //InitializeComponent();
            setUpForm();
        }

        TabControl menuTab = new TabControl() { Name = "menuTab", Width = 715, Height = 397, Location = new Point(0,25) };
        Panel scientificGraph = new Panel() { Width = 420, Height = 320, Location = new Point(265, 5), BackColor = Color.White };
        List<HelpSheet> helpSheets = new List<HelpSheet>();

        private void setUpForm()
        {
            this.Width = 715;
            this.Height = 422;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Text = "Calculator";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormClosed += Simple_Calculator_FormClosed;

            this.Controls.Add(menuTab);
            menuTab.TabPages.Add("Graphing Calculator"); //Index: 0
            menuTab.TabPages.Add("Stats"); //Index: 1
            menuTab.TabPages.Add("Integration"); //Index: 2
            menuTab.TabPages.Add("Polynomials"); //Index: 3
            menuTab.TabPages.Add("Number Base Conversion"); //Index: 4
            menuTab.TabPages.Add("Simultaneous Equations"); //Index: 5
            menuTab.TabPages.Add("Tables"); //Index: 6
            menuTab.TabPages.Add("Parametric Equations"); //Index: 7

         
            MenuStrip menuStrip = new MenuStrip(); //Create menu strip object
            menuStrip.Items.Add("Help"); //add help button
            this.Controls.Add(menuStrip); //add the menu strip to the form
            menuStrip.Items[0].Click += Simple_Calculator_Click; //create the click event

            setUpGraphingCalculator();
            setupPolynomials();
            setUpStats();
            setUpSimultaneousEquations();
            setUpParametricEquations();
            setUpCalculus();
            setUpNumber();
            setUpTables();
        }

        private void Simple_Calculator_Click(object sender, EventArgs e)
        {
            if (helpSheets.Count != 0)
            {
                helpSheets[0].Close();
                helpSheets.RemoveAt(0);
            }

            string title = "", text = "";
            StreamReader sr;
            switch (menuTab.SelectedIndex)
            {
                case 0:
                    sr = new StreamReader("helpsheets/scientificCalculatorHelp.txt");
                    text = sr.ReadToEnd();
                    title = "Graphing Calulator";
                    break;
                case 1:
                    sr = new StreamReader("helpsheets/statsHelpSheet.txt");
                    text = sr.ReadToEnd();
                    title = "Stats";
                    break;
                case 2:
                    sr = new StreamReader("helpsheets/statsHelpSheet.txt");
                    text = sr.ReadToEnd();
                    title = "Integration";
                    break;
                case 3:
                    sr = new StreamReader("helpsheets/polynomialsHelpSheet.txt");
                    text = sr.ReadToEnd();
                    title = "Polynomials";
                    break;
                case 4:
                    sr = new StreamReader("helpsheets/numberHelpSheet.txt");
                    text = sr.ReadToEnd();
                    title = "Number Conversions";
                    break;
                case 5:
                    sr = new StreamReader("helpsheets/simultaneousHelpSheet.txt");
                    text = sr.ReadToEnd();
                    title = "Simultaneous Equations";
                    break;
                case 6:
                    sr = new StreamReader("helpsheets/tablesHekpSheet.txt");
                    text = sr.ReadToEnd();
                    title = "Tables";
                    break;
                case 7:
                    sr = new StreamReader("helpsheets/parametricEquations.txt");
                    text = sr.ReadToEnd();
                    title = "Parametric Equations";
                    break;

            }
            HelpSheet testSheet = new HelpSheet(title, text);
            helpSheets.Add(testSheet);
            testSheet.Show();
        }

        #region Tables

        private void setUpTables()
        {
            Label instructionsLabel = new Label()
            {
                Text = "Insert as many functions as you wish.\nEach function is to be placed on a new line.\nEach equation must be in terms of x",
                AutoSize = true,
                Location = new Point(5, 5)
            };
            menuTab.TabPages[6].Controls.Add(instructionsLabel); //0

            RichTextBox functionsTextbox = new RichTextBox() { Width = 250, Height = 150, Location = new Point(5, 55) };
            menuTab.TabPages[6].Controls.Add(functionsTextbox); //1

            Label minLabel = new Label() { Text = "Min:", Location = new Point(5, 215), AutoSize = true };
            menuTab.TabPages[6].Controls.Add(minLabel); //2

            TextBox minValue = new TextBox() { Location = new Point(37, 215), Width = 75 };
            menuTab.TabPages[6].Controls.Add(minValue); //3

            Label maxLabel = new Label() { Text = "Max:", Location = new Point(5, 245), AutoSize = true };
            menuTab.TabPages[6].Controls.Add(maxLabel); //4

            TextBox maxValue = new TextBox() { Location = new Point(37, 245), Width = 75 };
            menuTab.TabPages[6].Controls.Add(maxValue); //5

            Label stepLabel = new Label() { Text = "Step:", Location = new Point(5, 275), AutoSize = true };
            menuTab.TabPages[6].Controls.Add(stepLabel); //6

            TextBox stepValue = new TextBox() { Location = new Point(37, 275), Width = 75 };
            menuTab.TabPages[6].Controls.Add(stepValue); //7

            Button exportToCSV = new Button() { Text = "Export to .csv", Location = new Point(117, 215), Width = 100 };
            menuTab.TabPages[6].Controls.Add(exportToCSV); //8
            exportToCSV.Click += ExportToCSV_Click;



        }

        private void ExportToCSV_Click(object sender, EventArgs e)
        {
            try
            {
                string[] equations = menuTab.TabPages[6].Controls[1].Text.Split('\n');
                double min = Convert.ToDouble(menuTab.TabPages[6].Controls[3].Text);
                double max = Convert.ToDouble(menuTab.TabPages[6].Controls[5].Text);
                double step = Convert.ToDouble(menuTab.TabPages[6].Controls[7].Text);

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "csv|*.csv";
                DialogResult result = sfd.ShowDialog();
                if (result == DialogResult.OK && min < max && step > 0 && (max - min) * step * equations.Length <= 10000)
                {
                    string location = sfd.FileName;
                    System.Threading.Thread createTable = new System.Threading.Thread(() => myMath.tables(equations, min, max, step, location));
                    createTable.Start();
                }
                else if (result == DialogResult.Cancel)
                {

                }
                else
                    MessageBox.Show("Something went wrong. The minimum must be less than the maximum and the step must not be less than 0 and the number of calculations in less then 10,000.");
            }
            catch
            {
                MessageBox.Show("Make sure you have input valid numbers and make sure the file you're trying to save to is not open");
            }
        }



        #endregion

        #region Integration

        private void setUpCalculus()
        {
            Label instructionsLabel = new Label { Text = "Insert equation below in terms of x:", Location = new Point(5, 5), AutoSize = false, Width = 175 };
            menuTab.TabPages[2].Controls.Add(instructionsLabel); //0

            TextBox inputTextbox = new TextBox() { Location = new Point(5, 30), Width = 175 };
            menuTab.TabPages[2].Controls.Add(inputTextbox); //1

            Button submitCalculusButton = new Button { Text = "Submit", Location = new Point(5, 55), Width = 100 };
            menuTab.TabPages[2].Controls.Add(submitCalculusButton); //2
            submitCalculusButton.Click += SubmitCalculusButton_Click;

            Label outputLabel = new Label()
            {
                BorderStyle = BorderStyle.Fixed3D,
                Location = new Point(5, 83),
                AutoSize = false,
                Width = 250,
                Height = 100,
                Text = "Result:\n\nArea Under Curve: "
            };
            menuTab.TabPages[2].Controls.Add(outputLabel); //3

            Panel panel = new Panel() { Width = 420, Height = 320, Location = new Point(265, 5), BackColor = Color.White };
            menuTab.TabPages[2].Controls.Add(panel); //4

            Label minLabel = new Label() { Location = new Point(5, 203), Text = "min:", AutoSize = true };
            menuTab.TabPages[2].Controls.Add(minLabel); //5
            
            Label maxLabel = new Label() { Location = new Point(80, 203), Text = "max:", AutoSize = true };
            menuTab.TabPages[2].Controls.Add(maxLabel); //6

            TextBox minTextbox = new TextBox() { Location = new Point(5, 223), Width = 70, AutoSize = true };
            menuTab.TabPages[2].Controls.Add(minTextbox); //7

            TextBox maxTextbox = new TextBox() { Location = new Point(80, 223), Width = 70, AutoSize = true };
            menuTab.TabPages[2].Controls.Add(maxTextbox); //8
        }

        private void SubmitCalculusButton_Click(object sender, EventArgs e)
        {
            try
            {
                double min = Convert.ToDouble(menuTab.TabPages[2].Controls[7].Text);
                double max = Convert.ToDouble(menuTab.TabPages[2].Controls[8].Text);

                if (min >= max)
                    MessageBox.Show("Min must be less than max", "Error");
                else
                {
                    string equation = menuTab.TabPages[2].Controls[1].Text.Trim();
                    Panel p = (Panel)menuTab.TabPages[2].Controls[4];
                    draw(equation, p, new List<string>());

                    Label outputLabel = (Label)menuTab.TabPages[2].Controls[3];

                    myMath.SimpsonsRule(outputLabel, equation, (decimal)min, (decimal)max);
                }
            }
            catch
            {
                MessageBox.Show("Min and Max must be numbers.", "Error");
            }
        }

        #endregion

        #region Number Conversion

        private void setUpNumber()
        {
            Label numberInputLabel = new Label() { Text = "Number:", Location = new Point(5,5), Width = 60 };
            menuTab.TabPages[4].Controls.Add(numberInputLabel); //0

            TextBox inputNmber = new TextBox() { Width = 100, Location = new Point(65, 5) };
            menuTab.TabPages[4].Controls.Add(inputNmber); //1

            Label convertFromBaseLabel = new Label() { Text = "Convert from base (2-16):", Location = new Point(175, 5), Width = 126 };
            menuTab.TabPages[4].Controls.Add(convertFromBaseLabel); //2

            ComboBox numberInputBaseComboBox = new ComboBox() { Width = 45, Location = new Point(305, 5)};
            for (int i = 2; i <= 16; i++)
                numberInputBaseComboBox.Items.Add(i);
            menuTab.TabPages[4].Controls.Add(numberInputBaseComboBox); //3

            Label convertTOBaseLabel = new Label() { Text = "Convert to base (2-16):", Location = new Point(5, 35), Width = 115 };
            menuTab.TabPages[4].Controls.Add(convertTOBaseLabel);

            ComboBox convertToBaseComboBox = new ComboBox() { Width = 45, Location = new Point(125, 35) };
            for (int i = 2; i <= 16; i++)
                convertToBaseComboBox.Items.Add(i);
            menuTab.TabPages[4].Controls.Add(convertToBaseComboBox);

            Label outputLabel = new Label() { Text = "Result: ", Width = 165, BorderStyle = BorderStyle.Fixed3D, Location = new Point(185, 35) };
            menuTab.TabPages[4].Controls.Add(outputLabel);

            Button submitNumberConversion = new Button() { Text = "Submit", Width = 100, Location = new Point(5, 70) };
            menuTab.TabPages[4].Controls.Add(submitNumberConversion);
            submitNumberConversion.Click += submitNumberConversion_Click;
        }

        void submitNumberConversion_Click(object sender, EventArgs e)
        {
            //try-catch so that it won't crash if invalid numbers are put in for each base
            try
            {
                Label output = (Label)menuTab.TabPages[4].Controls[6];

                int numberFromBase = Convert.ToInt16(menuTab.TabPages[4].Controls[3].Text);
                int numberToBase = Convert.ToInt16(menuTab.TabPages[4].Controls[5].Text);
                string number = menuTab.TabPages[4].Controls[1].Text;

                //will check if the bas is within the accepted range, if it is, it will check if the number is in the correct base
                if (numberFromBase >= 2 && numberFromBase <= 16 && numberToBase >= 2 && numberToBase <= 16)
                {
                    bool isValid = true;
                    //holds a list of whitelisted charatcers
                    string accepted = "1234567890ABCDEF";

                    foreach (char c in number)
                    {
                        //converts each number into their numbered equivilent
                        if (accepted.Contains(c))
                        {
                            int currentNumber = 0;
                            switch (c)
                            {
                                case 'A':
                                    currentNumber = 10;
                                    break;
                                case 'B':
                                    currentNumber = 11;
                                    break;
                                case 'C':
                                    currentNumber = 12;
                                    break;
                                case 'D':
                                    currentNumber = 13;
                                    break;
                                case 'E':
                                    currentNumber = 14;
                                    break;
                                case 'F':
                                    currentNumber = 15;
                                    break;
                                default:
                                    currentNumber = Convert.ToInt32(c.ToString());
                                    break;
                            }

                            //if the number that it's currently looking at is greater than or equal to the base from
                            //it will break the loop and set isValid to false
                            if (currentNumber >= numberFromBase)
                            {
                                isValid = false;
                                break;
                            }
                        }
                        else
                        {
                            //if the base base isn't between 2 and 16 inclusive, it will set isValid to false
                            isValid = false;
                            break;
                        }

                        //if the input is valid, it will convert it using the ConvertNumber() method
                        if (isValid == true)
                            output.Text = "Result: " + myMath.ConvertNumber(number, numberToBase, numberFromBase);
                        else
                            MessageBox.Show("Input was not in correct format");
                    }
                }
            }
            catch
            {
                MessageBox.Show("1 more more numbers could not be converted. Please make sure you have input integer numbers");
            }
        }

        #endregion

        #region Parametric Equations

        private void setUpParametricEquations()
        {
            Label instructionsLabel = new Label() { Location = new Point(5, 90), Text = "Please enter 2 equations, \none for 'x' and one for 'y', \nin terms of 't'.", AutoSize = true };
            menuTab.TabPages[7].Controls.Add(instructionsLabel); //0

            Label xLabel = new Label() { Text = "x =", Location = new Point(5, 15), AutoSize = false, Width = 22 };
            menuTab.TabPages[7].Controls.Add(xLabel); //1

            Label yLabel = new Label() { Text = "y =", Location = new Point(5, 40), AutoSize = false, Width = 22 };
            menuTab.TabPages[7].Controls.Add(yLabel); //2

            TextBox xTextbox = new TextBox() { Width = 100, Location = new Point(30, 15) };
            menuTab.TabPages[7].Controls.Add(xTextbox); //3

            TextBox yTextbox = new TextBox() { Width = 100, Location = new Point(30, 40) };
            menuTab.TabPages[7].Controls.Add(yTextbox); //4

            Button submitButton = new Button() { Location = new Point(30, 65), Width = 100, Text = "Submit" };
            menuTab.TabPages[7].Controls.Add(submitButton); //5
            submitButton.Click += submitParametric_Click;

            Panel panel = new Panel() { Width = 420, Height = 320, Location = new Point(265, 5), BackColor = Color.White };
            menuTab.TabPages[7].Controls.Add(panel); //6
        }

        private void drawParametric(string xEquation, string yEquation)
        {
            Panel panel = (Panel)menuTab.Controls[7].Controls[6];
            /*creates a graphics function and assigns 
            the CreateGraphics() function of the groupbox to it*/
            Graphics g = panel.CreateGraphics();

            BufferedGraphics bufferedGraphics;
            Rectangle rectangle = new Rectangle();
            rectangle = panel.ClientRectangle;
            bufferedGraphics = BufferedGraphicsManager.Current.Allocate(g, rectangle);

            //resets the graph so that each new equation can be graphed
            bufferedGraphics.Graphics.Clear(Color.White);

            //creats pens. One for the axis and the other for the line
            Pen p = new Pen(Color.Black);
            Pen p2 = new Pen(Color.Blue);

            //Draws the axis
            int width = panel.Width;
            int height = panel.Height;
            bufferedGraphics.Graphics.DrawLine(p, 0, 320 / 2, 420, 320 / 2);
            bufferedGraphics.Graphics.DrawLine(p, 420 / 2, 0, 420 / 2, 320);

            //this sets the origin of the graph to the centre of the groupbox
            bufferedGraphics.Graphics.TranslateTransform(panel.Width / 2, panel.Height / 2);

            float scale = 5;
            //float x1 = -graph.Width / 2, y1 = -graph.Height / 2, y2 = 0;

            //This initialises the equation
            List<string> arrayXEquation = myMath.initialise(xEquation, new List<string>());
            List<string> arrayYEquation = myMath.initialise(yEquation, new List<string>());

            //string o = "";
            //foreach (string s in arrayEquation)
            //    o += s+ "\n";
            //MessageBox.Show(o);

            if (arrayXEquation.Contains("x") || arrayYEquation.Contains("x"))
                return;

            if (arrayXEquation != null && arrayYEquation != null)
            {
                List<string> toGraphX = new List<string>(arrayXEquation);
                List<string> toGraphY = new List<string>(arrayYEquation);
                float x1 = -width / 2, y1 = 0;
                bool firstOne = true;
                for (float t = -panel.Width / (2 * scale); t < panel.Width / (2 * scale); t += 0.1f)
                {

                    //replaces all the x values with the value currently assigned to x
                    toGraphX = new List<string>(arrayXEquation);
                    toGraphY = new List<string>(arrayYEquation);
                    for (int i = 0; i < toGraphX.Count; i++)
                    {
                        toGraphX[i] = toGraphX[i].Replace("t", Convert.ToString(t));
                        //MessageBox.Show(toGraph[i]);
                    }

                    for (int i = 0; i < toGraphY.Count; i++)
                    {
                        toGraphY[i] = toGraphY[i].Replace("t", Convert.ToString(t));
                        //MessageBox.Show(toGraph[i]);
                    }

                    //checks if it will be seen of the graph. If not, it won't draw it because there is no point in drawing what can't be seen
                    float x2 = (float)Convert.ToDouble(myMath.solvePreInitialised(toGraphX));
                    float y2 = (float)Convert.ToDouble(myMath.solvePreInitialised(toGraphY));

                    //if the value is not real, it cannot draw it, so it will set the boolean to true so that it doesn't attempt to draw it.
                    if (y2.ToString() == "NaN" || x2.ToString() == "NaN")
                        firstOne = true;

                    //draws the next part of the graph. the y values are set to negative because the axis ae inversed
                    try
                    {
                        //added so that it doesn't draw the first line as it is a starting coordinate, and so that it doesn't draw NaN values
                        switch (firstOne)
                        {
                            case false:
                                bufferedGraphics.Graphics.DrawLine(p2, x1 * scale, -y1 * scale, x2 * scale, -y2 * scale);
                                break;
                            case true:
                                firstOne = false;
                                break;
                        }
                    }
                    catch { }
                    x1 = x2;
                    y1 = y2;
                }
            }
            bufferedGraphics.Render(g);
        }

        private void submitParametric_Click(object sender, EventArgs e)
        {
            string xEquation = menuTab.Controls[7].Controls[3].Text;
            string yEquation = menuTab.Controls[7].Controls[4].Text;

            drawParametric(xEquation, yEquation);

            //for (int t = -p.Width / 2; t < p.Width / 2; t += 0.25) ;
        }

        #endregion

        #region Simultaneous Equations

        private void setUpSimultaneousEquations()
        {
            Panel simultaneousEquationsPanel = new Panel() { Location = new Point(5, 5), Width = 500, Height = 300, BackColor = Color.White };
            menuTab.TabPages[5].Controls.Add(simultaneousEquationsPanel); //0

            #region RadioButtons
            RadioButton Unknowns2Setup = new RadioButton() { Text = "2 Unknowns", Location = new Point(5, 5), Checked = true };
            RadioButton Unknowns3Setup = new RadioButton() { Text = "3 Unknowns", Location = new Point(5, 25) };
            simultaneousEquationsPanel.Controls.Add(Unknowns2Setup); //0
            simultaneousEquationsPanel.Controls.Add(Unknowns3Setup); //1
            Unknowns2Setup.CheckedChanged += Unknowns2Setup_CheckedChanged;
            Unknowns3Setup.CheckedChanged += Unknowns3Setup_CheckedChanged;
            #endregion

            #region 2Unknowns
            Panel Unknowns2Panel = new Panel() { Location = new Point(5, 50), Width = 400, Height = 200 };
            simultaneousEquationsPanel.Controls.Add(Unknowns2Panel); //2

            for (int x = 0; x < 2; x++)
            {
                for (int i = 0; i <= 1; i++)
                {
                    Label label1 = new Label() { AutoSize = true };
                    TextBox textbox1 = new TextBox() { Width = 40 };
                    switch (i)
                    {
                        case 0:
                            label1.Text = "x +";
                            break;
                        case 1:
                            label1.Text = "y =";
                            break;
                        default:
                            break;
                    }
                    label1.Location = new Point((i + 1) * 80 - 20, x * 40 + 5);
                    textbox1.Location = new Point((i + 1) * 80 - 60, x * 40 + 5);
                    Unknowns2Panel.Controls.Add(label1);
                    Unknowns2Panel.Controls.Add(textbox1);
                }
                TextBox t1 = new TextBox() { Width = 40 };
                t1.Location = new Point(2 * 80 + 20, x * 40 + 5);
                Unknowns2Panel.Controls.Add(t1);
            }


            #endregion

            #region 3Uknowns
            Panel Unknowns3Panel = new Panel() { Location = new Point(5, 50), Width = 400, Height = 200 };
            simultaneousEquationsPanel.Controls.Add(Unknowns3Panel); //3
            Unknowns3Panel.Visible = false;

            for (int x = 0; x < 3; x++)
            {
                for (int i = 0; i <= 2; i++)
                {
                    Label label1 = new Label() { AutoSize = true };
                    TextBox textbox1 = new TextBox() { Width = 40 };
                    switch (i)
                    {
                        case 0:
                            label1.Text = "x +";
                            break;
                        case 1:
                            label1.Text = "y +";

                            break;
                        case 2:
                            label1.Text = "z =";

                            break;
                        default:
                            break;
                    }
                    label1.Location = new Point((i + 1) * 80 - 20, x * 40 + 5);
                    textbox1.Location = new Point((i + 1) * 80 - 60, x * 40 + 5);
                    Unknowns3Panel.Controls.Add(label1);
                    Unknowns3Panel.Controls.Add(textbox1);
                }
                TextBox t1 = new TextBox() { Width = 40 };
                t1.Location = new Point(3 * 80 + 20, x * 40 + 5);
                Unknowns3Panel.Controls.Add(t1);
            }

            #endregion


            Button submitButton = new Button() { Text = "Submit", Location = new Point(515, 5), AutoSize = true };
            menuTab.TabPages[5].Controls.Add(submitButton); //1
            submitButton.Click += submitButton_Click;

            Label outputLabel = new Label() { Text = "Results:\n", Location = new Point(515, 35), AutoSize = false, Width = 100, Height = 200 };
            menuTab.TabPages[5].Controls.Add(outputLabel); //2
        }

        void Unknowns2Setup_CheckedChanged(object sender, EventArgs e)
        {
            Panel temp = (Panel)menuTab.TabPages[5].Controls[0].Controls[2];
            Panel temp2 = (Panel)menuTab.TabPages[5].Controls[0].Controls[3];
            temp.Visible = true;
            temp2.Visible = false;
        }

        void Unknowns3Setup_CheckedChanged(object sender, EventArgs e)
        {
            Panel temp = (Panel)menuTab.TabPages[5].Controls[0].Controls[3];
            Panel temp2 = (Panel)menuTab.TabPages[5].Controls[0].Controls[2];
            temp.Visible = true;
            temp2.Visible = false;
        }

        void submitButton_Click(object sender, EventArgs e)
        {
            RadioButton rb1 = (RadioButton)menuTab.TabPages[5].Controls[0].Controls[0];
            Label l1 = (Label)menuTab.TabPages[5].Controls[2];
            Panel Unknowns2 = (Panel)menuTab.TabPages[5].Controls[0].Controls[2];
            Panel Unknowns3 = (Panel)menuTab.TabPages[5].Controls[0].Controls[3];

            if (rb1.Checked == true)
                myMath.simultaneousEquations(0, Unknowns2, l1);
            else
                myMath.simultaneousEquations(1, Unknowns3, l1);
        }

        #endregion

        #region Polynomials
        private void setupPolynomials()
        {
            #region Insert Equation
            Label insertEquationLabel = new Label() { Text = "Insert Equation", Font = new Font("", 12), Location = new Point(5, 5), AutoSize = true };
            menuTab.TabPages[3].Controls.Add(insertEquationLabel); //0

            Label xCubedLabel = new Label() { Text = "x³", Location = new Point(50, 30), AutoSize = true };
            menuTab.TabPages[3].Controls.Add(xCubedLabel); //1

            TextBox xCubedTextbox = new TextBox() { Location = new Point(20, 25), Width = 30, Text = "0" };
            menuTab.TabPages[3].Controls.Add(xCubedTextbox); //2

            Label xSquaredLabel = new Label() { Text = "x²", Location = new Point(95, 30), AutoSize = true };
            menuTab.TabPages[3].Controls.Add(xSquaredLabel); //3

            TextBox xSquaredTextbox = new TextBox() { Location = new Point(65, 25), Width = 30, Text = "0" };
            menuTab.TabPages[3].Controls.Add(xSquaredTextbox); //4

            Label xLabel = new Label() { Text = "x", Location = new Point(140, 30), AutoSize = true };
            menuTab.TabPages[3].Controls.Add(xLabel); //5

            TextBox xTextbox = new TextBox() { Location = new Point(110, 25), Width = 30, Text = "0" };
            menuTab.TabPages[3].Controls.Add(xTextbox); //6

            Label solutionTypeLabel = new Label() { Text = "= 0", Location = new Point(185, 30), AutoSize = true };
            menuTab.TabPages[3].Controls.Add(solutionTypeLabel); //7

            TextBox constantTextbox = new TextBox() { Location = new Point(155, 25), Width = 30, Text = "0" };
            menuTab.TabPages[3].Controls.Add(constantTextbox); //8
            #endregion

            Label solutionsLabel = new Label() { Text = "Values for which, y = 0", Location = new Point(5, 100), AutoSize = true, Font = new Font("", 12) };
            menuTab.TabPages[3].Controls.Add(solutionsLabel); //9

            TextBox solutionsTextbox = new TextBox() { Location = new Point(20, 125), Width = 400, ReadOnly = true };
            menuTab.TabPages[3].Controls.Add(solutionsTextbox); //10

            Label yInterceptLabel = new Label() { Text = "y-intercept: ", Location = new Point(5, 150), AutoSize = true, Font = new Font("", 12) };
            menuTab.TabPages[3].Controls.Add(yInterceptLabel); //11

            Label integrationLabel = new Label() { Text = "Integration: ", Location = new Point(250, 150), AutoSize = true, Font = new Font("", 12) };
            menuTab.TabPages[3].Controls.Add(integrationLabel); //12

            Label differentiationLabel = new Label() { Text = "Differentiation: ", Location = new Point(250, 200), AutoSize = true, Font = new Font("", 12) };
            menuTab.TabPages[3].Controls.Add(differentiationLabel); //13

            Button submitPolynomailButton = new Button() { Text = "Submit", AutoSize = true, Location = new Point(210, 23) };
            menuTab.TabPages[3].Controls.Add(submitPolynomailButton); //14
            submitPolynomailButton.Click += submitPolynomailButton_Click;
        }

        void submitPolynomailButton_Click(object sender, EventArgs e)
        {
            try
            {
                int a = Convert.ToInt32(menuTab.TabPages[3].Controls[2].Text);
                int b = Convert.ToInt32(menuTab.TabPages[3].Controls[4].Text);
                int c = Convert.ToInt32(menuTab.TabPages[3].Controls[6].Text);
                int d = Convert.ToInt32(menuTab.TabPages[3].Controls[8].Text);

                TextBox solutions = (TextBox)menuTab.TabPages[3].Controls[10];
                Label differnetiationLabel = (Label)menuTab.TabPages[3].Controls[13];
                Label integrationLabel = (Label)menuTab.TabPages[3].Controls[12];
                Label yIntercept = (Label)menuTab.TabPages[3].Controls[11];

                //added if statement so that number of terms can be detected
                int terms = 0;
                if (a == 0 && b == 0)
                    terms = 0;
                else if (a != 0)
                    terms = 2;
                else if (b != 0)
                    terms = 1;


                myMath.polynomialSolve(terms, a, b, c, d, solutions, differnetiationLabel, integrationLabel, yIntercept);
            }
            catch
            {
                MessageBox.Show("Cannot Convert Numbers", "Error");
            }
        }
        #endregion

        #region Stats
        private void setUpStats()
        {
            ListBox statsItemsListbox = new ListBox() { Location = new Point(5, 5), Width = 120, Height = 328 };
            menuTab.TabPages[1].Controls.Add(statsItemsListbox); //0
            statsItemsListbox.KeyPress += statsItemsListbox_KeyPress;

            TextBox statsValueInputTextbox = new TextBox() { Location = new Point(130, 5), Width = 104 };
            menuTab.TabPages[1].Controls.Add(statsValueInputTextbox); //1
            statsValueInputTextbox.KeyPress += statsValueInputTextbox_KeyPress;

            Button submitStatButton = new Button() { Location = new Point(130, 27), Text = "Add", Width = 50 };
            menuTab.TabPages[1].Controls.Add(submitStatButton); //2
            submitStatButton.Click += submitStatButton_Click;

            Button removeStatButton = new Button() { Location = new Point(180, 27), Text = "Remove", Width = 55 };
            menuTab.TabPages[1].Controls.Add(removeStatButton); //3
            removeStatButton.Click += removeStatButton_Click;

            Button clearStatsButton = new Button() { Location = new Point(130, 53), Text = "Clear", Width = 104 };
            menuTab.TabPages[1].Controls.Add(clearStatsButton); //4
            clearStatsButton.Click += clearStatsButton_Click;

            Label statsOutputLabel = new Label() { Text = "Averages:", BorderStyle = BorderStyle.Fixed3D, Location = new Point(130, 80), AutoSize = false, Width = 200, Height = 242 };
            menuTab.TabPages[1].Controls.Add(statsOutputLabel); //5

            Button importStatsDataButton = new Button() { Text = "Import Data", Location = new Point(235, 4) };
            menuTab.TabPages[1].Controls.Add(importStatsDataButton); //6
            importStatsDataButton.Click += ImportStatsDataButton_Click;

            Button submitStatsButton = new Button() { Text = "Submit", Location = new Point(235, 27) };
            menuTab.TabPages[1].Controls.Add(submitStatsButton); //7
            submitStatsButton.Click += SubmitStatsButton_Click;

            Panel boxPlotPanel = new Panel() { Location = new Point(340, 5), Width = 346, Height = 317, BackColor = Color.White };
            menuTab.TabPages[1].Controls.Add(boxPlotPanel); //8
        }

        private void SubmitStatsButton_Click(object sender, EventArgs e)
        {
            //As the only global variable is the menuTab, if I want to edit an object, I have to redeclare it
            ListBox l1 = (ListBox)menuTab.TabPages[1].Controls[0];
            Panel p1 = (Panel)menuTab.TabPages[1].Controls[8];

            if (l1.Items.Count != 0)
            {
                List<double> items = new List<double>();
                //This will take all the values in the listbox and store them into a list of doubles
                foreach (double s in l1.Items)
                {
                    try
                    {
                        items.Add(Convert.ToDouble(s));
                    }
                    catch
                    {
                        
                    }
                }

                Label outputLabel = (Label)menuTab.TabPages[1].Controls[5];
                myMath.stats(items, outputLabel, p1);
            }
        }

        private void ImportStatsDataButton_Click(object sender, EventArgs e)
        {
            //creates an OpenFileDialog
            OpenFileDialog ofd = new OpenFileDialog() { Filter = "Text File|*.txt" };

            //opens the dialog and checks if the selected directory exists, if not, nothing will happen
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //redeclares the listbox and empties it
                ListBox l1 = (ListBox)menuTab.TabPages[1].Controls[0];
                l1.Items.Clear();

                List<double> items = new List<double>();

                //deslares a StreamReader and it will read the file the user selected
                StreamReader sr = new StreamReader(ofd.FileName);
                string s;
                //It will read each line until there are no more lines (this is when it s becomes null) and stores it in s.
                while ((s = sr.ReadLine()) != null)
                {
                    //If for some reason, it cannot convert a value into a double, it will skip that value and continue,
                    //otherwise it will add it to the listbox and the list of doubles
                    try
                    {
                        l1.Items.Add(Convert.ToDouble(s));
                        items.Add(Convert.ToDouble(s));
                    }
                    catch
                    {
                        
                    }

                }
                sr.Close();

                //if the list isn't empty, it will redeclare the outputLable and panel and pass them into the stats() method in the myMath class
                if (l1.Items.Count != 0)
                {
                    Label outputLabel = (Label)menuTab.TabPages[1].Controls[5];
                    Panel p1 = (Panel)menuTab.TabPages[1].Controls[8];
                    myMath.stats(items, outputLabel, p1);
                }
            }
        }

        void submitStatButton_Click(object sender, EventArgs e)
        {
            submitStats();
        }

        void removeStatButton_Click(object sender, EventArgs e)
        {
            //redeclare the listbox
            ListBox l1 = (ListBox)menuTab.TabPages[1].Controls[0];
            //If the selected index is -1 which means no items are selected, 
            //it will not remove the item and cause the program to crash and return an error
            if (l1.SelectedIndex >= 0)
                l1.Items.RemoveAt(l1.SelectedIndex);
        }

        void clearStatsButton_Click(object sender, EventArgs e)
        {
            //This will clear the listbox
            ListBox l1 = (ListBox)menuTab.TabPages[1].Controls[0];
            l1.Items.Clear();
        }

        void statsValueInputTextbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            //If the enter key is pressed while in the textbox, it will call the submitStats() subroutine
            if (e.KeyChar == (char)13) //13 = [ENTER]
                submitStats();
        }

        void submitStats()
        {
            ListBox l1 = (ListBox)menuTab.TabPages[1].Controls[0];
            TextBox t1 = (TextBox)menuTab.TabPages[1].Controls[1];
            //It will try and convert the value into a double. If it can, it will add it to the listbox and empty the textbox
            try
            {
                l1.Items.Add(Convert.ToDouble(t1.Text));
                t1.Text = "";
            }
            //If it cannot conver the value into a double, it will return an error
            catch
            {
                MessageBox.Show("Value is invalid", "Error");
            }
        }

        void statsItemsListbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            //If the backspace key is pressed, it will remove the selected item in the listbox
            ListBox l1 = (ListBox)menuTab.TabPages[1].Controls[0];
            if (e.KeyChar == (char)Keys.Back && l1.SelectedIndex != -1)
            {
                l1.Items.RemoveAt(l1.SelectedIndex);
            }
        }

        #endregion

        #region Calculator
        private void setUpGraphingCalculator()
        {
            TextBox equationTextbox = new TextBox() { Text = "", Width = 200, Height = 20, Location = new Point(5, 5) };
            menuTab.TabPages[0].Controls.Add(equationTextbox); //Index: 0
            equationTextbox.KeyPress += equationTextbox_KeyPress;

            Button submitCalculationButton = new Button() { Text = "Submit", Width = 50, Height = 20, Location = new Point(210, 5) };
            menuTab.TabPages[0].Controls.Add(submitCalculationButton);
            submitCalculationButton.Click += submitCalculationButton_Click; //Index: 1

            ListBox historyListbox = new ListBox() { Width = 255, Height = 150, Location = new Point(5, 30) };
            menuTab.TabPages[0].Controls.Add(historyListbox); //Index: 2
            historyListbox.KeyPress += historyListbox_KeyPress;

            ListBox variableListbox = new ListBox() { Width = 140, Height = 150, Location = new Point(5, 180) };
            menuTab.TabPages[0].Controls.Add(variableListbox); //Index: 3
            variableListbox.Items.Add("-Variables-");

            Button removeHistoryButton = new Button() { Text = "Remove", Width = 110, Height = 20, Location = new Point(150, 180) };
            menuTab.TabPages[0].Controls.Add(removeHistoryButton);
            removeHistoryButton.Click += removeHistoryButton_Click; //Index: 4

            Button clearHistroyButton = new Button() { Text = "Clear", Width = 110, Height = 20, Location = new Point(150, 205) };
            menuTab.TabPages[0].Controls.Add(clearHistroyButton);
            clearHistroyButton.Click += clearHistroyButton_Click; //Index: 5

            menuTab.TabPages[0].Controls.Add(scientificGraph);
            scientificGraph.MouseDown += graph_MouseDown; //Index: 6
            scientificGraph.Scroll += Graph_Scroll;

            LoadVariablesAndHistroy();
        }

        private void Graph_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void historyListbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            ListBox l1 = (ListBox)menuTab.TabPages[0].Controls[2];
            if (e.KeyChar == (char)Keys.Back && l1.SelectedIndex != -1)
            {
                l1.Items.RemoveAt(l1.SelectedIndex);
            }
        }

        private void equationTextbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) //13 = [ENTER]
            {
                sumbitPressed();
            }
        }


        private void draw(string equation, Panel graph, List<string> variablesList)
        {
            /*creates a graphics function and assigns 
            the CreateGraphics() function of the groupbox to it*/
            Graphics g = graph.CreateGraphics();

            BufferedGraphics bufferedGraphics;
            Rectangle rectangle = new Rectangle();  
            rectangle = graph.ClientRectangle;
            bufferedGraphics = BufferedGraphicsManager.Current.Allocate(g, rectangle);

            //resets the graph so that each new equation can be graphed
            bufferedGraphics.Graphics.Clear(Color.White);

            //creats pens. One for the axis and the other for the line
            Pen p = new Pen(Color.Black);
            Pen p2 = new Pen(Color.Blue);

            //Draws the axis
            bufferedGraphics.Graphics.DrawLine(p, 0, 320 / 2, 420, 320 / 2);
            bufferedGraphics.Graphics.DrawLine(p, 420 / 2, 0, 420 / 2, 320);

            //this sets the origin of the graph to the centre of the groupbox
            bufferedGraphics.Graphics.TranslateTransform(graph.Width / 2, graph.Height / 2);

            float scale = 5;
            float x1 = -graph.Width / 2, y1 = -graph.Height / 2, y2 = 0;

            //This initialises the equation
            List<string> arrayEquation = myMath.initialise(equation, variablesList);

            //string o = "";
            //foreach (string s in arrayEquation)
            //    o += s+ "\n";
            //MessageBox.Show(o);

            if (arrayEquation.Contains("t"))
                return;

            if (arrayEquation != null)
            {
                bool firstOne = true;
                List<string> toGraph = new List<string>(arrayEquation);
                for (float x = -graph.Width / (2 * scale); x < graph.Width / (2 * scale) - 1; x += 0.1f)
                {
                    //replaces all the x values with the value currently assigned to x
                    toGraph = new List<string>(arrayEquation);
                    for (int i = 0; i < toGraph.Count; i++)
                    {
                        toGraph[i] = toGraph[i].Replace("x", Convert.ToString(x));
                        //MessageBox.Show(toGraph[i]);
                    }

                    //checks if it will be seen of the graph. If not, it won't draw it because there is no point in drawing what can't be seen
                    y2 = (float)Convert.ToDouble(myMath.solvePreInitialised(toGraph));
                    if (y2.ToString() == "NaN")
                        firstOne = true;
                    //draws the next part of the graph. the y values are set to negative because the axis ae inversed
                    try
                    {
                        //g.DrawLine(p, x1 * scale, -y1 * scale, x * scale, -y2 * scale);
                        switch (firstOne)
                        {
                            case false:
                                bufferedGraphics.Graphics.DrawLine(p2, x1 * scale, -y1 * scale, x * scale, -y2 * scale);
                                break;
                            case true:
                                firstOne = false;
                                break;
                        }

                    }
                    catch { }
                    x1 = x;
                    y1 = y2;
                }
            }
            bufferedGraphics.Render(g);
        }

        private void graph_MouseDown(object sender, EventArgs e)
        {

        }

        private void clearHistroyButton_Click(object sender, EventArgs e)
        {
            ListBox l1 = (ListBox)menuTab.TabPages[0].Controls[2];
            l1.Items.Clear();
        }

        private void removeHistoryButton_Click(object sender, EventArgs e)
        {
            ListBox l1 = (ListBox)menuTab.TabPages[0].Controls[2];
            if (l1.SelectedIndex != -1)
                l1.Items.RemoveAt(l1.SelectedIndex);
        }

        private void submitCalculationButton_Click(object sender, EventArgs e)
        {
            sumbitPressed();
        }

        private void sumbitPressed()
        {
            string acceptedVariables = "abcdfghijklmnopqrstuvw";
            string equation = menuTab.TabPages[0].Controls[0].Text;

            int count = 0;
            foreach (char c in equation)
                if (c == '=')
                    count++;
            string[] variable = equation.Split('=');

            //MODIFICATION MADE SURE TAHT IT DOESN'T CRASH WHEN IT CONSTAINS X, Y, OR Z
            if (count == 1 && acceptedVariables.Contains(variable[0]) && !variable[1].Contains("x") && !variable[1].Contains("y") && !variable[1].Contains("z"))
            {
                equation = equation.ToLower();
                
                string variableValueS = myMath.solve(variable[1], new List<string>());
                double variableValue = 0;
                try
                {
                    variableValue = Convert.ToDouble(variableValueS);
                }
                catch
                {
                    MessageBox.Show(equation + " is invalid. Please make sure that the expression is correct");
                }

                if (equation.Contains("=") && (variableValue <= 0 || variableValue > 0) && variable[0].Length == 1 && acceptedVariables.Contains(variable[0]) && !acceptedVariables.Contains(variable[1]))
                {
                    ListBox variablesListbox = (ListBox)menuTab.TabPages[0].Controls[3];
                    int addWhere = 0;
                    for (int i = 1; i < variablesListbox.Items.Count; i++)
                    {
                        if (variablesListbox.Items[i].ToString()[0] == variable[0][0])
                        {
                            variablesListbox.Items.RemoveAt(i);
                            addWhere = i;
                        }
                    }

                    if (addWhere == 0)
                        variablesListbox.Items.Add(variable[0] + "=" + variableValue);
                    else
                        variablesListbox.Items.Insert(addWhere, variable[0] + "=" + variableValue);
                }
                else if (variable[0].Length != 1)
                    MessageBox.Show("The variable must be a single character");
            }
            else if (count > 1)
            {
                MessageBox.Show(equation + " is invalid. Make sure there is only 1 equals (=)");
            }
            else
            {
                ListBox variablesListbox = (ListBox)menuTab.TabPages[0].Controls[3];
                List<string> variables = new List<string>();

                for (int i = 1; i < variablesListbox.Items.Count; i++)
                    variables.Add(variablesListbox.Items[i].ToString());

                submitEquation(variables);
            }
        }

        private void submitEquation(List<string> variablesList)
        {
            //ADDED EQUATION VARIABLE TO PASS INTO SOLVE() TO FIX SUBSTITUATION ERROR HIGHLIGNTED IN SUBSTITUTEEVARLUES()
            string equation = menuTab.TabPages[0].Controls[0].Text.Trim();
            if (menuTab.TabPages[0].Controls[0].Text.Trim() != "")
            {
                ListBox l1 = (ListBox)menuTab.TabPages[0].Controls[2];
                if (menuTab.TabPages[0].Controls[0].Text.IndexOf("x") == -1)
                {
                    l1.Items.Add(equation + " = " + myMath.solve(equation, variablesList));
                }
                else //ADDED THE ABILITY TO HAVE VAIRABLES IN GRAPHS
                {
                    string equationOriginal = menuTab.TabPages[0].Controls[0].Text;
                    string equationSubstituted = equationOriginal;

                    if (variablesList.Count != 0)
                    {
                        foreach (string s in variablesList)
                        {
                            string[] variables = s.Split('=');
                            equationSubstituted = equationSubstituted.Replace(variables[0], "(" + variables[1] + ")");

                        }
                    }
                    draw(equationSubstituted, scientificGraph, variablesList);
                }

                l1.SelectedIndex = l1.Items.Count - 1;
            }
        }

        #endregion

        private void menuButton_Click(object sender, EventArgs e)
        {

        }

        private void Simple_Calculator_Load(object sender, EventArgs e)
        {

        }

        //SAVE VARIABLES AND HISTORY
        private void Simple_Calculator_FormClosed(object sender, FormClosedEventArgs e)
        {
            ListBox historyListbox = (ListBox)menuTab.TabPages[0].Controls[2];
            ListBox variablesListbox = (ListBox)menuTab.TabPages[0].Controls[3];

            StreamWriter sw = new StreamWriter("saves/histroy.txt");

            for (int i = 0; i < historyListbox.Items.Count; i++)
            {
                sw.WriteLine(historyListbox.Items[i]);
            }

            sw.Close();
            sw = new StreamWriter("saves/variables.txt");

            for (int i = 1; i < variablesListbox.Items.Count; i++)
            {
                sw.WriteLine(variablesListbox.Items[i]);
            }

            sw.Close();
        }

        //LOAD VARIABLES AND HISTORY
        private void LoadVariablesAndHistroy()
        {
            ListBox historyListbox = (ListBox)menuTab.TabPages[0].Controls[2];
            ListBox variablesListbox = (ListBox)menuTab.TabPages[0].Controls[3];

            string s = "";
            StreamReader sr; 
            if (File.Exists("saves/histroy.txt"))
            {
                sr = new StreamReader("saves/histroy.txt");
                while ((s = sr.ReadLine()) != null)
                    historyListbox.Items.Add(s);
                sr.Close();
            }
            
            if (File.Exists("saves/variables.txt"))
            {
                sr = new StreamReader("saves/variables.txt");
                s = "";

                while ((s = sr.ReadLine()) != null)
                    variablesListbox.Items.Add(s);

                sr.Close();
            }
        }
    }
}