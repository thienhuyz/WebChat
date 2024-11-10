document.addEventListener('DOMContentLoaded', function () {
    const usersList = document.getElementById('users-list');
    const chatMessageInput = document.getElementById('chat-message');
    const emojisContainer = document.getElementById('emojis-container');
    const emojiButton = document.getElementById('emojibtn');
    const sendMessageButton = document.getElementById('btn-send-message');

    if (usersList) {
        usersList.addEventListener('click', function (event) {
            const target = event.target.closest('li');
            if (target) {
                const username = target.querySelector("input[type=hidden].username").value;
                let text = chatMessageInput.value;
                if (text.startsWith("/")) {
                    text = text.split(")")[1];
                }
                chatMessageInput.value = `/private(${username}) ${text.trim()}`;
                chatMessageInput.dispatchEvent(new Event('input'));
                chatMessageInput.focus();
            }
        });
    }

    if (emojisContainer) {
        emojisContainer.addEventListener('click', function (event) {
            const target = event.target.closest('a');
            if (target) {
                const value = target.querySelector("input").value;
                chatMessageInput.value += value;
                chatMessageInput.focus();
                chatMessageInput.dispatchEvent(new Event('input'));
            }
        });
    }

    if (emojiButton) {
        emojiButton.addEventListener('click', function () {
            emojisContainer.classList.toggle("d-none");
        });
    }

    if (chatMessageInput && sendMessageButton) {
        chatMessageInput.addEventListener('click', function () {
            emojisContainer.classList.add("d-none");
        });

        sendMessageButton.addEventListener('click', function () {
            emojisContainer.classList.add("d-none");
        });
    }

    document.querySelectorAll('.modal').forEach(function (modal) {
        modal.addEventListener('hidden.bs.modal', function () {
            modal.querySelectorAll(".modal-body input:not(#newRoomName)").forEach(input => input.value = "");
        });
    });

    document.querySelectorAll(".alert .close").forEach(function (closeBtn) {
        closeBtn.addEventListener('click', function () {
            closeBtn.parentNode.style.display = "none";
        });
    });
});
