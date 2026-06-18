namespace VerticalSliceTest.Orders.Api.Infrastructure.Persistence.Outbox;

internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");

        builder.HasKey(outboxMessage => outboxMessage.Id);

        builder.Property(outboxMessage => outboxMessage.Type)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(outboxMessage => outboxMessage.Content)
            .IsRequired();

        builder.Property(outboxMessage => outboxMessage.Error);

        builder.HasIndex(outboxMessage => new
        {
            outboxMessage.ProcessedOnUtc,
            outboxMessage.OccurredOnUtc,
        });
    }
}
