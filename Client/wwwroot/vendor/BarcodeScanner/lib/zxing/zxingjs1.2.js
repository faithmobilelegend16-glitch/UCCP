var codeReader = null;
var id = null;
export function init(autostop, wrapper, element, elementid) {
    console.log('init' + elementid);
    id = elementid;
    let selectedDeviceId;
    const sourceSelect = element.querySelector("[data-action=sourceSelect]");
    const sourceSelectPanel = element.querySelector("[data-action=sourceSelectPanel]");
    let startButton = element.querySelector("[data-action=startButton]");
    let resetButton = element.querySelector("[data-action=resetButton]");
    let closeButton = element.querySelector("[data-action=closeButton]");

    console.log('init' + startButton.innerHTML);
   //const codeReader = new ZXing.BrowserBarcodeReader()
    codeReader = new ZXing.BrowserMultiFormatReader()
    console.log('ZXing code reader initialized')
    codeReader.getVideoInputDevices()
        .then((videoInputDevices) => {
            let cameraSetting = localStorage.getItem("superapp-default-camera");
            if (cameraSetting === null || cameraSetting === '') {
                selectedDeviceId = videoInputDevices[videoInputDevices.length - 1].deviceId;
            }
            else {
                selectedDeviceId = cameraSetting;
            }
            
            console.log("defaultCamera: " + selectedDeviceId);
            console.log('videoInputDevices:' + videoInputDevices.length);
            if (videoInputDevices.length > 1) {
                videoInputDevices.forEach((element) => {
                    const sourceOption = document.createElement('option');
                    sourceOption.text = element.label;
                    sourceOption.value = element.deviceId;
                    sourceOption.selected = selectedDeviceId;

                    try {
                        if (document.querySelector("[data-action=sourceSelect] option[value=\"" + element.deviceId + "\"]").length <= 0) {
                            sourceSelect.appendChild(sourceOption);
                        }
                    }
                    catch { sourceSelect.appendChild(sourceOption); }
                })

                try {
                    document.querySelector("[data-action=sourceSelect] option[value=\"" + selectedDeviceId + "\"]").selected = true;
                }
                catch { }

                sourceSelect.onchange = () => {
                    selectedDeviceId = sourceSelect.value;
                    localStorage.setItem("superapp-default-camera", selectedDeviceId);
                    codeReader.reset();
                    StartScan();
                }

                sourceSelectPanel.style.display = 'block'
            }

            StartScan(autostop);

            startButton.addEventListener('click', () => {
                StartScan();
            })

            function StartScan(autostop) {
                setTimeout(function () {
                    console.log('Started: ' + selectedDeviceId);
                    codeReader.decodeOnceFromVideoDevice(selectedDeviceId, 'video').then((result) => {
                        console.log(result)

                        var supportsVibrate = "vibrate" in navigator;
                        if (supportsVibrate) navigator.vibrate(1000);

                        if (autostop) {
                            console.log('autostop');
                            codeReader.reset();
                            return wrapper.invokeMethodAsync("GetResult", result.text);
                        } else {
                            console.log('None-stop');
                            codeReader.reset();
                            wrapper.invokeMethodAsync("GetResult", result.text);
                        }

                    }).catch((err) => {
                        console.log(err)
                        wrapper.invokeMethodAsync("GetError", err + '');
                    })
                    console.log(`Started continous decode from camera with id ${selectedDeviceId}`)
                }, 1000);
            }

        })
        .catch((err) => {
            console.log(err)
            wrapper.invokeMethodAsync("GetError", err + '');
        })

    resetButton.addEventListener('click', () => {
        if (undefined !== codeReader && null !== codeReader && id == elementid)
            codeReader.reset();

        console.log('Reset.')
    })

    closeButton.addEventListener('click', () => {
        if (undefined !== codeReader && null !== codeReader && id == elementid)
            codeReader.reset();

        console.log('closeButton.')
        wrapper.invokeMethodAsync("CloseScan");
    })
}
export function destroy(elementid) {
    if (undefined !== codeReader && null !== codeReader && id == elementid) {
        codeReader.reset();
        codeReader = null;
        //id = null;
        console.log(id, 'destroy');
    }
}