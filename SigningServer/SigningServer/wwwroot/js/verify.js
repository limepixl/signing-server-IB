import "./jszip.js"
import "./FileSaver.js";
import { hash } from './helpers.js'

let fileInput = document.getElementById("fileToVerify")

document.getElementById("verifyButton").addEventListener("click", async (event) => {

    let file = fileInput.files

    if(file.length != 1)
        return

    file = file[0]

    let zip = new JSZip();

    let unzipped = await zip.loadAsync(file)

    let promises = [];

    unzipped.forEach(f => {
        if(f.name == 'signature.nashsvet')
            return
        promises.push(unzipped.file(f).async("base64"));
    })
    
    let signature = await unzipped.file("signature.nashsvet").async("string")

    let encoded = (await Promise.all(promises)).join()

    let hashed = await hash(encoded)

    let res = await fetch("/Verify/RequestVerification", {
        method: "POST",
        body: JSON.stringify({ hashed, signature })
    })

    if(res.status != 200 || res.redirected)
        return;

    console.log(await res.text())
})
