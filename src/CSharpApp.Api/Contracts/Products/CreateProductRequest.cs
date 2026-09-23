public sealed record CreateProductRequest(
    string Title,
    decimal Price,
    string Description,
    int CategoryId,
    IReadOnlyCollection<string> Images);