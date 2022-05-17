import "./jszip.js"
import "./FileSaver.js";
import { encode, hash } from './helpers.js'

const currDate = new Date();
const dateWithOffset = new Date(currDate.getTime() - currDate.getTimezoneOffset() * 60000);
JSZip.defaults.date = dateWithOffset;

async function makePackage(files, signature) {
    
    let zip = new JSZip();

    const currDate = new Date();

    for(let f of files) {
        zip.file(f.name, f, {
            date: new Date(f.lastModified - currDate.getTimezoneOffset() * 60000)
        })
    }

    zip.file("signature.nashsvet", signature)

    zip.generateAsync({type:"blob"}).then(function(content) {
        saveAs(content, "example.zip");
    });
}


let filesEl = document.getElementById("filesToSign")

document.getElementById("signButton").addEventListener("click", async (event) => {
    
    let encoded = await encode(filesEl.files)

    let hashed = await hash(encoded)

    console.log(hashed);

    let res = await fetch("/Sign/RequestSignature", {
        method: "POST",
        body: hashed
    })

    if(res.status != 200 || res.redirected)
        return;

    let signature = (await res.text()) || "miki milane"

    let zip = makePackage(filesEl.files, signature);

})

