namespace Api.Domain.Constants;

public static class BookingStatuses
{
    public const string Draft = "Draft";
    public const string Confirmed = "Confirmed";
    public const string Cancelled = "Cancelled";

    public static readonly string[] All =
    [
        Draft,
        Confirmed,
        Cancelled
    ];
}
