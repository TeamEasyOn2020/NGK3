using Microsoft.Extensions.Options;
using Moq;
using NGK3.Controllers;
using NGK3.Models;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using WebApi;
using WebApi.Context;

namespace NGK3.UnitTest
{
    public class AccountControllerTest
    {
        WeatherContext DbContext;
        AppSettings appSettings;
        IOptions<AppSettings> Options;
        AccountController uut;

        [SetUp]
        void Setup()
        {
            DbContext = Substitute.For<WeatherContext>();
            appSettings = new AppSettings();
            uut = new AccountController(DbContext, Options);
        }

        [Test]
        void register_regUser()
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
            var result = uut.Register(user);

            // Assert
            Assert.That(result.Equals(user));
        }


    }
}
