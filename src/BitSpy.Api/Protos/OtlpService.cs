using Grpc.Core;

namespace BitSpy.Api.Protos;

public class OtlpService : Otlp.OtlpBase
{
    public override Task<RcvResponse> Receiver(ActivityList request, ServerCallContext context)
    {
        Console.WriteLine(request);
        return base.Receiver(request, context);
    }
}