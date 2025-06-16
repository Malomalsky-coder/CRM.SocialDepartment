/*
    Назначение: Для страницы списка активных пациентов.
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

    //Получить модальное окно: Добавить пациента
    $('#add-patient').on('click', function () {
        GetFormModal(window.location.origin + '/patient/modal/create', 'Добавить пациента');
    });

    //Обновить данные в таблице
    $('#reload').on('click', function () {
        dataTable.ajax.reload();
    });

    //Получить модальное окно: Редактировать пациента
    $('#table').on('click', '#edit-patient-link', function (e) {
        var patientId = e.target.dataset.patientId
        GetFormModal(window.location.origin + '/patient/modal/edit/?patientId=' + patientId, 'Редактировать пациента');
    });

    //Отправить данные из модального окна: Добавить пациента
    $('#form-modal').on('submit', '#create-patient', function (e) {
        e.preventDefault();

        $this = $(this);
        var url = $this.attr('action');
        var data = $this.serialize(); //malomalsky.utils.json.formToJson($this);
        var headers = {
            "CSRF-TOKEN": $this.find('input[name="__RequestVerificationToken"]').val()
        };

        if (PostFormModal(url, data, headers)) {
            dataTable.ajax.reload();
        }
    });

    //Отправить данные из модального окна: Редактировать пациента
    $('#form-modal').on('submit', '#edit-patient', function (e) {
        e.preventDefault();

        $this = $(this);
        var url = $this.attr('action');
        var data = $this.serialize();
        var headers = {
            "CSRF-TOKEN": $this.find('input[name="__RequestVerificationToken"]').val()
        };

        if (PostFormModal(url, data, headers)) {
            dataTable.ajax.reload();
        }
    });

    //Удалить пациента. Вызов из модального окна: Редактирование пациента.
    $('#form-modal').on('click', '#delete-patient', function (e) {
        var patientId = e.target.dataset.patientId;
        if (DeleteFormModal(window.location.origin + '/api/patients/?patientId=' + patientId)) {
            dataTable.ajax.reload();
        }
    });

    //Удалить пациента. Вызов из таблицы.
    $('#table').on('click', '#delete-patient-link', function (e) {
        var patientId = e.target.dataset.patientId;
        if (HttpDelete(window.location.origin + '/api/patients/?patientId=' + patientId)) {
            dataTable.ajax.reload();
        }
    });

    // МОДАЛЬНОЕ ОКНО: "ДОБАВИТЬ ПАЦИЕНТА" ////////////////////////////////////////////////

    //Тип госпитализации
    $('#form-modal').on('change', '#HospitalizationType', function (e) {
        $selected = $(this).find(':selected').text();
        if ($selected === 'Добровольный' || $selected === 'Статья 435 УК РФ') {
            $('#ResolutionIsEnable').hide();
        }
        else if ($selected === 'Принудительно') {
            $('#ResolutionIsEnable').show();
        }
    });

    //Гражданство
    $('#form-modal').on('change', '#CitizenshipInfo_Citizenship', function (e) {
        $('#RegistrationIsEnable').show();
        $('#NoRegistrationIsEnable').show();
        $('#LbgIsEnable').hide();
        $('#DocumentIsEnable').show();

        $radioVal = $(this).val();

        if ($radioVal === 'РФ') {
            $('#CountryIsEnable').hide();
            $('#Country').val('Россия');
        }
        else if ($radioVal === 'Иностранец') {
            $('#NoRegistrationIsEnable').hide();
            $('#Country').val('');
            $('#CountryIsEnable').show();
        }
        else if ($radioVal === 'ЛБГ') {
            $('#RegistrationIsEnable').hide();
            $('#NoRegistrationIsEnable').hide();
            $('#CountryIsEnable').hide();
            $('#Country').val('');
            $('#LbgIsEnable').show();
            $('#DocumentIsEnable').hide();
        }
    });

    //БОМЖ
    $('#form-modal').on('change', '#NoRegistrationIsEnable', function (e) {
        console.log($(this));

        if (e.target.checked) {
            $('#EarlyRegistrationIsEnable').show();
            $('#RegistrationIsEnable').hide();
        }
        else {
            $('#EarlyRegistrationIsEnable').hide();
            $('#RegistrationIsEnable').show();
        }

    });

    //Появляющиеся поля для недееспособного
    $('#form-modal').on('change', '#IsCapable', function (e) {
        $('#CapableIsEnable').fadeToggle();
    });

    //Появляющиеся поля для пенсии
    $('#form-modal').on('change', '#PensionIsActive', function (e) {
        $('#PensionFieldsetIsEnable').fadeToggle();
    });

    //Появляющиеся поля для даты с какого числа пенсия
    $('#form-modal').on('change', '#DisabilityGroup', function (e) {
        if ($(this).find(':selected').text().includes('б/с')) {
            $('#PensionStartDateTimeIsEnable').show();
            return;
        }

        $('#PensionStartDateTimeIsEnable').hide();
    });

    ///////////////////////////////////////////////////////////////////////////////////////

    // ПЕЧАТЬ ДОКУМЕНТОВ //////////////////////////////////////////////////////////////////

    //Открыть модальное окно: Печать документов
    function printModalShow() {
        var patientId = $('#edit-form').find('#PatientId').val();
        $('#print-select').attr('data-patient-id', patientId);

        $('#form-modal').modal('hide'); //закрыть форму редактирования
        $('#form-print-modal').modal('show'); //открыть форму для выбора документа
    }

    //Обработчик для выбора файла на печать
    $('#print-select').change(function () {
        var patientId = $(this).attr('data-patient-id');
        var url = `/patients/docs?${patientId}&handler=`;
        var flag = false;

        switch ($(this).val()) {
            case '0':
                $('#print-form-data').html('');
                break;
            case '1':
                url += 'OneDoc'; //где OneDoc, обработчик OnGetOneDocAsync(int patientId) в файле: /Patients/Docs/Index.cshtml.cs
                flag = true;
                break;
            case '2':
                url += 'TwoDoc';
                flag = true;
                break;

            //Шаблон обработчика OPTION в SELECT
            //case '':
            //    url += '';
            //    flag = true;
            //    break;
        }

        if (flag) {
            jQueryPrintModalGet(url);
        }
    });

    //Неизвестно работает ли?
    //Получить модель для печати
    jQueryPrintModalGet = (url) => {
        try {
            $.ajax({
                type: 'GET',
                url: url,
                contentType: false,
                processData: false,
                success: function (res) {
                    $this = $('#print-form-data');
                    $this.html(res.html);
                },
                error: function (err) {
                    console.log(err);
                    alert('Возникла ошибка, обратитесь к системному администратору!');
                }
            });
        } catch (ex) {
            console.log(ex);
            alert('Возникла ошибка, обратитесь к системному администратору!');
        }
    };

    //Неизвестно работает ли?
    //Отправить данные для генерации документа
    printModalPost = async (form) => {
        $('#form-print-modal').find(':submit').attr('disabled', true);

        fetch(from.action, {
            method: 'POST',
            body: new FormData(form)
        })
            .then((response) => {
                response.blob();
            })
            .then((blob) => {
                $('#form-print-modal').modal('hide');
                $('#print-form-data').html(''); //очистить форму
                $('#print-select'). //сбросить выбор списка по умолчанию
                    $('#form-print-modal').find(':submit').attr('disabled', false);

                var url = URL.createObjectURL(blob);

                // Создаем ссылку
                var $a = $('<a></a>');
                $a.attr('href', url);
                //$a.attr('download', 'nameFile');
                // Добавляем ссылку в тело документа и кликаем по ней
                $('body').append($a);
                $a[0].click();

                // Удаляем ссылку после скачивания
                $a.remove();
                window.URL.revokeObjectURL(url);
            });


        //const response = await fetch(from.action, {
        //    method: 'POST',
        //    body: new FormData(form)
        //})

        //return response;
    };

    //устаревший - не используется
    jQueryPrintModalPost = form => {
        try {
            $.ajax({
                type: 'POST',
                url: form.action,
                data: new FormData(form),
                contentType: false,
                processData: false,
                beforeSend: function () {
                    $('#form-print-modal').find(':submit').attr('disabled', true);
                },
                success: function (data, status, xhr) {
                    const blob = new Blob([data], { type: xhr.getResponseHeader('Content-Type') });
                    const url = window.URL.createObjectURL(blob);

                    // Создаем ссылку
                    const $a = $('<a></a>');
                    $a.attr('href', url);
                    //$a.attr('download', 'nameFile');
                    // Добавляем ссылку в тело документа и кликаем по ней
                    $('body').append($a);
                    $a[0].click();

                    // Удаляем ссылку после скачивания
                    $a.remove();
                    window.URL.revokeObjectURL(url);

                    $('#form-print-modal').modal('hide');
                    $('#form-print-modal').find(':submit').attr('disabled', false);
                },
                error: function (err) {
                    alert('Возникла ошибка, обратитесь к системному администратору!');
                    console.log(err);
                }
            });
            return false;
        } catch (ex) {
            console.log(ex);
        }
    };
});