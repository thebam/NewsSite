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

        private ApplicationDbContext _dbContext;
        public TagTest()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase();
            var context = new ApplicationDbContext(optionsBuilder.Options);
            
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
                context.ArticleTag.Add(new ArticleTag()
                {
                    ArticleId = 1,
                    TagId = 1
                });
                context.ArticleTag.Add(new ArticleTag()
                {
                    ArticleId = 1,
                    TagId = 2
                });
            context.SaveChanges();
            _dbContext = context;
        }



        [Fact]
        public async void TagsController_Index() {
            //TO DO write test to check usagecnt
            var ctrlr = new TagsController(_dbContext);
            var result = await ctrlr.Index();
            var resultView = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<List<TagIndexViewModel>>(resultView.ViewData.Model).ToList();
            Assert.True(1 <= viewModel.Count(d => d.TagName == "stuff"));
            Assert.True(1 <= viewModel.Count(d => d.TagName == "category"));
            Assert.True(1 <= viewModel.Count(d => d.TagName == "interest"));
        }

        [Fact]
        public async void TagsController_Create()
        {
            var ctrlr = new TagsController(_dbContext);
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
            var ctrlr = new TagsController(_dbContext);
            Tag tag = _dbContext.Tag.SingleOrDefault(d => d.TagId == 1);
            tag.TagName = "more stuff";
            var result = await ctrlr.Edit(tag.TagId,tag);
            var resultView = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(1, _dbContext.Tag.Count(d => d.TagName == "more stuff"));
        }

        public async void TagsController_Delete()
        {
            var ctrlr = new TagsController(_dbContext);
            var result = await ctrlr.DeleteConfirmed(2);
            var resultView = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(0, _dbContext.Tag.Count(d => d.TagId == 2));
            Assert.Equal(1, _dbContext.ArticleTag.Count());
            Assert.Equal(2, _dbContext.MediaKitFileTag.Count());
        }
    }
}
