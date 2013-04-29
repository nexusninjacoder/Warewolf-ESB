﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Dev2.DynamicServices;
using Dev2.Runtime.Hosting;
using Dev2.Workspaces;

namespace Dev2.Runtime.ESB.Management.Services
{
    /// <summary>
    /// Find dependencies for a service
    /// </summary>
    public class FindDependencies : IEsbManagementEndpoint
    {
        public string Execute(IDictionary<string, string> values, IWorkspace theWorkspace)
        {
            string resourceName;
            values.TryGetValue("ResourceName", out resourceName);

            if(string.IsNullOrEmpty(resourceName))
            {
                throw new InvalidDataContractException("ResourceName is empty or null");
            }

            // BUG 7850 - TWR - 2013.03.11 - ResourceCatalog refactor
            var result = string.Format("<graph title=\"Dependency Graph Of {0}\">", resourceName) + FindDependenciesRecursive(resourceName, theWorkspace.ID) + "</graph>";
            return result;
        }

        public string HandlesType()
        {
            return "FindDependencyService";
        }

        public DynamicService CreateServiceEntry()
        {
            var ds = new DynamicService
            {
                Name = HandlesType(),
                DataListSpecification = @"<DataList><ResourceName/><Dev2System.ManagmentServicePayload ColumnIODirection=""Both""></Dev2System.ManagmentServicePayload></DataList>"
            };

            var sa = new ServiceAction
            {
                Name = HandlesType(),
                ActionType = enActionType.InvokeManagementDynamicService,
                SourceMethod = HandlesType()
            };

            ds.Actions.Add(sa);

            return ds;

        }

        #region Private Methods

        // BUG 7850 - TWR - 2013.03.11 - ResourceCatalog refactor
        private string FindDependenciesRecursive(string resourceName, Guid workspaceID)
        {
            var resource = ResourceCatalog.Instance.GetResource(workspaceID, resourceName);
            var sb = new StringBuilder();
            var dependencies = resource.Dependencies;
            if(dependencies != null)
            {
                sb.Append(string.Format("<node id=\"{0}\" x=\"\" y=\"\" broken=\"false\">", resourceName));
                dependencies.ForEach(c => sb.Append(string.Format("<dependency id=\"{0}\" />", c.ResourceName)));
                sb.Append("</node>");
            }
            if(dependencies != null)
            {
                dependencies.ToList().ForEach(c => sb.Append(FindDependenciesRecursive(c.ResourceName, workspaceID)));
            }
            sb.Append(string.Format("<node id=\"{0}\" x=\"\" y=\"\" broken=\"false\">", resourceName));
            sb.Append("</node>");
            var findDependenciesRecursive = sb.ToString();
            return findDependenciesRecursive;
        }


        #endregion



    }
}
