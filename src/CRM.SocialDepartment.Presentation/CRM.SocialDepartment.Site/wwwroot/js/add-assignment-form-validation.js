// add-assignment-form-validation.js
// Модуль для валидации форм "Создать назначение" и "Редактировать назначение"

(function (global) {
    // --- Вспомогательные функции ---
    function clearValidationErrors() {
        $('.form-control, .form-select, .btn-group').removeClass('is-invalid');
        $('.invalid-feedback, .text-danger.field-validation-error').remove();
        $('.validation-errors').remove();
        $('.alert-danger').remove();
    }

    function addFieldErrorMessage($field, errorMessage) {
        $field.siblings('.invalid-feedback, .text-danger.field-validation-error').remove();

        const $errorSpan = $('<span class="text-danger field-validation-error"></span>');
        $errorSpan.text(errorMessage);

        if ($field.hasClass('btn-group')) {
            $field.after($errorSpan);
        } else {
            $field.after($errorSpan);
        }
    }

    function showFieldError(fieldName, errorMessage) {
        let $field = $(`[name="${fieldName}"], [id="${fieldName}"]`).first();

        if (!$field.length && fieldName === 'ForwardDepartment') {
            $field = $(`input[name="${fieldName}"]`).closest('.btn-group');
        }

        if (!$field.length) return;

        $field.addClass('is-invalid');

        let $errorSpan = $field.siblings('.invalid-feedback, .text-danger').first();
        if (!$errorSpan.length) {
            $errorSpan = $(`span[data-valmsg-for="${fieldName}"]`);
        }
        if (!$errorSpan.length) {
            addFieldErrorMessage($field, errorMessage);
        } else {
            $errorSpan.text(errorMessage).show();
        }
    }

    function highlightErrorFields(errors) {
        const displayNameToName = {
            'Название': 'Name',
            'Описание': 'Description',
            'Срок': 'DueDate',
            'Исполнитель': 'Assignee',
            'Номер отделения': 'DepartmentNumber',
            'Куда передано': 'ForwardDepartment',
            'Кому передано': 'ForwardDepartment',
            'Заметка': 'Note',
            'Пациент': 'PatientId',
            'Дата создания': 'CreatedDate',
            'Дата принятия': 'AcceptDate',
            'Дата передачи': 'ForwardDate',
            'Дата передачи в отделение': 'DepartmentForwardDate'
        };

        errors.forEach(function (error) {
            for (const [display, name] of Object.entries(displayNameToName)) {
                if (error.includes(display)) {
                    let $field = null;

                    switch (name) {
                        case 'Name':
                            $field = $('input[name="Name"]');
                            break;
                        case 'Description':
                            $field = $('textarea[name="Description"]');
                            break;
                        case 'DueDate':
                            $field = $('input[name="DueDate"]');
                            break;
                        case 'Assignee':
                            $field = $('input[name="Assignee"]');
                            break;
                        case 'DepartmentNumber':
                            $field = $('input[name="DepartmentNumber"]');
                            break;
                        case 'ForwardDepartment':
                            $field = $('input[name="ForwardDepartment"]');
                            if (!$field.length) {
                                $field = $('input[name="ForwardDepartment"]').closest('.btn-group');
                            }
                            break;
                        case 'Note':
                            $field = $('textarea[name="Note"]');
                            break;
                        case 'PatientId':
                            $field = $('input[name="PatientId"]');
                            break;
                        case 'CreatedDate':
                            $field = $('input[name="CreatedDate"]');
                            break;
                        case 'AcceptDate':
                            $field = $('input[name="AcceptDate"]');
                            break;
                        case 'ForwardDate':
                            $field = $('input[name="ForwardDate"]');
                            break;
                        case 'DepartmentForwardDate':
                            $field = $('input[name="DepartmentForwardDate"]');
                            break;
                    }

                    if ($field && $field.length) {
                        $field.addClass('is-invalid');
                        addFieldErrorMessage($field, error);
                    }
                    return; 
                }
            }

            if (error.includes('Название обязательно') || error.includes('Название не заполнено')) {
                const $field = $('input[name="Name"]');
                $field.addClass('is-invalid'); addFieldErrorMessage($field, error);
                return;
            }

            if (error.includes('Срок обязателен') || error.includes('Некорректный срок')) {
                const $field = $('input[name="DueDate"]');
                $field.addClass('is-invalid'); addFieldErrorMessage($field, error);
                return;
            }

            if (error.includes('Номер отделения') && (error.includes('должен быть') || error.includes('некорректн'))) {
                const $field = $('input[name="DepartmentNumber"]');
                $field.addClass('is-invalid'); addFieldErrorMessage($field, error);
                return;
            }

            if (error.toLowerCase().includes('пациент') && (error.includes('обязател') || error.includes('не найден'))) {
                const $field = $('input[name="PatientId"]');
                $field.addClass('is-invalid'); addFieldErrorMessage($field, error);
                return;
            }

            if (error.toLowerCase().includes('дата') && (error.includes('некоррект') || error.includes('раньше') || error.includes('позже'))) {
                const $dates = $('input[name="DueDate"], input[name="CreatedDate"], input[name="AcceptDate"], input[name="ForwardDate"], input[name="DepartmentForwardDate"]');
                $dates.addClass('is-invalid').each(function () { addFieldErrorMessage($(this), error); });
                return;
            }

            const $form = $('#create-assignment-form, #edit-assignment-form').first();
            if ($form.length) {
                const $summary = $('<div class="text-danger field-validation-error validation-errors"></div>');
                $summary.text(error);
                $form.prepend($summary);
            }
        });
    }

    function showValidationErrors(errors) {
        clearValidationErrors();
        if (!Array.isArray(errors) || errors.length === 0) return;

        let errorHtml = '<div style="text-align: left;">';
        errorHtml += '<h6 style="color: #dc2626; margin-bottom: 1rem;"><i class="fa fa-exclamation-triangle"></i> Ошибки валидации:</h6>';
        errorHtml += '<ul style="margin: 0; padding-left: 1.5rem; color: #991b1b;">';
        errors.forEach(function (error) {
            errorHtml += '<li style="margin-bottom: 0.5rem;">' + error + '</li>';
        });
        errorHtml += '</ul></div>';

        if (typeof Swal !== 'undefined') {
            Swal.fire({
                title: 'Ошибки валидации',
                html: errorHtml,
                icon: 'error',
                confirmButtonText: 'Понятно',
                confirmButtonColor: '#dc2626',
                width: '600px',
                customClass: {
                    popup: 'validation-error-popup',
                    htmlContainer: 'validation-error-content'
                }
            });
        } else {
            console.error('SweetAlert2 не найден');
        }

        highlightErrorFields(errors);
    }

    function testAllFormFields() {
        const fieldTests = [
            { name: 'Name', selector: 'input[name="Name"]' },
            { name: 'Description', selector: 'textarea[name="Description"]' },
            { name: 'DueDate', selector: 'input[name="DueDate"]' },
            { name: 'Assignee', selector: 'input[name="Assignee"]' },
            { name: 'DepartmentNumber', selector: 'input[name="DepartmentNumber"]' },
            { name: 'ForwardDepartment', selector: 'input[name="ForwardDepartment"]' },
            { name: 'Note', selector: 'textarea[name="Note"]' },
            { name: 'PatientId', selector: 'input[name="PatientId"]' },
            { name: 'CreatedDate', selector: 'input[name="CreatedDate"]' },
            { name: 'AcceptDate', selector: 'input[name="AcceptDate"]' },
            { name: 'ForwardDate', selector: 'input[name="ForwardDate"]' },
            { name: 'DepartmentForwardDate', selector: 'input[name="DepartmentForwardDate"]' }
        ];

        console.log('🧪 Тестирование полей формы (Assignments)');
        fieldTests.forEach(function (test) {
            const $field = $(test.selector);
            const found = $field.length > 0;
            console.log(`${found ? '✅' : '❌'} ${test.name}: ${found ? 'найдено' : 'НЕ НАЙДЕНО'}`);
            if (found) {
                console.log(`   - Тип: ${$field.prop('tagName')}`);
                console.log(`   - Классы: ${$field.attr('class') || 'нет'}`);
                console.log(`   - Значение: "${$field.val() || 'пустое'}"`);
            }
        });
    }

    function initErrorClearing() {
        $(document).on('input focus', '.form-control.is-invalid, .form-select.is-invalid, .btn-group.is-invalid', function () {
            $(this).removeClass('is-invalid');
            $(this).siblings('.invalid-feedback, .text-danger.field-validation-error').remove();
        });
        $(document).on('change', '.form-select.is-invalid', function () {
            $(this).removeClass('is-invalid');
            $(this).siblings('.invalid-feedback, .text-danger.field-validation-error').remove();
        });
    }

    global.AddAssignmentFormValidation = {
        clearValidationErrors,
        showValidationErrors,
        highlightErrorFields,
        addFieldErrorMessage,
        showFieldError,
        testAllFormFields,
        initErrorClearing
    };

    $(document).ready(function () {
        initErrorClearing();
    });

})(window);
