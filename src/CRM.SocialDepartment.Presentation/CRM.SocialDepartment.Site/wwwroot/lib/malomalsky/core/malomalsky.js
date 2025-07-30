/*
    Назначение: Бибилиотека от Маломальского
    Версия: 1.0.0
*/

var malomalsky = malomalsky || {};

(function () {

    // MESSAGE //////////////////////////////////////////////////////////////

    malomalsky.message = malomalsky.message || {};

    malomalsky.message._showMessage = function (message, title) {
        alert((title || '') + ' ' + message);
    };

    malomalsky.message.info = function (title, message) {
        _showMessage(message, title);
    };

    malomalsky.message.success = function (title, message) {
        _showMessage(message, title);
    };

    malomalsky.message.warn = function (title, message) {
        _showMessage(message, title);
    };

    malomalsky.message.error = function (message, title = 'Ошибка!') {
        _showMessage(message, title);
    };

    // AJAX ////////////////////////////////////////////////////////////////

    malomalsky.ajax = malomalsky.ajax || {};

    malomalsky.ajax.request = function (url, method, options) {
        options = options || {};
        
        var settings = {
            url: url,
            type: method,
            dataType: options.dataType || 'json',
            contentType: options.contentType || 'application/x-www-form-urlencoded; charset=UTF-8',
            beforeSend: options.beforeSend || function () { },
            success: options.success || function (data) { },
            error: options.error || function (xhr, status, error) { },
            complete: options.complete || function () { }
        };

        if (options.data) {
            if (options.data instanceof FormData) {
                settings.data = options.data;
                settings.processData = false;
                settings.contentType = false;
            } else {
                settings.data = options.data;
            }
        }

        if (options.headers) {
            settings.headers = options.headers;
        }

        return $.ajax(settings);
    };

    malomalsky.ajax.handleErrorStatusCode = function (status, responseText) {
        var message = 'Произошла ошибка при выполнении запроса';
        
        switch (status) {
            case 400:
                message = 'Неверный запрос. Проверьте введенные данные.';
                break;
            case 401:
                message = 'Необходима авторизация.';
                break;
            case 403:
                message = 'Доступ запрещен.';
                break;
            case 404:
                message = 'Запрашиваемый ресурс не найден.';
                break;
            case 500:
                message = 'Внутренняя ошибка сервера.';
                break;
            case 0:
                message = 'Нет соединения с сервером.';
                break;
        }
        
        malomalsky.message.error(message);
    };

    // UTILS ////////////////////////////////////////////////////////////////

    malomalsky.utils = malomalsky.utils || {};

    //Escape HTML to help prevent XSS attacks.
    malomalsky.utils.htmlEscape = function (html) {
        return typeof html === 'string' ? html.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;') : html;
    };

})();