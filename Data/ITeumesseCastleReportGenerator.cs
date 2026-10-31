//
// Part of PingCastle 3.0.0.0 by Vincent Le Toux / Ping Castle SAS (https://www.pingcastle.com).
// Licensed under the Non-Profit OSL 3.0. See LICENSE.md in the project root for full license information.
//
// Modifications Copyright (c) 2026 Teumesse — TeumesseCastle project.
//
using System.Collections.Generic;
using System.Net;

namespace TeumesseCastle.Data
{

    public class TeumesseCastleAnalyzerParameters
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public NetworkCredential Credential { get; set; }
        public bool PerformExtendedTrustDiscovery { get; set; }
        public List<string> AdditionalNamesForDelegationAnalysis { get; set; }
    }

    public interface ITeumesseCastleAnalyzer<T> where T : ITeumesseCastleReport
    {
        T PerformAnalyze(TeumesseCastleAnalyzerParameters parameters);
    }
}
