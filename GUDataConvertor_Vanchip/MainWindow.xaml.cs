using System;
using System.Windows;
using System.Windows.Forms;
using System.IO;

namespace GUDataConvertor_Vanchip
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();

                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtGUDataFile.Text = dlg.FileName;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("System error: " + ex.Message);
            }
        }

        private void btnConvert_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string vcPath = txtGUDataFile.Text;
                if (!File.Exists(vcPath))
                {
                    System.Windows.MessageBox.Show("File does not exist, " + vcPath + "!");
                    return;
                }
                string mtPath = Path.Combine(Path.GetDirectoryName(vcPath), "GUData_" + Path.GetFileNameWithoutExtension(vcPath)+".csv");
                if (File.Exists(mtPath))
                {
                    System.Windows.MessageBox.Show("File has already been exist, " + mtPath + "!");
                    return;
                }
                using (FileStream fsWriter = new FileStream(mtPath, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    using (StreamWriter sw = new StreamWriter(fsWriter))
                    {
                        using (FileStream fsReader = new FileStream(vcPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            using (StreamReader sr = new StreamReader(fsReader))
                            {
                                string line = string.Empty;
                                string lineData = string.Empty;
                                bool limitStart = false;
                                bool dataStart = false;
                                while ((line = sr.ReadLine()) != null)
                                {
                                    if (line.Split(',')[0].ToUpper() == "PRODUCT")
                                    {
                                        sw.WriteLine(line);
                                        continue;
                                    }

                                    if (line.Split(',')[0].ToUpper() == "TEST PLAN")
                                    {
                                        sw.WriteLine(line);
                                        continue;
                                    }

                                    if (line.Split(',')[0].ToUpper() == "RELEASE DATE")
                                    {
                                        sw.WriteLine(line);
                                        continue;
                                    }

                                    if (line.Split(',')[0].ToUpper() == "COMMENT")
                                    {
                                        sw.WriteLine(line);
                                        sw.WriteLine();
                                        continue;
                                    }

                                    if (line.Split(',')[0].ToUpper() == "DEVICE#")
                                    {
                                        var cell = line.Split(',');
                                        string lineTestNumber = "Device #,Site,X-coordinate,Y-Coordinate,Pass Fail,Hard Bin,Soft Bin,Test Time,Test Number";
                                        for (int i = 3; i < cell.Length; i++)
                                        {
                                            lineTestNumber += "," + (i - 2).ToString();
                                        }
                                        sw.WriteLine(lineTestNumber);
                                        limitStart = true;
                                        continue;
                                    }

                                    if (limitStart)
                                    {
                                        var cell = line.Split(',');
                                        string lineTestName = ",,,,,,,,Test Name";
                                        for (int i = 3; i < cell.Length; i++)
                                        {
                                            lineTestName += "," + cell[i];
                                        }
                                        sw.WriteLine(lineTestName);
                                        sw.WriteLine();
                                        sr.ReadLine();
                                        sr.ReadLine();
                                        sr.ReadLine();
                                        limitStart = false;
                                        dataStart = true;
                                        continue;
                                    }

                                    if (dataStart)
                                    {
                                        var cell = line.Split(',');
                                        for (int i = 0; i < cell.Length; i++)
                                        {
                                            if (i == 0)
                                            {
                                                lineData = cell[i];
                                            }
                                            else if (i == 2)
                                            {
                                                lineData += ",Xcor,Ycor,Pass,0,0,0,";
                                            }
                                            else
                                            {
                                                lineData += "," + cell[i];
                                            }
                                        }
                                        sw.WriteLine(lineData);
                                    }
                                }
                            }
                        }
                    }
                }
                System.Windows.MessageBox.Show("GU data has been converted!");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("System error: " + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
