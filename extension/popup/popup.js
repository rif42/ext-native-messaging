let port = null;

// Wait for the DOM to be fully loaded
document.addEventListener('DOMContentLoaded', () => {
  // Get references to DOM elements
  const sendButton = document.getElementById('sendButton');
  const resultDiv = document.getElementById('result');
  const receiveDataDiv = document.getElementById('receive-data');
  const inputValue = document.getElementById('send-data');

  // Connect to the native messaging host when the popup opens
  connectToNativeHost();

  // Add click event listener to the button
  sendButton.addEventListener('click', async () => {
    try {
      if (!port) {
        connectToNativeHost();
      }

      const message = inputValue.value;
      resultDiv.textContent = `Data sent: ${message}`;

      // Send message to native host
      port.postMessage({ text: message });
    } catch (error) {
      console.error('Error:', error);
      resultDiv.textContent = `Error: ${error.message}`;
    }
  });

  function connectToNativeHost() {
    try {
      port = chrome.runtime.connectNative('com.nativemessaging.test');

      port.onMessage.addListener((message) => {
        console.log('Received message from native host:', message);
        receiveDataDiv.textContent = JSON.stringify(message, null, 2);
      });

      port.onDisconnect.addListener(() => {
        if (chrome.runtime.lastError) {
          console.error('Connection error:', chrome.runtime.lastError);
          receiveDataDiv.textContent = `Connection error: ${chrome.runtime.lastError.message}`;
        }
        port = null;
      });
    } catch (error) {
      console.error('Failed to connect:', error);
      receiveDataDiv.textContent = `Failed to connect: ${error.message}`;
      port = null;
    }
  }
}); 