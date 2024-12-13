using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
