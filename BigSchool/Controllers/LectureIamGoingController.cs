using BigSchool.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BigSchool.Controllers
{
    public class LectureIamGoingController : Controller
    {
        // GET: LectureIamGoing
        public ActionResult LectureIamGoing()
        {
            ApplicationUser currentUser =
            System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
            .FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            BigSchoolContext context = new BigSchoolContext();
            //danh sách giảng viên được theo dõi bởi người dùng (đăng nhập) hiện tại
            var listFollwee = context.Following.Where(p => p.FollowerId ==

            currentUser.Id).ToList();

            //danh sách các khóa học mà người dùng đã đăng ký
            var listAttendances = context.Attendance.Where(p => p.Attendee ==

            currentUser.Id).ToList();

            var courses = new List<Course>();
            foreach (var course in listAttendances)

            {
                foreach (var item in listFollwee)
                {
                    if (item.FolloweeId == course.Course.LectureId)
                    {
                        Course objCourse = course.Course;
                        objCourse.LectureName =
                        System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                        .FindById(objCourse.LectureId).Name;
                        courses.Add(objCourse);
                    }
                }
            }
            return View(courses);
        }
    }
}