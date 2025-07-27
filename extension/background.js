// Background service worker for Chrome extension
console.log('Background service worker initialized');

// Example: Listen for installation event
chrome.runtime.onInstalled.addListener((details) => {
  if (details.reason === 'install') {
    console.log('Extension installed');
  } else if (details.reason === 'update') {
    console.log(`Extension updated from ${details.previousVersion} to ${chrome.runtime.getManifest().version}`);
  }
});

// Example: Message handling between extension components
chrome.runtime.onMessage.addListener((message, sender, sendResponse) => {
  console.log('Received message:', message);
  
  // Send a response back
  sendResponse({ status: 'Message received by background service worker' });
  
  // Return true to indicate you wish to send a response asynchronously
  return true;
}); 