/*
    Назначение: Модуль расширения jQuery
    Версия: 1.0.0
*/

var malomalsky = malomalsky || {};

(function ($) {

    // AJAX /////////////////////////////////////////////////////////////////
    malomalsky.ajax = malomalsky.ajax || {};

    $.extend(malomalsky.ajax, {
        defaultError: {
            title: 'Произошла ошибка!',
            message: 'Подробности об ошибке не отправлены сервером.'
        },

        defaultError400: {
            title: 'Ошибка запроса!',
            message: 'Некорректный запрос к серверу.'
        },

        defaultError401: {
            title: 'Вы неавторизованы!',
            message: 'Войдите в свою учетную запись.'
        },

        defaultError403: {
            title: 'Доступ запрещен!',
            message: 'Вам отказано в доступе, обратитесь к системному администратору.'
        },

        defaultError404: {
            title: 'Страница не найдена!',
            message: 'Данный ресурс не может быть найден на сервере.'
        },

        showError: function (error) {
            if (error.title) {
                return malomalsky.message.error(error.message, error.title);
            } else {
                return malomalsky.message.error(error.message || malomalsky.ajax.defaultError.message);
            }
        },

        showValidationErrors: function (errors) {
            if (!Array.isArray(errors) || errors.length === 0) {
                malomalsky.ajax.showError(malomalsky.ajax.defaultError400);
                return;
            }

            // Используем ValidationHelper если он доступен
            var errorHtml = '';
            var recommendationsHtml = '';
            
            if (typeof ValidationHelper !== 'undefined') {
                errorHtml = ValidationHelper.createErrorHtml(errors, 'Ошибки валидации');
                recommendationsHtml = ValidationHelper.createRecommendationsHtml(errors);
            } else {
                // Fallback на простое отображение
                errorHtml = '<div style="text-align: left;"><ul style="margin: 0; padding-left: 20px;">';
                errors.forEach(function(error) {
                    errorHtml += '<li>' + malomalsky.utils.htmlEscape(error) + '</li>';
                });
                errorHtml += '</ul></div>';
            }

            // Комбинируем ошибки и рекомендации
            var fullHtml = errorHtml + recommendationsHtml;

            // Показываем через SweetAlert2 с HTML поддержкой
            malomalsky.message.validationError('Исправьте ошибки в форме', fullHtml);
            
            // Подсвечиваем поля с ошибками
            malomalsky.ajax.highlightErrorFields(errors);
        },

        highlightErrorFields: function (errors) {
            // Очищаем предыдущие подсветки
            $('.form-control, .form-select').removeClass('is-invalid');
            $('.invalid-feedback').remove();
            
            if (!Array.isArray(errors)) return;
            
            errors.forEach(function(error) {
                // Определяем какие поля подсвечивать на основе ошибок
                if (error.includes('ФИО') || error.includes('FullName')) {
                    $('input[name="FullName"]').addClass('is-invalid');
                }
                if (error.includes('Паспорт') || error.includes('passport')) {
                    $('input[name*="Documents"][name*="0"]').addClass('is-invalid');
                }
                if (error.includes('Полис ОМС') || error.includes('MedicalPolicy')) {
                    $('input[name*="Documents"][name*="1"]').addClass('is-invalid');
                }
                if (error.includes('СНИЛС') || error.includes('Snils')) {
                    $('input[name*="Documents"][name*="2"]').addClass('is-invalid');
                }
                if (error.includes('дата рождения') || error.includes('Birthday')) {
                    $('input[name="Birthday"]').addClass('is-invalid');
                }
            });
        },

        handleErrorStatusCode: function (status, responseText) {
            switch (status) {
                case 400:
                    // Пытаемся обработать детальные ошибки валидации
                    if (responseText) {
                        try {
                            var response = JSON.parse(responseText);
                            if (response && response.Data && response.Data.Errors && Array.isArray(response.Data.Errors)) {
                                // Используем ValidationHelper для улучшенного отображения ошибок
                                malomalsky.ajax.showValidationErrors(response.Data.Errors);
                                return;
                            }
                        } catch (parseError) {
                            console.log('Не удалось распарсить ответ с ошибками:', parseError);
                        }
                    }
                    // Fallback на общее сообщение если не удалось распарсить
                    malomalsky.ajax.showError(malomalsky.ajax.defaultError400);
                    break;
                case 401:
                    malomalsky.ajax.showError(malomalsky.ajax.defaultError401)
                    break;
                case 403:
                    malomalsky.ajax.showError(malomalsky.ajax.defaultError403);
                    break;
                case 404:
                    malomalsky.ajax.showError(malomalsky.ajax.defaultError404);
                    break;
                default:
                    malomalsky.ajax.showError(malomalsky.ajax.defaultError);
                    break;
            }
        },

        //Общий метод для выполнения AJAX запроса
        request: function (url, httpMethod, options) {
            /*
                --- Пример использования: --------------------------------------------
                let form = {...};

                let options = {
                    headers: {
                        "key": "value"
                    },
                    data: new FormData(form),
                    beforeSend: function () {
                        //какая-то операция до запроса
                    },
                    success = function(res) {
                        //обработка успешного запроса
                    },
                    statusCode400Handle: function (res) {
                        //обработка ошибок
                    },
                    error = function(err) {
                        //обработка ошибок
                    }
                }

                _httpRequest('/path', 'GET', options);

                --- Примечание -------------------------------------------------------

                Добавить:
                * В options => dataType: string и contentType: string в частности для работы с 'application/json'
                ----------------------------------------------------------------------
            */

            if (httpMethod === 'DELETE') {
                //todo: реализовать через sweetAlert2 ???
                if (!confirm('Вы действительно хотите удалить запись?')) {
                    return;
                }
            }

            return _httpRequest(url, httpMethod, options);

            //* ---------------------------------------------------------------------- */
            function _httpRequest(url, httpMethod, options) {
                var option = options || {};

                var deferred = $.ajax({
                    type: httpMethod || "GET",
                    url: url,
                    headers: option.headers || {},
                    data: option.data || null,
                    dataType: option.dataType || 'html',
                    contentType: option.contentType || 'application/x-www-form-urlencoded',
                    timeout: 30000, // 30 секунд (в миллисекундах)
                    processData: false,
                    beforeSend: function () {
                        if (typeof option.beforeSend === 'function') {
                            option.beforeSend();
                        }
                    },
                    success: function (res) {
                        if (typeof option.success === 'function') {
                            option.success(res);
                        }
                        //console.log('success');
                    },
                    error: function (err) { //request, status, error
                        //err.status, err.statusText, err.responseText

                        if (typeof option.error === 'function') {
                            option.error(err);
                        }
                        else {
                            malomalsky.ajax.handleErrorStatusCode(err.status, err.responseText);
                            throw new Error;
                        }
                    }
                });

                deferred
                    //.done(function () {
                    //    console.log('done');
                    //    return true;
                    //})
                    .catch(function () {
                        //console.log('catch');
                        return false;
                    });
                    //.fail(function () {
                    //    console.log('fail');
                    //    return false;
                    //});

                return true;
            }
        }
    });

    // UTILS ///////////////////////////////////////////////////////////////
    malomalsky.utils = malomalsky.utils || {};
    malomalsky.utils.json = malomalsky.utils.json || {};

    //Html form convert to json
    malomalsky.utils.json.formToJson = function (form) {
        let unindexed_array = form.serializeArray();
        let indexed_array = {};

        $.map(unindexed_array, function (n, i) {
            indexed_array[n['name']] = n['value'];
        });

        return JSON.stringify(indexed_array);
    };

})(jQuery);