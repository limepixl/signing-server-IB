import "./jszip.js"
import "./FileSaver.js";
import { hash } from './helpers.js'
import AttachArea from "./attachArea.js";

let attachAreaRef = new AttachArea("div#dropArea")

document.getElementById("verifyButton").addEventListener("click", async (event) => {

    let file = attachAreaRef.getFiles()

    if(file.length != 1)
        return

    file = file[0]

    if(!file.name.endsWith(".zip")) {
        alert("You can only verify zip files")
        return
    }

    let zip = new JSZip();

    let unzipped = await zip.loadAsync(file)

    let promises = [];

    unzipped.forEach(f => {
        if(f == 'signature.nashsvet')
            return
        promises.push(unzipped.file(f).async("base64"));
    })
    
    let params = JSON.parse(await unzipped.file("signature.nashsvet").async("text"))

    let encoded = (await Promise.all(promises)).join()

    let hashed = await hash(encoded)

    let res = await fetch("/Verify/RequestVerification", {
        method: "POST",
        body: JSON.stringify({ hashed, user: params.user, signature: params.signature })
    }).then(response => response.json())
        .catch(err => alert("maybe some error occured"));

    let resText = await res;
    console.log(resText);
    if(resText != "NO")
        window.location.href = "https://localhost:7096/VerifyResult";
})
