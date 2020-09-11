using System;
using Xunit;
using SquareDMS.Core;

namespace SquareDMS.Core_Test
{
    /// <summary>
    /// Tests the Credential Class
    /// </summary>
    public class TestCredential
    {
        static readonly string _username = "bob";
        static readonly int _iterations = 100_000;

        static readonly Credential _cred1 = new Credential(_username, "Supersecure!246", _iterations);
        static readonly Credential _cred2 = new Credential(_username, "Supersecure!247", _iterations);

        /// <summary>
        /// Checks if the constructor works for valid objects.
        /// </summary>
        [Fact]
        public void ConstructorValid_Test()
        {
            var validCred1 = new Credential(_username, "Supersecure!246", _iterations);
            var validCred2 = new Credential(_username, "Supersecure!247", _iterations);

            Assert.NotNull(validCred1);
            Assert.NotNull(validCred2);
        }

        /// <summary>
        /// Checks if the constructor works for Invalid objects.
        /// </summary>
        [Fact]
        public void ConstructorInValid_Test()
        {
            Assert.Throws<ArgumentException>(() => new Credential(_username, "Supersecure!247", 0));
            //Assert.Throws<ArgumentException>(() => new Credential("ad", "Supersecure!247", _iterations));
            //Assert.Throws<ArgumentException>(() => new Credential(_username, "asdf", _iterations));
        }

        /// <summary>
        /// Tests if same password hashes are equal.
        /// </summary>
        [Fact]
        public void HashPasswordEquality_Test()
        {
            var hash1 = _cred1.HashPassword();
            var hash2 = _cred1.HashPassword();

            bool match = Credential.MatchPasswordHashes(hash1, hash2);

            Assert.True(match);
        }

        /// <summary>
        /// Tests if different password hashes are inequal.
        /// </summary>
        [Fact]
        public void HashPasswordInEquality_Test()
        {
            var hash1 = _cred1.HashPassword();
            var hash2 = _cred2.HashPassword();

            bool match = Credential.MatchPasswordHashes(hash1, hash2);

            Assert.False(match);
        }
    }
}
