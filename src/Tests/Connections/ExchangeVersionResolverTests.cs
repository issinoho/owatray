// ------------------------------------------------------------------
//  OWAtray
//  DrunkenBakery.OWAtray.Tests.Connections
//
//  <copyright file="ExchangeVersionResolverTests.cs" company="The Drunken Bakery">
//      Copyright (c) 2009-2026 The Drunken Bakery. All rights reserved.
//  </copyright>
// ------------------------------------------------------------------

namespace DrunkenBakery.OWAtray.Tests.Connections
{
    using DrunkenBakery.OWAtray.Connections.EWS;

    using NUnit.Framework;

    [TestFixture]
    public class ExchangeVersionResolverTests
    {
        [TestCase("Exchange2016", "Exchange2013_SP1")]
        [TestCase("Exchange2019", "Exchange2013_SP1")]
        [TestCase("ExchangeServerSE", "Exchange2013_SP1")]
        [TestCase("Exchange2010_SP3", "Exchange2010_SP2")]
        public void ResolveWireVersion_AliasedVersion_ReturnsWireCompatibleVersion(
            string selectedVersion, string expectedWireVersion)
        {
            Assert.AreEqual(expectedWireVersion, ExchangeVersionResolver.ResolveWireVersion(selectedVersion));
        }

        [TestCase("Default")]
        [TestCase("Exchange2007_SP1")]
        [TestCase("Exchange2010")]
        [TestCase("Exchange2010_SP1")]
        [TestCase("Exchange2010_SP2")]
        [TestCase("Exchange2013")]
        [TestCase("Exchange2013_SP1")]
        public void ResolveWireVersion_UnaliasedVersion_PassesThroughUnchanged(string selectedVersion)
        {
            Assert.AreEqual(selectedVersion, ExchangeVersionResolver.ResolveWireVersion(selectedVersion));
        }

        [TestCase("Exchange2016", "Exchange2013_SP1", "Exchange2016")]
        [TestCase("Exchange2019", "Exchange2013_SP1", "Exchange2019")]
        [TestCase("ExchangeServerSE", "Exchange2013_SP1", "ExchangeServerSE")]
        [TestCase("Exchange2010_SP3", "Exchange2010_SP2", "Exchange2010_SP3")]
        public void ResolveDisplayVersion_CandidateMatchesAlias_PreservesUserSelection(
            string selectedVersion, string candidateVersion, string expectedDisplayVersion)
        {
            Assert.AreEqual(
                expectedDisplayVersion,
                ExchangeVersionResolver.ResolveDisplayVersion(selectedVersion, candidateVersion));
        }

        [Test]
        public void ResolveDisplayVersion_CandidateDiffersFromAlias_ReturnsCandidateUnchanged()
        {
            // e.g. a real Exchange 2016 server actually reports a version string that isn't the
            // "Exchange2013_SP1" alias target (this can happen once connected) — the real, more
            // specific value should win over the user's original selection.
            var result = ExchangeVersionResolver.ResolveDisplayVersion("Exchange2016", "Exchange2010_SP1");

            Assert.AreEqual("Exchange2010_SP1", result);
        }

        [Test]
        public void ResolveDisplayVersion_UnaliasedSelection_ReturnsCandidateUnchanged()
        {
            var result = ExchangeVersionResolver.ResolveDisplayVersion("Exchange2013_SP1", "Exchange2013_SP1");

            Assert.AreEqual("Exchange2013_SP1", result);
        }
    }
}
