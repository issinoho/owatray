//------------------------------------------------------------------
// DrunkenBakery OWA Tray Monitor
// EmailType Class
//
// <copyright file="EmailType.cs" company="The Drunken Bakery">
//     Copyright (c) 2012 The Drunken Bakery. All rights reserved.
// </copyright>
//
// Lists the various email connection types that are supported
//
//------------------------------------------------------------------

using DrunkenBakery.OWAtray.Connections.Abstract.Properties;

namespace DrunkenBakery.OWAtray.Connections.Abstract
{
	public enum EmailType
	{
		Exchange
	}

    public static partial class EnumExtensions
    {
        public static string Description(this EmailType e)
        {
            var state = "";

            switch (e)
            {
                case EmailType.Exchange:
                    state = Resources.EnumExtensions_Description_Exchange;
                    break;
            }

            return state;
        }
    }

}