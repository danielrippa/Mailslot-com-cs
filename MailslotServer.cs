using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using static Win32.Kernel32;

namespace Mailslot {

  [ComVisible(true)]
  [ClassInterface(ClassInterfaceType.AutoDispatch)]
  [Guid("A1B2C3D4-E5F6-7890-1234-567890ABCDEF")]
  [ProgId("Mailslot.Server")]

  public class Server {

    private IntPtr _mailslotHandle = IntPtr.Zero;
    private bool _isRunning = false;

    // Create mailslot server
    public bool CreateServer(string mailslotName = "Mailslot") {
      try {
        string fullName = "\\\\.\\mailslot\\" + mailslotName;
        
        _mailslotHandle = Win32.Kernel32.CreateMailslot(
          fullName,
          0, // Max message size (0 = no limit)
          MAILSLOT_WAIT_FOREVER, // Read timeout
          IntPtr.Zero // Security attributes
        );

        if (_mailslotHandle == INVALID_HANDLE_VALUE) {
          return false;
        }

        _isRunning = true;
        return true;
      } catch (Exception) {
        return false;
      }
    }





    // Stop the server
    public bool StopServer() {
      try {
        _isRunning = false;

        if (_mailslotHandle != IntPtr.Zero && _mailslotHandle != INVALID_HANDLE_VALUE) {
          CloseHandle(_mailslotHandle);
          _mailslotHandle = IntPtr.Zero;
        }

        return true;
      } catch (Exception) {
        return false;
      }
    }

    // Get server status
    public bool IsRunning() {
      return _isRunning;
    }

    // Check if there are messages waiting (SERVER READS)
    public bool HasMessages() {
      try {
        if (_mailslotHandle == IntPtr.Zero || _mailslotHandle == INVALID_HANDLE_VALUE) {
          return false;
        }

        uint maxMessageSize, nextSize, messageCount, readTimeout;
        
        if (GetMailslotInfo(_mailslotHandle, out maxMessageSize, out nextSize, out messageCount, out readTimeout)) {
          return messageCount > 0;
        }
        
        return false;
      } catch (Exception) {
        return false;
      }
    }

    // Get number of waiting messages (SERVER READS)
    public int GetMessageCount() {
      try {
        if (_mailslotHandle == IntPtr.Zero || _mailslotHandle == INVALID_HANDLE_VALUE) {
          return 0;
        }

        uint maxMessageSize, nextSize, messageCount, readTimeout;
        
        if (GetMailslotInfo(_mailslotHandle, out maxMessageSize, out nextSize, out messageCount, out readTimeout)) {
          return (int)messageCount;
        }
        
        return 0;
      } catch (Exception) {
        return 0;
      }
    }

    // Read a message from the mailslot (SERVER READS)
    public string ReadMessage() {
      try {
        if (_mailslotHandle == IntPtr.Zero || _mailslotHandle == INVALID_HANDLE_VALUE) {
          return "";
        }

        uint maxMessageSize, nextSize, messageCount, readTimeout;
        
        if (!GetMailslotInfo(_mailslotHandle, out maxMessageSize, out nextSize, out messageCount, out readTimeout)) {
          return "";
        }

        if (messageCount == 0 || nextSize == 0) {
          return "";
        }

        byte[] buffer = new byte[nextSize];
        uint bytesRead;
        
        if (ReadFile(_mailslotHandle, buffer, nextSize, out bytesRead, IntPtr.Zero)) {
          if (bytesRead > 0) {
            string message = Encoding.UTF8.GetString(buffer, 0, (int)bytesRead);
            return message.Trim();
          }
        }
        
        return "";
      } catch (Exception) {
        return "";
      }
    }

    // Read all waiting messages (SERVER READS)
    public string ReadAllMessages() {
      try {
        var messages = new System.Collections.Generic.List<string>();
        
        while (HasMessages()) {
          string message = ReadMessage();
          if (!string.IsNullOrEmpty(message)) {
            messages.Add(message);
          } else {
            break;
          }
        }
        
        if (messages.Count == 0) {
          return "";
        }
        
        return string.Join("|", messages.ToArray());
      } catch (Exception) {
        return "";
      }
    }

    // Destructor
    ~Server() {
      StopServer();
    }

  }
}