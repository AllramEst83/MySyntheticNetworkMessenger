"use strict";

$(document).ready(function () {

    var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
    var selectedTabId = null

    //Disable the send button until connection is established.
    $("#send-button").prop('disabled', true);

    connection.on("ReceiveMessage", function (user, message, tabId) {

        var chatArea = $('#chat-area' + tabId);
        var messageDiv = $('<div></div>').addClass('message received').text(message);

        chatArea.append(messageDiv);
        chatArea.scrollTop(chatArea.prop('scrollHeight'));

    });

    connection.start().then(function () {

        CreateDefaultUser();
    }).catch(function (err) {
        return console.error(err.toString());
    });

    $('#message-input').on('keydown', function (event) {
        if (event.key === 'Enter') {
            SendMessage();
        }
    });

    $("#send-button").on("click", function (event) {
        event.preventDefault();
        SendMessage();
    });

    function SendMessage() {
        if (!selectedTabId) {
            console.error('No contact selected');
            return;
        }

        var message = $('#message-input').val();
        var chatArea = $('#chat-area' + selectedTabId);

        var messageDiv = $('<div></div>').addClass('message sent').text(message);

        chatArea.append(messageDiv);
        chatArea.scrollTop(chatArea.prop('scrollHeight'));

        $('#message-input').val('');


        var userId = selectedTabId;

        connection.invoke("SendMessage", userId.toString(), message, selectedTabId)
            .catch(function (err) {
                return console.error(err);
            });

    }

    $('#contacts ul').on('click', 'li', function () {
        var tabId = $(this).attr('id');
        switchTab(tabId);
    });

    function switchTab(tabId) {

        tabId = parseInt(tabId, 10); 

        $('#contacts ul li').removeClass('selected');
        $('#' + tabId).addClass('selected');
        $('.chat-area').hide();
        $('#chat-area' + tabId).show();

        selectedTabId = tabId;
    }

    $('#message-input').on('input', function () {
        var messageLength = $(this).val().length;
        var messagesExist = $('#chat-area' + selectedTabId).children('.message').length > 0;
        var inputNotEmpty = messageLength > 1;

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

        chatAreas.each(function () {
            var id = parseInt(this.id.replace('chat-area', ''), 10);
            if (id > maxId) {
                maxId = id;
            }
        });

        return maxId + 1;
    }

    function CreateDefaultUser() {

        var nextChatId = generateNextChatId();

        var formData = {
            chatId: nextChatId,
            name: 'Farfar Urban',
            manKvinna: true,
            era: 6,
            age: 68,
            politeness: 3,
            formality: 1,
            humor: 5,
            confidence: 3,
            goofiness: 5,
            shortAnswers: 3
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

        switchTab(nextChatId);
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

