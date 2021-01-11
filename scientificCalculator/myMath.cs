using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace scientificCalculator
{

    class myMath
    {
        #region Calculator

        public static double solvePreInitialised(List<string> equation)
        {
            //string o = "";
            //foreach (string s in equation)
            //    o += s;
            //MessageBox.Show(o);

            equation = splitToBracket(equation);
            
            return calculate(equation);
        }

        public static List<string> initialise(string equation, List<string> variablesList)
        {
            equation = equation.Replace(" ", "");
            //This allows for bracket multiplication such as (3)(90)

            equation = equation.Replace(")(", ")*(");

            equation = replaceFuntionsWithIdentifiers(equation);

            equation = substituteValues(equation, variablesList);
            
            equation = simplifyNegativeAndPositive(equation);

            //Sorts the equation into a list
            List<string> arrayEquation = new List<string> (sortIntoList(equation));

            //string o = "";
            //foreach (string s in arrayEquation)
            //    o += s;
            //MessageBox.Show(o);

            bool foundError = errorFound(arrayEquation);

            string previousString = "";
            int indexOfBracketsNextToNumber = 0;

            if (foundError == false)
            {
                arrayEquation = solvePowers(arrayEquation);
                for (int i = 0; i < arrayEquation.Count - 1; i++)
                {
                    string s = arrayEquation[i];
                    //without this try catch statment, the program will crash when there is a string that it can't convert into a list
                    try
                    {
                        //this puts in an asterisk infront of a closed bracket if there is a number after it
                        //or an askerisk behind an open bracket if there is a number behind it
                        if (s == "(" && (Convert.ToDouble(previousString) == 0 || Convert.ToDouble(previousString) != 0))
                            arrayEquation.Insert(indexOfBracketsNextToNumber, "*");
                        else if (previousString == ")" && (Convert.ToDouble(s) == 0 || Convert.ToDouble(s) != 0))
                            arrayEquation.Insert(indexOfBracketsNextToNumber, "*");
                    }
                    catch
                    {

                    }

                    if (s == ")" && arrayEquation[i + 1] == "(")
                        arrayEquation.Insert(indexOfBracketsNextToNumber + 1, "*");

                    previousString = s;
                    indexOfBracketsNextToNumber++;
                }
                return arrayEquation;
            }
            return null;
        }

        //NET METHOD SO IT WONT CREATE SYNTAX ERROR IF FUNCTIONS CONTAIN VARIABLE NAMES
        private static string substituteValues(string equation, List<string> variablesList)
        {
            string equationOriginal = equation;
            string equationSubstituted = equationOriginal;

            if (variablesList.Count != 0)
            {
                foreach (string s in variablesList)
                {
                    string[] variables = s.Split('=');
                    equationSubstituted = equationSubstituted.Replace(variables[0], "(" + variables[1] + ")");

                }
            }

            return equationSubstituted;
        }

        private static string replaceFuntionsWithIdentifiers(string equation)
        {
            equation = equation.Replace("arcsin", "[");
            equation = equation.Replace("arccos", "]");
            equation = equation.Replace("arctan", "{");

            equation = equation.Replace("sin", "£");
            equation = equation.Replace("cos", "$");
            equation = equation.Replace("tan", "&");

            equation = equation.Replace("csc", "}");
            equation = equation.Replace("sec", "'");
            equation = equation.Replace("cot", "@");

            equation = equation.Replace("π", "(" + Math.PI + ")");
            equation = equation.Replace("pi", "(" + Math.PI + ")");

            equation = equation.Replace("e", "(" + Math.E + ")");

            return equation;
        }

        public static string solveBACKUP(string equation)
        {
            //loops forever

            //This makes it so that the program won't crash if there is a space between terms
            equation = equation.Replace(" ", "");
            //This allows for bracket multiplication such as (3)(90)

            equation = equation.Replace(")(", ")*(");

            equation = simplifyNegativeAndPositive(equation);

            //Sorts the equation into a list
            List<string> arrayEquation = sortIntoList(equation);

            arrayEquation = solvePowers(arrayEquation);


            //Displays the equation for debugging purposes
            //foreach (string s in arrayEquation)
            //{
            //    Console.WriteLine(s);
            //}
            //Console.ReadLine();

            string previousString = "";
            int indexOfBracketsNextToNumber = 0;

            for (int i = 0; i < arrayEquation.Count - 1; i++)
            {
                string s = arrayEquation[i];
                //without this try catch statment, the program will crash when there is a string that it can't convert into a list
                try
                {
                    //this puts in an asterisk infront of a closed bracket if there is a number after it
                    //or an askerisk behind an open bracket if there is a number behind it
                    if (s == "(" && (Convert.ToDouble(previousString) == 0 || Convert.ToDouble(previousString) != 0))
                        arrayEquation.Insert(indexOfBracketsNextToNumber, "*");
                    else if (previousString == ")" && (Convert.ToDouble(s) == 0 || Convert.ToDouble(s) != 0))
                        arrayEquation.Insert(indexOfBracketsNextToNumber, "*");
                }
                catch
                {

                }

                if (s == ")" && arrayEquation[i + 1] == "(")
                    arrayEquation.Insert(indexOfBracketsNextToNumber + 1, "*");

                previousString = s;
                indexOfBracketsNextToNumber++;
            }


            if (errorFound(arrayEquation) == false)
            {
                //It will loop until there are no more brackets in the equation
                while (arrayEquation.Contains("("))
                    arrayEquation = splitToBracket(arrayEquation);

                //This solves the rest of the equation and displays the results.
                return Convert.ToString(calculate(arrayEquation));
            }
            else
                return "Syntax Error";
        }

        public static string solve(string equation, List<string> variablesList)
        {
            List<string> arrayEquation = initialise(equation, variablesList);

            if (arrayEquation == null)
                return "Syntax Error";

            double output = solvePreInitialised(arrayEquation);

            return output.ToString();
        }

        private static string simplifyNegativeAndPositive(string equation)
        {
            //This will hold the whole equation but it will split up the plus and minus signs in each index. for example:
            //"2--6" would be stored in the list as "2", "--", "6"
            List<string> positiveAndNegative = new List<string>();
            string nextString = "";
            foreach (char c in equation)
            {
                if (c == '-' || c == '+')
                    nextString += c;
                else
                {
                    positiveAndNegative.Add(nextString);
                    positiveAndNegative.Add(Convert.ToString(c));
                    nextString = "";
                }
            }
            positiveAndNegative.Add(nextString);

            //This counts the number of negative signs in each index and replaces the plus and minuses accordingly
            for (int i = 0; i < positiveAndNegative.Count - 1; i++)
            {
                int numberOfNegatives = 0;
                for (int x = 0; x < positiveAndNegative[i].Length; x++)
                {
                    if (positiveAndNegative[i][x] == '-')
                        numberOfNegatives++;

                }

                //If there is an odd number of negative numbers it will replace the string with a just a '-'
                if (numberOfNegatives % 2 == 1 && numberOfNegatives != 0)
                    positiveAndNegative[i] = "-";
                //if there is an even number of minus signs and there numberOfNegatives doesnt equal 
                //or it contains a plus, it will replace it with a single '+'
                else if (numberOfNegatives % 2 == 0 && numberOfNegatives != 0 || positiveAndNegative[i].Contains("+"))
                    positiveAndNegative[i] = "+";
            }

            string output = "";
            //This will put the equation back into a string
            foreach (string s in positiveAndNegative)
                output += s;

            //return the final product
            return output;
        }

        private static bool errorFound(List<string> equation)
        {
            bool errorFound = false;
            int count = 0;
            string prevS = "";

            string whitelist = "+-/*^1234567890()tx.£$&[]{}'@";
            string functions = "£$&[]{}'@";

            if (countCharacters(equation, ")") > countCharacters(equation, "("))
                errorFound = true;

            if (functions.Contains(equation[equation.Count - 1]))
                errorFound = true;

            foreach (string s in equation)
            {
                //I can't add a backslash or speech mark into the string so I had to add it using its 
                //character codes
                

                if (s == "+" && (prevS == "+" || prevS == "*" || prevS == "/" || prevS == "^" || prevS == "-" || prevS == "(" || prevS == "" || functions.Contains(prevS) || count == equation.Count - 1))
                    errorFound = true;
                else if (s == "*" && (prevS == "*" || prevS == "+" || prevS == "/" || prevS == "^" || prevS == "-" || prevS == "(" || prevS == "" || functions.Contains(prevS) || count == equation.Count - 1))
                    errorFound = true;
                else if (s == "-" && (prevS == "" || count == equation.Count - 1))
                    errorFound = true;
                else if (s == "/" && (prevS == "/" || prevS == "+" || prevS == "*" || prevS == "^" || prevS == "-" || prevS == "(" || functions.Contains(prevS) || prevS == "" || count == equation.Count - 1))
                    errorFound = true;
                else if (s == "^" && (prevS == "^" || prevS == "+" || prevS == "/" || prevS == "*" || prevS == "-" || prevS == "(" || functions.Contains(prevS) || prevS == "" || count == equation.Count - 1))
                    errorFound = true;
                else if (s == ")" && (prevS == "" || functions.Contains(prevS) || prevS == "("))
                    errorFound = true;
                else if (s == "(" && count == equation.Count - 1)
                    errorFound = true;
                else if (s == "." && (prevS == "(" || prevS == ")"))
                    errorFound = true;
                else if (prevS == ")" && isNumber(s))
                    errorFound = true;
                else if (s == ".")
                    errorFound = true;
                else if (prevS == "+" & s == null)
                    errorFound = true;

                foreach (char c in s)
                    if (!whitelist.Contains(c))
                        errorFound = true;

                if (errorFound == true)
                {
                    string o = "";
                    
                    //foreach (string s1 in equation)
                    //    o += s1 + "\n";
                    //
                    //MessageBox.Show(o);

                    return errorFound;
                }

                prevS = s;
                count++;
            }
            
            return errorFound;
        }

        private static List<string> solvePowers(List<string> equation)
        {
            for (int i = equation.Count - 1; i > 0; i--)
            {
                if (equation[i] == "^" && equation[i + 1] == "(")
                {
                    int countBrackets = 0;
                    for (int x = i; x < equation.Count; x++)
                    {
                        if (equation[x] == ")")
                            countBrackets--;
                        else if (equation[x] == "(")
                            countBrackets++;
                        //MessageBox.Show(x.ToString() + " : " + equation[x] + " | " + countBrackets);
                        if (equation[x] == ")" && countBrackets == 0)
                            equation.Insert(x, ")");
                    }
                }
                else if (equation[i] == "^" && equation[i + 1] != "(" && equation[i + 1] != "-") 
                {
                    equation.Insert(i + 2, ")");
                }

                if (equation[i] == "^" && equation[i - 1] == ")") 
                {
                    int countBrackets = 0;
                    for (int x = i; x >= 0; x--)
                    {
                        if (equation[x] == ")")
                            countBrackets++;
                        else if (equation[x] == "(")
                            countBrackets--;

                        //MessageBox.Show(x.ToString() + " : " + equation[x] + " | " + countBrackets);
                        if (equation[x] == "(" && countBrackets == 0)
                            equation.Insert(x, "(");
                    }
                }
                else if (equation[i] == "^" && equation[i - 1] != ")")
                {
                    double number = 0;
                    try
                    {
                        number = Convert.ToDouble(equation[i - 1]);
                    }
                    catch
                    {

                    }

                    if (number < 0)
                    {
                        equation.RemoveAt(i - 1);
                        equation.Insert(i - 1, (-number).ToString());
                        equation.Insert(i - 1, "(");
                        equation.Insert(i - 1, "-");

                        if (equation[0] == "-")
                        {
                            equation.Insert(i - 1, "+");
                            equation.Insert(0, "0");
                        }
                    }
                    else
                        equation.Insert(i - 1, "(");
                }
            }

            //string o = "";
            //foreach (string s in equation)
            //    o += s;
            //MessageBox.Show(o);
            return equation;
        }

        private static List<string> identifyBrackets(List<string> equation)
        {
            List<string> brackets = new List<string>();
            int bracketsCount = 0;
            foreach (string s in equation)
            {
                if (s == "(")
                {
                    bracketsCount++;
                    brackets.Add(Convert.ToString(bracketsCount));
                }
                else if (s == ")")
                {
                    brackets.Add(Convert.ToString(bracketsCount));
                    bracketsCount--;
                }
                else
                    brackets.Add(Convert.ToString(bracketsCount));
            }

            return brackets;
        }

        //TOO SLOW
        private static List<string> splitToBracket(List<string> equation)
        {
            while (equation.Contains("("))
            {
            //This holds values to identify brackets
            List<string> brackets = new List<string>();

            //This for loop allows the program to have equations that have a number multiplied by a bracket e.g. 4(30)
            

            //Displays the equation for debugging
            //foreach (string s in equation)
            //{
            //    Console.Write(s);
            //}
            //Console.WriteLine();

            //This identifies where the brackets are and the level they are on e.g.
            //2/(67(77)-9+(2*8))
            //001112222111222221
            int maxBracket = 0;

            brackets = identifyBrackets(equation);

            //identifies the most embedded bracket
            foreach (string s in brackets)
            {
                if (maxBracket < Convert.ToInt32(s))
                    maxBracket = Convert.ToInt32(s);
            }


            //Console.WriteLine("mx " + maxBracket);

            //This will hold the most embedded bracket equation
            List<string> bracketEquation = new List<string>();

            //Holds where the open bracket of the most embedded bracket is
            int startIndex = 0;

            //Works out where it is by cycling through until maxBracket and equation[startIndex] are the same
            while (brackets[startIndex] != Convert.ToString(maxBracket))
                startIndex++;

            //To make sure that the starting position is correct
            //Console.WriteLine("st: " + startIndex);

            //Identifies the first set of brackets it needs to solve based on its most embedded bracket
            int count = startIndex;
            while (count < brackets.Count && brackets[count] == Convert.ToString(maxBracket))
            {
                bracketEquation.Add(equation[count]);
                count++;
            }

            //To make sure that all the values are correct for debugging
            //Console.WriteLine("st: " + startIndex);
            //foreach (string s in equation)
            //{
            //    Console.Write(s);
            //}
            //Console.WriteLine();
            //foreach (string s in bracketEquation)
            //{
            //    Console.Write(s);
            //}
            //Console.WriteLine();
            //
            //foreach (string s in brackets)
            //{
            //    Console.Write(s);
            //}
            //Console.WriteLine();

            //Holds how large the equation is so it can remove that part from the equation list
            int toRemove = bracketEquation.Count;

            bracketEquation.Remove("(");
            bracketEquation.Remove(")");

            //Console.WriteLine("tr: " + toRemove);
            //Solves the brackets
            double answer = calculate(bracketEquation);

            //Console.WriteLine(answer);

            //replaces the bracket in the equation with its value
            equation = replaceAnswer(equation, Convert.ToDouble(answer), startIndex, toRemove);

            //If the bracket is negative, it will make the answer negative e.g. -(81/9) == -9
            if (startIndex != 0 && equation[startIndex - 1] == "-")
            {
                equation[startIndex] = Convert.ToString(0 - Convert.ToDouble(equation[startIndex]));
                equation.RemoveAt(startIndex - 1);
            }

            //foreach (string s in equation)
            //{
            //    Console.Write(s);
            //}
            //Console.WriteLine();

            //returns the new equation with the most embedded bracket solved
            
            }

            return equation;

        }

        public static List<string> sortIntoList(string equation)
        {
            //holds the equation in list form
            List<string> arrayEquation = new List<string>();
            string nextIndex = "";
        
            //splits each term of the equation into a list
            char lastChar = '#';
            //sin  cos  tan  arcsin   arccos   arctan
            string functionIdentifiers = "£$&[]{}'@";
            string allowableVariables = "abcdefghklmnopqrstuvwxyz";
            foreach (char c in equation)
            {
                //it will know if it needs to go onto the next term when it finds one of these
                if (c == '+' && (lastChar == '#' || lastChar == '/' || lastChar == '*' || lastChar == '^' || lastChar == '('))
                {                                                                                                             
                    nextIndex = nextIndex.Trim();                                                                             
                    arrayEquation.Add(nextIndex);                                                                             
                    nextIndex = "+";                                                                                          
                }                                                                                                             
                else if (c == '+' || c == '*' || c == '/' || c == '(' || c == ')' || c == '^')                                
                {                                                                                                             
                    //it adds the number to the list, then the operation then sets the string to blank                        
                    nextIndex = nextIndex.Trim();                                                                             
                    arrayEquation.Add(nextIndex);                                                                             
                    arrayEquation.Add(Convert.ToString(c));                                                                   
                    nextIndex = "";                                                                                           
                }                                                                                                             
                else if (c == '-' && (lastChar == '#' || lastChar == '/' || lastChar == '*' || lastChar == '^'))              
                {                                                                                                             
                    nextIndex = nextIndex.Trim();                                                                             
                    arrayEquation.Add(nextIndex);                                                                             
                    nextIndex = "-";                                                                                          
                }                                                                                                             
                else if (c == '-' && (lastChar != '/' || lastChar != '*' || lastChar != '^') && lastChar != '(')              
                {                                                                                                             
                    nextIndex = nextIndex.Trim();                                                                             
                    arrayEquation.Add(nextIndex);                                                                             
                    arrayEquation.Add("+");                                                                                   
                    nextIndex = "-";                                                                                          
                }                                                                                                             
                else if (c == '-' && (lastChar == '/' || lastChar == '*' || lastChar != '^'))                                 
                {
                    nextIndex = nextIndex.Trim();
                    arrayEquation.Add(nextIndex);
                    nextIndex = "-";
                }
                else if (allowableVariables.Contains(c))
                {
                    nextIndex = nextIndex.Trim();
                    arrayEquation.Add(nextIndex);
                    arrayEquation.Add("(");
                    arrayEquation.Add(c.ToString());
                    arrayEquation.Add(")");
                    nextIndex = "";
                }
                else if (functionIdentifiers.Contains(c))
                {
                    nextIndex = nextIndex.Trim();
                    arrayEquation.Add(nextIndex);
                    arrayEquation.Add("(");
                    arrayEquation.Add(c.ToString());
                    
                    nextIndex = "";
                }
                else
                {
                    nextIndex += c;
                }
                lastChar = c;
            }
            arrayEquation.Add(nextIndex);
        
            //removes all empty strings in the array
            while (arrayEquation.Contains("") == true)
            {
                arrayEquation.Remove("");
            }

            arrayEquation = encloseFunctions(arrayEquation, functionIdentifiers);

            if (arrayEquation[0] == "-")
            {
                arrayEquation.Insert(0, "+");
                arrayEquation.Insert(0, "0");
            }

            for (int i = 0; i < arrayEquation.Count - 1; i++)
            {
                if (arrayEquation[i] == "-" && arrayEquation[i - 1] == "(")
                {
                    arrayEquation.Insert(i, "+");
                    arrayEquation.Insert(i, "0");
                }
            }

            string o = "";

            foreach (string s in arrayEquation)
                o += s + "\n";
            //MessageBox.Show(o);

            return arrayEquation;
        }

        private static bool isNumber(string number)
        {
            try
            {
                Convert.ToInt32(number.ToString());
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static List<string> encloseFunctions(List<string> equation, string functionIdentifiers)
        {

            //string o1 = "";
            //foreach (string s in equation)
            //    o1 += s + " ";
            
            //o1 += "\n";
            //foreach (string s in brackets)
            //    o1 += s + " ";
            //MessageBox.Show(o1, "Before");

            List<string> brackets = new List<string>(identifyBrackets(equation));

            for (int i = 0; i < equation.Count; i++)
            {

                if (functionIdentifiers.Contains(equation[i]))
                {
                    //if there are 2 open brackets next to eachother before a function identifier then 
                    //it will remove one of those brackets having 2 is unnecessary
                    if (i - 2 >= 0 && equation[i - 1] == "(" && equation[i - 2] == "(")
                    {
                        equation.RemoveAt(i - 2);
                        brackets = new List<string>(identifyBrackets(equation));
                    }
                    //if there is a plus next to a function identifier, it will remove it
                    else if (i+1 <= equation.Count - 1 && equation[i+1] == "+")
                    {
                        equation.RemoveAt(i + 1);
                        brackets = new List<string>(identifyBrackets(equation));
                    }

                    //if there is a bracket before a fucntion identifier, it will search forward until it needs to place a closed bracket
                    if (i - 1 >= 0 && i+2 <= equation.Count -1 && equation[i - 1] == "(") 
                    {
                        //identifies the value of the bracket before the identifier
                        string toFind = Convert.ToString(Convert.ToInt32(brackets[i + 2]));

                        //checks if there is a bracket to the right of the symbol, otherwise, it will place the closed bracket next to it i.e
                        // sin(30) will be read changed to sin()30) and that will return a syntax error
                        int temp = 0;
                        
                        if (equation[i + 1] == "(")
                            temp = 1;

                        //This will go from either the inside of the bracket enclosing the function, or 
                        for (int o = i + 2 + temp; o < equation.Count - 1; o++)
                        {
                            //if it encounters a power then it will skip it and go into the next index for example;
                            //sin30^2 is calculated as "sin(30^2)" so it has to skip the power symbol
                            bool canConvert = true;
                            try
                            {
                                Convert.ToDouble(equation[o]);
                            }
                            catch
                            {
                                canConvert = false;
                            }

                            if (equation[i] == "x" || equation[i] == "t")
                                canConvert = true;

                            if (brackets[o] == toFind && equation[o] == "^")
                            {
                                o++;
                            }
                            //if there are 2 functions next to eachother, it needs to close off the inner operation. It will then do the outer operation, 
                            //and it's brackets will continue on until they're manually closed
                            //added the x and t conditions because it was placing the bracket in the wrong plac causing a syntax error
                            else if (brackets[o] == toFind && (functionIdentifiers.Contains(equation[o]) || equation[o] == "x" || equation[o] == "t"))
                            {
                                o = o + 2;
                            }
                            //if toFind and bracket[o] is the same and there is no '^', it can place a closed bracket
                            else if (brackets[o] == toFind && (equation[o] != "^" || !functionIdentifiers.Contains(equation[o]) || equation[o] != "("))
                            {
                                int temp2 = 0;
                                if (canConvert == true)
                                    temp2 = 1;
                                //places closed bracket and updates the 'brackets' list so it fits with the 'equation' list
                                equation.Insert(o + temp2, ")");
                                brackets = new List<string>(identifyBrackets(equation));
                                break;
                            }
                        }
                    }
                }
            }

            //string o2 = "";
            //foreach (string s in equation)
            //    o2 += s;
            //MessageBox.Show(o2, "After");
            return equation;
        }
        
        public static double calculate(List<string> equation)
        {
            //this solves the equation in the order of BIDMAS. This means the powers will go first, then division, 
            //multiplication, subtraction and addition
            while (equation.Count != 1)
            {
                if (equation.Contains("£")) //sin
                {
                    int index = find(equation, "£");
                    
                    if (equation[index + 1] != "(" || equation[index + 1] != ")")
                    {
                        double answer = Math.Sin(Convert.ToDouble(equation[index + 1]));

                        equation = replaceAnswer(equation, answer, index, 2);

                    }
                }
                else if (equation.Contains("$")) //cos
                {
                    int index = find(equation, "$");

                    if (equation[index + 1] != "(" || equation[index + 1] != ")")
                    {
                        double answer = Math.Cos(Convert.ToDouble(equation[index + 1]));

                        equation = replaceAnswer(equation, answer, index, 2);

                    }
                }
                else if (equation.Contains("&")) //tan
                {
                    int index = find(equation, "&");

                    if (equation[index + 1] != "(" || equation[index + 1] != ")")
                    {
                        double answer = Math.Tan(Convert.ToDouble(equation[index + 1]));

                        equation = replaceAnswer(equation, answer, index, 2);

                    }
                }
                else if (equation.Contains("[")) //arcsin
                {
                    int index = find(equation, "[");

                    if (equation[index + 1] != "(" || equation[index + 1] != ")")
                    {
                        double answer = Math.Asin(Convert.ToDouble(equation[index + 1]));

                        equation = replaceAnswer(equation, answer, index, 2);

                    }
                }
                else if (equation.Contains("]")) //arccos
                {
                    int index = find(equation, "]");

                    if (equation[index + 1] != "(" || equation[index + 1] != ")")
                    {
                        double answer = Math.Acos(Convert.ToDouble(equation[index + 1]));

                        equation = replaceAnswer(equation, answer, index, 2);

                    }
                }
                else if (equation.Contains("{")) //arctan
                {
                    int index = find(equation, "{");

                    if (equation[index + 1] != "(" || equation[index + 1] != ")")
                    {
                        double answer = Math.Atan(Convert.ToDouble(equation[index + 1]));

                        equation = replaceAnswer(equation, answer, index, 2);

                    }
                }
                else if (equation.Contains("}")) //cosec
                {
                    int index = find(equation, "}");

                    if (equation[index + 1] != "(" || equation[index + 1] != ")")
                    {
                        double answer = 1 / Math.Sin(Convert.ToDouble(equation[index + 1]));

                        equation = replaceAnswer(equation, answer, index, 2);

                    }
                }
                else if (equation.Contains("'")) //sec
                {
                    int index = find(equation, "'");

                    if (equation[index + 1] != "(" || equation[index + 1] != ")")
                    {
                        double answer = 1 / Math.Cos(Convert.ToDouble(equation[index + 1]));

                        equation = replaceAnswer(equation, answer, index, 2);

                    }
                }
                else if (equation.Contains("@")) //cot
                {
                    int index = find(equation, "@");

                    if (equation[index + 1] != "(" || equation[index + 1] != ")")
                    {
                        double answer = 1/Math.Tan(Convert.ToDouble(equation[index + 1]));

                        equation = replaceAnswer(equation, answer, index, 2);

                    }
                }
                else if (equation.Contains("^"))
                {
                    int index = find(equation, "^");
                    if ((equation[index - 1] != "(" || equation[index - 1] != ")") && (equation[index + 1] != "(" || equation[index + 1] != ")"))
                    {
                        string toReplace = equation[index - 1] + equation[index] + equation[index + 1];
                
                        //bool isNegaive = false;
                        //if (Convert.ToDouble(equation[index - 1]) < 0)
                        //{
                        //    equation[index - 1] = Convert.ToString(0 - Convert.ToDouble(equation[index - 1]));
                        //    isNegaive = true;
                        //}
                
                        double answer = Math.Pow(Convert.ToDouble(equation[index - 1]), Convert.ToDouble(equation[index + 1]));
                
                        //if (isNegaive)
                        //    answer = -answer;
                
                
                        equation = replaceAnswer(equation, answer, index - 1, 3);
                
                    }
                }
                if (equation.Contains("/"))
                {
                    int index = find(equation, "/");

                    if ((equation[index - 1] != "(" || equation[index - 1] != ")") && (equation[index + 1] != "(" || equation[index + 1] != ")"))
                    {
                        string toReplace = equation[index - 1] + equation[index] + equation[index + 1];

                        double answer = Convert.ToDouble(equation[index - 1]) / Convert.ToDouble(equation[index + 1]);

                        equation = replaceAnswer(equation, answer, index - 1, 3);
                    }
                }
                else if (equation.Contains("*"))
                {
                    int index = find(equation, "*");

                    if ((equation[index - 1] != "(" || equation[index - 1] != ")") && (equation[index + 1] != "(" || equation[index + 1] != ")"))
                    {
                        string toReplace = equation[index - 1] + equation[index] + equation[index + 1];

                        double answer = Convert.ToDouble(equation[index - 1]) * Convert.ToDouble(equation[index + 1]);

                        equation = replaceAnswer(equation, answer, index - 1, 3);
                    }
                }
                else if (equation.Contains("-"))
                {
                    int index = find(equation, "-");

                    if ((equation[index - 1] != "(" || equation[index - 1] != ")") && (equation[index + 1] != "(" || equation[index + 1] != ")"))
                    {
                        string toReplace = equation[index - 1] + equation[index] + equation[index + 1];

                        double answer = Convert.ToDouble(equation[index - 1]) - Convert.ToDouble(equation[index + 1]);

                        equation = replaceAnswer(equation, answer, index - 1, 3);
                    }
                }
                else if (equation.Contains("+"))
                {
                    int index = find(equation, "+");

                    if ((equation[index - 1] != "(" || equation[index - 1] != ")") && (equation[index + 1] != "(" || equation[index + 1] != ")"))
                    {
                        string toReplace = equation[index - 1] + equation[index] + equation[index + 1];

                        double answer = Convert.ToDouble(equation[index - 1]) + Convert.ToDouble(equation[index + 1]);

                        equation = replaceAnswer(equation, answer, index - 1, 3);
                    }
                }
            }
            return (Convert.ToDouble(equation[0]));
        }

        private static int find(List<string> equation, string toFind)
        {

            //int index = 0;

            //finds a specific character in a list. It works the same as the indexOf function
            //MessageBox.Show(Convert.ToString(equation.Count - 1));
            for (int i = equation.Count - 1; i >= 0; i--)
            {
                if (equation[i] == toFind)
                    return i;
            }

            //foreach (string s in equation)
            //    if (s != toFind)
            //    {
            //        index++;
            //    }
            //    else
            //        return index;

            return -1;
        }

        //this subroutine will replace each individual equation with the answer
        private static List<string> replaceAnswer(List<string> equation, double answer, int startIndex, int howManyToRemove)
        {
            //removes all the indexes from the list that they need to remove
            for (int i = 0; i < howManyToRemove; i++)
                equation.RemoveAt(startIndex);

            //replaces it with the answer
            equation.Insert(startIndex, Convert.ToString(answer));

            return equation;
        }

        private static int countCharacters(List<string> equation, string toFind)
        {
            int counter = 0;
            foreach (string s in equation)
            {
                if (s == toFind)
                    counter++;
            }
            return counter;
        }
        #endregion

        #region stats

        public static void stats(List<double> items, Label output, Panel p)
        {   
            //sorts the algorithm in ascending order
            items = new List<double>(bubbleSort(items));

            double total = 0;
            double totalSquared = 0;
            int numberOfItems = items.Count;
            foreach (double d in items)
            {
                total += d;
                totalSquared += d * d;
            }

            double mean = total / numberOfItems;
            
            double median;

            //if there is an odd number of items, the median is the middle value, otherwise it's the mean of the two middle values
            if (numberOfItems % 2 == 1)
                median = items[numberOfItems / 2];
            else
                median = ((items[numberOfItems / 2] + items[numberOfItems / 2 - 1]) / 2);

            //mode is an array because you can have more than 1
            double[] mode = findMode(items);

            double range = items[items.Count - 1] - items[0];
            output.Text += "Range: " + range + "\n";

            //variance
            double variance = 0;
            foreach (double d in items)
                variance += ((d - mean) * (d-mean));

            variance = variance / (items.Count - 1);
            double standardDeviation = Math.Pow(variance, 0.5);


            double firstQuartile = 0, lastQuartile = 0;
            List<double> firstQuartileList = new List<double>();
            List<double> lastQuartileList = new List<double>();

            //If there are more then 2 items, then the first and third quartile can be foudn
            if (numberOfItems > 2)
            {
                //this splits the list of values into 2 lists in the middle
                if (numberOfItems % 2 == 0)
                {
                    firstQuartileList = items.GetRange(0, numberOfItems / 2);

                    lastQuartileList = items.GetRange(numberOfItems / 2, numberOfItems / 2);
                }
                else
                {
                    firstQuartileList = items.GetRange(0, (int)((numberOfItems / 2)));

                    lastQuartileList = items.GetRange((int)((numberOfItems / 2) + 1), (int)((numberOfItems / 2) + 0.5));
                }

                //the this will find the median of each half of the list of items the same way the median is found for the whole list
                int firstQuartileCount = firstQuartileList.Count;
                if (numberOfItems != 1)
                {
                    if (firstQuartileCount % 2 == 1)
                        firstQuartile = firstQuartileList[firstQuartileCount / 2];
                    else
                        firstQuartile = ((firstQuartileList[firstQuartileCount / 2] + firstQuartileList[firstQuartileCount / 2 - 1]) / 2);

                    int lastQuartileCount = lastQuartileList.Count;
                    if (lastQuartileCount % 2 == 1)
                        lastQuartile = lastQuartileList[lastQuartileCount / 2] + 1;
                    else
                        lastQuartile = ((lastQuartileList[lastQuartileCount / 2] + lastQuartileList[lastQuartileCount / 2 - 1]) / 2);
                }
                else
                {
                    lastQuartile = 0;
                    firstQuartile = 0;
                }
            }
            else
            {
                //if the number of items is less than or equal to 2, the quartiles are the first and last value
                firstQuartile = items[0];
                lastQuartile = items[items.Count - 1];
            }

            double interQuartileRange = lastQuartile - firstQuartile;

            //outputs the data calculated
            output.Text = "Averages:\n";
            if (mode == null)
            {
                output.Text += "Mode: N/A\n";
            }
            else
            {
                output.Text += "Mode: ";
                foreach (double d in mode)
                {
                    output.Text += d + ", ";
                }
                output.Text += "\n";
            }

            //char(931) is the character code for sigma, which means 'the sum of'
            output.Text += "x̄: " + mean
                + "\n" + (char)931 + "x: " + total
                + "\n" + (char)931 + "x²: " + totalSquared
                + "\nσ²: " + variance
                + "\nσ: " + standardDeviation
                + "\nn: " + numberOfItems
                + "\nMin(x): " + items[0]
                + "\nQ1: " + firstQuartile
                + "\nMed: " + median
                + "\nQ3: " + lastQuartile
                + "\nQ-Range: " + interQuartileRange
                + "\nMax(x): " + items[numberOfItems - 1]
                + "\nRange: " + range;

            //if there is only one item, there is no need to draw the box plot as it will be just a rectangle and serve no purpose
            if (numberOfItems != 1)
                drawBoxPlot(p, items[0], items[items.Count - 1], firstQuartile, lastQuartile, median, range);
            else
                MessageBox.Show("We need more information in order to draw a box plot", "Error: Cannot Draw Box Plot");
        }

        private static void drawBoxPlot(Panel p, double min, double max, double q1, double q2, double median, double range)
        {
            Graphics g = p.CreateGraphics();
            g.Clear(Color.White);

            Pen p1 = new Pen(Color.Black);
            //g.DrawLine(p1, 5, 0, 5, p.Width);

            //Min
            g.DrawLine(p1, 5, p.Width / 3, 5, (p.Width * 2) / 3);
            
            //Max
            g.DrawLine(p1, p.Width - 5, p.Width / 3, p.Width - 5, (p.Width * 2) / 3);

            double ratio = (p.Width - 10) / range;

            /*If the ratio is less than 1, there are more values than pixels. This means that some lines may merge. 
              This can be shown in the large data set screenshot where the lower quartile, upper quartile and median 
              are all drawn on the same pixel*/

            if (ratio < 1)
                MessageBox.Show("The produced box plot may not be accurate as the ratio between the pixels and values is too small. This means that if values are too close, the lines may merge.", "Warning");

            //MessageBox.Show("Ratio = " + ratio);

            //p1.Color = Color.Red;
            double q1Location = (q1 - min) * ratio + 5;
            g.DrawLine(p1, (float)q1Location, p.Width / 3, (float)q1Location, (p.Width * 2) / 3);
            

            //p1.Color = Color.Blue;
            double q2Location = (q2 - min) * ratio + 5;
            g.DrawLine(p1, (float)q2Location, p.Width / 3, (float)q2Location, (p.Width * 2) / 3);
            

            //p1.Color = Color.Green;
            double meanLocation = (median - min) * ratio + 5;
            g.DrawLine(p1, (float)meanLocation, p.Width / 3, (float)meanLocation, (p.Width * 2) / 3);

            //Connecting Quartile Box
            //p1.Color = Color.Black;
            g.DrawLine(p1, 5, p.Width / 2, (float)q1Location, p.Width / 2);
            g.DrawLine(p1, (float)q2Location, p.Width / 2, p.Width - 5, p.Width / 2);
            g.DrawLine(p1, (float)q1Location, p.Width / 3, (float)q2Location, p.Width / 3);
            g.DrawLine(p1, (float)q1Location, (p.Width * 2 / 3), (float)q2Location, (p.Width * 2) / 3);
        }

        public static double[] findMode(List<double> items)
        {
            //to find the mode, I created 2 lists of identical length. One will hold each value in the list, and the other will hold how many times it occurs
            List<double> allItems = new List<double>();
            List<int> count = new List<int>();
            foreach (double d in items)
            {
                if (!allItems.Contains(d))
                {
                    allItems.Add(d);
                    count.Add(1);
                }
                else if (allItems.Contains(d))
                {
                    count[allItems.IndexOf(d)] += 1;
                }
            }

            int max = 0; ;
            int firstValue = count[0];
            bool hasMode = false;
            //if all the digits occur the same amount of times, there is no mode
            for (int i = 0; i < count.Count; i++)
            {
                if (count[i] != firstValue)
                    hasMode = true;
            }

            //if it has a mode, it will find the largest value, if there is no mode, it will return nothing
            if (hasMode == true)
                for (int i = 0; i < count.Count; i++)
                {
                    if (count[i] > max)
                        max = count[i];
                }
            else
                return null;

            //calculates now many modes there are
            int numberOfModes = 0;
            foreach (double d in count)
                if (d == max)
                    numberOfModes++;


            int counter = 0;
            //creates an array with the number of modes the data set contains
            double[] mostCommon = new double[numberOfModes];
            //it will add the modes to an array which it will then return
            for (int i = 0; i < allItems.Count; i++)
            {
                if (count[i] == max)
                {
                    mostCommon[counter] = allItems[i];
                    counter++;
                }
            }

            return mostCommon;
        }

        public static List<double> bubbleSort(List<double> items)
        {
            int length = items.Count;
            double temp;
            //it will go through each item and sort them into ascending order
            for (int i = 0; i < length - 1; i++)
                for (int j = 0; j < length - 1 - i; j++)
                    if (items[j] > items[j + 1])
                    {
                        temp = items[j];
                        items[j] = items[j + 1];
                        items[j + 1] = temp;
                    }

            return items;
        }

        #endregion

        #region tables

        public static void tables(string[] equations, double min, double max, double step, string location)
        {
            int numberOfValues = (int)((max - min) / step);

            double[,] values = new double[equations.Length + 1, numberOfValues + 1];
            int currentIndex = 0;

            for (double x = min; x <= max; x = x + step)
            {
                values[0, currentIndex] = x;
                currentIndex++;
            }
            bool issue = false;
            for (int i = 1; i < equations.Length + 1; i++)
            {
                try
                {
                    List<string> currentEquation = new List<string>(initialise(equations[i - 1], new List<string>()));
                    currentIndex = 0;
                    if (currentEquation != null)
                    {
                        List<string> temp = new List<string>();
                        for (double x = min; x <= max; x += step)
                        {
                            temp = new List<string>(currentEquation);
                            for (int o = 0; o < temp.Count - 1; o++)
                                if (temp[o] == "x")
                                    temp[o] = temp[o].Replace("x", x.ToString());

                            double value = solvePreInitialised(temp);

                            values[i, currentIndex] = value;
                            currentIndex++;
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Something went wrong. Task is ending");
                    issue = true;
                    break;
                }
            }
            if (!issue)
                createCSV(equations, values, numberOfValues, location);
        }

        private static void createCSV(string[] equations, double[,] values, int numberOfValues, string location)
        {
            StreamWriter streamWriter = new StreamWriter(location);
            string firstLine = "x,";
            foreach (string s in equations)
                firstLine += s + ",";
            streamWriter.WriteLine(firstLine);

            for (int i = 0; i < numberOfValues + 1; i++)
            {
                string toWrite = values[0, i] + ",";
                for (int currentEquation = 1; currentEquation < equations.Length + 1; currentEquation++)
                {
                    toWrite += values[currentEquation, i] + ",";
                }
                streamWriter.WriteLine(toWrite);
            }
            streamWriter.Close();
            MessageBox.Show("Finished saving: " + location);
        }

        #endregion

        #region Simultaneous Equations

        //MODIFY Added conditions so that it shows what the lines are
        public static Label simultaneousEquations(int unknowns, Panel p, Label output)
        {
            switch (unknowns)
            {
                case 0:
                    //MessageBox.Show("2 Unknowns.");
                    //1,3,4,6,8,9
                    try
                    {
                        double a = Convert.ToDouble(p.Controls[1].Text);
                        double b = Convert.ToDouble(p.Controls[3].Text);
                        double c = Convert.ToDouble(p.Controls[4].Text);
                        double d = Convert.ToDouble(p.Controls[6].Text);
                        double e = Convert.ToDouble(p.Controls[8].Text);
                        double f = Convert.ToDouble(p.Controls[9].Text);
                        //MessageBox.Show(a + "\n" + b + "\n" + c + "\n" + d + "\n" + e + "\n" + f);

                        double x = (c * e - b * f) / (a * e - b * d);
                        double y = (c * d - a * f) / (b * d - a * e);

                        if (x.ToString() != "NaN" && y.ToString() != "NaN")
                            output.Text = "Result:\nx = " + x + "\ny = " + y;
                        else if (x.ToString() == "NaN" && y.ToString() == "NaN")
                            output.Text = "Result:\nEqual Lines";
                        else if (x.ToString().Contains("∞") || y.ToString().Contains("∞"))
                                output.Text = "Result:\nParallel Lines";
                    }
                    catch
                    {
                        MessageBox.Show("Something went wrong. Make sure all inputs are valid numbers.");
                    }
                    
                    break;
                case 1:
                    //MessageBox.Show("3 Unknowns.");

                    try
                    {
                        double a = Convert.ToDouble(p.Controls[1].Text);
                        double b = Convert.ToDouble(p.Controls[3].Text);
                        double c = Convert.ToDouble(p.Controls[5].Text);
                        double d = Convert.ToDouble(p.Controls[6].Text);
                        double e = Convert.ToDouble(p.Controls[8].Text);
                        double f = Convert.ToDouble(p.Controls[10].Text);
                        double g = Convert.ToDouble(p.Controls[12].Text);
                        double h = Convert.ToDouble(p.Controls[13].Text);
                        double i = Convert.ToDouble(p.Controls[15].Text);
                        double j = Convert.ToDouble(p.Controls[17].Text);
                        double k = Convert.ToDouble(p.Controls[19].Text);
                        double l = Convert.ToDouble(p.Controls[20].Text);
                        //MessageBox.Show(a + "\n" + b + "\n" + c + "\n" + d + "\n" + e + "\n" + f + "\n" + g + "\n" + h + "\n" + i + "\n" + j + "\n" + k + "\n" + l);

                        double z = ((d * e - a * h) * (f * i - e * j) - (h * i - e * l) * (b * e - a * f)) / ((c * e - a * g) * (f * i - e * j) - (b * e - a * f) * (g * i - e * k));

                        double y = ((g * i - e * k) * (d * e - a * h) - (c * e - a * g) * (h * i - e * l)) / ((g * i - e * k) * (b * e - a * f) - (c * e - a * g) * (f * i - e * l));

                        double x = ((d * g - h * c) * (f * k - j * g) - (h * k - l * g) * (b * g - f * c)) / ((a * g - e * c) * (f * k - j * g) - (e * k - i * g) * (b * g - f * c));

                        if (x.ToString() != "NaN" && y.ToString() != "NaN" && z.ToString() != "NaN")
                            output.Text = "Result:\nx = " + x + "\n\ny = " + y + "\n\nz = " + z;
                        else if (x.ToString() == "NaN" && y.ToString() == "NaN" && z.ToString() == "NaN")
                            output.Text = "Result:\nEqual Lines";
                        else if (x.ToString().Contains("∞") || y.ToString().Contains("∞") || z.ToString().Contains("∞"))
                            output.Text = "Result:\nParallel Lines";

                        
                    }
                    catch
                    {
                        MessageBox.Show("Something went wrong. Make sure all inputs are valid numbers.");
                    }
                    break;
                default:
                    MessageBox.Show("Error: Not valid number of unknowns");
                    break;
            }

            return null;
        }

        #endregion

        #region Polynomials

        //Fixed not showing roots because it was solved.Text = "x = " + e + ", ";
        public static void polynomialSolve(int terms, double a, double b, double c, double d, TextBox solved, Label differentiationLabel, Label integrationLabel, Label yInterceptLabel)
        {
            //checks the value of term. If its a 2, it is a cubic equation.If it's a 1, it is a quadratic.
            switch (terms)
            {
                case 1:
                    {
                        //This array will hold the return of the quadraticFormuula() method with b, c, d passed in
                        double[] values = quadraticFormula(b, c, d);
                        //clears the text of the solved textbox so that it can output the results without overlaying them ontop of what's already there
                        solved.Text = "";
                        //If a value isn't a real number, it will output NaN. This means that the 
                        //equation doesn't touch the x-axis OR it only touches it in one place if only one value is real
                        foreach (double e in values)
                            if (e.ToString() != "NaN")
                                solved.Text += "x = " + e + ", ";

                        //Ouputs the differnetial equation
                        differentiationLabel.Text = "Differentiation\n" + differentiatePolynomial(b, c, d, 1);

                        //Outputs the integral equation
                        integrationLabel.Text = "Integration\n" + integratePolynomial(b, c, d, 0, 1);

                        //Outputs the constant
                        yInterceptLabel.Text = "y-intercept\n" + d;
                        break;
                    }
                case 2:
                    {
                        //Firstly, it will find the first root
                        double firstRoot = findFirstRoot(a, b, c, d);
                        //The next step is to divide the equation by the root so we are left with a quadratic
                        double[] dividedEquation = dividePolynomial(a, b, c, d, firstRoot);

                        //the other two roots are calculated using the quadratic formual
                        double[] lastRoots = quadraticFormula(dividedEquation[0], dividedEquation[1], dividedEquation[2]);

                        //It will output the first root. This doesn't have a NaN condition because a cubic equation always crosses the x-axis once
                        solved.Text = "x = " + firstRoot + ", ";

                        foreach (double e in lastRoots)
                            if (e.ToString() != "NaN")
                                solved.Text += "x = " + e + ", ";

                        differentiationLabel.Text = "Differentiation\n" + differentiatePolynomial(a, b, c, 2);

                        integrationLabel.Text = "Integration\n" + integratePolynomial(a, b, c, d, 2);

                        yInterceptLabel.Text = "y-intercept\n" + d;


                        break;
                    }
            }
        }

        private static double[] quadraticFormula(double a, double b, double c)
        {
            double[] values = new double[2];
            values[0] = (-b + Math.Sqrt(Math.Pow(b, 2) - 4 * a * c)) / 2 * a;
            values[1] = (-b - Math.Sqrt(Math.Pow(b, 2) - 4 * a * c)) / 2 * a;
            return values;
        }

        private static string differentiatePolynomial(double a, double b, double c, int terms)
        {
            switch (terms)
            {
                case 1:
                    {
                        if (b > 0)
                            return 2 * a + "x + " + b;
                        else
                            return 2 * a + "x -" + (-b);
                    }
                case 2:
                    {
                        return 3 * a + "x^2 + " + 2 * b + "x + " + c;
                    }
            }
            return "Error";
        }

        //increase power by one, divide by new power
        private static string integratePolynomial(double a, double b, double c, double d, int term)
        {
            switch (term)
            {
                case 1:
                    {
                        return a / 3 + "x^3 + " + b / 2 + "x^2 + " + c + "x + c";
                    }
                case 2:
                    {
                        return a / 4 + "x^4 + " + b / 3 + "x^3 + " + c / 2 + "x^2 + " + d + "x + d";
                    }
            }
            return "Error";
        }

        private static double findFirstRoot(double a, double b, double c, double d)
        {
            //notFound will be the value to store if a factor has been found
            bool notFound = true;
            double max = 1000, min = -1000;
            double valueToSearch = (max + min) / 2;
            double firstFactor = 0;
            int numberOfTries = 0;

            List<string> equation = initialise(a + "*x^3 +" + b + "*x^2 +" + c + "x +" + d, new List<string>());
            List<string> temp = new List<string>(equation);

            while (notFound == true)
            {
                //It will reasign the value of equation back to the origional equation and not the modified version
                equation = new List<string>(temp);
                //This for loop is resposible for replacing x with valueToSearch
                for (int i = 0; i < equation.Count - 1; i++)
                    if (equation[i] == "x")
                        equation[i] = Convert.ToString(valueToSearch);
                //It will solve the equation and store the value in answer
                double answer = solvePreInitialised(equation);

                //If the output of a quadratic is 0, the value of x is a factor
                //I am rounding the values so that it doesn't search for an exact value because 
                //there may not be one in the maximum decimal places a double can have, which will make it loop forever
                if (Math.Round(answer, 5) == 0)
                {
                    notFound = false;
                    firstFactor = Math.Round(valueToSearch, 5);
                }
                //If it's greater than 0, we know that the maximum is too big, 
                //so we set it to the valueToSearch as it doesn't need to check anything between max and itself 
                else if (Math.Round(answer, 5) > 0)
                    max = valueToSearch;
                //If it's less than 0, we know that the minimum is too big, 
                //so we set it to the valueToSearch as it doesn't need to check anything between min and itself 
                else if (Math.Round(answer, 5) < 0)
                    min = valueToSearch;

                //MessageBox.Show(answer.ToString());

                //This will be the middle of the two values
                valueToSearch = (max + min) / 2;

                //This is the maximum number of attempts the program is allowed to make before it will no longer search for solutions
                numberOfTries++;
                if (numberOfTries == 1000000)
                {
                    break;
                }
            }

            //If it ran out of attempts, it will display an error message and it will not try to find the other 2 factors
            if (notFound == false)
            {
                return valueToSearch;
            }
            else
            {
                MessageBox.Show("Error: Unable to find a factor. This may be because the factor(s) are outside the applicable range.", "Error");
            }
            //Returns the polynomial it has identified it as
            return 0;
        }

        private static double[] dividePolynomial(double d, double e, double f, double g, double divideBy)
        {
            //see diagram for mathematical proof
            double a = d, b, c;

            if (divideBy != 0)
                c = g / -divideBy;
            else
                c = f;

            b = e + a * divideBy;

            //Rounds all the values to 5 decimal places for consistency
            a = Math.Round(a, 5);
            b = Math.Round(b, 5);
            c = Math.Round(c, 5);

            //Displays result for debugging
            //MessageBox.Show("(" + d + "x^3 + " + e + "x^2 + " + f + "x + " + g + ")/(x-" + z +") = " +a + "x^2 + " + b + "x + " + c, "Division");
            //Returns the coefficients
            double[] quadratic = { a, b, c };
            return quadratic;
        }

        #endregion

        #region Number Conversions

        public static string ConvertNumber(string number, int to, int from)
        {
            //Convert to decimal
            long decimalNumber = 0;
            int startAt = number.Length - 1;
            foreach (char c in number)
            {
                string s = "" + c;
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
                        currentNumber = Convert.ToInt32(s);
                        break;
                }
                int toAdd = currentNumber * (int)Math.Pow(from, startAt);
                //MessageBox.Show(currentNumber + " * " + from + "^" + startAt + " = " + toAdd);
                decimalNumber = decimalNumber + toAdd;
                startAt--;
            }

           // MessageBox.Show(number + " in base " + from + " is " + decimalNumber + " in base 10");
            
            //Convert from decimal
            string output = "";
            while (decimalNumber > 0)
            {
                long remainder = decimalNumber % to;
                decimalNumber = (long)(decimalNumber / to);

                switch (remainder)
                {
                    case 10:
                        output = "A" + output;
                        break;
                    case 11:
                        output = "B" + output;
                        break;
                    case 12:
                        output = "C" + output;
                        break;
                    case 13:
                        output = "D" + output;
                        break;
                    case 14:
                        output = "E" + output;
                        break;
                    case 15:
                        output = "F" + output;
                        break;
                    default:
                        output = remainder + output;
                        break;
                }
            }
            //MessageBox.Show(originalNumber + " in base " + from + " is " + output + " in base " + to);
            return output;
        }

        #endregion

        #region Integration

        public static void SimpsonsRule(Label output, string equation, decimal start, decimal end)
        {
            const decimal changeInX = 0.005M;
            List<string> equationList = initialise(equation, new List<string>());
            List<decimal> yS = new List<decimal>();

            //gets all y values and adds them to a list
            for (decimal x = start; x <= end; x += changeInX)
            {
                List<string> toSolve = new List<string>(equationList);

                for (int i = 0; i < equationList.Count - 1; i++)
                    if (toSolve[i] == "x")
                        toSolve[i] = x.ToString();

                decimal yCoordinate = Convert.ToDecimal(solvePreInitialised(toSolve));

                yS.Add(yCoordinate);
            }
            decimal sum = 0;

            //this works out the brackts inside the formula
            for (int i = 1; i < yS.Count - 1; i++)
            {
                if (i % 2 == 0)
                    sum += 2 * yS[i];
                else
                    sum += 4 * yS[i];
            }

            //adds the starting and last value as they are not multiplied by anything.
            sum += yS[0] + yS[yS.Count - 1];
            sum = (changeInX / 3) * sum;

            output.Text = "Result:\n\nArea Under Curve:\n" + Math.Round(sum, 8);
        }

        #endregion
    }
}