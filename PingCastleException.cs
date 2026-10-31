//
// Part of PingCastle 3.0.0.0 by Vincent Le Toux / Ping Castle SAS (https://www.pingcastle.com).
// Licensed under the Non-Profit OSL 3.0. See LICENSE.md in the project root for full license information.
//
// Modifications Copyright (c) 2026 Teumesse — TeumesseCastle project.
//
using System;
using System.Runtime.Serialization;

namespace PingCastle
{
    [Serializable]
    public class PingCastleException : Exception
    {
        public PingCastleException()
        {
        }

        public PingCastleException(string message)
            : base(message)
        {
        }

        public PingCastleException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected PingCastleException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
