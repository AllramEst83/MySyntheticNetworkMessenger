"use strict";

//Some jQuery mixed in with the vanilla JS. Not ideal, But wanted the ease of useing the bootstrap & jQuery Modal for the form.
$(document).ready(function () {

    var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
    var selectedTabId = null

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

    $('#contacts ul').on('click', 'li', function () {
        var tabId = $(this).attr('id');
        switchTab(tabId);
    });


    function switchTab(tabId) {
        // Remove 'selected' class from all tabs
        $('#contacts ul li').removeClass('selected');

        // Add 'selected' class to the current tab
        $('#' + tabId).addClass('selected');

        // Hide all chat areas
        $('.chat-area').hide();

        // Show the current chat area
        $('#chat-area' + tabId).show();

        // Update the selectedTabId variable
        selectedTabId = tabId;
    }


    $('#message-input').on('input', function () {
        var messageLength = $(this).val().length;
        var messagesExist = $('#chat-area' + selectedTabId).children('.message').length > 0;
        var inputNotEmpty = messageLength > 1;

        // Enable send button only if a tab is selected and
        // (there are messages and input is not empty) or there are no messages.
        var enableButton = selectedTabId !== null && ((messagesExist && inputNotEmpty) || !messagesExist);

        $("#send-button").prop('disabled', !enableButton);
    });


    $('#add-contact-button').click(function () {
        $('#personalityModal').modal('show');
    });

    $('#close-button').click(function () {
        $('#personalityModal').modal('hide');
    });


    function generateNextChatId() {

        var chatAreas = $('[id^="chat-area"]');
        var maxId = 0;

        // Loop through each chat area and determine the highest ID
        chatAreas.each(function () {
            var id = parseInt(this.id.replace('chat-area', ''), 10);
            if (id > maxId) {
                maxId = id;
            }
        });

        return maxId + 1;
    }
    $('#personality-form').submit(function (event) {
        event.preventDefault();

        var nextChatId = generateNextChatId();

        var formData = {
            chatId: nextChatId,
            name: $('#name').val(),
            manKvinna: $('#mankvinna').prop('checked'),
            era: parseInt($('#era-select').find(':selected').val(), 10),
            age: parseInt($('#age').val(), 10),
            politeness: parseInt($('#politeness').val(), 10),
            formality: parseInt($('#formality').val(), 10),
            humor: parseInt($('#humor').val(), 10),
            confidence: parseInt($('#confidence').val(), 10),
            goofiness: parseInt($('#goofiness').val(), 10),
            shortAnswers: parseInt($('#shortanswers').val(), 10)
        };

        connection.invoke("AddContact", formData)
            .catch(function (err) {

                return console.error(err.toString());
            });

        var newContactTab = $('<li></li>')
            .attr('id', nextChatId)
            .text(formData.name)
            .on('click', function () { switchTab(nextChatId); })
            .appendTo('#contacts ul');

        var newChatArea = $('<div></div>')
            .attr('id', 'chat-area' + nextChatId)
            .addClass('chat-area')
            .css('display', 'none')
            .insertBefore('#input-area');

        this.reset();

        $('#personalityModal').modal('hide');
        switchTab(nextChatId);
    });
});

