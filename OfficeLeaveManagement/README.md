# Office Leave Management System

A clean, responsive, and professional **Office Leave Management System** built with **ASP.NET Core MVC** (.NET 8.0 LTS).
This is a basic college demonstration project focused on frontend UI and clean MVC architecture.

---

## 🚀 Technology Stack

### Backend Framework
- **ASP.NET Core MVC** (.NET 8.0 LTS)
- **C#** (Nullable reference types enabled)
- **Session State** for authentication and role-based simulation

### Frontend & UI
- **Razor Views** (`.cshtml`) with strongly-typed ViewModels and Tag Helpers
- **HTML5** & **CSS3** (Responsive design, custom theme, card hover effects)
- **Bootstrap 5.3.3** (Grid, Navbar, Sidebar drawer, Modals, Badges, Alerts, Progress bars)
- **Bootstrap Icons 1.11.3**
- **JavaScript** (Dynamic leave day calculation, client-side table search & filter)
- **Chart.js 4.4.1** (Monthly trends, department breakdown, leave status distributions)

### Data Layer
- **In-Memory C# Collections** (`List<T>` with thread-safe synchronization locks)
- No SQL Server, Entity Framework Core, migrations, or external database dependencies

---

## 🏗️ Architecture

```
Browser (HTTP Request)
      ↓
 Razor Views (HTML5 / Bootstrap 5 / Tag Helpers)
      ↓
 MVC Controllers ([AuthorizeRole] / Session Filter)
      ↓
 Services Layer (IEmployeeService, ILeaveService, IReportService, IAuditService)
      ↓
 In-Memory Data Collections (Singleton storage with thread safety)
```

---

## ⚠️ Important Limitation

> [!IMPORTANT]
> **This application uses an In-Memory Data Layer.**
> There is **no database** (no SQL Server, no EF Core, no SQLite). All data—including employees, departments, leave balances, applications, and audit logs—is stored in memory while the application is running.
> **All data will reset back to the default demo state whenever the application restarts.**
> Authentication is simulated using ASP.NET Core Session and is designed for demo and evaluation purposes, not for production use.

---

## 👥 Demo User Accounts

Use these pre-configured accounts on `/Account/Login` (also available via 1-click fill buttons on the login page):

| Role | Email | Password | Access Level |
| :--- | :--- | :--- | :--- |
| **Admin** | `admin@office.com` | `Admin@123` | Full administrative control, employee/department/leave management, balances, reports, audit logs |
| **Manager** | `manager@office.com` | `Manager@123` | Manager dashboard, pending leave approvals/rejections with comments, department employee history |
| **Employee** | `employee@office.com` | `Employee@123` | Employee dashboard, apply leave with live validation, my leaves, balances, profile editing |

---

## 📁 Project Structure

```
OfficeLeaveManagement/
│
├── Controllers/
│   ├── HomeController.cs           # Landing redirect based on session role
│   ├── AccountController.cs        # Login, Logout, AccessDenied
│   ├── EmployeeController.cs       # Employee Dashboard, Profile, Balance
│   ├── LeaveController.cs          # Apply Leave, MyLeaves, Details, Cancel
│   ├── ManagerController.cs        # Manager Dashboard, PendingRequests, Approve, Reject, EmployeeHistory
│   ├── AdminController.cs          # Admin Dashboard, Employees, Create/Edit Employee, LeaveBalances, AllLeaves
│   ├── DepartmentController.cs     # Department Index, Create, Edit
│   ├── LeaveTypeController.cs      # LeaveType Index, Create, Edit
│   ├── ReportController.cs         # Reports & Chart.js visualizations
│   └── AuditController.cs          # System audit log tracking and filtering
│
├── Filters/
│   └── AuthorizeRoleAttribute.cs   # Session-based role authorization filter
│
├── Models/
│   ├── Enums.cs                    # LeaveStatus, UserRole
│   ├── Employee.cs                 # Employee entity
│   ├── Department.cs               # Department entity
│   ├── LeaveType.cs                # LeaveType entity
│   ├── LeaveBalance.cs             # LeaveBalance entity (Total, Used, Remaining)
│   ├── LeaveApplication.cs         # LeaveApplication entity
│   ├── AuditLog.cs                 # System audit log entity
│   └── User.cs                     # Login user entity
│
├── Services/
│   ├── IAuditService.cs & AuditService.cs
│   ├── IEmployeeService.cs & EmployeeService.cs
│   ├── ILeaveService.cs & LeaveService.cs
│   └── IReportService.cs & ReportService.cs
│
├── ViewModels/
│   ├── LoginViewModel.cs
│   ├── EmployeeDashboardViewModel.cs
│   ├── ManagerDashboardViewModel.cs
│   ├── AdminDashboardViewModel.cs
│   ├── ApplyLeaveViewModel.cs
│   ├── LeaveDetailsViewModel.cs
│   ├── EmployeeViewModel.cs
│   ├── DepartmentViewModel.cs
│   ├── LeaveTypeViewModel.cs
│   ├── LeaveBalanceViewModel.cs
│   ├── ProfileViewModel.cs
│   └── ReportViewModel.cs
│
├── Views/
│   ├── Account/ (Login, AccessDenied)
│   ├── Admin/ (Dashboard, Employees, CreateEmployee, EditEmployee, LeaveBalances, AllLeaves)
│   ├── Audit/ (Index)
│   ├── Department/ (Index, Create, Edit)
│   ├── Employee/ (Dashboard, Profile, Balance)
│   ├── Home/ (Index)
│   ├── Leave/ (Apply, MyLeaves, Details)
│   ├── LeaveType/ (Index, Create, Edit)
│   ├── Manager/ (Dashboard, PendingRequests, LeaveRequests, EmployeeHistory)
│   ├── Report/ (Index)
│   └── Shared/ (_Layout, _Alerts, _ValidationScriptsPartial, Error)
│
├── wwwroot/
│   ├── css/ (site.css, dashboard.css, responsive.css)
│   └── js/ (site.js, leave.js, dashboard.js)
│
├── Program.cs                      # Service DI, Session registration, Middleware
├── appsettings.json
└── README.md
```

---

## ⚙️ Prerequisites

- **.NET 8.0 SDK** (or higher)
- Any code editor: **Visual Studio 2022**, **Visual Studio Code**, or **JetBrains Rider**

---

## 🏃 How to Run the Application

1. Open a terminal and navigate to the project directory:
   ```bash
   cd OfficeLeaveManagement
   ```

2. Restore NuGet dependencies:
   ```bash
   dotnet restore
   ```

3. Build the solution:
   ```bash
   dotnet build
   ```

4. Run the web application:
   ```bash
   dotnet run
   ```

5. Open your browser and navigate to:
   ```
   http://localhost:5000  (or the port shown in the console, e.g. http://127.0.0.1:5218)
   ```

---

## ✨ Features Implemented

### 1. Authentication & Role-Based Authorization
- Clean session-based simulation using `[AuthorizeRole("Admin", "Manager", "Employee")]`.
- Automatic redirection to role-specific dashboard upon login.
- Unauthenticated access redirects to `/Account/Login?returnUrl=...`.
- Unauthorized access redirects to user-friendly `/Account/AccessDenied`.
- Logout clears session and redirects to login.

### 2. Employee Module
- **Dashboard**: KPI cards for Total, Used, Remaining, Pending, Approved, and Rejected leaves; visual progress bars for each leave balance; recent applications list.
- **Apply Leave**:
  - Live client-side JavaScript calculation of selected calendar days.
  - Server-side date validation (`EndDate >= StartDate`).
  - Balance validation: Prevents paid leaves exceeding remaining balance; allows Leave Without Pay (LWP).
  - Overlap check: Prevents applying for dates that overlap with existing Pending or Approved leaves.
- **My Leaves**: Filterable list of all personal leave requests with status badges, detail links, and cancellation option.
- **Leave Balance**: Detailed breakdown with interactive progress bars.
- **Profile**: View employee code, department, joining date; editable phone and name.

### 3. Manager Module
- **Dashboard**: Department pending requests counter, employees currently on leave, department leave statistics chart.
- **Pending Requests**: Review table with instant approval or rejection requiring a mandatory manager comment.
- **Employee History**: Accordion view of all employees in the manager's department with leave balances and complete past leave history.

### 4. Admin Module
- **Dashboard**: Organization-wide metrics, status distribution chart, department breakdown bar chart, monthly trend line chart.
- **Employee Management**: Search, filter by department/role, add new employee with initial balance creation, edit employee, activate/deactivate.
- **Department Management**: Add, edit, and toggle active status for departments.
- **Leave Type Management**: Configure annual allocations, add, and edit leave types.
- **Leave Balances**: Search employees and update annual allocations with automatic remaining calculation.
- **All Leaves**: Filterable across employee, department, leave type, status, and date ranges.

### 5. Reports & Analytics
- Monthly leave trends (Chart.js bar chart).
- Department leave distribution (Chart.js doughnut chart).
- Leave status breakdown (Chart.js pie chart).
- Comprehensive employee leave statistics table.

### 6. Audit Trail
- System-wide logging for `LOGIN`, `LOGOUT`, `APPLY_LEAVE`, `APPROVE_LEAVE`, `REJECT_LEAVE`, `CANCEL_LEAVE`, `CREATE_EMPLOYEE`, `UPDATE_EMPLOYEE`, `CREATE_DEPARTMENT`, `UPDATE_DEPARTMENT`, `CREATE_LEAVE_TYPE`, `UPDATE_LEAVE_TYPE`, and `UPDATE_BALANCE`.
- Filterable by user, action type, and search keyword.

---

## 📱 Responsive Design

- Optimized for Desktop, Laptop, Tablet, and Mobile screens.
- Mobile drawer sidebar with backdrop toggle.
- Responsive horizontal table wrappers to avoid viewport clipping.
- Bootstrap 5 grid system for cards and metrics.
