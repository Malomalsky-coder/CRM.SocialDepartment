/*
    Назначение: Для страницы списка активных пациентов.
    Версия: 1.0.0
*/

$(document).ready(function () {

    //var dataTable = new DataTable('#table', {

    var dataTable = $('#table').DataTable({
        "bAutoWidth": false,
        "processing": true,
        "serverSide": true,
        "filter": true,
        //"order": [[6, "asc"], [7, "asc"]],
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
            "url": "/api/patients/active",
            "type": "POST",
            //"contentType": "application/json",
            "datatype": "json"
        },
        //"columnDefs": [
        //    //{
        //    //    "targets": [1],
        //    //    "visible": true,
        //    //    "searchable": false,
        //    //    orderable: false
        //    //}
        //    {
        //        targets: [0],
        //        orderable: false,
        //        render: function (patietnId) {
        //            return ""
        //                + "<div class='dropdown'>"
        //                + "<button class='btn btn-secondary btn-sm dropdown-toggle' type='button' data-bs-toggle='dropdown' aria-expanded='false'>Действие</button>"
        //                + "<ul class='dropdown-menu' data-popper-placement='bottom-start'>"
        //                + `<li><a class='dropdown-item' href='/patients/${patietnId}'>Посмотреть</a></li>`
        //                + `<li><a id='edit-patient-link' data-patient-id='${patietnId}' class='dropdown-item' href='#'>Редактировать</a></li>`
        //                + `<li><a id='delete-patient-link' data-patient-id='${patietnId}' class='dropdown-item' href='#'>Удалить</a></li>`
        //                + "</ul>"
        //                + "</div>";
        //        }
        //    },
        //    {
        //        targets: [1],
        //        visible: false
        //    }
        //],
        "columns": [
            //{
            //    "render": function (data, row) { return "<a href='#' class='btn btn-danger' onclick=DeleteCustomer('" + row.id + "'); >Delete</a>"; }
            //},
            //{ data: "patientId" },
            { data: "Id", name: "Id" },
            //{ data: "hospitalizationType", name: "HospitalizationType", orderable: false },
            //{ data: "resolution", name: "Resolution", orderable: false },
            //{ data: "numberDocument", name: "NumberDocument", orderable: false },
            //{ data: "dateOfReceipt", name: "DateOfReceipt", orderable: false },
            //{ data: "numberDepartment", name: "NumberDepartment" },
            { data: "FullName", name: "FullName" },
            //{ data: "birthday", name: "Birthday" },
            //{ data: "isChildren", name: "IsChildren" },
            //{ data: "citizenship", name: "Citizenship" },
            //{ data: "country", name: "Country" },
            //{ data: "registration", name: "Registration" },
            //{ data: "noRegistration", name: "NoRegistration" },
            //{ data: "earlyRegistration", name: "EarlyRegistration" },
            //{ data: "placeOfBirth", name: "PlaceOfBirth" },
            //{ data: "isCapable", name: "IsCapable" },
            //{ data: "pensionIsActive", name: "PensionIsActive" },
            //{ data: "disabilityGroup", name: "DisabilityGroup" },
            //{ data: "note", name: "Note" }
        ]
    });

    //Обновить данные в таблице
    $('#reload').on('click', function () {
        dataTable.ajax.reload();
    });
});