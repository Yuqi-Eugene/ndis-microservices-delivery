namespace Api.Domain.Constants;

public static class ServiceDeliveryStatuses
{
    public const string Draft = "Draft";
    public const string Submitted = "Submitted";
    public const string Approved = "Approved";
    public const string Rejected = "Rejected";

    public static readonly string[] All =
    [
        Draft,
        Submitted,
        Approved,
        Rejected
    ];
}
