using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Stl.Fusion.EntityFramework.Operations
{
    public interface IDbOperationLogChangeMonitor<TDbContext>
        where TDbContext : DbContext
    {
        // Should return immediately
        Task WaitForChangesAsync(CancellationToken cancellationToken = default);
    }
}
