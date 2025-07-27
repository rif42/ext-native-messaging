# Chrome Extension Boilerplate (Manifest V3)

A clean, well-structured boilerplate for creating Chrome extensions using Manifest V3.

## Features

- Modern JavaScript with Babel and Webpack
- Clean folder structure
- Background service worker
- Content script
- Popup UI with HTML, CSS, and JavaScript
- Message passing between components
- Utility functions for common tasks
- Build system for development and production
- ZIP packaging for distribution

## Getting Started

### Prerequisites

- Node.js (v14 or later)
- npm (v6 or later)

### Installation

1. Clone this repository or download it
2. Install dependencies:

```bash
npm install
```

### Development

To build the extension in development mode and watch for changes:

```bash
npm run dev
```

### Production Build

To build the extension for production:

```bash
npm run build
```

### Create ZIP for Distribution

To package the extension for submission to the Chrome Web Store:

```bash
npm run zip
```

## Loading the Extension

1. Build the extension using one of the commands above
2. Open Chrome and navigate to `chrome://extensions`
3. Enable "Developer mode" in the top right corner
4. Click "Load unpacked" and select the `dist` directory

## Project Structure

```
├── dist/                  # Build output
├── icons/                 # Extension icons
├── popup/                 # Popup UI files
│   ├── popup.html
│   ├── popup.js
│   └── popup.css
├── scripts/               # Build scripts
├── src/                   # Source files
├── background.js          # Background service worker
├── content.js             # Content script
├── manifest.json          # Extension manifest
├── package.json           # Dependencies and scripts
└── webpack.config.js      # Webpack configuration
```

## Customization

### Manifest

Edit the `manifest.json` file to customize your extension's metadata, permissions, and features.

### Icons

Replace the icon files in the `icons` directory with your own icons.

### Popup

Modify the files in the `popup` directory to customize the popup UI.

### Background Script

Edit the `background.js` file to implement background functionality.

### Content Script

Edit the `content.js` file to implement functionality that interacts with web pages.

## License

This project is licensed under the MIT License - see the LICENSE file for details. 