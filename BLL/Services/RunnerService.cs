using DAL.Repositories;
using Domain.Models;

namespace BLL.Services {
  public class RunnerService(RunnerRepo rr) {
    public Runner? GetById(int id) {
      return rr.GetById(id);
    }

    public Runner? GetByName(string firstname, string lastname) {
      return rr.GetRunnerByName(firstname, lastname);
    }

    public IEnumerable<Runner> Search(string query) {
      return rr.Search(query).Where(r => !r.IsAnonymous);
    }
  }
}
