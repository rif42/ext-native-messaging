// Utility functions for the extension

export const utils = {
  /**
   * Log a message with a timestamp
   * @param {string} message - The message to log
   */
  logMessage(message) {
    const timestamp = new Date().toISOString();
    console.log(`[${timestamp}] ${message}`);
  },
  
  /**
   * Store data in chrome.storage.local
   * @param {Object} data - The data to store
   * @returns {Promise} A promise that resolves when the data is stored
   */
  storeData(data) {
    return new Promise((resolve, reject) => {
      chrome.storage.local.set(data, () => {
        if (chrome.runtime.lastError) {
          reject(new Error(chrome.runtime.lastError.message));
        } else {
          resolve();
        }
      });
    });
  },
  
  /**
   * Retrieve data from chrome.storage.local
   * @param {string|Array<string>} keys - The key(s) to retrieve
   * @returns {Promise<Object>} A promise that resolves with the retrieved data
   */
  getData(keys) {
    return new Promise((resolve, reject) => {
      chrome.storage.local.get(keys, (result) => {
        if (chrome.runtime.lastError) {
          reject(new Error(chrome.runtime.lastError.message));
        } else {
          resolve(result);
        }
      });
    });
  }
}; 