﻿using System;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Dev2.DynamicServices;

namespace Dev2.Workspaces
{
    [Serializable]
    public partial class WorkspaceItem : IWorkspaceItem
    {
        public const string ServiceServiceType = "DynamicService";
        public const string SourceServiceType = "Source";

        #region Initialization

        public WorkspaceItem(Guid workspaceID, Guid serverID,Guid environmentID,Guid resourceID)
        {
            ID = resourceID;
            WorkspaceID = workspaceID;
            ServerID = serverID;
            EnvironmentID = environmentID;
        }

        public Guid EnvironmentID { get; private set; }

        #endregion

        #region ID

        /// <summary>
        /// The unique ID of the item.
        /// </summary>
        public Guid ID
        {
            get;
            private set;
        }

        #endregion

        #region WorkspaceID

        /// <summary>
        /// Gets or sets the workspace ID.
        /// </summary>
        public Guid WorkspaceID
        {
            get;
            private set;
        }

        #endregion

        #region ServerID

        /// <summary>
        /// Gets or sets the server ID.
        /// </summary>
        public Guid ServerID
        {
            get;
            private set;
        }

        #endregion

        #region Action

        /// <summary>
        /// Gets or sets the action to be taken on the item.
        /// </summary>
        public WorkspaceItemAction Action
        {
            get;
            set;
        }

        #endregion

        #region ServiceName

        /// <summary>
        /// Gets or sets the name of the service.
        /// </summary>
        public string ServiceName
        {
            get;
            set;
        }

        #endregion

        #region ServiceType

        /// <summary>
        /// Gets or sets the type of the service.
        /// <remarks>Must map to a <see cref="enDynamicServiceObjectType"/> value</remarks>
        /// </summary>
        public string ServiceType
        {
            get;
            set;
        }

        #endregion

        #region ISerializable

        protected WorkspaceItem(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            ID = (Guid)info.GetValue("ID", typeof(Guid));
            WorkspaceID = (Guid)info.GetValue("WorkspaceID", typeof(Guid));
            ServerID = (Guid)info.GetValue("ServerID", typeof(Guid));
            Action = (WorkspaceItemAction)info.GetValue("Action", typeof(WorkspaceItemAction));
            ServiceName = (string)info.GetValue("ServiceName", typeof(string));
            IsWorkflowSaved = (bool)info.GetValue("IsWorkflowSaved", typeof(bool));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            info.AddValue("ID", ID);
            info.AddValue("WorkspaceID", WorkspaceID);
            info.AddValue("ServerID", ServerID); 
            info.AddValue("Action", Action);
            info.AddValue("ServiceName", ServiceName);
            info.AddValue("IsWorkflowSaved", IsWorkflowSaved);
        }

        #endregion

        public bool IsWorkflowSaved
        {
            get;
            set;
        }

        #region IEquatable       

        public bool Equals(IWorkspaceItem other)
        {
            if (other == null)
            {
                return false;
            }
            return ID == other.ID && EnvironmentID == other.EnvironmentID;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var item = obj as IWorkspaceItem;
            return item != null && Equals(item);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        #endregion

        #region XML Serialization

        public WorkspaceItem(XElement xml)
        {
            ID = Guid.Parse(GetAttributeValue(xml, "ID"));
            Guid tryGetWorkspaceID;
            if (Guid.TryParse(GetAttributeValue(xml, "WorkspaceID"), out tryGetWorkspaceID))
            {
                WorkspaceID = tryGetWorkspaceID;
            }
            Guid tryGetServerID;
            if (Guid.TryParse(GetAttributeValue(xml, "ServerID"), out tryGetServerID))
            {
                ServerID = tryGetServerID;
            }
            Guid envID;
            if (Guid.TryParse(GetAttributeValue(xml, "EnvironmentID"), out envID))
            {
                EnvironmentID = envID;
            } 
            ServiceName = GetAttributeValue(xml, "ServiceName");
            bool isWorkflowSaved;
            string attributeValue = GetAttributeValue(xml, "IsWorkflowSaved");
            if(String.IsNullOrEmpty(attributeValue))
            {
                isWorkflowSaved = true;
            }
            else
            {
                bool.TryParse(attributeValue, out isWorkflowSaved);
            }
            IsWorkflowSaved = isWorkflowSaved;
            ServiceType = GetAttributeValue(xml, "ServiceType");

            WorkspaceItemAction action;
            Action = Enum.TryParse(GetAttributeValue(xml, "Action"), true, out action) ? action : WorkspaceItemAction.None;
        }

        /// <summary>
        /// Gets the XML representation of this instance.
        /// </summary>
        public virtual XElement ToXml()
        {
            var result = new XElement("WorkspaceItem",
                new XAttribute("ID", ID),
                new XAttribute("WorkspaceID", WorkspaceID),
                new XAttribute("ServerID", ServerID),
                new XAttribute("EnvironmentID", EnvironmentID),
                new XAttribute("Action", Action),
                new XAttribute("ServiceName", ServiceName ?? string.Empty),
                new XAttribute("IsWorkflowSaved", IsWorkflowSaved),
                new XAttribute("ServiceType", ServiceType ?? string.Empty)
                );
            return result;
        }

        static string GetAttributeValue(XElement x, string name)
        {
            var attr = x.Attribute(name);
            return attr == null ? string.Empty : attr.Value;
        }

        #endregion
    }
}