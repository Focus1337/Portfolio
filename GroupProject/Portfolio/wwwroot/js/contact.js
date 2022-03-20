function saveContactFormData() {
    let contactName = document.getElementById("contactName");
    let contactEmail = document.getElementById("contactEmail");
    let contactSubject = document.getElementById("contactSubject");
    let contactMessage = document.getElementById("contactMessage");
    contactName.oninput = () => {
        sessionStorage.setItem("contactName", contactName.value);
    }
    contactEmail.oninput = () => {
        sessionStorage.setItem("contactEmail", contactEmail.value);
    }
    contactSubject.oninput = () => {
        sessionStorage.setItem("contactSubject", contactSubject.value);
    }
    contactMessage.oninput = () => {
        sessionStorage.setItem("contactMessage", contactMessage.value);
    }
    contactName.value = sessionStorage.getItem("contactName");
    contactEmail.value = sessionStorage.getItem("contactEmail");
    contactSubject.value = sessionStorage.getItem("contactSubject");
    contactMessage.value = sessionStorage.getItem("contactMessage");
}
saveContactFormData();