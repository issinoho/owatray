// ------------------------------------------------------------------
//  OWAtray
//  DrunkenBakery.OWAtray.Tests.Connections
//
//  <copyright file="SecurityExtensionsTests.cs" company="The Drunken Bakery">
//      Copyright (c) 2009-2026 The Drunken Bakery. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------

namespace DrunkenBakery.OWAtray.Tests.Connections
{
    using DrunkenBakery.OWAtray.Connections.Abstract;

    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="SecurityExtensions"/>. These exercise the platform's user-scoped data
    /// protection API (Windows DPAPI, or Mono's non-DPAPI-compatible equivalent on other platforms) —
    /// only round-tripping on the same machine/user is guaranteed, which is all the application relies
    /// on (a scenario file encrypted on one machine is never expected to decrypt on another).
    /// </summary>
    [TestFixture]
    public class SecurityExtensionsTests
    {
        [Test]
        public void EncryptThenDecrypt_RoundTripsOriginalValue()
        {
            const string Original = "correct-horse-battery-staple";

            var encrypted = Original.Encrypt();
            var decrypted = encrypted.Decrypt();

            Assert.AreEqual(Original, decrypted);
        }

        [Test]
        public void Encrypt_DoesNotReturnThePlainTextValue()
        {
            const string Original = "correct-horse-battery-staple";

            var encrypted = Original.Encrypt();

            Assert.AreNotEqual(Original, encrypted);
            StringAssert.DoesNotContain(Original, encrypted);
        }

        [Test]
        public void EncryptThenDecrypt_EmptyString_RoundTrips()
        {
            // Unlike the GUI and ShellIntegration copies of SecurityExtensions, this one (used by
            // AbstractConnection.Password) has no empty-string short-circuit, so it still calls into
            // the underlying data-protection API for an empty password. Locking in that it still
            // round-trips correctly rather than throwing.
            var encrypted = string.Empty.Encrypt();

            Assert.AreEqual(string.Empty, encrypted.Decrypt());
        }

        [Test]
        public void Encrypt_SameValueTwice_ProducesDifferentCipherText()
        {
            const string Original = "correct-horse-battery-staple";

            // DPAPI salts its output, so the same plaintext should not encrypt to the same bytes twice.
            var first = Original.Encrypt();
            var second = Original.Encrypt();

            Assert.AreNotEqual(first, second);
            Assert.AreEqual(Original, first.Decrypt());
            Assert.AreEqual(Original, second.Decrypt());
        }
    }
}
