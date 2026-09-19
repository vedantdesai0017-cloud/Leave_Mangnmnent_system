namespace OfficeLeaveManagement.Services;

using OfficeLeaveManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;

public class EmployeeService : IEmployeeService
{
    private readonly List<Employee> _employees = new();
    private readonly List<Department> _departments = new();
    private readonly List<User> _demoUsers = new();
    private readonly object _lock = new();
    private int _nextEmployeeId = 1;
    private int _nextDepartmentId = 1;
    private int _nextUserId = 1;

    public EmployeeService()
    {
        SeedData();
    }

    private void SeedData()
    {
        lock (_lock)
        {
            _departments.Add(new Department { DepartmentId = 1, DepartmentName = "IT", Description = "Information Technology", IsActive = true });
            _departments.Add(new Department { DepartmentId = 2, DepartmentName = "HR", Description = "Human Resources", IsActive = true });
            _departments.Add(new Department { DepartmentId = 3, DepartmentName = "Finance", Description = "Finance & Accounting", IsActive = true });
            _departments.Add(new Department { DepartmentId = 4, DepartmentName = "Marketing", Description = "Marketing & Sales", IsActive = true });
            _departments.Add(new Department { DepartmentId = 5, DepartmentName = "Operations", Description = "Operations & Logistics", IsActive = true });
            _nextDepartmentId = 6;

            var seedEmployees = new List<(string code, string email, string password, UserRole role, int deptId, string fullName, string phone)>
            {
                ("EMP001", "admin@office.com", "Admin@123", UserRole.Admin, 1, "Admin User", "9876543210"),
                ("EMP002", "manager@office.com", "Manager@123", UserRole.Manager, 1, "Rajesh Kumar", "9876543211"),
                ("EMP003", "employee@office.com", "Employee@123", UserRole.Employee, 1, "Suresh Patel", "9876543212"),
                ("EMP004", "rahul.sharma@office.com", "Emp@1234", UserRole.Employee, 2, "Rahul Sharma", "9876543213"),
                ("EMP005", "priya.patel@office.com", "Emp@1234", UserRole.Employee, 3, "Priya Patel", "9876543214"),
                ("EMP006", "amit.kumar@office.com", "Emp@1234", UserRole.Employee, 4, "Amit Kumar", "9876543215"),
                ("EMP007", "neha.gupta@office.com", "Emp@1234", UserRole.Employee, 5, "Neha Gupta", "9876543216"),
            };

            foreach (var s in seedEmployees)
            {
                var emp = new Employee
                {
                    EmployeeId = _nextEmployeeId++,
                    EmployeeCode = s.code,
                    Email = s.email,
                    Role = s.role,
                    DepartmentId = s.deptId,
                    FullName = s.fullName,
                    Phone = s.phone,
                    JoiningDate = new DateTime(2024, 1, 15).AddMonths(_nextEmployeeId - 2),
                    IsActive = true
                };
                _employees.Add(emp);

                _demoUsers.Add(new User
                {
                    UserId = _nextUserId++,
                    Email = s.email,
                    Password = s.password,
                    Role = s.role,
                    FullName = s.fullName,
                    EmployeeId = emp.EmployeeId
                });
            }
        }
    }

    private Employee PopulateDepartment(Employee emp)
    {
        var dept = _departments.FirstOrDefault(d => d.DepartmentId == emp.DepartmentId);
        if (dept != null)
        {
            emp.DepartmentName = dept.DepartmentName;
        }
        return emp;
    }

    public List<Employee> GetAll()
    {
        lock (_lock)
        {
            return _employees.Select(PopulateDepartment).ToList();
        }
    }

    public List<Employee> GetActive()
    {
        lock (_lock)
        {
            return _employees.Where(e => e.IsActive).Select(PopulateDepartment).ToList();
        }
    }

    public Employee? GetById(int id)
    {
        lock (_lock)
        {
            var emp = _employees.FirstOrDefault(e => e.EmployeeId == id);
            return emp != null ? PopulateDepartment(emp) : null;
        }
    }

    public Employee? GetByEmail(string email)
    {
        lock (_lock)
        {
            var emp = _employees.FirstOrDefault(e => e.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            return emp != null ? PopulateDepartment(emp) : null;
        }
    }

    public List<Employee> GetByDepartment(int departmentId)
    {
        lock (_lock)
        {
            return _employees.Where(e => e.DepartmentId == departmentId).Select(PopulateDepartment).ToList();
        }
    }

    public List<Employee> Search(string? searchTerm, int? departmentId, string? role)
    {
        lock (_lock)
        {
            var results = _employees.AsEnumerable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                results = results.Where(e => e.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                             e.EmployeeCode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                             e.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            if (departmentId.HasValue)
            {
                results = results.Where(e => e.DepartmentId == departmentId.Value);
            }

            if (!string.IsNullOrEmpty(role) && Enum.TryParse<UserRole>(role, true, out var parsedRole))
            {
                results = results.Where(e => e.Role == parsedRole);
            }

            return results.Select(PopulateDepartment).ToList();
        }
    }

    public void Add(Employee employee)
    {
        lock (_lock)
        {
            employee.EmployeeId = _nextEmployeeId++;
            _employees.Add(employee);

            _demoUsers.Add(new User
            {
                UserId = _nextUserId++,
                Email = employee.Email,
                Password = "Default@123",
                Role = employee.Role,
                FullName = employee.FullName,
                EmployeeId = employee.EmployeeId
            });
        }
    }

    public void Update(Employee employee)
    {
        lock (_lock)
        {
            var index = _employees.FindIndex(e => e.EmployeeId == employee.EmployeeId);
            if (index != -1)
            {
                _employees[index] = employee;

                var userIndex = _demoUsers.FindIndex(u => u.EmployeeId == employee.EmployeeId);
                if (userIndex != -1)
                {
                    _demoUsers[userIndex].FullName = employee.FullName;
                    _demoUsers[userIndex].Role = employee.Role;
                }
            }
        }
    }

    public List<Department> GetAllDepartments()
    {
        lock (_lock)
        {
            return _departments.ToList();
        }
    }

    public List<Department> GetActiveDepartments()
    {
        lock (_lock)
        {
            return _departments.Where(d => d.IsActive).ToList();
        }
    }

    public Department? GetDepartmentById(int id)
    {
        lock (_lock)
        {
            return _departments.FirstOrDefault(d => d.DepartmentId == id);
        }
    }

    public void AddDepartment(Department department)
    {
        lock (_lock)
        {
            department.DepartmentId = _nextDepartmentId++;
            _departments.Add(department);
        }
    }

    public void UpdateDepartment(Department department)
    {
        lock (_lock)
        {
            var index = _departments.FindIndex(d => d.DepartmentId == department.DepartmentId);
            if (index != -1)
            {
                _departments[index] = department;
            }
        }
    }

    public List<User> GetDemoUsers()
    {
        lock (_lock)
        {
            return _demoUsers.ToList();
        }
    }

    public User? ValidateLogin(string email, string password)
    {
        lock (_lock)
        {
            return _demoUsers.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && u.Password == password);
        }
    }
}
