/*
    Назначение: Для страницы списка задач (assignments).
    Версия: 1.0.0
*/

console.log('Проверяем наличие библиотек:');
console.log('jQuery:', typeof $ !== 'undefined' ? 'Загружен' : 'Не загружен');
console.log('DataTables:', typeof $.fn.DataTable !== 'undefined' ? 'Загружен' : 'Не загружен');
if (typeof $.fn.DataTable !== 'undefined') {
    console.log('DataTables версия:', $.fn.dataTable.version);
}

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
        console.log(title + ': ' + message);
    }
}

const ASSIGNMENT_API = {
    listActive: '/api/assignments/active',   // POST
    createOrUpdate: '/api/assignments',      // POST (Create/Update)
    delete: (id) => `/api/assignments/${id}`,// DELETE
    modalCreate: '/Assignment/modal/create', // GET (PartialView)
    modalEdit: (id) => `/Assignment/modal/edit?id=${id}` // GET (PartialView)
};


const COLUMNS_MAP = {
    id: 'id',
    acceptDate: 'acceptDate',                     // Дата приема заявки от отделения
    departmentNumber: 'departmentNumber',         // Номер отделения
    description: 'description',                   // Описание
    forwardDate: 'forwardDate',                   // Дата направления
    forwardDepartment: 'forwardDepartment',       // Куда направили документы
    name: 'name',                                 // Что сделано (при необходимости переименуйте на workDone)
    departmentForwardDate: 'departmentForwardDate', // Дата передачи в отделение
    assignee: 'assignee',                         // Исполнитель
    note: 'note',                                 // Примечание
    createdDate: 'createdDate',                   // Дата создания задачи
    patient: 'patient'                            // Пациент (строка ФИО или код)
};

// Вспомогательные функции
function updateConnectionStatus() {
    const isConnected = true; // В реальном приложении выполните реальную проверку соединения
    $('#connection-status').html(
        isConnected
            ? '<i class="fa fa-circle text-success"></i><small>Подключено</small>'
            : '<i class="fa fa-circle text-warning"></i><small>Polling режим</small>'
    );
}

function updateLastUpdateTime() {
    const now = new Date();
    const timeString = now.toLocaleTimeString('ru-RU', {
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit'
    });
    $('#last-update').text(timeString);
}

function updateRecordsCount(count) {
    $('#records-badge').text(count || 0);
}

let assignmentAutoRefreshInterval;
let assignmentSubmitInProgress = false;
function startAssignmentAutoRefresh() {
    if ($('#auto-refresh-setting').is(':checked') && window.assignmentDataTable) {
        stopAssignmentAutoRefresh();
        assignmentAutoRefreshInterval = setInterval(() => {
            window.assignmentDataTable.ajax.reload(null, false);
            updateLastUpdateTime();
        }, 30000);
    }
}

function stopAssignmentAutoRefresh() {
    if (assignmentAutoRefreshInterval) {
        clearInterval(assignmentAutoRefreshInterval);
        assignmentAutoRefreshInterval = null;
    }
}

// Функция для ручной загрузки данных
function loadAssignmentData() {
    if (window.assignmentDataTable && $.fn.DataTable.isDataTable('#table')) {
        ajaxReloadCounter++;
        console.log(`🔄 Вызов ajax.reload #${ajaxReloadCounter}`);
        window.assignmentDataTable.ajax.reload(null, false);
    }
}


function loadAssignmentSettings() {
    const savedSettings = localStorage.getItem('assignmentTableSettings');
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

function openCreateAssignment(patientId) {
    const url = patientId ? (ASSIGNMENT_API.modalCreate + '?patientId=' + encodeURIComponent(patientId))
        : ASSIGNMENT_API.modalCreate;

    $.get(url, function (html) {
        $('#form-modal .modal-title').text('Создать задачу');
        $('#form-modal .modal-body').html(html);
        $('#form-modal').modal('show');
        if (window.AddAssignmentFormValidation) {
            AddAssignmentFormValidation.initErrorClearing();
        }
    }).fail(function () {
        showMessage('error', 'Ошибка', 'Не удалось загрузить форму создания задачи');
    });
}

function openEditAssignment(assignmentId) {
    $.get(ASSIGNMENT_API.modalEdit(assignmentId), function (html) {
        $('#form-modal .modal-title').text('Редактировать задачу');
        $('#form-modal .modal-body').html(html);
        $('#form-modal').modal('show');
        if (window.AddAssignmentFormValidation) {
            AddAssignmentFormValidation.initErrorClearing();
        }
    }).fail(function () {
        showMessage('error', 'Ошибка', 'Не удалось загрузить форму редактирования');
    });
}

// Удаление задачи
function deleteAssignment(assignmentId) {
    Swal.fire({
        title: 'Подтверждение',
        text: 'Вы уверены, что хотите удалить эту задачу?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Да, удалить',
        cancelButtonText: 'Отмена'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: ASSIGNMENT_API.delete(assignmentId),
                type: 'DELETE',
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }).done(function (response, status, xhr) {
                // Для 204 No Content нет тела — считаем успехом
                showMessage('success', 'Успешно', 'Задача удалена.');
                if (window.assignmentDataTable) {
                    window.assignmentDataTable.ajax.reload();
                }
            }).fail(function (xhr) {
                let msg = 'Ошибка при удалении задачи.';
                try {
                    // Пытаемся извлечь сообщение из JSON-ответа API
                    const json = xhr.responseJSON || JSON.parse(xhr.responseText);
                    msg = json?.message || json?.Message || json?.error || msg;
                    // Если ответ в формате ApiResponse<T>
                    if (json && json.Success === false && json.Message) {
                        msg = json.Message;
                    }
                } catch (_) { /* ignore parse errors */ }
                showMessage('error', 'Ошибка', msg);
            });
        }
    });
}

// Счетчик инициализаций для диагностики
let initializationCounter = 0;
let dataSrcCounter = 0;
let ajaxRequestCounter = 0;
let initCompleteCounter = 0;
let formSubmitCounter = 0;
let documentReadyCounter = 0;

// Флаг для предотвращения повторной отправки формы
let isFormSubmitting = false;

// Инициализация DataTable для задач
function initializeAssignmentDataTable() {
    initializationCounter++;
    console.log(`🔧 Начинаем инициализацию DataTable (попытка #${initializationCounter})`);
    
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
    const url = ASSIGNMENT_API.listActive;
    console.log('📡 URL для AJAX запроса:', url);

    try {
        const dataTable = $('#table').DataTable({
            processing: true,
            serverSide: true,
            rowId: COLUMNS_MAP.id,
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
                    ajaxRequestCounter++;
                    console.log(`🚀 Отправляем AJAX запрос #${ajaxRequestCounter} на:`, url);
                    console.log('🔍 Параметры запроса:', this.data);
                },
                dataSrc: function (json) {
                    dataSrcCounter++;
                    console.log(`📦 Получен ответ от сервера (вызов #${dataSrcCounter}):`, json);
                    
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
                        console.log(`✅ Получено ${json.data.length} записей (вызов #${dataSrcCounter})`);
                        updateRecordsCount(json.recordsTotal || 0);
                        updateLastUpdateTime();
                        return json.data;
                    }
                    
                    console.log(`📭 Получен пустой результат от сервера (вызов #${dataSrcCounter})`);
                    updateRecordsCount(0);
                    updateLastUpdateTime();
                    return [];
                },
                error: function (xhr, status, error) {
                    console.error('❌ Ошибка AJAX запроса:', {xhr, status, error});
                    
                    // Показываем сообщение об ошибке только если это не поисковый запрос
                    if (status !== 'abort') {
                        showMessage('error', 'Ошибка', 'Не удалось загрузить данные задач');
                    }
                    
                    // Возвращаем пустой массив для предотвращения ошибок DataTable
                    return [];
                }
            },
            columns: [
                // Действия (просмотр/редактирование/удаление)
                {
                    data: COLUMNS_MAP.id,
                    title: 'Действия',
                    orderable: false,
                    searchable: false,
                    className: 'text-center',
                    render: function (data, type, row) {
                        return `
                            <div class="btn-group" role="group">
                                <button type="button" class="btn btn-outline-primary btn-sm btn-view-assignment" data-assignment-id="${data}" title="Просмотр">
                                    <i class="fa fa-eye"></i>
                                </button>
                                <button type="button" class="btn btn-outline-warning btn-sm btn-edit-assignment" data-assignment-id="${data}" title="Редактировать">
                                    <i class="fa fa-edit"></i>
                                </button>
                                <button type="button" class="btn btn-outline-danger btn-sm btn-delete-assignment" data-assignment-id="${data}" title="Удалить">
                                    <i class="fa fa-trash"></i>
                                </button>
                            </div>
                        `;
                    }
                },

                // Столбцы по Вашим заголовкам
                { data: COLUMNS_MAP.id, title: 'ID', visible: false, searchable: false },

                { data: COLUMNS_MAP.acceptDate, title: 'Дата приема заявки от отделения', className: 'text-center',
                    render: function (d) { return formatDate(d); } },

                { data: COLUMNS_MAP.departmentNumber, title: 'Номер отделения', className: 'text-center' },

                { data: COLUMNS_MAP.description, title: 'Описание', className: 'fw-medium' },

                { data: COLUMNS_MAP.forwardDate, title: 'Дата направления', className: 'text-center',
                    render: function (d) { return formatDate(d); } },

                { data: COLUMNS_MAP.forwardDepartment, title: 'Куда направили документы', className: 'text-center' },

                { data: COLUMNS_MAP.name, title: 'Что сделано', className: 'fw-medium' },

                { data: COLUMNS_MAP.departmentForwardDate, title: 'Дата передачи в отделение', className: 'text-center',
                    render: function (d) { return formatDate(d); } },

                { data: COLUMNS_MAP.assignee, title: 'Исполнитель', className: 'text-center' },

                { data: COLUMNS_MAP.note, title: 'Примечание', className: 'fw-medium' },

                { data: COLUMNS_MAP.createdDate, title: 'Дата создания задачи', className: 'text-center',
                    render: function (d) { return formatDate(d); } },

                { data: COLUMNS_MAP.patient, title: 'Пациент', className: 'fw-medium' }
            ],
            order: [[11, 'desc']], // сортируем по "Дата создания задачи" (индекс колонки может отличаться — проверьте)
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
                initCompleteCounter++;
                console.log(`✅ DataTable инициализирована (вызов #${initCompleteCounter})`);
                console.log(`📊 Текущее количество записей в таблице: ${this.api().page.info().recordsTotal}`);
                
                // Принудительно загружаем данные, если их нет
                if (this.api().page.info().recordsTotal === 0) {
                    console.log('🔄 Принудительно загружаем данные...');
                    this.api().ajax.reload(null, false);
                }
                
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

        window.assignmentDataTable = dataTable;
        console.log('💾 DataTable сохранена в window.assignmentDataTable');
        console.log('🔍 DataTable объект:', dataTable);
        return dataTable;
    } catch (error) {
        console.error('❌ Ошибка при инициализации DataTable:', error);
        showMessage('error', 'Ошибка', 'Не удалось инициализировать таблицу данных задач.');
        return null;
    }
}

// Форматирование даты dd.MM.yyyy
function formatDate(date) {
    if (!date) return '';
    const d = new Date(date);
    if (isNaN(d.getTime())) return '';
    const day = String(d.getDate()).padStart(2, '0');
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const year = d.getFullYear();
    return `${day}.${month}.${year}`;
}

// Преобразование из dd.MM.yyyy в ISO (если потребуется)
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
    documentReadyCounter++;
    console.log(`🚀 Document ready: assignment.js (попытка #${documentReadyCounter})`);

    if (typeof $ === 'undefined') {
        console.error('jQuery не загружен');
        return;
    }
    if (typeof $.fn.DataTable === 'undefined') {
        console.error('DataTables не загружен');
        return;
    }

    // Инициализация окружения
    loadAssignmentSettings();
    updateConnectionStatus();
    updateLastUpdateTime();

    // Инициализация DataTable
    console.log('📊 Инициализируем DataTable');
    const dataTableResult = initializeAssignmentDataTable();
    if (dataTableResult) {
        console.log('✅ DataTable успешно инициализирована');
        startAssignmentAutoRefresh();
        
        // Дополнительная проверка через небольшую задержку
        setTimeout(function() {
            if (window.assignmentDataTable && window.assignmentDataTable.page.info().recordsTotal === 0) {
                console.log('🔄 Дополнительная проверка: данных нет, принудительно загружаем...');
                window.assignmentDataTable.ajax.reload(null, false);
            }
        }, 500);
    } else {
        console.error('❌ Не удалось инициализировать DataTable');
    }

    // Кнопка "Добавить задачу" — одноразовая привязка
    $(document).off('click.assignment', '#add-assignment').on('click.assignment', '#add-assignment', function (e) {
        e.preventDefault();
        openCreateAssignment();
    });

    // Отправка данных из модального окна: Создать задачу
    $(document).off('submit.assignment', '#create-assignment-form').on('submit.assignment', '#create-assignment-form', function (e) {
        e.preventDefault();
        formSubmitCounter++;
        console.log(`📝 Обработчик формы создания задачи вызван (попытка #${formSubmitCounter})`);

        const $form = $(this);
        const $submitBtn = $("#form-modal").find(':submit');
        
        // Строгая защита от повторной отправки
        if (isFormSubmitting || $submitBtn.attr('disabled') === 'disabled') {
            console.log('⚠️ Форма уже отправляется, игнорируем повторную отправку');
            return;
        }
        
        // Устанавливаем флаг отправки
        isFormSubmitting = true;

        const url = $form.attr('action');
        
        const formData = new FormData($form[0]);
        const data = new URLSearchParams();
        for (const pair of formData.entries()) {
            data.append(pair[0], pair[1]);
        }
        
        // Генерируем уникальный ID запроса для отслеживания
        const requestId = 'req_' + Date.now() + '_' + Math.random().toString(36).substr(2, 9);
        console.log(`🆔 Уникальный ID запроса: ${requestId}`);
        
        const headers = {
            "CSRF-TOKEN": $form.find('input[name="__RequestVerificationToken"]').val(),
            "X-Request-ID": requestId
        };

        // Очищаем предыдущие ошибки
        if (window.AddAssignmentFormValidation) {
            AddAssignmentFormValidation.clearValidationErrors();
        }

        // Отправляем данные
        $.ajax({
            url: url,
            type: 'POST',
            data: data.toString(),
            contentType: 'application/x-www-form-urlencoded',
            headers: headers,
            beforeSend: function () {
                console.log(`🚀 Отправляем AJAX запрос создания задачи (ID: ${requestId})`);
                $("#form-modal").find(':submit').attr('disabled', true);
                $("#form-modal").find(':submit').html('<div class="spinner-border spinner-border-sm" role="status"></div>');
            },
            success: function (response) {
                console.log(`✅ Задача успешно создана (ID запроса: ${requestId}), ответ сервера:`, response);
                $('#form-modal').modal('hide');
                showMessage('success', 'Успешно', 'Задача добавлена');
                
                // Сбрасываем флаг отправки
                isFormSubmitting = false;
                
                // Обновляем таблицу после создания
                if (window.assignmentDataTable) {
                    console.log('🔄 Обновляем таблицу после создания задачи');
                    window.assignmentDataTable.ajax.reload(null, false);
                }
            },
            error: function (xhr) {
                console.log(`❌ Ошибка при создании задачи (ID запроса: ${requestId}):`, xhr);
                $("#form-modal").find(':submit').html('Сохранить');
                $("#form-modal").find(':submit').attr('disabled', false);
                
                // Сбрасываем флаг отправки при ошибке
                isFormSubmitting = false;

                // Обработка ошибок валидации (400)
                if (xhr.status === 400) {
                    try {
                        const response = JSON.parse(xhr.responseText);
                        if (response && response.Data && response.Data.Errors && Array.isArray(response.Data.Errors)) {
                            if (window.AddAssignmentFormValidation) {
                                AddAssignmentFormValidation.showValidationErrors(response.Data.Errors);
                                return;
                            }
                        }
                    } catch (parseError) {
                        console.log('Не удалось распарсить ответ с ошибками:', parseError);
                    }
                }

                showMessage('error', 'Ошибка', 'Произошла ошибка при добавлении задачи');
            }
        });
    });

    // Отправка данных из модального окна: Редактировать задачу (PATCH)
    $(document).off('submit.assignment', '#edit-assignment-form').on('submit.assignment', '#edit-assignment-form', function (e) {
        e.preventDefault();

        const $form = $(this);
        const url = $form.attr('action'); // /api/assignments/{id}

        if (window.AddAssignmentFormValidation) {
            AddAssignmentFormValidation.clearValidationErrors();
        }

        const formData = new FormData($form[0]);
        const data = new URLSearchParams();
        for (const pair of formData.entries()) {
            data.append(pair[0], pair[1]);
        }

        const headers = {
            "CSRF-TOKEN": $form.find('input[name="__RequestVerificationToken"]').val()
        };

        $.ajax({
            url: url,
            type: 'PATCH',
            data: data.toString(),
            contentType: 'application/x-www-form-urlencoded',
            headers: headers,
            beforeSend: function () {
                $("#form-modal").find(':submit').attr('disabled', true).html('<div class="spinner-border spinner-border-sm" role="status"></div>');
            },
            success: function () {
                $('#form-modal').modal('hide');
                showMessage('success', 'Успешно', 'Задача обновлена');

                // Обновляем таблицу после редактирования
                if (window.assignmentDataTable) {
                    window.assignmentDataTable.ajax.reload(null, false);
                }
            },
            error: function (xhr) {
                $("#form-modal").find(':submit').html('Сохранить').attr('disabled', false);

                if (xhr.status === 400) {
                    try {
                        const response = JSON.parse(xhr.responseText);

                        if (Array.isArray(response)) {
                            AddAssignmentFormValidation.showValidationErrors(response);
                            return;
                        }
                        if (response && response.Data && Array.isArray(response.Data.Errors)) {
                            AddAssignmentFormValidation.showValidationErrors(response.Data.Errors);
                            return;
                        }
                    } catch (parseError) {
                        console.log('Не удалось распарсить ответ с ошибками:', parseError);
                    }
                }

                const errorMessage = 'Произошла ошибка при обновлении задачи';
                showMessage('error', 'Ошибка', errorMessage);
            }
        });
    });

    // Очистка ошибок при вводе
    $(document).on('input focus', '.form-control.is-invalid, .form-select.is-invalid', function () {
        $(this).removeClass('is-invalid');
        $(this).siblings('.invalid-feedback, .text-danger.field-validation-error').remove();
    });
    $(document).on('change', '.form-select.is-invalid', function () {
        $(this).removeClass('is-invalid');
        $(this).siblings('.invalid-feedback, .text-danger.field-validation-error').remove();
    });

    // Кнопка "Обновить данные"
    $(document).on('click', '#reload-data', function (e) {
        e.preventDefault();
        if (window.assignmentDataTable) {
            window.assignmentDataTable.ajax.reload(function () {
                updateLastUpdateTime();
            }, false);
        }
    });

    // Кнопка "Полноэкранный режим"
    $(document).on('click', '#fullscreen-toggle', function (e) {
        e.preventDefault();
        e.stopPropagation();

        const $container = $('.modern-datatable-container');
        const $icon = $(this).find('i');

        if (!$container.hasClass('fullscreen-mode')) {
            $container.addClass('fullscreen-mode');
            $('body').addClass('datatable-fullscreen');
            $icon.removeClass('fa-expand').addClass('fa-compress');

            $container.css({
                'position': 'fixed',
                'top': '0',
                'left': '0',
                'right': '0',
                'bottom': '0',
                'z-index': '1040',
                'background': '#fff'
            });

            if (window.assignmentDataTable) {
                setTimeout(function () {
                    window.assignmentDataTable.columns.adjust().draw();
                }, 200);
            }
        } else {
            $container.removeClass('fullscreen-mode');
            $('body').removeClass('datatable-fullscreen');
            $icon.removeClass('fa-compress').addClass('fa-expand');

            $container.css({
                'position': '',
                'top': '',
                'left': '',
                'right': '',
                'bottom': '',
                'z-index': '',
                'background': ''
            });

            if (window.assignmentDataTable) {
                setTimeout(function () {
                    window.assignmentDataTable.columns.adjust();
                    window.assignmentDataTable.draw();
                }, 300);
            }
        }
    });

    // Делегированные обработчики для действий в строке
    $(document).on('click', '.btn-view-assignment', function (e) {
        e.preventDefault();
        const assignmentId = $(this).data('assignment-id');
        // Если есть страница карточки — раскомментируйте:
        // window.location.href = `/Assignment/Card/${assignmentId}`;
        showMessage('info', 'Информация', 'Переход к карточке задачи не настроен');
    });

    $(document).on('click', '.btn-edit-assignment', function (e) {
        e.preventDefault();
        const assignmentId = $(this).data('assignment-id');
        openEditAssignment(assignmentId);
    });

    $(document).on('click', '.btn-delete-assignment', function (e) {
        e.preventDefault();
        const assignmentId = $(this).data('assignment-id');
        deleteAssignment(assignmentId);
    });

    // Настройки таблицы
    $('#apply-settings').on('click', function () {
        const pageLength = $('#page-length-setting').val();
        const autoRefresh = $('#auto-refresh-setting').is(':checked');
        const compactMode = $('#compact-mode-setting').is(':checked');

        if (window.assignmentDataTable) {
            $('#table').toggleClass('table-sm', compactMode);

            const settings = {
                pageLength: pageLength,
                autoRefresh: autoRefresh,
                compactMode: compactMode
            };
            localStorage.setItem('assignmentTableSettings', JSON.stringify(settings));

            window.assignmentDataTable.page.len(pageLength === '-1' ? -1 : parseInt(pageLength)).draw();

            if (autoRefresh) {
                startAssignmentAutoRefresh();
            } else {
                stopAssignmentAutoRefresh();
            }
        }

        $('#table-settings-modal').modal('hide');
    });

    // Горячие клавиши
    $(document).on('keydown', function (e) {
        if (!$(e.target).is('input, textarea, select')) {
            if (e.ctrlKey && e.key === 'n') {
                e.preventDefault();
                $('#add-assignment').click();
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

    console.log('🎯 Инициализация завершена');
    console.log('📊 Счетчики диагностики:');
    console.log(`   - Document ready: ${documentReadyCounter}`);
    console.log(`   - Инициализаций DataTable: ${initializationCounter}`);
    console.log(`   - AJAX запросов: ${ajaxRequestCounter}`);
    console.log(`   - Вызовов dataSrc: ${dataSrcCounter}`);
    console.log(`   - Вызовов initComplete: ${initCompleteCounter}`);
    console.log(`   - Отправок формы создания: ${formSubmitCounter}`);
    console.log('DataTable instance:', window.assignmentDataTable);
    console.log('AJAX URL:', window.assignmentDataTable?.ajax?.url());
});

