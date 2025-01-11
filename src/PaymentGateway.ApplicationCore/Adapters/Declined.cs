using System.Runtime.InteropServices;
using OneOf;

namespace PaymentGateway.ApplicationCore.Adapters;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct Declined {}

public static class OneOfExtensions
{
    public static bool IsDeclined<T>(this OneOf<T, Declined> result)
    {
        return result.IsT1;
    }

    public static T GetCode<T>(this OneOf<T, Declined> result)
    {
        return result.AsT0;
    }
}