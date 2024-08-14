using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HTTP5112_Cumulative_3.Models;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Cors;
using System.Globalization;

namespace HTTP5112_Cumulative_3.Controllers
{
    public class TeacherDataController : Controller
    {
        private SchoolDbContext School = new SchoolDbContext();

        /// <summary>
        /// This controller will access the teachers table of school database and return a list of teachers in the school database.
        /// </summary>
        /// <returns>A list of teachers (first names and last names)</returns>
        /// <example>GET api/TeacherData/ListTeachers</example>
        [HttpGet]
        [Route("api/TeacherData/ListTeachers/{SearchKey?}")]
        public IEnumerable<Teacher> ListTeachers(string SearchKey = null)
        {
            MySqlConnection Conn = School.AccessDatabase();

            Conn.Open();

            MySqlCommand cmd = Conn.CreateCommand();

            cmd.CommandText = "SELECT * FROM teachers where lower(concat(teacherfname, ' ', teacherlname)) like lower(@key)";

            cmd.Parameters.AddWithValue("@key", "%" + SearchKey + "%");
            cmd.Prepare();

            MySqlDataReader ResultSet = cmd.ExecuteReader();

            List<Teacher> Teachers = new List<Teacher> { };

            while (ResultSet.Read())
            {
                int TeacherId = Convert.ToInt32(ResultSet["teacherid"]);
                string TeacherFname = ResultSet["teacherfname"].ToString();
                string TeacherLname = ResultSet["teacherlname"].ToString();
                string EmployeeNumber = ResultSet["employeenumber"].ToString();
                DateTime dtHireDate;
                string HireDate;
                bool boolHireDate = DateTime.TryParse(ResultSet["hiredate"].ToString(), out dtHireDate);
                if (boolHireDate)
                {
                    HireDate = dtHireDate.ToString("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    HireDate = dtHireDate.ToString();
                }
                decimal Salary = Convert.ToDecimal(ResultSet["salary"]);

                Teacher MyTeacher = new Teacher();
                MyTeacher.TeacherId = TeacherId;
                MyTeacher.TeacherFname = TeacherFname;
                MyTeacher.TeacherLname = TeacherLname;
                MyTeacher.EmployeeNumber = EmployeeNumber;
                MyTeacher.HireDate = HireDate;
                MyTeacher.Salary = Salary;

                Teachers.Add(MyTeacher);
            }

            Conn.Close();

            return Teachers;
        }

        /// <summary>
        /// This controller will find a teacher in the system given an ID
        /// </summary>
        /// <param name="id">The teacher primary key</param>
        /// <returns>A teacher object</returns>
        /// <example>GET api/TeacherData/FindTeacher</example>
        [HttpGet]
        [Route("api/TeacherData/FindTeacher")]
        public Teacher FindTeacher(int id)
        {
            MySqlConnection Conn = School.AccessDatabase();

            Conn.Open();

            MySqlCommand cmd = Conn.CreateCommand();

            cmd.CommandText = "SELECT * FROM teachers LEFT OUTER JOIN classes ON classes.teacherid = teachers.teacherid WHERE teachers.teacherid = @id";

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();

            MySqlDataReader ResultSet = cmd.ExecuteReader();

            Teacher NewTeacher = new Teacher();
            List<Class> NewClasses = new List<Class> { };

            while (ResultSet.Read())
            {
                int TeacherId = Convert.ToInt32(ResultSet["teacherid"]);
                string TeacherFname = ResultSet["teacherfname"].ToString();
                string TeacherLname = ResultSet["teacherlname"].ToString();
                string EmployeeNumber = ResultSet["employeenumber"].ToString();
                DateTime dtHireDate;
                string HireDate;
                bool boolHireDate = DateTime.TryParse(ResultSet["hiredate"].ToString(), out dtHireDate);
                if (boolHireDate)
                {
                    HireDate = dtHireDate.ToString("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    HireDate = dtHireDate.ToString();
                }
                decimal Salary = Convert.ToDecimal(ResultSet["salary"]);

                NewTeacher.TeacherId = TeacherId;
                NewTeacher.TeacherFname = TeacherFname;
                NewTeacher.TeacherLname = TeacherLname;
                NewTeacher.EmployeeNumber = EmployeeNumber;
                NewTeacher.HireDate = HireDate;
                NewTeacher.Salary = Salary;

                if (ResultSet["classid"].ToString() != "")
                {
                    Class NewClass = new Class();
                    int ClassId = Convert.ToInt32(ResultSet["classid"]);
                    string ClassCode = ResultSet["classcode"].ToString();
                    string StartDate = ResultSet["startdate"].ToString();
                    string FinishDate = ResultSet["finishdate"].ToString();
                    string ClassName = ResultSet["classname"].ToString();

                    NewClass.ClassId = ClassId;
                    NewClass.ClassCode = ClassCode;
                    NewClass.TeacherId = TeacherId;
                    NewClass.StartDate = StartDate;
                    NewClass.FinishDate = FinishDate;
                    NewClass.ClassName = ClassName;

                    NewClasses.Add(NewClass);
                }
            }

            NewTeacher.CourseTaught = NewClasses;

            Conn.Close();

            return NewTeacher;
        }

        /// <summary>
        /// Deletes an Teacher from the connected MySQL Database if the ID of that Teacher exists. Does NOT maintain relational integrity. Non-Deterministic.
        /// </summary>
        /// <param name="id">The ID of the Teacher.</param>
        /// <example>POST /api/TeacherData/DeleteTeacher/3</example>
        [HttpPost]
        public void DeleteTeacher(int id)
        {
            MySqlConnection Conn = School.AccessDatabase();

            Conn.Open();

            MySqlCommand cmd = Conn.CreateCommand();

            cmd.CommandText = "Delete from Teachers where Teacherid=@id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();

            cmd.ExecuteNonQuery();

            cmd.CommandText = "Update Classes Set Teacherid=null where Teacherid=@id";
            cmd.Prepare();

            cmd.ExecuteNonQuery();

            Conn.Close();
        }

        /// <summary>
        /// Adds an Teacher to the MySQL Database.
        /// </summary>
        /// <param name="NewTeacher">An object with fields that map to the columns of the Teacher's table. Non-Deterministic.</param>
        /// <example>
        /// POST api/TeacherData/AddTeacher 
        /// FORM DATA / POST DATA / REQUEST BODY 
        /// {
        ///	"TeacherFname":"Caitlin",
        ///	"TeacherLname":"Cummings",
        ///	"EmployeeNumber":"T381",
        ///	"HireDate":"10/6/2014 12:00:00 AM"
        ///	"Salary":"2.77"
        /// }
        /// </example>
        [HttpPost]
        //[EnableCors(origins: "*", methods: "*", headers: "*")]
        public void AddTeacher([FromBody] Teacher NewTeacher)
        {
            MySqlConnection Conn = School.AccessDatabase();

            Conn.Open();

            MySqlCommand cmd = Conn.CreateCommand();

            cmd.CommandText = "insert into Teachers (Teacherfname, Teacherlname, EmployeeNumber, HireDate, Salary) values (@TeacherFname, @TeacherLname, @EmployeeNumber, @HireDate, @Salary)";
            cmd.Parameters.AddWithValue("@TeacherFname", NewTeacher.TeacherFname);
            cmd.Parameters.AddWithValue("@TeacherLname", NewTeacher.TeacherLname);
            cmd.Parameters.AddWithValue("@EmployeeNumber", NewTeacher.EmployeeNumber);
            cmd.Parameters.AddWithValue("@HireDate", NewTeacher.HireDate);
            cmd.Parameters.AddWithValue("@Salary", NewTeacher.Salary);
            cmd.Prepare();

            cmd.ExecuteNonQuery();

            Conn.Close();

        }

        /// <summary>
        /// Updates an Teacher on the MySQL Database. Non-Deterministic.
        /// </summary>
        /// <param name="NewTeacher">An object with fields that map to the columns of the Teacher's table.</param>
        /// <example>
        /// POST api/TeacherData/UpdateTeacher/208 
        /// FORM DATA / POST DATA / REQUEST BODY 
        /// {
        ///	"TeacherTitle":"My Sound Adventure in Italy",
        ///	"TeacherBody":"I really enjoyed Italy. The food was amazing!",
        /// }
        /// </example>
        [HttpPost]
        public void UpdateTeacher(int id, [FromBody] Teacher NewTeacher)
        {
            MySqlConnection Conn = School.AccessDatabase();
            Conn.Open();

            if (!NewTeacher.IsValid()) throw new ApplicationException("Posted Data was not valid.");
            try
            {
                MySqlCommand cmd = Conn.CreateCommand();

                cmd.CommandText = "UPDATE Teachers SET Teacherfname=@TeacherFname, Teacherlname=@TeacherLname, EmployeeNumber=@EmployeeNumber, HireDate=@HireDate, Salary=@Salary WHERE Teacherid=@TeacherId";

                cmd.Parameters.AddWithValue("@TeacherFname", NewTeacher.TeacherFname);
                cmd.Parameters.AddWithValue("@TeacherLname", NewTeacher.TeacherLname);
                cmd.Parameters.AddWithValue("@EmployeeNumber", NewTeacher.EmployeeNumber);
                cmd.Parameters.AddWithValue("@HireDate", NewTeacher.HireDate);
                cmd.Parameters.AddWithValue("@Salary", NewTeacher.Salary);
                cmd.Parameters.AddWithValue("@TeacherID", id);

                Debug.WriteLine(cmd.CommandText);

                cmd.Prepare();

                cmd.ExecuteNonQuery();

            }
            catch (MySqlException ex)
            {
                Debug.WriteLine(ex);
                throw new ApplicationException("Issue was a database issue.", ex);
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
                throw new ApplicationException("There was a server issue.", ex);
            }
            finally
            {
                Conn.Close();

            }

        }
    }
}

