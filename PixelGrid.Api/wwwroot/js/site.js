﻿// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.querySelectorAll(".dselect").forEach(e => dselect(e, {
    search: true
}))

Dropzone.options.projectUpload = {
    withCredentials: true,
    parallelUploads: 5,
    chunking: false,
    maxFilesize: 5000,
    clickable: false,
    init: function() {
        const dropzone = this;
        
        dropzone.on("sending", (file, xhr, data) => {
            console.log(file.fullPath);
            if (file.fullPath) {
                data.append("dzfullpath", file.fullPath);
            }
        });

        dropzone.on("complete", function(file) {
            if (file.status === "success")
                dropzone.removeFile(file);
        });
        
        /*
        if (isFolderDialogSupported()) {
            let dropzone = document.querySelector('.dz-hidden-input');
            dropzone.setAttribute('directory', 'true');
            dropzone.setAttribute('webkitdirectory', 'true');
            dropzone.setAttribute('mozdirectory', 'true');
            dropzone.setAttribute('msdirectory', 'true');
            dropzone.setAttribute('odirectory', 'true');
        }
         */
    }
};


const isFolderDialogSupported = () => {
    const tmpInput = document.createElement('input');
    return ('webkitdirectory' in tmpInput
        || 'mozdirectory' in tmpInput
        || 'odirectory' in tmpInput
        || 'msdirectory' in tmpInput
        || 'directory' in tmpInput
        || false
    );
};
