﻿using DDD.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DDD.Repositories
{
    public interface IReadOnlyRepository<TEntity> where TEntity : class, IEntity
    {
        /// <summary>
        /// Получает сущность по условию.
        /// </summary>
        /// <param name="predicate">Условие для фильтрации.</param>
        /// <returns>Сущность или ничего</returns>
        Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получает все сущности, с возможностью фильтрации.
        /// </summary>
        /// <param name="predicate">Необязательное условие для фильтрации.</param>
        /// <returns>Список сущностей</returns>
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);
    }

    public interface IReadOnlyRepository<TEntity, TKey> : IReadOnlyRepository<TEntity> where TEntity : class, IEntity<TKey>
    {
    }
}