﻿using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using Dev2.Network.Messaging;
using Dev2.Network.Messaging.Messages;

namespace Dev2.Runtime.Configuration
{
    /// <summary>
    /// Do NOT instantiate directly - use static <see cref="Instance" /> property instead; use for testing only!
    /// </summary>
    public class SettingsProvider : NetworkMessageProviderBase<ISettingsMessage>
    {
        #region Singleton Instance

        //
        // Multi-threaded implementation - see http://msdn.microsoft.com/en-us/library/ff650316.aspx
        //
        // This approach ensures that only one instance is created and only when the instance is needed. 
        // Also, the variable is declared to be volatile to ensure that assignment to the instance variable
        // completes before the instance variable can be accessed. Lastly, this approach uses a syncRoot 
        // instance to lock on, rather than locking on the type itself, to avoid deadlocks.
        //
        static volatile SettingsProvider _instance;
        static readonly object SyncRoot = new Object();

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static SettingsProvider Instance
        {
            get
            {
                if(_instance == null)
                {
                    lock(SyncRoot)
                    {
                        if(_instance == null)
                        {
                            _instance = new SettingsProvider();
                        }
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region CTOR

        /// <summary>
        /// Do NOT instantiate directly - use static <see cref="Instance" /> property instead;
        /// </summary>
        public SettingsProvider()
        {
            AssemblyHashCode = GetAssemblyHashCode();
            Configuration = new Settings.Configuration();
        }

        #endregion

        public string AssemblyHashCode { get; private set; }

        public Settings.Configuration Configuration { get; private set; }

        #region ProcessMessage

        public override ISettingsMessage ProcessMessage(ISettingsMessage request)
        {
            if(request == null)
            {
                throw new ArgumentNullException("request");
            }

            switch(request.Action)
            {
                case NetworkMessageAction.Write:
                case NetworkMessageAction.Overwrite:
                    return ProcessWrite(request);

                default:
                    return ProcessRead(request);
            }
        }

        #endregion

        #region ProcessRead

        ISettingsMessage ProcessRead(ISettingsMessage request)
        {
            if(request.AssemblyHashCode != AssemblyHashCode)
            {
                request.Result = NetworkMessageResult.VersionConflict;
                request.AssemblyHashCode = AssemblyHashCode;
                request.Assembly = GetAssemblyBytes();
            }
            else
            {
                request.Assembly = null;
                request.Result = NetworkMessageResult.Success;
            }

            request.ConfigurationXml = Configuration.ToXml();

            return request;
        }

        #endregion

        #region ProcessWrite

        ISettingsMessage ProcessWrite(ISettingsMessage request)
        {
            return request;
        }

        #endregion

        //
        // Static Helpers
        //

        #region GetAssemblyHashCode

        static string GetAssemblyHashCode()
        {
            var assemblyBytes = GetAssemblyBytes();

            var hashAlgorithm = SHA256.Create();
            var hash = hashAlgorithm.ComputeHash(assemblyBytes);
            var hex = BitConverter.ToString(hash).Replace("-", string.Empty);

            return hex;
        }

        #endregion

        #region GetAssemblyBytes

        static byte[] GetAssemblyBytes()
        {
            var assembly = Assembly.GetAssembly(typeof(IConfigurationAssemblyMarker));
            return File.ReadAllBytes(assembly.Location);
        }

        #endregion

        //#region GetConfigurationXml

        //public XElement GetConfigurationXml()
        //{
        //    var appPath = Path.GetFullPath(Assembly.GetExecutingAssembly().Location);
        //    var path = Path.Combine(appPath, ConfigurationManager.AppSettings["ConfigurationSettingsFolder"]);
        //    if(!File.Exists(path))
        //    {
        //        var result = GetConfiguration();
        //        result.Save(path);
        //        return result;
        //    }
        //    return XElement.Load(path);
        //}

        //#endregion

    }
}