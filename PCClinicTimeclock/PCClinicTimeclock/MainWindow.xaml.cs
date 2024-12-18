using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PCClinicTimeclock
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TimeClock _timeClock = new TimeClock();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ClockInButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.TryParse(EmployeeIdTextBox.Text, out int employeeId))
                {
                    _timeClock.ClockIn(employeeId);
                    UpdateStatus($"Employee {employeeId} clocked in.");
                    EmployeeIdTextBox.Clear(); // Clear the textbox after clicking
                }
                else
                {
                    UpdateStatus("Invalid Employee ID.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                UpdateStatus("Error: " + ex.Message);
            }

            
        }

        private void ClockOutButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.TryParse(EmployeeIdTextBox.Text, out int employeeId))
                {
                    _timeClock.ClockOut(employeeId);
                    UpdateStatus($"Employee {employeeId} clocked out.");
                }
                else
                {
                    UpdateStatus("Invalid Employee ID.");
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

        private void AdminViewButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentlyClockedIn = _timeClock.GetCurrentlyClockedIn();
                string clockedInMessage = "Currently Clocked In:\n" +
                    string.Join("\n", currentlyClockedIn.Select(e => $"Employee {e.EmployeeId} clocked in at {e.ClockInTime}"));

                var reportWindow = new ReportWindow(_timeClock);
                reportWindow.Show();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                UpdateStatus("Error: " + ex.Message);
            }
            
        }

    }

}