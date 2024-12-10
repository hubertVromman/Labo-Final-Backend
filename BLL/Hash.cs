using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BLL {
    internal static class Hash {
        public static string HashTo64(this string data) {
            string adding = "La Saint-Nicolas c'est le 6 décembre";
            string adding2 = "La Noël c'est le 24-25 décembre";
            byte[] bytes = Encoding.UTF8.GetBytes(adding + data + adding2);
            byte[] hash = SHA512.HashData(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
