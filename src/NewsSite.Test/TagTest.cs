using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsSite.Controllers;
using NewsSite.Data;
using NewsSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NewsSite.Test
{
    public class TagTest
    {

        private ApplicationDbContext _dbContext { get; set; }
        public TagsController ctrlr { get; set; }
        public TagTest()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase();
            this._dbContext = new ApplicationDbContext(optionsBuilder.Options);

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
                _dbContext.MediaKitFileTag.Add(new MediaKitFileTag()
                {
                    MediaKitFileId = 1,
                    TagId = 1
                });
                _dbContext.MediaKitFileTag.Add(new MediaKitFileTag()
                {
                    MediaKitFileId = 1,
                    TagId = 2
                });
                _dbContext.ArticleTag.Add(new ArticleTag()
                {
                    ArticleId = 1,
                    TagId = 1
                });
                _dbContext.ArticleTag.Add(new ArticleTag()
                {
                    ArticleId = 1,
                    TagId = 2
                });
            }
            _dbContext.SaveChangesAsync();
            ctrlr = new TagsController(this._dbContext);
        }



        [Fact]
        public async void TagsController_Index() {
            var result = await ctrlr.Index();
            var resultView = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<List<Tag>>(resultView.ViewData.Model).ToList();
            Assert.Equal(1, viewModel.Count(d => d.TagName == "stuff"));
            Assert.Equal(1, viewModel.Count(d => d.TagName == "category"));
            Assert.Equal(1, viewModel.Count(d => d.TagName == "interest"));
        }

        [Fact]
        public async void TagsController_Create()
        {
            Tag tag = new Tag()
            {
                TagName = "misc"
            };
            var result = await ctrlr.Create(tag);
            var resultView = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(1, _dbContext.Tag.Count(d => d.TagName == "misc"));

            Tag dupTag = new Tag()
            {
                TagName = "misc"
            };
            result = await ctrlr.Create(dupTag);
            Assert.Equal(1, _dbContext.Tag.Count(d => d.TagName == "misc"));

            //last comment has potential fix http://stackoverflow.com/questions/37558049/modelstate-isvalid-always-true-when-testing-controller-in-asp-net-mvc-web-api
            //Owner blankOwner = new Owner()
            //{
            //    Name = ""
            //};
            //result = await ctrlr.Create(blankOwner);
            //Assert.Equal(0, _dbContext.Owner.Count(d => d.Name == ""));

            Tag charTag = new Tag()
            {
                TagName = "`1234567890-=~!@#$%^&*()_+{}|:\"<>?[]\\;',./"
            };
            result = await ctrlr.Create(charTag);
            resultView = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(1, _dbContext.Tag.Count(d => d.TagName == "`1234567890-=~!@#$%^&*()_+{}|:\"<>?[]\\;',./"));
        }

        [Fact]
        public async void TagsController_Edit()
        {
            Tag tag = _dbContext.Tag.SingleOrDefault(d => d.TagName == "stuff");
            tag.TagName = "more stuff";
            var result = await ctrlr.Edit(tag.TagId,tag);
            var resultView = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(1, _dbContext.Tag.Count(d => d.TagName == "more stuff"));
        }

        public async void TagsController_Delete()
        {
            var result = await ctrlr.DeleteConfirmed(1);
            var resultView = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(0, _dbContext.Tag.Count(d => d.TagName == "stuff"));
            Assert.Equal(1, _dbContext.ArticleTag.Count());
            Assert.Equal(2, _dbContext.MediaKitFileTag.Count());
        }
    }
}
