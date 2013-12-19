﻿using Dev2.DataList.Contract;
using Dev2.Runtime.Hosting;
using Dev2.Runtime.ServiceModel.Data;

namespace Dev2.Services.Execution
{
    public class MockServiceExecutionAbstract<TService, TSource> : ServiceExecutionAbstract<TService, TSource>
        where TService : Service, new() where TSource : Resource, new()
    {

        public bool DidExecuteServiceInvoke { get; private set; }

        public MockServiceExecutionAbstract(IDSFDataObject dataObj, bool handlesOutputFormatting = false)
            : base(dataObj, handlesOutputFormatting, false)
        {
        }

        #region Overrides of ServiceExecutionAbstract<TService,TSource>

        public override void BeforeExecution(ErrorResultTO errors)
        {
        }

        public override void AfterExecution(ErrorResultTO errors)
        {
        }

        protected override object ExecuteService(out ErrorResultTO errors)
        {
            errors = new ErrorResultTO();
            DidExecuteServiceInvoke = true;
            return null;
        }

        #endregion

        #region Exposed Functions

        public void MockCreateService(ResourceCatalog catalog)
        {
            CreateService(catalog);
        }

        public void MockExecuteImpl(IDataListCompiler compiler, out DataList.Contract.ErrorResultTO errors)
        {
            ExecuteImpl(compiler, out errors);
        }
        
        #endregion
    }
}