using System.Collections.Generic;
using JetBrains.Annotations;

namespace Generaid
{
    public sealed class ModelGenerator : ITransformer
    {
        public string Name => "root";
        public string TransformText() => "root";
    }
    [UsedImplicitly]
    public class CompanyGenerator : ITransformer, ICanChooseToEscapeGeneration
    {
        public CompanyGenerator(Company company) { _company = company; }
        private readonly Company _company;
        public string Name =>
            (_company.NeedSubfolder ? @"companies\" : "")
            + _company.Name;
        public string TransformText() => _company.Name;
        public bool DoNotGenerate => _company.DoNotGenerate;
    }
    [UsedImplicitly]
    public class EmployeeGenerator : ITransformer
    {
        public EmployeeGenerator(Employee employee) { _employee = employee; }
        private readonly Employee _employee;
        public string Name => _employee.Name;
        public string TransformText() => _employee.Name;
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
        public bool NeedSubfolder { get; set; }
        public bool DoNotGenerate { get; set; }

        public Company(string name, params Employee[] employees)
        {
            Name = name;
            Employees = employees;
        }
    }

    public sealed class Model
    {
        public readonly List<Company> Companies = new List<Company> {
            new Company("Microsoft",
                new Employee("John"),
                new Employee("Marry")),
            new Company("Apple",
                new Employee("Alice"),
                new Employee("Bob")) };
    }
}
