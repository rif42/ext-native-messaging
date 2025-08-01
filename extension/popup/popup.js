import * as signalR from '@microsoft/signalr';

// Wait for the DOM to be fully loaded
document.addEventListener('DOMContentLoaded', () => {
  // Get references to DOM elements
  const sendButton = document.getElementById('sendButton');
  // const resultDiv = document.getElementById('result');
  const receiveDataDiv = document.getElementById('receive-data');
  const inputValue = document.getElementById('send-data');

  sendButton.addEventListener('click', async () => {
    try {
      const conn = await initSignalR();
      receiveDataDiv.textContent = JSON.stringify(`signalR connection established ${conn}`, null, 2);
      await conn.invoke('SendMessage', 'Extension', inputValue.value);
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
      return connection;
    } catch (err) {
      alert(err);
    }
  }
}); 