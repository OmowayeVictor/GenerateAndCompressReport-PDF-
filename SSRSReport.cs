using Azure;
using DotNetEnv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GenerateAndCompressPDF
{
    public class SSRSReport
    {
        private readonly string? serverURL;
        private readonly string? environment;
        private readonly HttpClient? httpClient;

        public SSRSReport()
        {
            Env.TraversePath().Load();

            this.environment = Environment.GetEnvironmentVariable("Environment");
            this.serverURL = environment == "Live"
                ? Environment.GetEnvironmentVariable("SSRSLiveRequestURL")
                : Environment.GetEnvironmentVariable("SSRSDevRequestURL");

            if (string.IsNullOrEmpty(serverURL))
            {
                throw new InvalidOperationException("The request URL cannot be null or empty.");
            }

            var username = Environment.GetEnvironmentVariable("SSRSUsername");

            var password = environment == "Live"
                ? Environment.GetEnvironmentVariable("SSRSLivePassword")
                : Environment.GetEnvironmentVariable("SSRSDevPassword");

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new InvalidOperationException("The username or password cannot be null or empty.");
            }

            var handler = new HttpClientHandler
            {
                Credentials = new NetworkCredential(username, password)
            };

            this.httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(serverURL)
            };
        }

        public async Task<byte[]> FetchBufferAsync(string? isMexico, string customerId, string period, string reportType)
        {
            int maxRetries = 3;
            int delayBetweenRetries = 2000;
            int retryCount = 0;

            while (true)
            {
                try
                {
                    var requestUrl = GetRequestUrl(reportType, isMexico, customerId, period);
                    var response = await httpClient!.GetAsync(requestUrl);
                    response.EnsureSuccessStatusCode();

                    return await response.Content.ReadAsByteArrayAsync();
                }
                catch (HttpRequestException ex)
                {
                    retryCount++;
                    if (retryCount > maxRetries)
                    {
                        Console.WriteLine($"Max retry attempts reached. Error: {ex.Message}");
                        throw;
                    }

                    Console.WriteLine($"Error fetching Buffer (attempt {retryCount}): {ex.Message}. Retrying in {delayBetweenRetries * retryCount} ms...");
                    await Task.Delay(delayBetweenRetries * retryCount);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                    throw;
                }
            }
        }

        public string? GetRequestUrl(string reportType, string? isMexico, string customerId, string period)
        {
            string? fullrequestUrl = null;
            if (reportType == "Bonus Summary")
            {
                fullrequestUrl = isMexico == null ? $"{serverURL}?/WebReports/COMMISSIONS/DISTRIBUTOR/CommissionBonusSummary&rs:Format=PDF&customerid={customerId}&period={period}"
                                                  : $"{serverURL}?/WebReports/COMMISSIONS/DISTRIBUTOR/CommissionBonusSummary{isMexico}&rs:Format=PDF&customerid={customerId}&period={period}";
            }
            else if (reportType == "Business Profile")
            {
                fullrequestUrl = $"{serverURL}?/WebReports/COMMISSIONS/DISTRIBUTOR/CommissionBusinessProfileAPI&rs:Format=PDF&customerid={customerId}&period={period}";
            }
            return fullrequestUrl;
        }
    }
}
