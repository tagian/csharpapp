public sealed record CreateProductModel(
    string Title,
    decimal Price,
    string Description,
    int CategoryId,
    IReadOnlyCollection<string> Images);
