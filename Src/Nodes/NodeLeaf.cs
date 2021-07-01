using System;
using System.Collections.Generic;
using Microsoft.PowerShell.SHiPS;
using k8s;
using k8s.Models;

namespace K8sPSDrive.Nodes
{
    [SHiPSProvider(UseCache=true)]
    public class NodeLeaf: SHiPSLeaf
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

        public TimeSpan Age
        { 
            get
            {
                return DateTime.Now - this.K8sObject.Metadata.CreationTimestamp.Value;
            }
        }
        public Version Version 
        {
            get
            {
                Version version = null;
                Version.TryParse(this.K8sObject.Status.NodeInfo.KubeletVersion, out version);
                return version;
            }
        }

        public String Os 
        { 
            get 
            {
                return this.K8sObject.Status.NodeInfo.OperatingSystem;
            }
        }

        public NodeLeaf(string name, V1Node node): base(name)
        {
           _ = node ?? throw new ArgumentNullException(nameof(node));
           this.K8sObject = Root._client.ReadNode(node.Metadata.Name);
        }

        public string GetContent()
        {
            var boundParams = this.ProviderContext.BoundParameters;
            return Helper.ConvertToYaml(this.K8sObject);
        }
    }
}