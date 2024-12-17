using Dapper;
using Domain.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DAL.Repositories {
  public class UserRepo(SqlConnection conn) {
    public int AddUser(string email, string password, int runnerId, string activationCode) {
      //string sql = "INSERT INTO [user] (Email, Password, RunnerId) " +
      //    "VALUES (@email, @password, @runnerId)";
      int userId = conn.QuerySingle<int>("Register", new { email, password, runnerId, activationCode }, commandType: CommandType.StoredProcedure);
      return userId;
    }

    public int AddResetPasswordCode(string email, string resetPasswordCode, DateTime resetPasswordExpiration) {
      return conn.QuerySingle<int>("UPDATE [user] SET ResetPasswordCode = @resetPasswordCode, ResetPasswordExpiration = @resetPasswordExpiration OUTPUT inserted.UserId WHERE Email = @email AND IsActive = 1", new { email, resetPasswordCode, resetPasswordExpiration });
    }

    public bool ResetPassword(int userId, string resetPasswordCode, string newPassword) {
      return conn.Execute("ResetPassword", new { userId, resetPasswordCode, newPassword }, commandType: CommandType.StoredProcedure) > 0;
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
      return conn.QuerySingleOrDefault<FullUser>("SELECT * FROM [user] JOIN runner ON runner.RunnerId = [user].RunnerId WHERE RefreshToken = @refreshToken AND AccessToken = @accessToken AND RefreshTokenExpiration > GETDATE()", new { accessToken, refreshToken });
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

    public bool ChangeAnonymous(int userId, bool isAnonymous) {
      string sql = "UPDATE r SET r.IsAnonymous = @isAnonymous FROM runner r JOIN [user] u ON u.RunnerId = r.RunnerId WHERE u.UserId = @userId";
      return conn.Execute(sql, new { userId, isAnonymous }) > 0;
    }

    public bool Activate(int userId, string activationCode) {
      string sql = "UPDATE [user] SET IsActive = 1 WHERE ActivationCode = @activationCode AND UserId = @userId";
      conn.Execute(sql, new { userId, activationCode });

      return conn.QuerySingle<bool>("SELECT IsActive FROM [user] WHERE UserId = @userId", new { userId });
    }
  }
}
