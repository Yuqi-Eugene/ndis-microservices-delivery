using Api.Application.Bookings;
using Api.Application.Participants;
using Api.Application.ServiceDeliveries.Commands;
using Api.Application.ServiceDeliveries.Validators;
using Api.Domain.Entities;
using Api.Dtos;
using Api.Dtos.Bookings;
using Api.Dtos.Participants;
using AutoMapper;

var failures = new List<string>();

Run("CreateBookingCommandValidator rejects invalid duration", () =>
{
    var validator = new CreateBookingCommandValidator();
    var command = new CreateBookingCommand(new BookingCreateDto
    {
        ParticipantId = Guid.NewGuid(),
        ProviderId = Guid.NewGuid(),
        ServiceType = "Support",
        DurationMinutes = 0
    });

    var result = validator.Validate(command);

    Assert(!result.IsValid, "Expected validation failure.");
    Assert(result.Errors.Any(x => x.ErrorMessage == "DurationMinutes is invalid."), "Expected duration error.");
});

Run("SubmitServiceDeliveryCommandValidator requires id", () =>
{
    var validator = new SubmitServiceDeliveryCommandValidator();
    var command = new SubmitServiceDeliveryCommand(Guid.Empty, "user-1", false);

    var result = validator.Validate(command);

    Assert(!result.IsValid, "Expected validation failure.");
    Assert(result.Errors.Any(x => x.ErrorMessage == "ServiceDeliveryId is required."), "Expected id error.");
});

Run("AutoMapper profile shapes participant list response", () =>
{
    var mapper = new MapperConfiguration(cfg => cfg.AddProfile<ApiMappingProfile>()).CreateMapper();
    var source = new ParticipantListResult(
        1,
        2,
        10,
        new[]
        {
            new Participant
            {
                Id = Guid.NewGuid(),
                FullName = "Test User",
                Status = "Active"
            }
        });

    var result = mapper.Map<ParticipantListResponseDto>(source);

    Assert(result.Total == 1, "Expected total to match.");
    Assert(result.Page == 2, "Expected page to match.");
    Assert(result.Items.Count == 1, "Expected one mapped item.");
    Assert(result.Items[0].FullName == "Test User", "Expected mapped participant name.");
});

if (failures.Count == 0)
{
    Console.WriteLine("All application-layer checks passed.");
    return 0;
}

foreach (var failure in failures)
{
    Console.Error.WriteLine(failure);
}

return 1;

void Run(string name, Action test)
{
    try
    {
        test();
        Console.WriteLine($"PASS: {name}");
    }
    catch (Exception ex)
    {
        failures.Add($"FAIL: {name} - {ex.Message}");
    }
}

static void Assert(bool condition, string message)
{
    if (!condition)
        throw new InvalidOperationException(message);
}
