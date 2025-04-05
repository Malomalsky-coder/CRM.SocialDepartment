/*
    Назначение: Для страницы списка пользователей.
    Версия: 1.0.0
*/

$(document).ready(function () {

    var dataTable = $('#table').DataTable({
        "bAutoWidth": false,
        "processing": true,
        "serverSide": true,
        "filter": true,
        //"order": [[6, "asc"], [7, "asc"]],`
        "language": {
            "processing": "Загрузка данных...",
            "lengthMenu": "Показать _MENU_ записей на странице",
            "zeroRecords": "Ничего не найдено",
            "info": "Страница _PAGE_ из _PAGES_",
            "infoEmpty": "Нет записей",
            "infoFiltered": "(отфильтровано из _MAX_ записей)",
            "search": "Поиск:",
            "paginate": {
                "first": "В начало",
                "next": "Следующая",
                "previous": "Предыдущая",
                "last": "В конец"
            }
        },
        "lengthMenu": [50, 100, 250],
        "pageLength": 50,
        "pagingType": "full_numbers",
        "responsive": true,
        "ajax": {
            "url": "/User/GetAllUsers",
            //"type": "POST",
            //"contentType": "application/json",
            "datatype": "json"
        },
        "columns": [
            { data: "UserName", name: "UserName" },
            { data: "Email", name: "Email" },
            { data: "CreatedOn", name: "CreatedOn" },
        ]
    });

    //Обновить данные в таблице
    $('#reload').on('click', function () {
        dataTable.ajax.reload();
    });
});