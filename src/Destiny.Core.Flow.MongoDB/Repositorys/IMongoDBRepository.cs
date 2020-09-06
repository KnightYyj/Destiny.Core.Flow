﻿using Destiny.Core.Flow.Entity;
using Destiny.Core.Flow.Filter;
using Destiny.Core.Flow.Filter.Abstract;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Destiny.Core.Flow.MongoDB.Repositorys
{
    public interface IMongoDBRepository<TEntity, Tkey> 
        where TEntity : IEntity<Tkey>
    {
        //Find<T> – 返回集合中与提供的搜索条件匹配的所有文档。
        //InsertOne – 插入提供的对象作为集合中的新文档。
        //ReplaceOne – 将与提供的搜索条件匹配的单个文档替换为提供的对象。
        //DeleteOne – 删除与提供的搜索条件匹配的单个文档。
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        Task InsertAsync(TEntity entity);
        /// <summary>
        /// 数组创建
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        Task InsertAsync(TEntity[] entitys);
        IMongoQueryable<TEntity> Entities { get; }

        IMongoCollection<TEntity> Collection { get; }

        /// <summary>
        /// 异步分页
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="request"></param>
        /// <param name="orderConditions"></param>
        /// <returns></returns>
        Task<PageResult<TEntity>> ToPageAsync(Expression<Func<TEntity, bool>> predicate, IPagedRequest request);
    }
}
