namespace Api.Dtos;

public sealed record CollectionResponseDto<T>(
    int Count,
    IReadOnlyList<T> Items);
