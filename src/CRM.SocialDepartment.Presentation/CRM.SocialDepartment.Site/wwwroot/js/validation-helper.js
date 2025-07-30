/*
    Назначение: Модуль для работы с ошибками валидации
    Версия: 1.0.0
*/

var ValidationHelper = ValidationHelper || {};

/**
 * Улучшает отображение ошибок валидации, делая их более понятными для пользователя
 * @param {Array} errors - массив ошибок от сервера
 * @returns {Array} - улучшенные ошибки
 */
ValidationHelper.improveErrorMessages = function(errors) {
    if (!Array.isArray(errors)) {
        return errors;
    }

    return errors.map(function(error) {
        // Новые улучшенные сообщения от сервера уже хорошие - не трогаем их
        if (error.includes("Необходимо заполнить номер документа:")) {
            return error; // Уже понятное сообщение от сервера
        }
        
        if (error.includes("должен быть в формате:") || error.includes("должен содержать")) {
            return error; // Уже понятное сообщение от сервера
        }
        
        if (error === "ФИО обязательно для заполнения" || 
            error === "Дата рождения обязательна для заполнения") {
            return error; // Уже понятное сообщение от сервера
        }
        
        // Обрабатываем устаревшие ошибки (fallback для старых версий)
        if (error === "The Number field is required.") {
            return "Необходимо заполнить номер документа";
        }
        
        if (error === "The value '' is invalid.") {
            return "Введено некорректное значение";
        }
        
        // Переводим другие общие ошибки
        if (error.includes("Number field is required")) {
            return "Необходимо заполнить номера документов";
        }
        
        if (error.includes("field is required")) {
            var fieldMatch = error.match(/The (\w+) field is required/);
            if (fieldMatch) {
                var fieldName = ValidationHelper.translateFieldName(fieldMatch[1]);
                return `Поле '${fieldName}' обязательно для заполнения`;
            }
        }
        
        // Возвращаем оригинальную ошибку, если не смогли улучшить
        return error;
    });
};

/**
 * Переводит названия полей на русский язык
 * @param {string} fieldName - английское название поля
 * @returns {string} - русское название поля
 */
ValidationHelper.translateFieldName = function(fieldName) {
    var translations = {
        'Number': 'Номер документа',
        'FullName': 'ФИО',
        'Birthday': 'Дата рождения',
        'Note': 'Примечание'
    };
    
    return translations[fieldName] || fieldName;
};

/**
 * Создает HTML для отображения ошибок валидации
 * @param {Array} errors - массив ошибок
 * @param {string} title - заголовок блока ошибок
 * @returns {string} - HTML код для отображения ошибок
 */
ValidationHelper.createErrorHtml = function(errors, title) {
    title = title || 'Ошибки валидации';
    
    if (!Array.isArray(errors) || errors.length === 0) {
        return '';
    }
    
    var improvedErrors = ValidationHelper.improveErrorMessages(errors);
    
    var html = '<div class="alert alert-danger validation-errors" role="alert">';
    html += '<h6 class="alert-heading"><i class="fa fa-exclamation-triangle"></i> ' + malomalsky.utils.htmlEscape(title) + '</h6>';
    
    if (improvedErrors.length === 1) {
        html += '<p class="mb-0">' + malomalsky.utils.htmlEscape(improvedErrors[0]) + '</p>';
    } else {
        html += '<ul class="mb-0">';
        improvedErrors.forEach(function(error) {
            html += '<li>' + malomalsky.utils.htmlEscape(error) + '</li>';
        });
        html += '</ul>';
    }
    
    html += '</div>';
    return html;
};

/**
 * Добавляет рекомендации для исправления ошибок
 * @param {Array} errors - массив ошибок
 * @returns {string} - HTML с рекомендациями
 */
ValidationHelper.createRecommendationsHtml = function(errors) {
    if (!Array.isArray(errors) || errors.length === 0) {
        return '';
    }
    
    var recommendations = [];
    var hasDocumentErrors = false;
    var hasPassportError = false;
    var hasPolicyError = false;
    var hasSnilsError = false;
    
    errors.forEach(function(error) {
        // Проверяем новые конкретные ошибки
        if (error.includes('Паспорт') || error.includes('паспорт')) {
            hasPassportError = true;
            hasDocumentErrors = true;
        }
        if (error.includes('Полис ОМС') || error.includes('полис')) {
            hasPolicyError = true;
            hasDocumentErrors = true;
        }
        if (error.includes('СНИЛС') || error.includes('снилс')) {
            hasSnilsError = true;
            hasDocumentErrors = true;
        }
        
        // Проверяем старые общие ошибки (fallback)
        if (error.includes('Number field is required') || error.includes('номер документа')) {
            hasDocumentErrors = true;
            if (!hasPassportError && !hasPolicyError && !hasSnilsError) {
                // Если конкретные ошибки не найдены, показываем все рекомендации
                hasPassportError = hasPolicyError = hasSnilsError = true;
            }
        }
    });
    
    if (hasDocumentErrors) {
        var html = '<div class="alert alert-info mt-2" role="alert">';
        html += '<h6 class="alert-heading"><i class="fa fa-info-circle"></i> Рекомендации по заполнению:</h6>';
        html += '<ul class="mb-0">';
        
        if (hasPassportError) {
            html += '<li><strong>Паспорт:</strong> введите серию и номер в формате <code>1234 567890</code></li>';
        }
        if (hasPolicyError) {
            html += '<li><strong>Полис ОМС:</strong> введите номер полиса <code>16 цифр подряд</code></li>';
        }
        if (hasSnilsError) {
            html += '<li><strong>СНИЛС:</strong> введите номер в формате <code>123-456-789 01</code></li>';
        }
        
        html += '</ul>';
        html += '</div>';
        return html;
    }
    
    return '';
};

/**
 * Инициализирует автоматическую очистку ошибок при вводе в поля
 */
ValidationHelper.initErrorClearing = function() {
    $(document).on('input focus', '.form-control.is-invalid, .form-select.is-invalid', function() {
        $(this).removeClass('is-invalid');
        $(this).siblings('.invalid-feedback').remove();
    });
    
    // Очистка ошибок при изменении значения select
    $(document).on('change', '.form-select.is-invalid', function() {
        $(this).removeClass('is-invalid');
        $(this).siblings('.invalid-feedback').remove();
    });
};

// Автоматическая инициализация при загрузке DOM
$(document).ready(function() {
    if (typeof ValidationHelper !== 'undefined') {
        ValidationHelper.initErrorClearing();
    }
}); 