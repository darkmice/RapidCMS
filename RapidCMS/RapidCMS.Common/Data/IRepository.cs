﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RapidCMS.Common.Interfaces;

namespace RapidCMS.Common.Data
{
    public interface IRepository
    {
        Task<IEntity> GetByIdAsync(int id, int? parentId);
        Task<IEnumerable<IEntity>> GetAllAsObjectsAsync(int? parentId);

        Task<IEntity> NewAsync(int? parentId);
        Task UpdateAsync(int id, int? parentId, IEntity entity);
    }
    
    public interface IRepository<TKey, TEntity> : IRepository
        where TEntity : IEntity
    {
        Task<TEntity> GetByIdAsync(TKey id, int? parentId);
        Task<IEnumerable<TEntity>> GetAllAsync(int? parentId);

        new Task<TEntity> NewAsync(int? parentId);
        Task UpdateAsync(int id, int? parentId, TEntity entity);
    }

    // TODO: find solution for int and TKey
    // TODO: add TKey? awesomeness

    public abstract class BaseRepository<TKey, TEntity> : IRepository, IRepository<int, TEntity>
        where TEntity : IEntity
    {
        public abstract Task<TEntity> GetByIdAsync(int id, int? parentId);
        public abstract Task<IEnumerable<TEntity>> GetAllAsync(int? parentId);
        public abstract Task<TEntity> NewAsync(int? parentId);
        public abstract Task UpdateAsync(int id, int? parentId, TEntity entity);

        async Task<IEntity> IRepository.GetByIdAsync(int id, int? parentId)
        {
            return (await GetByIdAsync(id, parentId)) as IEntity;
        }

        async Task<IEnumerable<IEntity>> IRepository.GetAllAsObjectsAsync(int? parentId)
        {
            return (await GetAllAsync(parentId)).Cast<IEntity>();
        }

        async Task<IEntity> IRepository.NewAsync(int? parentId)
        {
            return (await NewAsync(parentId)) as IEntity;
        }

        async Task IRepository.UpdateAsync(int id, int? parentId, IEntity entity)
        {
            await UpdateAsync(id, parentId, (TEntity)entity);
        }

    }
}
