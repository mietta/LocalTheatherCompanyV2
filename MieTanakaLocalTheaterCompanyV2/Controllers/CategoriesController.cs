using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MieTanakaLocalTheaterCompanyV2.Models;
/// <summary>
/// Name:Mie Tanaka
/// Name:02/03/2017
/// Description: allows users to create, view, edit, delete categories

namespace MieTanakaLocalTheaterCompanyV2.Controllers
{
    public class CategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Categories
        /// <summary>
        /// Returns Categories/Index view filled with a list of all categories
        /// </summary>
        /// <returns> returns list of categories saved on database </returns>
        public ActionResult Index()
        {
                return View(db.Categories.ToList());
        }

        // GET: Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // GET: Categories/Create
        /// <summary>
        /// Returns a blank Categories/Create view of category
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// returns Categories/Index view after input data got updated in the database
        /// if error return Categories/create view with current category data
        /// </summary>
        /// <param name="category"></param>
        /// <returns> Category/Index </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CategoryId,CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET: Categories/Edit/5
        /// <summary>
        /// Returns Categories/Edit view filled with previously selected category name
        /// </summary>
        /// <param name="id"></param>
        /// <returns>category</returns>
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Returns Categories/Index View after updating changed data on database
        /// if error returns current view
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CategoryId,CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        /// <summary>
        /// Returns Categories/Delete view filled with selected category name with delete confirmation button
        /// </summary>
        /// <param name="id"></param>
        /// <returns>category</returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Categories/Delete/5
        /// <summary>
        /// Returns category index view after deleting selected category name on click of confirming delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns> Categories/Index</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try { 
                Category category = db.Categories.Find(id);
                db.Categories.Remove(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }catch(Exception ex)
            {
                ModelState.AddModelError("customError", "Error: " + ex);
                return RedirectToAction("Index");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
