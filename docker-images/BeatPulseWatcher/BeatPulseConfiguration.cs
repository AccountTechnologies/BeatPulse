using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeatPulseWatcher
{
    public class BeatPulseConfiguration
    {
        public SqlServersConfiguration SqlServers { get; set; }
        public UrlGroupsConfiguration UrlGroups { get; set; }

        public BeatPulseEndPointsConfiguration BeatPulseEndPoints { get; set; }
    }

    public class BeatPulseEndPointsConfiguration
    {
        public BeatPulseEndPointConfiguration[] Entries { get; set; }
    }

    public class BeatPulseEndPointConfiguration
    {
        public string Name { get; set; }
        public string Path { get; set; } = "beatpulse";
        public string Uri { get; set; }
    }

    public class UrlGroupsConfiguration
    {
        public UrlGroupConfiguration[] Entries { get; set; }
    }

    public class UrlGroupConfiguration
    {
        public string Name { get; set; }
        public string Path { get; set; } = "uri-group";
        public string[] Entries { get; set; }
    }

    public class SqlServerConfiguration
    {
        public string Name { get; set; }
        public string Connection { get; set; }
        public string Query { get; set; } = "SELECT 1;";
        public string Path { get; set; } = "sqlserver";

    }

    public class SqlServersConfiguration
    {
        public SqlServerConfiguration[] Entries { get; set; }
    }
}
