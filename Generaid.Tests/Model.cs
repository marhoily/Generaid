using System.Collections.Generic;

namespace Generaid
{
    partial class CompanyGenerator : ITransformer
    {
        public CompanyGenerator(Company company) { _company = company; }
        private readonly Company _company;
        public string Name => "company.cs";
    }
    partial class EmployeeGenerator : ITransformer
    {
        public EmployeeGenerator(Employee employee) { _employee = employee; }
        private readonly Employee _employee;
        public string Name => "employee.cs";
    }
    public sealed class Employee
    {
        public string Name { get; }

        public Employee(string name)
        {
            Name = name;
        }
    }
    public sealed class Company
    {
        public string Name { get; }
        public Employee[] Employees { get; }

        public Company(string name, params Employee[] employees)
        {
            Name = name;
            Employees = employees;
        }
    }

    public sealed class Model
    {
        public List<Company> Companies = new List<Company> {
            new Company("Microsoft",
                new Employee("John"),
                new Employee("Marry")),
            new Company("Apple",
                new Employee("Alice"),
                new Employee("Bob")) };
    }
}
