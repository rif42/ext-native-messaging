// Content script that runs on web pages matching the patterns in manifest.json
console.log('Content script loaded');

// Example: Send a message to the background service worker
// function sendMessageToBackground() {
//   chrome.runtime.sendMessage(
//     { action: 'contentScriptAction', data: 'Hello from content script' },
//     (response) => {
//       console.log('Response from background:', response);
//     }
//   );
// }

// Example: Execute code when the page is fully loaded
document.addEventListener('DOMContentLoaded', () => {
  console.log('DOM fully loaded');

  // Example of interacting with the page
  // Uncomment if needed
  /*
  const pageTitle = document.title;
  console.log('Page title:', pageTitle);
  */
}); 