//
// Part of PingCastle 3.0.0.0 by Vincent Le Toux / Ping Castle SAS (https://www.pingcastle.com).
// Licensed under the Non-Profit OSL 3.0. See LICENSE.md in the project root for full license information.
//
// Modifications Copyright (c) 2026 Teumesse — TeumesseCastle project.
//
using System.Collections.Generic;

namespace TeumesseCastle.Bot
{
    public class BotInputOutput
    {
        public List<BotData> Data { get; set; }

    }
    public class BotData
    {
        public BotData()
        {

        }
        public BotData(string Key, string Value) : this()
        {
            this.Key = Key;
            this.Value = Value;
        }

        public string Key { get; set; }
        public string Value { get; set; }
    }
}
