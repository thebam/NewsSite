using System;
using System.Collections.Generic;
using System.Linq;
using NewsSite.Controllers;
using NewsSite.Data;
using NewsSite.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace NewsSite.Tests
{
    public class OwnerTest
    {
        protected IHostingEnvironment hostingEnv { get; set; }

        public class Hosting : IHostingEnvironment
        {
            public string ApplicationName
            {
                get;set;
            }

            public IFileProvider ContentRootFileProvider
            {
                get
                {
                    throw new NotImplementedException();
                }

                set
                {
                    throw new NotImplementedException();
                }
            }

            public string ContentRootPath
            {
                get;set;
            }

            public string EnvironmentName
            {
                get; set;
            }

            public IFileProvider WebRootFileProvider
            {
                get
                {
                    throw new NotImplementedException();
                }

                set
                {
                    throw new NotImplementedException();
                }
            }

            public string WebRootPath
            {
                get; set;
            }
        }

        private ApplicationDbContext _dbContext { get; set; }
        public OwnersController ctrlr { get; set; }
        public OwnerTest()
        {
            hostingEnv = new Hosting()
            {
                ApplicationName = "NewsSite",
                ContentRootPath = "c:\\users\\saundersj\\documents\\visual studio 2015\\projects\\newssite\\src\\newssite",
                EnvironmentName = "Development",
                WebRootPath = "c:\\users\\saundersj\\documents\\visual studio 2015\\projects\\newssite\\src\\newssite\\wwwroot"
            };
            
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase();
            this._dbContext = new ApplicationDbContext(optionsBuilder.Options);

            

            // Add sample data
            if (_dbContext.Owner.Count()==0)
            {
                _dbContext.Owner.Add(new Owner()
                {
                    Name = "Savannah State University",
                    Address = "3219 College St., Savannah, GA 31404",
                    Phone = "",
                    Email = "marketingandcommunications@savannahstate.edu",
                    SocialMedia = "https://www.facebook.com/savannahstate",
                    Website = "http://www.savannahstate.edu/"
                });
                _dbContext.Owner.Add(new Owner()
                {
                    Name = "Meaghan Gerard",
                    Address = "",
                    Phone = "",
                    Email = "gerardm@savannahstate.edu",
                    SocialMedia = "",
                    Website = ""
                });
                _dbContext.Owner.Add(new Owner()
                {
                    Name = "Litus Marshall",
                    Address = "",
                    Phone = "",
                    Email = "",
                    SocialMedia = "https://www.facebook.com/litus.marshall",
                    Website = "http://www.viliphotos.com"
                });
                _dbContext.SaveChanges();

                Owner tempOwner = _dbContext.Owner.SingleOrDefault(d => d.Name == "Meaghan Gerard");
                if (_dbContext.MediaKitFile.Count() == 0)
                {
                    _dbContext.MediaKitFile.Add(new MediaKitFile()
                    {
                        URL = "test.jpg",
                        AltText = "test",
                        Description = "test",
                        OwnerId = tempOwner.OwnerId,
                        CopyrightDate = DateTime.Now,
                        MediaType = "image",
                        Enabled = true
                    });
                    _dbContext.SaveChanges();
                }
                if (_dbContext.Tag.Count() == 0)
                {
                    _dbContext.Tag.Add(new Tag()
                    {
                        TagName = "stuff",
                        Enabled = true
                    });
                    _dbContext.Tag.Add(new Tag()
                    {
                        TagName = "category",
                        Enabled = true
                    });
                    _dbContext.Tag.Add(new Tag()
                    {
                        TagName = "interest",
                        Enabled = true
                    });

                    _dbContext.SaveChanges();

                    Tag stuffTag = _dbContext.Tag.SingleOrDefault(d => d.TagName == "stuff");
                    Tag catTag = _dbContext.Tag.SingleOrDefault(d => d.TagName == "category");
                    Tag intTag = _dbContext.Tag.SingleOrDefault(d => d.TagName == "interest");

                    MediaKitFile media = _dbContext.MediaKitFile.SingleOrDefault(d => d.URL == "test.jpg");
                    _dbContext.MediaKitFileTag.Add(new MediaKitFileTag()
                    {
                        MediaKitFileId = media.MediaKitFileId,
                        TagId = stuffTag.TagId
                    });
                    _dbContext.MediaKitFileTag.Add(new MediaKitFileTag()
                    {
                        MediaKitFileId = media.MediaKitFileId,
                        TagId = catTag.TagId
                    });
                    _dbContext.SaveChanges();
                }
                
            }
            ctrlr = new OwnersController(this._dbContext, this.hostingEnv);
        }


        [Fact]
        public async void OwnersController_Index()
        {
            var result = await ctrlr.Index();
            var resultView = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<List<Owner>>(resultView.ViewData.Model).ToList();
            Assert.Equal(1, viewModel.Count(d => d.Name == "Savannah State University"));
            Assert.Equal(1, viewModel.Count(d => d.Name == "Meaghan Gerard"));
            Assert.Equal(1, viewModel.Count(d => d.Name == "Litus Marshall"));
        }
        [Fact]
        public async void OwnersController_Details()
        {
            Owner detailsOwner = _dbContext.Owner.SingleOrDefault(d => d.Name == "Savannah State University");
            var result = await ctrlr.Details(detailsOwner.OwnerId);
            var resultView = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<Owner>(resultView.ViewData.Model);
            Assert.Equal("Savannah State University", viewModel.Name);
        }
        
        //[Fact]
        //public async void OwnersController_CreateAjax() { }
        [Fact]
        public async void OwnersController_Delete() {
            Owner owner = _dbContext.Owner.SingleOrDefault(d => d.Name == "Meaghan Gerard");
            var result = await ctrlr.DeleteConfirmed(owner.OwnerId);
            var resultView = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(0, _dbContext.Owner.Count(d => d.Name == "Meaghan Gerard"));
            Assert.Equal(0, _dbContext.MediaKitFile.Count(f => f.URL == "test.jpg"));
        }
        //[Fact]
        //public async void OwnersController_Edit() { }
        [Fact]
        public void OwnersController_OwnerExistsByName() {
            var result = ctrlr.OwnerExistsByName("Litus Marshall");
            Assert.Equal(true, result);
            result = ctrlr.OwnerExistsByName("Jimmy JoJo");
            Assert.Equal(false, result);
        }
    }
}
