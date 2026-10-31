//
// Part of PingCastle 3.0.0.0 by Vincent Le Toux / Ping Castle SAS (https://www.pingcastle.com).
// Licensed under the Non-Profit OSL 3.0. See LICENSE.md in the project root for full license information.
//
// Modifications Copyright (c) 2026 Teumesse — TeumesseCastle project.
//
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace PingCastle.Exports
{
    public interface IExport
    {
        void Initialize(RuntimeSettings settings);
        void Export(string filename);
        string Name { get; }
        string Description { get; }
        DisplayState QueryForAdditionalParameterInInteractiveMode();
    }
}
