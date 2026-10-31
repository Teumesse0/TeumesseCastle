//
// Copyright (c) Ping Castle. All rights reserved.
// https://www.pingcastle.com
//
// Licensed under the Non-Profit OSL. See LICENSE file in the project root for full license information.
//
// Modifications Copyright (c) 2026 Teumesse — TeumesseCastle project.
//

namespace TeumesseCastle.Rules
{
    public interface IRuleScore
    {
        RiskRuleCategory Category { get; }
        RiskModelCategory Model { get; }
        string RiskId { get; }
        int Points { get; }
        
    }
}
