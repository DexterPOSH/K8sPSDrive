using System;
using System.Collections.Generic;
using Microsoft.PowerShell.SHiPS;
using k8s;
using k8s.Models;

namespace K8sPSDrive.NameSpaces
{
    [SHiPSProvider(UseCache=false)]
    class PodDirectory : SHiPSDirectory
    {
        public PodDirectory(string name, string ns) : base(name)
        {
            this.NameSpace = ns;
        }

        public string NameSpace { get; }

        public override object[] GetChildItem()
        {
            var childItems = new List<PodLeaf>();

            // Read the pods in the namespace
            var podsList = Root._client.ListNamespacedPod(this.NameSpace);
            foreach (var pod in podsList.Items)
            {
                childItems.Add(
                    new PodLeaf(pod.Name(), this.NameSpace, pod)
                );
            }
            return childItems.ToArray();
        }
    }
}
