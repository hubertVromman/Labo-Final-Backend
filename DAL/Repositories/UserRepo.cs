using DAL.Models.DTO;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories {
    public class UserRepo(SqlConnection conn) {
        public bool AddUser(string email, string password, int runnerId) {
            string sql = "INSERT INTO [user] (Email, Password, RunnerId) " +
                "VALUES (@email, @password, @runnerId)";

            return conn.Execute(sql, new { email, password, runnerId }) > 0;
        }

        public IEnumerable<User> GetAll() {
            string sql = "SELECT * FROM [user]";
            return conn.Query<User>(sql);
        }

        public FullUser? GetFullUserByEmailAndPassword(string email, string password) {

            string sql = "SELECT * FROM [user] JOIN runner ON runner.RunnerId = [user].RunnerId WHERE Email = @email AND Password = @password";
            return conn.QuerySingleOrDefault<FullUser>(sql, new { email, password });
        }

        public FullUser? GetFullUserFromToken(string accessToken, string refreshToken) {
            return conn.QuerySingleOrDefault<FullUser>("SELECT * FROM  [user] JOIN runner ON runner.RunnerId = [user].RunnerId WHERE RefreshToken = @refreshToken AND AccessToken = @accessToken AND RefreshTokenExpiration > GETDATE()", new { accessToken, refreshToken });
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
    }
}
