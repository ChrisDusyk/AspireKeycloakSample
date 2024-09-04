namespace AspireKeycloak.MessageContracts
{
    public record ReportingMessage
    {
        public required string Text { get; init; }
        public required DateTimeOffset Timestamp { get; init; }
    }
}