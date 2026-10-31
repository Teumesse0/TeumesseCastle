//
// Part of PingCastle 3.0.0.0 by Vincent Le Toux / Ping Castle SAS (https://www.pingcastle.com).
// Licensed under the Non-Profit OSL 3.0. See LICENSE.md in the project root for full license information.
//
// Modifications Copyright (c) 2026 Teumesse — TeumesseCastle project.
//
using TeumesseCastle.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeumesseCastle.Cloud.Data
{
    public class AzureADKey
    {
        public string TenantName { get; set; }
        public string TenantID { get; set; }
        public bool IsComplete { get { return TenantID != null && TenantName != null; } }

        private AzureADKey()
        {
        }
        public static AzureADKey Create(string TenantName, string TenantID)
        {
            var key = new AzureADKey(TenantName, TenantID);
            return key;
        }

        protected AzureADKey(string tenantName, string tenantID)
        {
            if (!string.IsNullOrEmpty(tenantName))
            {
                TenantName = tenantName.ToLowerInvariant();
            }
            if (!string.IsNullOrEmpty(tenantID))
            {
                Guid guid;
                if (!Guid.TryParse(tenantID, out guid))
                {
                    Trace.WriteLine("Unable to parse the TenantID " + tenantID);
                    throw new TeumesseCastleException("Unable to parse the TenantID \"" + tenantID + "\" - it should be a guid");
                }
                TenantID = tenantID;
            }
        }
    }
}
