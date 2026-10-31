using TeumesseCastle.Graph.Reporting;
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
    [RuleModel("P-SchemaAdmin", RiskRuleCategory.PrivilegedAccounts, RiskModelCategory.IrreversibleChange)]
    [RuleComputation(RuleComputationType.TriggerOnPresence, 10)]
    [RuleSTIG("V-72835", "Membership to the Schema Admins group must be limited", STIGFramework.Forest)]
    [RuleANSSI("R13", "subsection.3.2")]
    [RuleMaturityLevel(3)]
    [RuleMitreAttackMitigation(MitreAttackMitigation.PrivilegedAccountManagement)]
    public class HeatlcheckRulePrivilegedSchemaAdmins : RuleBase<HealthcheckData>
    {
        protected override int? AnalyzeDataNew(HealthcheckData healthcheckData)
        {
            foreach (HealthCheckGroupData group in healthcheckData.PrivilegedGroups)
            {
                if (group.GroupName == GraphObjectReference.SchemaAdministrators)
                {
                    if (group.NumberOfMember > 0)
                    {
                        return group.NumberOfMember;
                    }
                }
            }
            return 0;
        }
    }
}
