using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CSharpClassViewer
{
    public class CSharpFile
    {
        private string fileContents;

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
            if (!fileContents.Contains(Environment.NewLine, StringComparison.CurrentCulture))
            {
                MessageBox.Show("No End of line in file " + filename);
                MessageBox.Show("Find --> " + fileContents);
                return false;
            }
            return true;
        }
    }
}
