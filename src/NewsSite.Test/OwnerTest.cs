using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewsSite.Controllers;
using NewsSite.Data;
using NewsSite.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Microsoft.AspNetCore.Mvc;

namespace NewsSite.Tests
{
    public class OwnerTest
    {
        public ApplicationDbContext _dbContext { get; set; }
        public OwnersController ctrlr { get; set; }
        public OwnerTest()
        {
            
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
                    Phone = "111-111-1111",
                    Email = "test@savannahstate.edu",
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
                    Website = "www.viliphotos.com"
                });

                //add media file associated with owner and tags associated with media file. On delete of owner, files and filetag looks ups should be deleted.
                _dbContext.MediaKitFile.Add(new MediaKitFile()
                {
                    URL = "test.jpg",
                    AltText = "test",
                    Description = "test",
                    OwnerId = 1,
                    CopyrightDate = DateTime.Now,
                    MediaType = "image",
                    Enabled = true
                });

                _dbContext.Tag.Add(new Tag(){
                    TagName = "stuff",
                    Enabled =true
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
                _dbContext.MediaKitFileTag.Add(new MediaKitFileTag()
                {
                    MediaKitFileId=1,
                    TagId =1
                });
                _dbContext.MediaKitFileTag.Add(new MediaKitFileTag()
                {
                    MediaKitFileId = 1,
                    TagId = 2
                });
                _dbContext.SaveChanges();
            }
                ctrlr = new OwnersController(this._dbContext);
            
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
            var result = await ctrlr.Details(1);
            var resultView = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<Owner>(resultView.ViewData.Model);
            Assert.Equal("Savannah State University", viewModel.Name);
        }
        [Fact]
        public async void OwnersController_Create() {
            Owner owner = new Owner()
            {
                Name = "Jason Saunders"
            };
            var result = await ctrlr.Create(owner);
            var resultView = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(1, _dbContext.Owner.Count(d => d.Name == "Jason Saunders"));

            Owner dupOwner = new Owner()
            {
                Name = "Jason Saunders"
            };
            result = await ctrlr.Create(dupOwner);
            Assert.Equal(1, _dbContext.Owner.Count(d => d.Name == "Jason Saunders"));

            //last comment has potential fix http://stackoverflow.com/questions/37558049/modelstate-isvalid-always-true-when-testing-controller-in-asp-net-mvc-web-api
            //Owner blankOwner = new Owner()
            //{
            //    Name = ""
            //};
            //result = await ctrlr.Create(blankOwner);
            //Assert.Equal(0, _dbContext.Owner.Count(d => d.Name == ""));

            Owner charOwner = new Owner()
            {
                Name = "`1234567890-=~!@#$%^&*()_+{}|:\"<>?[]\\;',./"
            };
            result = await ctrlr.Create(charOwner);
            resultView = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(1, _dbContext.Owner.Count(d => d.Name == "`1234567890-=~!@#$%^&*()_+{}|:\"<>?[]\\;',./"));
        }
        //[Fact]
        //public async void OwnersController_CreateAjax() { }
        [Fact]
        public async void OwnersController_Delete() {
            var result = await ctrlr.DeleteConfirmed(1);
            var resultView = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(0, _dbContext.Owner.Count(d => d.Name == "Savannah State University"));
            Assert.Equal(0, _dbContext.MediaKitFile.Count());
        }
        //[Fact]
        //public async void OwnersController_Edit() { }
        [Fact]
        public void OwnersController_OwnerExistsByName() {
            var result = ctrlr.OwnerExistsByName("Savannah State University");
            Assert.Equal(true, result);
            result = ctrlr.OwnerExistsByName("Jimmy JoJo");
            Assert.Equal(false, result);
        }
    }
}
