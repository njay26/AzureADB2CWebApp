using System;
using Microsoft.PowerBI.Api.V2.Models;

namespace AzureB2C.Models
{
    public class PowerBiEmbedConfig
    {
        public string Id { get; set; }
        public string EmbedUrl { get; set; }
        public EmbedToken EmbedToken { get; set; }
        public int MinutesToExpiration
        {
            get
            {
                var minutesToExpiration = EmbedToken.Expiration.Value - DateTime.UtcNow;
                return minutesToExpiration.Minutes;
            }
        }
        public string ErrorMessage { get; internal set; }
    }
}