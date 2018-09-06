using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers {
  [Authorize(Roles = "Professor")]
  public class ProfessorController : CommonController {
    public IActionResult Index() {
      return View();
    }

    public IActionResult Students(string subject, string num, string season, string year) {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      return View();
    }

    public IActionResult Class(string subject, string num, string season, string year) {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      return View();
    }

    public IActionResult Categories(string subject, string num, string season, string year) {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      return View();
    }

    public IActionResult CatAssignments(string subject, string num, string season, string year, string cat) {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      ViewData["cat"] = cat;
      return View();
    }

    public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname) {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      ViewData["cat"] = cat;
      ViewData["aname"] = aname;
      return View();
    }

    public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname) {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      ViewData["cat"] = cat;
      ViewData["aname"] = aname;
      return View();
    }

    public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid) {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      ViewData["cat"] = cat;
      ViewData["aname"] = aname;
      ViewData["uid"] = uid;
      return View();
    }

    /*******Begin code to modify********/

    // TODO: Many of the following queries are only slight modifications of one
    // another. Many of them are also surprisingly long. What are some
    // alternative solutions?

    // TODO: Is it possible and/or reasonable to return a query from a method?
    // For example, there are several instances where the same query is
    // written with only a few modifications to the last couple of lines. This
    // is the case when searching for a specific 'Class' instance from only
    // the subject, course number, season, and year.

    /// <summary>
    /// Returns a JSON array of all the students in a class.
    /// Each object in the array should have the following fields:
    /// "fname" - first name
    /// "lname" - last name
    /// "uid" - user ID
    /// "dob" - date of birth
    /// "grade" - the student's grade in this class
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetStudentsInClass(string subject, int num, string season, int year) {

      // Select all classes for this subject
      var query = from courses in db.Course
                  join classes in db.Class
                  on courses.CourseId equals classes.CourseId

                  // Select all classes for this subject matching the
                  // arguments passed to this method.
                  where courses.Subject == subject &&
                  courses.Number == num &&
                  classes.Season == season &&
                  classes.Year == year

                  // Select all enrollments for this class.
                  join enrollments in db.Enrolled
                  on classes.ClassId equals enrollments.ClaId

                  // select all students who are enrolled in this class.
                  join students in db.Student
                  on enrollments.UId equals students.UId

                  select new {
                    fname = students.FName,
                    lname = students.LName,
                    uid = students.UId,
                    dob = students.Dob,
                    grade = convertToLetterGrade(enrollments.Grade)
                  };

      return Json(query.ToArray());
    }



    /// <summary>
    /// Assume that a specific class can not have two categories with the same name.
    /// Returns a JSON array with all the assignments in an assignment category for a class.
    /// If the "category" parameter is null, return all assignments in the class.
    /// Each object in the array should have the following fields:
    /// "aname" - The assignment name
    /// "cname" - The assignment category name.
    /// "due" - The due DateTime
    /// "submissions" - The number of submissions to the assignment
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class, or null to return assignments from all categories</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category) {

                   // Select all classes for this subject
      var query1 = from courses in db.Course
                   join classes in db.Class
                   on courses.CourseId equals classes.CourseId

                   // Select all classes for this subject matching the
                   // arguments passed to this method.
                   where courses.Subject == subject &&
                   courses.Number == num &&
                   classes.Season == season &&
                   classes.Year == year

                   // Select all assignment categories for this class.
                   join categories in db.AsgnCategory
                   on classes.ClassId equals categories.ClaId

                   // Select all assignments and categories for this class.
                   join assignments in db.Assignment
                   on categories.CatId equals assignments.CatId

                   // A list of all assignments and their corresponding categories
                   // for this class.
                   select new { assignments, categories };

      // TODO: The fact that the 'var' keyword must be defined when declared
      // makes the following if-else block rather repetitive. Is there a
      // solution to avoid the repetition?

      JsonResult result;
      if (category != null) {

        // Select only those assignments which belong to the given category.
        var query2 = from q in query1
                     where q.categories.Name == category
                     select new {
                       aname = q.assignments.Name,
                       cname = q.categories.Name,
                       due = q.assignments.Due,
                       submissions = (from s in db.Submission where s.AId == q.assignments.AId select s).Count()
                     };

        result = Json(query2.ToArray());

      } else {

        // Select all assignments
        var query2 = from q in query1
                     select new {
                       aname = q.assignments.Name,
                       cname = q.categories.Name,
                       due = q.assignments.Due,
                       submissions = (from s in db.Submission where s.AId == q.assignments.AId select s).Count()
                     };

        result = Json(query2.ToArray());
      }

      return result;
    }


    /// <summary>
    /// Returns a JSON array of the assignment categories for a certain class.
    /// Each object in the array should have the folling fields:
    /// "name" - The category name
    /// "weight" - The category weight
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetAssignmentCategories(string subject, int num, string season, int year) {

                  // Combine 'Course' and 'Class' tables for retrieval of
                  // the appropriate 'subject' and 'Class' instance.
      var query = from courses in db.Course
                  join classes in db.Class
                  on courses.CourseId equals classes.CourseId

                  // Match to the appropriate 'Class' instance of the
                  // given course.
                  where courses.Subject == subject &&
                  courses.Number == num &&
                  classes.Season == season &&
                  classes.Year == year

                  // Combine previously joined tables with 'AsgnCategory'
                  // for retrieval of assignment category names to match
                  // with the given category.
                  join categories in db.AsgnCategory
                  on classes.ClassId equals categories.ClaId

                  select new { name = categories.Name, weight = categories.Weight };

      return Json(query.ToArray());
    }

    /// <summary>
    /// Creates a new assignment category for the specified class.
    /// A class can not have two categories with the same name.
    /// If a category of the given class with the given name already exists, return success = false.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The new category name</param>
    /// <param name="catweight">The new category weight</param>
    /// <returns>A JSON object containing {success = true/false} </returns>
    public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight) {

      // Note: The uniqueness constraint for names within the same class is
      // provided by SQL. When an attempt is made to add a category with the
      // same name for the same class, an exception is thrown and this
      // method returns a JSON object where 'success = false'.

                  // Combine 'Course' and 'Class' tables for retrieval of
                  // the appropriate 'subject' and 'Class' instance.
      var query = from courses in db.Course
                  join classes in db.Class
                  on courses.CourseId equals classes.CourseId

                  // Match to the appropriate 'Class' instance of the
                  // given course.
                  where courses.Subject == subject &&
                  courses.Number == num &&
                  classes.Season == season &&
                  classes.Year == year

                  // The class ID to which this new assignment category
                  // will belong.
                  select classes.ClassId;

      // TODO: See the same questions about a similar code block within the
      // 'CreateAssignment' method.

      AsgnCategory asgnCategory = new AsgnCategory();
      asgnCategory.CatId = db.AsgnCategory.Count() + 1;
      asgnCategory.Name = category;
      asgnCategory.Weight = catweight;
      asgnCategory.ClaId = query.First(); // The query will never be empty.

      db.AsgnCategory.Add(asgnCategory);

      try {
        db.SaveChanges();
      } catch (Exception e) {
        // Most likely, this assignment category name for this class
        // already exists.
        return Json(new { success = false });
      }

      return Json(new { success = true });
    }

    /// <summary>
    /// Creates a new assignment for the given class and category.
    /// An assignment category (which belongs to a class) can not have two assignments with 
    /// the same name.
    /// If an assignment of the given category with the given name already exists, return success = false. 
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The new assignment name</param>
    /// <param name="asgpoints">The max point value for the new assignment</param>
    /// <param name="asgdue">The due DateTime for the new assignment</param>
    /// <param name="asgcontents">The contents of the new assignment</param>
    /// <returns>A JSON object containing success = true/false</returns>
    public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents) {

      // Note: The uniqueness constraint for names within the same category is
      // provided by SQL. When an attempt is made to add an assignment with the
      // same name for the same category, an exception is thrown and this
      // method returns a JSON object where 'success = false'.

      // Combine 'Course' and 'Class' tables for retrieval of
      // the appropriate 'subject' and 'Class' instance.
      var query1 = from courses in db.Course
                  join classes in db.Class
                  on courses.CourseId equals classes.CourseId

                  // Match to the appropriate 'Class' instance of the
                  // given course.
                  where courses.Subject == subject &&
                  courses.Number == num &&
                  classes.Season == season &&
                  classes.Year == year

                  select classes;

      // Combine previously joined tables with 'AsgnCategory'
      // for retrieval of assignment category names to match
      // with the given category.
      var query2 = from classes in query1
                  join categories in db.AsgnCategory
                  on classes.ClassId equals categories.ClaId
                  where categories.Name == category

                  // The category ID to which this new assignment
                  // will belong.
                  select categories.CatId;

      // TODO: Is there any particular reason to simply change the 'Assignment'
      // table to auto-increment the 'AId' column? When is the
      // 'db.Assignment.Count() + 1' actually reading from the database? Is this
      // any slower than having the column auto-increment?

      // TODO: 'FirstOrDefault' vs. 'First'. Is there an instance where the
      // above query will be empty?

      Assignment assignment = new Assignment();
      assignment.AId = db.Assignment.Count() + 1;
      assignment.Name = asgname;
      assignment.Contents = asgcontents;
      assignment.Due = asgdue;
      assignment.Points = asgpoints;
      assignment.CatId = query2.First(); // The query will never be empty.

      db.Assignment.Add(assignment);

      // All students enrolled in this course
      var query3 = from classes in query1
                   join enrollments in db.Enrolled
                   on classes.ClassId equals enrollments.ClaId
                   select enrollments.UId;

      try {
        db.SaveChanges();

        // TODO: Is this the best place for a call to updateGrade?
        // TODO: Modify updateGrade to take the classId and the uID as the parameter

        foreach (string uid in query3) {
          UpdateGrade(subject, num, season, year, uid);
        }

      } catch (Exception e) {
        // Most likely, this assignment name for this assignment category
        // already exists or there was a problem updating the grade for any of
        // the students in this class.
        Debug.WriteLine(e.Message);
        return Json(new { success = false });
      }

      return Json(new { success = true });
    }

    /// <summary>
    /// Gets a JSON array of all the submissions to a certain assignment.
    /// Each object in the array should have the following fields:
    /// "fname" - first name
    /// "lname" - last name
    /// "uid" - user ID
    /// "time" - DateTime of the submission
    /// "score" - The score given to the submission
    /// 
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The name of the assignment</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname) {

                   // Combine 'Course' and 'Class' tables for retrieval of
                   // the appropriate 'subject' and 'Class' instance.
      var query = from courses in db.Course
                  join classes in db.Class
                  on courses.CourseId equals classes.CourseId

                  // Match to the appropriate 'Class' instance of the
                  // given course.
                  where courses.Subject == subject &&
                  courses.Number == num &&
                  classes.Season == season &&
                  classes.Year == year

                  // Combine previously joined tables with 'AsgnCategory'
                  // for retrieval of assignment category names to match
                  // with the given category.
                  join categories in db.AsgnCategory
                  on classes.ClassId equals categories.ClaId
                  where categories.Name == category

                  // Now join the prior data with assignments to search
                  // for all assignments with the given assignment name.
                  // At this point, there should only be one assignment
                  // for the given name, for the given category, for the
                  // given class, for the given course.
                  join assignments in db.Assignment
                  on categories.CatId equals assignments.CatId
                  where assignments.Name == asgname

                  // Search for all submissions by the given student. At
                  // this point there should only be a single submission
                  // for the given assignment.
                  join submissions in db.Submission
                  on assignments.AId equals submissions.AId

                  // Now combine tables with 'Student' to retrieve the
                  // attributes of the specific student.
                  join students in db.Student
                  on submissions.UId equals students.UId

                  select new {
                    fname = students.FName,
                    lname = students.LName,
                    uid = students.UId,
                    time = submissions.Time,
                    score = submissions.Score
                  };

      return Json(query.ToArray());
    }

    /// <summary>
    /// Set the score of an assignment submission
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The name of the assignment</param>
    /// <param name="uid">The uid of the student who's submission is being graded</param>
    /// <param name="score">The new score for the submission</param>
    /// <returns>A JSON object containing success = true/false</returns> 
    public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score) {

                  // Combine 'Course' and 'Class' tables for retrieval of
                  // the appropriate 'subject' and 'Class' instance.
      var query = from courses in db.Course
                  join classes in db.Class
                  on courses.CourseId equals classes.CourseId

                  // Match to the appropriate 'Class' instance of the
                  // given course.
                  where courses.Subject == subject &&
                  courses.Number == num &&
                  classes.Season == season &&
                  classes.Year == year

                  // Combine previously joined tables with 'AsgnCategory'
                  // for retrieval of assignment category names to match
                  // with the given category.
                  join categories in db.AsgnCategory
                  on classes.ClassId equals categories.ClaId
                  where categories.Name == category

                  // Now join the prior data with assignments to search
                  // for all assignments with the given assignment name.
                  // At this point, there should only be one assignment
                  // for the given name, for the given category, for the
                  // given class, for the given course.
                  join assignments in db.Assignment
                  on categories.CatId equals assignments.CatId
                  where assignments.Name == asgname

                  // TODO: Did we decide to allow multiple submissions?
                  // Or was this something we changed in the ER model
                  // beforehand?

                  // Search for all submissions by the given student. At
                  // this point there should only be a single submission
                  // for the given assignment.
                  join submissions in db.Submission
                  on assignments.AId equals submissions.AId
                  where submissions.UId == uid

                  select submissions;

      // TODO: In the case where a row already exists for a given table,
      // and that row requires 1 or more columns be updated, is there ever
      // a need for an anonymous class? Either way, what exactly is the
      // problem when a query such as follows is iterated over in the
      // foreach loop, attempting to update the specified columns?
      // 
      // select new {
      //          submissions.UId,
      //          submissions.Score,
      //          submissions.AId
      //        };
      //
      // foreach (...) { // update cols };
      //
      // In other words, how do I make the following code work with
      // anonymous types? Or is this never necessary?

      foreach (Submission s in query) {
        s.Score = score;
      }

      // TODO: What is the difference between 'SaveChanges' and 'SubmitChanges'?
      // The following works just fine but online examples suggest the latter.
      
      // TODO: What is the scope of the local database? For example, would it be
      // possible to invoke 'UpdateGrades' before 'SaveChanges' and simply leave
      // out the invocation of 'SaveChanges' within the 'UpdateGrades' method?
      // What is the best practice? Also, confirm that 'UpdateGrades' will not
      // be called here if there is an exception thrown from 'SaveChanges' before
      // calling 'UpdateGrades'.

      try {
        db.SaveChanges(); // Update score for this assignment.
        UpdateGrade(subject, num, season, year, uid); // Update class grade.
      } catch (Exception e) {
        Debug.WriteLine(e.Message);
        return Json(new { success = false });
      }

      return Json(new { success = true });
    } // GradeSubmission

    /// <summary>
    /// Returns a JSON array of the classes taught by the specified professor
    /// Each object in the array should have the following fields:
    /// "subject" - The subject abbreviation of the class (such as "CS")
    /// "number" - The course number (such as 6016)
    /// "name" - The course name
    /// "season" - The season part of the semester in which the class is taught
    /// "year" - The year part of the semester in which the class is taught
    /// </summary>
    /// <param name="uid">The professor's uid</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetMyClasses(string uid) {
   
      var query = from classes in db.Class
                  join courses in db.Course
                  on classes.CourseId equals courses.CourseId
                  where classes.UId == uid

                  select new {
                    courses.Subject,
                    courses.Number,
                    courses.Name,
                    classes.Season,
                    classes.Year
                  };

      return Json(query.ToArray());
    } // GetMyClasses

    /// <summary>
    /// A helper method to auto-update letter grades. This method updates the
    /// grade for a single class for a single student. Multiple classes and or
    /// students will require sequential invocations.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class for which to update the grade</param>
    /// <param name="year">The year part of the semester for the class for which to update the grade</param>
    /// <param name="uid">The uid of the student who's grade is being updated</param>
    private void UpdateGrade(string subject, int num, string season, int year, string uid) {

      // TODO: For a more efficient execution, update this method to take the
      // class ID and student's uID as the only two parameters.

      // TODO: Should the max grade be the weight of that category if there are
      // no assignments in the other categories?

      // This query contains a single row representing this class. The class ID
      // will be used to query the assignment categories for this class
      // (implemented immediately below this query) as well as the enrollments
      // for this class (near the end of this method).
      var query1 = from classes in db.Class
                   join courses in db.Course
                   on classes.CourseId equals courses.CourseId

                   where courses.Subject == subject &&
                   courses.Number == num &&
                   classes.Season == season &&
                   classes.Year == year

                   select classes; // Class ID is the important attribute here.

      // Select all assignment categories for this class.
      var query2 = from classes in query1
                  join categories in db.AsgnCategory
                  on classes.ClassId equals categories.ClaId
                  select categories;

      int classWeight = 0;
      float numericGradeTotal = 0;

      foreach (AsgnCategory cat in query2) {

        int catTotalPoints = 0;
        int catEarned = 0;

        // Select all assignments for this category.
        var query3 = from categories in query2
                     join assignments in db.Assignment
                     on categories.CatId equals assignments.CatId
                     where assignments.CatId == cat.CatId
                     select assignments;

        if (query3.Count() == 0) {
          continue;
        }

        foreach (Assignment asg in query3) {

          catTotalPoints += asg.Points;
          
          // Select all submissions of this assignment for for the provided uID.
          // This assumes there may only be a single submission per student per
          // assignment.
          var query4 = from assignments in query3
                       join submissions in db.Submission
                       on assignments.AId equals submissions.AId
                       where submissions.UId == uid &&
                       assignments.AId == asg.AId
                       select submissions;

          // This query should return either 0 or 1 (whether or not the student
          // has submitted an assignment. Unsubmitted assignments are
          // given a score of 0.
          
          if (query4.Count() > 0) {
            catEarned += query4.First().Score;
          }

        }

        // For each assignment category do this
        float catPercent = (catEarned / (float)catTotalPoints);
        float catScore = catPercent * cat.Weight;

        numericGradeTotal += (int)catScore;
        classWeight += cat.Weight;
      }

      numericGradeTotal *= (100f / (float)classWeight); // Scale

      // Select all enrollments for this class by the provided uID.
      var query5 = from classes in query1
                   join enrollments in db.Enrolled
                   on classes.ClassId equals enrollments.ClaId
                   where enrollments.UId == uid
                   select enrollments;

      // Convert to letter grade here?
      query5.First().Grade = (int)numericGradeTotal;

      // TODO: Would it be better to place this in a try-catch here and return
      // false on error? This would be redirecting to another try-catch outside
      // this method. What is best?

      db.SaveChanges(); // Update the database with the new numeric grade.
    }

    /// <summary>
    /// Returns the letter grade representation of the numeric grade.
    /// </summary>
    /// <param name="numericGrade">The numeric representation.</param>
    /// <returns></returns>
    private string convertToLetterGrade(int numericGrade) {

      string letterGrade = "";

      if (numericGrade >= 93)
        letterGrade = "A";
      else if (numericGrade >= 90)
        letterGrade = "A-";
      else if (numericGrade >= 87)
        letterGrade = "B+";
      else if (numericGrade >= 83)
        letterGrade = "B";
      else if (numericGrade >= 80)
        letterGrade = "B-";
      else if (numericGrade >= 77)
        letterGrade = "C+";
      else if (numericGrade >= 73)
        letterGrade = "C";
      else if (numericGrade >= 70)
        letterGrade = "C-";
      else if (numericGrade >= 67)
        letterGrade = "D+";
      else if (numericGrade >= 63)
        letterGrade = "D";
      else if (numericGrade >= 60)
        letterGrade = "D-";
      else
        letterGrade = "E";

      return letterGrade;
    }

  }
}