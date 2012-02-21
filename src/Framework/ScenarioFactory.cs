//------------------------------------------------------------------
// DrunkenBakery OWA Tray Monitor
// ScenarioFactory Class
//
// <copyright file="ScenarioFactory.cs" company="The Drunken Bakery">
//     Copyright (c) 2012 The Drunken Bakery. All rights reserved.
// </copyright>
//
// Factory class that knows how to build a scenario
//
//------------------------------------------------------------------

using DrunkenBakery.OWAtray.Connections.Abstract;

namespace DrunkenBakery.OWAtray.Framework
{
	public static class ScenarioFactory
	{
		public static Scenario CreateScenario(string filename)
		{
			var scenario = new Scenario {ScenarioFile = filename};

			// Initialise properties
			if (scenario.Connections == null) scenario.Connections = new EmailConnections();

			// Load from file
			scenario.Load(filename);

			return scenario;
		}
	}
}