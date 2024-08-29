
function sendFiles() {
    let files = document.getElementById('fileSender').files;
    let data = new FormData();
    for (let i = 0; i < files.length; i++)
        data.append('files', files[i]);
    axios.post("/api/ExcelProcessing/ParseXls", data,
        {
            headers: {
                'Content-Type': 'multipart/form-data'
            }

        })
        .then(function (response) {
            console.log(response);
            document.getElementById("filesForUpload").innerHTML = null;
            document.getElementById('uploadForm').reset();
            setDbFileNamesList(response.data);
        });
}

function getFileData(filename) {
    axios.get("/api/ExcelProcessing/ExportDbInfo", { params: { fileName: filename } })
        .then(function (response) {
            downloadableFileFilename = filename;
            displayTable(response.data);
        });
}

function getDbFileNamesList() {
    axios.get("/api/ExcelProcessing/GetStoringFilesNames")
        .then(function (response) {
            setDbFileNamesList(response.data);
        });
}

function addFilesToUploadList() {
    let files = document.getElementById('fileSender').files;
    let element = document.getElementById("filesForUpload");
    let list = "";
    for (let i = 0; i < files.length; i++) {
        list += `<li>${files[i].name}`;
    }
    element.innerHTML = list;
}

function setDbFileNamesList(filenames) {
    let element = document.getElementById("dbFiles");
    element.innerHTML = null;
    for (let i = 0; i < filenames.length; i++) {
        let li = document.createElement("li");
        let button = document.createElement("button");

        button.setAttribute("class", "filenames");
        button.setAttribute("type", "button");
        button.setAttribute("onclick", `getFileData("${filenames[i]}")`);
        button.appendChild(document.createTextNode(filenames[i]));

        li.appendChild(button);
        element.appendChild(li);
    }
}

var downloadableFileFilename;

function displayTable(tableData) {
    let btn = document.getElementById("exportButton");
    btn.href = downloadableFileFilename;
    btn.download = downloadableFileFilename;
    let headers = document.getElementsByTagName("h2");
    document.getElementsByTagName("table")[0].style = "display: block";
    document.getElementsByTagName("h1")[0].style = "display: block";
    headers[0].style = "display: block";
    headers[1].style = "display: block";
    headers[0].innerText = `за период с 01.01.${tableData[0].tableYear} по 31.12.${tableData[0].tableYear}`;
    headers[1].innerText = `по банку "${tableData[0].bankName}"`;

    let tbody = document.getElementsByTagName("tbody")[0];
    tbody.innerHTML = null;
    const classes = [
        "КЛАСС  1  Денежные средства, драгоценные металлы и межбанковские операции",
        "КЛАСС  2  Кредитные и иные активные операции с клиентами",
        "КЛАСС  3  Счета по операциям клиентов",
        "КЛАСС  4  Ценные бумаги",
        "КЛАСС  5  Долгосрочные финансовые вложения в уставные фонды юридических лиц, основные средства и прочее имущество",
        "КЛАСС  6  Прочие активы и прочие пассивы",
        "КЛАСС  7  Собственный капитал банка",
        "КЛАСС  8  Доходы банка",
        "КЛАСС  9  Расходы банка"
    ]
    let classSell;
    let accountClass = 0, textNodes = [];
    for (let row of tableData) {
        if (accountClass != Math.floor(row.accountId / 1000) && row.accountId >= 1000) {
            classSell = tbody.insertRow().insertCell();
            classSell.colSpan = "7";
            classSell.style = "font-weight: bold; height: 30px";
            classSell.appendChild(document.createTextNode(classes[accountClass]));
            accountClass = Math.floor(row.accountId / 1000);
        }

        let newRow = tbody.insertRow();
        if (row.accountId < 1000)
            newRow.setAttribute("style", "font-weight: bold");
        let newCells = [];
        for (let i = 0; i < 7; i++)
            newCells[i] = newRow.insertCell();

        if (row.accountId == -1)
            row.accountId = "ПО КЛАССУ";
        if (row.accountId == -2)
            row.accountId = "БАЛАНС";

        textNodes[0] = document.createTextNode(row.accountId);
        textNodes[1] = document.createTextNode(row.inActive);
        textNodes[2] = document.createTextNode(row.inPassive);
        textNodes[3] = document.createTextNode(row.debt);
        textNodes[4] = document.createTextNode(row.credit);
        textNodes[5] = document.createTextNode(row.outActive);
        textNodes[6] = document.createTextNode(row.outPassive);

        for (let i = 0; i < 7; i++) {
            newCells[i].appendChild(textNodes[i]);
        }
    }
}