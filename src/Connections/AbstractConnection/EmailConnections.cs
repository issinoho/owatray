// ------------------------------------------------------------------
//  DrunkenBakery OWA Tray Monitor
//  OWAtray.DrunkenBakery.OWAtray.Connections.Abstract
//
//  <copyright file="EmailConnections.cs" company="The Drunken Bakery">
//      Copyright (c) 2009-2012 The Drunken Bakery. All rights reserved.
//  </copyright>
//
//  Author: IRS
// ------------------------------------------------------------------

namespace DrunkenBakery.OWAtray.Connections.Abstract
{
    using System.Collections.Generic;

    /// <summary>
    /// The email connections.
    /// </summary>
    public class EmailConnections : List<IEmailInterface>
    {
    }
}