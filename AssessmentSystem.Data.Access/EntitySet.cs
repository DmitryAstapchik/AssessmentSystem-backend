using System.Data.Entity;
using System.Linq;

namespace AssessmentSystem.Data.Access
{
    internal class EntitySet<TEntity> : EntitySetBase<TEntity>
        where TEntity : class
    {
        private readonly IDbSet<TEntity> _dbSet;

        public EntitySet(IDbSet<TEntity> dbSet)
        {
            _dbSet = dbSet;
        }

        protected override IQueryable<TEntity> Queryable => _dbSet;

        public override TEntity Add(TEntity entity)
        {
            return _dbSet.Add(entity);
        }

        public override TEntity Remove(TEntity entity)
        {
            return _dbSet.Remove(entity);
        }
    }
}
