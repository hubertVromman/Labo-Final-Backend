using BLL.Tools;
using DAL.Repositories;
using Domain.Models;

namespace BLL.Services {
  public class ResultService(ResultRepo rr) {
    public IEnumerable<Result> GetByRunnerId(int runnerId) {
      return rr.GetByRunnerId(runnerId);
    }

    public ObjectList<Result> GetByRaceId(int runnerId, int offset, int limit) {
      ObjectList<Result> results = rr.GetByRaceId(runnerId, offset, limit);
      results.Objects = results.Objects.Select(o => {
        o.Runner = o.Runner.Anonymize();
        return o;
      });
      return results;
    }
  }
}
