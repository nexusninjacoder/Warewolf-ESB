﻿using System;
using Dev2.Studio.Core.Interfaces;

namespace Dev2.Studio.Feedback
{
    public interface IAsyncFeedbackAction : IFeedbackAction
    {
        void StartFeedback(Action<Exception> onCompleted);
        void FinishFeedBack(IEnvironmentModel environmentModel = null);
        void CancelFeedback();
    }
}