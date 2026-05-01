using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
[assembly: InternalsVisibleTo( "LMSControllerTests" )]
namespace LMS.Controllers
{
    public class AdministratorController : Controller
    {
        private readonly LMSContext db;

        public AdministratorController(LMSContext _db)
        {
            db = _db;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Department(string subject)
        {
            ViewData["subject"] = subject;
            return View();
        }

        public IActionResult Course(string subject, string num)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }

        /*******Begin code to modify********/

        /// <summary>
        /// Create a department which is uniquely identified by it's subject code
        /// </summary>
        /// <param name="subject">the subject code</param>
        /// <param name="name">the full name of the department</param>
        /// <returns>A JSON object containing {success = true/false}.
        /// false if the department already exists, true otherwise.</returns>
        
        /// this part is done- reman
        public IActionResult CreateDepartment(string subject, string name)
        {
            bool exists = db.Departments.Any(d => d.Subject == subject);

            if (exists)
            {
                return Json(new { success = false });
            }

            Department dept = new Department
            {
                Subject = subject,
                Name = name
            };
            db.Departments.Add(dept);
            db.SaveChanges();
            return Json(new { success = true});
        }


        /// <summary>
        /// Returns a JSON array of all the courses in the given department.
        /// Each object in the array should have the following fields:
        /// "number" - The course number (as in 5530)
        /// "name" - The course name (as in "Database Systems")
        /// </summary>
        /// <param name="subjCode">The department subject abbreviation (as in "CS")</param>
        /// <returns>The JSON result</returns>
        
        /// This part is done - Reman
        public IActionResult GetCourses(string subject)
        {
            var result = from c in db.Courses
                where c.DeptSubject == subject
                select new
                {
                    number = c.CourseNum,
                    name = c.Name
                };
            return Json(result);
        }

        /// <summary>
        /// Returns a JSON array of all the professors working in a given department.
        /// Each object in the array should have the following fields:
        /// "lname" - The professor's last name
        /// "fname" - The professor's first name
        /// "uid" - The professor's uid
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <returns>The JSON result</returns>
        ///
        ///  This part is done - Reman
        public IActionResult GetProfessors(string subject)
        {
            var result = from p in db.Professors
                where p.DeptSubject == subject
                select new
                {
                    lname = p.LName,
                    fname = p.FName,
                    uid = p.UId
                };
            return Json(result);
            
        }



        /// <summary>
        /// Creates a course.
        /// A course is uniquely identified by its number + the subject to which it belongs
        /// </summary>
        /// <param name="subject">The subject abbreviation for the department in which the course will be added</param>
        /// <param name="number">The course number</param>
        /// <param name="name">The course name</param>
        /// <returns>A JSON object containing {success = true/false}.
        /// false if the course already exists, true otherwise.</returns>
        /// /// This part is done - Reman
        public IActionResult CreateCourse(string subject, int number, string name)
        {           
            bool exists = db.Courses.Any(c => c.DeptSubject == subject && c.CourseNum == number);

            if (exists)
            {
                return Json(new { success = false });
            }

            Course course = new Course
            {
                DeptSubject = subject,
                CourseNum = (uint)number,
                Name = name
            };
            db.Courses.Add(course);
            db.SaveChanges();
            return Json(new { success = true});
        }



        /// <summary>
        /// Creates a class offering of a given course.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="number">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="start">The start time</param>
        /// <param name="end">The end time</param>
        /// <param name="location">The location</param>
        /// <param name="instructor">The uid of the professor</param>
        /// <returns>A JSON object containing {success = true/false}. 
        /// false if another class occupies the same location during any time 
        /// within the start-end range in the same semester, or if there is already
        /// a Class offering of the same Course in the same Semester,
        /// true otherwise.</returns>
        ///
        /// This part is done- Reman
        public IActionResult CreateClass(string subject, int number, string season, int year, DateTime start, DateTime end, string location, string instructor)
        {            
            bool duplicateCourse = db.Classes.Any(c => c.Course.DeptSubject == subject && c.Course.CourseNum == number&& c.SemesterSeason == season && c.SemesterYear == year);

            if (duplicateCourse)
            {
                return Json(new { success = false });
            }
            bool conflict = db.Classes.Any(c=> c.Location == location && c.SemesterSeason == season && c.SemesterYear == year 
                                               && TimeOnly.FromDateTime(start) < c.EndTime &&
                                               TimeOnly.FromDateTime(end) > c.StartTime);

            if (conflict)
            {
                return Json(new { success = false });
            }
            Course course = db.Courses.First(c => c.DeptSubject == subject && c.CourseNum == number);

            Class newClass = new Class
            {
                CourseId = course.CourseId,
                ProfuId = instructor,
                SemesterSeason = season,
                SemesterYear = (uint)year,
                Location = location,
                StartTime = TimeOnly.FromDateTime(start),
                EndTime = TimeOnly.FromDateTime(end)
            };
            db.Classes.Add(newClass);
            db.SaveChanges();
            
            return Json(new { success = true});
        }


        /*******End code to modify********/

    }
}

