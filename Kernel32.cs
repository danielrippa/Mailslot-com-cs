using System;
using System.Runtime.InteropServices;

namespace Win32 {

  internal static class Kernel32 {

    private const string Dll = "kernel32.dll";

    // P/Invoke declarations for mailslot functions
    [DllImport(Dll, SetLastError = true)]
    public static extern IntPtr CreateFile(
      string lpFileName,
      uint dwDesiredAccess,
      uint dwShareMode,
      IntPtr lpSecurityAttributes,
      uint dwCreationDisposition,
      uint dwFlagsAndAttributes,
      IntPtr hTemplateFile);

    [DllImport(Dll, SetLastError = true)]
    public static extern bool ReadFile(
      IntPtr hFile,
      byte[] lpBuffer,
      uint nNumberOfBytesToRead,
      out uint lpNumberOfBytesRead,
      IntPtr lpOverlapped);

    [DllImport(Dll, SetLastError = true)]
    public static extern bool CloseHandle(IntPtr hObject);

    [DllImport(Dll, SetLastError = true)]
    public static extern bool GetMailslotInfo(
      IntPtr hMailslot,
      out uint lpMaxMessageSize,
      out uint lpNextSize,
      out uint lpMessageCount,
      out uint lpReadTimeout);

    [DllImport(Dll)]
    public static extern uint GetLastError();

    public const uint GENERIC_READ = 0x80000000;
    public const uint GENERIC_WRITE = 0x40000000;
    public const uint OPEN_EXISTING = 3;
    public const uint FILE_SHARE_READ = 0x00000001;
    public const uint FILE_SHARE_WRITE = 0x00000002;
    public static readonly IntPtr INVALID_HANDLE_VALUE = (IntPtr)(-1);
    public const uint ERROR_NO_MORE_FILES = 18;
    public const uint MAILSLOT_WAIT_FOREVER = 0xFFFFFFFF;

    [DllImport(Dll, SetLastError = true)]
    public static extern IntPtr CreateMailslot(
      string lpName,
      uint nMaxMessageSize,
      uint lReadTimeout,
      IntPtr lpSecurityAttributes
    );

    [DllImport(Dll, SetLastError = true)]
    public static extern bool WriteFile(
      IntPtr hFile,
      byte[] lpBuffer,
      uint nNumberOfBytesToWrite,
      out uint lpNumberOfBytesWritten,
      IntPtr lpOverlapped);

  }

}