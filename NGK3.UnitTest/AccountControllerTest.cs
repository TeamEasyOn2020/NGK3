using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NGK3.Controllers;
using NGK3.Models;
using NSubstitute;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using WebApi;
using WebApi.Context;
using static System.Net.WebRequestMethods;

namespace NGK3.UnitTest
{
    public class AccountControllerTest
    {
        WeatherContext DbContext;
        IOptions<AppSettings> OptionsSW;
        AccountController uut;

        [SetUp]
        public void Setup()
        {
            DbContext = new WeatherContext();
            OptionsSW = Options.Create<AppSettings>(new AppSettings() { SecretKey = "ncSK45=)7@#qwKDSopevvkj3274687236" });
            uut = new AccountController(DbContext, OptionsSW);
        }

        [TearDown]
        public void TearDown()
        {
            DbContext.WeatherReading.RemoveRange(DbContext.WeatherReading);
            DbContext.Place.RemoveRange(DbContext.Place);
            DbContext.User.RemoveRange(DbContext.User);
            DbContext.SaveChanges();
        }
  

        [Test]
        public async Task RegisterReturnsCreatedAtActionResult()
        {
            // Arrange
            UserDto user = new UserDto
            {
                FirstName = "Victor",
                LastName = "Steiner",
                Email = "vsk@mail.dk",
                Password = "Kode123"
            };

            // Action
            var result = await uut.Register(user);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(result.Result);

        }

        [Test]
        public async Task RegisterReturnsBadRequest()
        {
            // Arrange
            UserDto user = new UserDto
            {
                FirstName = "Victor",
                LastName = "Steiner",
                Email = "vsk@mail.dk",
                Password = "Kode123"
            };

            // Action
            await uut.Register(user);
            var result = await uut.Register(user);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);

        }

        [Test]
        public async Task RegisterUserReturned()
        {
            // Arrange
            UserDto user = new UserDto
            {
                FirstName = "Victor",
                LastName = "Steiner",
                Email = "vsk@mail.dk",
                Password = "Kode123"
            };

            // Action
            var result = await uut.Register(user);

            // Assert

            var result1 = (ObjectResult)result.Result;
            Assert.AreEqual(user, result1.Value);
        }

        [Test]
        public async Task LoginTokenTypeReturned()
        {
            // Arrange
            UserDto user = new UserDto
            {
                FirstName = "Victor",
                LastName = "Steiner",
                Email = "vsk@mail.dk",
                Password = "Kode123"
            };


            // Action
            await uut.Register(user);
            var result = await uut.Login(user);

            // Assert

            Assert.IsInstanceOf<TokenDto>(result.Value);
        }

        [Test]
        public async Task LoginTokenContentNotNull()
        {
            // Arrange
            UserDto user = new UserDto
            {
                FirstName = "Victor",
                LastName = "Steiner",
                Email = "vsk@mail.dk",
                Password = "Kode123"
            };


            // Action
            await uut.Register(user);
            var result = await uut.Login(user);

            // Assert

            Assert.That(result.Value.JWT != null);
        }
        [Test]
        public async Task LoginBadRequestReturned()
        {
            // Arrange
            UserDto user = new UserDto
            {
                FirstName = "Victor",
                LastName = "Steiner",
                Email = "vsk@mail.dk",
                Password = "Kode123"
            };

            // Action
            await uut.Register(user);

            user.Password = "Forkert Kode";
            var result = await uut.Login(user);
            // Assert

            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
        }
    }
}
