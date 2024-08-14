using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HTTP5112_Cumulative_3.Models;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.X509;

namespace HTTP5112_Cumulative_3.Controllers
{
    public class ClassController : Controller
    {
        // GET: Class
        public ActionResult Index()
        {
            ClassDataController controller = new ClassDataController();
            IEnumerable<Class> Classes = controller.ListClasses();
            return View(Classes);
        }

        //GET : /Class/List
        public ActionResult List(string classCode = null, string className = null, List<string> semester = null)
        {
            ClassDataController controller = new ClassDataController();
            IEnumerable<Class> Classes = controller.ListClasses(classCode, className, semester);
            return View(Classes);
        }

        //GET : /Class/Show/{id}
        public ActionResult Show(int id)
        {
            ClassDataController controller = new ClassDataController();
            Class NewClass = controller.FindClass(id);

            return View(NewClass);
        }
    }
}

