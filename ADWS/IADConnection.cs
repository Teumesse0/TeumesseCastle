//
// Part of PingCastle 3.0.0.0 by Vincent Le Toux / Ping Castle SAS (https://www.pingcastle.com).
// Licensed under the Non-Profit OSL 3.0. See LICENSE.md in the project root for full license information.
//
// Modifications Copyright (c) 2026 Teumesse — TeumesseCastle project.
//
using System.Security.Principal;

namespace PingCastle.ADWS
{
    public interface IADConnection
    {
        ADDomainInfo GetDomainInfo();

        void Enumerate(string distinguishedName, string filter, string[] properties, WorkOnReturnedObjectByADWS callback, string scope);

        string ConvertSIDToName(string sidstring, out string referencedDomain);

        SecurityIdentifier ConvertNameToSID(string nameToResolve);

        IFileConnection FileConnection { get; }

        void ThreadInitialization();

    }
}
