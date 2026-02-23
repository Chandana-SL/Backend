using TimeTrack.API.Models;

namespace TimeTrack.API.Repository.IRepository;

public interface IBreakRepository : IGenericRepository<Break>
{
    Task<IEnumerable<Break>> GetBreaksByTimeLogIdAsync(Guid timeLogId);
    Task<Break?> GetActiveBreakForTimeLogAsync(Guid timeLogId);
    Task<Break?> GetActiveBreakForUserAsync(Guid userId);
}
