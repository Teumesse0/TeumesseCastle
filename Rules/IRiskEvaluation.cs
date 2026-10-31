//
// Copyright (c) Ping Castle. All rights reserved.
// https://www.pingcastle.com
//
// Licensed under the Non-Profit OSL. See LICENSE file in the project root for full license information.
//
// Modifications Copyright (c) 2026 Teumesse — TeumesseCastle project.
//
using System.Collections.Generic;

namespace TeumesseCastle.Rules
{
    public interface IRiskEvaluation
    {
        int GlobalScore { get; set; }
        int StaleObjectsScore { get; set; }
        int PrivilegiedGroupScore { get; set; }
        int TrustScore { get; set; }
        int AnomalyScore { get; set; }
        int MaturityLevel { get; set; }
        IList<IRuleScore> AllRiskRules { get; }
    }
}
