/*
    Назначение: Модуль для общего использования.
    Версия: 1.0.0
*/

var malomalsky = malomalsky || {};

// FORM MODAL /////////////////////////////////////////////////////////////

//Открыть модальное окно с запрашиваемой моделью
GetFormModal = (url, title) => {
    let options = {
        dataType: 'html',
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
            $("#form-modal").find(':submit').html('Сохранить');
            $("#form-modal").find(':submit').attr('disabled', false);

            // Обработка ошибок валидации (400 и 422)
            if (err.status === 422) {
                $('#form-modal .modal-body').html(err.responseText);
                throw new Error;
            }
            
            if (err.status === 400) {
                // Пытаемся распарсить JSON ответ с ошибками валидации
                try {
                    var response = JSON.parse(err.responseText);
                    if (response && response.Data && response.Data.Errors && Array.isArray(response.Data.Errors)) {
                        // Используем ValidationHelper для улучшенного отображения ошибок
                        var errorHtml = '';
                        if (typeof ValidationHelper !== 'undefined') {
                            errorHtml = ValidationHelper.createErrorHtml(response.Data.Errors);
                            errorHtml += ValidationHelper.createRecommendationsHtml(response.Data.Errors);
                        } else {
                            // Fallback на простое отображение, если ValidationHelper не загружен
                            errorHtml = '<div class="alert alert-danger" role="alert">';
                            errorHtml += '<h6 class="alert-heading">Ошибки валидации:</h6>';
                            errorHtml += '<ul class="mb-0">';
                            response.Data.Errors.forEach(function(error) {
                                errorHtml += '<li>' + malomalsky.utils.htmlEscape(error) + '</li>';
                            });
                            errorHtml += '</ul></div>';
                        }
                        
                        // Показываем ошибки через SweetAlert2
                        if (typeof malomalsky !== 'undefined' && malomalsky.message && malomalsky.message.validationError) {
                            malomalsky.message.validationError('Исправьте ошибки в форме', errorHtml);
                        } else {
                            // Fallback на добавление в модальное окно если SweetAlert2 недоступен
                            var existingContent = $('#form-modal .modal-body').html();
                            $('#form-modal .modal-body').html(errorHtml + existingContent);
                        }
                        
                        // Подсвечиваем поля с ошибками
                        if (typeof malomalsky !== 'undefined' && malomalsky.ajax && malomalsky.ajax.highlightErrorFields) {
                            malomalsky.ajax.highlightErrorFields(response.Data.Errors);
                        }
                        
                        throw new Error;
                    }
                } catch (parseError) {
                    console.log('Не удалось распарсить ответ с ошибками:', parseError);
                }
            }

            // Fallback на общую обработку ошибок
            malomalsky.ajax.handleErrorStatusCode(err.status, err.responseText);
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