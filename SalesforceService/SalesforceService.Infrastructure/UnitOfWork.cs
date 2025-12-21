using SalesforceService.Application;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SalesforceService.Application.Exceptions;
using SalesforceService.Infrastructure.Persistence;

namespace SalesforceService.Infrastructure;

public class UnitOfWork<T> : IUnitOfWork where T : DbContext
{
    private readonly DbContext _db;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(T db)
    {
        _db = db;
    }

    async Task IUnitOfWork.BeginTransactionAsync(IsolationLevel isolationLevel)
    {
        if (_db.Database.CurrentTransaction != null) return;
        _transaction = await _db.Database.BeginTransactionAsync(isolationLevel);
    }

    async Task IUnitOfWork.CommitAsync()
    {
        if (_transaction == null) throw new Exception("You must call 'BeginTransaction' before Commit is called");

        try
        {
            await _db.SaveChangesAsync();
            await _transaction.CommitAsync();
        }
        // Exception for unique constraint violations
        catch (DbUpdateException ex)
        {
            var exception = DbExceptionTranslator.Translate(ex);

            if (exception is not DuplicateEventException)
            {
                await _transaction.RollbackAsync();
            }

            throw exception;
        }
        catch (Exception)
        {
            await _transaction.RollbackAsync();
            throw;
        }
        finally
        {
            await _transaction.DisposeAsync();
        }
    }

    async Task IUnitOfWork.RollbackAsync()
    {
        if (_transaction == null) throw new Exception("You must call 'BeginTransaction' before Rollback is called");

        await _transaction.RollbackAsync();
        await _transaction.DisposeAsync();
    }
}