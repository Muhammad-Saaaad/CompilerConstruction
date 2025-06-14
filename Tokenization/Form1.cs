using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Tokenization
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public Dictionary<String, String> dtype_var; 
        public Dictionary<String, String> var_value;

        public bool add_word(String first_word, String datatype) // first_word = "a = 10 | a" here you must have a data type
        {
            if (first_word.Contains("=")) // means inilization "num a = 10"
            {
                var variable = first_word.Split('=')[0].Trim(); // variable name
                var value = first_word.Split('=')[1].Trim(); // variable value

                if (dtype_var.ContainsKey(variable)) // if the variable already exists
                {
                    lbValid.Text = "Build Failed"; // set the label to build failed
                    MessageBox.Show($"Variable {variable} already exists: ");
                    return false;
                }

                dtype_var[variable] = datatype; // add to dtype_var dictionary
                var_value[variable] = value; // add to var_value

                listBox1.Items.Add(variable);
                listBox1.Items.Add("="); // add equal sign
                listBox1.Items.Add(value);

                return true;
            }
            else // only deleration "num a" 
            { 
                var variable = first_word.Trim(); // variable name

                if (dtype_var.ContainsKey(variable)) // if the variable already exists
                {
                    lbValid.Text = "Build Failed"; // set the label to build failed
                    MessageBox.Show($"Variable {variable} already exists: ");
                    return false;
                }

                dtype_var[variable] = datatype; // add to dtype_var dictionary
                var_value[variable] = null; // add to memory with null value

                listBox1.Items.Add(variable);

                return true;
            }
        }

        public void add_memory(String line)
        {
            line = line.Trim(';');

            String[] words = line.Split(new char[] { ',' });

            String dtype = words[0].Split(' ')[0];

            listBox1.Items.Add(dtype);

            if (words.Length == 1) // single inilization or decleration
            {
                var sub_words = string.Join(" ", words[0].Split(' ').Skip(1)); // got the first value

                if(!add_word(sub_words, dtype))
                {
                    listBox1.Items.RemoveAt(listBox1.Items.Count - 1); // if the variable already exists then remove the data type from listbox
                }

            }
            else // Multiple words
            {
                var sub_words = string.Join(" ", words[0].Split(' ').Skip(1));
                
                if (!add_word(sub_words, dtype))
                {
                    listBox1.Items.RemoveAt(listBox1.Items.Count - 1); // if the variable already exists then remove the data type from listbox
                }

                foreach (var word in words.Skip(1))
                {
                    add_word(word, dtype);
                }
            }
        }

        public  Boolean type_validation(String variable, String value) // used only when inilization "a=10;"
        {
            try
            {
                if (dtype_var[variable] == "num")
                {
                    // get the data type form data_var dictionary if its integer then insert
                    if (Regex.IsMatch(value, @"^\d+$")) // if it is integer
                    {
                        var_value[variable] = value;
                        return true;
                    }
                }
                else if (dtype_var[variable] == "float" || dtype_var[variable] == "decimal")
                {
                    if (Regex.IsMatch(value, @"^[0-9]*[0-9]*[.]?[0-9][0-9]*$"))
                    {
                        var_value[variable] = value;
                        return true;
                    }
                }
                else if (dtype_var[variable] == "text")
                {
                    if (Regex.IsMatch(value, @"^""[^""]*""$"))
                    {
                        var_value[variable] = value;
                        return true;
                    }
                }
                else if (dtype_var[variable] == "char")
                {
                    if (Regex.IsMatch(value, @"^'[^""]'$"))
                    {
                        var_value[variable] = value;
                        return true;
                    }
                }
                else if (dtype_var[variable] == "bool")
                {
                    if (Regex.IsMatch(value, @"^(true|false)$"))
                    {
                        var_value[variable] = value;
                        return true;
                    }
                }

                MessageBox.Show("Type Mismatch:  " + value);
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return false;
        }

        public string get_input(string input_str)
        {
            var extractedText = input_str.Split('"')[1];
            string userInput = Microsoft.VisualBasic.Interaction.InputBox(
            extractedText,              // Message
            "Input Required",           // Title
            "",                         // Default text in input
            -1, -1);                    // X and Y position (-1 for center)

            return userInput;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            String input = richTextBox1.Text;
            input = input.Trim();

            var_value = new Dictionary<string, string>();
            dtype_var = new Dictionary<string, string>();

            listBox1.Items.Clear();

            //All initialization single
            Regex ILine = new Regex(@"^[_a-zA-Z][_a-zA-Z0-9]*\s*=\s*((""[^""]*"")|('[^']')|([0-9]*[0-9]*[.]?[0-9][0-9]*)|(true|false))\s*;$");

            //int initialization and declaration single and multiple
            Regex IntIdLine = new Regex(@"^num\s+([_a-zA-Z][_a-zA-Z0-9]*\s*(=\s*\d+)?\s*)(,\s*[_a-zA-Z][_a-zA-Z0-9]*\s*(=\s*\d+)?\s*)*;$");
            
            //float | decimal initialization and declaration single and multiple
            Regex FloatIdLine = new Regex(@"^(float|decimal)\s+([_a-zA-Z][_a-zA-Z0-9]*\s*(=\s*\d+(\.\d+)?\s*)?)(\s*,\s*[_a-zA-Z][_a-zA-Z0-9]*\s*(=\s*\d+(\.\d+)?\s*)?)*\s*;$");

            // text(String) initialization and declaration single and multiple

            Regex StringIdLine = new Regex(@"^text\s+([_a-zA-Z][_a-zA-Z0-9]*\s*(=\s*""[^""]*""\s*)?)(\s*,\s*[_a-zA-Z][_a-zA-Z0-9]*\s*(=\s*""[^""]*""\s*)?)*\s*;$");
            
            // char initialization and declaration single and multiple

            Regex CharIdLine = new Regex(@"^char\s+([_a-zA-Z][_a-zA-Z0-9]*\s*(=\s*'[^']'\s*)?)(\s*,\s*[_a-zA-Z][_a-zA-Z0-9]*\s*(=\s*'[^']'\s*)?)*\s*;$");
            
            // boolean initialization and declaration single and multiple

            Regex BoolIdLine = new Regex(@"^bool\s+([_a-zA-Z][_a-zA-Z0-9]*\s*(=\s*(true|false)\s*)?)(\s*,\s*[_a-zA-Z][_a-zA-Z0-9]*\s*(=\s*(true|false)\s*)?)*\s*;$");



            //"print string" 
            Regex printSLine = new Regex(@"^print\s*\(\s*(?:""[^""]*""|\w+||\([^()]*\))(?:\s*[\+\-\*/%]\s*(?:""[^""]*""|\w+|\([^()]*\)))*\s*\)\s*;$");

            //"input string"
            Regex inputLine = new Regex(@"^[_A-z][_A-z0-9]*\s*=\s*input\s*\(\s*""[\w\s=:]*""\s*\)\s*;$");

            string[] Input = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            richTextBox2.Text = "";

            foreach (var line_ in Input)
            {
                // decimal a, b1=1.2,b2=4.2;

                var line = line_.Trim(); // line_ can have a space after ";" so i remove it here

                // for initialization and declaration of any data type

                if (IntIdLine.IsMatch(line) || FloatIdLine.IsMatch(line) || StringIdLine.IsMatch(line) 
                    || CharIdLine.IsMatch(line) || BoolIdLine.IsMatch(line))
                {
                    add_memory(line); // add to memory

                    if (lbValid.Text != "Build Failed") // if the build is not failed then set the label to build successfull
                    {
                        lbValid.Text = "Build Sucessfull";
                    }
                    else // build failed stop the process
                    {
                        break;
                    }
                }

                else if (ILine.IsMatch(line)) // Inilization all 
                {

                    string[] Iwords = line.Split('=');

                    // checking if the variable exists or not, if yes then check the type of input and variable

                    var variable = Iwords[0].Trim();

                    var value = Iwords[1].Trim(new Char[] { ';' }).Trim();

                    if (type_validation(variable, value) == true){
                        listBox1.Items.Add(variable + "\n"); // variable added

                        listBox1.Items.Add("=" + "\n"); // equal added

                        listBox1.Items.Add(value + "\n"); // value added

                        lbValid.Text = "Build Sucessfull";
                    }
                    else
                    {
                        lbValid.Text = "Build Failed";
                    }
                    

                }
                else if (printSLine.IsMatch(line)) // print 
                {

                    foreach (var stmt in line)
                    {
                        var inner = ExtractPrintContent(stmt);
                        var output = EvaluateExpression(inner, var_value);
                        Console.WriteLine(output);
                    }

                    //string[] coutwords = line.Split('"'); 

                    //listBox1.Items.Add("print" + "\n");

                    //listBox1.Items.Add(coutwords[1].Trim() + "\n");

                    //lbValid.Text = "Build Sucessfull";

                }

                else if (inputLine.IsMatch(line)) // input issue!
                {
                    bool got_inp = false;
                    var words = line.Split('=');

                    if (words.Length > 2)
                    {
                        for (int i =2; i < words.Length; i++)
                        {
                            words[1] = words[1] + "=" +words[i];
                        }
                    }

                    foreach (var variable in dtype_var.Keys)
                    {
                        if (variable == words[0].Trim()) // check is the variable exist or not
                        {
                            // if exists then check its type and then assign the value accordingly

                            if (dtype_var[variable] == "num" || dtype_var[variable] == "float" || dtype_var[variable] == "decimal" ||
                                dtype_var[variable] == "char" || dtype_var[variable] == "bool")
                            {
                                String userInput = get_input(words[1]);

                                if (userInput == "") // if no input is entered then return error
                                {
                                    lbValid.Text = "Build Failed";
                                    break;
                                }

                                if (type_validation(variable, userInput) == true) //  check if the user input is correct and then add input
                                {
                                    listBox1.Items.Add(userInput);
                                    got_inp = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (got_inp == true)
                    {
                        lbValid.Text = "Build Sucessfull";
                    }
                    else
                    {
                        lbValid.Text = "Build Failed";
                    }
                }
                else
                {
                    lbValid.Text = "Build Failed";
                    break;
                }
            }

            if (lbValid.Text == "Build Sucessfull")
            {
                lbValid.ForeColor = Color.DarkGreen;
            }
            else
            {
                lbValid.ForeColor = Color.DarkRed;
            }

            lbValid.Visible = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        // 1) grab what's inside print(...)
        static string ExtractPrintContent(string s)
        {
            var m = Regex.Match(s, @"^print\s*\(\s*(.*)\s*\)\s*;$");
            return m.Success ? m.Groups[1].Value : "";
        }

        // 2) split top‑level on '+' only (depth==0)
        static List<string> SplitOnTopLevelPlus(string expr)
        {
            var parts = new List<string>();
            int depth = 0, last = 0;
            for (int i = 0; i < expr.Length; i++)
            {
                char c = expr[i];
                if (c == '(') depth++;
                else if (c == ')') depth--;
                else if (c == '+' && depth == 0)
                {
                    parts.Add(expr.Substring(last, i - last));
                    last = i + 1;
                }
            }
            parts.Add(expr.Substring(last));
            return parts;
        }

        // 3) evaluate each segment, then concatenate results
        static string EvaluateExpression(string expr, Dictionary<string, object> vars)
        {
            expr = expr.Trim();
            if (string.IsNullOrEmpty(expr))
                return "";

            //expr = expr.Replace("\\", "/");

            // replace variables with their values (strings get quoted)
            foreach (var kvp in vars)
            {
                string pat = $@"\b{kvp.Key}\b";
                string val = kvp.Value is string ? $"\"{kvp.Value}\"" : kvp.Value.ToString();
                expr = Regex.Replace(expr, pat, val);
            }

            var segments = SplitOnTopLevelPlus(expr);
            string result = "";

            foreach (var seg in segments)
            {
                var t = seg.Trim();
                // is it a double‑quoted string?
                if (t.StartsWith("\"") && t.EndsWith("\""))
                {
                    result += t.Substring(1, t.Length - 2);
                }
                else
                {
                    // otherwise treat as math and compute
                    var table = new DataTable();
                    try
                    {
                        var val = table.Compute(t, "");
                        result += val.ToString();
                    }
                    catch
                    {
                        result += "[Error]";
                    }
                }
            }

            return result;
        }
    }
}
