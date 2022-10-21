using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stefan_Ljubinkovic_PZ1
{
    public class LoadXMLs
    {

        public static string[] getXmls()
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {


                DialogResult result = fbd.ShowDialog();
           
                string[] files;


                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    files = Directory.GetFiles(fbd.SelectedPath);
                    return files;
                }
               
            }
            return null;


        }
    }
}
