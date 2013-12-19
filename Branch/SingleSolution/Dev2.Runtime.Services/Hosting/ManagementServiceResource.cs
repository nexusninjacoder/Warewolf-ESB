﻿using System;
using Dev2.Data.ServiceModel;
using Dev2.DynamicServices;
using Dev2.Runtime.ServiceModel.Data;

namespace Dev2.Runtime.Hosting
{
    public class ManagementServiceResource : Resource
    {
        public DynamicService Service { get; private set; }

        public ManagementServiceResource(DynamicService service)
        {
            if(service == null)
            {
                throw new ArgumentNullException("service");
            }
            Service = service;
            ResourceID = service.ID == Guid.Empty ? Guid.NewGuid() : service.ID;
            ResourceName = service.Name;
            ResourceType = ResourceType.ReservedService;
            ResourcePath = "Management Services";
            DataList = service.DataListSpecification;
        }
    }
}