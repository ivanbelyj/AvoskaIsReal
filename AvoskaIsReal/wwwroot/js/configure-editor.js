$(() => {
    // $('#editor').tinymce({ height: 500, /* other settings... */ });
    $('#editor').tinymce({
        height: 500,
        menubar: false,
        plugins: [
            'advlist', 'autolink', 'lists', 'link', 'image', 'charmap', 'preview',
            'anchor', 'searchreplace', 'visualblocks', 'fullscreen',
            'insertdatetime', 'media', 'table', 'code', 'help', 'wordcount'
        ],
        toolbar: 'undo redo | blocks | bold italic backcolor | ' +
            'alignleft aligncenter alignright alignjustify | ' +
            'bullist numlist outdent indent | removeformat | imageupload | image | help',
        images_upload_url: '/Upload/UploadImage',
        automatic_uploads: true,
        images_reuse_filename: true
    });
});
