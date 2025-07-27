const fs = require('fs');
const path = require('path');
const archiver = require('archiver');

// Get package version
const packageJson = require('../package.json');
const version = packageJson.version;

// Create output directory if it doesn't exist
const outputDir = path.join(__dirname, '../dist-zip');
if (!fs.existsSync(outputDir)) {
  fs.mkdirSync(outputDir);
}

// Create a file to stream archive data to
const output = fs.createWriteStream(path.join(outputDir, `chrome-extension-v${version}.zip`));
const archive = archiver('zip', {
  zlib: { level: 9 } // Sets the compression level
});

// Listen for all archive data to be written
output.on('close', function() {
  console.log(`Archive created: ${archive.pointer()} total bytes`);
  console.log(`Archive has been finalized and the output file descriptor has closed.`);
});

// Good practice to catch warnings
archive.on('warning', function(err) {
  if (err.code === 'ENOENT') {
    console.warn(err);
  } else {
    throw err;
  }
});

// Good practice to catch this error explicitly
archive.on('error', function(err) {
  throw err;
});

// Pipe archive data to the file
archive.pipe(output);

// Append files from the dist directory
archive.directory(path.join(__dirname, '../dist/'), false);

// Finalize the archive (i.e. we are done appending files)
archive.finalize(); 