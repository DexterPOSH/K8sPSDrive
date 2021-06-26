using System;
using System.Collections.Generic;
using Microsoft.PowerShell.SHiPS;
using k8s;
using k8s.Models;
using YamlDotNet.Serialization;

namespace K8sPSDrive.NameSpaces
{
    [SHiPSProvider(UseCache = false)]
    public class DeploymentLeaf : SHiPSLeaf
    {
        public V1Deployment K8sObject { get; }
        public string NameSpace { get; }
        
        public int? Ready
        {
            get
            {
                return this.K8sObject.Status.ReadyReplicas;
            }
        }

        public int? Available
        {
            get
            {
                return this.K8sObject.Status.AvailableReplicas;
            }
        }
        public int? Updated 
        {

            get
            {
                return this.K8sObject.Status.UpdatedReplicas;
            }
        }

        public TimeSpan? Age
        {
            get
            {
                return DateTime.Now - this.K8sObject.Metadata.CreationTimestamp.Value;
            }
        }

        public DeploymentLeaf(string name, string ns,  V1Deployment deployment) : base(name)
        {
            _ = deployment ?? throw new ArgumentNullException(nameof(deployment));
            this.NameSpace = ns;
            this.K8sObject = Root._client.ReadNamespacedDeployment(deployment.Metadata.Name, deployment.Metadata.NamespaceProperty);

        }

        
        public string GetContent()
        {
            var boundParams = this.ProviderContext.BoundParameters;
            return Helper.ConvertToYaml(this.K8sObject);
        }
    }
}