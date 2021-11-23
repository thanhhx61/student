
$(document).ready(function () {
    var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

    connection.start().then(function () {
        console.log('SignalR Started...')
        viewModel.loadMessageHistory();
    }).catch(function (err) {
        return console.error(err);
    });

    connection.on("ReceiveMessage", function (messageView) {
        console.log(messageView, viewModel.myName())
        var isMine = messageView.userName == viewModel.myName();
        var message = new ChatMessage(messageView.content, messageView.timestamps,
            messageView.userName, isMine, messageView.avatar, messageView.messagesId);
        viewModel.chatMessages.push(message);
        $(".chat-body").animate({ scrollTop: $(".chat-body")[0].scrollHeight }, 1000);
    });

    connection.on("getProfileInfo", function (user) {
        viewModel.myName(user.userName);
        viewModel.myAvatar(user.avatar);
    });

    connection.on("removeMessage", function (id) {
        viewModel.messageDeleted(id);
    });

    connection.on("onError", function (message) {
      
    });

    connection.on("onMessageDeleted", function (message) {
    });
    
    connection.on("updateMessage", function (message) {
        console.log(message);
        viewModel.messageUpdated(new ChatMessageUpdate(message.id, message.content));
    });

    function AppViewModel() {
        var self = this; // == AppViewModel

        self.message = ko.observable("");
        self.messageUpdate = ko.observable("");
        self.chatMessages = ko.observableArray([]);
        self.myName = ko.observable("");
        self.myAvatar = ko.observable("avatar1.png");
        self.isLoading = ko.observable(true);

        self.onEnter = function (d, e) {
            if (e.keyCode === 13) {
                self.sendNewMessage();
            }
            return true;
        }

        self.sendNewMessage = function () {
            var text = self.message();

            var url_string = window.location.href;
            var eventId = url_string.substring(url_string.lastIndexOf('/') + 1);

            self.sendPrivate(eventId, text);
            self.message("");
            $('.emojionearea-editor').html('');
        }

        // call 1 method của hub từ client
        self.sendPrivate = function (eventId, message) {
            if (eventId.length > 0 && message.length > 0) {
                connection.invoke("SendMessage", eventId, message.trim()).catch(function (err) {
                    return console.error(err.toString());
                });
            }
        }

        self.loadMessageHistory = function (receiver, message) {
            self.messageHistory();
        }

        self.getEnventId = function () {
            var url_string = window.location.href;
            var eventId = url_string.substring(url_string.lastIndexOf('/') + 1);
            return eventId;
        }
        self.messageHistory = function () {
            var eventId = self.getEnventId();
            $.ajax({
                type: "GET",
                url: '/Events/GetAllMessageByEventId/' + eventId,
                success: function (data) {
                    console.log(data);
                    data.forEach((item, index) => {
                        var isMine = item.userName == self.myName();
                        self.chatMessages.push(new ChatMessage(item.content,
                            item.timestamps,
                            item.userName,
                            item.isMe,
                            item.avatar,
                            item.messagesId))
                    });
                },
                error: function (error) {
                    alert('Error: ' + error.responseText);
                }
            });
        }
        self.deleteMessage = function (messageId, message) {
            $.ajax({
                type: "DELETE",
                url: '/messages/' + messageId,
                success: function (data) {
                    console.log(data);
                },
                error: function (error) {
                    alert('Error: ' + error.responseText);
                }
            });
        }
        self.editMessage = function (messageId, message) {
            message.isEdit(!message.isEdit());
        }
        self.updateMessage = function (messageId, message) {
            var eventId = self.getEnventId();
            $.ajax({
                type: "PUT",
                url: '/messages/' + messageId,
                data: message,
                success: function (data) {
                    console.log(data);
                    self.editMessage(messageId, message);
                },
                error: function (error) {
                    alert('Error: ' + error.responseText);
                }
            });
        }

        self.messageUpdated = function (messageUpdated) {
            var message = ko.utils.arrayFirst(self.chatMessages(), function (item) {
                return messageUpdated.messageId() == item.messageId();
            });
            message.content(messageUpdated.content());
        }

        self.messageDeleted = function (messageId) {
            var temp;
            ko.utils.arrayForEach(self.chatMessages(), function (message) {
                if (message.messageId() == messageId)
                    temp = message;
            });
            self.chatMessages.remove(temp);
        }
    }

    // đối tượng user
    function ChatUser(userName, displayName, avatar) {
        var self = this;
        self.userName = ko.observable(userName);
        self.displayName = ko.observable(displayName);
        self.avatar = ko.observable(avatar);
    }
    // đối  tượng message
    function ChatMessage(content, timestamp, from, isMine, avatar, messageId) {
        var self = this;
        self.content = ko.observable(content);
        self.timestamp = ko.observable(timestamp);
        self.timestamps = ko.observable(timestamp);
        self.from = ko.observable(from);
        self.userName = ko.observable(from);
        self.isMine = ko.observable(isMine);
        self.avatar = ko.observable(avatar);
        self.messageId = ko.observable(messageId);
        self.isEdit = ko.observable(false);
    }

    function ChatMessageUpdate(messageId, content) {
        var self = this;
        self.content = ko.observable(content);
        self.messageId = ko.observable(messageId);
    }

    var viewModel = new AppViewModel();
    ko.applyBindings(viewModel);
});
