using System;
using System.Runtime.InteropServices;
using System.Text;

using static Win32.Kernel32;

namespace Mailslot {

  [ComVisible(true)]
  [ClassInterface(ClassInterfaceType.AutoDispatch)]
  [Guid("B2C3D4E5-F6A7-8901-2345-6789ABCDEF01")]
  [ProgId("Mailslot.Client")]

  public class Client {

    private IntPtr _mailslotHandle = IntPtr.Zero;
    private string _mailslotName = "";

    // Connect to a mailslot
    public bool Connect(string mailslotName = "Mailslot") {
      try {
        _mailslotName = mailslotName;
        // Clients don't need to keep a handle open - they open/close for each message
        return true;
      } catch (Exception) {
        return false;
      }
    }

    // Send message to mailslot
    public bool SendMessage(string message) {
      try {
        string fullName = "\\\\.\\mailslot\\" + _mailslotName;
        
        IntPtr clientHandle = CreateFile(
          fullName,
          GENERIC_WRITE,
          FILE_SHARE_READ,
          IntPtr.Zero,
          OPEN_EXISTING,
          0,
          IntPtr.Zero
        );

        if (clientHandle == INVALID_HANDLE_VALUE) {
          return false;
        }

        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        uint bytesWritten;
        
        bool result = WriteFile(
          clientHandle,
          messageBytes,
          (uint)messageBytes.Length,
          out bytesWritten,
          IntPtr.Zero
        );

        CloseHandle(clientHandle);
        return result;
      } catch (Exception) {
        return false;
      }
    }

    // Disconnect from mailslot
    public bool Disconnect() {
      try {
        // Clients don't maintain persistent connections
        return true;
      } catch (Exception) {
        return false;
      }
    }

    // Get connection status
    public bool IsConnected() {
      return !string.IsNullOrEmpty(_mailslotName);
    }

    // Get last error code
    public int GetLastErrorCode() {
      return (int)GetLastError();
    }

    // Destructor
    ~Client() {
      Disconnect();
    }

  }
}