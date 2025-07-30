/*
    Назначение: Для страницы архива пациентов.
    Версия: 2.0.0 - Современный дизайн
*/

$(document).ready(function () {

    var dataTable = $('#table').DataTable({
        "bAutoWidth": false,
        "processing": true,
        "serverSide": true,
        "filter": true,
        "language": {
            "processing": '<i class="fa fa-spinner fa-spin"></i> Загрузка архива...',
            "lengthMenu": "Показать _MENU_ записей",
            "zeroRecords": '<div class="text-center py-4"><i class="fa fa-archive fa-2x text-muted mb-2"></i><br><strong>Архив пуст</strong><br><small class="text-muted">Архивированные пациенты не найдены</small></div>',
            "info": "Записи _START_-_END_ из _TOTAL_",
            "infoEmpty": "Нет архивных записей",
            "infoFiltered": "(отфильтровано из _MAX_ записей)",
            "search": '<i class="fa fa-search me-2"></i>Поиск в архиве:',
            "searchPlaceholder": "Поиск в архиве...",
            "paginate": {
                "first": '<i class="fa fa-angle-double-left"></i>',
                "next": '<i class="fa fa-angle-right"></i>',
                "previous": '<i class="fa fa-angle-left"></i>',
                "last": '<i class="fa fa-angle-double-right"></i>'
            },
            "aria": {
                "sortAscending": ": активировать для сортировки по возрастанию",
                "sortDescending": ": активировать для сортировки по убыванию"
            }
        },
        "lengthMenu": [[25, 50, 100, 250], [25, 50, 100, 250]],
        "pageLength": 25,
        "pagingType": "full_numbers",
        "responsive": true,
        "stateSave": true,
        "stateDuration": 60 * 60 * 24, // 24 часа
        "deferRender": true,
        "ajax": {
            "url": "/api/patients/archive",
            "type": "POST",
            "contentType": "application/x-www-form-urlencoded; charset=UTF-8",
            "datatype": "json",
            "error": function(xhr, error, code) {
                console.error('Patient Archive DataTable AJAX Error:', error, code, xhr);
                
                // Обработка различных типов ошибок
                if (xhr.status === 400 && xhr.responseJSON) {
                    const response = xhr.responseJSON;
                    if (response.error === "Некорректный символ в поиске") {
                        // Показываем специальное сообщение для ошибок поиска
                        if (window.malomalsky && window.malomalsky.message) {
                            malomalsky.message.warning(
                                'Некорректный поиск', 
                                response.message + '<br><small class="text-muted">' + response.details + '</small>'
                            );
                        }
                        
                        // Очищаем поле поиска
                        $('.dataTables_filter input').val('').trigger('keyup');
                        return;
                    }
                }
                
                // Обработка общих ошибок
                if (window.malomalsky && window.malomalsky.message) {
                    let errorMessage = 'Не удалось загрузить архив пациентов. Попробуйте обновить страницу.';
                    
                    if (xhr.responseJSON && xhr.responseJSON.message) {
                        errorMessage = xhr.responseJSON.message;
                    }
                    
                    malomalsky.message.error('Ошибка загрузки данных', errorMessage);
                }
            }
        },
        "columns": [
            { 
                data: "Id", 
                name: "Id",
                title: "ID",
                className: "text-center",
                width: "80px"
            },
            { 
                data: "FullName", 
                name: "FullName",
                title: "ФИО пациента",
                className: "fw-medium"
            },
            { 
                data: "ArchivedDate", 
                name: "ArchivedDate",
                title: "Дата архивирования",
                className: "text-center",
                width: "150px"
            }
        ],
        "columnDefs": [
            {
                "targets": 0,
                "render": function(data, type, row) {
                    if (type === 'display') {
                        return '<span class="badge bg-secondary">' + data + '</span>';
                    }
                    return data;
                }
            },
            {
                "targets": 1,
                "render": function(data, type, row) {
                    if (type === 'display' && data) {
                        return '<div class="d-flex align-items-center">' +
                               '<i class="fa fa-user-circle text-secondary me-2"></i>' +
                               '<span>' + data + '</span>' +
                               '<span class="badge bg-warning ms-2"><i class="fa fa-archive"></i></span>' +
                               '</div>';
                    }
                    return data || '';
                }
            },
            {
                "targets": 2,
                "render": function(data, type, row) {
                    if (type === 'display' && data) {
                        const date = new Date(data);
                        const formatted = date.toLocaleDateString('ru-RU', {
                            year: 'numeric',
                            month: 'short',
                            day: 'numeric'
                        });
                        return '<span class="badge bg-light text-dark border" title="' + date.toLocaleString('ru-RU') + '">' +
                               '<i class="fa fa-calendar me-1"></i>' + formatted +
                               '</span>';
                    }
                    return data || '';
                }
            }
        ],
        "order": [[2, "desc"]],
        "drawCallback": function(settings) {
            // Добавляем анимацию для новых строк
            $(this.api().table().body()).find('tr').addClass('fade-in');
            
            // Обновляем счетчики
            updateTableStats();
            
            // Инициализируем tooltips если есть
            $('[data-bs-toggle="tooltip"]').tooltip();
        },
        "initComplete": function(settings, json) {
            console.log('Archive DataTable инициализирована успешно');
            
            // Улучшаем поле поиска
            $('.dataTables_filter input')
                .addClass('form-control-sm')
                .attr('placeholder', 'Поиск в архиве...')
                .on('keyup', debounce(function() {
                    // Добавляем индикатор поиска
                    if (this.value.length > 0) {
                        $(this).addClass('searching');
                    } else {
                        $(this).removeClass('searching');
                    }
                }, 300));
            
            // Улучшаем селект количества записей
            $('.dataTables_length select').addClass('form-select-sm');
            
            // Добавляем кнопку обновления
            $('.dataTables_filter').append(
                '<button type="button" class="btn btn-outline-secondary btn-sm ms-2" id="refresh-archive" title="Обновить архив">' +
                '<i class="fa fa-refresh"></i>' +
                '</button>'
            );
            
            $('#refresh-archive').on('click', function() {
                dataTable.ajax.reload(null, false);
                $(this).find('i').addClass('fa-spin');
                setTimeout(() => {
                    $(this).find('i').removeClass('fa-spin');
                }, 1000);
            });
        }
    });

    // Обновить данные в таблице
    $('#reload').on('click', function () {
        const $btn = $(this);
        const $icon = $btn.find('i');
        
        $icon.addClass('fa-spin');
        dataTable.ajax.reload(function() {
            $icon.removeClass('fa-spin');
            malomalsky.message.success('Обновлено', 'Архив обновлен');
        }, false);
    });

    // Функция для обновления статистики таблицы
    function updateTableStats() {
        const info = dataTable.page.info();
        
        // Обновляем заголовок карточки с количеством записей
        let title = 'Архив пациентов';
        if (info.recordsTotal > 0) {
            title += ` (${info.recordsTotal})`;
        }
        
        // Можно добавить бейдж с количеством
        $('.card-header h5, .page-title').text(title);
    }

    // Утилита debounce для оптимизации поиска
    function debounce(func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    }

    // Обработка ошибок AJAX глобально
    $(document).ajaxError(function(event, xhr, settings, thrownError) {
        if (settings.url && settings.url.includes('/api/patients/archive')) {
            console.error('Archive API Error:', thrownError);
        }
    });

    // Добавляем обработчик для адаптивности
    $(window).on('resize', debounce(function() {
        dataTable.columns.adjust().draw();
    }, 250));

    // Экспорт DataTable для глобального доступа
    window.archiveDataTable = dataTable;
});