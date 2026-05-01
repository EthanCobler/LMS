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
    public class CommonController : Controller
    {
        private readonly LMSContext db;

        public CommonController(LMSContext _db)
        {
            db = _db;
        }

        /*******Begin code to modify********/

        /// <summary>
        /// Retreive a JSON array of all departments from the database.
        /// Each object in the array should have a field called "name" and "subject",
        /// where "name" is the department name and "subject" is the subject abbreviation.
        /// </summary>
        /// <returns>The JSON array</returns>
        
        /// this part is done, Reman
        public IActionResult GetDepartments()
        {
            var result = from d in db.Departments
                select new
                {
                    name = d.Name, subject = d.Subject
                };
            return Json(result);
        }



        /// <summary>
        /// Returns a JSON array representing the course catalog.
        /// Each object in the array should have the following fields:
        /// "subject": The subject abbreviation, (e.g. "CS")
        /// "dname": The department name, as in "Computer Science"
        /// "courses": An array of JSON objects representing the courses in the department.
        ///            Each field in this inner-array should have the following fields:
        ///            "number": The course number (e.g. 5530)
        ///            "cname": The course name (e.g. "Database Systems")
        /// </summary>
        /// <returns>The JSON array</returns>
        ///
        /// done - ethan
        public IActionResult GetCatalog()
        {
            var result = from d in db.Departments
                join c in db.Courses on d.Subject equals c.DeptSubject
                group c by new {d.Subject, d.Name} into g
                select new
                {
                    subject = g.Key.Subject, 
                    dname = g.Key.Name, 
                    courses = g.Select (c=> new {number = c.CourseNum, cname = c.Name})
                };
                
            return Json(result);
        }

        /// <summary>
        /// Returns a JSON array of all class offerings of a specific course.
        /// Each object in the array should have the following fields:
        /// "season": the season part of the semester, such as "Fall"
        /// "year": the year part of the semester
        /// "location": the location of the class
        /// "start": the start time in format "hh:mm:ss"
        /// "end": the end time in format "hh:mm:ss"
        /// "fname": the first name of the professor
        /// "lname": the last name of the professor
        /// </summary>
        /// <param name="subject">The subject abbreviation, as in "CS"</param>
        /// <param name="number">The course number, as in 5530</param>
        /// <returns>The JSON array</returns>
        ///
        /// done - ethan
        public IActionResult GetClassOfferings(string subject, int number)
        {
            var result = from cl in db.Classes
                join p in db.Professors on cl.ProfuId equals p.UId
                join c in db.Courses on cl.CourseId equals c.CourseId
                where c.DeptSubject == subject && c.CourseNum == number
                select new
                {
                    season = cl.SemesterSeason,
                    year = cl.SemesterYear,
                    location = cl.Location,
                    start = cl.StartTime,
                    end = cl.EndTime,
                    fname = p.FName,
                    lname = p.LName,
                };
            return Json(result);
        }

        /// <summary>
        /// This method does NOT return JSON. It returns plain text (containing html).
        /// Use "return Content(...)" to return plain text.
        /// Returns the contents of an assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment in the category</param>
        /// <returns>The assignment contents</returns>
        ///
        /// done - ethan
        public IActionResult GetAssignmentContents(string subject, int num, string season, int year, string category, string asgname)
        {            
            var result = (from cl in db.Classes
                join c in db.Courses on cl.CourseId equals c.CourseId
                join ac in db.AssignmentCategories on cl.ClassId equals ac.ClassId
                join a in db.Assignments on ac.CategoryId equals a.CategoryId
                
                where c.DeptSubject == subject && c.CourseNum == num && cl.SemesterSeason == season && cl.SemesterYear == year && ac.Name == category && a.Name == asgname
                select a.Contents).FirstOrDefault();
            
            return Content(result ?? "");
        }


        /// <summary>
        /// This method does NOT return JSON. It returns plain text (containing html).
        /// Use "return Content(...)" to return plain text.
        /// Returns the contents of an assignment submission.
        /// Returns the empty string ("") if there is no submission.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment in the category</param>
        /// <param name="uid">The uid of the student who submitted it</param>
        /// <returns>The submission text</returns>
        ///
        /// done - ethan
        public IActionResult GetSubmissionText(string subject, int num, string season, int year, string category, string asgname, string uid)
        {
            var result = (from cl in db.Classes
                join c in db.Courses on cl.CourseId equals c.CourseId
                join ac in db.AssignmentCategories on cl.ClassId equals ac.ClassId
                join a in db.Assignments on ac.CategoryId equals a.CategoryId
                join s in db.Submissions on a.AssignmentId equals s.AssignmentId
                where c.DeptSubject == subject
                      && c.CourseNum == num
                      && cl.SemesterSeason == season
                      && cl.SemesterYear == year
                      && ac.Name == category
                      && a.Name == asgname
                      && s.StudentuId == uid
                select s.Contents).FirstOrDefault();
            return Content("");
        }


        /// <summary>
        /// Gets information about a user as a single JSON object.
        /// The object should have the following fields:
        /// "fname": the user's first name
        /// "lname": the user's last name
        /// "uid": the user's uid
        /// "department": (professors and students only) the name (such as "Computer Science") of the department for the user. 
        ///               If the user is a Professor, this is the department they work in.
        ///               If the user is a Student, this is the department they major in.    
        ///               If the user is an Administrator, this field is not present in the returned JSON
        /// </summary>
        /// <param name="uid">The ID of the user</param>
        /// <returns>
        /// The user JSON object 
        /// or an object containing {success: false} if the user doesn't exist
        /// </returns>
        ///
        /// done - ethan
        public IActionResult GetUser(string uid)
        {
            var professor = (from p in db.Professors
                join d in db.Departments on p.DeptSubject equals d.Subject
                where p.UId == uid
                select new
                {
                    fname = p.FName,
                    lname = p.LName,
                    uid = p.UId,
                    department = d.Name
                }).FirstOrDefault();
            
            if (professor != null)
                return Json(professor);
            
            var student = (from s in db.Students
                join d in db.Departments on s.MajorSubject equals d.Subject
                where s.UId == uid
                select new
                {
                    fname = s.FName,
                    lname = s.LName,
                    uid = s.UId,
                    department = d.Name
                }).FirstOrDefault();
            
            if (student != null)
                return Json(student);

            var admin = (from a in db.Administrators
                where a.UId == uid
                select new
                {
                    fname = a.FName,
                    lname = a.LName,
                    uid = a.UId
                }).FirstOrDefault();
            
            if (admin != null)
                return Json(admin);
                        
            return Json(new { success = false });
        }


        /*******End code to modify********/
    }
}

