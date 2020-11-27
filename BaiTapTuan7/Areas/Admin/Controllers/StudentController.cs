﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BaiTapTuan7.Models;
using PagedList;

namespace BaiTapTuan7.Areas.Admin.Controllers
{
    [AuthorizeController]
    public class StudentController : Controller
    {
        TechEduEntities db = new TechEduEntities();
        // GET: Student
        public ActionResult Index(int page = 1,int pageSize = 10)
        {

            var listStudent = db.tb_Student.OrderByDescending(m => m.FirstName).ToPagedList(page,pageSize);
            return View(listStudent);
        }
        public ActionResult Details(int id)
        {
            var stu = db.tb_Student.Find(id);
            if (stu == null)
                return new HttpNotFoundResult();
            return View(stu);
        }
        public ActionResult EditStudent(int id)
        {
            tb_Student stu = db.tb_Student.FirstOrDefault(m => m.StudentId == id);
            return View("EditStudent", stu);
        }
        [HttpPost]
        public ActionResult EditStudent(tb_Student stu)
        {
            using (TechEduEntities db = new TechEduEntities())
            {
                if (ModelState.IsValid)
                {
                    tb_Student oldstu = db.tb_Student.FirstOrDefault(m => m.StudentId == stu.StudentId);
                    if (oldstu != null)
                    {
                        oldstu.FirstName = stu.FirstName;
                        oldstu.LastName = stu.LastName;
                        oldstu.Gmail = stu.Gmail;
                        oldstu.PhoneNumber = stu.PhoneNumber;
                        oldstu.DateOfBirth = stu.DateOfBirth;
                        oldstu.PlaceOfBirth = stu.PlaceOfBirth;
                        HttpPostedFileBase upload = Request.Files["image"];
                        if(upload.FileName != "")
                        {
                            using (var binaryReader = new BinaryReader(upload.InputStream))
                                oldstu.Images = binaryReader.ReadBytes(upload.ContentLength);
                        }
                        db.Entry(oldstu).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index", "Student");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Edit Student failed");
                        return View("EditStudent", stu);
                    }
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
        }
    }
}