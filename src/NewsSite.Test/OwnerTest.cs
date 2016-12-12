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
                get; set;
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
                get; set;
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
            var context = new ApplicationDbContext(optionsBuilder.Options);

            context.Owner.Add(new Owner()
            {
                Name = "Savannah State University",
                Address = "3219 College St., Savannah, GA 31404",
                Phone = "",
                Email = "marketingandcommunications@savannahstate.edu",
                SocialMedia = "https://www.facebook.com/savannahstate",
                Website = "http://www.savannahstate.edu/"
            });
            context.Owner.Add(new Owner()
            {
                Name = "Meaghan Gerard",
                Address = "",
                Phone = "",
                Email = "gerardm@savannahstate.edu",
                SocialMedia = "",
                Website = ""
            });
            context.Owner.Add(new Owner()
            {
                Name = "Litus Marshall",
                Address = "",
                Phone = "",
                Email = "",
                SocialMedia = "https://www.facebook.com/litus.marshall",
                Website = "http://www.viliphotos.com"
            });


            context.MediaKitFile.Add(new MediaKitFile()
            {
                URL = "test.jpg",
                AltText = "test",
                Description = "test",
                OwnerId = 1,
                CopyrightDate = DateTime.Now,
                MediaType = "image",
                Enabled = true
            });

            context.Tag.Add(new Tag()
            {
                TagName = "stuff",
                Enabled = true
            });
            context.Tag.Add(new Tag()
            {
                TagName = "category",
                Enabled = true
            });
            context.Tag.Add(new Tag()
            {
                TagName = "interest",
                Enabled = true
            });


            context.MediaKitFileTag.Add(new MediaKitFileTag()
            {
                MediaKitFileId = 1,
                TagId = 1
            });
            context.MediaKitFileTag.Add(new MediaKitFileTag()
            {
                MediaKitFileId = 1,
                TagId = 2
            });
            context.SaveChanges();
            _dbContext = context;



        }


        [Fact]
        public async void OwnersController_Index()
        {
            OwnersController ctrlr = new OwnersController(_dbContext, hostingEnv);
            var result = await ctrlr.Index();
            var resultView = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<List<Owner>>(resultView.ViewData.Model).ToList();
            Assert.Equal(1, viewModel.Count(d => d.OwnerId == 1));
            Assert.Equal(1, viewModel.Count(d => d.OwnerId==2));
            Assert.Equal(1, viewModel.Count(d => d.OwnerId==3));
        }
        [Fact]
        public async void OwnersController_Details()
        {
            OwnersController ctrlr = new OwnersController(_dbContext, hostingEnv);
            Owner detailsOwner = _dbContext.Owner.SingleOrDefault(d => d.Name == "Savannah State University");
            var result = await ctrlr.Details(detailsOwner.OwnerId);
            var resultView = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<Owner>(resultView.ViewData.Model);
            Assert.Equal("Savannah State University", viewModel.Name);
        }

        //[Fact]
        //public async void OwnersController_CreateAjax() { }
        [Fact]
        public async void OwnersController_Delete()
        {
            OwnersController ctrlr = new OwnersController(_dbContext, hostingEnv);
            var result = await ctrlr.DeleteConfirmed(1);
            var resultView = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(0, _dbContext.Owner.Count(d => d.OwnerId == 1));
            Assert.Equal(0, _dbContext.MediaKitFile.Count(f => f.URL == "test.jpg"));
        }
        //[Fact]
        //public async void OwnersController_Edit() { }
        [Fact]
        public void OwnersController_OwnerExistsByName()
        {
            OwnersController ctrlr = new OwnersController(_dbContext, hostingEnv);
            var result = ctrlr.OwnerExistsByName("Litus Marshall");
            Assert.Equal(true, result);
            result = ctrlr.OwnerExistsByName("Jimmy JoJo");
            Assert.Equal(false, result);
        }
    }
}
