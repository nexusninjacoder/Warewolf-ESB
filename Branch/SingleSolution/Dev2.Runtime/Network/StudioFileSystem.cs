﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dev2.Common;

namespace Dev2.DynamicServices
{
    public class StudioFileSystem
    {
        #region Instance Fields
        private string _rootDirectory;
        private string[] _ensuredDirectories;
        #endregion

        #region Public Properties
        public string RootDirectory { get { return _rootDirectory; } }
        #endregion

        #region Constructor
        public StudioFileSystem(string rootDirectory, IEnumerable<string> ensuredDirectories)
        {
            Initialize(rootDirectory, ensuredDirectories.ToList());
        }
        #endregion

        #region Initialization Handling
        private void Initialize(string rootDirectory, List<string> ensuredDirectories)
        {
            if(ensuredDirectories == null) ensuredDirectories = new List<string>();
            if(String.IsNullOrEmpty(rootDirectory) || rootDirectory.IndexOfAny(Path.GetInvalidPathChars()) != -1) throw new ArgumentException("rootDirectory must be a valid file path.");
            if(!Path.IsPathRooted(rootDirectory)) rootDirectory = Path.Combine(Environment.CurrentDirectory, rootDirectory);
            _rootDirectory = rootDirectory;

            if(!Directory.Exists(_rootDirectory)) Directory.CreateDirectory(_rootDirectory);

            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            List<string> sourceDirectories = ensuredDirectories;
            bool logsEnsured = sourceDirectories.Contains("Logs", comparer);
            bool dataEnsured = sourceDirectories.Contains("Data", comparer);
            bool cacheEnsured = sourceDirectories.Contains("Cache", comparer);

            if(sourceDirectories.Count == 0)
            {
                if(!logsEnsured) sourceDirectories.Add("Logs");
                if(!dataEnsured) sourceDirectories.Add("Data");
                if(!cacheEnsured) sourceDirectories.Add("Cache");
            }
            else
            {
                if(!cacheEnsured) sourceDirectories.Insert(0, "Cache");
                if(!dataEnsured) sourceDirectories.Insert(0, "Data");
                if(!logsEnsured) sourceDirectories.Insert(0, "Logs");
            }

            List<string> actualEnsuredDirectories = new List<string>();

            for(int i = 0; i < sourceDirectories.Count; i++)
            {
                string currentDir = sourceDirectories[i];
                if(String.IsNullOrEmpty(currentDir)) continue;

                try
                {
                    currentDir = Path.IsPathRooted(currentDir) ? currentDir : Path.Combine(_rootDirectory, currentDir);
                }
                catch(Exception ex)
                {
                    ServerLogger.LogError(ex);
                    currentDir = null;
                }

                if(currentDir != null)
                {
                    try
                    {
                        if(!Directory.Exists(currentDir)) Directory.CreateDirectory(currentDir);
                    }
                    catch(Exception ex)
                    {
                        ServerLogger.LogError(ex);
                        currentDir = null;
                    }
                    if(currentDir != null) actualEnsuredDirectories.Add(currentDir);
                }
            }

            _ensuredDirectories = actualEnsuredDirectories.ToArray();
        }
        #endregion

        #region Relative Path Handling
        public string GetRelativePath(string relativePath, SpecialFolder folder)
        {
            return GetRelativePath(folder.ToString() + "\\" + relativePath);
        }

        public string GetRelativePath(string relativePath)
        {
            if(_rootDirectory == null) return relativePath;
            string tempPath = "";

            try
            {
                tempPath = Path.IsPathRooted(relativePath) ? (relativePath.Contains(_rootDirectory) ? relativePath : null) : Path.Combine(_rootDirectory, relativePath);

                if(String.IsNullOrEmpty(tempPath))
                {
                    string fileName = null;

                    try
                    {
                        fileName = Path.GetFileName(relativePath);
                    }
                    catch(Exception ex)
                    {
                        ServerLogger.LogError(ex);
                        fileName = null;
                    }

                    if(String.IsNullOrEmpty(fileName)) fileName = Path.GetDirectoryName(relativePath);
                    tempPath = Path.Combine(_ensuredDirectories[1], fileName);
                }
            }
            catch(Exception ex)
            {
                ServerLogger.LogError(ex);
                tempPath = relativePath;
            }

            return tempPath;
        }
        #endregion

        #region Ensured Path Handling
        public string GetEnsuredPath(string path, SpecialFolder folder)
        {
            return GetEnsuredPath(folder.ToString() + "\\" + path, false);
        }

        public string GetEnsuredPath(string path)
        {
            return GetEnsuredPath(path, true);
        }

        private string GetEnsuredPath(string path, bool searchEnsured)
        {
            if(String.IsNullOrEmpty(path)) return null;
            bool isFile = false;

            try
            {
                if(!Path.IsPathRooted(path))
                    path = Path.Combine(_rootDirectory, path);
                if(Path.HasExtension(path))
                    isFile = true;

                if((isFile ? File.Exists(path) : Directory.Exists(path)))
                    return path;
                else if(!isFile)
                    return null;

                path = Path.GetFileName(path);
            }
            catch(Exception ex)
            {
                ServerLogger.LogError(ex);
                return null;
            }

            if(!searchEnsured) return null;

            try
            {

                string fileName = path;

                for(int i = 0; i < _ensuredDirectories.Length; i++)
                {
                    path = Path.Combine(_ensuredDirectories[i], fileName);
                    if(File.Exists(path)) return path;
                }

                path = null;
            }
            catch(Exception ex)
            {
                ServerLogger.LogError(ex);
                path = null;
            }

            return path;
        }
        #endregion

        public enum SpecialFolder
        {
            Logs = 0,
            Data = 1,
            Cache = 2
        }
    }
}