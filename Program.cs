using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace MPCLogger2
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static  void Main(string[] args)
        {
            string fp = args[0];
            LogOpening(fp);
            StartMPCHC(fp);
        }
        static void StartMPCHC(string filepath)
        {
            Process process = new Process();
            process.StartInfo.FileName = "mpc-hc.exe";
            process.StartInfo.Arguments = "\"" + filepath + "\"";
            process.Start();
        }

        /// <summary>
        /// Log the opening of a media file set to have this program as the default opener.
        /// </summary>
        /// <param name="fp">The path to the file being opened.</param>
        static void LogOpening(string fp)
        {
            // Get universal sorting time, example: 2013-05-05 17:53
            string currentDateTime = DateTime.Now.ToString("u");
            // Create the output string using the datetime and the filepath.
            string logOutput = String.Format("{0} - INFO - {1}", currentDateTime, fp);
            string userProfileDirectory = System.Environment.GetEnvironmentVariable("USERPROFILE");
            string logFile = Path.Combine(userProfileDirectory, "MPC-HC.log");
            string DropBoxDirectory = DropboxPath();
            
            if (!File.Exists(logFile)) {
                File.Create(logFile);
            }
            // Append to the file.
            using (StreamWriter w = File.AppendText(logFile))
            {
                w.WriteLine(logOutput);
            }
            // Copy into Dropbox if it exists.
            if (DropBoxDirectory != null) {
                File.Copy(logFile, Path.Combine(DropBoxDirectory, "MPC-HC.log"), true);
            }
        }

        static string DropboxPath() {
            try
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string dbPath = System.IO.Path.Combine(appDataPath, "Dropbox\\host.db");
                var lines = System.IO.File.ReadAllLines(dbPath);
                var dbBase64Text = Convert.FromBase64String(lines[1]);
                string folderPath = System.Text.ASCIIEncoding.ASCII.GetString(dbBase64Text);
                return folderPath;
            }
            catch (Exception) {
                return null;
            }
        }
    }
}
