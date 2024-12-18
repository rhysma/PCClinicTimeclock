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
            if (int.TryParse(EmployeeIdTextBox.Text, out int employeeId))
            {
                _timeClock.ClockIn(employeeId);
                UpdateStatus($"Employee {employeeId} clocked in.");
            }
            else
            {
                UpdateStatus("Invalid Employee ID.");
            }
        }

        private void ClockOutButton_Click(object sender, RoutedEventArgs e)
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

        private void UpdateStatus(string message)
        {
            StatusTextBlock.Text = message;
        }
    }
}