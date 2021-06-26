using System;
using System.Collections.Generic;
using Microsoft.PowerShell.SHiPS;
using k8s;
using k8s.Models;

namespace K8sPSDrive.Nodes
{
    [SHiPSProvider(UseCache=true)]
    public class Nodes: SHiPSDirectory
    {
        public Nodes(string name): base(name)
        {
        }

        public override object[] GetChildItem()
        {
            var nodes = Root._client.ListNode() ;
            var childItems = new List<object>();
            foreach (var node in nodes.Items)
            {
                childItems.Add(
                    new NodeLeaf(node.Metadata.Name, node)
                );
            }
            return childItems.ToArray();
        }
    }
}
    