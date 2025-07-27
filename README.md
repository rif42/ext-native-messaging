# Chrome Native Messaging with C# Broker

This project demonstrates native messaging between a Chrome extension and a C# broker application, which can optionally communicate with a WPF application.

## Project Structure

- `extension/`: Chrome extension files
- `broker/`: C# broker application that handles native messaging
- `com.nativemessaging.test.json`: Native messaging host manifest file

## Setup Instructions

### 1. Build the Broker Application

1. Open the broker project in Visual Studio
2. Build the solution to produce `broker.exe`
3. Note the location of the executable

### 2. Register the Native Messaging Host

1. Edit `com.nativemessaging.test.json` to update the path to the absolute path of your broker.exe
2. Update the `allowed_origins` field with your Chrome extension ID
3. Register the manifest by adding it to the Windows registry:

For user-level installation (PowerShell):
```powershell
$manifestPath = "FULL_PATH_TO_YOUR_MANIFEST_FILE"
New-Item -Path "HKCU:\Software\Google\Chrome\NativeMessagingHosts\com.nativemessaging.test" -Force
Set-ItemProperty -Path "HKCU:\Software\Google\Chrome\NativeMessagingHosts\com.nativemessaging.test" -Name "(Default)" -Value $manifestPath
```

For system-level installation (requires admin, PowerShell):
```powershell
$manifestPath = "FULL_PATH_TO_YOUR_MANIFEST_FILE"
New-Item -Path "HKLM:\Software\Google\Chrome\NativeMessagingHosts\com.nativemessaging.test" -Force
Set-ItemProperty -Path "HKLM:\Software\Google\Chrome\NativeMessagingHosts\com.nativemessaging.test" -Name "(Default)" -Value $manifestPath
```

### 3. Install the Chrome Extension

1. Open Chrome and navigate to `chrome://extensions/`
2. Enable "Developer mode"
3. Click "Load unpacked" and select the `extension` folder
4. Note the extension ID and update it in your native messaging host manifest file if needed

## Usage

1. Open the Chrome extension popup
2. Enter a message in the input field and click "Send"
3. The message will be sent to the broker application, which will:
   - Try to forward it to the WPF app if available
   - Return a response that will be displayed in the extension popup

## Troubleshooting

If the extension cannot connect to the native messaging host:

1. Check if the broker is running (it will be launched automatically by Chrome)
2. Verify the manifest file is correctly registered in the registry
3. Ensure the extension ID in the manifest matches your actual extension ID
4. Check `broker_log.txt` in the broker directory for error messages 