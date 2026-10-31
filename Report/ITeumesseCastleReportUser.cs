//
// Part of PingCastle 3.0.0.0 by Vincent Le Toux / Ping Castle SAS (https://www.pingcastle.com).
// Licensed under the Non-Profit OSL 3.0. See LICENSE.md in the project root for full license information.
//
// Modifications Copyright (c) 2026 Teumesse — TeumesseCastle project.
//
using TeumesseCastle.Cloud.Data;
using TeumesseCastle.Data;

namespace TeumesseCastle.Report
{
    public delegate string GetUrlDelegateDomain(DomainKey domainKey, string displayName, string optionalRiskId);
    public delegate string GetUrlDelegateAzureAD(AzureADKey domainKey, string displayName, string optionalRiskId);
    public delegate string GetAdditionInfoDelegate(DomainKey domainKey);
    public delegate string AddHtmlToTabSection(string section);

    public interface ITeumesseCastleReportUser<T> where T : ITeumesseCastleReport
    {
        string GenerateReportFile(T report, ADHealthCheckingLicense license, string filename);
        string GenerateRawContent(T report);
        void SetUrlDisplayDelegate(GetUrlDelegateDomain uRLDelegate);
        void SetUrlDisplayDelegate(GetUrlDelegateAzureAD uRLDelegate);
    }
}
