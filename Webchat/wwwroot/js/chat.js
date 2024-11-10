document.addEventListener('DOMContentLoaded', function () {
    var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

    connection.start().then(function () {
        console.log('SignalR Started...');
        viewModel.roomList();
        viewModel.userList();
    }).catch(function (err) {
        return console.error(err);
    });

    connection.on("newMessage", function (messageView) {
        var isMine = messageView.from === viewModel.myName();
        var message = new ChatMessage(messageView.content, messageView.timestamp,
            messageView.from, isMine, messageView.avatar);
        viewModel.chatMessages.push(message);
        document.querySelector(".chat-body").scrollTop = document.querySelector(".chat-body").scrollHeight;
    });

    connection.on("getProfileInfo", function (displayName, avatar) {
        viewModel.myName(displayName);
        viewModel.myAvatar(avatar);
        viewModel.isLoading(false);
    });

    connection.on("addUser", function (user) {
        viewModel.userAdded(new ChatUser(user.username, user.fullName, user.avatar, user.currentRoom, user.device));
    });

    connection.on("removeUser", function (user) {
        viewModel.userRemoved(user.username);
    });

    connection.on("addChatRoom", function (room) {
        viewModel.roomAdded(new ChatRoom(room.id, room.name));
    });

    connection.on("updateChatRoom", function (room) {
        viewModel.roomUpdated(new ChatRoom(room.id, room.name));
    });

    connection.on("removeChatRoom", function (id) {
        viewModel.roomDeleted(id);
    });

    connection.on("onError", function (message) {
        viewModel.serverInfoMessage(message);
        const errorAlert = document.getElementById("errorAlert");
        errorAlert.classList.remove("d-none");
        setTimeout(() => errorAlert.classList.add("d-none"), 5000);
    });

    connection.on("onRoomDeleted", function (message) {
        viewModel.serverInfoMessage(message);
        const errorAlert = document.getElementById("errorAlert");
        errorAlert.classList.remove("d-none");
        setTimeout(() => errorAlert.classList.add("d-none"), 5000);

        if (viewModel.chatRooms().length == 0) {
            viewModel.joinedRoom("");
        } else {
            setTimeout(function () {
                document.querySelector("ul#room-list li a").click();
            }, 50);
        }
    });

    function AppViewModel() {
        var self = this;

        self.message = ko.observable("");
        self.chatRooms = ko.observableArray([]);
        self.chatUsers = ko.observableArray([]);
        self.chatMessages = ko.observableArray([]);
        self.joinedRoom = ko.observable("");
        self.joinedRoomId = ko.observable("");
        self.serverInfoMessage = ko.observable("");
        self.myName = ko.observable("");
        self.myAvatar = ko.observable("avatar1.png");
        self.isLoading = ko.observable(true);

        self.onEnter = function (d, e) {
            if (e.keyCode === 13) {
                self.sendNewMessage();
            }
            return true;
        };

        self.filter = ko.observable("");
        self.filteredChatUsers = ko.computed(function () {
            if (!self.filter()) {
                return self.chatUsers();
            } else {
                return ko.utils.arrayFilter(self.chatUsers(), function (user) {
                    var displayName = user.displayName().toLowerCase();
                    return displayName.includes(self.filter().toLowerCase());
                });
            }
        });

        self.sendNewMessage = function () {
            var text = self.message();
            if (text.startsWith("/")) {
                var receiver = text.substring(text.indexOf("(") + 1, text.indexOf(")"));
                var message = text.substring(text.indexOf(")") + 1);
                self.sendPrivate(receiver, message);
            } else {
                self.sendToRoom(self.joinedRoom(), self.message());
            }
            self.message("");
        };

        self.sendToRoom = function (roomName, message) {
            if (roomName && message) {
                fetch('/api/Messages', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({
                        content: message,
                        room: roomName,
                        timestamp: new Date().toISOString(),
                        from: viewModel.myName(),
                        avatar: viewModel.myAvatar()
                    })
                });
            }
        };

        self.sendPrivate = function (receiver, message) {
            if (receiver && message) {
                connection.invoke("SendPrivate", receiver.trim(), message.trim());
            }
        };

        self.joinRoom = function (room) {
            connection.invoke("Join", room.name()).then(function () {
                self.joinedRoom(room.name());
                self.joinedRoomId(room.id());
                self.userList();
                self.messageHistory();
            });
        };

        self.roomList = function () {
            fetch('/api/Rooms')
                .then(response => response.json())
                .then(data => {
                    self.chatRooms.removeAll();
                    data.forEach(room => self.chatRooms.push(new ChatRoom(room.id, room.name)));
                    if (self.chatRooms().length > 0) self.joinRoom(self.chatRooms()[0]);
                });
        };

        self.userList = function () {
            connection.invoke("GetUsers", self.joinedRoom()).then(function (result) {
                self.chatUsers.removeAll();
                result.forEach(user => self.chatUsers.push(new ChatUser(user.username, user.fullName, user.avatar || "default-avatar.png", user.currentRoom, user.device)));
            });
        };

        self.createRoom = function () {
            var roomName = document.getElementById("roomName").value;
            fetch('/api/Rooms', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ name: roomName })
            });
        };

        self.editRoom = function () {
            var roomId = self.joinedRoomId();
            var roomName = document.getElementById("newRoomName").value;
            fetch(`/api/Rooms/${roomId}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ id: roomId, name: roomName })
            });
        };

        self.deleteRoom = function () {
            fetch(`/api/Rooms/${self.joinedRoomId()}`, {
                method: 'DELETE',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ id: self.joinedRoomId() })
            });
        };

        self.messageHistory = function () {
            fetch(`/api/Messages/Room/${viewModel.joinedRoom()}`)
                .then(response => response.json())
                .then(data => {
                    self.chatMessages.removeAll();
                    data.forEach(msg => {
                        var isMine = msg.from == self.myName();
                        self.chatMessages.push(new ChatMessage(msg.content, msg.timestamp, msg.from, isMine, msg.avatar));
                    });
                    document.querySelector(".chat-body").scrollTop = document.querySelector(".chat-body").scrollHeight;
                });
        };

        self.roomAdded = function (room) {
            self.chatRooms.push(room);
        };

        self.roomUpdated = function (updatedRoom) {
            var room = self.chatRooms().find(item => updatedRoom.id() == item.id());
            if (room) room.name(updatedRoom.name());
            if (self.joinedRoomId() == room.id()) self.joinRoom(room);
        };

        self.roomDeleted = function (id) {
            self.chatRooms.remove(self.chatRooms().find(room => room.id() == id));
        };

        self.userAdded = function (user) {
            self.chatUsers.push(user);
        };

        self.userRemoved = function (id) {
            self.chatUsers.remove(self.chatUsers().find(user => user.userName() == id));
        };

        self.uploadFiles = function () {
            var form = document.getElementById("uploadForm");
            fetch('/api/Upload', {
                method: 'POST',
                body: new FormData(form)
            }).then(() => {
                document.getElementById("UploadedFile").value = "";
            }).catch(error => alert('Error: ' + error.message));
        };
    }

    function ChatRoom(id, name) {
        this.id = ko.observable(id);
        this.name = ko.observable(name);
    }

    function ChatUser(userName, displayName, avatar, currentRoom, device) {
        this.userName = ko.observable(userName);
        this.displayName = ko.observable(displayName);
        this.avatar = ko.observable(avatar);
        this.currentRoom = ko.observable(currentRoom);
        this.device = ko.observable(device);
    }

    function ChatMessage(content, timestamp, from, isMine, avatar) {
        this.content = ko.observable(content);
        this.timestamp = ko.observable(timestamp);
        this.from = ko.observable(from);
        this.isMine = ko.observable(isMine);
        this.avatar = ko.observable(avatar);
    }

    var viewModel = new AppViewModel();
    ko.applyBindings(viewModel);
});
