using UnitTest.Selenium;
using Selenium;
using System.Configuration;
using System;
using System.Threading;
using NUnit.Framework;

namespace Utilities.UnitTest.Tests.Selenium
{
    /// <summary>
    ///This is a test class for When_using_selenium_server and is intended
    ///to contain all SeleniumSupport_Test Unit Tests
    ///</summary>
    [TestFixture]
    public class When_using_selenium_server
    {

        private string _location;
        private string _package;
        private string _firefoxBrowser;
        private string _ieBrowser;
        public SeleniumServer SeleniumServer { get; set; }

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            _location = ConfigurationManager.AppSettings["seleniumLocation"];
            _package = ConfigurationManager.AppSettings["seleniumPackage"];
            _firefoxBrowser = ConfigurationManager.AppSettings["firefoxBrowser"];
            _ieBrowser = ConfigurationManager.AppSettings["ieBrowser"];

        }

        [TestFixtureTearDown]
        public  void FixtureTearDown()
        {
            // Ensure that no selenium server is left running dependent on
            // what test ran last
            CreateTarget(false).Dispose();

        }

        [TearDown]
        public void TearDown()
        {
            // When running tests individually machine goes so fast
            // selenium cannot keep up leading to tests failing
            Thread.Sleep(1000);
        }

        private SeleniumSupport CreateTarget(bool showWindow)
        {
            if(SeleniumServer == null)
                SeleniumServer = SeleniumServer.Create(_location, _package, showWindow);
            
            return SeleniumSupport.Create(SeleniumServer);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void Should_thow_exception_when_starting_instance_due_to_serverhost_not_provided()
        {
            SeleniumSupport target = CreateTarget(true);

            target.Start("", 4444, _firefoxBrowser, "http://www.google.co.uk");
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void Should_thow_exception_when_starting_instance_due_to_browser_string_not_provided()
        {
            SeleniumSupport target = CreateTarget(true);

            target.Start("localhost",4444, "", "http://www.google.co.uk");
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void Should_thow_exception_when_starting_instance_due_to_url_not_provided()
        {
            SeleniumSupport target = CreateTarget(true);

            target.Start("localhost", 4444, _firefoxBrowser, "");

        }

        [Test]
        public void Should_create_selenium_instance_using_firefox_browser()
        {
            SeleniumSupport target = CreateTarget(true);

            ISelenium actual = target.CreateInstance("localhost", 4444, _firefoxBrowser, "http://www.google.co.uk");

            Assert.IsNotNull(actual);
            Assert.AreEqual(1, target.Instances.Count);

            // Dispose of the selenium support instance otherwise tests to ensure errors thrown when
            // it is not running will not work
            target.Dispose();
            
        }

        [Test]
        public void Should_create_selenium_instance_using_ie_browser()
        {
            SeleniumSupport target = CreateTarget(true);

            ISelenium actual = target.CreateInstance("localhost", 4444, _ieBrowser, "http://www.google.co.uk");

            Assert.IsNotNull(actual);
            Assert.AreEqual(1, target.Instances.Count);

            // must dispose of the instance to prevent further tests failing
            target.DisposeInstance(actual);

            // Dispose of the selenium support instance otherwise tests to ensure errors thrown when
            // it is not running will not work
            target.Dispose();

        }

        [Test]
        public void Should_start_default_instance_of_firefox()
        {
            SeleniumSupport target = CreateTarget(true);

            const string serverHost = "localhost";
            const int serverPort = 4444;
            string browserString = _firefoxBrowser;
            const string browserUrl = "http://www.google.co.uk";
            
            ISelenium actual  = target.Start(serverHost, serverPort, browserString, browserUrl);
            
            Assert.IsNotNull(actual);
            Assert.AreEqual(1, target.Instances.Count);
            Assert.AreSame(target.Selenium , actual);

            // Dispose of the selenium support instance otherwise tests to ensure errors thrown when
            // it is not running will not work
            target.Dispose();

        }

        [Test]
        public void Should_start_default_selenium_instance_of_ie()
        {
            SeleniumSupport target = CreateTarget(true);

            const string serverHost = "localhost";
            const int serverPort = 4444;
            string browserString = _ieBrowser;
            const string browserUrl = "http://www.google.co.uk";

            ISelenium actual = target.Start(serverHost, serverPort, browserString, browserUrl);

            Assert.IsNotNull(actual);
            Assert.AreEqual(1, target.Instances.Count);
            Assert.AreSame(target.Selenium, actual);

            // Dispose of the selenium support instance otherwise tests to ensure errors thrown when
            // it is not running will not work
            target.Dispose();

        }

        [Test]
        public void Should_be_able_to_start_multiple_explicit_instances_for_different_sites()
        {

            SeleniumSupport target = CreateTarget(true);

            const string serverHost = "localhost";
            const int serverPort = 4444;
            string browserString = _firefoxBrowser;

            ISelenium firstInstance  = target.CreateInstance(serverHost, serverPort, browserString, "http://www.google.co.uk");

            Assert.IsNotNull(firstInstance);
            Assert.AreEqual(1, target.Instances.Count);

            ISelenium secondInstance  = target.CreateInstance(serverHost, serverPort, browserString, "http://www.microsoft.com");

            Assert.IsNotNull(secondInstance);
            Assert.AreEqual(2, target.Instances.Count);
            Assert.AreNotSame(firstInstance, secondInstance);

            // Dispose of class to clear up resources
            target.DisposeInstance(firstInstance);
            target.DisposeInstance(secondInstance);

            target.Dispose();
        }

        [Test]
        public void Should_start_standard_instance_and_an_explicit_instance()
        {

            SeleniumSupport target = CreateTarget(true);

            const string serverHost = "localhost";
            const int serverPort = 4444;
            string browserString = _firefoxBrowser;

            ISelenium defaultInstance  = target.Start(serverHost, serverPort, browserString, "http://www.google.co.uk");

            Assert.IsNotNull(defaultInstance);
            Assert.AreEqual(1, target.Instances.Count);

            ISelenium explicitInstance = target.CreateInstance(serverHost, serverPort, browserString, "http://www.microsoft.com");


            Assert.IsNotNull(explicitInstance);
            Assert.AreEqual(2, target.Instances.Count);
            Assert.AreNotSame(defaultInstance, explicitInstance);
            Assert.AreSame(target.Selenium, defaultInstance);

            // Dispose of class to clear up resources
            target.Dispose();


        }

        [Test]
        public void Should_be_able_to_dispose_of_an_explicit_instance_without_affecting_standard_instance()
        {

            SeleniumSupport target = CreateTarget(true);

            const string serverHost = "localhost";
            const int serverPort = 4444;
            string browserString = _firefoxBrowser;

            ISelenium defaultInstance = target.Start(serverHost, serverPort, browserString, "http://www.google.co.uk");

            Assert.IsNotNull(defaultInstance);
            Assert.AreEqual(1, target.Instances.Count);

            ISelenium explicitInstance  = target.CreateInstance(serverHost, serverPort, browserString, "http://www.microsoft.com");

            Assert.IsNotNull(explicitInstance);
            Assert.AreEqual(2, target.Instances.Count);
            Assert.AreNotSame(defaultInstance, explicitInstance);
            Assert.AreSame(target.Selenium, defaultInstance);


            target.DisposeInstance(explicitInstance);
            Assert.AreEqual(1, target.Instances.Count);
            Assert.IsNotNull(target.Selenium);

            // Dispose of class to clear up resources
            target.Dispose();
        }

    }
}
