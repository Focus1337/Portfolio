    function loadContactStatus() {
        let xmlHttpRequest = new XMLHttpRequest();
        xmlHttpRequest.onreadystatechange = function () {
            if (this.readyState === 4 && this.status === 200) {
                document.getElementById("status").hidden = false;
                document.getElementById("status").innerHTML = "Your message has been sent. Thank you!";
            }
        };
        xmlHttpRequest.open("POST", "https://localhost:7212/Contact/Send", true);
        xmlHttpRequest.send();

        setTimeout(clearContactStatus, 8000);
    }

    function clearContactStatus() {
        document.getElementById("status").hidden = true;
        document.getElementById("status").innerHTML = "";
    }