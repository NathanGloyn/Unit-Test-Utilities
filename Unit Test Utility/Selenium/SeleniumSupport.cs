using System;
using System.Collections.Generic;
using Selenium;
using System.Security.Permissions;

namespace UnitTest.Selenium
{
    /// <summary>
    /// Class that provides support for using selenium in unit tests
    /// </summary>
    /// <remarks>Class is created as a Singleton so that it is not continually created and destroyed</remarks>
    public class SeleniumSupport : IDisposable
    {
        /// <summary>
        /// Holds a reference to the class that provides access to the actual Selenium server
        /// </summary>
        private SeleniumServer Server { get; set; }

        private bool _disposed;

        /// <summary>
        /// Private constructor to remove ability for class to be instantiated
        /// </summary>
        SeleniumSupport(SeleniumServer seleniumServer)
        {
            this.Server = seleniumServer;
            _disposed = false;
            Instances = new List<ISelenium>();
        }

        /// <summary>
        /// List of instances created
        /// </summary>
        /// <remarks>Created as internally accessible as only used for testing purposes</remarks>
        internal List<ISelenium> Instances { get; private set; }

        /// <summary>
        /// Provides access to the singleton instance of this support class
        /// </summary>
        public static SeleniumSupport Instance { get; private set;}

        /// <summary>
        /// Creates a new instance of this class
        /// </summary>
        /// <param name="seleniumServer">Instance of the SeleniumServer to use</param>
        /// <returns>Instance of SeleniumSupport class</returns>
        /// <exception cref="ArgumentNullException">Exception thrown if the selenium server is not provided</exception>
        public static SeleniumSupport Create(SeleniumServer seleniumServer)
        {
            if(seleniumServer == null)
            {
                throw new ArgumentNullException("seleniumServer");
            }

            if(Instance == null || Instance._disposed)
            {
                Instance = new SeleniumSupport(seleniumServer);
            }

            return Instance;
        }

        /// <summary>
        /// Returns instance of current selenium object 
        /// </summary>
        public ISelenium Selenium { get; private set; }

        /// <summary>
        /// Indicates if the selenium server has been started
        /// </summary>
        public bool SeleniumServerStarted 
        {
            get { return Server.Started; }
        }

        /// <summary>
        /// Starts an instance of selenium
        /// </summary>
        /// <param name="serverHost">the server hosting the site to be tested</param>
        /// <param name="serverPort">the port that selenium use for running</param>
        /// <param name="browserString">string holding the browser that we want to test in</param>
        /// <param name="browserUrl">Url to open to begin testing</param>
        /// <returns>Instance of selenium opened using arguments supplied</returns>
        /// <exception cref="ArgumentNullException">This exception is thrown if any of the expected
        /// string arguments are passed as null or empty string</exception>
        public ISelenium Start(string serverHost, int serverPort, string browserString, string browserUrl)
        {
            Selenium = CreateInstance(serverHost, serverPort, browserString, browserUrl);

            return Selenium;
        }

        /// <summary>
        /// Creates an instance of selenium to be used by the user
        /// </summary>
        /// <param name="serverHost">the server hosting the site to be tested</param>
        /// <param name="serverPort">the port that selenium use for running</param>
        /// <param name="browserString">string holding the browser that we want to test in</param>
        /// <param name="browserUrl">Url to open to begin testing</param>
        /// <returns>Instance of selenium ready to use</returns>
        /// <exception cref="ArgumentNullException">This exception is thrown if any of the expected
        /// string arguments are passed as null or empty string</exception>
        public ISelenium CreateInstance(string serverHost, int serverPort, string browserString, string browserUrl)
        {

            if (string.IsNullOrEmpty(serverHost))
                throw new ArgumentNullException("serverHost");

            if (string.IsNullOrEmpty(browserString))
                throw new ArgumentNullException("browserString");

            if (string.IsNullOrEmpty(browserUrl))
                throw new ArgumentNullException("browserUrl");

            // If the actual selenium server isn't running then we need to start it up
            if (! SeleniumServerStarted)
                Server.Start();

            ISelenium newInstance = new DefaultSelenium(serverHost, serverPort, browserString, browserUrl);
            Instances.Add(newInstance);

            newInstance.Start();

            return newInstance;
        }

        /// <summary>
        /// Disposes of an instance of selenium
        /// </summary>
        /// <param name="instance">The instance to be disposed</param>
        /// <remarks>This method only needs to be called if a specific instance of selenium has been created
        /// and used rather than the default instance provided by the class</remarks>
        public void DisposeInstance(ISelenium instance)
        {

            try
            {
                if(SeleniumServerStarted)
                    instance.Stop();
            }
            catch
            {
                // If we get an error here not so worried as currently removing the instance.
                // We don't want to throw an error in case it stops the tests running
            }

            Instances.Remove(instance);
        }



        #region IDisposable Members

        /// <summary>
        /// Dispose method exposed to allow client to perform cleanup
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// Disposes resources
        /// </summary>
        /// <param name="disposing">Flag to indicate we are disposing of resources</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Execution)]
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {

                    try
                    {

                        if (Instances.Count > 0)
                        {

                            for (int i = Instances.Count; i > 1; i--)
                            {
                                DisposeInstance(Instances[i-1]);
                            }

                            Instances[0].ShutDownSeleniumServer();
                            
                        }

                    }
                    catch
                    {
                        // If an error occurs disposing of the instances then simply stop the server
                        if(Server.Started)
                            Server.Stop();
                    }
                    finally
                    {
                       Instances.Clear();
                       Selenium = null; 
                    }

                }
            }

            _disposed = true;
        }
    }
}
