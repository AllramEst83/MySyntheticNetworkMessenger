"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
var selectedTabId = 1;

//Disable the send button until connection is established.
document.getElementById("send-button").disabled = true;

connection.on("ReceiveMessage", function (user, message, tabId) {
    var chatArea = document.getElementById('chat-area' + tabId);

    var messageDiv = document.createElement("div");
    messageDiv.className = "message received";
    messageDiv.textContent = message;

    chatArea.appendChild(messageDiv);
    chatArea.scrollTop = chatArea.scrollHeight;
});


connection.start().then(function () {


    switchTab(selectedTabId)
}).catch(function (err) {
    return console.error(err.toString());
});

document.addEventListener('keydown', function (event) {
    if (event.key === 'Enter') {

        SendMessage()
    }
});


document.getElementById("send-button").addEventListener("click", function (event) {
    event.preventDefault();

    SendMessage();
});

function SendMessage() {
    if (!selectedTabId) {
        console.error('No contact selected');
        return;
    }

    var message = document.getElementById('message-input').value;
    var chatArea = document.getElementById('chat-area' + selectedTabId);

    var messageDiv = document.createElement("div");

    messageDiv.className = "message sent";
    messageDiv.textContent = message;

    chatArea.appendChild(messageDiv);
    chatArea.scrollTop = chatArea.scrollHeight;

    document.getElementById('message-input').value = "";

    var userId = selectedTabId;

    connection.invoke("SendMessage", userId.toString(), message, selectedTabId.toString())
        .catch(function (err) {
            return console.error(err.toString());
        });

    console.log('Sent message to contact with ID:', selectedTabId);
}


function switchTab(tabId) {
    var chats = document.querySelectorAll('.chat-area');
    chats.forEach(chat => chat.style.display = 'none');

    var contacts = document.querySelectorAll('#contacts li');
    contacts.forEach(contact => contact.classList.remove('selected'));

    document.getElementById('chat-area' + tabId).style.display = 'block';
    document.getElementById(tabId).classList.add('selected');

    selectedTabId = tabId;
    document.getElementById("send-button").disabled = false;
}