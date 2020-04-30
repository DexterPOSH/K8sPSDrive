using System;
using System.Collections.Generic;
using Microsoft.PowerShell.SHiPS;
using k8s;
using k8s.Models;
using YamlDotNet.Serialization;

namespace K8sPSDrive
{
    [SHiPSProvider(UseCache=true)]
    public class NameSpacePod: SHiPSLeaf
    {
        public V1Pod K8sObject { get; set; }
        public string Phase { get; set; }
        public TimeSpan? Age {get; }
        public NameSpacePod(string name, V1Pod pod): base(name)
        {
            _ = pod ?? throw new ArgumentNullException(nameof(pod));
            this.K8sObject = Root._client.ReadNamespacedPod(pod.Metadata.Name, pod.Metadata.NamespaceProperty);
            this.Phase = this.K8sObject.Status.Phase;
            this.Age = DateTime.Now - this.K8sObject.Status.StartTime;
        }

        public string GetContent()
        {
            var boundParams = this.ProviderContext.BoundParameters;
            return Helper.ConvertToYaml(this.K8sObject);
        }
    }
}