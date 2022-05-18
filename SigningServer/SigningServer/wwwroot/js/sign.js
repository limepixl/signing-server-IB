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

    zip.file("signature.nashsvet", JSON.stringify({...reply, hashed}))

    zip.generateAsync({type:"blob"}).then(function(content) {
        saveAs(content, "signed.zip");
    });
}

document.getElementById("signButton").addEventListener("click", async (event) => {
    
    let encoded = await encode(attachAreaRef.getFiles())

    let hashed = await hash(encoded)

    let res = await fetch("/Sign/RequestSignature", {
        method: "POST",
        body: hashed
    })

    if(res.status != 200 || res.redirected)
        return;

    let reply = await res.json()

    makePackage(attachAreaRef.getFiles(), reply, hashed);
})

