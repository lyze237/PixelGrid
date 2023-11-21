// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.querySelectorAll(".dselect").forEach(e => dselect(e, {
    search: true
}))

Dropzone.options.projectUpload = {
    withCredentials: true,
    parallelUploads: 5,
    chunking: true,
    retryChunks: true,
    maxFilesize: 5000000000,
    chunkSize: 2000000,
    init: function() {
        this.on("addedfile", file => {
            console.log("A file has been added");
        });
        
        this.on("sending", (file, xhr, data) => {
            if(file.fullPath) {
                data.append("dzfullpath", file.fullPath);
            }
        });
    }
};
