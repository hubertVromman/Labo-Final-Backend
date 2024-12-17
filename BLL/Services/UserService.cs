using BLL.Models;
using DAL.Repositories;
using Domain.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BLL.Services {
  public class UserService(UserRepo ur, RunnerRepo rr, MailService ms) {

    public bool Register(string email, string password, string firstname, string lastname) {
      password = password.HashTo64();
      Runner r = rr.GetRunnerByName(firstname, lastname) ?? rr.AddRunner(firstname, lastname);
      string activationCode = RNG(20);
      int userId = ur.AddUser(email, password, r.RunnerId, activationCode);
      if (userId > 0) {
        _ = ms.SendValidation(email, userId, activationCode);
      }
      return userId > 0;
    }

    public bool ForgotPasswordRequest(string email) {
      string resetPasswordCode = RNG(20);
      DateTime resetPasswordExpiration = DateTime.Now.AddMinutes(10);
      int userId = ur.AddResetPasswordCode(email, resetPasswordCode, resetPasswordExpiration);
      _ = ms.SendResetPassword(email, userId, resetPasswordCode);
      return userId > 0;
    }

    public bool ResetPassword(int userId, string resetPasswordCode, string newPassword) {
      return ur.ResetPassword(userId, resetPasswordCode, newPassword.HashTo64());
    }

    public Token Login(string email, string password) {
      password = password.HashTo64();
      FullUser fu = ur.GetFullUserByEmailAndPassword(email, password) ?? throw new Exception("Mauvais email ou mot de passe");
      if (!fu.IsActive)
        throw new Exception("Utilisateur pas encore activé, veuillez vérifier vos mails");
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

      JwtSecurityTokenHandler handler = new();

      return handler.WriteToken(token);
    }

    public string RNG(int size) {
      var randomNumber = new byte[size];
      using var rng = RandomNumberGenerator.Create();
      rng.GetBytes(randomNumber);
      return Convert.ToBase64String(randomNumber);
    }

    public Token GenerateTokensFromUser(FullUser fu, bool updateExpiration = true) {

      Token t = new Token() {
        AccessToken = GenerateAccessToken(fu),
        RefreshToken = RNG(32),
      };

      ur.SaveToken(t.AccessToken, t.RefreshToken, fu.UserId);

      return t;
    }

    public Token RefreshToken(Token t) {
      FullUser u = ur.GetFullUserFromToken(t.AccessToken, t.RefreshToken)
          ?? throw new Exception("Token non valide ou expiré");
      return GenerateTokensFromUser(u, false);
    }

    public FullUser? GetByEmail(string email) {
      return ur.GetByEmail(email);
    }

    public FullUser? GetByName(string firstname, string lastname) {
      return ur.GetByName(firstname, lastname);
    }

    public bool ChangeAnonymous(int userId, bool isAnonymous) {
      return ur.ChangeAnonymous(userId, isAnonymous);
    }

    public bool Activate(int userId, string activationCode) {
      return ur.Activate(userId, activationCode);
    }
  }
}
