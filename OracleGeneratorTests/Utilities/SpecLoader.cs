using OracleGeneratorTests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace OracleGeneratorTests.Utilities
{
    public static class SpecLoader
    {
        public static ApiSpec LoadFromYaml(string path)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();

            var yaml = File.ReadAllText(path);
            var spec = deserializer.Deserialize<ApiSpec>(yaml);
            return spec;
        }
    }
}
