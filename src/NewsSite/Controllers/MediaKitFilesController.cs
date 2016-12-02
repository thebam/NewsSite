using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewsSite.Data;
using NewsSite.Models;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;

namespace NewsSite.Controllers
{
    public class MediaKitFilesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IHostingEnvironment hostingEnv;
        public MediaKitFilesController(ApplicationDbContext context,IHostingEnvironment env)
        {
            _context = context;
            this.hostingEnv = env;
        }

        // GET: MediaKitFiles
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.MediaKitFile.Include(m => m.Owner);
            return View(await applicationDbContext.ToListAsync());
        }
        [HttpGet]
        public JsonResult GetAllFiles()
        {
            List<ImageLibraryItem> library = new List<ImageLibraryItem>();
            List<MediaKitFile> mediaFiles = _context.MediaKitFile.Where(m => m.Enabled == true).ToList();
            foreach (MediaKitFile mediaFile in mediaFiles) {
                library.Add(new ImageLibraryItem() {
                    title = mediaFile.Description,
                    value = "/mediakitfiles/"+mediaFile.URL
                });
            }
            return Json(library);
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
        [HttpPost]
        public async Task<IActionResult> UploadFilesAjax()
        {
            long size = 0;
            var files = Request.Form.Files;
            var filename = "";
            var convertedFilename = "";
            foreach (var file in files)
            {
                filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                var fileExt = "";
                string[] fileParts = filename.ToString().Split('.');

                if(fileParts.Length>=2) {
                    fileExt = "."+fileParts[fileParts.Length-1];
                }

                filename = hostingEnv.WebRootPath + $@"\mediakitfiles\{Request.Form["url"] + fileExt}";
                size += file.Length;
                using (FileStream fs = System.IO.File.Create(filename))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
                convertedFilename = Request.Form["url"] + fileExt;
            }
            MediaKitFile newKitFile = new MediaKitFile();
            MediaKitFile tempKitFile = new MediaKitFile();
            tempKitFile = _context.MediaKitFile.SingleOrDefault(m => m.URL.ToLower() == convertedFilename);
            if (tempKitFile == null)
            {
                Int32 ownerid = 0;
                if (!string.IsNullOrEmpty(Request.Form["ownerId"]))
                {
                    bool result = Int32.TryParse(Request.Form["ownerId"].ToString().Trim(), out ownerid);
                }
                if (!string.IsNullOrEmpty(Request.Form["ownerName"])) {
                    Owner newOwner = new Owner();
                    newOwner.Name = Request.Form["ownerName"].ToString().Trim();
                    newOwner.Address = Request.Form["address"].ToString().Trim();
                    newOwner.Email = Request.Form["email"].ToString().Trim();
                    newOwner.Phone = Request.Form["phone"].ToString().Trim();
                    newOwner.SocialMedia = Request.Form["socialMedia"].ToString().Trim();
                    newOwner.Website = Request.Form["website"].ToString().Trim();
                    newOwner.DateCreated = DateTime.Now;
                    
                    _context.Add(newOwner);
                    await _context.SaveChangesAsync();
                    ownerid = newOwner.OwnerId;
                }

                newKitFile.DateCreated = DateTime.Now;
                newKitFile.DateModified = DateTime.Now;
                newKitFile.Description = Request.Form["description"];
                newKitFile.Enabled = true;
                newKitFile.MediaType = Request.Form["mediaType"];
                newKitFile.URL = convertedFilename;
                newKitFile.OwnerId = ownerid;
                newKitFile.CopyrightDate = Convert.ToDateTime(Request.Form["copyrightDate"]);
                _context.Add(newKitFile);
                await _context.SaveChangesAsync();


                List<MediaKitFileTag> fileTags = new List<MediaKitFileTag>();
                string newFileTags = Request.Form["FileTags"].ToString();

                if (!string.IsNullOrEmpty(newFileTags)) {
                    if (newFileTags.Substring(0, 1) == ",")
                    {
                        newFileTags = newFileTags.Substring(1, newFileTags.Length - 1);
                    }

                    if (newFileTags.Substring(newFileTags.Length - 1, 1) == ",")
                    {
                        newFileTags = newFileTags.Substring(0, newFileTags.Length - 1);
                    }

                    string[] tags = newFileTags.Split(',');

                    for (var x = 0; x < tags.Length; x++)
                    {
                        _context.MediaKitFileTag.Add(new MediaKitFileTag() { MediaKitFileId = newKitFile.MediaKitFileId, TagId = Convert.ToInt32(tags[x]) });
                    }
                    await _context.SaveChangesAsync();
                }

            }
            return Json(newKitFile);
        }
    }

    public class ImageLibraryItem
    {
        public string title { get; set; }
        public string value { get; set; }
    }
}
