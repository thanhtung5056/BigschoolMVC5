using Bigschool.Models;
using Bigschool.ViewModel;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bigschool.Controllers
{
    public class CoursesController : Controller
    {
        [Authorize]
        public ActionResult Mine()
        {
            var userId = User.Identity.GetUserId();
            var courses = _dbContext.Courses
                .Where(c => c.LecturerId == userId && c.Datetime > DateTime.Now)
                .Include(l => l.Lecturer)
                .Include(c => c.Category)
                .ToList();
            return View(courses);
        }
        [Authorize]
        public ActionResult Attending()
        {
            var userId = User.Identity.GetUserId();
            var courses = _dbContext.Attendances
                .Where(a => a.AttendeeId == userId)
                .Select(a => a.Course)
                .Include(l => l.Lecturer)
                .Include(l => l.Category)
                .ToList();
            var viewModel = new CoursesViewModel
            {
                UpcommingCourses = courses,
                ShowAction = User.Identity.IsAuthenticated
            };
            return View(viewModel);
        }
        private readonly ApplicationDbContext _dbContext;
        // GET: Courses
        public CoursesController()
        {
            _dbContext = new ApplicationDbContext();
            
        }
        public ActionResult Create()
        {
            var viewModel = new CourseViewModel
            {
                categories = _dbContext.categories.ToList()
            };
            return View(viewModel);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CourseViewModel viewModel)
        {
            if(!ModelState.IsValid)
            {
                viewModel.categories = _dbContext.categories.ToList();
                return View("Create", viewModel);
            }
            var course = new Course
            {
                LecturerId=User.Identity.GetUserId(),
                Datetime= viewModel.GetDateTime(),
                CategoryId= viewModel.Category,
                Place =viewModel.Place
            };
            _dbContext.Courses.Add(course);
            _dbContext.SaveChanges();
            return RedirectToAction("Index","Home");
        }
    }
}