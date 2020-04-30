using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Serialization;

namespace K8sPSDrive
{
    public class Helper
    {
        public static string ConvertToYaml(object InputObject)
        {
            var serializer = new Serializer();
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(serializer.Serialize(InputObject));
            return stringBuilder.ToString();
        }
    }
}