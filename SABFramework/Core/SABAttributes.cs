using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SABFramework.Core.DataCore;

namespace SABFramework.Core
{
    public enum DependencyType { Required, Optional };

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class Dependency : Attribute
    {
        public DependencyType DependencyType { get; set; }
        public RequestType RequestType { get; set; }

        public Dependency(DependencyType dependencyType, RequestType dependencyRequestType)
        {
            DependencyType = dependencyType;
            RequestType = dependencyRequestType;
        }
    }


}