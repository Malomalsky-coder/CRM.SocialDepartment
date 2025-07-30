/*
    Назначение: Обертка для SweetAlert2
    Версия: 1.0.0
*/

var malomalsky = malomalsky || {};

(function ($) {
    if (!$) {
        console.log('Модуль jQuery не подключен!');
        return;
    }

    if (!Swal) {
        console.log('Модуль sweetalert2 не подключен!');
        return;
    }

    // DEFAULTS ////////////////////////////////////////////////////

    malomalsky.libs = malomalsky.libs || {};

    malomalsky.libs.sweetAlert2 = {
        config: {
            'default': {

            },
            info: {
                icon: 'info'
            },
            success: {
                icon: 'success'
            },
            warn: {
                icon: 'warning'
            },
            error: {
                icon: 'error'
            }//,
            //confirm: {
            //    icon: 'warning',
            //    title: 'Вы уверены?',
            //    showCancelButton: true,
            //    reverseButtons: true
            //}
        }
    };

    // MESSAGES ///////////////////////////////////////////////////

    malomalsky.utils = malomalsky.utils || {};
    malomalsky.utils.htmlEscape = malomalsky.utils.htmlEscape || function (str) { return str; };

    var showMessage = function (type, title, message) {
        var opts = $.extend(
            {},
            malomalsky.libs.sweetAlert2.config['default'],
            malomalsky.libs.sweetAlert2.config[type],
            {
                title: title,
                text: message
                //todo: мб добавить html: malomalsky.utils.htmlEscape(message).replace(/\n/g, '<br>')
                //todo: мб добавить showConfirmButton: false,
            }
        );

        return $.Deferred(function ($dfd) {
            Swal.fire(opts).then(function () {
                $dfd.resolve();
            });
        });
    };

    malomalsky.message.info = function (title, message) {
        return showMessage('info', title, message);
    };

    malomalsky.message.success = function (title, message) {
        return showMessage('success', title, message);
    };

    malomalsky.message.warn = function (title, message) {
        return showMessage('warn', title, message);
    };

    malomalsky.message.error = function (message, title = 'Ошибка!') {
        return showMessage('error', title, message);
    };

    malomalsky.message.validationError = function (title, htmlContent) {
        var opts = $.extend(
            {},
            malomalsky.libs.sweetAlert2.config['default'],
            malomalsky.libs.sweetAlert2.config.error,
            {
                title: title,
                html: htmlContent, // Используем html вместо text для поддержки HTML
                width: '600px',
                confirmButtonText: 'Понятно',
                customClass: {
                    popup: 'validation-error-popup',
                    htmlContainer: 'validation-error-content'
                }
            }
        );

        return $.Deferred(function ($dfd) {
            Swal.fire(opts).then(function () {
                $dfd.resolve();
            });
        });
    };

})(jQuery)