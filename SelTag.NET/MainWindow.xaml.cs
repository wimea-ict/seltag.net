using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.IO;
using System.Collections.ObjectModel;
using GenericUtilities;


namespace SelTag.NET
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Row> rows = null;
        SDataTable sdt = null;
        public MainWindow()
        {
            InitializeComponent();
            rows = new List<Row>();
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "DATA Files|*.dat;*.txt";
            int count = 0;
            if (ofd.ShowDialog(this) == true)
            {
               // try
                {                 
                    StreamReader reader = new StreamReader( ofd.FileName);
                    string rowLine = "";
                    while ((rowLine = reader.ReadLine()) != null)
                    {
                        if (rowLine.Contains("TXT") && rowLine.Contains("TZ") && !rowLine.Contains("ERR"))
                        {
                            rows.Add(new Row(rowLine));
                            count++;
                        }
                    }
                    MessageBox.Show("Process complete. " + count + " records processed", "COMPLETE",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    lblName.Content = ofd.FileName + "  -  " + count + " records";
                }
               /* catch (Exception ex)
                {
                   MessageBox.Show("An error occured.\n DETAILS: " + ex.Message, "ERROR",MessageBoxButton.OK, MessageBoxImage.Error);
                }   */
            }

            List <string> txtTagNames = new List<string>();

            count = 0;
            foreach (Row row in rows)
            {
                IEnumerable<Tag> tags = (from myTags in row.Tags.AsEnumerable()
                                         where myTags.Name.Contains("TXT")
                                         select myTags);
                if (tags.Count() > 0)
                {
                    string txtTag = tags.ElementAt(0).Value.ToString();
                    txtTagNames.Add(txtTag);
                }         
                ++count;
                if (count == 5000)  /*not-so-good programming! Assumption: by 5000 records, 
                                     * all nodes must have sent atleast one value  */
                {
                    break;
                }     
            }
            txtTagNames = txtTagNames.Distinct().ToList();
            cbxTxt.ItemsSource = new ObservableCollection<string>(txtTagNames);
        }

        private void cbxTxt_DropDownClosed(object sender, EventArgs e)
        {
            string selection = cbxTxt.Text;
            var result = from myRows in rows.AsEnumerable()
                         where myRows.Tags.Contains <Tag> (new Tag("TXT", selection), new TagComparer())
                         select myRows;

            List<string> tagNames = new List<string>();
            foreach (Row row in result)
            {
                tagNames.AddRange((from myTags in row.Tags.AsEnumerable()
                                   select myTags.Name).ToList());
            }
            tagNames = tagNames.Distinct().ToList();

            sdt= new SDataTable(tagNames);

            foreach (Row row in result)
            {
                sdt.AppendRow(row); //different implementation of AppendRow needed. Match columns. to do
            }

            dtGridResults.ItemsSource = sdt.DefaultView;
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Filter = "CSV File|*.csv";
            if (sfd.ShowDialog(this) == true)
            {
                try
                {
                    if (sdt.Rows.Count > 0)
                    {
                        Utilities.WriteCSVFile(sdt, sfd.FileName + ".csv");
                    }
                    else
                        MessageBox.Show("There are no rows to export.", "ERROR");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occured.\n DETAILS: " + ex.Message, "ERROR");
                }
            }

        }

        private void menuCrossC_Click(object sender, RoutedEventArgs e)
        {
           /* double[] times = new double[sdt.Rows.Count-1];
            double[] stimes = new double[10];
            for (int i = 0; i < sdt.Rows.Count-1; i++ )
            {
               int val = Convert.ToDateTime(sdt.Rows[i+1][0]).Second - Convert.ToDateTime(sdt.Rows[i][0]).Second;
               if (val < 0) val += 60;

               times[i] = val;
            }
            for (int i = 0; i < 10; i++)
            {
                stimes[i] = times[i];
            }

            //cross correlate
            int[] res = CEDAT.MathLab.SignalProcessor.CrossCorrelation(times, stimes, 9);
            Utilities.ToFile(res, "sample.csv", "", 1);*/
        }

        private void menuGetStat_Click(object sender, RoutedEventArgs e)
        {
            string[] parameters = { "T", "T1", "V_MCU", "V_IN", "V_A1", "T_SHT2X", "RH_SHT2X", "AH", "T_DEW",
                                    "T_HEAT_IDX", "V_A3", "P_MS5611", "P0_T", "V_A1", "V_A2", "V_AD1", "V_AD2", "V_AD4" };
            StreamWriter sw = new StreamWriter("stat-" + cbxTxt.Text + ".csv");

            foreach (DataColumn col in sdt.Columns)
            {
                if(parameters.Contains<string>(col.ColumnName))
                {
                    List<double> list = new List<double>();
                    foreach(DataRow row in sdt.Rows)
                    {
                        if (row[col] != DBNull.Value && row[col].ToString() != "")
                        {
                            list.Add(Convert.ToDouble(row[col]));
                        }
                    }
                    StringBuilder line = new StringBuilder();
                    line.Append(col.ColumnName);
                    line.Append(",");
                    double min = list.Min<double>();
                    line.Append(min);
                    line.Append(",");
                    string date = "";
                    foreach(DataRow row in sdt.Rows)
                    {
                        if (row[col] != DBNull.Value && row[col].ToString()!="")
                        {
                            if (Convert.ToDouble(row[col]) == min)
                            {
                                date = row["DATETIME"].ToString();
                                break;
                            }
                        }
                    }
                    line.Append(date);
                    line.Append(",");
                    double max = list.Max<double>();
                    line.Append(max);
                    line.Append(",");
                    foreach (DataRow row in sdt.Rows)
                    {
                        if (row[col] != DBNull.Value && row[col].ToString() != "")
                        {
                            if (Convert.ToDouble(row[col]) == max)
                            {
                                date = row["DATETIME"].ToString();
                                break;
                            }
                        }
                    }
                    line.Append(date);
                    sw.WriteLine(line.ToString());
                }
            }
            sw.Close();

        }
    }
}
