
function sendFiles() {
    var files = document.getElementById('fileSender').files;
    var data = new FormData();
    for (var i = 0; i < files.length; i++)
        data.append('files', files[i]);
    axios.post("https://localhost:7188/api/ExcelProcessing/ParseXls", data,
        {
            headers: {
                'Content-Type': 'multipart/form-data'
            }

        })
        .then(function (response) {
            console.log(response);
            document.getElementById("filesForUpload").innerHTML = null;
            document.getElementById('uploadForm').reset();
            getDbFileNames(response.data);
        });
}

function addFilesToUploadList() {
    var files = document.getElementById('fileSender').files;
    var element = document.getElementById("filesForUpload");
    var list = "";
    for (var i = 0; i < files.length; i++) {
        list += `<li>${files[i].name}`;
    }
    element.innerHTML = list;
}

function getDbFileNames(filenames)
{
    var element = document.getElementById("dbFiles");
    var list = "";
    for (var i = 0; i < filenames.length; i++) {
        list += `<li>${filenames[i]}`;
    }
    element.innerHTML = list;
}