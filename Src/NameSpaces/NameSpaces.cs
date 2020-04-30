using System;
using System.Collections.Generic;
using Microsoft.PowerShell.SHiPS;
using k8s;
using k8s.Models;

namespace K8sPSDrive
{
    [SHiPSProvider(UseCache=true)]
    public class NameSpaces: SHiPSDirectory
    {
        public NameSpaces(string name): base(name)
        {
        }

        public override object[] GetChildItem()
        {
            var namespaces = Root._client.ListNamespace();
            var childItems = new List<object>();
            foreach (var ns in namespaces.Items)
            {
                childItems.Add(
                    new NameSpace(ns.Metadata.Name, ns)
                );
            }
            return childItems.ToArray();
        }
    }
}
    