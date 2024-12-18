using BLL.Tools;
using DAL.Repositories;
using Domain.Models;

namespace BLL.Services {
  public class ResultService(ResultRepo rr) {
    public IEnumerable<Result> GetByRunnerId(int runnerId) {
      return rr.GetByRunnerId(runnerId);
    }

    public ObjectList<Result> GetByRaceId(int runnerId, int offset, int limit, string? name) {
      ObjectList<Result> results = name is null ? rr.GetByRaceId(runnerId, offset, limit) : rr.GetByRaceIdWithFilter(runnerId, offset, limit, name);
      results.Objects = results.Objects.Select(o => {
        o.Runner = o.Runner.Anonymize();
        return o;
      }).Where(o => name is null || !o.Runner.IsAnonymous);
      if (name is not null)
        results.Count = results.Objects.Count();
      return results;
    }
  }
}
