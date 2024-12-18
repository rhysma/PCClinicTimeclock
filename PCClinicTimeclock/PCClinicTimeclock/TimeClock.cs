using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.IO;

namespace PCClinicTimeclock
{
    public class TimeClock
    {
        private const string ConnectionString = "Data Source=timeclock.db;Version=3;";
        // Stores all time entries.
        private readonly List<TimeEntry> _timeEntries = new List<TimeEntry>();

        public TimeClock()
        {
            InitializeDatabase();
        }


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

        private void InitializeDatabase()
        {
            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            string createTableQuery = @"CREATE TABLE IF NOT EXISTS TimeEntries (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                EmployeeId INTEGER NOT NULL,
                ClockInTime TEXT NOT NULL,
                ClockOutTime TEXT
            );";

            using var command = new SQLiteCommand(createTableQuery, connection);
            command.ExecuteNonQuery();
        }



        // Clock in an employee.
        public void ClockIn(int employeeId)
        {
            if (IsEmployeeClockedIn(employeeId))
            {
                Console.WriteLine($"Employee {employeeId} is already clocked in.");
                return;
            }

            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            string insertQuery = "INSERT INTO TimeEntries (EmployeeId, ClockInTime) VALUES (@EmployeeId, @ClockInTime);";
            using var command = new SQLiteCommand(insertQuery, connection);
            command.Parameters.AddWithValue("@EmployeeId", employeeId);
            command.Parameters.AddWithValue("@ClockInTime", DateTime.Now.ToString("o"));

            command.ExecuteNonQuery();

            Console.WriteLine($"Employee {employeeId} clocked in at {DateTime.Now}.");
        }


        // Clock out an employee.
        public void ClockOut(int employeeId)
        {
            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            string selectQuery = "SELECT Id, ClockInTime FROM TimeEntries WHERE EmployeeId = @EmployeeId AND ClockOutTime IS NULL;";
            using var selectCommand = new SQLiteCommand(selectQuery, connection);
            selectCommand.Parameters.AddWithValue("@EmployeeId", employeeId);

            using var reader = selectCommand.ExecuteReader();
            if (!reader.Read())
            {
                Console.WriteLine($"Employee {employeeId} is not clocked in.");
                return;
            }

            int entryId = reader.GetInt32(0);

            string updateQuery = "UPDATE TimeEntries SET ClockOutTime = @ClockOutTime WHERE Id = @Id;";
            using var updateCommand = new SQLiteCommand(updateQuery, connection);
            updateCommand.Parameters.AddWithValue("@ClockOutTime", DateTime.Now.ToString("o"));
            updateCommand.Parameters.AddWithValue("@Id", entryId);

            updateCommand.ExecuteNonQuery();

            Console.WriteLine($"Employee {employeeId} clocked out at {DateTime.Now}.");
        }

        // Check if an employee is clocked in.
        public bool IsEmployeeClockedIn(int employeeId)
        {
            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            string query = "SELECT COUNT(1) FROM TimeEntries WHERE EmployeeId = @EmployeeId AND ClockOutTime IS NULL;";
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@EmployeeId", employeeId);

            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }

        public List<TimeEntry> GetCurrentlyClockedIn()
        {
            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            string query = "SELECT EmployeeId, ClockInTime FROM TimeEntries WHERE ClockOutTime IS NULL;";
            using var command = new SQLiteCommand(query, connection);
            using var reader = command.ExecuteReader();

            var result = new List<TimeEntry>();
            while (reader.Read())
            {
                result.Add(new TimeEntry
                {
                    EmployeeId = reader.GetInt32(0),
                    ClockInTime = DateTime.Parse(reader.GetString(1))
                });
            }

            return result;
        }

        public List<TimeEntry> GetEntriesForPeriod(DateTime startDate, DateTime endDate)
        {
            using var connection = new SQLiteConnection(ConnectionString);
            connection.Open();

            string query = "SELECT EmployeeId, ClockInTime, ClockOutTime FROM TimeEntries WHERE ClockInTime >= @StartDate AND ClockInTime <= @EndDate;";
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddWithValue("@StartDate", startDate.ToString("o"));
            command.Parameters.AddWithValue("@EndDate", endDate.ToString("o"));

            using var reader = command.ExecuteReader();

            var result = new List<TimeEntry>();
            while (reader.Read())
            {
                result.Add(new TimeEntry
                {
                    EmployeeId = reader.GetInt32(0),
                    ClockInTime = DateTime.Parse(reader.GetString(1)),
                    ClockOutTime = reader.IsDBNull(2) ? (DateTime?)null : DateTime.Parse(reader.GetString(2))
                });
            }

            return result;
        }

        public void ExportToCsv(List<TimeEntry> entries, string filePath)
        {
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("EmployeeId,ClockInTime,ClockOutTime");

            foreach (var entry in entries)
            {
                csvBuilder.AppendLine($"{entry.EmployeeId},{entry.ClockInTime},{entry.ClockOutTime}");
            }

            File.WriteAllText(filePath, csvBuilder.ToString());
        }


    }
}
