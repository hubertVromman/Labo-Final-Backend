using API.Models.Forms;
using BLL.Models;
using Domain.Models;

namespace API.Mappers {
  public static class Mapper {

    public static Token ToBLL(this TokenForm t) {
      return new Token() {
        AccessToken = t.AccessToken,
        RefreshToken = t.RefreshToken,
      };
    }


    public static Race ToDomain(this RaceForm r) {
      return new Race() {
        RaceName = r.RaceName!,
        Place = r.Place!,
        StartDate = (DateOnly)r.StartDate!,
        RaceType = r.RaceType!,
        Distance = (Decimal)r.Distance!,
        RealDistance = (Decimal)r.RealDistance!,
      };
    }
  }
}
