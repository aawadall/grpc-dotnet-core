using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrpcServer
{
   public static class Employees
    {
        public static List<Employee> employees = new List<Employee>()
        {
            new Employee
            {
                Id = 1,
                BadgeNumber = 1001,
                FirstName = "Abbas",
                LastName = "Firnas",
                VacationAccuralRate = 1.2f,
                VacationAccured = 3f
            },
            new Employee
            {
                Id = 2,
                BadgeNumber = 1002,
                FirstName = "Jabir",
                LastName = "Ibn Hayyan",
                VacationAccuralRate = 2.3f,
                VacationAccured = 23.4f
            },
            new Employee
            {
                Id = 3,
                BadgeNumber = 1003,
                FirstName = "Hasan",
                LastName = "Ibn Al-Haytham",
                VacationAccuralRate = 1.2f,
                VacationAccured = 12f
            }
        };
    }
}
