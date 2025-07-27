// Background service worker for Chrome extension
console.log('Background service worker initialized');

// Example: Listen for installation event
chrome.runtime.onInstalled.addListener((details) => {
  if (details.reason === 'install') {
    console.log('Extension installed');
    // Test native messaging connection on install
    testNativeConnection();
  } else if (details.reason === 'update') {
    console.log(`Extension updated from ${details.previousVersion} to ${chrome.runtime.getManifest().version}`);
  }
});

// Example: Message handling between extension components
chrome.runtime.onMessage.addListener((message, sender, sendResponse) => {
  console.log('Received message:', message);
  
  if (message.action === 'sendToNative') {
    // Forward message to native app
    chrome.runtime.sendNativeMessage('com.nativemessaging.test', 
      message.data,
      (response) => {
        if (chrome.runtime.lastError) {
          console.error('Error sending native message:', chrome.runtime.lastError);
          sendResponse({ error: chrome.runtime.lastError.message });
        } else {
          console.log('Native response:', response);
          sendResponse({ response: response });
        }
      }
    );
    return true; // Indicates asynchronous response
  }
  
  // Default response
  sendResponse({ status: 'Message received by background service worker' });
  return true;
});

// Function to test native connection
function testNativeConnection() {
  try {
    chrome.runtime.sendNativeMessage(
      'com.nativemessaging.test',
      { action: 'ping', data: 'Testing connection' },
      (response) => {
        if (chrome.runtime.lastError) {
          console.error('Native connection test failed:', chrome.runtime.lastError);
        } else {
          console.log('Native connection successful:', response);
        }
      }
    );
  } catch (error) {
    console.error('Exception testing native connection:', error);
  }
} 