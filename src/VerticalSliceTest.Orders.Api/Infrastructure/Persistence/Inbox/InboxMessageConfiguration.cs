namespace VerticalSliceTest.Orders.Api.Infrastructure.Persistence.Inbox;

internal sealed class InboxMessageConfiguration : IEntityTypeConfiguration<InboxMessage>
{
    public void Configure(EntityTypeBuilder<InboxMessage> builder)
    {
        builder.ToTable("InboxMessages");

        builder.HasKey(inboxMessage => inboxMessage.Id);

        builder.Property(inboxMessage => inboxMessage.Type)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(inboxMessage => inboxMessage.Content)
            .IsRequired();

        builder.Property(inboxMessage => inboxMessage.Error);

        builder.HasIndex(inboxMessage => new
        {
            inboxMessage.ProcessedOnUtc,
            inboxMessage.OccurredOnUtc,
        });
    }
}
