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
    public class MediaKitFilesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MediaKitFilesController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: MediaKitFiles
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.MediaKitFile.Include(m => m.Owner);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: MediaKitFiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mediaKitFile = await _context.MediaKitFile.SingleOrDefaultAsync(m => m.MediaKitFileId == id);
            if (mediaKitFile == null)
            {
                return NotFound();
            }

            return View(mediaKitFile);
        }

        // GET: MediaKitFiles/Create
        public IActionResult Create()
        {
            ViewData["OwnerId"] = new SelectList(_context.Owner, "OwnerId", "Name");
            return View();
        }

        // POST: MediaKitFiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MediaKitFileId,DateCreated,DateModified,Description,Enabled,MediaType,OwnerId,ThumbnailURL,URL")] MediaKitFile mediaKitFile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mediaKitFile);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["OwnerId"] = new SelectList(_context.Owner, "OwnerId", "Name", mediaKitFile.OwnerId);
            return View(mediaKitFile);
        }

        // GET: MediaKitFiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mediaKitFile = await _context.MediaKitFile.SingleOrDefaultAsync(m => m.MediaKitFileId == id);
            if (mediaKitFile == null)
            {
                return NotFound();
            }
            ViewData["OwnerId"] = new SelectList(_context.Owner, "OwnerId", "Name", mediaKitFile.OwnerId);
            return View(mediaKitFile);
        }

        // POST: MediaKitFiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MediaKitFileId,DateCreated,DateModified,Description,Enabled,MediaType,OwnerId,ThumbnailURL,URL")] MediaKitFile mediaKitFile)
        {
            if (id != mediaKitFile.MediaKitFileId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mediaKitFile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MediaKitFileExists(mediaKitFile.MediaKitFileId))
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
            ViewData["OwnerId"] = new SelectList(_context.Owner, "OwnerId", "Name", mediaKitFile.OwnerId);
            return View(mediaKitFile);
        }

        // GET: MediaKitFiles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mediaKitFile = await _context.MediaKitFile.SingleOrDefaultAsync(m => m.MediaKitFileId == id);
            if (mediaKitFile == null)
            {
                return NotFound();
            }

            return View(mediaKitFile);
        }

        // POST: MediaKitFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mediaKitFile = await _context.MediaKitFile.SingleOrDefaultAsync(m => m.MediaKitFileId == id);
            _context.MediaKitFile.Remove(mediaKitFile);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool MediaKitFileExists(int id)
        {
            return _context.MediaKitFile.Any(e => e.MediaKitFileId == id);
        }
    }
}
