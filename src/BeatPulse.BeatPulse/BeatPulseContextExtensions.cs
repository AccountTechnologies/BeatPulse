using BeatPulse.Core;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;


namespace BeatPulse.BeatPulse
{
    public static class BeatPulseContextExtensions
    {
        public static BeatPulseContext AddBeatPulseGroup(this BeatPulseContext context, Uri uri, string defaultPath = "liveness", string name = nameof(BeatPulseLiveness))
        {
            return context.AddLiveness(name, setup =>
            {
                setup.UsePath(defaultPath);
                setup.UseFactory(sp => new BeatPulseLiveness(uri, sp.GetService<ILogger<BeatPulseLiveness>>()));
            });
        }
    }
}
