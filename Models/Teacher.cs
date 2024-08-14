using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Extensions.FileSystemGlobbing.Internal;

namespace HTTP5112_Cumulative_3.Models
{
    public class Teacher
    {
        public int TeacherId; 
        public string TeacherFname;
        public string TeacherLname;
        public string EmployeeNumber;
        public string HireDate;
        public decimal Salary;
        public List<Class> CourseTaught;

        // Validate fields except TeacherId and CourseTaught
        public bool IsValid()
        {
            Regex nameCheck = new Regex(@"[A-Za-z]+");
            Regex numberCheck = new Regex(@"[T]\d{3}");
            Regex dateCheck = new Regex(@"[12]\d{3}-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01])\s(0[0-9]|1[0-2]):[0-5]\d:[0-5]\d");
            Regex salaryCheck = new Regex(@"[0-9]\d{0,9}(\.\d{1,2})?$");

            if (!nameCheck.IsMatch(TeacherFname))
                return false;
            else if (!nameCheck.IsMatch(TeacherLname))
                return false;
            else if (!numberCheck.IsMatch(EmployeeNumber))
                return false;
            else if (!dateCheck.IsMatch(HireDate))
                return false;
            else if (!salaryCheck.IsMatch(Salary.ToString()))
                return false;
            else
                return true;
        }
    }
}

