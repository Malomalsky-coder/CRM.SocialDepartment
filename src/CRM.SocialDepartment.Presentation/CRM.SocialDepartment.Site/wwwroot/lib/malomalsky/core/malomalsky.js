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

    // UTILS ////////////////////////////////////////////////////////////////

    malomalsky.utils = malomalsky.utils || {};

    //Escape HTML to help prevent XSS attacks.
    malomalsky.utils.htmlEscape = function (html) {
        return typeof html === 'string' ? html.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;') : html;
    };

})();