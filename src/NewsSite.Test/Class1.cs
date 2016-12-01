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

        public OwnerTest()
        {

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase();
            this._dbContext = new ApplicationDbContext(optionsBuilder.Options);
            // Add sample data
            _dbContext.Owner.Add(new Owner()
            {
                OwnerId = 1,
                Name = "Savannah State University",
                Address = "3219 College St., Savannah, GA 31404",
                Phone = "111-111-1111",
                Email = "test@savannahstate.edu",
                SocialMedia = "https://www.facebook.com/savannahstate",
                Website = "http://www.savannahstate.edu/"
            });
            //_dbContext.Article.Add(new Article() { Title = "Title 2" });
            //_dbContext.Tag.Add(new Tag() { Title = "Title 3" });
            //_dbContext.MediaKitFile.Add(new MediaKitFile() { Title = "Title 3" });

            //_dbContext.ArticleMediaKitFile.Add(new Dinner() { Title = "Title 3" });
            //_dbContext.ArticleTag.Add(new Dinner() { Title = "Title 3" });
            //_dbContext.MediaKitFileTag.Add(new Dinner() { Title = "Title 3" });





            _dbContext.SaveChanges();
        }


        //https://xunit.github.io/docs/getting-started-dotnet-core.html
        //https://msdn.microsoft.com/en-us/magazine/mt703433.aspx
        [Fact]
        public async void PassingTest()
        {
            OwnersController ctrlr = new OwnersController(this._dbContext);
            var result = await ctrlr.Index();
            var resultView = Assert.IsType<ViewResult>(result);

            var viewModel = Assert.IsType<IEnumerable<Owner>>(resultView.ViewData.Model).ToList();

            //Assert.IsInstanceOfType(result.ViewData.Model, typeof(List<ArtistIndexViewModel>));
            //var artists = (List<ArtistIndexViewModel>)result.ViewData.Model;


            //Assert.Equal(1, viewModel.Count(d => d.Title == "Title 1"));
        }
    }
}
