using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace InstaTollLearningApp.Security
{
    internal class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 100000;

        public static string HashPassword(string password)
        {
            byte[] salt = new byte[SaltSize];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(
                password, salt, Iterations))
            {
                byte[] hash = pbkdf2.GetBytes(HashSize);

                return Iterations + ":" +
                    Convert.ToBase64String(salt) + ":" +
                    Convert.ToBase64String(hash);
            }
        }

        public static bool VerifyPassword(
            string password, string storedHash)
        {
            try
            {
                string[] parts = storedHash.Split(':');

                if (parts.Length != 3)
                    return false;

                int iterations = int.Parse(parts[0]);

                if (iterations < 100000 || iterations > 1000000)
                    return false;

                byte[] salt = Convert.FromBase64String(parts[1]);
                byte[] expectedHash =
                    Convert.FromBase64String(parts[2]);

                if (salt.Length != SaltSize ||
                    expectedHash.Length != HashSize)
                    return false;

                using (var pbkdf2 = new Rfc2898DeriveBytes(
                    password, salt, iterations))
                {
                    byte[] actualHash =
                        pbkdf2.GetBytes(HashSize);

                    int difference = 0;

                    for (int i = 0; i < HashSize; i++)
                    {
                        difference |=
                            actualHash[i] ^ expectedHash[i];
                    }

                    return difference == 0;
                }
            }
            catch (FormatException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }
}
