namespace Int.Database.Domain.OpenTelemetries
{
    public class OpenTelemetryOptions
    {
        public bool Enabled { get; set; }
        public string ServiceName { get; set; }
        public string ServiceVersion { get; set; }
        public OtlpExporterOptions OtlpExporter { get; set; }
    }

    public class OtlpExporterOptions
    {
        public bool Enabled { get; set; }
        public string Endpoint { get; set; }
        public string Protocol { get; set; }
        public Dictionary<string, string> Headers { get; set; }
    }

}
