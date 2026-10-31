//
// Part of PingCastle 3.0.0.0 by Vincent Le Toux / Ping Castle SAS (https://www.pingcastle.com).
// Licensed under the Non-Profit OSL 3.0. See LICENSE.md in the project root for full license information.
//
// Modifications Copyright (c) 2026 Teumesse — TeumesseCastle project.
//
using PingCastle.Cloud.Data;
using PingCastle.Healthcheck;
using PingCastle.Rules;
using System;
using System.Collections.Generic;
using System.Text;

namespace PingCastle.Report
{
    public interface IActionPlan
    {
        void GenerateMainActionPlan(StringBuilder sb, Healthcheck.HealthcheckRiskRule rule, Rules.RuleBase<HealthcheckData> hcrule);

        void GenerateDetailledActionPlan(StringBuilder sb, HealthcheckRiskRule rule, Rules.RuleBase<HealthcheckData> hcrule, string d);
    }

    public interface IAADActionPlan
    {
        void GenerateMainActionPlan(StringBuilder sb, HealthCheckCloudDataRiskRule rule, RuleBase<HealthCheckCloudData> hcrule);
        void GenerateDetailledActionPlan(StringBuilder sb, HealthCheckCloudDataRiskRule rule, RuleBase<HealthCheckCloudData> hcrule, string d);

    }


    public interface IActionPlanConsolidation : IActionPlan
    {
        void LoadDomain(HealthcheckData data);
    }
}
