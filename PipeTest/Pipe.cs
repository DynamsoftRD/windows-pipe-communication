using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace PipeCommunication
{
    [Flags]
    public enum PipeOpenModeFlags : uint
    {
        PIPE_ACCESS_DUPLEX = 0x00000003,
        PIPE_ACCESS_INBOUND = 0x00000001,
        PIPE_ACCESS_OUTBOUND = 0x00000002,
        FILE_FLAG_FIRST_PIPE_INSTANCE = 0x00080000,
        FILE_FLAG_WRITE_THROUGH = 0x80000000,
        FILE_FLAG_OVERLAPPED = 0x40000000,
        WRITE_DAC = 0x00040000,
        WRITE_OWNER = 0x00080000,
        ACCESS_SYSTEM_SECURITY = 0x01000000
    }

    [Flags]
    public enum PipeModeFlags : uint
    {
        //One of the following type modes can be specified. The same type mode must be specified for each instance of the pipe.
        PIPE_TYPE_BYTE = 0x00000000,
        PIPE_TYPE_MESSAGE = 0x00000004,
        //One of the following read modes can be specified. Different instances of the same pipe can specify different read modes
        PIPE_READMODE_BYTE = 0x00000000,
        PIPE_READMODE_MESSAGE = 0x00000002,
        //One of the following wait modes can be specified. Different instances of the same pipe can specify different wait modes.
        PIPE_WAIT = 0x00000000,
        PIPE_NOWAIT = 0x00000001,
        //One of the following remote-client modes can be specified. Different instances of the same pipe can specify different remote-client modes.
        PIPE_ACCEPT_REMOTE_CLIENTS = 0x00000000,
        PIPE_REJECT_REMOTE_CLIENTS = 0x00000008
    }

    [Flags]
    public enum DesireMode : uint
    {
        GENERIC_READ = 0x80000000,
        GENERIC_WRITE = 0x40000000
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SECURITY_ATTRIBUTES
    {
        public uint nLength;
        public IntPtr lpSecurityDescriptor;
        public bool bInheritHandle;
    }

    public class Pipe
    {
        public readonly static IntPtr INVALID_HANDLE = new IntPtr(-1);

        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateNamedPipe(
            string lpName,
            uint dwOpenMode,
            uint dwPipeMode,
            uint uiMaxInstances,
            uint uiOutBufferSize,
            uint uiInBufferSize,
            uint uiDefaultTimeout,
            /*[In] ref SECURITY_ATTRIBUTES */ IntPtr lpSecurityAttributes);

        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateFile(
            string lpName,
            uint uiDesireAccess,
            uint uiShareMode,
            /*[In] ref SECURITY_ATTRIBUTES*/ IntPtr lpSecurityAttributes,
            uint uiCreationDisposition,
            uint uiFlagsAndAttributes,
            IntPtr hTemplateFile);

        [DllImport("kernel32.dll")]
        public static extern bool ConnectNamedPipe(
            IntPtr hHandle,
            [In] ref NativeOverlapped lpOverlapped);

        [DllImport("kernel32.dll")]
        public static extern bool ReadFile(
            IntPtr hHandle, 
            [Out] byte[] arybuffer, 
            uint uiNumberOfBytesToRead, 
            [Out] uint uiNumberOfBytesRead, 
            [In] ref NativeOverlapped lpOverlapped);

        [DllImport("kernel32.dll")]
        public static extern bool WriteFile(
            IntPtr hHandle, 
            byte[] aryBuffer, 
            uint uiNumberOfBytesToWrite, 
            [Out] uint uiNumberOfBytesWritten, 
            [In] ref NativeOverlapped lpOverlapped);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hHandle);

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();
    }
}
