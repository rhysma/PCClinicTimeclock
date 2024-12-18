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


        /// <summary>
        /// Constructor
        /// </summary>
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

        /// <summary>
        /// Init the connection to the SQLite databases for storing time entries
        /// and error logging
        /// </summary>
        private void InitializeDatabase()
        {
            try
            {
                using var connection = new SQLiteConnection(ConnectionString);
                connection.Open();

                string createTableQuery = @"CREATE TABLE IF NOT EXISTS TimeEntries (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                EmployeeId INTEGER NOT NULL,
                ClockInTime TEXT NOT NULL,
                ClockOutTime TEXT
            );";

                string createErrorLogTable = @"CREATE TABLE IF NOT EXISTS ErrorLog (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ErrorMessage TEXT NOT NULL,
                Timestamp TEXT NOT NULL
            );";

                using var command = new SQLiteCommand(createTableQuery, connection);
                command.ExecuteNonQuery();

                using var command2 = new SQLiteCommand(createErrorLogTable, connection);
                command2.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogError(ex.Message);
            }
        }



        /// <summary>
        /// Clocks in an employee. Adds an entry to the sql table.
        /// </summary>
        /// <param name="employeeId"></param>
        public void ClockIn(int employeeId)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogError(ex.Message);
            }

        }


        /// <summary>
        /// Clocks out an employee. Updates the clock status of the employee in the sql table
        /// </summary>
        /// <param name="employeeId"></param>
        public void ClockOut(int employeeId)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogError(ex.Message);
            }
            
        }

        /// <summary>
        /// Checks to see if the employee is currently clocked in.
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public bool IsEmployeeClockedIn(int employeeId)
        {
            try
            {
                using var connection = new SQLiteConnection(ConnectionString);
                connection.Open();

                string query = "SELECT COUNT(1) FROM TimeEntries WHERE EmployeeId = @EmployeeId AND ClockOutTime IS NULL;";
                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@EmployeeId", employeeId);

                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogError(ex.Message);
                return false; 
            }
            
        }

        /// <summary>
        /// Runs a query to get all users who are currently clocked in.
        /// </summary>
        /// <returns></returns>
        public List<TimeEntry> GetCurrentlyClockedIn()
        {
            var result = new List<TimeEntry>();

            try
            {
                using var connection = new SQLiteConnection(ConnectionString);
                connection.Open();

                string query = "SELECT EmployeeId, ClockInTime FROM TimeEntries WHERE ClockOutTime IS NULL;";
                using var command = new SQLiteCommand(query, connection);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    result.Add(new TimeEntry
                    {
                        EmployeeId = reader.GetInt32(0),
                        ClockInTime = DateTime.Parse(reader.GetString(1))
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogError(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Runs a query that gets all timeclock entries in the database for a particular time period
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<TimeEntry> GetEntriesForPeriod(DateTime startDate, DateTime endDate)
        {
            var result = new List<TimeEntry>();
            try
            {
                using var connection = new SQLiteConnection(ConnectionString);
                connection.Open();

                // Adjust the endDate to include the entire day
                endDate = endDate.AddDays(1).AddTicks(-1);

                string query = @"SELECT EmployeeId, ClockInTime, ClockOutTime 
                     FROM TimeEntries 
                     WHERE ClockInTime >= @StartDate AND ClockInTime <= @EndDate;";
                using var command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@StartDate", startDate.ToString("o"));
                command.Parameters.AddWithValue("@EndDate", endDate.ToString("o"));

                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    result.Add(new TimeEntry
                    {
                        EmployeeId = reader.GetInt32(0),
                        ClockInTime = DateTime.Parse(reader.GetString(1)),
                        ClockOutTime = reader.IsDBNull(2) ? (DateTime?)null : DateTime.Parse(reader.GetString(2))
                    });
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogError(ex.Message);
            }
            

            return result;
        }

        /// <summary>
        /// Exports the current report for the start and end dates into a CSV file. 
        /// </summary>
        /// <param name="entries"></param>
        /// <param name="filePath"></param>
        public void ExportToCsv(List<TimeEntry> entries, string filePath)
        {
            try
            {
                var csvBuilder = new StringBuilder();
                csvBuilder.AppendLine("EmployeeId,ClockInTime,ClockOutTime");

                foreach (var entry in entries)
                {
                    csvBuilder.AppendLine($"{entry.EmployeeId},{entry.ClockInTime},{entry.ClockOutTime}");
                }

                File.WriteAllText(filePath, csvBuilder.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                LogError(ex.Message);
            }
            
        }

        /// <summary>
        /// Writes out an error to the sql table
        /// </summary>
        /// <param name="errorMessage"></param>
        public void LogError(string errorMessage)
        {
            try
            {
                using var connection = new SQLiteConnection(ConnectionString);
                connection.Open();

                string insertErrorQuery = @"INSERT INTO ErrorLog (ErrorMessage, Timestamp) VALUES (@ErrorMessage, @Timestamp);";
                using var command = new SQLiteCommand(insertErrorQuery, connection);
                command.Parameters.AddWithValue("@ErrorMessage", errorMessage);
                command.Parameters.AddWithValue("@Timestamp", DateTime.Now.ToString("o"));

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }


    }
}
