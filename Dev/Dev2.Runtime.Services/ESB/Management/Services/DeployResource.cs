﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Dev2.Common.ExtMethods;
using Dev2.DynamicServices;
using Dev2.Runtime.Hosting;
using Dev2.Workspaces;

namespace Dev2.Runtime.ESB.Management.Services
{
    /// <summary>
    /// Deploy a resource
    /// </summary>
    public class DeployResource : IEsbManagementEndpoint
    {
        public string Execute(IDictionary<string, string> values, IWorkspace theWorkspace)
        {

            string resourceDefinition;
            string roles;

            values.TryGetValue("ResourceDefinition", out resourceDefinition);
            values.TryGetValue("Roles", out roles);

            if(string.IsNullOrEmpty(roles) || string.IsNullOrEmpty(resourceDefinition))
            {
                throw new InvalidDataContractException("Roles or ResourceDefinition missing");
            }
            resourceDefinition = resourceDefinition.Unescape();
            var result = ResourceCatalog.Instance.SaveResource(WorkspaceRepository.ServerWorkspaceID, resourceDefinition, roles);
            WorkspaceRepository.Instance.RefreshWorkspaces();
            Guid resourceID;
            var xml = XElement.Parse(resourceDefinition);
            if(Guid.TryParse(xml.AttributeSafe("ID"), out resourceID))
            {
                ResourceCatalog.Instance.FireUpdateMessage(resourceID);
            }
            return result.ToString();
        }

        public DynamicService CreateServiceEntry()
        {
            DynamicService deployResourceDynamicService = new DynamicService();
            deployResourceDynamicService.Name = HandlesType();
            deployResourceDynamicService.DataListSpecification = "<DataList><ResourceDefinition/><Roles/><Dev2System.ManagmentServicePayload ColumnIODirection=\"Both\"></Dev2System.ManagmentServicePayload></DataList>";

            ServiceAction deployResourceServiceAction = new ServiceAction();
            deployResourceServiceAction.Name = HandlesType();
            deployResourceServiceAction.ActionType = enActionType.InvokeManagementDynamicService;
            deployResourceServiceAction.SourceMethod = HandlesType();

            deployResourceDynamicService.Actions.Add(deployResourceServiceAction);

            return deployResourceDynamicService;
        }

        public string HandlesType()
        {
            return "DeployResourceService";
        }
    }
}
