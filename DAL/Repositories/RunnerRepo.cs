//using DAL.Models.DTO;
using Dapper;
using Domain.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json;

namespace DAL.Repositories {
  public class RunnerRepo(SqlConnection conn) {

    public Runner? GetRunnerByName(string firstname, string lastname) {
      string sql = "SELECT * FROM runner WHERE Firstname = @firstname AND Lastname = @lastname";
      return conn.QuerySingleOrDefault<Runner>(sql, new { firstname, lastname });
    }

    public Runner? GetById(int id) {
      string sql = "SELECT * FROM runner WHERE RunnerId = @id";
      return conn.QuerySingleOrDefault<Runner>(sql, new { id });
    }

    public Runner AddRunner(string firstname, string lastname, string? gender = null) {
      string sql = "INSERT INTO runner (Firstname, Lastname, Gender) " +
          "VALUES (@firstname, @lastname, @gender)";

      conn.Execute(sql, new { firstname, lastname, gender });

      return GetRunnerByName(firstname, lastname)!;
    }

    public void UpdateRunnerGender(int runnerId, string gender) {
      string sql = "UPDATE runner SET Gender = @gender " +
          "WHERE RunnerId = @runnerId";

      conn.Execute(sql, new { runnerId, gender });
    }

    public IEnumerable<Runner> Search(string query) {
      string[] fragments = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);

      string sql = "SELECT * FROM runner WHERE";

      var parameters = new DynamicParameters();
      for (int i = 0; i < fragments.Length; i++) {
        parameters.Add("fragment" + i, fragments[i] + "%", DbType.String, ParameterDirection.Input, fragments[i].Length + 1);
        if (i > 0)
          sql += " AND ";
        sql += " (Lastname LIKE @fragment" + i;
        sql += " OR Firstname LIKE @fragment" + i + ")";
      }
      return conn.Query<Runner>(sql, parameters);
    }
  }
}
