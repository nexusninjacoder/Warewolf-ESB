﻿using System;
using Dev2.DataList.Contract;
using Dev2.Services.Execution;
using Unlimited.Applications.BusinessDesignStudio.Activities;

namespace Dev2.Activities
{
    public class DsfPluginActivity : DsfActivity
    {
        ErrorResultTO _errorsTo;

        #region Overrides of DsfActivity

        protected override Guid ExecutionImpl(IEsbChannel esbChannel, IDSFDataObject dataObject, string inputs, string outputs, out ErrorResultTO tmpErrors)
        {
            _errorsTo = new ErrorResultTO();
            var compiler = DataListFactory.CreateDataListCompiler();

            ErrorResultTO invokeErrors;
            esbChannel.CorrectDataList(dataObject, dataObject.WorkspaceID, out invokeErrors, compiler);
            _errorsTo.MergeErrors(invokeErrors);
            var pluginServiceExecution = GetNewPluginServiceExecution(dataObject);
            tmpErrors = new ErrorResultTO();
            tmpErrors.MergeErrors(_errorsTo);
            var result = ExecutePluginService(pluginServiceExecution);
            tmpErrors.MergeErrors(_errorsTo);
            return result;
        }

        #endregion

        #region Protected Helper Functions

        protected virtual Guid ExecutePluginService(PluginServiceExecution container)
        {
            return container.Execute(out _errorsTo);
        }

        protected virtual PluginServiceExecution GetNewPluginServiceExecution(IDSFDataObject context)
        {
            return new PluginServiceExecution(context, false);
        }

        #endregion
    }
}