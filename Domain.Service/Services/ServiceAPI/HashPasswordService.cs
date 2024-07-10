using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace Domain.Service.Services.ServiceApi
{
    public class HashPasswordService
    {
        public string Password(string password)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            byte[] hashedBytes = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8
            );
            return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hashedBytes);
        }

        public bool VerifyPassword(string enteredPassword, string hashedPassword)
        {
            string[] parts = hashedPassword.Split(':');
            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] savedHashedBytes = Convert.FromBase64String(parts[1]);
            byte[] hashedEnteredPassword = KeyDerivation.Pbkdf2(
                password: enteredPassword,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8
            );
            // Comparar os hashes
            return hashedEnteredPassword.SequenceEqual(savedHashedBytes);
        }
    }
}