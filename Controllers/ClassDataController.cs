using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HTTP5112_Cumulative_3.Models;
using MySql.Data.MySqlClient;

namespace HTTP5112_Cumulative_3.Controllers
{
    public class ClassDataController : Controller
    {
        private SchoolDbContext School = new SchoolDbContext();

        /// <summary>
        /// This controller will access the classes table of school database and return a list of classes in the school database.
        /// </summary>
        /// <returns>A list of classes (class codes)</returns>
        /// <example>GET api/ClassData/ListClasses</example>
        [HttpGet]
        [Route("api/ClassData/ListClasses")]
        public IEnumerable<Class> ListClasses(string classCode = null, string className = null, List<string> semester = null)
        {
            MySqlConnection Conn = School.AccessDatabase();

            Conn.Open();

            MySqlCommand cmd = Conn.CreateCommand();

            if (semester.Count() > 0)
                cmd.CommandText = "SELECT * FROM classes where lower(classcode) like lower(@keyCode) and lower(classname) like lower(@keyName) and MONTH(startdate) IN (" + String.Join(", ", semester.ToArray()) + ")";
            else
                cmd.CommandText = "SELECT * FROM classes where lower(classcode) like lower(@keyCode) and lower(classname) like lower(@keyName) and MONTH(startdate) IN (\"\")";

            cmd.Parameters.AddWithValue("@keyCode", "%" + classCode + "%");
            cmd.Parameters.AddWithValue("@keyName", "%" + className + "%");
            cmd.Prepare();

            MySqlDataReader ResultSet = cmd.ExecuteReader();

            List<Class> Classes = new List<Class> { };

            while (ResultSet.Read())
            {
                int ClassId = Convert.ToInt32(ResultSet["classid"]);
                string ClassCode = ResultSet["classcode"].ToString();
                int? TeacherId = null;
                if (ResultSet["teacherid"].ToString() != "")
                    TeacherId = Convert.ToInt32(ResultSet["teacherid"]);
                string StartDate = ResultSet["startdate"].ToString();
                string FinishDate = ResultSet["finishdate"].ToString();
                string ClassName = ResultSet["classname"].ToString();

                Class MyClass = new Class();
                MyClass.ClassId = ClassId;
                MyClass.ClassCode = ClassCode;
                MyClass.TeacherId = TeacherId;
                MyClass.StartDate = StartDate;
                MyClass.FinishDate = FinishDate;
                MyClass.ClassName = ClassName;

                Classes.Add(MyClass);
            }

            Conn.Close();

            return Classes;
        }

        /// <summary>
        /// This controller will find a class in the system given an ID
        /// </summary>
        /// <param name="id">The class primary key</param>
        /// <returns>A class object</returns>
        /// <example>GET api/ClassData/FindClass</example>
        [HttpGet]
        [Route("api/ClassData/FindClass")]
        public Class FindClass(int id)
        {
            MySqlConnection Conn = School.AccessDatabase();

            Conn.Open();

            MySqlCommand cmd = Conn.CreateCommand();

            cmd.CommandText = "SELECT * FROM classes LEFT OUTER JOIN teachers ON teachers.teacherid = classes.teacherid WHERE classes.classid = @id";

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();

            MySqlDataReader ResultSet = cmd.ExecuteReader();

            Class NewClass = new Class();

            while (ResultSet.Read())
            {
                int ClassId = Convert.ToInt32(ResultSet["classid"]);
                string ClassCode = ResultSet["classcode"].ToString();
                int? TeacherId = null;
                if (ResultSet["teacherid"].ToString() != "")
                    TeacherId = Convert.ToInt32(ResultSet["teacherid"]);
                string StartDate = ResultSet["startdate"].ToString();
                string FinishDate = ResultSet["finishdate"].ToString();
                string ClassName = ResultSet["classname"].ToString();

                NewClass.ClassId = ClassId;
                NewClass.ClassCode = ClassCode;
                NewClass.TeacherId = TeacherId;
                NewClass.StartDate = StartDate;
                NewClass.FinishDate = FinishDate;
                NewClass.ClassName = ClassName;

                if (ResultSet["teacherid"].ToString() != "")
                {
                    Teacher NewTeacher = new Teacher();
                    int TeacherId2 = Convert.ToInt32(ResultSet["teacherid"]);
                    string TeacherFname = ResultSet["teacherfname"].ToString();
                    string TeacherLname = ResultSet["teacherlname"].ToString();
                    string EmployeeNumber = ResultSet["employeenumber"].ToString();
                    string HireDate = ResultSet["hiredate"].ToString();
                    decimal Salary = Convert.ToDecimal(ResultSet["salary"]);

                    NewTeacher.TeacherId = TeacherId2;
                    NewTeacher.TeacherFname = TeacherFname;
                    NewTeacher.TeacherLname = TeacherLname;
                    NewTeacher.EmployeeNumber = EmployeeNumber;
                    NewTeacher.HireDate = HireDate;
                    NewTeacher.Salary = Salary;

                    NewClass.ClassTeacher = NewTeacher;
                }
            }

            Conn.Close();

            return NewClass;
        }
    }
}

