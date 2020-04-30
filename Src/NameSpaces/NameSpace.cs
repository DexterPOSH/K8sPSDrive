using System;
using System.Collections.Generic;
using Microsoft.PowerShell.SHiPS;
using k8s;
using k8s.Models;

namespace K8sPSDrive
{

    [SHiPSProvider(UseCache=true)]
    public class NameSpace : SHiPSDirectory
    {
        public  V1Namespace K8sObject {get; }
        public String Phase {get; }

        public TimeSpan Age {get; }
        public NameSpace(string name, V1Namespace ns): base(name)
        {
           _ = ns ?? throw new ArgumentNullException(nameof(ns));
           this.K8sObject = Root._client.ReadNamespace(ns.Metadata.Name);
           this.Phase =  this.K8sObject.Status.Phase;
           this.Age = DateTime.Now - this.K8sObject.Metadata.CreationTimestamp.Value;
        }

        public override object[] GetChildItem()
        {
            var podsList = Root._client.ListNamespacedPod(base.Name);
            var childItems = new List<object>();
            foreach ( var pod in podsList.Items)
            {
                childItems.Add(
                    new NameSpacePod(pod.Metadata.Name, pod)
                );
            }
            return childItems.ToArray();
        }
    }
}