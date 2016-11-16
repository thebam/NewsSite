using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewsSite.Data;
using NewsSite.Models;

namespace NewsSite.Controllers
{
    public class OwnersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OwnersController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Owners
        public async Task<IActionResult> Index()
        {
            return View(await _context.Owner.ToListAsync());
        }

        // GET: Owners/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _context.Owner.SingleOrDefaultAsync(m => m.OwnerId == id);
            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }

        // GET: Owners/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Owners/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OwnerId,Address,DateCreated,Email,Enabled,Name,Phone,SocialMedia,Website")] Owner owner)
        {
            if (ModelState.IsValid)
            {
                _context.Add(owner);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(owner);
        }


        [HttpPost]
        public async Task<IActionResult> CreateAjax()
        {
            string message = "";
            if (!String.IsNullOrEmpty(Request.Form["address"].ToString().Trim())) {
                Owner tempOwner = new Owner();
                string ownerName = Request.Form["name"].ToString().Trim();
                tempOwner = _context.Owner.SingleOrDefault(o=>o.Name == ownerName);
                if (tempOwner!=null) {
                    Owner newOwner = new Owner();
                    newOwner.Address = Request.Form["address"].ToString().Trim();
                    newOwner.DateCreated = DateTime.Now;
                    newOwner.Email = Request.Form["email"].ToString().Trim();
                    newOwner.Enabled = true;
                    newOwner.Name = ownerName;
                    newOwner.Phone = Request.Form["phone"].ToString().Trim();
                    newOwner.SocialMedia = Request.Form["socialMedia"].ToString().Trim();
                    newOwner.Website = Request.Form["website"].ToString().Trim();
                    _context.Owner.Add(newOwner);
                    await _context.SaveChangesAsync();
                    message = "test" + newOwner.OwnerId;
                }
            } else {
                message = "error";
            }
            return Json(message);
        }



        // GET: Owners/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _context.Owner.SingleOrDefaultAsync(m => m.OwnerId == id);
            if (owner == null)
            {
                return NotFound();
            }
            return View(owner);
        }

        // POST: Owners/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OwnerId,Address,DateCreated,Email,Enabled,Name,Phone,SocialMedia,Website")] Owner owner)
        {
            if (id != owner.OwnerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(owner);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OwnerExists(owner.OwnerId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(owner);
        }

        // GET: Owners/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var owner = await _context.Owner.SingleOrDefaultAsync(m => m.OwnerId == id);
            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }

        // POST: Owners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var owner = await _context.Owner.SingleOrDefaultAsync(m => m.OwnerId == id);
            _context.Owner.Remove(owner);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool OwnerExists(int id)
        {
            return _context.Owner.Any(e => e.OwnerId == id);
        }
    }
}
