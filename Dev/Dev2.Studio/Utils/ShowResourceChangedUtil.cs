using System.Collections.Generic;
using Caliburn.Micro;
using Dev2.Providers.Logs;
using Dev2.Studio.Core.Interfaces;
using Dev2.Studio.Core.Messages;
using Dev2.Studio.Views.ResourceManagement;

namespace Dev2.Utils
{
    public class ShowResourceChangedUtil
    {
        readonly IEventAggregator _eventPublisher;
        public ShowResourceChangedUtil(IEventAggregator eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public void ShowResourceChanged(IContextualResourceModel resource, IList<string> numberOfDependants)
        {
            var dialog = new ResourceChangedDialog(resource, numberOfDependants.Count, StringResources.MappingChangedWarningDialogTitle);
            dialog.ShowDialog();
            if(dialog.OpenDependencyGraph)
            {
                if(numberOfDependants.Count == 1)
                {

                }
                else
                {
                    Logger.TraceInfo("Publish message of type - " + typeof(ShowReverseDependencyVisualizer));
                    _eventPublisher.Publish(new ShowReverseDependencyVisualizer(resource));
                }
            }
        }
    }
}