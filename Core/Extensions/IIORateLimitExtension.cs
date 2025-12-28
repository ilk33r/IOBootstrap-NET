using System;
using System.Text.Json;
using IOBootstrap.NET.Common.Exceptions.Common;
using IOBootstrap.NET.Core.Interfaces;
using IOBootstrap.NET.DataAccess.Context;

namespace IOBootstrap.NET.Core.Extensions;

public static class IIORateLimitExtension
{

    public static void CheckAndUpdateRateLimit<TViewModel, TDBContext>(this IIORateLimit<TViewModel, TDBContext> input, string apiName, int seconds, int requestCount)
    where TDBContext : IOBaseDatabaseContext<TDBContext>
    where TViewModel : IIOViewModel<TDBContext>, new()
    {
        string sessionName = "RateLimit" + apiName;

        string? rateLimitJson = input.HttpContext.Session.GetString(sessionName);
        if (String.IsNullOrEmpty(rateLimitJson))
        {
            input.CreateRateLimit(sessionName);
            return;
        }

        List<long>? rateLimit = JsonSerializer.Deserialize<List<long>>(rateLimitJson);
        if (rateLimit == null)
        {
            input.CreateRateLimit(sessionName);
            return;
        }

        long currentUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        List<long> removeValues = new List<long>();

        foreach (long requestSeconds in rateLimit)
        {
            if (requestSeconds + seconds < currentUnixTime)
            {
                removeValues.Add(requestSeconds);
            }
        }

        foreach (int removeValue in removeValues)
        {
            rateLimit.Remove(removeValue);
        }

        if (rateLimit.Count() > requestCount)
        {
            if (removeValues.Count() > 0)
            {
                input.UpdateRateLimit(sessionName, rateLimit);
            }

            throw new IORateLimitExceededException();
        }

        rateLimit.Add(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        input.UpdateRateLimit(sessionName, rateLimit);
    }

    private static void CreateRateLimit<TViewModel, TDBContext>(this IIORateLimit<TViewModel, TDBContext> input, string sessionName)
    where TDBContext : IOBaseDatabaseContext<TDBContext>
    where TViewModel : IIOViewModel<TDBContext>, new()
    {
        List<long> rateLimit = [DateTimeOffset.UtcNow.ToUnixTimeSeconds()];
        input.UpdateRateLimit(sessionName, rateLimit);
    }

    private static void UpdateRateLimit<TViewModel, TDBContext>(this IIORateLimit<TViewModel, TDBContext> input, string sessionName, List<long> rateLimit)
    where TDBContext : IOBaseDatabaseContext<TDBContext>
    where TViewModel : IIOViewModel<TDBContext>, new()
    {
        string rateLimitJson = JsonSerializer.Serialize(rateLimit);
        input.HttpContext.Session.SetString(sessionName, rateLimitJson);
    }
}
