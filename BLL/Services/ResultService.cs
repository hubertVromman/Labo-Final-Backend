using DAL.Repositories;
using Domain.Models;

namespace BLL.Services {
  public class ResultService(ResultRepo rr) {
    public IEnumerable<Result> GetByRunnerId(int runnerId) {
      return rr.GetByRunnerId(runnerId);
    }

    public IEnumerable<Result> GetByRaceId(int runnerId) {
      return rr.GetByRaceId(runnerId);
    }

  }
}
