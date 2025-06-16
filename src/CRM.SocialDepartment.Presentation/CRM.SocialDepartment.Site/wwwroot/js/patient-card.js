$(document).ready(function () {

    // МОДАЛЬНЫЕ ОКНА //////////////////////////////////////////////////////////////////////////
    
    //Получить модальное окно: Редактировать пациента
    $('#edit-patient').on('click', function () {
        var winLocation = window.location;
        var locationSectionArr = String(winLocation).split('/');
        var patientId = locationSectionArr[locationSectionArr.length - 1];

        GetFormModal(winLocation.origin + '/api/patients/modal/edit/?patientId=' + patientId, 'Редактировать пациента');
    });

    //Отправить данные из модального окна: Создать новую задачу в списке дел
    $('#form-modal').on('submit', '#edit-patient', function (e) {
        e.preventDefault();

        $this = $(this);
        var url = $this.attr('action');
        var data = $this.serialize();
        var headers = {
            "CSRF-TOKEN": $this.find('input[name="__RequestVerificationToken"]').val()
        };

        PostFormModal(url, data, headers);

    });

    //Получить модальное окно: Добавить задачу в список дел
    $('#add-todo').on('click', function () {
        var winLocation = window.location;
        var locationSectionArr = String(winLocation).split('/');
        var patientId = locationSectionArr[locationSectionArr.length - 1];

        GetFormModal(winLocation.origin + '/api/todos/modal/create/?patientId=' + patientId, 'Добавить задачу');
    });

    //Отправить данные из модального окна: Создать новую задачу в списке дел
    $('#form-modal').on('submit', '#create-todo', function (e) {
        e.preventDefault();

        $this = $(this);
        var url = $this.attr('action');
        var data = $this.serialize();
        var headers = {
            "CSRF-TOKEN": $this.find('input[name="__RequestVerificationToken"]').val()
        };

        PostFormModal(url, data, headers);
    });

    //Удалить пациента вызывая метод из формы редактирования пациента
    $('#form-modal').on('click', '#delete-patient', function () {
        console.log('#delete-patient');

        //в случае успеха, сделать редирект на таблицу пациентов
    });

    // ПРОЧЕЕ ////////////////////////////////////////////////////////////////////////////////

    //Обработчик для поиска названия задачи из списка заданий
    $('#form-modal').on('click', '#Task', function () {
        //не нужно, выбрал другой способ реализации
        //$(this).select2({
        //    placeholder: "Выбрать задачу",
        //    //theme: "bootstrap4",
        //    allowClear: true,
        //    ajax: {
        //        url: "/api/todos/search/tasks",
        //        contentType: "application/json; charset=utf-8",
        //        data: function (params) {
        //            return {
        //                term: params.term,
        //            };
        //        },
        //        processResults: function (result) {
        //            return {
        //                results: $.map(result, function (item) {
        //                    return {
        //                        //id: item.Id,
        //                        text: item.Value
        //                    };
        //                }),
        //            };
        //        }
        //    }
        //});

        $(this).autocomplete({
            source: '/api/todos/search/tasks',
            delay: 500,
            minLength: 3,
        });
    });
});