using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace PCClinicTimeclock
{
    /// <summary>
    /// Interaction logic for ReportWindow.xaml
    /// </summary>
    public partial class ReportWindow : Window
    {
        private TimeClock _timeClock;

        public ReportWindow(TimeClock timeClock)
        {
            InitializeComponent();
            _timeClock = timeClock;
        }

        private void GenerateReportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (StartDatePicker.SelectedDate.HasValue && EndDatePicker.SelectedDate.HasValue)
                {
                    DateTime startDate = StartDatePicker.SelectedDate.Value;
                    DateTime endDate = EndDatePicker.SelectedDate.Value;

                    var entries = _timeClock.GetEntriesForPeriod(startDate, endDate);

                    // Prompt to save CSV
                    var saveDialog = new SaveFileDialog
                    {
                        Filter = "CSV files (*.csv)|*.csv",
                        DefaultExt = ".csv",
                        FileName = "TimeReport.csv"
                    };

                    if (saveDialog.ShowDialog() == true)
                    {
                        _timeClock.ExportToCsv(entries, saveDialog.FileName);
                        UpdateStatus("Report saved to " + saveDialog.FileName);
                    }
                }
                else
                {
                    UpdateStatus("Please select both start and end dates.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                UpdateStatus("Error: " + ex.Message);
            }
            
        }


        private void UpdateStatus(string message)
        {
            StatusTextBlock.Text = message;
        }
    }
}
