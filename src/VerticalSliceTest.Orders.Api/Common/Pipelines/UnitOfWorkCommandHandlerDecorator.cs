namespace VerticalSliceTest.Orders.Api.Common.Pipelines;

internal sealed class UnitOfWorkCommandHandlerDecorator<TCommand, TResponse>(
    ICommandHandler<TCommand, TResponse> inner,
    ApplicationDbContext dbContext)
    : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    public async Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken)
    {
        await dbContext.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            TResponse response = await inner.Handle(command, cancellationToken).ConfigureAwait(false);

            if (!ShouldCommit(response))
            {
                await dbContext.RollbackTransactionAsync(cancellationToken).ConfigureAwait(false);
                return response;
            }

            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            await dbContext.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);

            return response;
        }
        catch
        {
            await dbContext.RollbackTransactionAsync(CancellationToken.None).ConfigureAwait(false);
            throw;
        }
    }

    private static bool ShouldCommit(TResponse response)
    {
        if (response is Result result)
        {
            return result.IsSuccess;
        }

        return true;
    }
}
