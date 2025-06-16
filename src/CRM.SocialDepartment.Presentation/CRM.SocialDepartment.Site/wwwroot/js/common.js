/*
    Назначение: Модуль для общего использования.
    Версия: 1.0.0
*/

var malomalsky = malomalsky || {};

// FORM MODAL /////////////////////////////////////////////////////////////

//Открыть модальное окно с запрашиваемой моделью
GetFormModal = (url, title) => {
    let options = {
        success: function (res) {
            $('#form-modal .modal-title').html(title);
            $('#form-modal .modal-body').html(res);
            $('#form-modal').modal('show');
        }
    }

    return malomalsky.ajax.request(url, 'GET', options);
}

//Отправить данные из модального окна
PostFormModal = (url, data, headers) => {
    let options = {
        headers: headers,
        data: data,
        //contentType: 'application/json', //убрал, проблема с парсингом json, поправлю позже
        beforeSend: function () {
            $("#form-modal").find(':submit').attr('disabled', true);
            $("#form-modal").find(':submit').html('<div class="spinner-border spinner-border-sm" role="status"></div>');
        },
        success: function (res) {
            $('#form-modal').modal('hide');
        },
        error: function (err) {
            if (err.status === 422) {
                $('#form-modal .modal-body').html(err.responseText);
                throw new Error;
            }

            malomalsky.ajax.handleErrorStatusCode(err.status);
            throw new Error;
        }
    }
    
    return malomalsky.ajax.request(url, 'POST', options);
}

//Удалить ресурс. Вызов из модального окна
DeleteFormModal = (url) => {
    let options = {
        beforeSend: function () {
            $("#form-modal").find(':submit').attr('disabled', true);
            $("#form-modal").find(':submit').html('<div class="spinner-border spinner-border-sm" role="status"></div>');
        },
        success: function (res) {
            $('#form-modal').modal('hide');
        }
    }

    return malomalsky.ajax.request(url, 'DELETE', options);
};

//Удалить ресурс. Из любого другого места.
HttpDelete = (url) => {
    return malomalsky.ajax.request(url, 'DELETE');
};

//переименовать в ExecuteDelete
GetRequestPathDelete = url => {
    if (confirm('Вы действительно хотите удалить запись?')) {
        try {
            $.ajax({
                type: 'GET',
                url: url,
                contentType: false,
                processData: false,
                success: function (res) {
                    //$('#viewAll').html(res.html);
                    $('#form-modal').modal('hide');
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
    }
    return false;
}