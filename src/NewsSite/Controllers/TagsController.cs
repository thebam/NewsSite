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
    public class TagsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TagsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tags
        public async Task<IActionResult> Index()
        {
            var query =
            from t in _context.Tag
            select new TagViewModel()
            {
                TagId= t.TagId,
                TagName = t.TagName,
                Enabled = t.Enabled,
                DateCreated = t.DateCreated,
                UsageCnt = (from m in _context.MediaKitFileTag
                         where m.TagId == t.TagId
                         select t).Count() + (from a in _context.ArticleTag
                                              where a.TagId == t.TagId
                                              select t).Count()
            };

            return View(await query.OrderByDescending(t => t.Enabled).ThenBy(ta=>ta.TagName).ToListAsync<TagViewModel>());
        }



        // GET: Tags/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tags/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TagId,Enabled,TagName")] Tag tag)
        {
            //if (ModelState.IsValid)
            //{
            //    Tag tempTag = _context.Tag.SingleOrDefault(t => t.TagName.ToLower().Trim() == tag.TagName.ToLower().Trim());
            //    if (tempTag == null)
            //    {
            //        tag.DateCreated = DateTime.Now;
            //        tag.Enabled = true;
            //        _context.Add(tag);
            //        await _context.SaveChangesAsync();
            //        return RedirectToAction("Index");
            //    }
            //    else
            //    {
            //        return RedirectToAction("Index");
            //    }
            //}
            //return View(tag);
            if (CreateTag(tag) == null)
            {
                ViewBag.errorMessage = "A tag named '" + tag.TagName + "' already exists. Please try again.";
                ViewBag.errorRedirect = "Create";
                return View("Error");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> CreateAjax()
        {
            //string tagName = "";
            //tagName = (string)Request.Query["tagName"];
            //Tag tempTag = _context.Tag.SingleOrDefault(t => t.TagName.ToLower().Trim() == tagName.ToLower().Trim());
            //if (tempTag == null)
            //{
            //    Tag tag = new Tag();
            //    tag.TagName = tagName.Trim();
            //    tag.DateCreated = DateTime.Now;
            //    tag.Enabled = true;
            //    _context.Add(tag);
            //    await _context.SaveChangesAsync();
            //    return Json(new { id = tag.TagId, tag = tag.TagName });
            //}
            //else
            //{
            //    return Json(new { status = "error", message = "Tag already exists." });
            //}
            string tagName = "";
            tagName = (string)Request.Query["tagName"];
            Tag tag = new Tag()
            {
                TagName = tagName.Trim()
            };
            if (CreateTag(tag) == null)
            {
                return Json(new { status = "error", message = "Tag already exists." });
            }
            else
            {
                return Json(new { id = tag.TagId, tag = tag.TagName });
            }
        }


        private Tag CreateTag(Tag tag) {
            Tag tempTag = _context.Tag.SingleOrDefault(t => t.TagName.ToLower().Trim() == tag.TagName.ToLower().Trim());
            if (tempTag == null)
            {
                tag.TagName = tag.TagName.Trim();
                tag.DateCreated = DateTime.Now;
                tag.Enabled = true;
                _context.Add(tag);
                _context.SaveChanges();
            }
            else {
                tag = null;
            }
            return tag;
        }

        // GET: Tags/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var tag = await _context.Tag.SingleOrDefaultAsync(m => m.TagId == id);

            var query =
    from t in _context.Tag
    where t.TagId == id
    select new TagViewModel()
    {
        TagId = t.TagId,
        TagName = t.TagName,
        Enabled = t.Enabled,
        DateCreated = t.DateCreated,
        Articles = (from a in _context.Article
                   from at in _context.ArticleTag
                   where a.ArticleId == at.ArticleId && at.TagId == id
                   select a
                   ).ToList<Article>(),
        //Change to MediaKitViewModel
        MediaKitFiles = (from m in _context.MediaKitFile
                    from mt in _context.MediaKitFileTag
                    where m.MediaKitFileId == mt.MediaKitFileId && mt.TagId == id
                    select( new MediaKitFileViewModel() {
                        MediaKitFileId = m.MediaKitFileId,
                        IconURL = m.URL,
                        URL = m.URL,
                        Description = m.Description
                    } )
                   ).ToList<MediaKitFileViewModel>()
    };


            if (query == null)
            {
                return NotFound();
            }
            return View(query.FirstOrDefault());
        }

        // POST: Tags/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TagId,Enabled,TagName")] Tag tag)
        {
            if (id != tag.TagId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tag);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TagExists(tag.TagId))
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
            return View(tag);
        }

        // GET: Tags/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tag = await _context.Tag.SingleOrDefaultAsync(m => m.TagId == id);
            if (tag == null)
            {
                return NotFound();
            }

            return View(tag);
        }

        // POST: Tags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tag = await _context.Tag.SingleOrDefaultAsync(m => m.TagId == id);
            _context.MediaKitFileTag.RemoveRange(_context.MediaKitFileTag.Where(t => t.TagId == id));
            _context.ArticleTag.RemoveRange(_context.ArticleTag.Where(t => t.TagId == id));
            _context.Tag.Remove(tag);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool TagExists(int id)
        {
            return _context.Tag.Any(e => e.TagId == id);
        }
    }
}
