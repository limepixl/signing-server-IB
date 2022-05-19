import "./jQuery.FileDrop.all.js";

function AttachArea(selector, options) {

    this.element = $(selector)[0]
    this.multiple = options?.multiple
    this.files = []

    $(selector).fileDrop({
        onFileRead: this.onAdd.bind(this),
        addClassTo: $(selector),
    })

    this.render()
    this.hiddenInput = document.getElementById("hiddenFilePicker");

    this.element.onclick = () => {
        this.hiddenInput.click();
    }

    this.hiddenInput.onchange = () => {
        let files = Array.from(this.hiddenInput.files)
        if(files.length == 0)
            return;
        for(let f of files) {
            if(this.files.find(x => x.name == f.name)) {
                alert("Already attached file with that name")
                return
            }
            this.files.push(f)
        }
        this.hiddenInput.value = null;
        this.render();
    }
}

AttachArea.prototype.getFiles = function() {
    return [...this.files]
}

AttachArea.prototype.onAdd = function(files) {
    if(!this.multiple && (this.files.length > 0 || files.length > 1)) {
        alert("You can only upload one file at a time")
        return
    }
    for(let f of files) {
        if(this.files.find(x => x.name == f.name)) {
            alert("Already attached file with that name")
            return
        }
        this.files.push(f)
    }

    this.render()
}

AttachArea.prototype.removeFile = function(file) {
    let index = this.files.findIndex(f => f.name == file.name)
    this.files.splice(index, 1)
    this.render()
}

AttachArea.prototype.render = function() {
    
    if(this.files.length == 0)
        this.element.children[0].style.display = 'flex'
    else
        this.element.children[0].style.display = 'none'
    
    let body = this.element.children[1]

    while(body.firstElementChild) {
        body.removeChild(body.firstElementChild)
    }

    for(let f of this.files) {
        let el = document.createElement("div")
        el.className = "dropFile"
        let title = document.createElement("div")
        title.className = "dropFileTitle"
        title.textContent = f.name
        let remove = document.createElement("i")
        remove.className = "dropFileRemove fa-solid fa-trash-can"
        remove.onclick = (event) => {
            event.stopPropagation()
            this.removeFile(f)
        }
        el.appendChild(title)
        el.appendChild(remove)
        body.appendChild(el)
    }
}

export default AttachArea