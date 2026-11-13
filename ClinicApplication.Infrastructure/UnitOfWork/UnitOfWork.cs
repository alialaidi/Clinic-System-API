using ClinicApplication.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore.Storage;
using ClinicApplication.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApplication.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ClinicDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(ClinicDbContext context)
        {
            _context = context;
        }

        public Task<int> CommitAsync(CancellationToken ct = default)
            => _context.SaveChangesAsync(ct);

        public async Task BeginTransactionAsync(CancellationToken ct = default)
            => _transaction = await _context.Database.BeginTransactionAsync(ct);

        public async Task CommitTransactionAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
            if (_transaction is not null)
                await _transaction.CommitAsync(ct);
        }

        public async Task RollbackTransactionAsync(CancellationToken ct = default)
        {
            if (_transaction is not null)
                await _transaction.RollbackAsync(ct);
        }
    }
}
