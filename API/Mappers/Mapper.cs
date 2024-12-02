using API.Models.Forms;
using BLL.Models;

namespace API.Mappers {
    public static class Mapper {

        public static Token ToBLL(this TokenForm t) {
            return new Token() {
                AccessToken = t.AccessToken,
                RefreshToken = t.RefreshToken,
            };
        }
    }
}
