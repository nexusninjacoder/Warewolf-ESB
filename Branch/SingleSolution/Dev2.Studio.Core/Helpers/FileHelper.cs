﻿using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using Dev2.Providers.Logs;
using Ionic.Zip;

namespace Dev2.Studio.Core.Helpers
{
    public static class FileHelper
    {
        // Used to migrate Dev2 -> Warewolf 
        private const string NewPath = @"Warewolf\";   
        private const string OldPath = @"Dev2\";
        

        /// <summary>
        /// Gets the ouput path.
        /// </summary>
        public static string GetUniqueOutputPath(string extension)
        {
            var path = Path.Combine(new[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                StringResources.App_Data_Directory,
                StringResources.Feedback_Recordings_Directory,
                Guid.NewGuid().ToString() + extension
            });
            return path;
        }

        /// <summary>
        /// Creates the file.
        /// </summary>
        /// <param name="outputTxt">The text to output to the path</param>
        /// <param name="outputPath">The output path.</param>
        /// <author>jurie.smit</author>
        /// <date>2013/01/15</date>
        public static void CreateTextFile(string outputTxt, string outputPath)
        {
            Logger.TraceInfo();
            EnsurePathIsvalid(outputPath, ".txt");
            var fs = File.Open(outputPath,
                                      FileMode.OpenOrCreate,
                                      FileAccess.Write);
            using (var writer = new StreamWriter(fs, Encoding.UTF8))
            {
                Logger.TraceInfo("Writing a text file");
                writer.Write(outputTxt);
            }
        }

        public static void CreateTextFile(StringBuilder outputTxt, string outputPath)
        {
            Logger.TraceInfo();
            EnsurePathIsvalid(outputPath, ".txt");
            var fs = File.Open(outputPath,
                                      FileMode.OpenOrCreate,
                                      FileAccess.Write);
            using(var writer = new StreamWriter(fs, Encoding.UTF8))
            {
                Logger.TraceInfo("Writing a text file");
                writer.Write(outputTxt);
            }
        }

        /// <summary>
        /// Ensures the path isvalid.
        /// </summary>
        /// <exception cref="System.IO.IOException">File specified in the output path already exists.</exception>
        public static void EnsurePathIsvalid(string outputPath, string validExtension)
        {
            var path = new FileInfo(outputPath);
            var extension = Path.GetExtension(outputPath);

            if(string.Compare(extension, validExtension, StringComparison.OrdinalIgnoreCase) != 0)
            {
                throw new InvalidOperationException("The output path can only be to a 'xml' or 'zip' file.");
            }

            if(path.Exists)
            {
                throw new IOException("File specified in the output path already exists.");
            }

            if(path.Directory == null)
            {
                throw new IOException("Output path is invalid.");
            }

            if(!path.Directory.Exists)
            {
                path.Directory.Create();
            }
        }

        /// <summary>
        /// Gets the full path based on a uri and the current assembly.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        /// <author>Jurie.smit</author>
        /// <date>3/6/2013</date>
        public static string GetFullPath(string uri)
        {
            var location = Assembly.GetExecutingAssembly().Location;
            var directory = Path.GetDirectoryName(location);
            if(directory == null) return null;
            var path = Path.Combine(directory, uri);
            return path;
        }


        /// <summary>
        /// Gets the app data path.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        public static string GetAppDataPath(string uri)
        {
            var result = Path.Combine(new[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                StringResources.App_Data_Directory,
                uri
            });

            return result;
        }

        public static string GetStudioLogTempPath()
        {
            var studioLog = CustomTextWriter.LoggingFileName;
            
            if(File.Exists(studioLog))
            {

                string fileContent;
                using(var fs = new FileStream(studioLog, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using(var sr = new StreamReader(fs, Encoding.Default))
                {
                    fileContent = sr.ReadToEnd();
                }

                string uniqueOutputPath = GetUniqueOutputPath(".txt");
                return CreateATemporaryFile(fileContent, uniqueOutputPath);
            }
            return null;
        }

        public static string CreateATemporaryFile(StringBuilder fileContent, string uniqueOutputPath)
        {
            CreateTextFile(fileContent, uniqueOutputPath);
            string sourceDirectoryName = Path.GetDirectoryName(uniqueOutputPath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(uniqueOutputPath);
            string destinationArchiveFileName = Path.Combine(sourceDirectoryName, fileNameWithoutExtension + ".zip");
            using(var zip = new ZipFile())
            {
                zip.AddFile(uniqueOutputPath, ".");
                zip.Save(destinationArchiveFileName);
            }
            return destinationArchiveFileName;
        }

        public static string CreateATemporaryFile(string fileContent, string uniqueOutputPath)
        {
            CreateTextFile(fileContent, uniqueOutputPath);
            string sourceDirectoryName = Path.GetDirectoryName(uniqueOutputPath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(uniqueOutputPath);
            string destinationArchiveFileName = Path.Combine(sourceDirectoryName, fileNameWithoutExtension + ".zip");
            using(var zip = new ZipFile())
            {
                zip.AddFile(uniqueOutputPath, ".");
                zip.Save(destinationArchiveFileName);
            }
            return destinationArchiveFileName;
        }

        public static string GetDebugItemTempFilePath(string uri)
        {
            Logger.TraceInfo();

            if (String.IsNullOrEmpty(uri))
            {
                Logger.TraceInfo("Uri is empty, an exception is thrown");
                throw new ArgumentNullException("uri", @"Cannot pass null or empty uri");
            }

            using (var client = new WebClient{Credentials = CredentialCache.DefaultCredentials})
            {
                string serverLogData = client.UploadString(uri, "");
                string value = serverLogData.Replace("<DataList><Dev2System.ManagmentServicePayload>", "").Replace("</Dev2System.ManagmentServicePayload></DataList>", "");
                string uniqueOutputPath = GetUniqueOutputPath(".txt");
                CreateTextFile(value, uniqueOutputPath);
                return uniqueOutputPath;
            }
        }

        public static void MigrateTempData(string rootPath)
        {

            string FullNewPath = Path.Combine(rootPath, NewPath);
            string FullOldPath = Path.Combine(rootPath, OldPath);

            if (!Directory.Exists(FullOldPath))
            {
                return;//no old data to migrate
            }

            if (!Directory.Exists(FullNewPath))
            {
                Directory.Move(FullOldPath, FullNewPath);
            }
        }

        public static void CreateDirectoryFromString(string filePath)
        {
            var file = new FileInfo(filePath);
            var directory = file.Directory;
            if(directory != null)
            {
                Directory.CreateDirectory(directory.ToString());
            }
            else
            {
                throw new ArgumentException("Invalid File Path");
            }
        }
    }
}