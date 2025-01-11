using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CSharpClassViewer
{
    public class CSharpFile
    {
        private string? fileContents;

        public bool Load(string filename)
        {
            TextReader reader;
            fileContents = "";

            try
            {
                reader = new StreamReader(filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            try
            {
                fileContents = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }
        public void Parse ()
        {
            string? line = "";
            using StringReader reader = new(fileContents);
            while ((line = reader.ReadLine()) != null)
            {
                line = Regex.Replace(line, @"\s+", " ");
                line = line.Trim();
                string[] tokens = line.Split(' ');
            }
        }
    }
}
