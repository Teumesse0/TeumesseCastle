//
// Part of PingCastle 3.0.0.0 by Vincent Le Toux / Ping Castle SAS (https://www.pingcastle.com).
// Licensed under the Non-Profit OSL 3.0. See LICENSE.md in the project root for full license information.
//
// Modifications Copyright (c) 2026 Teumesse — TeumesseCastle project.
//
using System;
using System.Runtime.Serialization;

namespace TeumesseCastle
{
    [Serializable]
    public class TeumesseCastleException : Exception
    {
        public TeumesseCastleException()
        {
        }

        public TeumesseCastleException(string message)
            : base(message)
        {
        }

        public TeumesseCastleException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected TeumesseCastleException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
