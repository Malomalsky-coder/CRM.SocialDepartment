/*
    Назначение: Для страницы списка задач.
    Версия: 1.0.0
*/

$(document).ready(function () {

    var dataTable = $('#table').DataTable({
        "bAutoWidth": false,
        "processing": true,
        "serverSide": true,
        "filter": true,
        //"order": [[6, "asc"], [7, "asc"]],
        "ajax": {
            "url": "/api/todos/active",
            "type": "POST",
            "datatype": "json"
        },
        "columnDefs": [
            {
                targets: [0],
                orderable: false,
                render: function (todoId) {
                    return ""
                        + "<div id='menu-action' class='dropdown'>"
                        +   "<button class='btn btn-secondary btn-sm dropdown-toggle' type='button' data-bs-toggle='dropdown' aria-expanded='false'>Действие</button>"
                        +   "<ul class='dropdown-menu' data-popper-placement='bottom-start'>"
                        +       `<li><a class='dropdown-item' href='/todos/${todoId}'>Посмотреть</a></li>`
                        +       `<li><a id='edit-todo' data-todo-id='${todoId}' class='dropdown-item' href='#'>Редактировать</a></li>`
                        //+     `<li><a class='dropdown-item' href='#' onclick="jQueryModalGet('?handler=Edit&id=${todoId}', 'Редактировать');">Редактировать</a></li>`
                        //+     "<li><a class='dropdown-item' href='#'>Удалить</a></li>"
                        +   "</ul>"
                        + "</div>";
                }
            },
            {
                targets: [1],
                visible: false
            }
        ],
        "columns": [
            { data: "id" },
            { data: "id", name: "Id" },
            { data: "dateOfAcceptanceFromDepartment", name: "DateOfAcceptanceFromDepartment" },
            { data: "numberDepartment", name: "NumberDepartment" },
            { data: "patientFullName", name: "PatientFullName" },
            { data: "task", name: "Task" },
            { data: "whoDocumentSentTo", name: "WhoDocumentSentTo" },
            { data: "dateOfReferral", name: "DateOfReferral" },
            { data: "whatHasBeenDone", name: "WhatHasBeenDone" },
            { data: "dateOfTransferToDepartment", name: "DateOfTransferToDepartment" },
            { data: "performer", name: "Performer" },
            { data: "note", name: "Note" },
            { data: "createdAt", name: "CreatedAt" }
        ]
    });

    //Обновить данные в таблице
    $('#reload').on('click', function () {
        dataTable.ajax.reload();
    });

    //Редактировать задачу
    $('#table').on('click', '#menu-action #edit-todo', function (e) {
        console.log("Пробую редактировать");
        console.log(e.target); //получить dataset data.todoId, пример: e.target.closest('tr').dataset.id;
        console.log($(this).attr('data-todo-id'));

        //GetFormModal('', "Редактировать задачу");
    });
});