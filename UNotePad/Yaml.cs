using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;

namespace UNotePad
{
    public static class Yaml
    {
        public static ISerializer DefaultSerializer = new SerializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
            .DisableAliases()
            .WithQuotingNecessaryStrings()
            .Build();

        public static IDeserializer DefaultDeserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();
    }
}
