using System;
using System.Collections.Generic;
using Microsoft.PowerShell.SHiPS;
using k8s;
using k8s.Models;


namespace K8sPSDrive.NameSpaces
{
    [SHiPSProvider(UseCache=false)]
    class DeploymentDirectory: SHiPSDirectory
    {
        public DeploymentDirectory(string name, string ns) : base(name)
        {
            this.NameSpace = ns;
        }

        public string NameSpace { get; }

        public override object[] GetChildItem()
        {
            var childItems = new List<DeploymentLeaf>();

            // Read the deployments in the namespace
            var deploymentsList = Root._client.ListNamespacedDeployment(this.NameSpace);
            foreach (var deployment in deploymentsList.Items)
            {
                childItems.Add(
                    new DeploymentLeaf(deployment.Name(), this.NameSpace, deployment)
                );
            }
            return childItems.ToArray();
        }
    }
}
