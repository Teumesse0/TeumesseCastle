//
// Copyright (c) Vincent LE TOUX for Ping Castle. All rights reserved.
// https://www.pingcastle.com
//
// Licensed under the Non-Profit OSL. See LICENSE file in the project root for full license information.
//
// Modifications Copyright (c) 2026 Teumesse — TeumesseCastle project.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeumesseCastle.Cloud
{
    class TeumesseCastleCloudException : Exception
    {
        public TeumesseCastleCloudException(string message)
            : base(message)
        {
        }
    }
}
