using System;
using System.Collections.Generic;
using Microsoft.PowerShell.SHiPS;
using k8s;

namespace K8sPSDrive
{
    public class Root: SHiPSDirectory
    {
        public static Kubernetes _client;
        public Root(string name): base(name)
        {
           
            try
            {
                var config = KubernetesClientConfiguration.BuildConfigFromConfigFile();

                _client = new Kubernetes(config);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            

        }

        public override object[] GetChildItem()
        {
            return new List<object>
            {
                new NameSpaces("namespaces")
            }.ToArray();
        }
    }

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
                    new NameSpace(ns.Metadata.Name)
                );
            }
            return childItems.ToArray();
        }
    }

    public class NameSpace : SHiPSLeaf
    {
        public NameSpace(string name): base(name)
        {
            
        }
    }

}
