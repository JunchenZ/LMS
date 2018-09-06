using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Controllers
{
    public class CommonController : Controller
    {

        // TODO: Add a protected database context variable once you have scaffolded your team database
        // for example: 
        protected Team7Context db;

        public CommonController()
        {
            // TODO: Initialize your context once you have scaffolded your team database
            // for example:
            db = new Team7Context();
        }

        /*
         * WARNING: This is the quick and easy way to make the controller
         *          use a different Context - good enough for our purposes.
         *          The "right" way is through Dependency Injection via the constructor (look this up if interested).
        */

        // TODO: Add a "UseContext" method if you wish to change the "db" context for unit testing
        //       See the lecture on testing

        // TODO: Uncomment this once you have created the variable "db"
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /*******Begin code to modify********/


        /// <summary>
        /// Retreive a JSON array of all departments from the database.
        /// Each object in the array should have a field called "name" and "subject",
        /// where "name" is the department name and "subject" is the subject abbreviation.
        /// </summary>
        /// <returns>The JSON array</returns>
        public IActionResult GetDepartments()
        {
            // done, unchecked

            var query = from d in db.Department
                        select new { name = d.Name, subject = d.Subject };

            return Json(query.ToArray());
        }


        /// /// <summary>
        /// Returns a JSON array representing the course catalog.
        /// Each object in the array should have the following fields:
        /// "subject": The subject abbreviation, (e.g. "CS")
        /// "dname": The department name, as in "Computer Science"
        /// "courses": An array of JSON objects representing the courses in the department.
        ///            Each field in this inner-array should have the following fields:
        ///            "number": The course number (e.g. 6016)
        ///            "cname": The course name (e.g. "Database Systems and Applications")
        /// </summary>
        /// <returns>The JSON array</returns>
        public IActionResult GetCatalog()
        {
            //done, unchecked

            var query = from d in db.Department
                        join c in db.Course on d.Subject equals c.Subject
                        select new
                        {
                            subject = d.Subject,
                            dname = d.Name,
                            courses = (from i in d.Course // to check
                                       select new
                                       {
                                           number = i.Number,
                                           cname = i.Name
                                       }
                                       ).ToArray()
                        };

            return Json(query.ToArray());
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
        /// <param name="subject"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public IActionResult GetClassOfferings(string subject, int number)
        {
            //done, unchecked

            var query = from c in db.Course
                        where c.Number == number && c.Subject == subject
                        join clas in db.Class on c.CourseId equals clas.CourseId
                        join prof in db.Professor on clas.UId equals prof.UId
                        select new
                        {
                            season = clas.Season,
                            year = clas.Year,
                            location = clas.Location,
                            start = clas.Start,
                            end = clas.End,
                            fname = prof.FirstName,
                            lname = prof.LastName
                        };

            return Json(query.ToArray());
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
        public IActionResult GetAssignmentContents(string subject, int num, string season, int year, string category, string asgname)
        {
            // done, unchecked

            var query = from c in db.Course
                        where c.Subject == subject && c.Number == num
                        join clas in db.Class on c.CourseId equals clas.CourseId
                        where clas.Season == season && clas.Year == year
                        join cat in db.AsgnCategory on clas.ClassId equals cat.ClaId
                        where cat.Name == category
                        join a in db.Assignment on cat.CatId equals a.CatId
                        where a.Name == asgname
                        select a;

            return Content(query.First().Contents); // to check
        }

        /// <summary>
        /// This method does NOT return JSON. It returns plain text (containing html).
        /// Use "return Content(...)" to return plain text.
        /// Returns the contents of an assignment submission.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment in the category</param>
        /// <param name="uid">The uid of the student who submitted it</param>
        /// <returns>The submission text</returns>
        public IActionResult GetSubmissionText(string subject, int num, string season, int year, string category, string asgname, string uid)
        {
            // done, unchecked

            var query = from c in db.Course
                        where c.Subject == subject && c.Number == num
                        join clas in db.Class on c.CourseId equals clas.CourseId
                        where clas.Season == season && clas.Year == year
                        join cat in db.AsgnCategory on clas.ClassId equals cat.ClaId
                        where cat.Name == category
                        join a in db.Assignment on cat.CatId equals a.CatId
                        where a.Name == asgname
                        join s in db.Submission on a.AId equals s.AId
                        where s.UId == uid
                        select s;

            if (query.Count() == 0) // to check
            {
                return Content("");
            }

            return Content(query.First().Contents);
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
        /// <returns>The user JSON object or an object containing {success: false} if the user doesn't exist</returns>
        public IActionResult GetUser(string uid)
        {
            // done, unchecked

            var query = from a in db.Administrator
                        where a.UId == uid
                        select new
                        {
                            fname = a.FName,
                            lname = a.LName,
                            uid = a.UId,
                        };

            if (query.Count() != 0)
            {
                return Json(query.First());
            }

            var query2 = from s in db.Student
                         where s.UId == uid
                         join d in db.Department on s.Subject equals d.Subject
                         select new
                         {
                             fname = s.FName,
                             lname = s.LName,
                             uid = s.UId,
                             department = d.Name
                         };

            if (query2.Count() != 0) // to check
            {
                return Json(query2.First());
            }

            var query3 = from p in db.Professor
                         where p.UId == uid
                         join d in db.Department on p.Subject equals d.Subject
                         select new
                         {
                             fname = p.FirstName,
                             lname = p.LastName,
                             uid = p.UId,
                             department = d.Name
                         };

            if (query3.Count() != 0)
            {
                return Json(query3.First());
            }

            return Json(new { success = false });
        }

        /*******End code to modify********/

    }
}