/*
    Назначение: Для страницы списка пользователей.
    Версия: 3.0.0 - Современный дизайн с полной функциональностью
*/

$(document).ready(function () {

    var dataTable = $('#table').DataTable({
        "bAutoWidth": false,
        "processing": true,
        "serverSide": true,
        "filter": true,
        "language": {
            "processing": '<i class="fa fa-spinner fa-spin"></i> Загрузка данных...',
            "lengthMenu": "Показать _MENU_ записей",
            "zeroRecords": '<div class="text-center py-4"><i class="fa fa-users fa-2x text-muted mb-2"></i><br><strong>Пользователи не найдены</strong><br><small class="text-muted">Попробуйте изменить критерии поиска</small></div>',
            "info": "Записи _START_-_END_ из _TOTAL_",
            "infoEmpty": "Нет пользователей для отображения",
            "infoFiltered": "(отфильтровано из _MAX_ записей)",
            "search": '<i class="fa fa-search me-2"></i>Поиск:',
            "searchPlaceholder": "Поиск пользователей...",
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
        "pageLength": 50,
        "pagingType": "full_numbers",
        "responsive": true,
        "stateSave": true,
        "stateDuration": 60 * 60 * 24, // 24 часа
        "deferRender": true,
        "ajax": {
            "url": "/api/User/GetAllUsers",
            "type": "GET",
            "contentType": "application/x-www-form-urlencoded; charset=UTF-8",
            "datatype": "json",
            "error": function(xhr, error, code) {
                console.error('User DataTable AJAX Error:', error, code, xhr);
                
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
                    let errorMessage = 'Не удалось загрузить список пользователей. Попробуйте обновить страницу.';
                    
                    if (xhr.responseJSON && xhr.responseJSON.message) {
                        errorMessage = xhr.responseJSON.message;
                    }
                    
                    malomalsky.message.error('Ошибка загрузки данных', errorMessage);
                }
            }
        },
        "columns": [
            { 
                data: "UserName", 
                name: "UserName",
                title: "Имя пользователя",
                className: "fw-medium"
            },
            { 
                data: "FirstName", 
                name: "FirstName",
                title: "Имя"
            },
            { 
                data: "LastName", 
                name: "LastName",
                title: "Фамилия"
            },
            { 
                data: "Email", 
                name: "Email",
                title: "Электронная почта"
            },
            { 
                data: "CreatedOn", 
                name: "CreatedOn",
                title: "Дата создания",
                className: "text-center",
                width: "150px"
            }
        ],
        "columnDefs": [
            {
                "targets": 0,
                "render": function(data, type, row) {
                    if (type === 'display' && data) {
                        return '<div class="d-flex align-items-center">' +
                               '<i class="fa fa-user text-primary me-2"></i>' +
                               '<span class="fw-medium">' + data + '</span>' +
                               '</div>';
                    }
                    return data || '';
                }
            },
            {
                "targets": 3,
                "render": function(data, type, row) {
                    if (type === 'display' && data) {
                        return '<div class="d-flex align-items-center">' +
                               '<i class="fa fa-envelope text-secondary me-2"></i>' +
                               '<a href="mailto:' + data + '" class="text-decoration-none">' + data + '</a>' +
                               '</div>';
                    }
                    return data || '';
                }
            },
            {
                "targets": 4,
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
        "order": [[4, "desc"]],
        "drawCallback": function(settings) {
            // Добавляем анимацию для новых строк
            $(this.api().table().body()).find('tr').addClass('fade-in');
            
            // Обновляем счетчики
            updateTableStats();
            
            // Инициализируем tooltips если есть
            $('[data-bs-toggle="tooltip"]').tooltip();
        },
        "initComplete": function(settings, json) {
            console.log('DataTable инициализирована успешно');
            
            // Улучшаем поле поиска
            $('.dataTables_filter input')
                .addClass('form-control-sm')
                .attr('placeholder', 'Поиск пользователей...')
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
                '<button type="button" class="btn btn-outline-secondary btn-sm ms-2" id="refresh-table" title="Обновить данные">' +
                '<i class="fa fa-refresh"></i>' +
                '</button>'
            );
            
            $('#refresh-table').on('click', function() {
                dataTable.ajax.reload(null, false);
                $(this).find('i').addClass('fa-spin');
                setTimeout(() => {
                    $(this).find('i').removeClass('fa-spin');
                }, 1000);
            });
        }
    });

    // Обработчики событий для кнопок
    $('#add-user').on('click', function () {
        GetFormModal(window.location.origin + '/user/modal/create', 'Добавить пользователя');
    });

    // Обновить данные в таблице
    $('#reload').on('click', function () {
        const $btn = $(this);
        const $icon = $btn.find('i');
        
        $icon.addClass('fa-spin');
        dataTable.ajax.reload(function() {
            $icon.removeClass('fa-spin');
            updateLastUpdateTime();
            if (window.malomalsky && window.malomalsky.message) {
                malomalsky.message.success('Обновлено', 'Данные таблицы обновлены');
            }
        }, false);
    });

    // Полноэкранный режим
    $('#fullscreen-toggle').on('click', function() {
        const $container = $('.modern-datatable-container');
        const $icon = $(this).find('i');
        
        if ($container.hasClass('fullscreen-mode')) {
            // Выход из полноэкранного режима
            $container.removeClass('fullscreen-mode');
            $('body').removeClass('datatable-fullscreen');
            $icon.removeClass('fa-compress').addClass('fa-expand');
            $(this).attr('title', 'Полноэкранный режим');
        } else {
            // Вход в полноэкранный режим
            $container.addClass('fullscreen-mode');
            $('body').addClass('datatable-fullscreen');
            $icon.removeClass('fa-expand').addClass('fa-compress');
            $(this).attr('title', 'Выйти из полноэкранного режима');
        }
        
        // Перерисовываем таблицу для корректного отображения
        setTimeout(() => {
            dataTable.columns.adjust().draw();
        }, 100);
    });

    // Функция для обновления статистики таблицы
    function updateTableStats() {
        const info = dataTable.page.info();
        
        // Обновляем бейдж с количеством записей
        $('#records-badge').text(info.recordsTotal);
        
        // Обновляем заголовок
        let title = 'Список пользователей';
        if (info.recordsTotal > 0) {
            title += ` (${info.recordsTotal})`;
        }
        $('#title-text').text(title);
        
        // Обновляем время последнего обновления
        updateLastUpdateTime();
    }

    // Функция для обновления времени последнего обновления
    function updateLastUpdateTime() {
        const now = new Date();
        const timeString = now.toLocaleTimeString('ru-RU', {
            hour: '2-digit',
            minute: '2-digit',
            second: '2-digit'
        });
        $('#last-update').text(timeString);
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
        if (settings.url && settings.url.includes('/User/GetAllUsers')) {
            console.error('User API Error:', thrownError);
        }
    });

    // Добавляем обработчик для адаптивности
    $(window).on('resize', debounce(function() {
        dataTable.columns.adjust().draw();
    }, 250));

    // Инициализация при загрузке страницы
    updateTableStats();
    updateLastUpdateTime();

    // Экспорт DataTable для глобального доступа
    window.userDataTable = dataTable;
});
