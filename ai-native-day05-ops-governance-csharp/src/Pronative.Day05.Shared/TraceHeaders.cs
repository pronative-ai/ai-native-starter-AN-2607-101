using System.Diagnostics;

namespace Pronative.Day05.Shared;

public static class TraceHeaders
{
    public static void Apply(HttpRequestMessage request, Day05Config config)
    {
        request.Headers.TryAddWithoutValidation("x-batch-id", config.BatchId);
        request.Headers.TryAddWithoutValidation("x-student-id", config.StudentId);
        request.Headers.TryAddWithoutValidation("x-environment-id", config.EnvironmentId);
        request.Headers.TryAddWithoutValidation("x-cost-center", config.CostCenter);

        var activity = Activity.Current;
        if (activity is not null)
        {
            request.Headers.TryAddWithoutValidation("traceparent", activity.Id);
            request.Headers.TryAddWithoutValidation("x-trace-id", activity.TraceId.ToString());
            request.Headers.TryAddWithoutValidation("x-operation-id", activity.SpanId.ToString());
        }
    }
}
