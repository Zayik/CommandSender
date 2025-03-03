function setdesiredState() {
    var ele = document.getElementById("desiredStates");
    if(ele.value > 10)
        ele.value = 10;
    else if(ele.value <= 0)
        ele.value = 1;

    setSettings();
}

function updateStateVisibilityOnChange() {
    var ele = document.getElementById("desiredStates");
    updateStateVisibility(ele.value);
}

function updateStateVisibility(desiredStates) {
    for(var i = 0; i < desiredStates; i++) {
        var stateId = "State" + i;
        document.getElementById(stateId).style.display = 'inline';
    }
    for(var i = desiredStates; i < 10; i++) {
        var stateId = "State" + i;
        document.getElementById(stateId).style.display = 'none';
    }
}

function validatePort(eleId) {
    if(eleId.value == null || eleId.value == "")
        eleId.value = eleId.placeholder;
    setSettings();
}

// This is required to tell the presentation to update visible states based on configurations when first loading.
loadConfigurationEvent = updateStateVisibilityOnChange;

// Extend the original connectElgatoStreamDeckSocket
function connectElgatoStreamDeckSocketExtension(websocket) {

    // Add custom error logging listener
    websocket.addEventListener("message", function(evt) {
        var jsonObj = JSON.parse(evt.data);
        if(jsonObj.event === "sendToPropertyInspector") {
            var payload = jsonObj.payload;
            if(payload.error) {
                displayError(payload.error);
            }
            else if(payload.errorLog) {
                loadErrorLog(payload.errorLog); // Initial log load
            }
        }
    });
}

function displayError(errorMessage) {
    var errorMessages = document.getElementById("errorMessages");
    var errorEntry = document.createElement("p");
    errorEntry.textContent = errorMessage;
    errorMessages.insertBefore(errorEntry, errorMessages.firstChild); // Add to top
    errorMessages.scrollTop = 0; // Scroll to top
}

function loadErrorLog(logEntries) {
    var errorMessages = document.getElementById("errorMessages");
    errorMessages.innerHTML = ""; // Clear existing entries
    // Reverse the order so newest (index 0) is added last and ends up at the top
    var reversedEntries = logEntries.slice().reverse();
    reversedEntries.forEach(entry => {
        var errorEntry = document.createElement("p");
        errorEntry.textContent = entry;
        errorMessages.insertBefore(errorEntry, errorMessages.firstChild); // Add in reverse order
    });
    errorMessages.scrollTop = 0; // Ensure top is visible
}

function clearErrors() {
    document.getElementById("errorMessages").innerHTML = "";

    sendValueToPlugin("clearErrorLog", "clearErrorLog")
}