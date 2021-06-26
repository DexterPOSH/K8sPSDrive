using System;
using System.Collections.Generic;
using Microsoft.PowerShell.SHiPS;
using k8s;
using k8s.Models;
using YamlDotNet.Serialization;

namespace K8sPSDrive.NameSpaces
{
    [SHiPSProvider(UseCache=false, BuiltinProgress=true)]
    public class PodLeaf: SHiPSLeaf
    {
        public string NameSpace { get; }
        public V1Pod K8sObject { get;}
        public string Phase
        { 
            get 
            { 
                return this.K8sObject.Status.Phase; 
            } 
        }
        public TimeSpan Age
        {
            get
            {
                return DateTime.Now - this.K8sObject.Status.StartTime.Value;
            }
        }
        public PodLeaf(string name, string ns, V1Pod pod): base(name)
        {
            _ = pod ?? throw new ArgumentNullException(nameof(pod));
            this.NameSpace = ns;
            this.K8sObject = Root._client.ReadNamespacedPod(pod.Metadata.Name, pod.Metadata.NamespaceProperty);
        }

        public string GetContent()
        {
            var boundParams = this.ProviderContext.BoundParameters;
            return Helper.ConvertToYaml(this.K8sObject);
        }
    }
}