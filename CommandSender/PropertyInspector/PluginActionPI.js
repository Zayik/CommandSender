// sdtools.common.js v1.0
var websocket = null,
    uuid = null,
    registerEventName = null,
    actionInfo = {},
    inInfo = {},
    runningApps = [],
    isQT = navigator.appVersion.includes('QtWebEngine');

function setdesiredState() {
    var ele = document.getElementById("desiredStates");
    if (ele.value > 10)
        ele.value = 10;
    else if (ele.value <= 0)
        ele.value = 1;
    updateStateVisibilityOnChange()

    setSettings()
}

function updateStateVisibilityOnChange() {
    var ele = document.getElementById("desiredStates");
    updateStateVisibility(ele.value)
}

function updateStateVisibility(desiredStates) {
    for (var i = 0; i < desiredStates; i++) {
        var stateId = "State" + i;
        document.getElementById(stateId).style.display = 'inline'
    }
    for (var i = desiredStates; i < 10; i++) {
        var stateId = "State" + i;
        document.getElementById(stateId).style.display = 'none'
    }
}

function validatePort(eleId) {
    if(eleId.value == null || eleId.value == "")
        eleId.value = eleId.placeholder;
    setSettings();
}
