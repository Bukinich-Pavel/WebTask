$(function () {
    $('#dropArea').filedrop({
        url: '@Url.Action("UploadedPhoto")',
        allowedfiletypes: ['image/jpeg', 'image/png'],
        allowedfileextensions: ['.jpg', '.jpeg', '.png'],
        paramname: 'file',
        maxfiles: 1,
        maxfilesize: 25,
        dragOver: function () {
            $('#dropArea').addClass('active-drop');
        },
        dragLeave: function () {
            $('#dropArea').removeClass('active-drop');
        },
        drop: function () {
            $('#dropArea').removeClass('active-drop');
        },
        afterAll: function (e) {

            //    $('#dropArea').html('file uploaded successfully');
        },
        uploadFinished: function (i, file, response, time) {
            $('#dropArea').addClass('ok-drop');
            $('#dropArea').html('Файл успешно добавлен');
            //    $('#uploadedFiles').append(file.name + '<br />')
        }
    })
})
