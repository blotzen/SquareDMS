using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Text;

namespace SquareDMS.Core
{
    public class Credential
    {
        private readonly int _iterations;

        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="iterations">Amount of iterations used (work factor).</param>
        /// <exception cref="ArgumentException"></exception>
        public Credential(string username, string password, int iterations)
        {
            if (!ValidateUsername(username))
                throw new ArgumentException("Username does not fulfil the requirements.");

            if (!ValidatePassword(password))
                throw new ArgumentException("Password does not fulfil the requirements.");

            if (iterations <= 10_000)
                throw new ArgumentException("Too few iterations are not secure.");

            UserName = username;
            Password = password;
            _iterations = iterations;
        }

        public string UserName { get; }

        public string Password { get; }

        /// <summary>
        /// Checks if both Hashes are equal.
        /// </summary>
        /// <param name="passwordHash1"></param>
        /// <param name="passwordHash2"></param>
        /// <returns></returns>
        public static bool MatchPasswordHashes(byte[] passwordHash1, byte[] passwordHash2)
        {
            if (passwordHash1 == null || passwordHash2 == null)
                return false;

            if (passwordHash1.Length != passwordHash2.Length)
                return false;

            for (int i = 0; i < passwordHash1.Length; i++)
            {
                if (passwordHash1[i] != passwordHash2[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if the password has the correct length,
        /// contains lower- and uppercase letters, special chars
        /// and numerics.
        /// </summary>
        /// <returns></returns>
        public bool ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            //TODO: implement logic.

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool ValidateUsername(string username)
        {
            //TODO: implement logic.

            return true;
        }

        /// <summary>
        /// Hashes a given password (PBKDF2). Uses the username as a salt.
        /// </summary>
        /// <returns>32 byte long array of hashed password.</returns>
        /// <exception cref="ArgumentException">If invalid arguments are passed.</exception>
        public byte[] HashPassword()
        {
            try
            {
                var salt = Encoding.ASCII.GetBytes(UserName);

                return KeyDerivation.Pbkdf2(Password,
                    salt,
                    KeyDerivationPrf.HMACSHA512,
                    _iterations,
                    32);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"One or more arguments are invalid. {ex.Message}");
            }
        }
    }
}
