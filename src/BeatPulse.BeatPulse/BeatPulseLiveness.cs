using BeatPulse.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BeatPulse.BeatPulse
{
    public class BeatPulseLiveness
        : IBeatPulseLiveness
    {
        private readonly Uri _uri;
        private readonly ILogger<BeatPulseLiveness> _logger;

        public BeatPulseLiveness(System.Uri uri, ILogger<BeatPulseLiveness> logger = null)
        {
            _uri = uri;
            _logger = logger;
        }

        public async Task<LivenessResult> IsHealthy(LivenessExecutionContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger?.LogInformation($"{nameof(BeatPulseLiveness)} is checking configured uri's.");


                if (cancellationToken.IsCancellationRequested)
                {
                    return LivenessResult.UnHealthy($"Liveness execution is cancelled.");
                }

                var content = string.Empty;
                using (var httpClient = new HttpClient())
                {
                    var requestMessage = new HttpRequestMessage(HttpMethod.Get, _uri);

                    var response = await httpClient.SendAsync(requestMessage);
                    content = await response.Content.ReadAsStringAsync();
                }

                var message = JsonConvert.DeserializeObject<OutputLivenessMessageResponse>(content);

                string unHealthyItems = string.Join(",", message.Checks.Where(b => !b.IsHealthy).Select(c => c.Name));

                if (message.Checks.Any(c => !c.IsHealthy))
                {
                    _logger?.LogWarning($"The {nameof(BeatPulseLiveness)} check fail for uri {_uri}");
                    return LivenessResult.UnHealthy($"{nameof(BeatPulseLiveness)} {unHealthyItems} failing.");
                }


                _logger?.LogDebug($"The {nameof(BeatPulseLiveness)} check success.");

                return LivenessResult.Healthy();
            }
            catch (Exception ex)
            {

                _logger?.LogWarning($"The {nameof(BeatPulseLiveness)} check fail with the exception {ex.ToString()}.");

                return LivenessResult.UnHealthy(ex);
            }
        }

        private class LivenessResultResponse
        {
            public string Name { get; set; }

            public string Message { get; set; }

            public string Exception { get; set; }

            public long MilliSeconds { get; set; }

            public bool Run { get; set; }

            public string Path { get; set; }

            public bool IsHealthy { get; set; }
        }

        private class OutputLivenessMessageResponse
        {
            public IEnumerable<LivenessResultResponse> Checks { get; set; }

            public DateTime StartedAtUtc { get; set; }

            public DateTime EndAtUtc { get; set; }

            public int Code { get; set; }

            public string Reason { get; set; }

        }
    }
}
