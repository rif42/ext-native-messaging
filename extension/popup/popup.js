let port = null;

// Wait for the DOM to be fully loaded
document.addEventListener('DOMContentLoaded', () => {
  // Get references to DOM elements
  const sendButton = document.getElementById('sendButton');
  const resultDiv = document.getElementById('result');

  // Add click event listener to the button
  sendButton.addEventListener('click', async () => {
    try {
      // Example: Send a message to the background script
      // const response = await sendMessageToBackground();
      // resultDiv.textContent = JSON.stringify(response);
      // resultDiv.style.display = 'block';

      const inputValue = document.getElementById('send-data');
      resultDiv.textContent = `data sent: ${inputValue.value}`;

      port = chrome.runtime.connectNative('com.nativemessaging.test');
      port.onDisconnect.addListener(() => {
        if (chrome.runtime.lastError) {
          console.log(chrome.runtime.lastError);
        }
      });

    } catch (error) {
      console.error('Error:', error);
      resultDiv.textContent = `Error: ${error.message}`;
      resultDiv.style.display = 'block';
    }
  });
});

// Function to send a message to the background script
// function sendMessageToBackground() {
//   return new Promise((resolve, reject) => {
//     chrome.runtime.sendMessage(
//       { action: 'popupAction', data: 'Hello from popup' },
//       (response) => {
//         if (chrome.runtime.lastError) {
//           reject(new Error(chrome.runtime.lastError.message));
//         } else {
//           resolve(response);
//         }
//       }
//     );
//   });
// } 