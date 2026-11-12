using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

public class Employee
{
    public int Id { get; set; }                  // 1
    public string FirstName { get; set; }        // 2
    public string LastName { get; set; }         // 3
    public string Department { get; set; }       // 4
    public string JobTitle { get; set; }         // 5
    public decimal MonthlySalary { get; set; }   // 6
    public double TaxRate { get; set; }          // 7
    public DateTime HireDate { get; set; }       // 8
    public bool IsActive { get; set; }           // 9
    public string Email { get; set; }            // 10
}

class Program
{
    // Starts EMPTY (0 pre-defined records)
    static List<Employee> employees = new List<Employee>();
    static int nextId = 1001;

    static void Main(string[] args)
    {
        bool running = true;
        while (running)
        {
            Console.Clear();
            Console.WriteLine("=== EPHRS - MAIN VERSION ===");
            Console.WriteLine("1. Create (Add Employee)");
            Console.WriteLine("2. Read (View All Employees)");
            Console.WriteLine("3. Search / Filter Employees");
            Console.WriteLine("4. Update Employee");
            Console.WriteLine("5. Delete Employee");
            Console.WriteLine("6. Exit");
            Console.Write("\nSelect an option: ");
            string choice = Console.ReadLine();
            Console.WriteLine();

            switch (choice)
            {
                case "1": CreateEmployee(); break;
                case "2": ReadEmployees(); break;
                case "3": SearchEmployees(); break;
                case "4": UpdateEmployee(); break;
                case "5": DeleteEmployee(); break;
                case "6": running = false; break;
                default: Console.WriteLine("Invalid choice."); break;
            }

            if (running) { Console.WriteLine("\nPress Enter to continue..."); Console.ReadLine(); }
        }
    }

    // ---------- Helpers ----------
    static string ReadText(string prompt)
    {
        Console.Write(prompt);
        return (Console.ReadLine() ?? "").Trim();
    }

    static void PrintHeader()
    {
        Console.WriteLine($"{"ID",-6}{"First",-10}{"Last",-10}{"Dept",-9}{"Job Title",-18}{"Salary",12}{"Tax",6}{"Net Pay",12} {"Hired",-11}{"Active",-7}Email");
        Console.WriteLine(new string('-', 125));
    }

    static void PrintRow(Employee e)
    {
        decimal net = e.MonthlySalary - (e.MonthlySalary * (decimal)e.TaxRate);
        Console.WriteLine($"{e.Id,-6}{e.FirstName,-10}{e.LastName,-10}{e.Department,-9}{e.JobTitle,-18}{e.MonthlySalary,12:N2}{e.TaxRate,6:P0}{net,12:N2} {e.HireDate:yyyy-MM-dd} {(e.IsActive ? "Yes" : "No"),-7}{e.Email}");
    }

    static void PrintList(IEnumerable<Employee> list)
    {
        PrintHeader();
        foreach (Employee e in list) PrintRow(e);
    }

    // ---------- CREATE ----------
    static void CreateEmployee()
    {
        Console.WriteLine("--- Add New Employee ---");
        string first = ReadText("First Name: ");
        string last = ReadText("Last Name: ");
        string dept = ReadText("Department (IT/HR/Sales/Finance): ");
        string job = ReadText("Job Title: ");

        decimal salary;
        while (!decimal.TryParse(ReadText("Monthly Salary: "), NumberStyles.Number, CultureInfo.InvariantCulture, out salary) || salary < 0)
            Console.WriteLine("Invalid salary. Try again.");

        double tax;
        while (!double.TryParse(ReadText("Tax Rate (e.g., 0.12 for 12%): "), NumberStyles.Float, CultureInfo.InvariantCulture, out tax) || tax < 0 || tax > 1)
            Console.WriteLine("Invalid tax rate. Enter a value from 0 to 1.");

        DateTime hire;
        while (!DateTime.TryParse(ReadText("Hire Date (yyyy-MM-dd): "), CultureInfo.InvariantCulture, DateTimeStyles.None, out hire))
            Console.WriteLine("Invalid date. Try again.");

        string activeInput;
        do
        {
            activeInput = ReadText("Is Active? (true/false): ").ToLower();
        } while (activeInput != "true" && activeInput != "false");

        string email = ReadText("Email: ");

        Employee emp = new Employee
        {
            Id = nextId++,
            FirstName = first,
            LastName = last,
            Department = dept,
            JobTitle = job,
            MonthlySalary = salary,
            TaxRate = tax,
            HireDate = hire,
            IsActive = bool.Parse(activeInput),
            Email = email
        };
        employees.Add(emp);

        Console.WriteLine($"\nEmployee added successfully! Assigned ID: {emp.Id}");
    }

    // ---------- READ ----------
    static void ReadEmployees()
    {
        Console.WriteLine("--- Employee Records ---");
        if (!employees.Any())
        {
            Console.WriteLine("No employees found.");
            return;
        }
        PrintList(employees.OrderBy(e => e.Id));
        Console.WriteLine($"\nTotal employees: {employees.Count}");
    }

    // ---------- SEARCH / FILTER ----------
    static void SearchEmployees()
    {
        Console.WriteLine("--- Search / Filter ---");
        Console.WriteLine("1. By Name (first or last)");
        Console.WriteLine("2. By Department");
        Console.WriteLine("3. By Minimum Monthly Salary");
        Console.WriteLine("4. Active employees only");
        Console.Write("\nChoose filter: ");
        string opt = Console.ReadLine();
        Console.WriteLine();

        List<Employee> results = new List<Employee>();

        switch (opt)
        {
            case "1":
                string name = ReadText("Enter name to search: ").ToLower();
                results = employees.Where(e => e.FirstName.ToLower().Contains(name)
                                            || e.LastName.ToLower().Contains(name)).ToList();
                break;
            case "2":
                string dept = ReadText("Enter department: ").ToLower();
                results = employees.Where(e => e.Department.ToLower() == dept).ToList();
                break;
            case "3":
                if (decimal.TryParse(ReadText("Enter minimum salary: "), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal min))
                    results = employees.Where(e => e.MonthlySalary >= min).ToList();
                else { Console.WriteLine("Invalid amount."); return; }
                break;
            case "4":
                results = employees.Where(e => e.IsActive).ToList();
                break;
            default:
                Console.WriteLine("Invalid filter.");
                return;
        }

        Console.WriteLine();
        if (!results.Any())
        {
            Console.WriteLine("No matching employees found.");
            return;
        }
        PrintList(results);
        Console.WriteLine($"\nMatches found: {results.Count}");
    }

    // ---------- UPDATE ----------
    static void UpdateEmployee()
    {
        ReadEmployees();
        if (!employees.Any()) return;

        Console.Write("\nEnter the ID of the employee to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("\nInvalid ID format.");
            return;
        }

        Employee emp = employees.FirstOrDefault(e => e.Id == id);
        if (emp == null)
        {
            Console.WriteLine("\nEmployee not found.");
            return;
        }

        Console.WriteLine("\n(Press Enter to keep the current value)");

        string v = ReadText($"First Name ({emp.FirstName}): ");
        if (!string.IsNullOrWhiteSpace(v)) emp.FirstName = v;

        v = ReadText($"Last Name ({emp.LastName}): ");
        if (!string.IsNullOrWhiteSpace(v)) emp.LastName = v;

        v = ReadText($"Department ({emp.Department}): ");
        if (!string.IsNullOrWhiteSpace(v)) emp.Department = v;

        v = ReadText($"Job Title ({emp.JobTitle}): ");
        if (!string.IsNullOrWhiteSpace(v)) emp.JobTitle = v;

        v = ReadText($"Monthly Salary ({emp.MonthlySalary}): ");
        if (decimal.TryParse(v, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal sal) && sal >= 0) emp.MonthlySalary = sal;

        v = ReadText($"Tax Rate ({emp.TaxRate}): ");
        if (double.TryParse(v, NumberStyles.Float, CultureInfo.InvariantCulture, out double tax) && tax >= 0 && tax <= 1) emp.TaxRate = tax;

        v = ReadText($"Hire Date ({emp.HireDate:yyyy-MM-dd}): ");
        if (DateTime.TryParse(v, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime hd)) emp.HireDate = hd;

        v = ReadText($"Is Active ({emp.IsActive}) [true/false]: ");
        if (bool.TryParse(v, out bool act)) emp.IsActive = act;

        v = ReadText($"Email ({emp.Email}): ");
        if (!string.IsNullOrWhiteSpace(v)) emp.Email = v;

        Console.WriteLine("\nEmployee updated successfully!");
    }

    // ---------- DELETE ----------
    static void DeleteEmployee()
    {
        ReadEmployees();
        if (!employees.Any()) return;

        Console.Write("\nEnter the ID of the employee to delete: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            int removed = employees.RemoveAll(e => e.Id == id);
            Console.WriteLine(removed > 0
                ? "\nEmployee deleted successfully."
                : "\nNo employee with that ID was found.");
        }
        else
        {
            Console.WriteLine("\nInvalid ID format.");
        }
    }
}