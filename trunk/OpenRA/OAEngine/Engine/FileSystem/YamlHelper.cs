using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Engine.FileSystem
{
    public static class YamlHelper
    {
        public static T Deserialize<T>(string filePath) where T : new()
        {
            if (!File.Exists(filePath))
            {
                return default(T);
            }
            T obj = default(T); 
            using (Stream stream = File.OpenRead(filePath))
            {
                var input = File.ReadAllText(filePath, Encoding.UTF8);
                
                var deserializer = new DeserializerBuilder().WithNamingConvention(new NullNamingConvention())
                    .Build();

                obj = deserializer.Deserialize<T>(input);
            }
            return obj;
        }
    }
}
