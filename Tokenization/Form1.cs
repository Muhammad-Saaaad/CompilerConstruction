using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
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

        public void add_word(String first_word, String datatype)
        {
            if (first_word.Contains("=")) // means inilization
            {
                var variable = first_word.Split('=')[0].Trim(); // variable name
                var value = first_word.Split('=')[1].Trim(); // variable value

                dtype_var[variable] = datatype; // add to dtype_var dictionary
                var_value[variable] = value; // add to var_value

                listBox1.Items.Add(variable);
                listBox1.Items.Add(value);
            }
            else // only deleration
            {
                var variable = first_word.Trim(); // variable name
                dtype_var[variable] = datatype; // add to dtype_var dictionary
                var_value[variable] = null; // add to memory with null value

                listBox1.Items.Add(variable);
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

                add_word(sub_words, dtype);

            }
            else // Multiple words
            {
                var sub_words = string.Join(" ", words[0].Split(' ').Skip(1));
                add_word(sub_words, dtype);

                foreach (var word in words.Skip(1))
                {
                    add_word(word, dtype);
                }
            }
        }

        public void draft_regex()
        {
            //float declaration 
            //Regex FloatdLine = new Regex("^float [a-zA-Z][a-zA-Z0-9]*[' ']*;$");

            //float  declaration multiple 
            //Regex FloatdMLine = new Regex("^int [a-zA-Z][a-zA-Z0-9]*[,][a-zA-Z][a-zA-Z0-9]*[' ']*[,' 'a-zA - Z0 - 9']*;$"); 

            //int declaration 
            //if (IntdLine.IsMatch(line))
            //{
            //    String[] sub = words[0].Split(',');

            //    foreach (string s in sub)
            //    {
            //        if (s == "")
            //        {
            //            break;
            //        }
            //        listBox1.Items.Add(s + "\n");
            //    }
            //    listBox1.Items.Add(words[1].Trim(new Char[] { ';' }) + "\n");
            //}

            //float declaration
            //if (FloatdLine.IsMatch(line))
            //{
            //    listBox1.Items.Add(words[0] + "\n"); listBox1.Items.Add(words[1].Trim(new Char[] { ';' }) + "\n");
            //}

            // declaration single
            //^(num | decimal | float | bool | char | text)\s + [_a - zA - Z][_a - zA - Z0 - 9] * [' '] *;$

            ////for Multiple declaration 
            //if (IntdMLine.IsMatch(line))
            //{
            //    string[] last = words[words.Length - 1].Split(';');
            //    foreach (var w in words[0].Split(','))
            //    {
            //        string[] Iwords = w.Split('='); 
            //        //listBox1.Items.Add(Iwords[0].Trim(new Char[] { ';' }) + "\n");
            //        listBox1.Items.Add(Iwords[0] + "\n");
            //        listBox1.Items.Add("=" + Iwords[1]);
            //    }
            //    listBox1.Items.Add(last[0] + "\n");
            //}

            //for Multiple declaration 
            //if (FloatdMLine.IsMatch(line))
            //{
            //    listBox1.Items.Add(words[0] + "\n");
            //    foreach (var w in words[1].Split(','))
            //    {
            //        string[] Iwords = w.Split('='); listBox1.Items.Add(Iwords[0].Trim(new Char[] { ';' }) + "\n");
            //    }
            //}

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
            Regex printSLine = new Regex(@"^print\s*\(\s*""[a-zA-Z0-9' '\=][a-zA-Z0-9' '\=]*""\s*\)\s*;$");

            //"input string"
            Regex inputLine = new Regex(@"^[_A-z][_A-z0-9]*\s*=\s*input\s*\(\s*""[\w\s]*""\s*\)\s*;$");

            string[] Input = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            richTextBox2.Text = "";

            foreach (var line in Input)
            {
                // decimal a, b1=1.2,b2=4.2;

                // for initialization and declaration of any data type

                if (IntIdLine.IsMatch(line) || FloatIdLine.IsMatch(line) || StringIdLine.IsMatch(line) 
                    || CharIdLine.IsMatch(line) || BoolIdLine.IsMatch(line))
                {
                    add_memory(line); // add to memory

                    lbValid.Text = "Build Sucessfull";
                }

                else if (ILine.IsMatch(line)) // Inilization all 
                {

                    string[] Iwords = line.Split('='); 
                    listBox1.Items.Add(Iwords[0].Trim() + "\n");

                    listBox1.Items.Add("=" + "\n");

                    listBox1.Items.Add(Iwords[1].Trim(new Char[] { ';' }) + "\n");

                    lbValid.Text = "Build Sucessfull";

                }
                else if (printSLine.IsMatch(line)) // print 
                {
                    string[] coutwords = line.Split('"'); 

                    listBox1.Items.Add("print" + "\n");

                    listBox1.Items.Add(coutwords[1].Trim() + "\n");

                    lbValid.Text = "Build Sucessfull";
                    
                }

                else if (inputLine.IsMatch(line)) // input
                {
                    var words = line.Split('=');
                    listBox1.Items.Add(words[0].Trim() + "\n");

                    Input inp = new Input();
                    
                    

                    listBox1.Items.Add(words[1].Trim(';').Trim(' '));

                    lbValid.Text = "Build Sucessfull";
                }
                else
                {
                    lbValid.Text = "Build Failed";
                    break;
                }
            }
            lbValid.Visible = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
