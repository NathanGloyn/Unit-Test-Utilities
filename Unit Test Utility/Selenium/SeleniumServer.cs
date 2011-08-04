using System;
using System.Security.Permissions;
using System.Diagnostics;
using System.IO;

namespace UnitTest.Selenium
{
    /// <summary>
    /// Class encapsulates functionality for starting and stopping Selenium Server
    /// </summary>
    public class SeleniumServer : IDisposable
    {
        private static SeleniumServer Server { get; set; }
        private bool _disposed = false;
        private Process _seleniumProcess;

        /// <summary>
        /// Indicates if the Selenium Server is started
        /// </summary>
        public bool Started { get; private set; }
        
        /// <summary>
        /// Location of the selenium package
        /// </summary>
        public string Location { get; private set; }
        
        /// <summary>
        /// Name of the selenium package file e.g. selenium-server.jar
        /// </summary>
        public string Package { get; private set; }

        /// <summary>
        /// Indicateds if the command window will be displayed
        /// </summary>
        public bool ShouldDisplayWindow { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="location">Directory holding the selenium package file</param>
        /// <param name="package">Name of the selenium package file.  Must contain words selenium-server to be valid</param>
        /// <param name="shoulDisplayCommandWindow">Indicateds if the command window should be displayed</param>
        /// <exception cref="ArgumentNullException">Thrown if location or package is null or empty string</exception>
        /// <exception cref="DirectoryNotFoundException">Thrown if the location provided is not valid</exception>
        /// <exception cref="ArgumentException">Thrown if the name of the package does not match expected pattern</exception>
        /// <exception cref="FileNotFoundException">Thrown if the package file cannot be found in the stated location</exception>
        SeleniumServer(string location, string package, bool shoulDisplayCommandWindow)
        {

            // Check the parameters have been provided
            if (string.IsNullOrEmpty(location))
                throw new ArgumentNullException("Selenium sever location must be supplied.");

            if (string.IsNullOrEmpty(package))
                throw new ArgumentNullException("Name of selenium package must be supplied.");

            // Check that the values supplied are valid
            if (!Directory.Exists(location))
                throw new DirectoryNotFoundException("Location provided for server is not a valid directory");

            if (!package.Contains("selenium-server"))
                throw new ArgumentException("Package does not appear to contain a valid selenium server package name");

            if (!File.Exists(Path.Combine(location, package.Substring(package.IndexOf("s")))))
                throw new FileNotFoundException("The Package name provided does not exist");

            this.Location = location;
            this.Package = package;
            this.ShouldDisplayWindow = shoulDisplayCommandWindow;
        }

        /// <summary>
        /// Creates a Selenium Server instance
        /// </summary>
        /// <param name="location">Directory where the selenium jar file is located</param>
        /// <param name="package">Is the name of the selenium jar file to execute</param>
        /// <param name="displayCommandWindow">Indicates if the command window running selenium should be displayed</param>
        /// <returns>Instance of the SeleniumServer class</returns>
        /// <remarks>Class acts as a singleton to ensure only one instance of the selenium server is running at any one time.</remarks>
        public static SeleniumServer Create(string location, string package, bool displayCommandWindow)
        {
            if (Server == null)
            {
                Server = new SeleniumServer(location, package, displayCommandWindow);
            }

            return Server;
        }

        /// <summary>
        /// Starts the selenium server
        /// </summary>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Execution)]
        public void Start()
        {
            if (!this.Started)
            {
                StartProcess(this.Location, " -singleWindow", this.ShouldDisplayWindow);
                this.Started = true;
            }
        }


        /// <summary>
        /// Stops all instances and the selenium server
        /// </summary>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Execution)]
        public void Stop()
        {
            _seleniumProcess.CloseMainWindow();
            _seleniumProcess.Kill();
            this.Started = false;
        }


        /// <summary>
        /// Creates a new process running the jar file
        /// </summary>
        /// <param name="workingDirectory">directory where application will be run from</param>
        /// <param name="arguments">Command line arguments to be passed to the application</param>
        /// <param name="showWindow">Whether the CMD window should be displayed</param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Execution)]
        private void StartProcess(string workingDirectory, string arguments, bool showWindow)
        {
            // Set the location of the selenium jar file
            string jarFileLocation = Path.Combine(workingDirectory, this.Package);

            // Start the selenium server
            _seleniumProcess = new Process();
            _seleniumProcess.StartInfo.FileName = "\"java\"";
            _seleniumProcess.StartInfo.Arguments = " -jar \"" + jarFileLocation + "\"" + arguments;
            _seleniumProcess.StartInfo.WorkingDirectory = workingDirectory;
            _seleniumProcess.StartInfo.UseShellExecute = false;
            _seleniumProcess.EnableRaisingEvents = true;
            _seleniumProcess.Exited += new EventHandler(Process_Exited);
            if (!showWindow)
                _seleniumProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            _seleniumProcess.Start();
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
            if (!this._disposed)
            {
                if (disposing)
                {
                    if (!_seleniumProcess.HasExited)
                    {
                        Stop();
                    }
                    Server = null;
                }
            }

            this._disposed = true;
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            Started = false;
        }
    }
}
