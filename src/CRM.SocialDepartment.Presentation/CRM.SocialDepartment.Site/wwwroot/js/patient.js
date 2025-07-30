/*
    Назначение: Для страницы списка активных пациентов.
    Версия: 3.2.0
*/

// Проверка наличия необходимых библиотек
console.log('🔍 Проверяем наличие библиотек:');
console.log('jQuery:', typeof $ !== 'undefined' ? '✅ Загружен' : '❌ Не загружен');
console.log('DataTables:', typeof $.fn.DataTable !== 'undefined' ? '✅ Загружен' : '❌ Не загружен');
if (typeof $.fn.DataTable !== 'undefined') {
    console.log('DataTables версия:', $.fn.dataTable.version);
}

// Универсальная функция для безопасного отображения сообщений
function showMessage(type, title, message) {
    if (window.malomalsky?.message?.[type] && typeof window.malomalsky.message[type] === 'function') {
        try {
            malomalsky.message[type](title, message);
            return;
        } catch (error) {
            console.error('Ошибка в malomalsky.message:', error);
        }
    }

        if (window.Swal) {
        const config = {
            title: title,
            text: message,
            confirmButtonText: 'OK'
        };

        switch (type) {
            case 'success': config.icon = 'success'; break;
            case 'error': config.icon = 'error'; break;
            case 'warning': config.icon = 'warning'; break;
            case 'info': config.icon = 'info'; break;
        }

        Swal.fire(config);
    } else {
        console.log(`${title}: ${message}`);
    }
}

// Функция для добавления пациента
function addPatient() {
    GetFormModal('/Patient/modal/create', 'Добавить пациента');
}

// Функция для редактирования пациента
function editPatient(patientId) {
    $.get(`/Patient/Edit/${patientId}`, function (data) {
        $('#edit-patient-modal .modal-body').html(data);
        $('#edit-patient-modal').modal('show');
    }).fail(function () {
        showMessage('error', 'Ошибка', 'Не удалось загрузить форму редактирования');
    });
}

// Функция для архивирования пациента
function archivePatient(patientId) {
    Swal.fire({
        title: 'Подтверждение',
        text: 'Вы уверены, что хотите архивировать этого пациента?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Да, архивировать',
        cancelButtonText: 'Отмена'
    }).then((result) => {
        if (result.isConfirmed) {
            $.post(`/Patient/Archive/${patientId}`)
                .done(function (response) {
                    if (response.success) {
                        showMessage('success', 'Успешно!', 'Пациент архивирован.');
                        if (window.patientDataTable) {
                            window.patientDataTable.ajax.reload();
                        }
} else {
                        showMessage('error', 'Ошибка!', response.message || 'Не удалось архивировать пациента.');
                    }
                })
                .fail(function () {
                    showMessage('error', 'Ошибка!', 'Ошибка сервера при архивировании пациента.');
                });
        }
    });
}

// Обновляем статус подключения
function updateConnectionStatus() {
    const isConnected = true; // В реальном приложении здесь должна быть проверка соединения
    $('#connection-status').html(
        isConnected
            ? '<i class="fa fa-circle text-success"></i><small>Подключено</small>'
            : '<i class="fa fa-circle text-warning"></i><small>Polling режим</small>'
    );
}

// Обновление времени последнего обновления
function updateLastUpdateTime() {
    const now = new Date();
    const timeString = now.toLocaleTimeString('ru-RU', {
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit'
    });
    $('#last-update').text(timeString);
}

// Обновление счетчика записей
function updateRecordsCount(count) {
    $('#records-badge').text(count || 0);
}

// Управление автообновлением данных
let autoRefreshInterval;
function startAutoRefresh() {
    if ($('#auto-refresh-setting').is(':checked') && window.patientDataTable) {
        stopAutoRefresh();
        autoRefreshInterval = setInterval(() => {
            window.patientDataTable.ajax.reload(null, false);
            updateLastUpdateTime();
        }, 30000);
    }
}

function stopAutoRefresh() {
    if (autoRefreshInterval) {
        clearInterval(autoRefreshInterval);
        autoRefreshInterval = null;
    }
}

// Загрузка сохраненных настроек
function loadSettings() {
    const savedSettings = localStorage.getItem('patientTableSettings');
    if (savedSettings) {
        try {
            const settings = JSON.parse(savedSettings);
            $('#page-length-setting').val(settings.pageLength || '50');
            $('#auto-refresh-setting').prop('checked', settings.autoRefresh !== false);
            $('#compact-mode-setting').prop('checked', settings.compactMode || false);
            $('#save-state-setting').prop('checked', settings.saveState !== false);
        } catch (e) {
            console.error('Ошибка при загрузке настроек:', e);
        }
    }
}

// Инициализация DataTable для пациентов
function initializePatientDataTable() {
    console.log('🔧 Начинаем инициализацию DataTable');
    
    // Проверяем наличие таблицы в DOM
    const $table = $('#table');
    console.log('📋 Таблица найдена в DOM:', $table.length > 0);
    if ($table.length === 0) {
        console.error('❌ Таблица с id="table" не найдена в DOM');
        return null;
    }
    
    if ($.fn.DataTable.isDataTable('#table')) {
        console.log('🔄 DataTable уже существует, уничтожаем старую');
        $('#table').DataTable().destroy();
        $('#table').empty();
    }

    // Создаем таблицу, если она не существует
    const url = '/api/patients/active';
    console.log('📡 URL для AJAX запроса:', url);
    
    try {
        const dataTable = $('#table').DataTable({
        processing: true,
        serverSide: true,
        ajax: {
            url: url,
            type: 'POST',
            dataType: 'json',
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            },
            data: function(d) {
                console.log('📤 Отправляемые данные DataTable:', d);
                return d;
            },
            beforeSend: function(xhr) {
                console.log('🚀 Отправляем AJAX запрос на:', url);
                console.log('🔍 Параметры запроса:', this.data);
            },
            dataSrc: function (json) {
                console.log('📦 Получен ответ от сервера:', json);
                
                // Проверяем, что json является объектом
                if (!json || typeof json !== 'object') {
                    console.error('❌ Неверный формат ответа от сервера');
                    return [];
                }
                
                // Проверяем наличие ошибки в ответе
                if (json.error) {
                    console.error('❌ Сервер вернул ошибку:', json.error);
                    showMessage('error', 'Ошибка', json.error);
                    return [];
                }
                
                // Проверяем, есть ли данные
                if (json.data && Array.isArray(json.data)) {
                    console.log('✅ Получено {Count} записей', json.data.length);
                    updateRecordsCount(json.recordsTotal || 0);
                    updateLastUpdateTime();
                    return json.data;
                }
                
                console.log('📭 Получен пустой результат от сервера');
                updateRecordsCount(0);
                updateLastUpdateTime();
                return [];
            },
            error: function (xhr, status, error) {
                console.error('❌ Ошибка AJAX запроса:', {xhr, status, error});
                
                // Показываем сообщение об ошибке только если это не поисковый запрос
                if (status !== 'abort') {
                    showMessage('error', 'Ошибка', 'Не удалось загрузить данные пациентов');
                }
                
                // Возвращаем пустой массив для предотвращения ошибок DataTable
                return [];
            }
        },
        columns: [
            {
                data: 'id',
                title: 'Действия',
                orderable: false,
                searchable: false,
                render: function (data, type, row) {
                    return `
                        <div class="dropdown">
                            <button class="btn btn-secondary btn-sm dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                                Действие
                            </button>
                            <ul class="dropdown-menu">
                                <li><a class="dropdown-item btn-view-patient" href="#" data-patient-id="${data}">Посмотреть</a></li>
                                <li><a class="dropdown-item btn-edit-patient" href="#" data-patient-id="${data}">Редактировать</a></li>
                                <li><a class="dropdown-item btn-archive-patient" href="#" data-patient-id="${data}">Архивировать</a></li>
                            </ul>
                        </div>
                    `;
                }
            },
            {
                data: 'id',
                title: 'ID',
                visible: false,
                searchable: false
            },
            { data: 'hospitalizationType', className: 'text-center' },
            { data: 'resolution', className: 'text-center' },
            { data: 'medicalHistoryNumber', className: 'text-center' },
            { data: 'dateOfReceipt', className: 'text-center' },
            { data: 'department', className: 'text-center' },
            { data: 'fullName', className: 'fw-medium' },
            { data: 'birthday', className: 'text-center' },
            { data: 'isChildren', className: 'text-center' },
            { data: 'citizenship', className: 'text-center' },
            { data: 'country', className: 'text-center' },
            { data: 'registration', className: 'fw-medium' },
            { data: 'notRegistered', className: 'text-center' },
            { data: 'earlyRegistration', className: 'fw-medium' },
            { data: 'placeOfBirth', className: 'text-center' },
            {
                data: 'IsCapable',
                className: 'text-center',
                render: function (data) {
                    return data ?
                        '<span class="badge bg-success">Дееспособен</span>' :
                        '<span class="badge bg-warning">Недееспособен</span>';
                }
            },
            { data: 'ReceivesPension', className: 'text-center' },
            { data: 'DisabilityGroup', className: 'text-center' },
            { data: 'Note', className: 'fw-medium' }
        ],
        order: [[7, 'asc']],
        pageLength: 100,
        lengthMenu: [[10, 25, 50, 100, -1], [10, 25, 50, 100, 'Все']],
        language: {
            url: '/lib/datatables.net-bs5/language/ru.json'
        },
        dom: '<"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6"f>>' +
               '<"row"<"col-sm-12"tr>>' +
               '<"row"<"col-sm-12 col-md-5"i><"col-sm-12 col-md-7"p>>',
        responsive: true,
        initComplete: function () {
            console.log('✅ DataTable инициализирована');
            
            // Добавляем обработчик для поиска
            this.api().on('search.dt', function () {
                console.log('🔍 Выполняется поиск в таблице');
            });
            
            // Добавляем обработчик для обработки пустых результатов
            this.api().on('xhr.dt', function () {
                console.log('📊 XHR запрос завершен');
            });
        }
    });

    window.patientDataTable = dataTable;
    console.log('💾 DataTable сохранена в window.patientDataTable');
    console.log('🔍 DataTable объект:', dataTable);
    return dataTable;
    } catch (error) {
        console.error('❌ Ошибка при инициализации DataTable:', error);
        showMessage('error', 'Ошибка', 'Не удалось инициализировать таблицу данных пациентов.');
        return null;
    }
}

// Функция для форматирования даты в формат dd.MM.yyyy
function formatDate(date) {
    if (!date) return '';

    const d = new Date(date);
    if (isNaN(d.getTime())) return '';

    const day = String(d.getDate()).padStart(2, '0');
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const year = d.getFullYear();

    return `${day}.${month}.${year}`;
}

// Функция для преобразования даты из формата dd.MM.yyyy в ISO формат для input type="date"
function formatDateForInput(dateString) {
    if (!dateString) return '';

    const parts = dateString.split('.');
    if (parts.length !== 3) return '';

    const day = parts[0];
    const month = parts[1];
    const year = parts[2];

    return `${year}-${month}-${day}`;
}

// Обработчики событий
$(document).ready(function () {
    console.log('🚀 Document ready, начинаем инициализацию');
    
    // Проверяем загрузку всех необходимых библиотек
    if (typeof $ === 'undefined') {
        console.error('❌ jQuery не загружен');
        return;
    }
    
    if (typeof $.fn.DataTable === 'undefined') {
        console.error('❌ DataTables не загружен');
        return;
    }
    
    console.log('✅ Все необходимые библиотеки загружены');
    
    // Инициализация
    loadSettings();
    updateConnectionStatus();
    updateLastUpdateTime();

    // Инициализация DataTable
    console.log('📊 Инициализируем DataTable');
    const dataTableResult = initializePatientDataTable();
    if (dataTableResult) {
        console.log('✅ DataTable успешно инициализирована');
        startAutoRefresh();
    } else {
        console.error('❌ Не удалось инициализировать DataTable');
    }

    // Обработчик кнопки "Добавить пациента"
    $(document).on('click', '#add-patient', function (e) {
        e.preventDefault();
        addPatient();
    });

    //Отправить данные из модального окна: Добавить пациента
    $('#form-modal').on('submit', '#create-patient', function (e) {
        e.preventDefault();

        $this = $(this);
        var url = $this.attr('action');
        
        // Обработка дат перед отправкой
        var formData = new FormData($this[0]);
        var data = new URLSearchParams();
        
        // Преобразуем FormData в URLSearchParams с правильной обработкой дат
        for (var pair of formData.entries()) {
            var key = pair[0];
            var value = pair[1];
            
            // Обрабатываем поля с датами
            if (key === 'Birthday' || key === 'MedicalHistory.DateOfReceipt' || 
                key === 'CitizenshipInfo.PlaceOfBirth' || key === 'Capable.TrialDate') {
                if (value) {
                    // Преобразуем дату в правильный формат для сервера
                    var date = new Date(value);
                    if (!isNaN(date.getTime())) {
                        // Форматируем дату в ISO формат для сервера
                        var year = date.getFullYear();
                        var month = String(date.getMonth() + 1).padStart(2, '0');
                        var day = String(date.getDate()).padStart(2, '0');
                        value = `${year}-${month}-${day}`;
                    }
                }
            }
            
            data.append(key, value);
        }
        
        var headers = {
            "CSRF-TOKEN": $this.find('input[name="__RequestVerificationToken"]').val()
        };

        // Очищаем предыдущие ошибки
        window.AddPatientFormValidation.clearValidationErrors();

        // Отправляем данные
        $.ajax({
            url: url,
            type: 'POST',
            data: data.toString(),
            contentType: 'application/x-www-form-urlencoded',
            headers: headers,
            beforeSend: function () {
                $("#form-modal").find(':submit').attr('disabled', true);
                $("#form-modal").find(':submit').html('<div class="spinner-border spinner-border-sm" role="status"></div>');
            },
            success: function (response) {
                $('#form-modal').modal('hide');
                
                // Показываем сообщение об успехе
                if (typeof malomalsky !== 'undefined' && malomalsky.message && malomalsky.message.success) {
                    malomalsky.message.success('Успешно!', 'Пациент добавлен');
                } else {
                    showMessage('success', 'Успешно!', 'Пациент добавлен');
                }
                
                // Обновляем таблицу
                if (window.patientDataTable) {
                    window.patientDataTable.ajax.reload();
                }
            },
            error: function (xhr) {
                $("#form-modal").find(':submit').html('Сохранить');
                $("#form-modal").find(':submit').attr('disabled', false);

                // Обработка ошибок валидации (400)
                if (xhr.status === 400) {
                    try {
                        var response = JSON.parse(xhr.responseText);
                        console.log('📋 Получен ответ с ошибками валидации:', response);
                        
                        if (response && response.Data && response.Data.Errors && Array.isArray(response.Data.Errors)) {
                            console.log('🔍 Ошибки валидации:', response.Data.Errors);
                            
                            // Тестируем все поля формы для отладки
                            AddPatientFormValidation.testAllFormFields();
                            
                            AddPatientFormValidation.showValidationErrors(response.Data.Errors);
                            return;
                        }
                    } catch (parseError) {
                        console.log('❌ Не удалось распарсить ответ с ошибками:', parseError);
                    }
                }

                // Общая обработка ошибок
                var errorMessage = 'Произошла ошибка при добавлении пациента';
                if (xhr.responseText) {
                    try {
                        var response = JSON.parse(xhr.responseText);
                        if (response && response.ErrorMessage) {
                            errorMessage = response.ErrorMessage;
                        }
                    } catch (e) {
                        // Игнорируем ошибки парсинга
                    }
                }

                if (typeof malomalsky !== 'undefined' && malomalsky.message && malomalsky.message.error) {
                    malomalsky.message.error('Ошибка!', errorMessage);
                } else {
                    showMessage('error', 'Ошибка!', errorMessage);
                }
            }
        });
    });

    // Очистка ошибок при вводе в поля
    $(document).on('input focus', '.form-control.is-invalid, .form-select.is-invalid', function() {
        $(this).removeClass('is-invalid');
        $(this).siblings('.invalid-feedback, .text-danger.field-validation-error').remove();
    });

    // Очистка ошибок при изменении значения select
    $(document).on('change', '.form-select.is-invalid', function() {
        $(this).removeClass('is-invalid');
        $(this).siblings('.invalid-feedback, .text-danger.field-validation-error').remove();
    });

    // Обработчик кнопки "Обновить данные"
    $(document).on('click', '#reload-data', function (e) {
        e.preventDefault();
        if (window.patientDataTable) {
            window.patientDataTable.ajax.reload(function () {
                updateLastUpdateTime();
            }, false); // false - чтобы сохранить текущую страницу и сортировку
        }
    });

    // Обработчик кнопки "Полноэкранный режим"
    $(document).on('click', '#fullscreen-toggle', function (e) {
        e.preventDefault();
        e.stopPropagation();

        const $container = $('.modern-datatable-container');
        const $icon = $(this).find('i');

        if (!$container.hasClass('fullscreen-mode')) {
            $container.addClass('fullscreen-mode');
            $('body').addClass('datatable-fullscreen');
            $icon.removeClass('fa-expand').addClass('fa-compress');

            // Фиксируем позиционирование
            $container.css({
                'position': 'fixed',
                'top': '0',
                'left': '0',
                'right': '0',
                'bottom': '0',
                'z-index': '1040',
                'background': '#fff'
            });

            // Перерисовываем таблицу
            if (window.patientDataTable) {
                setTimeout(function () {
                    window.patientDataTable.columns.adjust().draw();
                }, 200);
            }
        } else {
            $container.removeClass('fullscreen-mode');
            $('body').removeClass('datatable-fullscreen');
            $icon.removeClass('fa-compress').addClass('fa-expand');

            // Восстанавливаем позиционирование
            $container.css({
                'position': '',
                'top': '',
                'left': '',
                'right': '',
                'bottom': '',
                'z-index': '',
                'background': ''
            });

            // Перерисовываем таблицу
            if (window.patientDataTable) {
                setTimeout(function () {
                    window.patientDataTable.columns.adjust();
                    window.patientDataTable.draw();
                }, 300);
            }
        }
    });

    // Обработчик кнопки "Действие"
    $(document).on('click', '.dropdown-toggle', function (e) {
        e.preventDefault();
        e.stopPropagation();

        // Закрываем все открытые dropdown
        $('.dropdown-menu').not($(this).next('.dropdown-menu')).hide();

        // Открываем/закрываем текущий dropdown
        $(this).next('.dropdown-menu').toggle();
    });

    // Закрытие dropdown при клике вне его
    $(document).on('click', function (e) {
        if (!$(e.target).closest('.dropdown').length) {
            $('.dropdown-menu').hide();
        }
    });

    // Обработчики кнопок действий в таблице (делегирование событий)
    $(document).on('click', '.btn-view-patient', function () {
        const patientId = $(this).data('patient-id');
        window.location.href = `/Patient/Card/${patientId}`;
    });

    $(document).on('click', '.btn-edit-patient', function () {
        const patientId = $(this).data('patient-id');
        editPatient(patientId);
    });

    $(document).on('click', '.btn-archive-patient', function () {
        const patientId = $(this).data('patient-id');
        archivePatient(patientId);
    });

    // Настройки таблицы
    $('#apply-settings').on('click', function () {
        const pageLength = $('#page-length-setting').val();
        const autoRefresh = $('#auto-refresh-setting').is(':checked');
        const compactMode = $('#compact-mode-setting').is(':checked');

        if (window.patientDataTable) {
            $('#table').toggleClass('table-sm', compactMode);

            const settings = {
                pageLength: pageLength,
                autoRefresh: autoRefresh,
                compactMode: compactMode
            };
            localStorage.setItem('patientTableSettings', JSON.stringify(settings));

            window.patientDataTable.page.len(pageLength === '-1' ? -1 : parseInt(pageLength)).draw();

            if (autoRefresh) {
                startAutoRefresh();
            } else {
                stopAutoRefresh();
            }
        }

        $('#table-settings-modal').modal('hide');
    });

    // Горячие клавиши
    $(document).on('keydown', function (e) {
        if (!$(e.target).is('input, textarea, select')) {
            if (e.ctrlKey && e.key === 'n') {
                e.preventDefault();
                $('#add-patient').click();
            } else if (e.key === 'F5') {
                e.preventDefault();
                $('#reload-data').click();
            } else if (e.key === 'F11') {
                e.preventDefault();
                $('#fullscreen-toggle').click();
            } else if (e.key === 'Escape' && $('.modern-datatable-container').hasClass('fullscreen-mode')) {
                e.preventDefault();
                $('#fullscreen-toggle').click();
            }
        }
    });

    console.log('Инициализация завершена');
    console.log('DataTable instance:', window.patientDataTable);
    console.log('AJAX URL:', window.patientDataTable?.ajax?.url());

    });