import "./jszip.min.js"
import "./FileSaver.js";

async function encode(files) {

    if(!files.length)
        return

    let reader = new FileReader();

    let encoded = (await Promise.all(Array.from(files).map(f => {
        return new Promise((resolve) => {
            const reader = new FileReader();
            reader.onload = () => {
                let code = reader.result
                resolve(code.substring(code.indexOf(",")+1))
            }
            reader.readAsDataURL(f)
        })
    }))).join()

    return encoded
}

async function hash(string) {
    let encoder = new TextEncoder();
    let bytes = encoder.encode(string)
    let hashed = await crypto.subtle.digest("SHA-256", bytes);
    return hashed
}

async function makePackage(files, signature) {
    
    // ne e gotovo
    
    let zip = new JSZip();

    for(let f of files) {
        zip.file(f.name, f, {
            date: f.lastModified
        })
    }

    zip.file("signature.nashsvet", signature)

    zip.generateAsync({type:"blob"}).then(function(content) {
        saveAs(content, "example.zip");
    });
}


let filesEl = document.getElementById("Files")

document.getElementById("signButton").addEventListener("click", async (event) => {
    
    let encoded = await encode(filesEl.files)

    let hashed = await hash(encoded)

    let res = await fetch("/Sign/RequestSignature", {
        method: "POST",
        body: hashed
    })

    if(res.status != 200)
        return;

    let signature = res.body || "miki milane"

    let zip = makePackage(filesEl.files, signature);

})

