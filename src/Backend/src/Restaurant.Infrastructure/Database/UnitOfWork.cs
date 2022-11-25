using Restarant.Application.Abstractions;
using System.Data.Common;

namespace Restaurant.Infrastructure.Database
{
    internal sealed class UnitOfWork : IUnitOfWork
    {
        private readonly DbConnection _dbConnection;
        private DbTransaction? _dbTransaction = null;

        public UnitOfWork(DbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<DbTransaction> BeginTransactionAsync()
        {
            if (_dbTransaction is not null)
            {
                return _dbTransaction;
            }

            return await _dbConnection.BeginTransactionAsync();
        }
        
        public DbTransaction BeginTransaction()
        {
            if (_dbTransaction is not null)
            {
                return _dbTransaction;
            }

            _dbTransaction = _dbConnection.BeginTransaction();
            return _dbTransaction;
        }

        public async Task CommitAsync()
        {
            if (_dbTransaction is null)
            {
                throw new InvalidOperationException("Transaction was not started");
            }

            await _dbTransaction.CommitAsync();
            Dispose(); // dispose transaction
        }

        public void Commit()
        {
            if (_dbTransaction is null)
            {
                throw new InvalidOperationException("Transaction was not started");
            }

            _dbTransaction.Commit();
            Dispose(); // dispose transaction
        }

        public void Dispose()
        {
            if (_dbTransaction is not null)
            {
                _dbTransaction.Dispose();
            }

            _dbTransaction = null;
        }

        public Task RollbackAsync()
        {
            if (_dbTransaction is null)
            {
                throw new InvalidOperationException("Transaction was not started");
            }

            return _dbTransaction.RollbackAsync();
        }

        public void Rollback()
        {
            if (_dbTransaction is null)
            {
                throw new InvalidOperationException("Transaction was not started");
            }

            _dbTransaction.Rollback();
        }
    }
}
