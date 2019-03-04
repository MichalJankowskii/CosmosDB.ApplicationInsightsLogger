namespace CosmosDB.ApplicationInsightsLogger
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.Documents.Linq;

    public static class CosmosDbExtension
    {
        private static readonly TelemetryClient TelemetryClient = new TelemetryClient();

        public static string DependencyName { get; set; } = "";

        public static async Task<FeedResponse<T>> ExecuteWithStatsLogging<T>(this IQueryable<T> queryable, string commandName = null, string operationId = null)
        {
            var documentQuery = queryable.AsDocumentQuery();
            var now = DateTimeOffset.UtcNow;
            var stopwatch = Stopwatch.StartNew();
            var response = await documentQuery.ExecuteNextAsync<T>();
            stopwatch.Stop();
            LogStats(now, stopwatch.Elapsed, response.RequestCharge, commandName ?? string.Empty, operationId, response.ContentLocation ?? String.Empty, queryable.ToString());
            return response;
        }

        public static void LogStats(DateTimeOffset start, TimeSpan duration, double requestCharge, string commandName, string operationId, string contentLocation, string query)
        {
            var dependency = new DependencyTelemetry("CosmosDB", contentLocation, DependencyName, query, start, duration, "200", true);
            if (operationId != null)
            {
                dependency.Context.Operation.Id = operationId;
            }

            dependency.Type = "CosmosDB";
            dependency.Properties["commandName"] = commandName;
            dependency.Properties["requestCharge"] = requestCharge.ToString(CultureInfo.InvariantCulture);

            TelemetryClient.TrackDependency(dependency);
        }
    }
}
