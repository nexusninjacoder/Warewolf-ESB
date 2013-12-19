﻿using System;
using System.Collections.Generic;

namespace Dev2.Diagnostics
{
    public class DummyDebugProvider : IDebugProvider
    {
        private Guid _serverID = Guid.NewGuid();
        private Guid _workflow1ID = Guid.Parse("D72A6E61-EC79-43D4-99EC-9E26DB9A0A4B");
        private Guid _workflow2ID = Guid.NewGuid();
        private Guid _assign1ID = Guid.NewGuid();
        private Guid _assign2ID = Guid.NewGuid();
        private Guid _assign3ID = Guid.NewGuid();
        private DateTime _startDate = DateTime.Now;

        public Guid ServerID
        {
            get { return _serverID; }
            set { _serverID = value; }
        }

        public Guid Workflow1ID
        {
            get { return _workflow1ID; }
            set { _workflow1ID = value; }
        }

        public Guid Workflow2ID
        {
            get { return _workflow2ID; }
            set { _workflow2ID = value; }
        }

        public Guid Assign1ID
        {
            get { return _assign1ID; }
            set { _assign1ID = value; }
        }

        public Guid Assign2ID
        {
            get { return _assign2ID; }
            set { _assign2ID = value; }
        }

        public Guid Assign3ID
        {
            get { return _assign3ID; }
            set { _assign3ID = value; }
        }

        public IEnumerable<DebugState> GetDebugStates(string serverWebUri, DirectoryPath directory, FilePath path)
        {
            var list = new List<DebugState>();
            list.Add(new DebugState
                {
                    StateType = StateType.Before,
                    ServerID = ServerID,
                    ParentID = Guid.Empty,
                    ID = Workflow1ID,
                    DisplayName = "Workflow1",
                    HasError = false,
                    Name = "DsfActivity",
                    ActivityType = ActivityType.Workflow,
                    StartTime = _startDate,
                    EndTime = _startDate.AddMinutes(1)
                });
            list.Add(new DebugState
            {
                StateType = StateType.All,
                ServerID = ServerID,
                ParentID = Workflow1ID,
                ID = Assign1ID,
                DisplayName = "Assign1",
                HasError = false,
                Name = "Assign",
                ActivityType = ActivityType.Step,
                StartTime = _startDate.AddMinutes(1),
                EndTime = _startDate.AddMinutes(2)
            });
            list.Add(new DebugState
            {
                StateType = StateType.Before,
                ServerID = ServerID,
                ParentID = Workflow1ID,
                ID = Workflow2ID,
                DisplayName = "Workflow2",
                HasError = false,
                Name = "DsfActivity",
                ActivityType = ActivityType.Step,
                StartTime = _startDate.AddMinutes(2),
                EndTime = _startDate.AddMinutes(3)
            });
            list.Add(new DebugState
            {
                StateType = StateType.All,
                ServerID = ServerID,
                ParentID = Workflow2ID,
                ID = Assign2ID,
                DisplayName = "Assign2",
                HasError = false,
                Name = "Assign",
                ActivityType = ActivityType.Step,
                StartTime = _startDate.AddMinutes(3),
                EndTime = _startDate.AddMinutes(4)
            });
            list.Add(new DebugState
            {
                StateType = StateType.After,
                ServerID = ServerID,
                ParentID = Workflow1ID,
                ID = Workflow2ID,
                DisplayName = "Workflow2",
                HasError = false,
                Name = "DsfActivity",
                ActivityType = ActivityType.Step,
                StartTime = _startDate.AddMinutes(4),
                EndTime = _startDate.AddMinutes(5)
            });
            list.Add(new DebugState
            {
                StateType = StateType.After,
                ServerID = ServerID,
                ParentID = Guid.Empty,
                ID = Workflow1ID,
                DisplayName = "Workflow1",
                HasError = false,
                Name = "DsfActivity",
                ActivityType = ActivityType.Workflow,
                StartTime = _startDate.AddMinutes(5),
                EndTime = _startDate.AddMinutes(6)
            });
            list.Add(new DebugState
            {
                StateType = StateType.All,
                ServerID = ServerID,
                ParentID = Guid.Empty,
                ID = Assign3ID,
                DisplayName = "Assign3",
                HasError = false,
                Name = "Assign",
                ActivityType = ActivityType.Step,
                StartTime = _startDate.AddMinutes(6),
                EndTime = _startDate.AddMinutes(7)
            });
            return list;
        }

        public DebugState GetDebugState()
        {
            return new DebugState
                {
                    StateType = StateType.Before,
                    ServerID = ServerID,
                    ParentID = Guid.Empty,
                    ID = Workflow1ID,
                    OriginatingResourceID = Workflow1ID,
                    DisplayName = "TestWorkflow",
                    HasError = false,
                    Name = "DsfActivity",
                    ActivityType = ActivityType.Workflow,
                    StartTime = _startDate,
                    EndTime = _startDate.AddMinutes(1)
                };
        }
    }

}