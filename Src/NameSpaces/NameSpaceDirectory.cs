using System;
using System.Collections.Generic;
using Microsoft.PowerShell.SHiPS;
using k8s;
using k8s.Models;

namespace K8sPSDrive.NameSpaces
{
    [SHiPSProvider(UseCache=false)]
    public class NameSpaceDirectory : SHiPSDirectory
    {
        public  V1Namespace K8sObject {get; }
        public String Phase
        {
            get
            {
                return this.K8sObject.Status.Phase;
            }
        }

        public TimeSpan? Age
        {
            get
            {
                return DateTime.Now - this.K8sObject.Metadata.CreationTimestamp.Value;
            }
        }
        public NameSpaceDirectory(string name, V1Namespace ns): base(name)
        {
           _ = ns ?? throw new ArgumentNullException(nameof(ns));
           this.K8sObject = Root._client.ReadNamespace(ns.Metadata.Name);
        }

        public override object[] GetChildItem()
        {
            return new List<object>
            {
                new DeploymentDirectory("deployments", this.Name),
                new PodDirectory("pods", this.Name)
            }.ToArray();
        }
    }
}