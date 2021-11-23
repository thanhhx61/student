"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;
let userName = document.getElementById("userInput").value;

connection.on("ReceiveMessage", function (message) {
    debugger
    let userName = document.getElementById("userInput").value;
    let isMe = userName == message.userName;

    /*var str = `<div class="outgoing"><div style = "display: flex;  align-items: center;justify-content: right;flex-flow: row-reverse;"><div><div><img class="rounded-circle" style="width: 60px" src="/images/teacher.png" alt=""></div><div><p class="bubble">${user}</p></div></div><div><div class="bubble"><strong style="color:black">Tên</strong><p>${message}</p><br><span style="font-size: small;" class="msg_time_send">${timestamp}</span></div></div></div></div><p>quyetdeptrai</p>`;*/
    var str = `<div class="${(!isMe ? 'incoming' : 'outgoing')}"><div style = "display: flex;  align-items: center;${(isMe ? 'justify-content: right;flex-flow: row-reverse;' : '')}"><div><div><img class="rounded-circle" style="width: 60px" src="${message.avatar}" alt=""></div><div><p class="${(!isMe ? 'bubble' : 'bubble1')}">${message.userName}</p></div></div><div><div class="bubble"><strong style="color:black">${message.userName}</strong><p>${message.content}</p><br><span style="font-size: small;" class="msg_time_send">${message.timestamps}</span></div></div></div></div>`;

    $("#messagesList").append(str);
    $('#messageInput').text('');
    $('#messageInput').html('');
    $('.emojionearea-editor').html('');
    connection.invoke("GetAllMessageByEventId", eventId).catch(function (err) {
        return console.error(err.toString());
    });
});

//connection.on("ListAllMessageByEventId", function (user) {
//    console.log(user);
//    var li = document.createElement("li");
//    document.getElementById("messagesList").appendChild(li);
//    // We can assign user-supplied strings to an element's textContent because it
//    // is not interpreted as markup. If you're assigning in any other way, you 
//    // should be aware of possible script injection concerns.
//    li.textContent = `${user} says ${user}`;
//});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;

    var url_string = window.location.href;
    var eventId = url_string.substring(url_string.lastIndexOf('/') + 1);
    connection.invoke("GetAllMessageByEventId", eventId).catch(function (err) {
        return console.error(err.toString());
    });

    //connection.on("ListAllMessageByEventId", function (user) {
    //    console.log(user);
    //    var li = document.createElement("li");
    //    document.getElementById("messagesList").appendChild(li);
    //    // We can assign user-supplied strings to an element's textContent because it
    //    // is not interpreted as markup. If you're assigning in any other way, you 
    //    // should be aware of possible script injection concerns.
    //    li.textContent = `${user} says ${message}`;
    //});
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("getProfileInfo", (user) => {
    console.log(user);
    $('#userInput').val(user.userName);
});
connection.on("ListAllMessageByEventId", function (user) {
    let userName = document.getElementById("userInput").value;
    user.forEach((item, index) => {
        let isMe = userName == item.userName;
        var str = `<div class="${(!isMe ? 'incoming' : 'outgoing')} datamessage"><div style = "display: flex;  align-items: center;${(isMe ? 'justify-content: right;flex-flow: row-reverse;' : '')}"><div><div><img class="rounded-circle" style="width: 60px" src="${item.avatar}" alt=""></div><div><p class="${(!isMe ? 'bubble' : 'bubble1')}">${item.userName}</p></div></div><div class="d-flex ${(isMe ? "reveser" : "")}"><div class="bubble"><strong style="color:black">${item.userName}</strong><p>${item.content}</p><br><span style="font-size: small;" class="msg_time_send">${item.timestamps}</span></div> <div class="btn-group my-auto ${(!isMe ? 'd-none' : '')}"><a href="#" data-bs-toggle="dropdown" aria-expanded="false"><i class="bi bi-three-dots-vertical"></i></a><ul class="dropdown-menu dropdown-custom"><li><a href="#" data-id="${item.messagesId}" onclick="deleteMessage('this')">Xóa</a></li></ul></div> </div></div></div>`;
        $("#messagesList").append(str);
    });
});


document.getElementById("sendButton").addEventListener("click", function (event) {
    var message = document.getElementById("messageInput").value;
    var url_string = window.location.href;
    var eventId = url_string.substring(url_string.lastIndexOf('/') + 1);
    connection.invoke("SendMessage", eventId, message).catch(function (err) {
        return console.error(err.toString());
    });
    //var url_string = window.location.href;
    //var eventId = url_string.substring(url_string.lastIndexOf('/') + 1);
    //connection.invoke("GetAllMessageByEventId2", eventId).catch(function (err) {
    //    return console.error(err.toString());
    //});
    event.preventDefault();
});
function deleteMessage(e) {
    e.preventDefault();
    debugge;
    var id = $(this).data('id');
    var url_string = window.location.href;
    var eventId = url_string.substring(url_string.lastIndexOf('/') + 1);
    connection.invoke("DeleteMessageById", id).catch(function (err) {
        return console.error(err.toString());
    });
    $(e).remove();
}