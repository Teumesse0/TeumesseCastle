//
// Part of PingCastle 3.0.0.0 by Vincent Le Toux / Ping Castle SAS (https://www.pingcastle.com).
// Licensed under the Non-Profit OSL 3.0. See LICENSE.md in the project root for full license information.
//
// Modifications Copyright (c) 2026 Teumesse — TeumesseCastle project.
//
using TeumesseCastle.Healthcheck;

namespace TeumesseCastle.Report
{
    public class ReportHealthCheckSingleCompared : ReportHealthCheckSingle
    {
        HealthcheckData[] Reports;

        public string GenerateRawContent(HealthcheckData[] reports)
        {
            Reports = reports;
            reports[0].InitializeReportingData();
            reports[1].InitializeReportingData();
            Report = Reports[0];
            sb.Length = 0;
            GenerateContent();
            return sb.ToString();
        }

        protected override void GenerateSection(string title, GenerateContentDelegate generateContent)
        {
            Report = Reports[0];
            base.GenerateSection(title + " - " + Report.GenerationDate.ToString("u"), generateContent);
            Report = Reports[1];
            base.GenerateSection(title + " - " + Report.GenerationDate.ToString("u"), generateContent);
        }
    }
}
