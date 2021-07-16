using BigSchool.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BigSchool.Controllers
{
    public class CourseController : Controller
    {
        // GET: Course
        public ActionResult Create()
        {
            //get list category
            BigSchoolContext context = new BigSchoolContext();
            Course objCourse = new Course();
            objCourse.ListCategory = context.Category.ToList();
            return View(objCourse);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Course objCourse)
        {
            BigSchoolContext context = new BigSchoolContext();
            //không xét valid LecturerId vì bằng user đăng nhập
            ModelState.Remove("LecturerId");
            if (!ModelState.IsValid)
            {
                objCourse.ListCategory = context.Category.ToList();
                return View("Create", objCourse);
            }

            //lay login user ID
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            objCourse.LectureId = user.Id;
            //add vào csdl
            context.Course.Add(objCourse);
            context.SaveChanges();
            //tro ve home, action index
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Attending ()
        {
            BigSchoolContext context = new BigSchoolContext();
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var listAttendance = context.Attendance.Where(p => p.Attendee == currentUser.Id).ToList();
            var course = new List<Course>();
            foreach (Attendance temp in listAttendance)
            {
                Course objCourse = temp.Course;
                objCourse.LectureId = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(objCourse.LectureId).Name;
                course.Add(objCourse);
            }
            return View(course);
        }
        public ActionResult Mine ()
        {
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            BigSchoolContext context = new BigSchoolContext();
            var course = context.Course.Where(c => c.LectureId == currentUser.Id && c.DateTime > DateTime.Now ).ToList();
            foreach (Course i in course)
            {
                i.LectureName = currentUser.Name;
            }
            return View(course);
        }
        public ActionResult ListCourse ()
        {
            BigSchoolContext context = new BigSchoolContext();
            var listCourse = context.Course.ToList();
            return View(listCourse);

        }

        public ActionResult Edit(int? id)
        {
            BigSchoolContext context = new BigSchoolContext();
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = context.Course.Where(p => p.Id == id && p.LectureId == currentUser.Id).FirstOrDefault();
            if (course == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(context.Category, "Id", "Name", course.CategoryId);
            return View(course);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,LectureId,Place,DateTime,CategoryId")] Course course)
        {
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            BigSchoolContext context = new BigSchoolContext();
            course.LectureId = currentUser.Id;
            ModelState.Remove("LectureId");
            if (ModelState.IsValid)
            {
                context.Entry(course).State = EntityState.Modified;
                context.SaveChanges();
                return RedirectToAction("Mine");
            }
            ViewBag.CategoryId = new SelectList(context.Category, "Id", "Name", course.CategoryId);
            return View(course);
        }
        public ActionResult Delete(int? id)
        {
            BigSchoolContext context = new BigSchoolContext();
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = context.Course.Where(c => c.Id == id && c.LectureId == currentUser.Id).FirstOrDefault();
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCourse(int id)
        {
            BigSchoolContext context = new BigSchoolContext();
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            Course course = context.Course.Where(c => c.Id == id && c.LectureId == currentUser.Id).FirstOrDefault();
            Attendance attendance = context.Attendance.Where(a => a.CourseId == id).FirstOrDefault();
            context.Course.Remove(course);
            if (attendance != null)
            {
                context.Attendance.Remove(attendance);

            }
            context.SaveChanges();
            return RedirectToAction("Mine");
        }
        public ActionResult Index()
        {
            BigSchoolContext context = new BigSchoolContext();
            var upcommingCourse = context.Course.Where(p => p.DateTime >
            DateTime.Now).OrderBy(p => p.DateTime).ToList();
            //lấy user login hiện tại

            var userID = User.Identity.GetUserId();
            foreach (Course i in upcommingCourse)

            {
                //tìm Name của user từ lectureid
                ApplicationUser user =

                System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>(
                ).FindById(i.LectureId);
                i.Name = user.Name;
                //lấy ds tham gia khóa học
                if (userID != null)

                {
                    i.isLogin = true;
                    //ktra user đó chưa tham gia khóa học

                    Attendance find = context.Attendance.FirstOrDefault(p =>

                    p.CourseId == i.Id && p.Attendee == userID);
                    if (find == null)
                        i.isShowGoing = true;
                    //ktra user đã theo dõi giảng viên của khóa học ?

                    Following findFollow = context.Following.FirstOrDefault(p =>

                    p.FollowerId == userID && p.FolloweeId == i.LectureId);

                    if (findFollow == null)
                        i.isShowFollow = true;
                }
            }
            return View(upcommingCourse);
        }
    }
}