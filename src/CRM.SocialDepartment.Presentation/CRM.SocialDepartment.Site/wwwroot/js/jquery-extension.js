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

        handleErrorStatusCode: function (status) {
            switch (status) {
                case 400:
                    malomalsky.ajax.showError(malomalsky.ajax.defaultError400)
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
                    contentType: option.contentType || 'application/x-www-form-urlencoded',
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
                            malomalsky.ajax.handleErrorStatusCode(err.status);
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