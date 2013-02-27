// ------------------------------------------------------------------
//  OWAtray
//  DrunkenBakery.OWAtray.Framework.ScenarioFactory.cs
//  
//  <copyright file="ScenarioFactory.cs" company="The Drunken Bakery">
//      Copyright (c) 2009-2013 The Drunken Bakery. All rights reserved.
//  </copyright>
//  
//  Author: Iain Smith
// ------------------------------------------------------------------
namespace DrunkenBakery.OWAtray.Framework
{
    using DrunkenBakery.OWAtray.Connections.Abstract;

    /// <summary>
    ///     The scenario factory.
    /// </summary>
    public static class ScenarioFactory
    {
        #region Public Methods and Operators

        /// <summary>
        /// The create scenario.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <returns>
        /// The <see cref="Scenario"/>.
        /// </returns>
        public static Scenario CreateScenario(string filename)
        {
            var scenario = new Scenario { ScenarioFile = filename };

            // Initialise properties
            if (scenario.Connections == null)
            {
                scenario.Connections = new EmailConnections();
            }

            // Load from file
            scenario.Load(filename);

            return scenario;
        }

        #endregion
    }
}