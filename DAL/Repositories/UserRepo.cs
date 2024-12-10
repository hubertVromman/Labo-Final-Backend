using Dapper;
using Domain.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DAL.Repositories {
  public class UserRepo(SqlConnection conn) {
    public bool AddUser(string email, string password, int runnerId) {
      //string sql = "INSERT INTO [user] (Email, Password, RunnerId) " +
      //    "VALUES (@email, @password, @runnerId)";

      return conn.Execute("Register", new { email, password, runnerId }, commandType: CommandType.StoredProcedure) > 0;
    }

    public IEnumerable<User> GetAll() {
      string sql = "SELECT * FROM [user]";
      return conn.Query<User>(sql);
    }

    public FullUser? GetFullUserByEmailAndPassword(string email, string password) {
      //string sql = "SELECT * FROM [user] JOIN runner ON runner.RunnerId = [user].RunnerId WHERE Email = @email AND Password = @password";
      return conn.QuerySingleOrDefault<FullUser>("Login", new { email, password }, commandType: CommandType.StoredProcedure);
    }

    public FullUser? GetFullUserFromToken(string accessToken, string refreshToken) {
      return conn.QuerySingleOrDefault<FullUser>("SELECT * FROM fulluser WHERE RefreshToken = @refreshToken AND AccessToken = @accessToken AND RefreshTokenExpiration > GETDATE()", new { accessToken, refreshToken });
    }

    public void SaveToken(string accessToken, string refreshToken, int userId, bool updateExpiration = true) {
      string sql = "UPDATE [user] " +
          "SET AccessToken = @accessToken, " +
          "RefreshToken = @refreshToken, " +
          "RefreshTokenExpiration = @refreshTokenExpiration, " +
          "MaxRefreshTokenExpiration = @maxRefreshTokenExpiration " +
          "WHERE UserId = @userId";

      DateTime maxRefreshTokenExpiration = updateExpiration ? DateTime.Now.AddDays(7) : conn.QuerySingle<DateTime>("SELECT MaxRefreshTokenExpiration FROM [user] WHERE UserId = @userId", new { userId });
      DateTime refreshTokenExpiration = maxRefreshTokenExpiration > DateTime.Now.AddHours(3) ? DateTime.Now.AddHours(3) : maxRefreshTokenExpiration;

      conn.Execute(sql, new { accessToken, refreshToken, userId, maxRefreshTokenExpiration, refreshTokenExpiration });
    }

    public FullUser? GetByEmail(string email) {
      string sql = "SELECT * FROM [fulluser] WHERE Email = @email";
      return conn.QuerySingleOrDefault<FullUser>(sql, new { email });
    }

    public FullUser? GetByName(string firstname, string lastname) {
      string sql = "SELECT * FROM [fulluser] WHERE Firstname = @firstname AND Lastname = @lastname";
      return conn.QuerySingleOrDefault<FullUser>(sql, new { firstname, lastname });
    }
  }
}
