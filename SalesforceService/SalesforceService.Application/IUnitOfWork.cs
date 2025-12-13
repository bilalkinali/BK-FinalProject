using System.Data;

namespace SalesforceService.Application;

public interface IUnitOfWork
{
    Task CommitAsync();
    Task RollbackAsync();
    Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.Serializable);
}