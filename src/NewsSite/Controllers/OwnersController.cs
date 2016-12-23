using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewsSite.Data;
using NewsSite.Models;
using Microsoft.AspNetCore.Hosting;

namespace NewsSite.Controllers
{
    public class OwnersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IHostingEnvironment hostingEnv;
        public OwnersController(ApplicationDbContext context, IHostingEnvironment env)
        {
            _context = context;
            hostingEnv = env;   
        }

        // GET: Owners
        public async Task<IActionResult> Index()
        {
            return View(await _context.Owner.OrderBy(o=>o.Name).ToListAsync());
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

            //Get all media Kit files associated with this Owner. Must be done this way to get the Tag Names instead of just the TagId
            var kitfiles = _context.MediaKitFile
                .Where(o => o.OwnerId.Equals(owner.OwnerId))
                .Select(k => new MediaKitFileViewModel{
                    MediaKitFileId = k.MediaKitFileId,
                    URL = k.URL,
                    IconURL = k.URL,
                    Description = k.Description,
                    CopyrightDate = k.CopyrightDate,
                    TagNames = k.MediaKitFileTags
                    .Join(_context.Tag, mt => mt.TagId, t => t.TagId, (mt, t) => new { mt, t })
                    .Select(tt => new TagName
                    {
                        Name = tt.t.TagName,
                        Id = tt.t.TagId
                    }).ToList()
            });

            //Get all enabled tags
            List<Tag> tags = _context.Tag.Where(t => t.Enabled == true).OrderBy(g => g.TagName).ToList<Tag>();
            ViewBag.tags = tags;
            ViewBag.mediaKitFiles = kitfiles;
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
            //if (ModelState.IsValid)
            //{
            //    Owner tempOwner = new Owner();
            //    tempOwner = _context.Owner.SingleOrDefault(o => o.Name.ToLower() == owner.Name.ToLower().Trim());
            //    if (tempOwner == null)
            //    {
            //        _context.Add(owner);
            //        await _context.SaveChangesAsync();
            //    }
            //    else {
            //        ViewBag.errorMessage = "An author named '" + owner.Name+ "' already exists. Please try again.";
            //        ViewBag.errorRedirect = "Create";
            //        return View("Error");
            //    }
            //    return RedirectToAction("Index");
            //}
            //return View(owner);
            if (CreateOwner(owner) == null)
            {
                ViewBag.errorMessage = "An author named '" + owner.Name + "' already exists. Please try again.";
                ViewBag.errorRedirect = "Create";
                return View("Error");
            }
            else {
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateAjax()
        {
            //string message = "";
            //if (!String.IsNullOrEmpty(Request.Form["address"].ToString().Trim())) {
            //    Owner tempOwner = new Owner();
            //    string ownerName = Request.Form["name"].ToString().Trim();
            //    tempOwner = _context.Owner.SingleOrDefault(o=>o.Name == ownerName);
            //    if (tempOwner==null) {
            //        Owner newOwner = new Owner();
            //        newOwner.Address = Request.Form["address"].ToString().Trim();
            //        newOwner.DateCreated = DateTime.Now;
            //        newOwner.Email = Request.Form["email"].ToString().Trim();
            //        newOwner.Enabled = true;
            //        newOwner.Name = ownerName;
            //        newOwner.Phone = Request.Form["phone"].ToString().Trim();
            //        newOwner.SocialMedia = Request.Form["socialMedia"].ToString().Trim();
            //        newOwner.Website = Request.Form["website"].ToString().Trim();
            //        _context.Owner.Add(newOwner);
            //        await _context.SaveChangesAsync();
            //        message = "test" + newOwner.OwnerId;
            //    }
            //} else {
            //    message = "error";
            //}
            //return Json(message);

            string message = "";
            Owner newOwner = new Owner();
            newOwner.Address = Request.Form["address"].ToString().Trim();
            newOwner.DateCreated = DateTime.Now;
            newOwner.Email = Request.Form["email"].ToString().Trim();
            newOwner.Enabled = true;
            newOwner.Name = Request.Form["name"].ToString().Trim(); ;
            newOwner.Phone = Request.Form["phone"].ToString().Trim();
            newOwner.SocialMedia = Request.Form["socialMedia"].ToString().Trim();
            newOwner.Website = Request.Form["website"].ToString().Trim();
            if (CreateOwner(newOwner) == null)
            {
                message = "error";
            }
            else
            {
                message = "test" + newOwner.OwnerId;
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

            //Find all media kit files associated with the current ownerId. Loop thru them and delete the relationships between media files and tags, media files and articles, and media files records. Then delete the physical files
            List<MediaKitFile> mediaKitFiles = _context.MediaKitFile.Where(o => o.OwnerId == id).ToList();
            foreach (MediaKitFile mediaKitFile in mediaKitFiles)
            {
                _context.MediaKitFileTag.RemoveRange(_context.MediaKitFileTag.Where(m => m.MediaKitFileId == mediaKitFile.MediaKitFileId));
                _context.ArticleMediaKitFile.RemoveRange(_context.ArticleMediaKitFile.Where(m => m.MediaKitFileId == mediaKitFile.MediaKitFileId));
                _context.MediaKitFile.Remove(mediaKitFile);
                string filename = hostingEnv.WebRootPath + $@"\mediakitfiles\{mediaKitFile.URL}";
                if (System.IO.File.Exists(filename))
                {
                    System.IO.File.Delete(filename);
                }
            }

            _context.Owner.Remove(owner);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool OwnerExists(int id)
        {
            return _context.Owner.Any(e => e.OwnerId == id);
        }

        public bool OwnerExistsByName(String name)
        {
            return _context.Owner.Any(e => e.Name == name);
        }

        private Owner CreateOwner(Owner owner) {
            if (ModelState.IsValid)
            {
                Owner tempOwner = new Owner();
                tempOwner = _context.Owner.SingleOrDefault(o => o.Name.ToLower() == owner.Name.ToLower().Trim());
                if (tempOwner == null)
                {
                    _context.Add(owner);
                    _context.SaveChanges();
                }
                else
                {
                    owner = null;
                }
            }
            else
            {

                owner = null;
            }
            return owner;
        }

    }
}
