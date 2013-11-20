﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using Ionic.Zip;
using Procon.Service.Shared;

namespace Procon.Service {
    public class Updater {

        private TextWriter Writer { get; set; }

        public Updater() {
            this.Writer = new StreamWriter(Defines.UpdateLog);
        }

        public Updater Execute() {

            this.Writer.WriteLine("Updater initialized");

            // #1
            if (this.CheckUpdatesDirectory() == true) {

                // #2
                this.CreateRequiredDirectories();

                // #3
                this.CreateConfigBackup();

                // #4
                // this.TerminateProcess();

                // #5
                this.MoveDirectoryContents(Defines.UpdatesDirectory);

                // #6 Remove the updates directory
                this.DeleteDirectory(Defines.UpdatesDirectory);
            }

            return this;
        }

        public Updater Shutdown() {
            this.Writer.WriteLine("Closing Updater");
            this.Writer.Flush();
            this.Writer.Close();

            return this;
        }

        #region Step #1 - Checking if update is required

        /// <summary>
        /// Checks if the updates directory exists, logging the result
        /// </summary>
        /// <returns>true - dir exists, false otherwise</returns>
        protected bool CheckUpdatesDirectory() {

            bool updatesDirectoryExists = false;

            this.Writer.WriteLine((updatesDirectoryExists = Directory.Exists(Defines.UpdatesDirectory)) == true ? "Updates directory exists, beginning update" : "Updates directory does not exists");

            return updatesDirectoryExists;
        }

        #endregion

        #region Step #2 - Creating required directories

        /// <summary>
        /// Loops through all required directories the update needs
        /// 
        /// This is so the update can forego checks and error logging later.
        /// </summary>
        protected void CreateRequiredDirectories() {

            List<string> directories = new List<string>() { 
                Defines.ConfigsDirectory,
                Defines.ConfigsBackupDirectory
            };

            foreach (string directory in directories) {
                this.CreateDirectory(directory);
            }
        }

        #endregion

        #region Step #3 - Backing up configs (Procon update only, not normal package install)

        protected void CreateConfigBackup() {

            try {
                if (File.Exists(Defines.ProconDirectoryProconCoreDll) == true && File.Exists(Defines.UpdatesDirectoryProconCoreDll) == true) {
                    this.Writer.WriteLine("Creating config backup");

                    FileVersionInfo currentFv = FileVersionInfo.GetVersionInfo(Defines.ProconDirectoryProconCoreDll);
                    FileVersionInfo updatedFv = FileVersionInfo.GetVersionInfo(Defines.UpdatesDirectoryProconCoreDll);

                    String zipFileName = String.Format("{0}_to_{1}_backup.zip", currentFv.FileVersion, updatedFv.FileVersion);

                    using (ZipFile zip = new ZipFile()) {

                        DirectoryInfo configsDirectory = new DirectoryInfo(Defines.ConfigsDirectory);

                        foreach (FileInfo config in configsDirectory.GetFiles("*.cfg")) {
                            this.Writer.WriteLine("\tAdding {0} to archive", config.FullName);
                            zip.AddFile(config.FullName);
                        }

                        foreach (DirectoryInfo directory in configsDirectory.GetDirectories()) {
                            if (String.Compare(directory.FullName, Defines.ConfigsBackupDirectory, StringComparison.OrdinalIgnoreCase) != 0) {
                                this.Writer.WriteLine("\tAdding {0} to archive", directory.FullName);
                                zip.AddDirectory(directory.FullName);
                                // Add files from directory?
                            }
                        }

                        this.Writer.WriteLine("\tSaving archive to {0}", Path.Combine(Defines.ConfigsBackupDirectory, zipFileName));
                        zip.Save(Path.Combine(Defines.ConfigsBackupDirectory, zipFileName));
                    }
                }
            }
            catch (Exception e) {
                this.Writer.WriteLine("\tERROR: {0}", e.Message);
            }
        }

        #endregion

        #region Step #4 - Closing open instances of any executables in "/Updates" and "/"

        /// <summary>
        /// Terminates all executables running in "/" that will be replaced by executables in the updates directory.
        /// </summary>
        protected void TerminateProcess() {

            List<string> localExecutables = this.DiscoverExecutables(Defines.BaseDirectory);
            List<string> updatesExecutables = this.DiscoverExecutables(Defines.UpdatesDirectory);

            // This is so the updater does not attempt suicide.
            localExecutables.Remove(Defines.ProconDirectoryProconExe);

            // Close Procon.exe
            // Close Procon.UI.exe
            // Close Procon.Console.exe
            // Or any other executables we create later..

            foreach (String exe in localExecutables.Where(x => updatesExecutables.Select(Path.GetFileName).Contains(Path.GetFileName(x)))) {
                foreach (Process process in Process.GetProcesses()) {
                    try {
                        if (String.Compare(exe, Path.GetFullPath(process.MainModule.FileName), StringComparison.OrdinalIgnoreCase) == 0) {
                            try {
                                this.Writer.WriteLine("Killing process {1} at {0}", process.Id, exe);
                                process.Kill();
                            }
                            catch (Exception e) {
                                this.Writer.WriteLine("\tERROR: {0}", e.Message);
                            }
                        }
                    }
                    catch (Exception e) {
                        this.Writer.WriteLine("\tERROR: {0}", e.Message);
                    }
                }
            }
        }

        #endregion

        #region Step #5 - Moving and deleting the files from updates directory

        protected void MoveDirectoryContents(string path) {

            this.Writer.WriteLine("Moving contents of directory {0}", path);

            if (Directory.Exists(path) == true) {
                foreach (String file in Directory.GetFiles(path)) {

                    if (String.CompareOrdinal(Path.GetFileName(file), Defines.ProconExe) == 0 || String.CompareOrdinal(Path.GetFileName(file), "Ionic.Zip.Reduced.dll") == 0) {
                        this.Writer.WriteLine("Ignoring file {0} (Updater file)", file);
                        this.DeleteFile(file);
                    }
                    else {

                        String destinationFile = file.Remove(file.LastIndexOf("Updates" + Path.DirectorySeparatorChar, StringComparison.Ordinal), ("Updates" + Path.DirectorySeparatorChar).Length);

                        this.DeleteFile(destinationFile);

                        this.MoveFile(file, destinationFile);
                    }
                }

                foreach (string directory in Directory.GetDirectories(path)) {
                    this.MoveDirectoryContents(directory);

                    this.DeleteDirectory(directory);
                }
            }
        }

        #endregion

        #region File I/O with logging

        /// <summary>
        /// Creates a directory, logging the progress and errors (if any)
        /// </summary>
        /// <param name="path">The path to create</param>
        protected void CreateDirectory(string path) {
            if (Directory.Exists(path) == false) {
                this.Writer.WriteLine("Creating directory: {0}", path);

                try {
                    Directory.CreateDirectory(path);
                }
                catch (Exception e) {
                    this.Writer.WriteLine("\tERROR: {0}", e.Message);
                }
            }
        }

        /// <summary>
        /// Finds all .exe files at a given path
        /// </summary>
        /// <param name="path">The path to search for .exe's</param>
        /// <returns>A list of fullname exe files</returns>
        protected List<string> DiscoverExecutables(string path) {
            List<string> executables = new List<string>();

            try {
                if (Directory.Exists(path) == true) {
                    executables = new List<string>(Directory.GetFiles(path, "*.exe"));
                }
            }
            catch (Exception e) {
                this.Writer.WriteLine("\tERROR: {0}", e.Message);
            }

            return executables;
        }

        protected void DeleteDirectory(string path) {
            this.Writer.WriteLine("Deleting directory {0}", path);

            try {
                Directory.Delete(path);
            }
            catch (Exception e) {
                this.Writer.WriteLine("\tERROR: {0}", e.Message);
            }
        }

        protected void DeleteFile(string file) {
            this.Writer.WriteLine("Deleting file {0}", file);

            try {
                File.Delete(file);
            }
            catch (Exception e) {
                this.Writer.WriteLine("\tERROR: {0}", e.Message);
            }
        }

        protected void MoveFile(string file, string destination) {
            this.Writer.WriteLine("Moving {0} to {1}", file, destination);

            try {
                this.CreateDirectory(Path.GetDirectoryName(destination));

                File.Move(file, destination);
            }
            catch (Exception e) {
                this.Writer.WriteLine("\tERROR: {0}", e.Message);
            }
        }

        #endregion
    }
}
