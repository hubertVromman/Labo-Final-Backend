using Dapper;
using Domain.Models;
using Microsoft.Data.SqlClient;

namespace DAL.Repositories {
  public class LocalityRepo(SqlConnection conn) {

    public IEnumerable<Locality> GetAll() {
      string sql = "SELECT * FROM [locality]";
      return conn.Query<Locality>(sql);
    }
  }
}
