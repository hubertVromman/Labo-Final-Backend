using DAL.Repositories;
using Domain.Models;

namespace BLL.Services {
  public class ResultService(ResultRepo rr) {
    public IEnumerable<Result> GetByRunnerId(int runnerId) {
      return rr.GetByRunnerId(runnerId);
    }

    public ObjectList<Result> GetByRaceId(int runnerId, int offset, int limit) {
      return rr.GetByRaceId(runnerId, offset, limit);
    }

  }
}
