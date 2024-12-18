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
using System.Timers;

namespace PCClinicTimeclock
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TimeClock _timeClock = new TimeClock();
        private System.Timers.Timer _updateTimer;

        public MainWindow()
        {
            InitializeComponent();
            // Initialize the timer for periodic updates
            _updateTimer = new System.Timers.Timer(1000); // 10 minutes in milliseconds
            _updateTimer.Elapsed += UpdateCurrentlyClockedIn;
            _updateTimer.Start();

            UpdateCurrentlyClockedIn(null, null); // Initial update
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

        private void UpdateCurrentlyClockedIn(object sender, ElapsedEventArgs e)
        {
            try
            {
                var currentlyClockedIn = _timeClock.GetCurrentlyClockedIn();
                Dispatcher.Invoke(() =>
                {
                    CurrentlyClockedInList.Items.Clear();
                    foreach (var entry in currentlyClockedIn)
                    {
                        var workedTime = DateTime.Now - entry.ClockInTime;
                        CurrentlyClockedInList.Items.Add($"Employee {entry.EmployeeId} - Worked: {workedTime:hh\\:mm\\:ss}");
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                UpdateStatus("Error: " + ex.Message);
            }

        }

    }

}