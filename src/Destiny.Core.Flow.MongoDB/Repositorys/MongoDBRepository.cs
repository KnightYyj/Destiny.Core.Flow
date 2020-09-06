﻿using Destiny.Core.Flow.Entity;
using Destiny.Core.Flow.Extensions;
using Destiny.Core.Flow.Filter;
using Destiny.Core.Flow.Filter.Abstract;
using Destiny.Core.Flow.MongoDB.DbContexts;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Destiny.Core.Flow.MongoDB.Repositorys
{
    public class MongoDBRepository<TEntity, Tkey> : IMongoDBRepository<TEntity, Tkey>
       where TEntity : class, IEntity<Tkey>
       where Tkey : IEquatable<Tkey>
    {
        private readonly MongoDbContextBase _mongoDbContext = null;

        private readonly IPrincipal _principal;
        public virtual IMongoCollection<TEntity> Collection { get; private set; }
        //BsonDocument
        public MongoDBRepository(IServiceProvider serviceProvider, MongoDbContextBase mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;

            Collection = _mongoDbContext.Collection<TEntity>();
            _principal = serviceProvider.GetService<IPrincipal>();

        }
        public async Task InsertAsync(TEntity entity)
        {
            entity = entity.CheckInsert<TEntity, Tkey>(_principal);
            await Collection.InsertOneAsync(entity);
        }
        public async Task InsertAsync(TEntity[] entitys)
        {
            entitys = entitys.CheckInsert<TEntity, Tkey>(_principal);
            await Collection.InsertManyAsync(entitys);

        }




        public virtual IMongoQueryable<TEntity> Entities => Collection.AsQueryable();

        public virtual async Task<PageResult<TEntity>> ToPageAsync(Expression<Func<TEntity, bool>> predicate, IPagedRequest request)
        {
            var count = !predicate.IsNotNull() ? await Collection.CountDocumentsAsync(predicate) : await Collection.CountDocumentsAsync(FilterDefinition<TEntity>.Empty);
            var findFluent = Collection.Find(predicate).Skip(request.PageSize * (request.PageIndex - 1)).Limit(request.PageSize);

            findFluent = findFluent.OrderBy(request.OrderConditions);
            var lists = await findFluent.ToListAsync();
            return new PageResult<TEntity>() { ItemList = lists, Message = "加载成功", Success = true, Total = count.AsTo<int>() };

        }



    }
}

