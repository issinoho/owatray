// ------------------------------------------------------------------
//  OWAtray
//  DrunkenBakery.OWAtray.Tests.Framework
//
//  <copyright file="ScenarioTests.cs" company="The Drunken Bakery">
//      Copyright (c) 2009-2026 The Drunken Bakery. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------

namespace DrunkenBakery.OWAtray.Tests.Framework
{
    using System;
    using System.IO;

    using DrunkenBakery.OWAtray.Connections.Abstract;
    using DrunkenBakery.OWAtray.Connections.EWS;
    using DrunkenBakery.OWAtray.Framework;

    using NUnit.Framework;

    [TestFixture]
    public class ScenarioTests
    {
        private string scenarioFile;

        [SetUp]
        public void SetUp()
        {
            this.scenarioFile = Path.Combine(Path.GetTempPath(), "owatray-scenario-test-" + Guid.NewGuid() + ".xml");
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(this.scenarioFile))
            {
                File.Delete(this.scenarioFile);
            }
        }

        [Test]
        public void SaveThenLoad_RoundTripsConnectionProperties()
        {
            var original = new EwsConnection
            {
                Username = "jdoe",
                EmailAddress = "jdoe@example.com",
                Password = "hunter2",
                Interval = 15,
                EmailServer = "mail.example.com",
                AccountDomain = "EXAMPLE",
                Description = "Work account",
                UseAutodiscovery = false,
                OverrideServiceUrl = true,
                ServiceUrl = "https://mail.example.com/EWS/Exchange.asmx",
                OverrideEmailUrl = true,
                EmailUrl = "https://mail.example.com/owa",
                OnWindowsDomain = true,
                OverrideCertificate = true,
                AlwaysUseInternetExplorer = false,
                DisableCalendar = true,
                AutoLogin = true,
                Office365 = false,
                OverrideOffice365Login = false,
                OverrideAutodiscoveryValidation = false,
                ServerVersion = "Exchange2016"
            };

            var saved = new Scenario { Connections = new EmailConnections(), ScenarioFile = this.scenarioFile };
            saved.Connections.Add(original);
            saved.Save();

            Assert.IsTrue(File.Exists(this.scenarioFile));

            var loaded = new Scenario { Connections = new EmailConnections(), ScenarioFile = this.scenarioFile };
            loaded.Load();

            Assert.AreEqual(1, loaded.Connections.Count);
            var roundTripped = loaded.Connections[0];

            Assert.AreEqual(original.EmailAddress, roundTripped.EmailAddress);
            Assert.AreEqual(original.Username, roundTripped.Username);
            Assert.AreEqual(original.Password, roundTripped.Password);
            Assert.AreEqual(original.Interval, roundTripped.Interval);
            Assert.AreEqual(original.EmailServer, roundTripped.EmailServer);
            Assert.AreEqual(original.AccountDomain, roundTripped.AccountDomain);
            Assert.AreEqual(original.Description, roundTripped.Description);
            Assert.AreEqual(original.UseAutodiscovery, roundTripped.UseAutodiscovery);
            Assert.AreEqual(original.OverrideServiceUrl, roundTripped.OverrideServiceUrl);
            Assert.AreEqual(original.ServiceUrl, roundTripped.ServiceUrl);
            Assert.AreEqual(original.OverrideEmailUrl, roundTripped.OverrideEmailUrl);
            Assert.AreEqual(original.EmailUrl, roundTripped.EmailUrl);
            Assert.AreEqual(original.OnWindowsDomain, roundTripped.OnWindowsDomain);
            Assert.AreEqual(original.OverrideCertificate, roundTripped.OverrideCertificate);
            Assert.AreEqual(original.AlwaysUseInternetExplorer, roundTripped.AlwaysUseInternetExplorer);
            Assert.AreEqual(original.DisableCalendar, roundTripped.DisableCalendar);
            Assert.AreEqual(original.AutoLogin, roundTripped.AutoLogin);
            Assert.AreEqual(original.Office365, roundTripped.Office365);
            Assert.AreEqual(original.OverrideOffice365Login, roundTripped.OverrideOffice365Login);
            Assert.AreEqual(original.OverrideAutodiscoveryValidation, roundTripped.OverrideAutodiscoveryValidation);
            Assert.AreEqual("Exchange2016", roundTripped.ServerVersion);
        }

        [Test]
        public void SaveThenLoad_MultipleConnections_PreservesCount()
        {
            var scenario = new Scenario { Connections = new EmailConnections(), ScenarioFile = this.scenarioFile };
            scenario.Connections.Add(new EwsConnection { EmailAddress = "one@example.com" });
            scenario.Connections.Add(new EwsConnection { EmailAddress = "two@example.com" });
            scenario.Save();

            var loaded = new Scenario { Connections = new EmailConnections(), ScenarioFile = this.scenarioFile };
            loaded.Load();

            Assert.AreEqual(2, loaded.Connections.Count);
            Assert.AreEqual("one@example.com", loaded.Connections[0].EmailAddress);
            Assert.AreEqual("two@example.com", loaded.Connections[1].EmailAddress);
        }

        [Test]
        public void Load_NonExistentFile_LeavesConnectionsUntouched()
        {
            var scenario = new Scenario
            {
                Connections = new EmailConnections { new EwsConnection { EmailAddress = "existing@example.com" } },
                ScenarioFile = Path.Combine(Path.GetTempPath(), "owatray-scenario-does-not-exist-" + Guid.NewGuid() + ".xml")
            };

            scenario.Load();

            Assert.AreEqual(1, scenario.Connections.Count);
            Assert.AreEqual("existing@example.com", scenario.Connections[0].EmailAddress);
        }

        [Test]
        public void Load_ExistingFile_ClearsPreviousConnections()
        {
            var saved = new Scenario { Connections = new EmailConnections(), ScenarioFile = this.scenarioFile };
            saved.Connections.Add(new EwsConnection { EmailAddress = "saved@example.com" });
            saved.Save();

            var reloaded = new Scenario
            {
                Connections = new EmailConnections { new EwsConnection { EmailAddress = "stale@example.com" } },
                ScenarioFile = this.scenarioFile
            };
            reloaded.Load();

            Assert.AreEqual(1, reloaded.Connections.Count);
            Assert.AreEqual("saved@example.com", reloaded.Connections[0].EmailAddress);
        }

        [Test]
        public void ScenarioFactory_CreateScenario_LoadsFromFile()
        {
            var saved = new Scenario { Connections = new EmailConnections(), ScenarioFile = this.scenarioFile };
            saved.Connections.Add(new EwsConnection { EmailAddress = "factory@example.com" });
            saved.Save();

            var scenario = ScenarioFactory.CreateScenario(this.scenarioFile);

            Assert.IsNotNull(scenario.Connections);
            Assert.AreEqual(1, scenario.Connections.Count);
            Assert.AreEqual("factory@example.com", scenario.Connections[0].EmailAddress);
        }
    }
}
