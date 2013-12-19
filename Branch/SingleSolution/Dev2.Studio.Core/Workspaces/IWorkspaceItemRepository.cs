﻿using Dev2.Communication;
using Dev2.Studio.Core.Interfaces;
using Dev2.Workspaces;
using System.Collections.Generic;

namespace Dev2.Studio.Core.Workspaces
{
    public interface IWorkspaceItemRepository
    {
        IList<IWorkspaceItem> WorkspaceItems { get; }
        void Write();
        void AddWorkspaceItem(IContextualResourceModel model);
        ExecuteMessage UpdateWorkspaceItem(IContextualResourceModel resource, bool isLocalSave);
        void Remove(IContextualResourceModel resourceModel);
        void UpdateWorkspaceItemIsWorkflowSaved(IContextualResourceModel resourceModel);
    }
}