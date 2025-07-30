// add-patient-form-validation.js
// Модуль для валидации формы "Добавить пациента"

(function(global) {
    // --- Вспомогательные функции ---
    function clearValidationErrors() {
        $('.form-control, .form-select').removeClass('is-invalid');
        $('.invalid-feedback, .text-danger.field-validation-error').remove();
        $('.validation-errors').remove();
        $('.alert-danger').remove();
    }

    function addFieldErrorMessage($field, errorMessage) {
        $field.siblings('.invalid-feedback, .text-danger.field-validation-error').remove();
        const $errorSpan = $('<span class="text-danger field-validation-error"></span>');
        $errorSpan.text(errorMessage);
        $field.after($errorSpan);
    }

    function showFieldError(fieldName, errorMessage) {
        const $field = $(`[name="${fieldName}"], [id="${fieldName}"]`).first();
        if (!$field.length) return;
        $field.addClass('is-invalid');
        let $errorSpan = $field.siblings('.invalid-feedback, .text-danger').first();
        if (!$errorSpan.length) {
            $errorSpan = $(`span[data-valmsg-for="${fieldName}"]`);
        }
        if (!$errorSpan.length) {
            $errorSpan = $('<span class="text-danger field-validation-error"></span>');
            $field.after($errorSpan);
        }
        $errorSpan.text(errorMessage).show();
    }

    // --- Основная функция подсветки полей ---
    function highlightErrorFields(errors) {
        const displayNameToName = {
            'ФИО': 'FullName',
            'Дата рождения': 'Birthday',
            'Опекун': 'Capable.Guardian',
            'Решение суда': 'Capable.CourtDecision',
            'Распоряжение о назначении опекуна': 'Capable.GuardianOrderAppointment',
            'Постановление': 'MedicalHistory.Resolution',
            'Номер истории болезни': 'MedicalHistory.NumberDocument',
            'Тип госпитализации': 'MedicalHistory.HospitalizationType',
            'Гражданство': 'CitizenshipInfo.Citizenship',
            'Список гражданств': 'CitizenshipInfo.Citizenships',
            'Имеющиеся документы': 'Documents',
            'СНИЛС': 'Documents[2].Number',
            'Полис ОМС': 'Documents[1].Number',
            'Паспорт': 'Documents[0].Number',
            'Номер отделения': 'MedicalHistory.NumberDepartment',
            'Дата поступления': 'MedicalHistory.DateOfReceipt',
            'Группа инвалидности': 'Pension.DisabilityGroup',
            'Дата установления статуса пенсионера': 'Pension.PensionStartDateTime',
            'Способ получения пенсии': 'Pension.PensionAddress',
            'Филиал СФР': 'Pension.SfrBranch',
            'Отделение СФР': 'Pension.SfrDepartment',
            'РСД': 'Pension.Rsd'
        };

        errors.forEach(function(error) {
            console.log('🔍 Обрабатываем ошибку:', error);
            
            // 1. Прямое сопоставление с русскими названиями полей
            for (const [display, name] of Object.entries(displayNameToName)) {
                if (error.includes(display)) {
                    console.log(`✅ Найдено соответствие: "${display}" -> "${name}"`);
                    
                    // Ищем поле по имени
                    var $field = null;
                    
                    if (name === 'FullName') {
                        $field = $('input[name="FullName"]');
                    } else if (name === 'Birthday') {
                        $field = $('input[name="Birthday"]');
                    } else if (name === 'MedicalHistory.HospitalizationType') {
                        $field = $('select[name="MedicalHistory.HospitalizationType"]');
                    } else if (name === 'MedicalHistory.NumberDepartment') {
                        $field = $('input[name="MedicalHistory.NumberDepartment"]');
                    } else if (name === 'MedicalHistory.Resolution') {
                        $field = $('input[name="MedicalHistory.Resolution"]');
                    } else if (name === 'MedicalHistory.NumberDocument') {
                        $field = $('input[name="MedicalHistory.NumberDocument"]');
                    } else if (name === 'MedicalHistory.DateOfReceipt') {
                        $field = $('input[name="MedicalHistory.DateOfReceipt"]');
                    } else if (name === 'CitizenshipInfo.Citizenship') {
                        $field = $('input[name="CitizenshipInfo.Citizenship"]').closest('.btn-group');
                    } else if (name === 'Capable.Guardian') {
                        $field = $('input[name="Capable.Guardian"]');
                    } else if (name === 'Capable.CourtDecision') {
                        $field = $('input[name="Capable.CourtDecision"]');
                    } else if (name === 'Capable.GuardianOrderAppointment') {
                        $field = $('input[name="Capable.GuardianOrderAppointment"]');
                    } else if (name === 'Pension.DisabilityGroup') {
                        $field = $('select[name="Pension.DisabilityGroup"]');
                    } else if (name === 'Pension.PensionStartDateTime') {
                        $field = $('input[name="Pension.PensionStartDateTime"]');
                    } else if (name === 'Pension.PensionAddress') {
                        $field = $('select[name="Pension.PensionAddress"]');
                    } else if (name === 'Pension.SfrBranch') {
                        $field = $('input[name="Pension.SfrBranch"]');
                    } else if (name === 'Pension.SfrDepartment') {
                        $field = $('input[name="Pension.SfrDepartment"]');
                    } else if (name === 'Pension.Rsd') {
                        $field = $('input[name="Pension.Rsd"]');
                    }
                    
                    if ($field && $field.length > 0) {
                        $field.addClass('is-invalid');
                        addFieldErrorMessage($field, error);
                        console.log(`✅ Подсвечено поле: ${name}`);
                    } else {
                        console.log(`❌ Поле не найдено: ${name}`);
                    }
                    return;
                }
            }
            
            // 2. Обработка специальных случаев
            if (error.includes('ФИО обязательно для заполнения')) {
                const $field = $('input[name="FullName"]');
                $field.addClass('is-invalid');
                addFieldErrorMessage($field, error);
                return;
            }
            
            if (error.includes('Дата рождения')) {
                const $field = $('input[name="Birthday"]');
                $field.addClass('is-invalid');
                addFieldErrorMessage($field, error);
                return;
            }
            
            if (error.includes('Тип госпитализации обязателен')) {
                const $field = $('select[name="MedicalHistory.HospitalizationType"]');
                $field.addClass('is-invalid');
                addFieldErrorMessage($field, error);
                return;
            }
            
            if (error.includes('Номер отделения')) {
                const $field = $('input[name="MedicalHistory.NumberDepartment"]');
                $field.addClass('is-invalid');
                addFieldErrorMessage($field, error);
                return;
            }
            
            if (error.includes('Постановление')) {
                const $field = $('input[name="MedicalHistory.Resolution"]');
                $field.addClass('is-invalid');
                addFieldErrorMessage($field, error);
                return;
            }
            
            if (error.includes('Номер истории болезни')) {
                const $field = $('input[name="MedicalHistory.NumberDocument"]');
                $field.addClass('is-invalid');
                addFieldErrorMessage($field, error);
                return;
            }
            
            if (error.includes('Дата поступления')) {
                const $field = $('input[name="MedicalHistory.DateOfReceipt"]');
                $field.addClass('is-invalid');
                addFieldErrorMessage($field, error);
                return;
            }
            
            if (error.includes('Гражданство обязательно')) {
                const $field = $('input[name="CitizenshipInfo.Citizenship"]').closest('.btn-group');
                $field.addClass('is-invalid');
                addFieldErrorMessage($field, error);
                return;
            }
            
            if (error.includes('Опекун')) {
                const $field = $('input[name="Capable.Guardian"]');
                $field.addClass('is-invalid');
                addFieldErrorMessage($field, error);
                return;
            }
            
            if (error.includes('Решение суда')) {
                const $field = $('input[name="Capable.CourtDecision"]');
                $field.addClass('is-invalid');
                addFieldErrorMessage($field, error);
                return;
            }
            
            if (error.includes('Распоряжение о назначении опекуна')) {
                const $field = $('input[name="Capable.GuardianOrderAppointment"]');
                $field.addClass('is-invalid');
                addFieldErrorMessage($field, error);
                return;
            }
            
            if (error.includes('Группа инвалидности')) {
                const $field = $('select[name="Pension.DisabilityGroup"]');
                $field.addClass('is-invalid');
                addFieldErrorMessage($field, error);
                return;
            }
            
            if (error.includes('Дата установления статуса пенсионера')) {
                const $field = $('input[name="Pension.PensionStartDateTime"]');
                $field.addClass('is-invalid');
                addFieldErrorMessage($field, error);
                return;
            }
            
            if (error.includes('Способ получения пенсии')) {
                const $field = $('select[name="Pension.PensionAddress"]');
                $field.addClass('is-invalid');
                addFieldErrorMessage($field, error);
                return;
            }
            
            if (error.includes('Филиал СФР')) {
                const $field = $('input[name="Pension.SfrBranch"]');
                $field.addClass('is-invalid');
                addFieldErrorMessage($field, error);
                return;
            }
            
            if (error.includes('Отделение СФР')) {
                const $field = $('input[name="Pension.SfrDepartment"]');
                $field.addClass('is-invalid');
                addFieldErrorMessage($field, error);
                return;
            }
            
            if (error.includes('РСД')) {
                const $field = $('input[name="Pension.Rsd"]');
                $field.addClass('is-invalid');
                addFieldErrorMessage($field, error);
                return;
            }
            
            // 3. Обработка улучшенных сообщений от сервера
            if (error.includes('Дата рождения указана некорректно')) {
                const $field = $('input[name="Birthday"]');
                $field.addClass('is-invalid');
                addFieldErrorMessage($field, error);
                return;
            }
            
            if (error.includes('Некорректная дата получения документа')) {
                const $field = $('input[name="MedicalHistory.DateOfReceipt"]');
                $field.addClass('is-invalid');
                addFieldErrorMessage($field, error);
                return;
            }
            
            if (error.includes('Некорректный номер медицинского документа')) {
                const $field = $('input[name="MedicalHistory.NumberDocument"]');
                $field.addClass('is-invalid');
                addFieldErrorMessage($field, error);
                return;
            }
            
            if (error.includes('Номер отделения должен быть от 1 до 30')) {
                const $field = $('input[name="MedicalHistory.NumberDepartment"]');
                $field.addClass('is-invalid');
                addFieldErrorMessage($field, error);
                return;
            }
            
            if (error.includes('Некорректный тип гражданства')) {
                const $field = $('input[name="CitizenshipInfo.Citizenship"]').closest('.btn-group');
                $field.addClass('is-invalid');
                addFieldErrorMessage($field, error);
                return;
            }
            
            if (error.includes('Некорректные данные о дееспособности')) {
                $('input[name^="Capable"]').addClass('is-invalid').each(function() {
                    addFieldErrorMessage($(this), error);
                });
                return;
            }
            
            if (error.includes('Некорректные данные о пенсии')) {
                $('input[name^="Pension"], select[name^="Pension"]').addClass('is-invalid').each(function() {
                    addFieldErrorMessage($(this), error);
                });
                return;
            }
            
            // 4. Fallback для документов
            if (error.includes('документ') || error.includes('Документ') || error.includes('Некорректный формат номера документа')) {
                $('input[name^="Documents"]').addClass('is-invalid').each(function() {
                    addFieldErrorMessage($(this), error);
                });
                return;
            }
            
            // 5. Обработка общих ошибок формата
            if (error.includes('должен содержать') || error.includes('должен быть в формате')) {
                // Это ошибки валидации документов
                $('input[name^="Documents"]').addClass('is-invalid').each(function() {
                    addFieldErrorMessage($(this), error);
                });
                return;
            }
            
            console.log(`❌ Не удалось сопоставить ошибку: "${error}"`);
        });
    }

    function showValidationErrors(errors) {
        clearValidationErrors();
        if (!Array.isArray(errors) || errors.length === 0) return;
        // SweetAlert2
        let errorHtml = '<div style="text-align: left;">';
        errorHtml += '<h6 style="color: #dc2626; margin-bottom: 1rem;"><i class="fa fa-exclamation-triangle"></i> Ошибки валидации:</h6>';
        errorHtml += '<ul style="margin: 0; padding-left: 1.5rem; color: #991b1b;">';
        errors.forEach(function(error) {
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

    // --- Тестовая функция для отладки ---
    function testAllFormFields() {
        console.log('🧪 Тестирование всех полей формы');
        const fieldTests = [
            { name: 'FullName', selector: 'input[name="FullName"]' },
            { name: 'Birthday', selector: 'input[name="Birthday"]' },
            { name: 'MedicalHistory.NumberDocument', selector: 'input[name="MedicalHistory.NumberDocument"]' },
            { name: 'MedicalHistory.NumberDepartment', selector: 'input[name="MedicalHistory.NumberDepartment"]' },
            { name: 'MedicalHistory.HospitalizationType', selector: 'select[name="MedicalHistory.HospitalizationType"]' },
            { name: 'Documents[0].Number (Паспорт)', selector: 'input[name="Documents[0].Number"]' },
            { name: 'Documents[1].Number (Полис ОМС)', selector: 'input[name="Documents[1].Number"]' },
            { name: 'Documents[2].Number (СНИЛС)', selector: 'input[name="Documents[2].Number"]' },
            { name: 'CitizenshipInfo.Citizenship', selector: 'input[name="CitizenshipInfo.Citizenship"]' },
            { name: 'Capable.CourtDecision', selector: 'input[name="Capable.CourtDecision"]' },
            { name: 'Pension.DisabilityGroup', selector: 'select[name="Pension.DisabilityGroup"]' }
        ];
        fieldTests.forEach(function(test) {
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

    // --- Автоматическая очистка ошибок при вводе ---
    function initErrorClearing() {
        $(document).on('input focus', '.form-control.is-invalid, .form-select.is-invalid', function() {
            $(this).removeClass('is-invalid');
            $(this).siblings('.invalid-feedback, .text-danger.field-validation-error').remove();
        });
        $(document).on('change', '.form-select.is-invalid', function() {
            $(this).removeClass('is-invalid');
            $(this).siblings('.invalid-feedback, .text-danger.field-validation-error').remove();
        });
    }

    // --- Экспорт ---
    global.AddPatientFormValidation = {
        clearValidationErrors,
        showValidationErrors,
        highlightErrorFields,
        addFieldErrorMessage,
        showFieldError,
        testAllFormFields,
        initErrorClearing
    };

    // --- Автоинициализация ---
    $(document).ready(function() {
        initErrorClearing();
    });

})(window);