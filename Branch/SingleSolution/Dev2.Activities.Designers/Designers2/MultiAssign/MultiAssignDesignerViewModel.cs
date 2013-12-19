using System.Activities.Presentation.Model;
using Dev2.Activities.Designers2.Core;
using Unlimited.Applications.BusinessDesignStudio.Activities;

namespace Dev2.Activities.Designers2.MultiAssign
{
    public class MultiAssignDesignerViewModel : ActivityCollectionDesignerViewModel<ActivityDTO>
    {
        public MultiAssignDesignerViewModel(ModelItem modelItem)
            : base(modelItem)
        {
            AddTitleBarLargeToggle();
            AddTitleBarQuickVariableInputToggle();
            AddTitleBarHelpToggle();

            dynamic mi = ModelItem;
            InitializeItems(mi.FieldsCollection);
        }

        public override string CollectionName { get { return "FieldsCollection"; } }
    }
}