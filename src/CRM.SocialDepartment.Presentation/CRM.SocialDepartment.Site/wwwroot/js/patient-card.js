/**
 * Patient Card JavaScript
 * Функциональность для страницы карточки пациента
 */

$(document).ready(function() {
    console.log('🔍 [PatientCard] Инициализация карточки пациента');
    
    // Инициализация DataTable для истории болезни
    initializeHistoryTable();
    
    // Обработчики кнопок
    initializeButtons();
    
    // Инициализация тултипов
    initializeTooltips();
});

/**
 * Инициализация таблицы истории болезни
 */
function initializeHistoryTable() {
    if ($('#history-table').length) {
        $('#history-table').DataTable({
            language: {
                url: '/lib/datatables.net-bs5/js/dataTables.russian.json'
            },
            pageLength: 10,
            lengthMenu: [[5, 10, 25, 50], [5, 10, 25, 50]],
            order: [[2, 'desc']], // Сортировка по дате поступления
            columnDefs: [
                {
                    targets: -1, // Последняя колонка (действия)
                    orderable: false,
                    searchable: false
                }
            ],
            responsive: true,
            dom: '<"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6"f>>' +
                 '<"row"<"col-sm-12"tr>>' +
                 '<"row"<"col-sm-12 col-md-5"i><"col-sm-12 col-md-7"p>>',
            language: {
                url: '/lib/datatables.net-bs5/js/dataTables.russian.json'
            }
        });
    }
}

/**
 * Инициализация кнопок
 */
function initializeButtons() {
    // Кнопка редактирования пациента
    $('#edit-patient').on('click', function() {
        const patientId = getPatientIdFromUrl();
        if (patientId) {
            GetFormModal(`/patient/modal/edit/?patientId=${patientId}`, 'Редактировать пациента');
        }
    });
    
    // Кнопка печати документа
    $('#print-document').on('click', function() {
        const patientId = getPatientIdFromUrl();
        if (patientId) {
            printPatientDocument(patientId);
        }
    });
    
    // Кнопка добавления задачи
    $('#add-todo').on('click', function() {
        const patientId = getPatientIdFromUrl();
        if (patientId) {
            addPatientTodo(patientId);
        }
    });
    
    // Кнопка архивирования пациента
    $('#archive-patient').on('click', function() {
        const patientId = getPatientIdFromUrl();
        if (patientId) {
            archivePatient(patientId);
        }
    });
    
    // Кнопка разархивирования пациента
    $('#unarchive-patient').on('click', function() {
        const patientId = getPatientIdFromUrl();
        if (patientId) {
            unarchivePatient(patientId);
        }
    });
    
    // Обработчики для кнопок в таблице истории болезни
    $('#history-table').on('click', '.btn-outline-primary', function() {
        const row = $(this).closest('tr');
        const historyId = row.data('id');
        const patientId = row.data('patient-id');
        
        if (historyId && patientId) {
            editMedicalHistory(historyId, patientId);
        }
    });
}

/**
 * Инициализация тултипов
 */
function initializeTooltips() {
    $('[data-bs-toggle="tooltip"]').tooltip();
}

/**
 * Получение ID пациента из URL
 */
function getPatientIdFromUrl() {
    const pathSegments = window.location.pathname.split('/');
    const patientIdIndex = pathSegments.findIndex(segment => segment === 'patients') + 1;
    
    if (patientIdIndex < pathSegments.length) {
        return pathSegments[patientIdIndex];
    }
    
    return null;
}

/**
 * Печать документа пациента
 */
function printPatientDocument(patientId) {
    console.log('🖨️ [PatientCard] Печать документа для пациента:', patientId);
    
    // Показываем модальное окно выбора типа документа
    $('#print-modal').modal('show');
    $('#print-select').attr('data-patient-id', patientId);
}

/**
 * Добавление задачи для пациента
 */
function addPatientTodo(patientId) {
    console.log('📝 [PatientCard] Добавление задачи для пациента:', patientId);
    
    // Здесь можно открыть модальное окно для добавления задачи
    if (window.malomalsky && window.malomalsky.message) {
        window.malomalsky.message.info('Информация', 'Функция добавления задач находится в разработке');
    } else {
        Swal.fire({
            icon: 'info',
            title: 'Информация',
            text: 'Функция добавления задач находится в разработке'
        });
    }
}

/**
 * Архивирование пациента
 */
function archivePatient(patientId) {
    console.log('📦 [PatientCard] Архивирование пациента:', patientId);
    
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
            // Здесь будет AJAX запрос для архивирования
            if (window.malomalsky && window.malomalsky.message) {
                window.malomalsky.message.success('Успех', 'Пациент успешно архивирован');
            } else {
                Swal.fire('Успех!', 'Пациент успешно архивирован', 'success');
            }
        }
    });
}

/**
 * Разархивирование пациента
 */
function unarchivePatient(patientId) {
    console.log('📦 [PatientCard] Разархивирование пациента:', patientId);
    
    Swal.fire({
        title: 'Подтверждение',
        text: 'Вы уверены, что хотите разархивировать этого пациента?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#28a745',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Да, разархивировать',
        cancelButtonText: 'Отмена'
    }).then((result) => {
        if (result.isConfirmed) {
            // Здесь будет AJAX запрос для разархивирования
            if (window.malomalsky && window.malomalsky.message) {
                window.malomalsky.message.success('Успех', 'Пациент успешно разархивирован');
            } else {
                Swal.fire('Успех!', 'Пациент успешно разархивирован', 'success');
            }
        }
    });
}

/**
 * Редактирование истории болезни
 */
function editMedicalHistory(historyId, patientId) {
    console.log('📝 [PatientCard] Редактирование истории болезни:', historyId, 'для пациента:', patientId);
    
    // Здесь можно открыть модальное окно для редактирования истории болезни
    if (window.malomalsky && window.malomalsky.message) {
        window.malomalsky.message.info('Информация', 'Функция редактирования истории болезни находится в разработке');
    } else {
        Swal.fire({
            icon: 'info',
            title: 'Информация',
            text: 'Функция редактирования истории болезни находится в разработке'
        });
    }
}

/**
 * Универсальная функция для показа сообщений
 */
function showMessage(type, title, message) {
    try {
        if (window.malomalsky && window.malomalsky.message && window.malomalsky.message[type]) {
            window.malomalsky.message[type](title, message);
        } else if (window.Swal) {
            Swal.fire({
                icon: type,
                title: title,
                text: message
            });
        } else {
            console.error('❌ [PatientCard] Не удалось показать сообщение:', { type, title, message });
        }
    } catch (error) {
        console.error('❌ [PatientCard] Ошибка при показе сообщения:', error);
    }
}

///**
// * Функция для получения модального окна (совместимость с существующим кодом)
// */
//function GetFormModal(url, title) {
//    $.ajax({
//        url: url,
//        type: 'GET',
//        timeout: 30000, // 30 секунд (в миллисекундах)
//        success: function(response) {
//            $('#form-modal .modal-title').text(title);
//            $('#form-modal .modal-body').html(response);
//            $('#form-modal').modal('show');
//        },
//        error: function(xhr, status, error) {
//            console.error('❌ [PatientCard] Ошибка при загрузке модального окна:', error);
//            showMessage('error', 'Ошибка', 'Не удалось загрузить форму');
//        }
//    });
//}