// ------------------------------------------------------------------
//  OWAtray
//  DrunkenBakery.OWAtray.Tests.Connections
//
//  <copyright file="AbstractConnectionTests.cs" company="The Drunken Bakery">
//      Copyright (c) 2009-2026 The Drunken Bakery. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------

namespace DrunkenBakery.OWAtray.Tests.Connections
{
    using DrunkenBakery.OWAtray.Connections.Abstract;

    using NUnit.Framework;

    [TestFixture]
    public class AbstractConnectionTests
    {
        [Test]
        public void Constructor_SetsExpectedDefaults()
        {
            var connection = new TestEmailConnection();

            Assert.AreEqual(string.Empty, connection.Username);
            Assert.AreEqual(string.Empty, connection.Password);
            Assert.AreEqual(string.Empty, connection.EmailAddress);
            Assert.AreEqual(EmailType.Exchange, connection.Type);
            Assert.AreEqual(ConnectionState.Disconnected, connection.ConnectedState);
            Assert.AreEqual(5, connection.Interval);
            Assert.AreEqual(string.Empty, connection.Description);
            Assert.AreEqual(string.Empty, connection.EmailServer);
            Assert.AreEqual(string.Empty, connection.AccountDomain);
            Assert.IsFalse(connection.OverrideServiceUrl);
            Assert.AreEqual(string.Empty, connection.ServiceUrl);
            Assert.IsFalse(connection.OverrideEmailUrl);
            Assert.AreEqual(string.Empty, connection.EmailUrl);
            Assert.IsTrue(connection.UseAutodiscovery);
            Assert.IsFalse(connection.OnWindowsDomain);
            Assert.IsFalse(connection.OverrideCertificate);
            Assert.IsTrue(connection.AlwaysUseInternetExplorer);
            Assert.IsFalse(connection.DisableCalendar);
            Assert.IsFalse(connection.AutoLogin);
            Assert.IsFalse(connection.OverrideOffice365Login);
            Assert.IsTrue(connection.OverrideAutodiscoveryValidation);
            Assert.AreEqual("Default", connection.ServerVersion);
            Assert.IsFalse(connection.Office365);
        }

        [Test]
        public void IsConnected_ReflectsConnectedState()
        {
            var connection = new TestEmailConnection { ConnectedState = ConnectionState.Disconnected };
            Assert.IsFalse(connection.IsConnected);

            connection.ConnectedState = ConnectionState.Connected;
            Assert.IsTrue(connection.IsConnected);

            connection.ConnectedState = ConnectionState.Failed;
            Assert.IsFalse(connection.IsConnected);
        }

        [Test]
        public void Password_RoundTripsThroughEncryptedStorage()
        {
            var connection = new TestEmailConnection { Password = "correct-horse-battery-staple" };

            Assert.AreEqual("correct-horse-battery-staple", connection.Password);
            Assert.IsNotEmpty(connection.EncryptedPassword);
            StringAssert.DoesNotContain("correct-horse-battery-staple", connection.EncryptedPassword);
        }

        [Test]
        public void Password_EmptyString_StaysEmpty()
        {
            var connection = new TestEmailConnection { Password = string.Empty };

            Assert.AreEqual(string.Empty, connection.Password);
            Assert.AreEqual(string.Empty, connection.EncryptedPassword);
        }

        [Test]
        public void EncryptedPassword_SetDirectly_IsReadBackUnchanged()
        {
            var connection = new TestEmailConnection { Password = "hunter2" };
            var encrypted = connection.EncryptedPassword;

            var other = new TestEmailConnection { EncryptedPassword = encrypted };

            Assert.AreEqual("hunter2", other.Password);
        }

        [Test]
        public void AreEventsDefined_ReflectsLogMessageSubscription()
        {
            var connection = new TestEmailConnection();
            Assert.IsFalse(connection.AreEventsDefined);

            connection.LogMessage += (message, severity) => { };
            Assert.IsTrue(connection.AreEventsDefined);
        }
    }
}
