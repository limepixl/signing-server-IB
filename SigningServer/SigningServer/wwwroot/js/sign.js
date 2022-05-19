import "./jszip.js"
import "./FileSaver.js";
import { encode, hash } from './helpers.js'
import AttachArea from "./attachArea.js";

const currDate = new Date();
const dateWithOffset = new Date(currDate.getTime() - currDate.getTimezoneOffset() * 60000);
JSZip.defaults.date = dateWithOffset;

let attachAreaRef = new AttachArea("div#dropArea", {
    multiple: true
})

async function makePackage(files, reply, hashed) {
    
    let zip = new JSZip();

    const currDate = new Date();

    for(let f of files) {
        zip.file(f.name, f, {
            date: new Date(f.lastModified - currDate.getTimezoneOffset() * 60000)
        })
    }

    zip.file(".custom.signature", JSON.stringify({...reply, hashed}))

    let content = await zip.generateAsync({type:"blob"})
    saveAs(content, "signed.zip");
}

document.getElementById("signButton").addEventListener("click", async (event) => {

    let icon = document.getElementById("signButton").firstElementChild
    icon.nextSibling.textContent = " Signing "
    icon.style.animation = "loadingIcon 0.5s infinite alternate-reverse"

    let encoded = await encode(attachAreaRef.getFiles())

    let hashed = await hash(encoded)

    let res = await fetch("/Sign/RequestSignature", {
        method: "POST",
        body: hashed
    })

    let replyText = await res.text();
    if (replyText == "NO_2FA") {
        window.location.href = "/Identity/Account/Manage/TwoFactorAuthentication";
        return;
    }

    if(res.status != 200 || res.redirected)
        return;

    let reply = JSON.parse(replyText);

    await makePackage(attachAreaRef.getFiles(), reply, hashed);
    icon.style.removeProperty('animation')
    icon.nextSibling.textContent = " Sign "
})

