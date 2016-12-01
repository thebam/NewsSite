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
            if (_dbContext.Owner.Count()!=3)
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


    }
}
