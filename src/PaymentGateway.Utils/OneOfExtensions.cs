using OneOf;
using OneOf.Types;

namespace PaymentGateway.Utils;

public static class OneOfExtensions
{
    public static bool IsInvalid<T>(this OneOf<T, Error> result)
    {
        return result.IsT1;
    }

    public static T GetValue<T>(this OneOf<T, Error> result)
    {
        return result.AsT0;
    }
}