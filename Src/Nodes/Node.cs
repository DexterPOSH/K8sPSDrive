using System;
using System.Collections.Generic;
using Microsoft.PowerShell.SHiPS;
using k8s;
using k8s.Models;

namespace K8sPSDrive
{

    [SHiPSProvider(UseCache=true)]
    public class Node: SHiPSLeaf
    {
        public  V1Node K8sObject {get; }
        public String Status
        {
            get
            {
                return this.K8sObject.Status.Phase;
            }
        }
        public string Roles 
        {
            get
            {
                string roles = "";
                this.K8sObject.Metadata.Labels.TryGetValue("kubernetes.io/role", out roles);
                return roles;
            } 
        }
        public TimeSpan Age {get; }
        public Version Version 
        {
            get
            {
                Version version = null;
                Version.TryParse(this.K8sObject.Status.NodeInfo.KubeletVersion, out version);
                return version;
            }
        }
        public String Os {get; }
        public Node(string name, V1Node node): base(name)
        {
           _ = node ?? throw new ArgumentNullException(nameof(node));
           this.K8sObject = Root._client.ReadNode(node.Metadata.Name);
           //this.Status =  this.K8sObject.Status.Phase;
           this.Age = DateTime.Now - this.K8sObject.Metadata.CreationTimestamp.Value;
           //this.Roles = this.K8sObject.Metadata.Labels.;
           //Version.TryParse(this.K8sObject.Status.NodeInfo.KubeletVersion, out this.Version);
           this.Os = this.K8sObject.Status.NodeInfo.OperatingSystem;
        }

        public string GetContent()
        {
            var boundParams = this.ProviderContext.BoundParameters;
            return Helper.ConvertToYaml(this.K8sObject);
        }
    }
}