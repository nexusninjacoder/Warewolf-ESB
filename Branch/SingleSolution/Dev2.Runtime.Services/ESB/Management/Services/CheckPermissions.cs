﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using Dev2.DynamicServices;
using Dev2.DynamicServices.Objects;
using Dev2.Workspaces;

namespace Dev2.Runtime.ESB.Management.Services
{
    /// <summary>
    /// Checks a users permissions on the local file system
    /// </summary>
    public class CheckPermissions : IEsbManagementEndpoint
    {
        public StringBuilder Execute(Dictionary<string, StringBuilder> values, IWorkspace theWorkspace)
        {
            var result = new StringBuilder();

            string username;
            string path;
            string password;

            StringBuilder tmp;
            values.TryGetValue("Username", out tmp);
            username = tmp.ToString();
            values.TryGetValue("Password", out tmp);
            password = tmp.ToString();
            values.TryGetValue("FilePath", out tmp);
            path = tmp.ToString();

            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(path))
            {
                throw new InvalidDataContractException("FilePath or Username or Password is missing");
            }

            if(username == string.Empty)
            {
                if(!FileIO.CheckPermissions(WindowsIdentity.GetCurrent(), path, FileSystemRights.Read) &&
                   !FileIO.CheckPermissions(WindowsIdentity.GetCurrent(), path, FileSystemRights.ReadData))
                {
                    result.Append(string.Concat("Insufficient permission for '", path, "'."));
                }
            }
            else
            {
                // we have a username and password set :)

                if(!FileIO.CheckPermissions(username, password, path, FileSystemRights.Read) &&
                   FileIO.CheckPermissions(username, password, path, FileSystemRights.ReadData))
                {
                    result.Append("<Errors>");
                    result.Append(string.Concat("Insufficient permission for '", path, "'."));
                    result.Append("</Error>");
                }
            }

            return result;
        }

        public DynamicService CreateServiceEntry()
        {
            var checkPermissionsService = new DynamicService
            {
                Name = HandlesType(),
                DataListSpecification = "<DataList><FilePath/><Username/><Password/><Dev2System.ManagmentServicePayload ColumnIODirection=\"Both\"></Dev2System.ManagmentServicePayload></DataList>"
            };

            var checkPermissionsServiceAction = new ServiceAction
            {
                Name = HandlesType(),
                ActionType = enActionType.InvokeManagementDynamicService,
                SourceMethod = HandlesType()
            };

            checkPermissionsService.Actions.Add(checkPermissionsServiceAction);

            return checkPermissionsService;
        }

        public string HandlesType()
        {
            return "CheckPermissionsService";
        }
    }
}