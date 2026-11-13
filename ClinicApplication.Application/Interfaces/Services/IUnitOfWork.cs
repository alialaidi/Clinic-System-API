using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApplication.Application.Interfaces.Services
{
    public interface IUnitOfWork
    {
        Task<int> CommitAsync(CancellationToken ct = default);

        Task BeginTransactionAsync(CancellationToken ct = default);
        Task CommitTransactionAsync(CancellationToken ct = default);
        Task RollbackTransactionAsync(CancellationToken ct = default);
    }
}
