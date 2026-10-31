//
// Part of PingCastle 3.0.0.0 by Vincent Le Toux / Ping Castle SAS (https://www.pingcastle.com).
// Licensed under the Non-Profit OSL 3.0. See LICENSE.md in the project root for full license information.
//
// Modifications Copyright (c) 2026 Teumesse — TeumesseCastle project.
//
using System.ComponentModel;

namespace PingCastle.Rules
{
    public enum RiskModelObjective
    {
        None = 0,
        [Description("House keeping")]
        StaleObjectsHouseKeeping = 110,
        [Description("Best practices")]
        PrivilegedBestPractices = 220,
        [Description("Trust permeability")]
        TrustPermeability = 310,
        [Description("Best practices")]
        TrustBestPractices = 320,
        [Description("Access to critical priority objects")]
        AnomalyAccessCritical = 410,
        [Description("Access to high priority objects")]
        AnomalyAccessHigh = 420,
        [Description("Access to medium priority objects")]
        AnomalyAccessMedium = 430,

    }
}
