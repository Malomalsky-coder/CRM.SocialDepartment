### Сущности в приложении
- Пользователь приложения
- Пациент
- Задача проводимая над пациентом

### Действия сущностей
#### Пользователь приложения
* Добавить
* Редактировать
* Удалить
* Назначить роль
* Изменить пароль

#### Пациент
* Добавить
* Редактировать
* Удалить
* Архивировать/Рахарзивировать
* Добавить историю болезни

#### Задача проводимая над пациентом
* Добавить
* Редактировать
* Удалить

### Информация о сущностях
#### Пациент
- Id
- ФИО
- Дата рождения
- Несовершеннолетний
- Пол
- (?) Тип который бы вместил: Гражданство, Прописку. Заметка: Геоправовой
- Документы
- Дееспособный
- Получает ли пенсию
- Примечание
- Мягкое удаление
- В архиве

#### Документы
- Тип документа: Паспорт, Снилс, Медицинский полис

#### Дееспособный
- Решение суда
- Дата проведения суда
- Опекун
- Распоряжение о назначение опекуна

#### Пенсия
- Группа инвалидности
- С какого числа бессрочная группа
- Способ получения пенсии
- Филиал СФР
- Отделение СФР
- РСД

#### История болезни
- Номер истории болезни
- Тип госпитализации
- Постановление
- Дата поступления
- Дата выписки
- (?) Номер отделения
- Примечание
- IsActive

#### Задачи
- Дата приема заявки от отделения
- Номер отделения
- Задание
- Куда направили документы
- Дата направления
- Что сделано
- Дата передачи в отделение
- Кто исполнитель
- Примечание
- Когда создана задача
- Пациент: Id, ФИО

#### Аудит сущностей
- Старое свойство
- Новое свойство
- Дата изменения

### Печать документов
- На разработке

### События доменной модели
#### События
- Начало госпитализации
- Окончание госпитализации
- Редактирование пациента

#### Обработчики событий
- При добавление/редактирование/удаление пациента
  - Кеширование
  - Логирование
  - Аудит
  ##### Примечание:
  - Кеширование списка пациентов
    - Список пагинации (для перемещения назад)
    - Список пациентов (для перехода на страницу карточки пациента)
- При добавление/редактирование/удаление задач
  - Кеширование
  - Логирование