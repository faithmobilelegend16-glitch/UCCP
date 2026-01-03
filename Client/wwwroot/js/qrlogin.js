/*
import QrScanner from "https://unpkg.com/qr-scanner@1.4.2/qr-scanner.min.js";

export async function startScanner(dotNetHelper) {
    const videoElem = document.getElementById("qr-video");

    try {
        const cameras = await QrScanner.listCameras(true);

        if (cameras.length === 0) {
            alert("No camera detected. Please connect a camera or try uploading a QR image.");
            return;
        }

        const scanner = new QrScanner(videoElem, result => {
            scanner.stop();
            dotNetHelper.invokeMethodAsync("OnQrScanned", result.data);
        });

        await scanner.setCamera(cameras[0].id);
        await scanner.start();
    } catch (err) {
        console.error("Camera error:", err);
        dotNetHelper.invokeMethodAsync("OnQrScanError", err.message);
    }
}

export async function decodeImage(inputElem, dotNetHelper) {
    const file = inputElem.files[0];
    if (!file) return;

    try {
        const result = await QrScanner.scanImage(file);
        if (result) {
            dotNetHelper.invokeMethodAsync("OnQrUploaded", result);
        } else {
            alert("No QR code found in image.");
        }
    } catch (err) {
        alert("Error reading QR code: " + err);
    }
}
*/




import QrScanner from "https://unpkg.com/qr-scanner@1.4.2/qr-scanner.min.js";

export async function startScanner(dotNetHelper) {
    const videoElem = document.getElementById("qr-video");

    try {
        const cameras = await QrScanner.listCameras(true);

        if (cameras.length === 0) {
            alert("No camera detected. Please connect a camera or try uploading a QR image.");
            return;
        }

        const scanner = new QrScanner(videoElem, result => {
            scanner.stop();
            dotNetHelper.invokeMethodAsync("OnQrScanned", result.data); // .data is correct here
        });

        await scanner.setCamera(cameras[0].id);
        await scanner.start();
    } catch (err) {
        console.error("Camera error:", err);
        dotNetHelper.invokeMethodAsync("OnQrScanError", err.message);
    }
}

export async function decodeImage(inputElem, dotNetHelper) {
    const file = inputElem.files[0];
    if (!file) return;

    try {
        const result = await QrScanner.scanImage(file, { returnDetailedScanResult: true });
        // returnDetailedScanResult ensures you can get both .data and more info if needed

        if (result) {
            // For uploaded images, the returned result may already be a string
            const qrData = typeof result === "string" ? result : result.data;
            dotNetHelper.invokeMethodAsync("OnQrUploaded", qrData);
        } else {
            alert("No QR code found in image.");
        }
    } catch (err) {
        alert("Error reading QR code: " + err);
    }
}

