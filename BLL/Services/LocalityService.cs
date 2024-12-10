using DAL.Repositories;
using Domain.Models;

namespace BLL.Services {
  public class LocalityService(LocalityRepo lr) {

    public IEnumerable<Locality> GetAll() {
      return lr.GetAll();
    }
  }
}
