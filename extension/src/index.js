// This is the main entry point for your extension's source code
// You can use this file to import and organize your modules

// Example: Import utility functions
import { utils } from './utils.js';

// Example: Initialize your extension's core functionality
function initializeExtension() {
  console.log('Extension source initialized');
  
  // Use imported utilities
  utils.logMessage('Using utility function');
}

// Call the initialization function
initializeExtension(); 