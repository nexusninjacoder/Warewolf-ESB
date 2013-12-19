﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Dev2.Common;
using Dev2.Common.Common;
using Dev2.Common.ExtMethods;
using Dev2.Data.Enums;
using Dev2.Data.ServiceModel;
using Dev2.Data.ServiceModel.Messages;
using Dev2.DynamicServices;
using Dev2.DynamicServices.Objects;
using Dev2.DynamicServices.Objects.Base;
using Dev2.Providers.Errors;
using Dev2.Runtime.Compiler;
using Dev2.Runtime.ESB.Management;
using Dev2.Runtime.Security;
using Dev2.Runtime.ServiceModel.Data;
using ServiceStack.Common.Extensions;

namespace Dev2.Runtime.Hosting
{

    public class ResourceCatalog : IResourceCatalog
    {
        readonly ConcurrentDictionary<Guid, List<IResource>> _workspaceResources = new ConcurrentDictionary<Guid, List<IResource>>();
        readonly ConcurrentDictionary<Guid, object> _workspaceLocks = new ConcurrentDictionary<Guid, object>();
        readonly ConcurrentDictionary<string, object> _fileLocks = new ConcurrentDictionary<string, object>();
        readonly object _loadLock = new object();

        readonly ConcurrentDictionary<Guid, ManagementServiceResource> _managementServices = new ConcurrentDictionary<Guid, ManagementServiceResource>();
        readonly ConcurrentDictionary<string, List<DynamicServiceObjectBase>> _frequentlyUsedServices = new ConcurrentDictionary<string, List<DynamicServiceObjectBase>>();

        #region Singleton Instance

        //
        // Multi-threaded implementation - see http://msdn.microsoft.com/en-us/library/ff650316.aspx
        //
        // This approach ensures that only one instance is created and only when the instance is needed. 
        // Also, the variable is declared to be volatile to ensure that assignment to the instance variable
        // completes before the instance variable can be accessed. Lastly, this approach uses a syncRoot 
        // instance to lock on, rather than locking on the type itself, to avoid deadlocks.
        //
        static volatile ResourceCatalog _instance;
        static readonly object SyncRoot = new Object();

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static ResourceCatalog Instance
        {
            get
            {
                if(_instance == null)
                {
                    lock(SyncRoot)
                    {
                        if(_instance == null)
                        {
                            _instance = new ResourceCatalog(EsbManagementServiceLocator.GetServices());

                            // bootstrap the compile message repo ;)
                            CompileMessageRepo.Instance.Ping();

                        }
                    }
                }

                return _instance;
            }
        }


        #endregion

        #region CTOR

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceCatalog" /> class.
        /// <remarks>
        /// DO NOT instantiate directly - use static Instance property instead; this is for testing only!
        /// </remarks>
        /// </summary>
        /// <param name="managementServices">The management services to be loaded.</param>
        public ResourceCatalog(IEnumerable<DynamicService> managementServices = null)
        {
            // MUST load management services BEFORE server workspace!!
            if(managementServices != null)
            {
                foreach(var service in managementServices)
                {
                    var resource = new ManagementServiceResource(service);
                    _managementServices.TryAdd(resource.ResourceID, resource);
                }
            }

            LoadFrequentlyUsedServices().Wait();
        }

        #endregion

        #region Properties

        public int WorkspaceCount
        {
            get
            {
                return _workspaceResources.Count;
            }
        }

        #endregion

        #region GetResourceCount

        public int GetResourceCount(Guid workspaceID)
        {
            return GetResources(workspaceID).Count;
        }

        #endregion

        #region RemoveWorkspace

        public void RemoveWorkspace(Guid workspaceID)
        {
            object workspaceLock;
            lock(_loadLock)
            {
                if(!_workspaceLocks.TryRemove(workspaceID, out workspaceLock))
                {
                    workspaceLock = new object();
                }
            }
            lock(workspaceLock)
            {
                List<IResource> resources;
                _workspaceResources.TryRemove(workspaceID, out resources);
            }
        }

        #endregion

        #region GetResource

        public IResource GetResource(Guid workspaceID, string resourceName, ResourceType resourceType = ResourceType.Unknown, string version = null)
        {
            if(string.IsNullOrEmpty(resourceName))
            {
                throw new ArgumentNullException("resourceName");
            }
            var resources = GetResources(workspaceID);
            return resources.FirstOrDefault(r => string.Equals(r.ResourceName, resourceName, StringComparison.InvariantCultureIgnoreCase)
                && (resourceType == ResourceType.Unknown || r.ResourceType == resourceType));
        }

        public IResource GetResource(Guid workspaceID, Guid resourceID, Version version = null)
        {
            var resources = GetResources(workspaceID);
            var resource = resources.FirstOrDefault(r => r.ResourceID == resourceID && (version == null || r.Version == version));
            return resource;
        }

        #endregion

        #region GetResourceContents

        /// <summary>
        /// Gets the contents of the resource with the given name.
        /// </summary>
        /// <param name="workspaceID">The workspace ID to be queried.</param>
        /// <param name="resourceID">The resource ID to be queried.</param>
        /// <param name="version">The version to be queried.</param>
        /// <returns>The resource's contents or <code>string.Empty</code> if not found.</returns>
        public StringBuilder GetResourceContents(Guid workspaceID, Guid resourceID, Version version = null)
        {
            var resource = GetResource(workspaceID, resourceID, version);
            return GetResourceContents(resource);
        }

        /// <summary>
        /// Gets the contents of the resource with the given name.
        /// </summary>
        /// <param name="workspaceID">The workspace ID to be queried.</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>
        /// The resource's contents or <code>string.Empty</code> if not found.
        /// </returns>
        public StringBuilder GetResourceContents(Guid workspaceID, string resourceName)
        {
            var resource = GetResource(workspaceID, resourceName);
            return GetResourceContents(resource);
        }

        /// <summary>
        /// Gets the resource's contents.
        /// </summary>
        /// <param name="resource">The resource to be queried.</param>
        /// <returns>The resource's contents or <code>string.Empty</code> if not found.</returns>
        public StringBuilder GetResourceContents(IResource resource)
        {
            StringBuilder contents = new StringBuilder();

            if(resource == null || string.IsNullOrEmpty(resource.FilePath) || !File.Exists(resource.FilePath))
            {
                return contents;
            }

            // Open the file with the file share option of read. This will ensure that if the file is opened for write while this read operation
            // is happening the wite will fail.
            using(FileStream fs = new FileStream(resource.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using(StreamReader sr = new StreamReader(fs))
                {
                    while (!sr.EndOfStream)
                    {
                        contents.Append(sr.ReadLine());
                    }
                }
            
            }

            return contents;
        }

        #endregion

        #region GetPayload

        /// <summary>
        /// Gets the contents of the resource with the given guids.
        /// </summary>
        /// <param name="workspaceID">The workspace ID to be queried.</param>
        /// <param name="guidCsv">The guids to be queried.</param>
        /// <param name="type">The type string: WorkflowService, Service, Source, ReservedService or *, to be queried.</param>
        /// <returns>The resource's contents or <code>string.Empty</code> if not found.</returns>
        /// <exception cref="System.ArgumentNullException">type</exception>
        public StringBuilder GetPayload(Guid workspaceID, string guidCsv, string type)
        {
            if(type == null)
            {
                throw new ArgumentNullException("type");
            }

            if(guidCsv == null)
            {
                guidCsv = string.Empty;
            }

            var guids = new List<Guid>();
            foreach(var guidStr in guidCsv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                Guid guid;
                if(Guid.TryParse(guidStr, out guid))
                {
                    guids.Add(guid);
                }
            }
            var resourceTypes = ResourceTypeConverter.ToResourceTypes(type);

            var workspaceResources = GetResources(workspaceID);
            var resources = workspaceResources.FindAll(r => guids.Contains(r.ResourceID)
                                                            && resourceTypes.Contains(r.ResourceType));

            var result = ToPayload(resources);
            return result;
        }

        /// <summary>
        /// Gets the contents of the resources with the given source type.
        /// </summary>
        /// <param name="workspaceID">The workspace ID to be queried.</param>
        /// <param name="sourceType">The type of the source to be queried.</param>
        /// <returns>The resource's contents or <code>string.Empty</code> if not found.</returns>
        public IEnumerable GetModels(Guid workspaceID, enSourceType sourceType)
        {
            var resourceType = ResourceTypeConverter.ToResourceType(sourceType);

            var workspaceResources = GetResources(workspaceID);
            var resources = workspaceResources.FindAll(r => r.ResourceType == resourceType);

            IEnumerable result = null;

            // TODO : Convert sourceType to model type ;)
            switch(sourceType)
            {
                case enSourceType.Dev2Server:
                    result = BuildServerList(resources);
                break;

                case enSourceType.EmailSource:
                    result = BuildEmailList(resources);
                break;

                case enSourceType.SqlDatabase:
                    result = BuildSqlServerList(resources);
                break;

                case enSourceType.Plugin:
                    result = BuildPluginList(resources);
                break;

                case enSourceType.WebSource:
                    result = BuildWebList(resources);
                break;

                default:
                    result = null;
                break;
            }

            return result;
        }

        /// <summary>
        /// Gets the contents of the resources with the given source type.
        /// </summary>
        /// <param name="workspaceID">The workspace ID to be queried.</param>
        /// <param name="sourceType">The type of the source to be queried.</param>
        /// <returns>The resource's contents or <code>string.Empty</code> if not found.</returns>
        public StringBuilder GetPayload(Guid workspaceID, enSourceType sourceType)
        {
            var resourceType = ResourceTypeConverter.ToResourceType(sourceType);

            var workspaceResources = GetResources(workspaceID);
            var resources = workspaceResources.FindAll(r => r.ResourceType == resourceType);
            var result = ToPayload(resources);
            return result;
        }

        /// <summary>
        /// Gets the contents of the resource with the given name and type (WorkflowService, Service, Source, ReservedService or *).
        /// </summary>
        /// <param name="workspaceID">The workspace ID to be queried.</param>
        /// <param name="resourceName">The name of the resource to be queried.</param>
        /// <param name="type">The type string: WorkflowService, Service, Source, ReservedService or *, to be queried.</param>
        /// <param name="userRoles">The user roles to be queried.</param>
        /// <param name="useContains"><code>true</code> if matching resource name's should contain the given <paramref name="resourceName"/>;
        /// <code>false</code> if resource name's must exactly match the given <paramref name="resourceName"/>.</param>
        /// <returns>The resource's contents or <code>string.Empty</code> if not found.</returns>
        /// <exception cref="System.Runtime.Serialization.InvalidDataContractException">ResourceName and Type are missing from the request</exception>
        public StringBuilder GetPayload(Guid workspaceID, string resourceName, string type, string userRoles, bool useContains = true)
        {
            if(string.IsNullOrEmpty(resourceName) && string.IsNullOrEmpty(type))
            {
                throw new InvalidDataContractException("ResourceName and Type are missing from the request");
            }

            if(string.IsNullOrEmpty(resourceName) || resourceName == "*")
            {
                resourceName = string.Empty;
            }

            var resourceTypes = ResourceTypeConverter.ToResourceTypes(type);

            var workspaceResources = GetResources(workspaceID);
            var resources = useContains
                ? workspaceResources.FindAll(r => r.ResourceName.Contains(resourceName) && resourceTypes.Contains(r.ResourceType))
                : workspaceResources.FindAll(r => r.ResourceName.Equals(resourceName, StringComparison.InvariantCultureIgnoreCase)
                                                && resourceTypes.Contains(r.ResourceType));

            var result = ToPayload(resources);
            return result;
        }

        public IList<Resource> GetResourceList(Guid workspaceID, string guidCsv, string type)
        {
            if(type == null)
            {
                throw new ArgumentNullException("type");
            }

            if(guidCsv == null)
            {
                guidCsv = string.Empty;
            }

            var guids = new List<Guid>();
            foreach(var guidStr in guidCsv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                Guid guid;
                if(Guid.TryParse(guidStr, out guid))
                {
                    guids.Add(guid);
                }
            }
            var resourceTypes = ResourceTypeConverter.ToResourceTypes(type);

            var workspaceResources = GetResources(workspaceID);
            var resources = workspaceResources.FindAll(r => guids.Contains(r.ResourceID)
                                                            && resourceTypes.Contains(r.ResourceType));

            return resources.Cast<Resource>().ToList();
        }

        public IList<Resource> GetResourceList(Guid workspaceID, string resourceName, string type, string userRoles, bool useContains = true)
        {
            if(string.IsNullOrEmpty(resourceName) && string.IsNullOrEmpty(type))
            {
                throw new InvalidDataContractException("ResourceName and Type are missing from the request");
            }

            if(string.IsNullOrEmpty(resourceName) || resourceName == "*")
            {
                resourceName = string.Empty;
            }

            var resourceTypes = ResourceTypeConverter.ToResourceTypes(type);

            var workspaceResources = GetResources(workspaceID);
            var resources = useContains
                ? workspaceResources.FindAll(r => r.ResourceName.Contains(resourceName) && resourceTypes.Contains(r.ResourceType))
                : workspaceResources.FindAll(r => r.ResourceName.Equals(resourceName, StringComparison.InvariantCultureIgnoreCase)
                                                && resourceTypes.Contains(r.ResourceType));

            return resources.Cast<Resource>().ToList();
        }

        #endregion

        #region LoadWorkspace

        public void LoadWorkspace(Guid workspaceID)
        {
            var workspaceLock = GetWorkspaceLock(workspaceID);
            lock(workspaceLock)
            {
                _workspaceResources.AddOrUpdate(workspaceID,
                    id => LoadWorkspaceImpl(workspaceID),
                    (id, resources) => LoadWorkspaceImpl(workspaceID));
            }
        }

        #endregion

        #region LoadWorkspaceImpl

        List<IResource> LoadWorkspaceImpl(Guid workspaceID)
        {
            var folders = ServiceModel.Resources.RootFolders.Values.Distinct();
            var workspacePath = EnvironmentVariables.GetWorkspacePath(workspaceID);
            var userServices = LoadWorkspaceViaBuilder(workspacePath, folders.ToArray());

            var result = userServices.Union(_managementServices.Values);

            return result.ToList();
        }

        #endregion

        #region LoadWorkspaceAsync

        // Travis.Frisinger - 02.05.2013 
        // 
        // Removed the Async operation with file stream as it would fail to use the correct stream from time to time
        // causing the integration test suite to fail. By moving the operation into a Parallel.ForEach approach this 
        // appears to have nearly the same impact with better stability.
        // ResourceCatalogBuilder now contains the refactored async logic ;)

        /// <summary>
        /// Loads the workspace via builder.
        /// </summary>
        /// <param name="workspacePath">The workspace path.</param>
        /// <param name="folders">The folders.</param>
        /// <returns></returns>
        public IList<IResource> LoadWorkspaceViaBuilder(string workspacePath, params string[] folders)
        {
            ResourceCatalogBuilder builder = new ResourceCatalogBuilder();

            builder.BuildCatalogFromWorkspace(workspacePath, folders);

            return builder.ResourceList;
        } 

        #endregion

        #region CopyResource

        public bool CopyResource(Guid resourceID, Guid sourceWorkspaceID, Guid targetWorkspaceID, string userRoles = null)
        {
            var resource = GetResource(sourceWorkspaceID, resourceID);
            return CopyResource(resource, targetWorkspaceID, userRoles);
        }

        public bool CopyResource(IResource resource, Guid targetWorkspaceID, string userRoles = null)
        {
            if(resource != null)
            {
                var copy = new Resource(resource);
                var contents = GetResourceContents(resource);
                var saveResult = SaveImpl(targetWorkspaceID, copy, contents);
                return saveResult.Status == ExecStatus.Success;
            }
            return false;
        }

        #endregion

        #region SaveResource

        public ResourceCatalogResult SaveResource(Guid workspaceID, StringBuilder resourceXml, string userRoles = null)
        {
            if(resourceXml == null || resourceXml.Length == 0)
            {
                throw new ArgumentNullException("resourceXml");
            }

            var workspaceLock = GetWorkspaceLock(workspaceID);
            lock(workspaceLock)
            {
                var xml = resourceXml.ToXElement();

                var resource = new Resource(xml);
                resource.UpgradeXml(xml);

                StringBuilder result = xml.ToStringBuilder();

                return CompileAndSave(workspaceID, resource, result);    
            }
        }

        public ResourceCatalogResult SaveResource(Guid workspaceID, IResource resource, string userRoles = null)
        {
            if(resource == null)
            {
                throw new ArgumentNullException("resource");
            }

            var workspaceLock = GetWorkspaceLock(workspaceID);
            lock(workspaceLock)
            {
                if(resource.ResourceID == Guid.Empty)
                {
                    resource.ResourceID = Guid.NewGuid();
                }

                var result = resource.ToStringBuilder();

                return CompileAndSave(workspaceID, resource, result);
            }
        }

        #endregion

        #region DeleteResource

        public ResourceCatalogResult DeleteResource(Guid workspaceID, string resourceName, string type, string userRoles = null)
        {
            var workspaceLock = GetWorkspaceLock(workspaceID);
            lock(workspaceLock)
            {
                if(resourceName == "*")
                {
                    return new ResourceCatalogResult
                    {
                        Status = ExecStatus.NoWildcardsAllowed,
                        Message = "<Result>Delete resources does not accept wildcards.</Result>."
                    };
                }

                if(string.IsNullOrEmpty(resourceName) || string.IsNullOrEmpty(type))
                {
                    throw new InvalidDataContractException("ResourceName or Type is missing from the request");
                }

                var resourceTypes = ResourceTypeConverter.ToResourceTypes(type, false);

                var workspaceResources = GetResources(workspaceID);
                var resources = workspaceResources.FindAll(r =>
                    string.Equals(r.ResourceName, resourceName, StringComparison.InvariantCultureIgnoreCase)
                    && resourceTypes.Contains(r.ResourceType));

                switch(resources.Count)
                {
                    case 0:
                        return new ResourceCatalogResult
                        {
                            Status = ExecStatus.NoMatch,
                            Message = string.Format("<Result>{0} '{1}' was not found.</Result>", type, resourceName)
                        };

                    case 1:
                        return DeleteImpl(workspaceID, resources, workspaceResources);

                    default:
                        return new ResourceCatalogResult
                        {
                            Status = ExecStatus.DuplicateMatch,
                            Message = string.Format("<Result>Multiple matches found for {0} '{1}'.</Result>", type, resourceName)
                        };
                }
            }
        }

        public ResourceCatalogResult DeleteResource(Guid workspaceID, Guid resourceID, string type, string userRoles = null)
        {
            var workspaceLock = GetWorkspaceLock(workspaceID);
            lock(workspaceLock)
            {
                if(resourceID == Guid.Empty || string.IsNullOrEmpty(type))
                {
                    throw new InvalidDataContractException("ResourceID or Type is missing from the request");
                }

                var resourceTypes = ResourceTypeConverter.ToResourceTypes(type, false);

                var workspaceResources = GetResources(workspaceID);
                var resources = workspaceResources.FindAll(r =>
                    Equals(r.ResourceID, resourceID)
                    && resourceTypes.Contains(r.ResourceType));

                switch(resources.Count)
                {
                    case 0:
                        return new ResourceCatalogResult
                        {
                            Status = ExecStatus.NoMatch,
                            Message = string.Format("<Result>{0} '{1}' was not found.</Result>", type, resourceID)
                        };

                    case 1:
                        return DeleteImpl(workspaceID, resources, workspaceResources);

                    default:
                        return new ResourceCatalogResult
                        {
                            Status = ExecStatus.DuplicateMatch,
                            Message = string.Format("<Result>Multiple matches found for {0} '{1}'.</Result>", type, resourceID)
                        };
                }
            }
        }

        private ResourceCatalogResult DeleteImpl(Guid workspaceID, List<IResource> resources, List<IResource> workspaceResources)
        {
            var resource = resources[0];

            VersionControl(Path.GetDirectoryName(resource.FilePath), resource);

            workspaceResources.Remove(resource);
            if(File.Exists(resource.FilePath))
            {
                File.Delete(resource.FilePath);
            }
            var messages = new List<CompileMessageTO>
                {
                    new CompileMessageTO
                        {
                            ErrorType = ErrorType.Critical,
                            MessageID = Guid.NewGuid(),
                            MessagePayload = "The resource has been deleted",
                            MessageType = CompileMessageType.ResourceDeleted,
                            ServiceID = resource.ResourceID
                        }
                };
            UpdateDependantResourceWithCompileMessages(workspaceID, resource, messages);
            return new ResourceCatalogResult
                {
                    Status = ExecStatus.Success,
                    Message = "Success"
                };
        }

        #endregion

        #region RollbackResource

        public bool RollbackResource(Guid workspaceID, Guid resourceID, Version fromVersion, Version toVersion)
        {
            var resource = GetResource(workspaceID, resourceID, fromVersion);
            if(resource != null)
            {
                var folder = Path.GetDirectoryName(resource.FilePath);
                if(folder != null)
                {
                    var fileName = Path.Combine(folder, "VersionControl", string.Format("{0}.V{1}.xml", resource.ResourceName, toVersion.Major));
                    if(File.Exists(fileName))
                    {
                        StringBuilder fileContent = new StringBuilder();
                        using(FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            using(StreamReader sr = new StreamReader(fs))
                            {
                                while(!sr.EndOfStream)
                                {
                                    fileContent.Append(sr.ReadLine());
                                }
                            }
                        }

                        var isValid = HostSecurityProvider.Instance.VerifyXml(fileContent);
                        if(isValid)
                        {
                            lock(GetFileLock(resource.FilePath))
                            {
                                fileContent.WriteToFile(resource.FilePath, Encoding.UTF8);
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        #endregion

        #region SyncTo

        public void SyncTo(string sourceWorkspacePath, string targetWorkspacePath, bool overwrite = true, bool delete = true, IList<string> filesToIgnore = null)
        {
            if(filesToIgnore == null)
            {
                filesToIgnore = new List<string>();
            }

            var directories = new List<string> { "Sources", "Services" };

            foreach(var directory in directories)
            {
                var source = new DirectoryInfo(Path.Combine(sourceWorkspacePath, directory));
                var destination = new DirectoryInfo(Path.Combine(targetWorkspacePath, directory));

                if(!source.Exists)
                {
                    continue;
                }

                if(!destination.Exists)
                {
                    destination.Create();
                }

                //
                // Get the files from the source and desitnations folders, excluding the files which are to be ignored
                //
                var sourceFiles = source.GetFiles().Where(f => !filesToIgnore.Contains(f.Name)).ToList();
                var destinationFiles = destination.GetFiles().Where(f => !filesToIgnore.Contains(f.Name)).ToList();

                //
                // Calculate the files which are to be copied from source to destination, this respects the override parameter
                //

                var filesToCopyFromSource = new List<FileInfo>();

                if(overwrite)
                {
                    filesToCopyFromSource.AddRange(sourceFiles);
                }
                else
                {
                    filesToCopyFromSource.AddRange(sourceFiles
                        // ReSharper disable SimplifyLinqExpression
                        .Where(sf => !destinationFiles.Any(df => String.Compare(df.Name, sf.Name, StringComparison.OrdinalIgnoreCase) == 0)));
                    // ReSharper restore SimplifyLinqExpression
                }

                //
                // Calculate the files which are to be deleted from the destination, this respects the delete parameter
                //
                var filesToDeleteFromDestination = new List<FileInfo>();
                if(delete)
                {
                    filesToDeleteFromDestination.AddRange(destinationFiles
                        // ReSharper disable SimplifyLinqExpression
                        .Where(sf => !sourceFiles.Any(df => String.Compare(df.Name, sf.Name, StringComparison.OrdinalIgnoreCase) == 0)));
                    // ReSharper restore SimplifyLinqExpression
                }

                //
                // Copy files from source to desination
                //
                foreach(var file in filesToCopyFromSource)
                {
                    file.CopyTo(Path.Combine(destination.FullName, file.Name), true);
                }
            }
        }

        #endregion

        #region GetDynamicObjects

        public List<TServiceType> GetDynamicObjects<TServiceType>(Guid workspaceID, string resourceName, bool useContains = false)
            where TServiceType : DynamicServiceObjectBase
        {
            if(string.IsNullOrEmpty(resourceName))
            {
                throw new ArgumentNullException("resourceName");
            }

            List<DynamicServiceObjectBase> results;

            if(useContains)
            {
                var resources = GetResources(workspaceID);
                results = GetDynamicObjects(resources.Where(r => r.ResourceName.Contains(resourceName)));
            }
            else
            {
                var resource = GetResource(workspaceID, resourceName);
                results = resource == null ? new List<DynamicServiceObjectBase>() : GetDynamicObjects(resource);
            }
            return results.OfType<TServiceType>().ToList();
        }

        public List<TServiceType> GetDynamicObjects<TServiceType>(Guid workspaceID, Guid resourceID)
            where TServiceType : DynamicServiceObjectBase
        {
            if(resourceID == Guid.Empty)
            {
                throw new ArgumentNullException("resourceID");
            }

            var resource = GetResource(workspaceID, resourceID);
            var results = resource == null ? new List<DynamicServiceObjectBase>() : GetDynamicObjects(resource);
            return results.OfType<TServiceType>().ToList();
        }

        public List<DynamicServiceObjectBase> GetDynamicObjects(IResource resource)
        {
            if(resource == null)
            {
                throw new ArgumentNullException("resource");
            }

            var result = new List<DynamicServiceObjectBase>();
            AddResourceAsDynamicServiceObject(result, resource);
            return result;
        }

        public List<DynamicServiceObjectBase> GetDynamicObjects(Guid workspaceID)
        {
            var resources = GetResources(workspaceID);
            return GetDynamicObjects(resources);
        }

        public List<DynamicServiceObjectBase> GetDynamicObjects(IEnumerable<IResource> resources)
        {
            if(resources == null)
            {
                throw new ArgumentNullException("resources");
            }

            var result = new List<DynamicServiceObjectBase>();
            foreach(var resource in resources)
            {
                AddResourceAsDynamicServiceObject(result, resource);
            }
            return result;
        }

        #endregion

        //
        // Private Methods
        //

        #region Enum To Source Resource Conversion

        private IEnumerable BuildServerList(IEnumerable<IResource> resources)
        {
            var result = new List<Connection>();

            foreach(var resource in resources)
            {
                var payload = ToPayload(resource);
                XElement xe = payload.ToXElement();

                result.Add(new Connection(xe)); 
            }

            return result;
        }

        private IEnumerable BuildEmailList(IEnumerable<IResource> resources)
        {
            var result = new List<EmailSource>();

            foreach(var resource in resources)
            {
                var payload = ToPayload(resource);
                XElement xe = payload.ToXElement();

                result.Add(new EmailSource(xe));
            }

            return result;
        }

        private IEnumerable BuildSqlServerList(IEnumerable<IResource> resources)
        {
            var result = new List<DbSource>();

            foreach(var resource in resources)
            {
                var payload = ToPayload(resource);
                XElement xe = payload.ToXElement();

                result.Add(new DbSource(xe));
            }

            return result;
        }

        private IEnumerable BuildPluginList(IEnumerable<IResource> resources)
        {
            var result = new List<PluginSource>();

            foreach(var resource in resources)
            {
                var payload = ToPayload(resource);
                XElement xe = payload.ToXElement();

                result.Add(new PluginSource(xe));
            }
        
            return result;
        }

        private IEnumerable BuildWebList(IEnumerable<IResource> resources)
        {
            var result = new List<WebSource>();

            foreach(var resource in resources)
            {
                var payload = ToPayload(resource);
                XElement xe = payload.ToXElement();

                result.Add(new WebSource(xe));
            }

            return result;
        }

        #endregion

        #region GetResources

        List<IResource> GetResources(Guid workspaceID)
        {
            var workspaceLock = GetWorkspaceLock(workspaceID);
            lock(workspaceLock)
            {
                return _workspaceResources.GetOrAdd(workspaceID, LoadWorkspaceImpl);
            }
        }

        public virtual IResource GetResource(Guid workspaceID, Guid serviceID)
        {
            var workspaceLock = GetWorkspaceLock(workspaceID);
            lock(workspaceLock)
            {
                return _workspaceResources.GetOrAdd(workspaceID, LoadWorkspaceImpl).FirstOrDefault(c => c.ResourceID == serviceID);
            }
        }

        public virtual T GetResource<T>(Guid workspaceID, Guid serviceID) where T : Resource, new()
        {
            var resourceContents = ResourceContents<T>(workspaceID, serviceID);
            if(resourceContents == null || resourceContents.Length == 0) return null;
            return GetResource<T>(resourceContents);
        }

        static T GetResource<T>(StringBuilder resourceContents) where T : Resource, new()
        {
            var elm = resourceContents.ToXElement();
            object[] args = { elm };
                return (T)Activator.CreateInstance(typeof(T), args);    
            }

        public T GetResource<T>(Guid workspaceID, string resourceName) where T : Resource, new()
        {
            var resourceContents = ResourceContents<T>(workspaceID, resourceName);
            if(resourceContents == null || resourceContents.Length == 0) return null;
            return GetResource<T>(resourceContents);
        }

        StringBuilder ResourceContents<T>(Guid workspaceID, string resourceName) where T : Resource, new()
        {
            var resource = GetResource(workspaceID, resourceName);
            var resourceContents = GetResourceContents(resource);
            if(CheckType<T>(resource)) return null;
            return resourceContents;
        }

        StringBuilder ResourceContents<T>(Guid workspaceID, Guid resourceID) where T : Resource, new()
        {
            var resource = GetResource(workspaceID, resourceID);
            var resourceContents = GetResourceContents(resource);
            if(CheckType<T>(resource)) return null;
            return resourceContents;
        }

        static bool CheckType<T>(IResource resource) where T : Resource, new()
        {
            if(resource != null)
            {
                if(typeof(T) == typeof(Workflow) && resource.ResourceType != ResourceType.WorkflowService)
                {
                return true;
            }
                if(typeof(T) == typeof(DbService) && resource.ResourceType != ResourceType.DbService)
            {
                return true;
            }
                if(typeof(T) == typeof(DbSource) && resource.ResourceType != ResourceType.DbSource)
            {
                return true;
            }
                if(typeof(T) == typeof(PluginService) && resource.ResourceType != ResourceType.PluginService)
            {
                return true;
            }
                if(typeof(T) == typeof(PluginSource) && resource.ResourceType != ResourceType.PluginSource)
            {
                return true;
            }
                if(typeof(T) == typeof(WebService) && resource.ResourceType != ResourceType.WebService)
            {
                return true;
            }
                if(typeof(T) == typeof(WebSource) && resource.ResourceType != ResourceType.WebSource)
            {
                return true;
            }
            }
            return false;
        }

        #endregion

        #region GetWorkspaceLock

        object GetWorkspaceLock(Guid workspaceID)
        {
            lock(_loadLock)
            {
                return _workspaceLocks.GetOrAdd(workspaceID, guid => new object());
            }
        }

        #endregion

        #region GetFileLock

        object GetFileLock(string file)
        {
            return _fileLocks.GetOrAdd(file, o => new object());
        }

        #endregion GetFileLock

        #region VersionControl

        void VersionControl(string directoryName, IResource resource)
        {
            var versionDirectory = String.Format("{0}\\{1}", directoryName, "VersionControl");
            if(!Directory.Exists(versionDirectory))
            {
                Directory.CreateDirectory(versionDirectory);
            }

            if(File.Exists(resource.FilePath))
            {
                var count = Directory.GetFiles(versionDirectory, String.Format("{0}*.xml", resource.ResourceName)).Count();

                File.Copy(resource.FilePath, String.Format("{0}\\{1}.V{2}.xml", versionDirectory, resource.ResourceName, (count + 1).ToString(CultureInfo.InvariantCulture)), true);
            }
        }

        #endregion

        #region CompileAndSave

        ResourceCatalogResult CompileAndSave(Guid workspaceID, IResource resource, StringBuilder contents)
        {
            // Find the service before edits ;)
            DynamicService beforeService = Instance.GetDynamicObjects<DynamicService>(workspaceID, resource.ResourceName).FirstOrDefault();                                     
                        
            ServiceAction beforeAction = null;
            if(beforeService != null)
            {
                beforeAction = beforeService.Actions.FirstOrDefault();
            }

            var result = SaveImpl(workspaceID, resource, contents);

            if(result.Status == ExecStatus.Success)
            {
            CompileTheResourceAfterSave(workspaceID, resource, contents, beforeAction);
                SavedResourceCompileMessage(workspaceID, resource, result.Message);
            }

            return result;
        }

        #endregion

        #region SaveImpl

        ResourceCatalogResult SaveImpl(Guid workspaceID, IResource resource, StringBuilder contents)
        {
            var resources = GetResources(workspaceID);
            var conflicting = resources.FirstOrDefault(r => r.ResourceType != resource.ResourceType && r.ResourceName.Equals(resource.ResourceName, StringComparison.InvariantCultureIgnoreCase));
            if(conflicting != null)
            {
                return new ResourceCatalogResult
                {
                    Status = ExecStatus.DuplicateMatch,
                    Message = string.Format("Compilation Error: There is a {0} with the same name.", conflicting.ResourceType)
                };
            }

            var workspacePath = EnvironmentVariables.GetWorkspacePath(workspaceID);
            var directoryName = Path.Combine(workspacePath, ServiceModel.Resources.RootFolders[resource.ResourceType]);
            resource.FilePath = String.Format("{0}\\{1}.xml", directoryName, resource.ResourceName);

            #region Save to disk

            if(!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            VersionControl(directoryName, resource);

            if(File.Exists(resource.FilePath))
            {
                // Remove readonly attribute if it is set
                var attributes = File.GetAttributes(resource.FilePath);
                if((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    File.SetAttributes(resource.FilePath, attributes ^ FileAttributes.ReadOnly);
                }
            }

            XElement xml = contents.ToXElement();
            xml = resource.UpgradeXml(xml);

            StringBuilder result = xml.ToStringBuilder();

            var signedXml = HostSecurityProvider.Instance.SignXml(result);

            lock(GetFileLock(resource.FilePath))
            {
                signedXml.WriteToFile(resource.FilePath, Encoding.UTF8);
            }

            #endregion

            #region Add to catalog

            var index = resources.IndexOf(resource);
            var updated = false;
            if(index != -1)
            {
                resources.RemoveAt(index);
                updated = true;
            }
            resource.GetInputsOutputs(xml);
            resource.ReadDataList(xml);
            resource.SetIsNew(xml);
            resource.UpdateErrorsBasedOnXML(xml);

            resources.Add(resource);
            
            #endregion

            return new ResourceCatalogResult
            {
                Status = ExecStatus.Success,
                Message = string.Format("{0} {1} '{2}'", (updated ? "Updated" : "Added"), resource.ResourceType, resource.ResourceName)
            };
        }

        void SavedResourceCompileMessage(Guid workspaceID, IResource resource, string saveMessage)
        {
            var savedResourceCompileMessage = new List<CompileMessageTO>
            {
                new CompileMessageTO
                {
                    ErrorType = ErrorType.None,
                    MessageID = Guid.NewGuid(),
                    MessagePayload = saveMessage,                    
                    ServiceID = resource.ResourceID,
                    ServiceName = resource.ResourceName,
                    MessageType = CompileMessageType.ResourceSaved,
                    WorkspaceID = workspaceID,
                }
            };

            CompileMessageRepo.Instance.AddMessage(workspaceID, savedResourceCompileMessage);
        }

        public void CompileTheResourceAfterSave(Guid workspaceID, IResource resource, StringBuilder contents, ServiceAction beforeAction)
            {
            if(beforeAction != null)
            {
                // Compile the service 
                ServiceModelCompiler smc = new ServiceModelCompiler();

                var messages = GetCompileMessages(resource, contents, beforeAction, smc);
                if(messages != null)
                {
                    CompileMessageRepo.Instance.AddMessage(workspaceID, messages); //Sends the message for the resource being saved
                    UpdateDependantResourceWithCompileMessages(workspaceID, resource, messages);
                }
            }
        }

        static IList<CompileMessageTO> GetCompileMessages(IResource resource, StringBuilder contents, ServiceAction beforeAction, ServiceModelCompiler smc)
        {
            List<CompileMessageTO> messages = new List<CompileMessageTO>();
            switch(beforeAction.ActionType)
            {
                case enActionType.InvokeStoredProc:
                    messages.AddRange(smc.Compile(resource.ResourceID, ServerCompileMessageType.DbMappingChangeRule, beforeAction.ResourceDefinition, contents));
                    messages.AddRange(smc.Compile(resource.ResourceID, ServerCompileMessageType.DbIsRequireChangeRule, beforeAction.ResourceDefinition, contents));
                    break;
                case enActionType.InvokeWebService:
                    messages.AddRange(smc.Compile(resource.ResourceID, ServerCompileMessageType.WebServiceMappingChangeRule, beforeAction.ResourceDefinition, contents));
                    messages.AddRange(smc.Compile(resource.ResourceID, ServerCompileMessageType.WebServiceIsRequiredChangeRule, beforeAction.ResourceDefinition, contents));
                    break;
                case enActionType.Plugin:
                    messages.AddRange(smc.Compile(resource.ResourceID, ServerCompileMessageType.PluginMappingChangeRule, beforeAction.ResourceDefinition, contents));
                    messages.AddRange(smc.Compile(resource.ResourceID, ServerCompileMessageType.PluginIsRequiredChangeRule, beforeAction.ResourceDefinition, contents));
                    break;
                case enActionType.Workflow:
                    messages.AddRange(smc.Compile(resource.ResourceID, ServerCompileMessageType.WorkflowMappingChangeRule, beforeAction.ResourceDefinition, contents));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return messages;
        }

        //Sends the messages for effected resources
        void UpdateDependantResourceWithCompileMessages(Guid workspaceID, IResource resource, IList<CompileMessageTO> messages)
        {
            var dependants = Instance.GetDependentsAsResourceForTrees(workspaceID, resource.ResourceName);
            var dependsMessageList = new List<CompileMessageTO>();
            foreach(var dependant in dependants)
            {
                var affectedResource = GetResource(workspaceID, dependant.ResourceName);
                foreach(var compileMessageTO in messages)
                {
                    compileMessageTO.WorkspaceID = workspaceID;
                    compileMessageTO.UniqueID = dependant.UniqueID;
                    compileMessageTO.ServiceName = affectedResource.ResourceName;
                    compileMessageTO.ServiceID = affectedResource.ResourceID;
                    dependsMessageList.Add(compileMessageTO.Clone());
                }
                UpdateResourceXML(workspaceID, affectedResource, messages);
                }
            CompileMessageRepo.Instance.AddMessage(workspaceID, dependsMessageList);
            }

        void UpdateResourceXML(Guid workspaceID, IResource effectedResource, IList<CompileMessageTO> compileMessagesTO)
        {
            var resourceContents = GetResourceContents(workspaceID, effectedResource.ResourceID);
            UpdateXMLToDisk(effectedResource, compileMessagesTO, resourceContents);
            var serverResource = GetResource(Guid.Empty, effectedResource.ResourceName);
            if(serverResource != null)
            {
                resourceContents = GetResourceContents(Guid.Empty, serverResource.ResourceID);
                UpdateXMLToDisk(serverResource, compileMessagesTO, resourceContents);
            }
        }

        void UpdateXMLToDisk(IResource resource, IList<CompileMessageTO> compileMessagesTO, StringBuilder resourceContents)
        {

            var resourceElement = resourceContents.ToXElement();
            if(compileMessagesTO.Count > 0)
            {
                SetErrors(resourceElement, compileMessagesTO);
                UpdateIsValid(resourceElement);
            }
            else
            {
                UpdateIsValid(resourceElement);
            }

            StringBuilder contents = resourceElement.ToStringBuilder();
            contents.WriteToFile(resource.FilePath, Encoding.UTF8);
            
        }

        void SetErrors(XElement resourceElement, IList<CompileMessageTO> compileMessagesTO)
        {
            if(compileMessagesTO == null || compileMessagesTO.Count == 0)
            {
                return;
            }
            var errorMessagesElement = GetErrorMessagesElement(resourceElement);
            if(errorMessagesElement == null)
            {
                errorMessagesElement = new XElement("ErrorMessages");
                resourceElement.Add(errorMessagesElement);
            }
            else
            {
                //TODO: How do we determine what really changed e.g. remove 1 of 2 err msgs
                compileMessagesTO.ForEach(to =>
                {
                    IEnumerable<XElement> xElements = errorMessagesElement.Elements("ErrorMessage");
                    XElement firstOrDefault = xElements.FirstOrDefault(element =>
                    {
                        XAttribute xAttribute = element.Attribute("InstanceID");
                        if(xAttribute != null)
                        {
                            return xAttribute.Value == to.UniqueID.ToString();
            }
                        return false;
                    });
                    if(firstOrDefault != null)
                    {
                        firstOrDefault.Remove();
                    }
                });

            }

            foreach(var compileMessageTO in compileMessagesTO)
            {
                var errorMessageElement = new XElement("ErrorMessage");
                errorMessagesElement.Add(errorMessageElement);
                errorMessageElement.Add(new XAttribute("InstanceID", compileMessageTO.UniqueID));
                errorMessageElement.Add(new XAttribute("Message", compileMessageTO.MessageType.GetDescription()));
                errorMessageElement.Add(new XAttribute("ErrorType", compileMessageTO.ErrorType));
                errorMessageElement.Add(new XAttribute("MessageType", compileMessageTO.MessageType));
                errorMessageElement.Add(new XAttribute("FixType", compileMessageTO.ToFixType()));
                errorMessageElement.Add(new XAttribute("StackTrace", ""));
                errorMessageElement.Add(new XCData(compileMessageTO.MessagePayload));
            }
        }

        static XElement GetErrorMessagesElement(XElement resourceElement)
        {
            var errorMessagesElement = resourceElement.Element("ErrorMessages");
            return errorMessagesElement;
        }

        static void UpdateIsValid(XElement resourceElement)
        {
            bool isValid = false;
            var isValidAttrib = resourceElement.Attribute("IsValid");
            var errorMessagesElement = resourceElement.Element("ErrorMessages");
            if(errorMessagesElement == null || !errorMessagesElement.HasElements)
            {
                isValid = true;
            }
            if(isValidAttrib == null)
            {
                resourceElement.Add(new XAttribute("IsValid", isValid));
            }
            else
            {
                isValidAttrib.SetValue(isValid);
            }
        }

        #endregion

        #region ToPayload

        public StringBuilder ToPayload(IResource resource)
        {
            var result = new StringBuilder();

            if(resource.ResourceType == ResourceType.ReservedService)
            {
                result.AppendFormat("<Service Name=\"{0}\" ResourceType=\"{1}\" />", resource.ResourceName, resource.ResourceType);
            }
            else
            {
                var contents = GetResourceContents(resource);
                if(contents != null)
                {
                    contents = contents.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "");
                    result.Append(contents);
                }
            }

            return result;
        }

        StringBuilder ToPayload(IEnumerable<IResource> resources)
        {
            var result = new StringBuilder();
            foreach(var resource in resources)
            {
                if(resource.ResourceType == ResourceType.ReservedService)
                {
                    result.AppendFormat("<Service Name=\"{0}\" ResourceType=\"{1}\" />", resource.ResourceName, resource.ResourceType);
                }
                else
                {
                    var contents = GetResourceContents(resource);
                    if(contents != null)
                    {
                        contents = contents.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "");
                        result.Append(contents);
                    }
                }
            }

            return result;
        }

        #endregion

        #region AddResourceAsDynamicServiceObject

        void AddResourceAsDynamicServiceObject(List<DynamicServiceObjectBase> result, IResource resource)
        {
            if(resource.ResourceType == ResourceType.ReservedService)
            {
                var managementResource = resource as ManagementServiceResource;
                if(managementResource != null)
                {
                    result.Add(managementResource.Service);
                }
            }
            else
            {
                List<DynamicServiceObjectBase> objects;
                if(!_frequentlyUsedServices.TryGetValue(resource.ResourceName, out objects))
                {
                    objects = GenerateObjectGraph(resource);
                }
                else
                {
                    ServerLogger.LogError(string.Format("{0} -> Resource Catalog Cache HIT", resource.ResourceName));
                }
                if(objects != null)
                {
                    result.AddRange(objects);
                }
            }
        }

        List<DynamicServiceObjectBase> GenerateObjectGraph(IResource resource)
        {
            var xml = GetResourceContents(resource);
            if(xml == null || xml.Length > 0)
            {
                return new ServiceDefinitionLoader().GenerateServiceGraph(xml);
            }

            return null;
        }

        #endregion

        public async Task LoadFrequentlyUsedServices()
        {
            // do we really need this still - YES WE DO ELSE THERE ARE INSTALL ISSUES WHEN LOADING FROM FRESH ;)

            var serviceNames = new[]
            {
                "XXX"
            };

            foreach(var serviceName in serviceNames)
            {
                var resourceName = serviceName;
                
                var theTask = new Task(() =>
                {
                    var resource = GetResource(GlobalConstants.ServerWorkspaceID, resourceName);
                    var objects = GenerateObjectGraph(resource);
                    _frequentlyUsedServices.TryAdd(resourceName, objects);
                });
                theTask.Start();
                await theTask;
                theTask.Dispose();
            }

        }

        public List<string> GetDependants(Guid workspaceID, string resourceName)
        {
            // ReSharper disable LocalizableElement
            if(string.IsNullOrEmpty(resourceName)) throw new ArgumentNullException("resourceName", "No resource name given.");
            // ReSharper restore LocalizableElement

            var resources = GetResources(workspaceID);
            var dependants = new List<string>();
            resources.ForEach(resource =>
            {
                if(resource.Dependencies == null) return;
                resource.Dependencies.ForEach(tree =>
                {
                    if(tree.ResourceName == resourceName)
                    {
                        dependants.Add(resource.ResourceName);
                    }
                });
            });
            return dependants.ToList();
        }

        public ResourceCatalogResult RenameResource(Guid workspaceID, Guid resourceID, string newName)
        {
            if(resourceID == null)
            {
                throw new ArgumentNullException("resourceID", @"No value provided for resourceID");
            }
            if(string.IsNullOrEmpty(newName))
            {
                throw new ArgumentNullException("newName", @"No value provided for newName");
            }
            var resourcesToUpdate = Instance.GetResources(workspaceID, resource => resource.ResourceID == resourceID).ToArray();
            try
            {
                if(!resourcesToUpdate.Any())
                {
                    return new ResourceCatalogResult
                    {
                        Status = ExecStatus.Fail,
                        Message = string.Format("{0} '{1}' to '{2}'", "Failed to Find Resource", resourceID, newName)
                    };
                }

                //rename and save to workspace
                var renameResult = UpdateResourceName(workspaceID, resourcesToUpdate[0], newName);
                if(renameResult.Status != ExecStatus.Success)
                {
                    return new ResourceCatalogResult
                    {
                        Status = ExecStatus.Fail,
                        Message =
                            string.Format("{0} '{1}' to '{2}'",
                                            "Failed to Rename Resource", resourceID, newName)
                    };
                }
            }
            catch(Exception)
            {
                return new ResourceCatalogResult
                {
                    Status = ExecStatus.Fail,
                    Message = string.Format("{0} '{1}' to '{2}'", "Failed to Rename Resource", resourceID, newName)
                };
            }
            return new ResourceCatalogResult
            {
                Status = ExecStatus.Success,
                Message = string.Format("{0} '{1}' to '{2}'", "Renamed Resource", resourceID, newName)
            };
        }

        ResourceCatalogResult UpdateResourceName(Guid workspaceID, IResource resource, string newName)
        {
            //rename where used
            RenameWhereUsed(GetDependentsAsResourceForTrees(workspaceID, resource.ResourceName), workspaceID, resource.ResourceName, newName);

            //rename resource
            var resourceContents = GetResourceContents(workspaceID, resource.ResourceID);

            var resourceElement = resourceContents.ToXElement();
            //xml name attibute
            var nameAttrib = resourceElement.Attribute("Name");
            string oldName = null;
            if(nameAttrib == null)
            {
                resourceElement.Add(new XAttribute("Name", newName));
            }
            else
            {
                oldName = nameAttrib.Value;
                nameAttrib.SetValue(newName);
            }
            //xaml
            var actionElement = resourceElement.Element("Action");
            if(actionElement != null)
            {
                var xaml = actionElement.Element("XamlDefinition");
                if(xaml != null)
                {
                    xaml.SetValue(xaml.Value
                        .Replace("x:Class=\"" + oldName, "x:Class=\"" + newName)
                        .Replace("ToolboxFriendlyName=\"" + oldName, "ToolboxFriendlyName=\"" + newName)
                        .Replace("DisplayName=\"" + oldName, "DisplayName=\"" + newName));
                }
            }
            //xml display name element
            var displayNameElement = resourceElement.Element("DisplayName");
            if (displayNameElement != null)
            {
                displayNameElement.SetValue(newName);
            }

            resource.ResourceName = newName;
            //delete old resource in local workspace without updating dependants with compile messages
            if(File.Exists(resource.FilePath))
            {
                lock(GetFileLock(resource.FilePath))
                {
                    File.Delete(resource.FilePath);
                }
            }
            //update file path
            if (oldName != null)
            {
                resource.FilePath = resource.FilePath.Replace(oldName, newName);
            }
            //re-create, resign and save to file system the new resource
            StringBuilder contents = resourceElement.ToStringBuilder();

            return SaveImpl(workspaceID, resource, contents);
            
        }

        private void RenameWhereUsed(IEnumerable<ResourceForTree> dependants, Guid workspaceID, string oldName, string newName)
        {
            foreach(var dependant in dependants)
            {
                var dependantResource = GetResource(workspaceID, dependant.ResourceID);
                //rename where used
                var resourceContents = GetResourceContents(workspaceID, dependantResource.ResourceID);

                var resourceElement = resourceContents.ToXElement();
                    //in the xaml only
                    var actionElement = resourceElement.Element("Action");
                    if(actionElement != null)
                    {
                        var xaml = actionElement.Element("XamlDefinition");
                        if(xaml != null)
                        {
                            xaml.SetValue(xaml.Value
                                .Replace("DisplayName=\"" + oldName, "DisplayName=\"" + newName)
                                .Replace("ServiceName=\"" + oldName, "ServiceName=\"" + newName)
                                .Replace("ToolboxFriendlyName=\"" + oldName, "ToolboxFriendlyName=\"" + newName));
                        }
                    }
                    //delete old resource
                    if(File.Exists(dependantResource.FilePath))
                    {
                        lock(GetFileLock(dependantResource.FilePath))
                        {
                            File.Delete(dependantResource.FilePath);
                        }
                    }
                    //update dependancies
                    var renameDependent = dependantResource.Dependencies.FirstOrDefault(dep => dep.ResourceName == oldName);
                    if(renameDependent != null)
                    {
                        renameDependent.ResourceName = newName;
                    }
                    //re-create, resign and save to file system the new resource
                StringBuilder result = resourceElement.ToStringBuilder();

                SaveImpl(workspaceID, dependantResource, result);
            }
        }

        public ResourceCatalogResult RenameCategory(Guid workspaceID, string oldCategory, string newCategory, string resourceTypeStr)
        {
            if(oldCategory == null)
            {
                throw new ArgumentNullException("oldCategory", @"No value provided for oldCategory");
            }
            if(string.IsNullOrEmpty(newCategory))
            {
                throw new ArgumentNullException("newCategory", @"No value provided for oldCategory");
            }
            ResourceType resourceType;
            Enum.TryParse(resourceTypeStr, out resourceType);
            var resourcesToUpdate = Instance.GetResources(workspaceID, resource => resource.ResourcePath.Equals(oldCategory, StringComparison.OrdinalIgnoreCase) && resource.ResourceType == resourceType);
            try
            {
                foreach(var resource in resourcesToUpdate)
                {
                    UpdateResourceCategory(workspaceID, resource, newCategory);
                }
                return new ResourceCatalogResult
                {
                    Status = ExecStatus.Success,
                    Message = string.Format("<CompilerMessage>{0} from '{1}' to '{2}'</CompilerMessage>", "Updated Category", oldCategory, newCategory)
                };
            }
            catch(Exception)
            {
                return new ResourceCatalogResult
                {
                    Status = ExecStatus.Fail,
                    Message = string.Format("<CompilerMessage>{0} from '{1}' to '{2}'</CompilerMessage>", "Failed to Category", oldCategory, newCategory)
                };
            }
        }

        void UpdateResourceCategory(Guid workspaceID, IResource resource, string newCategory)
        {
            
            StringBuilder resourceContents = GetResourceContents(workspaceID, resource.ResourceID);

            XElement resourceElement = resourceContents.ToXElement();
                XElement categoryElement = resourceElement.Element("Category");
                if(categoryElement == null)
                {
                    resourceElement.Add(new XElement("Category", newCategory));
                }
                else
                {
                    categoryElement.SetValue(newCategory);
                }

                lock(GetFileLock(resource.FilePath))
                {
                    // Big problems with properties like this. Cat rename needs to update cache
                    // AND we need to update the resource model ;)

                    StringBuilder contents = resourceElement.ToStringBuilder();

                    contents.WriteToFile(resource.FilePath, Encoding.UTF8);    
                
                }

                resource.ResourcePath = newCategory;
            
        }

        IEnumerable<IResource> GetResources(Guid workspaceID, Func<IResource, bool> filterResources)
        {
            return GetResources(workspaceID).Where(filterResources);
        }

        public List<ResourceForTree> GetDependentsAsResourceForTrees(Guid workspaceID, string resourceName)
        {
            // ReSharper disable LocalizableElement
            if(string.IsNullOrEmpty(resourceName)) throw new ArgumentNullException("resourceName", "No resource name given.");
            // ReSharper restore LocalizableElement

            var resources = GetResources(workspaceID);
            var dependants = new List<ResourceForTree>();
            resources.ForEach(resource =>
            {
                if(resource.Dependencies == null) return;
                resource.Dependencies.ForEach(tree =>
                {
                    if(tree.ResourceName == resourceName)
                    {
                        dependants.Add(CreateResourceForTree(resource, tree));
                    }
                });
            });
            return dependants.ToList();
        }

        static ResourceForTree CreateResourceForTree(IResource resource, ResourceForTree tree)
        {
            return new ResourceForTree
            {
                UniqueID = tree.UniqueID,
                ResourceID = resource.ResourceID,
                ResourceName = resource.ResourceName,
                ResourceType = resource.ResourceType
            };
        }
    }

    
}