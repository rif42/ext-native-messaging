import * as signalR from '@microsoft/signalr';

// Wait for the DOM to be fully loaded
document.addEventListener('DOMContentLoaded', async () => {
  // Get references to DOM elements
  const sendButton = document.getElementById('sendButton');
  const resultDiv = document.getElementById('result');
  const receiveDataDiv = document.getElementById('receive-data');
  const inputValue = document.getElementById('send-data');
  const conn = await initSignalR();
  receiveDataDiv.textContent = JSON.stringify(`signalR connection established ${conn}`, null, 2);

  sendButton.addEventListener('click', async () => {
    try {
      await conn.invoke('SendMessage', 'Extension', inputValue.value);
      receiveDataDiv.textContent = JSON.stringify(`Message sent to desktop ${inputValue.value}`, null, 2);

    } catch (error) {

      alert(error);
    }
  });

  async function initSignalR() {
    try {
      const connection = new signalR.HubConnectionBuilder()
        .withUrl(
          'http://localhost:5000/chatHub'
        )
        .withAutomaticReconnect()
        .build();
      await connection.start();

      // signalR events
      connection.on('ConnectionAcknowledged', (message) => {
        receiveDataDiv.textContent = `Connection status: ${message}`;
      });

      connection.on('MessageAcknowledged', () => {
        setTimeout(() => {
          receiveDataDiv.textContent = 'Message Acknowledged';
        }, 1000);
      });

      connection.on('ReceiveMessage', (message) => {
        resultDiv.textContent = `${message}`;
      });

      return connection;
    } catch (err) {
      alert(err);
    }
  }
}); 