namespace AzureB2C.Models
{
    public class PowerBiReport
    {
        public string LoginUsername { get; set; }
        public string LoginPassword { get; set; }
        public string AuthorityUrl { get; set; }
        public string ResourceUrl { get; set; }
        public string ClientId { get; set; }
        public string ApiUrl { get; set; }
        public string GroupId { get; set; }
        public string ReportId { get; set; }
        public string DatasetId { get; set; }

        public string ApplicationId { get; set; }
        public string WorkspaceId { get; set; }

    }
}