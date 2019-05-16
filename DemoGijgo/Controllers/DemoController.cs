using DemoGijgo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoGijgo.Controllers
{
    public class DemoController : Controller
    {
        // GET: Demo
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetCustomers(int? page, int? limit, string sortBy, string direction, string companyName, string contactName, string country)
        {
            List<Customers> records;
            int total;
            // step 1
            using (DatabaseContext context = new DatabaseContext())
            {
                // step 2
                var query = (from tempcustomer in context.Customers
                             select tempcustomer);

                // step 3 applying where clause to linq query if companyName is not empty
                if (!string.IsNullOrWhiteSpace(companyName))
                {
                    query = query.Where(q => q.CompanyName.Contains(companyName));
                }

                // step 4 applying where clause to linq query if contactName is not empty
                if (!string.IsNullOrWhiteSpace(contactName))
                {
                    query = query.Where(q => q.ContactName != null && q.ContactName.Contains(contactName));
                }
                // step 5 applying where clause to linq query if country is not empty
                if (!string.IsNullOrWhiteSpace(country))
                {
                    query = query.Where(q => q.Country != null && q.Country.Contains(country));
                }

                // step 6 applying sorting asc to (companyname,contactname,country,city)

                if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(direction))
                {
                    if (direction.Trim().ToLower() == "asc")
                    {
                        switch (sortBy.Trim().ToLower())
                        {
                            case "companyname":
                                query = query.OrderBy(q => q.CompanyName);
                                break;
                            case "contactname":
                                query = query.OrderBy(q => q.ContactName);
                                break;
                            case "country":
                                query = query.OrderBy(q => q.Country);
                                break;
                            case "city":
                                query = query.OrderBy(q => q.City);
                                break;
                        }
                    }
                    else
                    {
                        // step 7 applying sorting desc (companyname,contactname,country,city)

                        switch (sortBy.Trim().ToLower())
                        {
                            case "companyname":
                                query = query.OrderByDescending(q => q.CompanyName);
                                break;
                            case "contactname":
                                query = query.OrderByDescending(q => q.ContactName);
                                break;
                            case "country":
                                query = query.OrderByDescending(q => q.Country);
                                break;
                            case "city":
                                query = query.OrderByDescending(q => q.City);
                                break;
                        }
                    }
                }
                else
                {
                    query = query.OrderBy(q => q.CustomerID);
                }

                // step 8 Total rows count 
                total = query.Count();
                // step 9 Paging 
                if (page.HasValue && limit.HasValue)
                {
                    int start = (page.Value - 1) * limit.Value;
                    records = query.Skip(start).Take(limit.Value).ToList();
                }
                else
                {
                    records = query.ToList();
                }
            }
            // step 10 returing json object
            return this.Json(new { records, total }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Save(Customers record)
        {
            if (ModelState.IsValid)
            {
                using (DatabaseContext context = new DatabaseContext())
                {
                    if (record.CustomerID > 0)
                    {
                        context.Entry(record).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                    else
                    {
                        context.Customers.Add(record);
                    }
                    context.SaveChanges();
                }
                return Json(new { result = true });
            }
            else
            {
                var errorList = (from item in ModelState
                                 where item.Value.Errors.Any()
                                 select item.Value.Errors[0].ErrorMessage).ToList();

                return Json(new { result = false, errorList = errorList });
            }
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                Customers entity = context.Customers.First(p => p.CustomerID == id);
                context.Customers.Remove(entity);
                context.SaveChanges();
            }
            return Json(new { result = true });
        }
    }
}