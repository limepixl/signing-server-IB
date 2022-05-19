import "./jszip.js"
import "./FileSaver.js";
import { hash } from './helpers.js'
import AttachArea from "./attachArea.js";

let attachAreaRef = new AttachArea("div#dropArea")

document.getElementById("verifyButton").addEventListener("click", async (event) => {

    let icon = document.getElementById("verifyButton").firstElementChild;

    let file = attachAreaRef.getFiles()

    if(file.length != 1)
        return

    file = file[0]

    if(!file.name.endsWith(".zip")) {
        alert("You can only verify zip files")
        return
    }

    icon.style.animation = "loadingIcon 0.5s infinite alternate-reverse"
    icon.nextSibling.textContent = " Verifying "

    let zip = new JSZip();

    let unzipped = await zip.loadAsync(file)

    let promises = [];

    unzipped.forEach(f => {
        if(f == '.custom.signature')
            return
        promises.push(unzipped.file(f).async("base64"));
    })
    
    let params;
    
    try {
        params = JSON.parse(await unzipped.file(".custom.signature").async("text"))
    } catch(e) {}

    if(!params) {
        window.location.href = "/VerifyResult/Deny";
        return
    }

    let encoded = (await Promise.all(promises)).join()

    let hashed = await hash(encoded)

    let res = await fetch("/Verify/RequestVerification", {
        method: "POST",
        body: JSON.stringify({ hashed, user: params.user, signature: params.signature })
    })

    let resText = await res.text();
    //console.log(resText);
    if (resText == "YES")
        window.location.href = "/VerifyResult/Accept";
    else if (resText == "NO")
        window.location.href = "/VerifyResult/Deny";
})
