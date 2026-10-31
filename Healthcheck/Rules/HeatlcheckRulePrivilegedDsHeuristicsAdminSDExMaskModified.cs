//
// Copyright (c) Ping Castle. All rights reserved.
// https://www.pingcastle.com
//
// Licensed under the Non-Profit OSL. See LICENSE file in the project root for full license information.
//
// Modifications Copyright (c) 2026 Teumesse — TeumesseCastle project.
//
using TeumesseCastle.Rules;

namespace TeumesseCastle.Healthcheck.Rules
{
    [RuleModel("P-DsHeuristicsAdminSDExMask", RiskRuleCategory.PrivilegedAccounts, RiskModelCategory.ACLCheck)]
    [RuleComputation(RuleComputationType.TriggerOnPresence, 5)]
    [RuleIntroducedIn(2, 7)]
    [RuleDurANSSI(1, "dsheuristics_bad", "Dangerous dsHeuristics settings")]
    [RuleMitreAttackMitigation(MitreAttackMitigation.PrivilegedAccountManagement)]
    public class HeatlcheckRulePrivilegedDsHeuristicsAdminSDExMaskModified : RuleBase<HealthcheckData>
    {
        protected override int? AnalyzeDataNew(HealthcheckData healthcheckData)
        {
            if (healthcheckData.DSHeuristics != null && healthcheckData.DSHeuristics.Length >= 16 && healthcheckData.DSHeuristics.Substring(15, 1) != "0")
            {
                return 1;
            }
            return 0;
        }
    }
}
