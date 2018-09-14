using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using AzureB2C.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.PowerBI.Api.V2;
using Microsoft.PowerBI.Api.V2.Models;
using Microsoft.Rest;

namespace AzureB2C.Utils
{
    public static class EmbedPowerBiReport
    {
        public static PowerBiEmbedConfig EmbedPbiReport(PowerBiReport reportProperties, string roles, string userName, bool rlsApplied = false)
        {
            var error = GetWebConfigErrors(reportProperties);

            if (error != null)
            {
                return new PowerBiEmbedConfig()
                {
                    ErrorMessage = error
                };
            }

            // Create a user password cradentials this will be power BI owner license credential.
            var credential = new UserPasswordCredential(reportProperties.LoginUsername, reportProperties.LoginPassword);

            // Authenticate using created credentials
            var authenticationContext = new AuthenticationContext(reportProperties.AuthorityUrl);

            var authenticationResult = authenticationContext.AcquireTokenAsync(reportProperties.ResourceUrl, reportProperties.ApplicationId, credential);

            if (authenticationResult == null)
            {
                return new PowerBiEmbedConfig()
                {
                    ErrorMessage = "Authentication Failed."
                };
            }

            var tokenCredentials = new TokenCredentials(authenticationResult.Result.AccessToken, "Bearer");

            // Create a Power BI Client object. It will be used to call Power BI APIs.
            var generateTokenRequestParameters = new GenerateTokenRequest();
            using (var client = new PowerBIClient(new Uri(reportProperties.ApiUrl), tokenCredentials))
            {
                // Get a list of reports.
                var reports = client.Reports.GetReportsInGroupAsync(reportProperties.WorkspaceId);

                var report = reports.Result.Value.FirstOrDefault(x => x.Id == reportProperties.ReportId);

                if (report == null)
                {
                    return new PowerBiEmbedConfig()
                    {
                        ErrorMessage = "WOorkspace has no reports."
                    };
                }

                EmbedToken tokenResponse;
                if (rlsApplied)
                {
                    if (userName == null && roles == null)
                    {
                        return new PowerBiEmbedConfig()
                        {
                            ErrorMessage = "User name or user role is null."
                        };
                    }

                    generateTokenRequestParameters = new GenerateTokenRequest("View", null, identities: new List<EffectiveIdentity>
                    {
                        new EffectiveIdentity(
                            userName,
                            roles: new List<string> {roles},
                            datasets: new List<string> { report.DatasetId }
                        )
                    });

                    tokenResponse = client.Reports.GenerateTokenInGroupAsync(reportProperties.WorkspaceId, report.Id, generateTokenRequestParameters).Result;

                }
                else
                {
                    tokenResponse = client.Reports.GenerateTokenInGroupAsync(reportProperties.WorkspaceId, report.Id, generateTokenRequestParameters).Result;
                }

                if (tokenResponse == null)
                {
                    return new PowerBiEmbedConfig()
                    {
                        ErrorMessage = "Failed to generate embed token."
                    };
                }

                // Generate Embed Configuration.
                var embedConfig = new PowerBiEmbedConfig()
                {
                    EmbedToken = tokenResponse,
                    EmbedUrl = report.EmbedUrl,
                    Id = report.Id
                };

                return embedConfig;
            }
        }

        public static PowerBiReport GetPowerBiReportProperties(string reportName)
        {
            if (string.IsNullOrEmpty(reportName))
                return null;
            return new PowerBiReport()
            {
                LoginUsername = ConfigurationManager.AppSettings["pbiUsername"],
                LoginPassword = ConfigurationManager.AppSettings["pbiPassword"],
                AuthorityUrl = ConfigurationManager.AppSettings["authorityUrl"],
                ResourceUrl = ConfigurationManager.AppSettings["resourceUrl"],
                ClientId = ConfigurationManager.AppSettings["clientId"],
                ApiUrl = ConfigurationManager.AppSettings["apiUrl"],
                GroupId = ConfigurationManager.AppSettings["groupId"],
                ReportId = ConfigurationManager.AppSettings[reportName],
                DatasetId = ConfigurationManager.AppSettings["datasetId"],
                ApplicationId = ConfigurationManager.AppSettings["applicationId"],
                WorkspaceId = ConfigurationManager.AppSettings["workspaceId"]
            };
        }

        /// <summary>
        /// Check if web.config embed parameters have valid values.
        /// </summary>
        /// <returns>Null if web.config parameters are valid, otherwise returns specific error string.</returns>
        private static string GetWebConfigErrors(PowerBiReport reportProperties)
        {
            Guid result;

            // Group Id must have a value.
            if (string.IsNullOrEmpty(reportProperties.WorkspaceId))
            {
                return "GroupId is empty. Please select a group you own and fill its Id in config";
            }

            // Group Id must be a Guid object.
            if (!Guid.TryParse(reportProperties.WorkspaceId, out result))
            {
                return "GroupId must be a Guid object. Please select a group you own and fill its Id in config";
            }

            // Group Id must have a value.
            if (string.IsNullOrEmpty(reportProperties.ApplicationId))
            {
                return "GroupId is empty. Please select a group you own and fill its Id in config";
            }

            // Group Id must be a Guid object.
            if (!Guid.TryParse(reportProperties.ApplicationId, out result))
            {
                return "Application Id must be a Guid object. Please select a Application you own and fill its Id in config";
            }

            // Username must have a value.
            if (string.IsNullOrEmpty(reportProperties.LoginUsername))
            {
                return "Username is empty. Please fill Power BI userName in config";
            }

            // Password must have a value.
            if (string.IsNullOrEmpty(reportProperties.LoginPassword))
            {
                return "Password is empty. Please fill password of Power BI userName in config";
            }

            return null;
        }

    }
}