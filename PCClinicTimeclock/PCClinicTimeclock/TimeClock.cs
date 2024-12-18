using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCClinicTimeclock
{
    public class TimeClock
    {
        public class TimeEntry
        {
            public int EmployeeId { get; set; }
            public DateTime ClockInTime { get; set; }
            public DateTime? ClockOutTime { get; set; }

            public TimeSpan GetTotalWorkedTime()
            {
                return ClockOutTime.HasValue ? ClockOutTime.Value - ClockInTime : TimeSpan.Zero;
            }
        }

        // Stores all time entries.
        private readonly List<TimeEntry> _timeEntries = new List<TimeEntry>();

        // Clock in an employee.
        public void ClockIn(int employeeId)
        {
            if (IsEmployeeClockedIn(employeeId))
            {
                Console.WriteLine($"Employee {employeeId} is already clocked in.");
                return;
            }

            _timeEntries.Add(new TimeEntry
            {
                EmployeeId = employeeId,
                ClockInTime = DateTime.Now
            });

            Console.WriteLine($"Employee {employeeId} clocked in at {DateTime.Now}.");
        }

        // Clock out an employee.
        public void ClockOut(int employeeId)
        {
            var entry = GetActiveTimeEntry(employeeId);

            if (entry == null)
            {
                Console.WriteLine($"Employee {employeeId} is not clocked in.");
                return;
            }

            entry.ClockOutTime = DateTime.Now;

            Console.WriteLine($"Employee {employeeId} clocked out at {DateTime.Now}. Total time worked: {entry.GetTotalWorkedTime()}.");
        }

        // Check if an employee is clocked in.
        public bool IsEmployeeClockedIn(int employeeId)
        {
            return GetActiveTimeEntry(employeeId) != null;
        }

        // Get the active time entry for an employee.
        private TimeEntry GetActiveTimeEntry(int employeeId)
        {
            return _timeEntries.Find(e => e.EmployeeId == employeeId && !e.ClockOutTime.HasValue);
        }

        // Retrieve all time entries for reporting purposes.
        public List<TimeEntry> GetTimeEntries()
        {
            return _timeEntries;
        }
    }
}
