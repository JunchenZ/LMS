using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers {
  [Authorize(Roles = "Administrator")]
  public class AdministratorController : CommonController {
    public IActionResult Index() {
      return View();
    }

    public IActionResult Department(string subject) {
      ViewData["subject"] = subject;
      return View();
    }

    public IActionResult Course(string subject, string num) {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      return View();
    }

    /// <summary>
    /// Returns a JSON array of all the courses in the given department.
    /// Each object in the array should have the following fields:
    /// "number" - The course number (as in 6016 for this course)
    /// "name" - The course name (as in "Database Systems..." for this course)
    /// </summary>
    /// <param name="subject">The department subject abbreviation</param>
    /// <returns></returns>
    public IActionResult GetCourses(string subject) {

      var query = from departments in db.Department
                  join courses in db.Course
                  on departments.Subject equals courses.Subject
                  where courses.Subject == subject
                  select new { number = courses.Number, name = courses.Name };

      return Json(query.ToArray());
    }

    /// <summary>
    /// Returns a JSON array of all the professors working in a given department.
    /// Each object in the array should have the following fields:
    /// "lname" - The professor's last name
    /// "fname" - The professor's first name
    /// "uid" - The professor's uid
    /// </summary>
    /// <param name="subject">The department subject abbreviation</param>
    /// <returns></returns>
    public IActionResult GetProfessors(string subject) {

      var query = from departments in db.Department
                  join professors in db.Professor
                  on departments.Subject equals professors.Subject
                  where professors.Subject == subject
                  select new { lname = professors.LastName, fname = professors.FirstName, uid = professors.UId };

      return Json(query.ToArray());
    }

    /// <summary>
    /// Creates a course.
    /// </summary>
    /// <param name="subject">The subject abbreviation for the department in which the course will be added</param>
    /// <param name="number">The course number</param>
    /// <param name="name">The course name</param>
    /// <returns>A JSON object containing {success = true/false}. False if the course already exists, true otherwise.</returns>
    public IActionResult CreateCourse(string subject, int number, string name) {

      // TODO: Consider making the course ID autoincrement. Although, it may
      // actually be doing so automatically, its simply hidden by the fact there
      // was testing data added manually. This might be the cause of only
      // being able to add a single course (id = 0) if db.Course.Count() is not
      // present below.

      Course course = new Course();
      course.CourseId = db.Course.Count() + 1;
      course.Subject = subject;
      course.Number = number;
      course.Name = name;

      db.Course.Add(course);

      try {
        db.SaveChanges();
      } catch (Exception e) {
        Debug.WriteLine(e.Message);
        return Json(new { success = false });
      }

      return Json(new { success = true });
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
    /// <returns>A JSON object containing {success = true/false}. False if another class occupies the same location during any time within the start-end range in the same semester.</returns>
    public IActionResult CreateClass(string subject, int number, string season, int year, TimeSpan start, TimeSpan end, string location, string instructor) {

      // TODO: Test and confirm changing DateTime to TimeSpan for the start and end arguments is okay.

      var query = from departments in db.Department
                  join courses in db.Course
                  on departments.Subject equals courses.Subject
                  where courses.Subject == subject &&
                  courses.Number == number
                  select courses.CourseId;

      Class c = new Class();
      c.ClassId = db.Class.Count() + 1;
      c.Year = year;
      c.Season = season;
      c.Start = start;
      c.End = end;
      c.Location = location;
      c.CourseId = query.First(); // First or default? Should there always be only 1 here?
      c.UId = instructor;

      db.Class.Add(c);

      try {
        db.SaveChanges();
      } catch (Exception e) {
        Debug.WriteLine(e.Message);
        return Json(new { success = false });
      }

      return Json(new { success = true });
    }

  }
}