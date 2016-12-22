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
            //Get all media Kit files associated with this Owner. Must be done this way to get the Tag Names instead of just the TagId
            var kitfiles = _context.MediaKitFile
                .Select(k => new MediaKitFileViewModel
                {
                    MediaKitFileId = k.MediaKitFileId,
                    URL = k.URL,
                    IconURL = k.URL,
                    DateModified = k.DateModified,
                    Description = k.Description,
                    CopyrightDate = k.CopyrightDate,
                    Owner = k.Owner,
                    TagNames = k.MediaKitFileTags
                    .Join(_context.Tag, mt => mt.TagId, t => t.TagId, (mt, t) => new { mt, t })
                    .Select(tt => new TagName
                    {
                        Name = tt.t.TagName
                    }).ToList()
                }).OrderByDescending(m=>m.DateModified);
            
            return View(kitfiles);
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
            //ViewData["OwnerId"] = new SelectList(_context.Owner, "OwnerId", "Name");
            List<Owner> owners = _context.Owner.Where(o => o.Enabled == true).OrderBy(n => n.Name).ToList<Owner>();
            List<Tag> tags = _context.Tag.Where(t => t.Enabled == true).OrderBy(g => g.TagName).ToList<Tag>();
            ViewBag.tags = tags;
            ViewBag.owners = owners;
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
            MediaKitFileViewModel mediaKitFilevm = new MediaKitFileViewModel();
            string selectedTagIds = ",";
            var mediaKitFile = await _context.MediaKitFile.SingleOrDefaultAsync(m => m.MediaKitFileId == id);
            if (mediaKitFile == null)
            {
                return NotFound();
            }
            else {
                mediaKitFilevm.MediaKitFileId = mediaKitFile.MediaKitFileId;
                mediaKitFilevm.Enabled = mediaKitFile.Enabled;
                mediaKitFilevm.IconURL = mediaKitFile.URL;
                mediaKitFilevm.URL = mediaKitFile.URL;
                mediaKitFilevm.AltText = mediaKitFile.AltText;
                mediaKitFilevm.Description = mediaKitFile.Description;
                mediaKitFilevm.MediaType = mediaKitFile.MediaType;
                mediaKitFilevm.Owner = _context.Owner.SingleOrDefault(o => o.OwnerId == mediaKitFile.OwnerId);
                mediaKitFilevm.CopyrightDate = mediaKitFile.CopyrightDate;
                
                mediaKitFilevm.DateCreated = mediaKitFile.DateCreated;
                mediaKitFilevm.DateModified = mediaKitFile.DateModified;

                List<TagName> tempTagNames = new List<TagName>();


                List<MediaKitFileTag> mediaTags = _context.MediaKitFileTag.Where(m => m.MediaKitFileId == mediaKitFile.MediaKitFileId).ToList<MediaKitFileTag>();
                foreach (var mediaTag in mediaTags) {
                    Tag temp = _context.Tag.SingleOrDefault(t => t.TagId == mediaTag.TagId);
                    tempTagNames.Add(new TagName() {
                        Id = temp.TagId,
                        Name = temp.TagName
                    });
                    selectedTagIds +=  temp.TagId + ",";
                }
                mediaKitFilevm.TagNames = tempTagNames;
            }
            ViewData["OwnerId"] = new SelectList(_context.Owner, "OwnerId", "Name", mediaKitFile.OwnerId);
            
            List<Owner> owners = _context.Owner.Where(o => o.Enabled == true).OrderBy(n => n.Name).ToList<Owner>();
            List<Tag> tags = _context.Tag.Where(t => t.Enabled == true).OrderBy(g => g.TagName).ToList<Tag>();

            
                foreach (TagName tagName in mediaKitFilevm.TagNames)
                {
                    Tag tempTag = _context.Tag.SingleOrDefault(t => t.TagId == tagName.Id);
                    tags.Remove(tempTag);
                }


            ViewBag.selectedTagIds = selectedTagIds;
            ViewBag.tags = tags;
            ViewBag.owners = owners;
            return View(mediaKitFilevm);
        }

        // POST: MediaKitFiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MediaKitFileId,Description,Enabled,MediaType,OwnerId,URL,AltText")] MediaKitFile mediaKitFile)
        {
            var files = Request.Form.Files;
            if (Request.Form.Files.Count()>=1) {
                MediaKitFile tempKitFile = _context.MediaKitFile.SingleOrDefault(m => m.MediaKitFileId == id);
                string filename = hostingEnv.WebRootPath + $@"\mediakitfiles\{tempKitFile.URL}";
                if (System.IO.File.Exists(filename))
                {
                    System.IO.File.Delete(filename);
                }

                foreach (var file in files)
                {
                    filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fileExt = "";
                    string[] fileParts = filename.ToString().Split('.');

                    if (fileParts.Length >= 2)
                    {
                        fileExt = "." + fileParts[fileParts.Length - 1];
                    }

                    filename = hostingEnv.WebRootPath + $@"\mediakitfiles\{Request.Form["url"] + fileExt}";
                    using (FileStream fs = System.IO.File.Create(filename))
                    {
                        file.CopyTo(fs);
                        fs.Flush();
                    }
                }
            }

                if (id != mediaKitFile.MediaKitFileId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {

                    Int32 ownerid = 0;
                    if (!string.IsNullOrEmpty(Request.Form["ownerId"]))
                    {
                        bool result = Int32.TryParse(Request.Form["ownerId"].ToString().Trim(), out ownerid);
                    }
                    if (!string.IsNullOrEmpty(Request.Form["ownerName"]))
                    {
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



                    List<MediaKitFileTag> selectedMediaKitFileTags = _context.MediaKitFileTag.Where(m => m.MediaKitFileId == id).ToList();

                    foreach (MediaKitFileTag mediaKitFileTag in selectedMediaKitFileTags)
                    {
                        _context.MediaKitFileTag.Remove(mediaKitFileTag);
                    }
                    _context.SaveChanges();
                    List<MediaKitFileTag> fileTags = new List<MediaKitFileTag>();
                    string newFileTags = Request.Form["FileTags"].ToString();

                    if (!string.IsNullOrEmpty(newFileTags))
                    {
                        if (newFileTags.Substring(0, 1) == ",")
                        {
                            newFileTags = newFileTags.Substring(1, newFileTags.Length - 1);
                        }

                        if (newFileTags.Substring(newFileTags.Length - 1, 1) == ",")
                        {
                            newFileTags = newFileTags.Substring(0, newFileTags.Length - 1);
                        }

                        List<String> tags = newFileTags.Split(',').ToList();

                        for (var x = 0; x < tags.Count; x++)
                        {
                            _context.MediaKitFileTag.Add(new MediaKitFileTag() { MediaKitFileId = id, TagId = Convert.ToInt32(tags[x]) });
                        }
                        await _context.SaveChangesAsync();
                    }

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

            List<MediaKitFileTag> mediaKitFileTags = _context.MediaKitFileTag.Where(m => m.MediaKitFileId == id).ToList();
            foreach (MediaKitFileTag mediaKitFileTag in mediaKitFileTags)
            {
                _context.MediaKitFileTag.Remove(mediaKitFileTag);
            }
            _context.SaveChanges();



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
            MediaKitFileViewModel mediaKitFilevm = new MediaKitFileViewModel();
            var mediaKitFile = await _context.MediaKitFile.SingleOrDefaultAsync(m => m.MediaKitFileId == id);
            if (mediaKitFile == null)
            {
                return NotFound();
            }
            else {
                mediaKitFilevm.MediaKitFileId = mediaKitFile.MediaKitFileId;
                mediaKitFilevm.Description = mediaKitFile.Description;
                mediaKitFilevm.IconURL = mediaKitFile.URL;
                mediaKitFilevm.CopyrightDate = mediaKitFile.CopyrightDate;
                mediaKitFilevm.DateCreated = mediaKitFile.DateCreated;
                mediaKitFilevm.DateModified = mediaKitFile.DateModified;
                mediaKitFilevm.Enabled = mediaKitFile.Enabled;
                mediaKitFilevm.Owner = _context.Owner.SingleOrDefault(o=>o.OwnerId == mediaKitFile.OwnerId);
            }

            return View(mediaKitFilevm);
        }

        // POST: MediaKitFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mediaKitFile = await _context.MediaKitFile.SingleOrDefaultAsync(m => m.MediaKitFileId == id);

            string filename = hostingEnv.WebRootPath + $@"\mediakitfiles\{mediaKitFile.URL}";
            if (System.IO.File.Exists(filename)) {
                System.IO.File.Delete(filename);
            }

            List<MediaKitFileTag> mediaKitFileTags = _context.MediaKitFileTag.Where(m => m.MediaKitFileId == id).ToList();
            List<ArticleMediaKitFile> articleKitFiles = _context.ArticleMediaKitFile.Where(m => m.MediaKitFileId == id).ToList();

            foreach (MediaKitFileTag mediaKitFileTag in mediaKitFileTags) {
                _context.MediaKitFileTag.Remove(mediaKitFileTag);
            }
            foreach (ArticleMediaKitFile articleKitFile in articleKitFiles)
            {
                _context.ArticleMediaKitFile.Remove(articleKitFile);
            }
            _context.MediaKitFile.Remove(mediaKitFile);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        private bool MediaKitFileExists(int id)
        {
            return _context.MediaKitFile.Any(e => e.MediaKitFileId == id);
        }

        [HttpGet]
        public bool MediaKitFileExists(string fileName)
        {
            string filename = hostingEnv.WebRootPath + $@"\mediakitfiles\{fileName}";
            if (System.IO.File.Exists(filename))
            {
                return true;
            }
            else {
                return false;
            }
        }


        //[HttpPost]
        //public async Task<IActionResult> UploadFileAjax()
        //{
        //    long size = 0;
        //    var files = Request.Form.Files;
        //    var filename = "";
        //    var convertedFilename = "";
        //    foreach (var file in files)
        //    {
        //        filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
        //        var fileExt = "";
        //        string[] fileParts = filename.ToString().Split('.');

        //        if(fileParts.Length>=2) {
        //            fileExt = "."+fileParts[fileParts.Length-1];
        //        }

        //        filename = hostingEnv.WebRootPath + $@"\mediakitfiles\{Request.Form["url"] + fileExt}";
        //        size += file.Length;
        //        using (FileStream fs = System.IO.File.Create(filename))
        //        {
        //            file.CopyTo(fs);
        //            fs.Flush();
        //        }
        //        convertedFilename = Request.Form["url"] + fileExt;
        //    }
        //    MediaKitFile newKitFile = new MediaKitFile();
        //    MediaKitFile tempKitFile = new MediaKitFile();
        //    MediaKitFileViewModel output = new MediaKitFileViewModel();
        //    tempKitFile = _context.MediaKitFile.SingleOrDefault(m => m.URL.ToLower() == convertedFilename);
        //    if (tempKitFile == null)
        //    {
        //        Int32 ownerid = 0;
        //        if (!string.IsNullOrEmpty(Request.Form["ownerId"]))
        //        {
        //            bool result = Int32.TryParse(Request.Form["ownerId"].ToString().Trim(), out ownerid);
        //        }
        //        if (!string.IsNullOrEmpty(Request.Form["ownerName"])) {
        //            Owner newOwner = new Owner();
        //            newOwner.Name = Request.Form["ownerName"].ToString().Trim();
        //            newOwner.Address = Request.Form["address"].ToString().Trim();
        //            newOwner.Email = Request.Form["email"].ToString().Trim();
        //            newOwner.Phone = Request.Form["phone"].ToString().Trim();
        //            newOwner.SocialMedia = Request.Form["socialMedia"].ToString().Trim();
        //            newOwner.Website = Request.Form["website"].ToString().Trim();
        //            newOwner.DateCreated = DateTime.Now;
                    
        //            _context.Add(newOwner);
        //            await _context.SaveChangesAsync();
        //            ownerid = newOwner.OwnerId;
        //        }

        //        newKitFile.DateCreated = DateTime.Now;
        //        newKitFile.DateModified = DateTime.Now;
        //        newKitFile.Description = Request.Form["description"];
        //        newKitFile.Enabled = true;
        //        newKitFile.MediaType = Request.Form["mediaType"];
        //        newKitFile.URL = convertedFilename;
        //        newKitFile.OwnerId = ownerid;
        //        newKitFile.CopyrightDate = Convert.ToDateTime(Request.Form["copyrightDate"]);
        //        newKitFile.AltText = Request.Form["altText"];
        //        _context.Add(newKitFile);
        //        await _context.SaveChangesAsync();


        //        List<MediaKitFileTag> fileTags = new List<MediaKitFileTag>();
        //        string newFileTags = Request.Form["FileTags"].ToString();
                
        //        if (!string.IsNullOrEmpty(newFileTags)) {
        //            if (newFileTags.Substring(0, 1) == ",")
        //            {
        //                newFileTags = newFileTags.Substring(1, newFileTags.Length - 1);
        //            }

        //            if (newFileTags.Substring(newFileTags.Length - 1, 1) == ",")
        //            {
        //                newFileTags = newFileTags.Substring(0, newFileTags.Length - 1);
        //            }

        //            List<String> tags = newFileTags.Split(',').ToList();

        //            for (var x = 0; x < tags.Count; x++)
        //            {
        //                _context.MediaKitFileTag.Add(new MediaKitFileTag() { MediaKitFileId = newKitFile.MediaKitFileId, TagId = Convert.ToInt32(tags[x]) });
        //            }
        //            await _context.SaveChangesAsync();

        //            output.MediaKitFileId = newKitFile.MediaKitFileId;
        //            output.URL = newKitFile.URL;
        //            output.CopyrightDate = newKitFile.CopyrightDate;
        //            output.Description = newKitFile.Description;
        //            output.IconURL = newKitFile.URL;

        //            output.TagNames = _context.Tag.Where(t => tags.Contains(t.TagId.ToString())).Select(tt=> new TagName{
        //                Name = tt.TagName
        //            }).ToList();
        //        }

        //    }
        //    //return Json(newKitFile);
        //    return Json(output);
        //}

        [HttpPost]
        public async Task<IActionResult> UploadFilesAjax()
        {
            Boolean blnError = false;
            String errorMessage = "";
            String errorElement = "";
            Int32 ownerid = 0;
            DateTime copyrightDate = DateTime.Now;
            string newFileTags = Request.Form["FileTags"].ToString();
            if (string.IsNullOrEmpty(newFileTags))
            {
                blnError = true;
                errorMessage = "Media kit file cannot be updated because there is a problem with the tags.";
                errorElement = "FileTags";
            }
            else
            {
                if (newFileTags.Length < 2)
                {
                    blnError = true;
                    errorMessage = "Media kit file cannot be updated because there is a problem with the tags.";
                    errorElement = "FileTags";
                }
            }
            string rawFilename = Request.Form["url"].ToString().Trim().ToLower();
            if (!string.IsNullOrEmpty(Request.Form["copyrightDate"]))
            {
                try
                {
                    copyrightDate = Convert.ToDateTime(Request.Form["copyrightDate"].ToString().Trim());
                }
                catch (Exception ex)
                {
                    blnError = true;
                    errorMessage = "Media kit file cannot be updated because there is a problem with the copyright date.";
                    errorElement = "copyrightDate";
                }
            }
            else
            {
                blnError = true;
                errorMessage = "Media kit file cannot be updated because there is a problem with the copyright date.";
                errorElement = "copyrightDate";
            }
            if (!string.IsNullOrEmpty(Request.Form["ownerId"]))
            {
                bool result = Int32.TryParse(Request.Form["ownerId"].ToString().Trim(), out ownerid);
                if (ownerid == 0)
                {
                    if (!string.IsNullOrEmpty(Request.Form["ownerName"]))
                    {
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
                }
            }
            if (ownerid == 0)
            {
                blnError = true;
                errorMessage = "Media kit file cannot be updated because there is a problem with the owner.";
                errorElement = "ownerId";
            }
            string description = Request.Form["description"].ToString().Trim();
            if (string.IsNullOrEmpty(description))
            {
                blnError = true;
                errorMessage = "Media kit file cannot be updated because there is a problem with the caption.";
                errorElement = "description";
            }
            string altText = Request.Form["altText"].ToString().Trim();
            if (string.IsNullOrEmpty(altText))
            {
                blnError = true;
                errorMessage = "Media kit file cannot be updated because there is a problem with the alt text.";
                errorElement = "altText";
            }
            var files = Request.Form.Files;
            if (files.Count==0)
            {
                blnError = true;
                errorMessage = "Media kit file cannot be updated because there is a problem with the file.";
                errorElement = "files";
            }

            if (blnError)
            {
                return Json(new { status = "error", message = errorMessage, element = errorElement });
            }
            else
            {
                
                var filename = "";
                var convertedFilename = "";
                var fileExt = "";
                foreach (var file in files)
                {
                    filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    
                    string[] fileParts = filename.ToString().Split('.');

                    if (fileParts.Length >= 2)
                    {
                        fileExt = "." + fileParts[fileParts.Length - 1];
                    }

                    filename = hostingEnv.WebRootPath + $@"\mediakitfiles\{Request.Form["url"] + fileExt}";
                    using (FileStream fs = System.IO.File.Create(filename))
                    {
                        file.CopyTo(fs);
                        fs.Flush();
                    }
                    convertedFilename = Request.Form["url"] + fileExt;
                }
                MediaKitFile newKitFile = new MediaKitFile();
                MediaKitFile tempKitFile = new MediaKitFile();
                MediaKitFileViewModel output = new MediaKitFileViewModel();
                tempKitFile = _context.MediaKitFile.SingleOrDefault(m => m.URL.ToLower() == convertedFilename);
                if (tempKitFile == null)
                {
                    newKitFile.DateCreated = DateTime.Now;
                    newKitFile.DateModified = DateTime.Now;
                    newKitFile.Description = description;
                    newKitFile.Enabled = true;
                    newKitFile.MediaType = getFileType(fileExt);
                    newKitFile.URL = convertedFilename;
                    newKitFile.OwnerId = ownerid;
                    newKitFile.CopyrightDate = copyrightDate;
                    newKitFile.AltText = altText;
                    _context.Add(newKitFile);
                    await _context.SaveChangesAsync();


                    List<MediaKitFileTag> fileTags = new List<MediaKitFileTag>();
                    

                    if (!string.IsNullOrEmpty(newFileTags))
                    {
                        if (newFileTags.Substring(0, 1) == ",")
                        {
                            newFileTags = newFileTags.Substring(1, newFileTags.Length - 1);
                        }

                        if (newFileTags.Substring(newFileTags.Length - 1, 1) == ",")
                        {
                            newFileTags = newFileTags.Substring(0, newFileTags.Length - 1);
                        }

                        List<String> tags = newFileTags.Split(',').ToList();

                        for (var x = 0; x < tags.Count; x++)
                        {
                            _context.MediaKitFileTag.Add(new MediaKitFileTag() { MediaKitFileId = newKitFile.MediaKitFileId, TagId = Convert.ToInt32(tags[x]) });
                        }
                        await _context.SaveChangesAsync();

                        output.MediaKitFileId = newKitFile.MediaKitFileId;
                        output.URL = newKitFile.URL;
                        output.CopyrightDate = newKitFile.CopyrightDate;
                        output.Description = newKitFile.Description;
                        output.IconURL = newKitFile.URL;

                        output.TagNames = _context.Tag.Where(t => tags.Contains(t.TagId.ToString())).Select(tt => new TagName
                        {
                            Name = tt.TagName
                        }).ToList();
                    }

                }
                //return Json(newKitFile);
                return Json(output);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditMediaKitFile()
        {
            Boolean blnError = false;
            String errorMessage = "";
            String errorElement = "";
            Int32 id = 0;
            if (!string.IsNullOrEmpty(Request.Form["id"]))
            {
                bool result = Int32.TryParse(Request.Form["id"].ToString().Trim(), out id);
                if (id == 0)
                {
                    blnError = true;
                    errorMessage = "Media kit file cannot be updated.";
                    errorElement = "id";
                }
            }
            else
            {
                blnError = true;
                errorMessage = "Media kit file cannot be updated.";
                errorElement = "id";
            }
            string newFileTags = Request.Form["FileTags"].ToString();
            if (string.IsNullOrEmpty(newFileTags))
            {
                blnError = true;
                errorMessage = "Media kit file cannot be updated because there is a problem with the tags.";
                errorElement = "FileTags";
            }
            else
            {
                if (newFileTags.Length < 2)
                {
                    blnError = true;
                    errorMessage = "Media kit file cannot be updated because there is a problem with the tags.";
                    errorElement = "FileTags";
                }
            }
            DateTime copyrightDate = DateTime.Now;
            string rawFilename = Request.Form["url"].ToString().Trim().ToLower();
            if (!string.IsNullOrEmpty(Request.Form["copyrightDate"]))
            {
                try
                {
                    copyrightDate = Convert.ToDateTime(Request.Form["copyrightDate"].ToString().Trim());
                }
                catch (Exception ex)
                {
                    blnError = true;
                    errorMessage = "Media kit file cannot be updated because there is a problem with the copyright date.";
                    errorElement = "copyrightDate";
                }
            }
            else
            {
                blnError = true;
                errorMessage = "Media kit file cannot be updated because there is a problem with the copyright date.";
                errorElement = "copyrightDate";
            }
            Int32 ownerid = 0;
            if (!string.IsNullOrEmpty(Request.Form["ownerId"]))
            {
                bool result = Int32.TryParse(Request.Form["ownerId"].ToString().Trim(), out ownerid);
                if (ownerid == 0)
                {
                    if (!string.IsNullOrEmpty(Request.Form["ownerName"]))
                    {
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
                }
            }

            if (ownerid == 0)
            {
                blnError = true;
                errorMessage = "Media kit file cannot be updated because there is a problem with the owner.";
                errorElement = "ownerId";
            }
            string description = Request.Form["description"].ToString().Trim();
            if (string.IsNullOrEmpty(description))
            {
                blnError = true;
                errorMessage = "Media kit file cannot be updated because there is a problem with the caption.";
                errorElement = "description";
            }
            string altText = Request.Form["altText"].ToString().Trim();
            if (string.IsNullOrEmpty(altText))
            {
                blnError = true;
                errorMessage = "Media kit file cannot be updated because there is a problem with the alt text.";
                errorElement = "altText";
            }
            if (blnError) {
                return Json(new { status = "error", message = errorMessage, element = errorElement });
            }
            else { 
                MediaKitFile mediaKitFile = _context.MediaKitFile.SingleOrDefault(m => m.MediaKitFileId == id);
                if (mediaKitFile != null)
                {
                    string filename = "";

                    var files = Request.Form.Files;
                    if (Request.Form.Files.Count() >= 1)
                    {
                        filename = hostingEnv.WebRootPath + $@"\mediakitfiles\{mediaKitFile.URL}";


                        if (System.IO.File.Exists(filename))
                        {
                            System.IO.File.Delete(filename);
                        }

                        foreach (var file in files)
                        {
                            filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                            string tempNewFileName = Request.Form["url"].ToString().Trim();
                            string uploadFileExt = getFileExt(filename);
                            mediaKitFile.MediaType = getFileType(uploadFileExt);
                            string cleanFileName = "";
                            if (!string.IsNullOrEmpty(tempNewFileName))
                            {
                                string customFileExt = getFileExt(tempNewFileName);
                                int place = tempNewFileName.LastIndexOf(customFileExt);

                                if (place == -1)
                                {
                                    cleanFileName = tempNewFileName;
                                }

                                cleanFileName = tempNewFileName.Remove(place, customFileExt.Length).Insert(place, "");

                                filename = hostingEnv.WebRootPath + $@"\mediakitfiles\{cleanFileName + uploadFileExt}";

                                using (FileStream fs = System.IO.File.Create(filename))
                                {
                                    file.CopyTo(fs);
                                    fs.Flush();
                                }
                                mediaKitFile.URL = cleanFileName + uploadFileExt;
                            }
                            else
                            {
                                //no custom url
                            }
                        }
                    }
                    else
                    {
                        if (mediaKitFile.URL.ToLower().Trim() != rawFilename)
                        {
                            string fileExt = getFileExt(mediaKitFile.URL);

                            string cleanFileName = "";
                            string customFileExt = getFileExt(rawFilename);
                            int place = rawFilename.LastIndexOf(customFileExt);

                            if (place == -1)
                            {
                                cleanFileName = rawFilename;
                            }

                            cleanFileName = rawFilename.Remove(place, customFileExt.Length).Insert(place, "");

                            System.IO.File.Move(hostingEnv.WebRootPath + $@"\mediakitfiles\{mediaKitFile.URL}", hostingEnv.WebRootPath + $@"\mediakitfiles\{cleanFileName + fileExt}");
                            mediaKitFile.URL = cleanFileName + fileExt;
                        }
                    }

                    mediaKitFile.Description = description;
                    mediaKitFile.OwnerId = ownerid;
                    mediaKitFile.DateModified = DateTime.Now;
                    mediaKitFile.CopyrightDate = copyrightDate;
                    mediaKitFile.AltText = altText;
                    String tempEnabled = Request.Form["enabled"].ToString().Trim();
                    if (!string.IsNullOrEmpty(tempEnabled))
                    {
                        if (tempEnabled.ToLower() == "true")
                        {
                            mediaKitFile.Enabled = true;
                        }
                        else
                        {
                            mediaKitFile.Enabled = false;
                        }
                    }



                    _context.Update(mediaKitFile);
                    await _context.SaveChangesAsync();


                    List<MediaKitFileTag> selectedMediaKitFileTags = _context.MediaKitFileTag.Where(m => m.MediaKitFileId == id).ToList();

                    foreach (MediaKitFileTag mediaKitFileTag in selectedMediaKitFileTags)
                    {
                        _context.MediaKitFileTag.Remove(mediaKitFileTag);
                    }
                    _context.SaveChanges();


                    List<MediaKitFileTag> mediaKitFileTags = _context.MediaKitFileTag.Where(m => m.MediaKitFileId == id).ToList();
                    foreach (MediaKitFileTag mediaKitFileTag in mediaKitFileTags)
                    {
                        _context.MediaKitFileTag.Remove(mediaKitFileTag);
                    }
                    _context.SaveChanges();

                    List<MediaKitFileTag> fileTags = new List<MediaKitFileTag>();
                    if (!string.IsNullOrEmpty(newFileTags))
                    {
                        if (newFileTags.Substring(0, 1) == ",")
                        {
                            newFileTags = newFileTags.Substring(1, newFileTags.Length - 1);
                        }

                        if (newFileTags.Substring(newFileTags.Length - 1, 1) == ",")
                        {
                            newFileTags = newFileTags.Substring(0, newFileTags.Length - 1);
                        }

                        List<String> tags = newFileTags.Split(',').ToList();

                        for (var x = 0; x < tags.Count; x++)
                        {
                            _context.MediaKitFileTag.Add(new MediaKitFileTag() { MediaKitFileId = id, TagId = Convert.ToInt32(tags[x]) });
                        }
                        await _context.SaveChangesAsync();
                    }
                }
            }
            return Json("st");
        }

        private string getFileExt(String filename) {
            var fileExt = "";
            string[] fileParts = filename.ToString().Split('.');
            if (fileParts.Length >= 2)
            {
                fileExt = "." + fileParts[fileParts.Length - 1];
            }
            return fileExt;
        }

        private string getFileType(String fileExt)
        {
            string output = "";
            switch (fileExt.ToLower().Trim()) {
                case ".jpg":
                    output = "image";
                    break;
                case ".gif":
                    output = "image";
                    break;
                case ".png":
                    output = "image";
                    break;
                case ".pdf":
                    output = "pdf";
                    break;
                case ".doc":
                    output = "word";
                    break;
                case ".docx":
                    output = "word";
                    break;
                case ".xls":
                    output = "spreadsheet";
                    break;
                case ".xlsx":
                    output = "spreadsheet";
                    break;
                case ".ppt":
                    output = "ppt";
                    break;
                case ".pptx":
                    output = "ppt";
                    break;
                default:
                    output = "text";
                    break;
            }
            return output;
        }

    }

    public class ImageLibraryItem
    {
        public string title { get; set; }
        public string value { get; set; }
    }
}