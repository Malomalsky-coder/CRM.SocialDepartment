using System;

namespace DDD.Entities
{
    [Serializable]
    public abstract class Entity : IEntity
    {
        protected Entity()
        {
        }

        public abstract object?[] GetKeys();

        public bool EntityEquals(IEntity other)
        {
            return EntityHelper.EntityEquals(this, other);
        }
    }

    [Serializable]
    public abstract class Entity<TKey> : Entity, IEntity<TKey>
    {
        public virtual TKey Id { get; protected set; } = default!;

        protected Entity()
        {
        }

        protected Entity(TKey id)
        {
            Id = id;
        }

        public override object?[] GetKeys()
        {
            return new object?[] { Id };
        }
    }
}