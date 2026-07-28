// ------------------------------------------------------------------
//  DrunkenBakery OWA Tray Monitor
//  OWAtray.DrunkenBakery.OWAtray.Connections.EWS
//
//  <copyright file="ExchangeVersionResolver.cs" company="The Drunken Bakery">
//      Copyright (c) 2009-2026 The Drunken Bakery. All rights reserved.
//  </copyright>
//
//  Author: Iain Smith
// ------------------------------------------------------------------

namespace DrunkenBakery.OWAtray.Connections.EWS
{
    using System.Collections.Generic;

    /// <summary>
    /// Resolves GUI-selectable server versions onto the EWS Managed API's <c>ExchangeVersion</c> enum,
    /// for versions that have no distinct enum value of their own.
    /// </summary>
    public static class ExchangeVersionResolver
    {
        #region Constants and Fields

        /// <summary>
        /// Maps GUI-selectable server versions that have no distinct <c>ExchangeVersion</c> enum value
        /// onto the closest wire-compatible one. Exchange 2016, 2019 and Server SE do not change the EWS
        /// schema over Exchange 2013 SP1, and the bundled EWS Managed API predates all three, so they
        /// negotiate using the 2013 SP1 schema.
        /// </summary>
        private static readonly Dictionary<string, string> WireVersionAliases = new Dictionary<string, string>
        {
            { "Exchange2010_SP3", "Exchange2010_SP2" },
            { "Exchange2016", "Exchange2013_SP1" },
            { "Exchange2019", "Exchange2013_SP1" },
            { "ExchangeServerSE", "Exchange2013_SP1" },
        };

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Resolves the version to pass to the EWS Managed API for a GUI-selected server version,
        /// substituting the closest wire-compatible <c>ExchangeVersion</c> enum name for versions that
        /// have none of their own. Unrecognised versions (including "Default") are passed through
        /// unchanged.
        /// </summary>
        /// <param name="selectedVersion">
        /// The version selected by the user (<see cref="DrunkenBakery.OWAtray.Connections.Abstract.AbstractConnection.ServerVersion"/>).
        /// </param>
        /// <returns>
        /// The wire-compatible version name to parse as an <c>ExchangeVersion</c> enum value.
        /// </returns>
        public static string ResolveWireVersion(string selectedVersion)
        {
            string wireVersion;
            return WireVersionAliases.TryGetValue(selectedVersion, out wireVersion) ? wireVersion : selectedVersion;
        }

        /// <summary>
        /// Resolves the version to display/persist for a connection, preferring the user's original
        /// selection over a candidate version (typically derived from the server, or from
        /// <see cref="ResolveWireVersion"/>) whenever that candidate is exactly what the selected
        /// version aliases onto. Otherwise the candidate is returned unchanged.
        /// </summary>
        /// <param name="selectedVersion">
        /// The version selected by the user (<see cref="DrunkenBakery.OWAtray.Connections.Abstract.AbstractConnection.ServerVersion"/>).
        /// </param>
        /// <param name="candidateVersion">
        /// The version that would otherwise be displayed.
        /// </param>
        /// <returns>
        /// <paramref name="selectedVersion"/> if it aliases onto <paramref name="candidateVersion"/>,
        /// otherwise <paramref name="candidateVersion"/> unchanged.
        /// </returns>
        public static string ResolveDisplayVersion(string selectedVersion, string candidateVersion)
        {
            string aliasedVersion;
            if (WireVersionAliases.TryGetValue(selectedVersion, out aliasedVersion) && candidateVersion == aliasedVersion)
            {
                return selectedVersion;
            }

            return candidateVersion;
        }

        #endregion
    }
}
