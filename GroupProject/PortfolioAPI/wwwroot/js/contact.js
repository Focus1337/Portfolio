const uri = 'api/Contact';

function sendMessage() {
    const sendNameTextbox = document.getElementById('send-name');
    const sendEmailTextbox = document.getElementById('send-email');
    const sendSubjectTextbox = document.getElementById('send-subject');
    const sendMessageTextbox = document.getElementById('send-message');

    const item = {
        name: sendNameTextbox.value.trim(),
        email: sendEmailTextbox.value.trim(),
        subject: sendSubjectTextbox.value.trim(),
        message: sendMessageTextbox.value
    };

    fetch(uri, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(item)
    })
        .then(response => response.json())
        .then(() => {
            showContactStatus();
            sendNameTextbox.value = '';
            sendEmailTextbox.value = '';
            sendSubjectTextbox.value = '';
            sendMessageTextbox.value = '';
            setTimeout(clearContactStatus, 8000);
        })
        .catch(error => console.error('Unable to add item.', error));
}

function clearContactStatus() {
    document.getElementById("status").hidden = true;
    document.getElementById("status").innerHTML = "";
}

function showContactStatus() {
    document.getElementById("status").hidden = false;
    document.getElementById("status").innerHTML = "Your message has been sent. Thank you!";
}