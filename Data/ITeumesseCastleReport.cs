//
// Copyright (c) Ping Castle. All rights reserved.
// https://www.pingcastle.com
//
// Licensed under the Non-Profit OSL. See LICENSE file in the project root for full license information.
//
// Modifications Copyright (c) 2026 Teumesse — TeumesseCastle project.
//
using System;
using System.Collections.Generic;

namespace TeumesseCastle.Data
{
    public interface ITeumesseCastleReport
    {
        DomainKey Domain { get; }
        DateTime GenerationDate { get; }
        string EngineVersion { get; }
        // used to get trusted domain and detect ubiquous name (ex: corp.local)
        IList<DomainKey> DomainKnown { get; }
        string GetHumanReadableFileName();
        string GetMachineReadableFileName();
        void SetExportLevel(TeumesseCastleReportDataExportLevel level);
        void SetIntegrity();
        void CheckIntegrity();
    }
}
