﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dev2.Network.Execution
{
    public interface IExecutionStatusCallbackDispatcher
    {
        bool Add(Guid callbackID, Action<ExecutionStatusCallbackMessage> callback);
        bool Remove(Guid callbackID);
        void RemoveRange(IList<Guid> callbackIDs);

        void Post(ExecutionStatusCallbackMessage message);
        void Send(ExecutionStatusCallbackMessage message);
    }
}