# Timeclock Application

This is a WPF-based timeclock application designed for employee time tracking and management. The application allows employees to clock in and out, while managers can generate detailed reports and view currently clocked-in employees.

## Features

### Employee Functions:
- **Clock In/Out**: Employees can clock in and out by entering their Employee ID.
- **Worked Time Display**: The main interface displays a list of currently clocked-in employees and their worked time, updated every 10 minutes.

### Manager Functions:
- **Error Logging**: Logs errors in an SQLite database table for troubleshooting.
- **Reports**:
  - Generate reports for a specific date range.
  - Save reports in CSV format.
  - View employees currently clocked in.

### Data Storage:
- **SQLite Integration**:
  - `TimeEntries` table for clock-in and clock-out records.
  - `ErrorLog` table for storing error messages and timestamps.

## Technologies Used
- **Frontend**: WPF (Windows Presentation Foundation)
- **Backend**: C#
- **Database**: SQLite

## Installation

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/your-repo/timeclock-app.git
   cd timeclock-app
   ```

2. **Build the Application**:
   - Open the project in Visual Studio.
   - Build the solution.

3. **Run the Application**:
   - Execute the application directly from Visual Studio or run the compiled `.exe` file from the `bin` directory.

## How to Use

### Employee Workflow
1. Enter your Employee ID in the provided text box.
2. Click "Clock In" to start tracking time or "Clock Out" to stop tracking time.

### Manager Workflow
1. Click "Open Report Form" to access the reporting interface.
2. Select the start and end dates using the DateTime pickers.
3. Generate and save a report in CSV format.

### Viewing Currently Clocked-In Employees
The bottom section of the main form lists:
- Employees currently clocked in.
- Total time worked since their last clock-in.

## Database Schema

### `TimeEntries` Table
| Column       | Type    | Description                          |
|--------------|---------|--------------------------------------|
| Id           | INTEGER | Primary key                         |
| EmployeeId   | INTEGER | ID of the employee                  |
| ClockInTime  | TEXT    | Timestamp of clock-in               |
| ClockOutTime | TEXT    | Timestamp of clock-out (nullable)   |

### `ErrorLog` Table
| Column       | Type    | Description                          |
|--------------|---------|--------------------------------------|
| Id           | INTEGER | Primary key                         |
| ErrorMessage | TEXT    | Error message description           |
| Timestamp    | TEXT    | Timestamp of when the error occurred|


