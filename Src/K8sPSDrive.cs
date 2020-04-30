using System;
using System.Collections.Generic;
using Microsoft.PowerShell.SHiPS;
using k8s;
using k8s.Models;

namespace K8sPSDrive
{
    [SHiPSProvider(UseCache=true)]
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
                new NameSpaces("namespaces"),
                new Nodes("nodes")
            }.ToArray();
        }
        
    }

}
