using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using FinalProject.DATA.EF;
using Microsoft.AspNet.Identity;

namespace FinalProject.UI.MVC.Controllers
{
    public class LessonsController : Controller
    {
        private LMSEntities db = new LMSEntities();

        // GET: Lessons
        public ActionResult Index()
        {
            var lessons = db.Lessons.Include(l => l.Cours);
            return View(lessons.ToList());
        }

        [Authorize(Roles = "Admin, Employee, Manager")]
        // GET: Lessons/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }

            if (User.IsInRole("Employee"))
            {
                LessonView lv = new LessonView();
                lv.UserId = User.Identity.GetUserId();
                lv.DateViewed = DateTime.Now;
                lv.LessonId = id;

                LessonView lvDupe = db.LessonViews.Where(x => x.LessonId == id && x.UserId == lv.UserId).SingleOrDefault();
                if (lvDupe == null)
                {
                    db.LessonViews.Add(lv);
                    db.SaveChanges();
                }


                int nbrLessons = db.Lessons.Where(x => x.CourseId == lesson.CourseId).Count();
                string userid = User.Identity.GetUserId();

                int nbrLessonViews = db.LessonViews.Where(x => x.Lesson.CourseId == lesson.CourseId && x.UserId == userid).Count();

                
                if (nbrLessons == nbrLessonViews)
                {
                   
                    CourseCompletion cc = new CourseCompletion();
                    cc.UserId = User.Identity.GetUserId();
                    cc.DateCompleted = DateTime.Now;
                    cc.CourseId = lesson.CourseId;

                    db.CourseCompletions.Add(cc);
                    db.SaveChanges();

                    var currentUserDetails = db.UserDetails.Where(x => x.UserId == cc.UserId).SingleOrDefault(); 

                    

                        
              
                    string messageBody2 = $" {currentUserDetails.FirstName} {currentUserDetails.LastName} has completed the following course: {lesson.Cours.CourseName} on {cc.DateCompleted}.";

                    MailMessage m = new MailMessage("noreply@kccodes.com", "kristin.cartmill@outlook.com", "Message From Edgewater Medical Center LMS", messageBody2);

                    m.IsBodyHtml = true;

                    SmtpClient mailServer = new SmtpClient("mail.kccodes.com");
                    mailServer.Credentials = new NetworkCredential("noreply@kccodes.com", "Puppies#1");

                    try
                    {
                        //attempt to send the email
                        mailServer.Send(m);
                    }
                    catch (System.Exception ex)
                    {
                        ////if they land here, sending email failed
                        //ViewBag.CustomerMessage =
                        //    $"We're sorry, the request failed. Please try again later.<h2>Error</h2>{ex.StackTrace}";
                        //return View();
                    }
                }
            }

            if (lesson.VideoURL !=null){


                var v = lesson.VideoURL.IndexOf("v=");
                var amp = lesson.VideoURL.IndexOf("&", v);
                string vid;
                // if the video id is the last value in the url
                if (amp == -1)
                {
                    vid = lesson.VideoURL.Substring(v + 2);
                    // if there are other parameters after the video id in the url
                }
                else
                {
                    vid = lesson.VideoURL.Substring(v + 2, amp - (v + 2));
                }
                ViewBag.VideoID = vid;

            }


            return View(lesson);
        }

        [Authorize(Roles = "Admin")]
        // GET: Lessons/Create
        public ActionResult Create()
        {
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName");
            return View();
        }

        [Authorize(Roles = "Admin")]
        // POST: Lessons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LessonId,LessonTitle,CourseId,Introduction,VideoURL,PdfFilename,IsActive")] Lesson lesson, HttpPostedFileBase lessonPdf)
        {
            if (ModelState.IsValid)
            {

                
                string pdfName = "NoDocument.pdf";//default name if none uploaded

                if (lessonPdf != null)
                {
                    pdfName = lessonPdf.FileName;
                    //this code is alternative to using the substring to get the extension
                    //the substring code will operate here as well.
                    //file was uploaded - process it.
                    string ext = pdfName.Substring(pdfName.LastIndexOf('.'));

                    //create a list of good extensions
                    string[] allowedExtentions = { ".pdf" };

                    if (allowedExtentions.Contains(ext.ToLower()))
                    {
                        //we can save the image name under numerous approaches,
                        //but the most important thing is that its unique
                        //quickest way = GUID
                        lessonPdf.SaveAs(Server.MapPath("/Content/images/PDFs/" + pdfName));

                    }

                    else
                    {
                        pdfName = "NoDocument.pdf";

                    }
                }
                lesson.PdfFilename = pdfName;


                db.Lessons.Add(lesson);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", lesson.CourseId);
            return View(lesson);
        }

        [Authorize(Roles = "Admin")]
        // GET: Lessons/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", lesson.CourseId);
            return View(lesson);
        }

        [Authorize(Roles = "Admin")]
        // POST: Lessons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LessonId,LessonTitle,CourseId,Introduction,VideoURL,PdfFilename,IsActive")] Lesson lesson, HttpPostedFileBase lessonPdf)
        {
            if (ModelState.IsValid)
            {
                if (lessonPdf != null)
                {
                    string pdfName = lessonPdf.FileName;

                    string ext = pdfName.Substring(pdfName.LastIndexOf('.'));

                    //create a list of good extensions
                    string[] allowedExtentions = { ".pdf" };

                    if (allowedExtentions.Contains(ext.ToLower()))
                    {

                        lessonPdf.SaveAs(Server.MapPath("/Content/images/PDFs/" + pdfName));

                        lesson.PdfFilename = pdfName;
                    }

                    //else
                    //{
                    //    pdfName = "NoDocument.pdf";
                    //}                   

                }

                
                db.Entry(lesson).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("CourseLessons", new { id = lesson.CourseId});
            }
        
            ViewBag.CourseId = new SelectList(db.Courses, "CourseId", "CourseName", lesson.CourseId);
            return View(lesson);
        }




        [Authorize(Roles = "Admin")]
        // GET: Lessons/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return HttpNotFound();
            }
            return View(lesson);
        }

        [Authorize(Roles = "Admin")]
        // POST: Lessons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Lesson lesson = db.Lessons.Find(id);
            db.Lessons.Remove(lesson);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult CourseLessons(int id)
        {
            var lessons = db.Lessons.Where(x => x.CourseId == id);
            return View(lessons.ToList());
        }
    }


}
