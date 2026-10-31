//
// Part of PingCastle 3.0.0.0 by Vincent Le Toux / Ping Castle SAS (https://www.pingcastle.com).
// Licensed under the Non-Profit OSL 3.0. See LICENSE.md in the project root for full license information.
//
// Modifications Copyright (c) 2026 Teumesse — TeumesseCastle project.
//
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace TeumesseCastle.Bot
{
    internal class BotStream
    {
        [DllImport("kernel32.dll", EntryPoint = "CreateFile", SetLastError = true)]
        private static extern IntPtr CreateFile(String lpFileName,
             UInt32 dwDesiredAccess, UInt32 dwShareMode,
             IntPtr lpSecurityAttributes, UInt32 dwCreationDisposition,
             UInt32 dwFlagsAndAttributes,
             IntPtr hTemplateFile);


        private const UInt32 GENERIC_READ = 0x80000000;
        private const UInt32 GENERIC_WRITE = 0x40000000;
        private const UInt32 OPEN_EXISTING = 3;

        public static FileStream OpenPipeStream(string pipeName)
        {
            Console.WriteLine("Opening pipe :" + pipeName);
            IntPtr p = CreateFile(@"\\.\pipe\" + pipeName, GENERIC_READ + GENERIC_WRITE, 0, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
            if (p.ToInt32() == -1)
            {
                throw new Win32Exception();
            }
            return new FileStream(new Microsoft.Win32.SafeHandles.SafeFileHandle(p, true), FileAccess.ReadWrite);
        }
    }
}
