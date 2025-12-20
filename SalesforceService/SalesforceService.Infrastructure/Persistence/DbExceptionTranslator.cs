using Microsoft.EntityFrameworkCore;
using Npgsql;
using SalesforceService.Application.Exceptions;

namespace SalesforceService.Infrastructure.Persistence;

public static class DbExceptionTranslator
{
    public static Exception Translate(DbUpdateException ex)
    {
        if (IsUniqueConstraintViolation(ex))
        {
            return new DuplicateEventException();
        }

        return ex;
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        // Postgres unique constraint violation code
        // https://www.postgresql.org/docs/current/errcodes-appendix.html
        // https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbupdateexception?view=efcore-10.0
        if (ex.InnerException is PostgresException { SqlState: "23505" }) 
            return true;

        // Generic check in case of other DB providers
        return ex.InnerException?.Message.Contains("unique constraint", StringComparison.OrdinalIgnoreCase) ?? false;
    }
}