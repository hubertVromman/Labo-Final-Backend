using BLL.Models;
using DAL.Models.DTO;
using DAL.Repositories;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BLL.Services {
    public class UserService(UserRepo ur, RunnerRepo rr) {

        public void Register(string email, string password, string firstname, string lastname) {
            Runner r = rr.GetRunnerByName(firstname, lastname) ?? rr.AddRunner(firstname, lastname);
            ur.AddUser(email, password, r.RunnerId);
        }

        public Token Login(string email, string password) {
            FullUser fu = ur.GetFullUserByEmailAndPassword(email, password) ?? throw new Exception("Mauvais email ou mot de passe");
            Console.WriteLine(JsonSerializer.Serialize(fu));
            return GenerateTokensFromUser(fu);
        }

        public IEnumerable<User> GetAll() {
            return ur.GetAll();
        }

        public static readonly string secretKey =
            "La Saint-Nicolas c'est le 6 décembre et Noël c'est le 25 décembre";

        public string GenerateAccessToken(FullUser u) {
            //Génération de la signin key
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

            //Génération du payload (Body)
            Claim[] myclaims =
            {
                new Claim(ClaimTypes.NameIdentifier, u.UserId.ToString()),
                new Claim(ClaimTypes.Email, u.Email),
                new Claim(ClaimTypes.Role, u.Role),
                new Claim(ClaimTypes.GivenName, u.Firstname),
                new Claim(ClaimTypes.Name, u.Lastname),
            };

            //Génération du token

            JwtSecurityToken token = new JwtSecurityToken(
                claims: myclaims,
                signingCredentials: credentials,
                expires: DateTime.Now.AddMinutes(20),
                issuer: "monapi.com"
            );

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            return handler.WriteToken(token);
        }

        public string GenerateRefreshToken() {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public Token GenerateTokensFromUser(FullUser fu, bool updateExpiration = true) {

            Token t = new Token() {
                AccessToken = GenerateAccessToken(fu),
                RefreshToken = GenerateRefreshToken(),
            };

            ur.SaveToken(t.AccessToken, t.RefreshToken, fu.UserId);

            return t;
        }

        public Token RefreshToken(Token t) {
            FullUser u = ur.GetFullUserFromToken(t.AccessToken, t.RefreshToken)
                ?? throw new Exception("Token non valide ou expiré");
            return GenerateTokensFromUser(u, false);
        }
    }
}
