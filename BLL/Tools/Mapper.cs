using Domain.Models;

namespace BLL.Tools {
  public static class Mapper {
    public static Runner Anonymize(this Runner r) {
      if (r.IsAnonymous) {
        r.Lastname = "Anonyme";
        r.Firstname = "Anonyme";
      }
      return r;
    }
  }
}
