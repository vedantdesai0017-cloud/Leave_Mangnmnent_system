namespace OfficeLeaveManagement.Services;

using OfficeLeaveManagement.Models;
using System.Collections.Generic;

public interface IEmployeeService
{
    List<Employee> GetAll();
    List<Employee> GetActive();
    Employee? GetById(int id);
    Employee? GetByEmail(string email);
    List<Employee> GetByDepartment(int departmentId);
    List<Employee> Search(string? searchTerm, int? departmentId, string? role);
    void Add(Employee employee);
    void Update(Employee employee);

    List<Department> GetAllDepartments();
    List<Department> GetActiveDepartments();
    Department? GetDepartmentById(int id);
    void AddDepartment(Department department);
    void UpdateDepartment(Department department);

    List<User> GetDemoUsers();
    User? ValidateLogin(string email, string password);
}
