using System;
using System.IO;
using NUnit.Framework;
using System.Configuration;
using UnitTest.Selenium;

namespace Utilities.UnitTest.Tests.Selenium
{
    [TestFixture]
    public class When_creating_an_instance_of_SeleniumServer
    {
        private static string _location;
        private static string _package;

        [TestFixtureSetUp]
        public void MyClassInitialize()
        {
            _location = ConfigurationManager.AppSettings["seleniumLocation"];
            _package = ConfigurationManager.AppSettings["seleniumPackage"];
        }

        [Test]
        public void Should_throw_exception_due_to_missing_location_setting()
        {
            Assert.Throws<ArgumentNullException>(() => SeleniumServer.Create("", _package, false));
        }

        [Test]
        public void Should_throw_exception_due_to_missing_package_setting()
        {
            Assert.Throws<ArgumentNullException>(() => SeleniumServer.Create(_location, "", false));
        }

        /// <summary>
        /// Test correct exception thrown if no location supplied
        /// </summary>
        [Test]
        public void Should_throw_exception_on_start_of_selenium_due_to_invalid_server_location()
        {
            Assert.Throws<DirectoryNotFoundException>(() =>  SeleniumServer.Create("C:\\Doesnotexist", _package, false));

        }

        [Test]
        public void Should_throw_exception_on_start_of_selenium_server_due_to_invalid_package()
        {
            Assert.Throws<ArgumentException>(() => SeleniumServer.Create(_location, "abc.txt", false));

        }


        [Test]
        public void Should_throw_exception_on_start_of_selenium_server_due_to_no_package_file()
        {
            Assert.Throws<FileNotFoundException>(() =>  SeleniumServer.Create(_location, "selenium-server.txt", false));
        }

        /// <summary>
        /// Test able to start the selenium server
        /// </summary>
        [Test]
        public void Should_start_selenium_server()
        {
            SeleniumServer target = SeleniumServer.Create(_location, _package, false);

            target.Start();

            Assert.IsTrue(target.Started);

            target.Dispose();
        }


        [Test]
        public void Should_stop_selenium_server()
        {
            SeleniumServer target = SeleniumServer.Create(_location, _package, false);

            target.Start();

            target.Stop();

            Assert.IsFalse(target.Started);

            target.Dispose();
        }
    }
}
