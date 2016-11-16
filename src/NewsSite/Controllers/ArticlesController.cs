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
    public class ArticlesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ArticlesController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Articles
        public async Task<IActionResult> Index()
        {
            return View(await _context.Article.ToListAsync());
        }

        // GET: Articles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Article.SingleOrDefaultAsync(m => m.ArticleId == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // GET: Articles/Create
        public IActionResult Create()
        {
            List<Tag> tags = _context.Tag.ToList<Tag>();
            List<Owner> owners = _context.Owner.ToList<Owner>();
            List<MediaKitFile> mediaFiles = _context.MediaKitFile.ToList<MediaKitFile>();
            ViewBag.tags = tags;
            ViewBag.owners = owners;
            ViewBag.mediaFiles = mediaFiles;
            return View();
        }

        // POST: Articles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Article article)
        {
            if (ModelState.IsValid)
            {
                article.DateCreated = DateTime.Now;
                article.DateModified = DateTime.Now;
                _context.Article.Add(article);
                await _context.SaveChangesAsync();
                
                List<ArticleTag> articleTags = new List<ArticleTag>();
                string newArticleTags = Request.Form["ArticleTags"].ToString();

                if (newArticleTags.Substring(0, 1) == ",") {
                    newArticleTags = newArticleTags.Substring(1,newArticleTags.Length-1);
                }

                if (newArticleTags.Substring(newArticleTags.Length-1, 1) == ",")
                {
                    newArticleTags = newArticleTags.Substring(0, newArticleTags.Length-1);
                }

                string[] tags = newArticleTags.Split(',');

                for (var x=0;x<tags.Length;x++) {
                    _context.ArticleTag.Add(new ArticleTag() { ArticleId = article.ArticleId, TagId = Convert.ToInt32(tags[x]) });
                }
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return View(article);
        }

        // GET: Articles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Article.SingleOrDefaultAsync(m => m.ArticleId == id);
            if (article == null)
            {
                return NotFound();
            }
            return View(article);
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ArticleId,Body,DateCreated,DateModified,EndDate,OGDescription,OGImage,OGTitle,StartDate,Title,URL")] Article article)
        {
            if (id != article.ArticleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(article);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.ArticleId))
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
            return View(article);
        }

        // GET: Articles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Article.SingleOrDefaultAsync(m => m.ArticleId == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.Article.SingleOrDefaultAsync(m => m.ArticleId == id);
            _context.Article.Remove(article);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ArticleExists(int id)
        {
            return _context.Article.Any(e => e.ArticleId == id);
        }
    }
}
