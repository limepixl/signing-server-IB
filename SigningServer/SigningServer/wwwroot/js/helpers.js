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
    return btoa(String.fromCharCode(...new Uint8Array(hashed)))
}

export { encode, hash }