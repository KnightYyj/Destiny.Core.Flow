﻿using Destiny.Core.Flow;
using Destiny.Core.Flow.Entity;
using Destiny.Core.Flow.ExpressionUtil;
using Destiny.Core.Flow.Filter;
using Destiny.Core.Flow.Modules;
using Destiny.Core.Flow.MongoDB;
using Destiny.Core.Flow.MongoDB.DbContexts;
using Destiny.Core.Flow.MongoDB.Repositorys;
using Destiny.Core.Flow.TestBase;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Destiny.Core.Flow.Extensions;
using Destiny.Core.Flow.Filter.Abstract;

namespace Destiny.Core.Tests
{


    public class MongoDBTests : IntegratedTest<MongoDBModelule>
    {

        private readonly IMongoDBRepository<TestDB, Guid> _mongoDBRepository = null;
        public MongoDBTests()
        {
            _mongoDBRepository = ServiceProvider.GetService<IMongoDBRepository<TestDB, Guid>>();
        }


        [Fact]

        public async Task InsertEntityAsync_Test()
        {

            TestDB test = new TestDB();
            test.IsDeleted = false;
            test.CreatedTime = DateTime.Now;
            test.Name = $"大黄瓜18CM";
            await _mongoDBRepository.InsertAsync(test);

            var entitie = await _mongoDBRepository.Entities.Where(o => o.Id == test.Id).FirstOrDefaultAsync();
            Assert.True(entitie.Name == "大黄瓜18CM");
        }

        [Fact]
        public async Task GetPageAsync_Test()
        {
            try
            {
                FilterCondition condition = new FilterCondition();
                QueryFilter filter = new QueryFilter();
                //condition.Field = "Name";
                //condition.Value = "大黄瓜18CM";
                //filter.Conditions.Add(condition);
                var exp = FilterBuilder.GetExpression<TestDB>(filter);
                OrderCondition[] orderConditions = new OrderCondition[] {
                new OrderCondition("Name",Flow.Enums.SortDirection.Descending),
                new OrderCondition("CreatedTime")
               };
                PagedRequest pagedRequest = new PagedRequest();
                pagedRequest.OrderConditions = orderConditions;
                var page=  await _mongoDBRepository.ToPageAsync(exp, pagedRequest);

            }
            catch (Exception ex)
            {

                throw;
            }

        }



        [Fact]

        public async Task DeleteAsync_Test()
        {
            var re = await _mongoDBRepository.Collection.DeleteManyAsync(o => o.Name == "大黄瓜18CM");
        }


    }

    public class PagedRequest : IPagedRequest
    {

        public PagedRequest()
        {
            PageIndex = 1;
            PageSize = 10;
            OrderConditions = new OrderCondition[] { };
        }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public OrderCondition[] OrderConditions { get; set; }
    }

    public class MongoDBModelule : MongoDBModuleBase
    {

        //public override void ConfigureServices(ConfigureServicesContext context)
        //{
        //    var builder = new ConfigurationBuilder();
        //    var configuration= builder.AddJsonFile("appsettings.json").Build();

        //    var dbpath = configuration["Destiny:DbContext:MongoDBConnectionString"];
        //    var basePath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath; //获取项目路径
        //    var dbcontext = Path.Combine(basePath, dbpath);
        //    if (!File.Exists(dbcontext))
        //    {
        //        throw new Exception("未找到存放数据库链接的文件");
        //    }
        //    var connection = File.ReadAllText(dbcontext).Trim();


        //    var services = context.Services;
        //    services.AddMongoDbContext<DefaultMongoDbContext>(options => {
        //        options.ConnectionString = connection;
        //    });
        //    //services.AddSingleton<MongoDbContextOptions>(mongoDbContextOptions);
        //    //services.AddScoped<MongoDbContext,MongoDbContext>();
        //    services.AddScoped(typeof(IMongoDBRepository<,>), typeof(MongoDBRepository<,>));
        //}
        protected override void AddDbContext(IServiceCollection services)
        {
            var builder = new ConfigurationBuilder();
            var configuration = builder.AddJsonFile("appsettings.json").Build();

            var dbpath = configuration["Destiny:DbContext:MongoDBConnectionString"];
            var basePath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath; //获取项目路径
            var dbcontext = Path.Combine(basePath, dbpath);
            if (!File.Exists(dbcontext))
            {
                throw new Exception("未找到存放数据库链接的文件");
            }
            var connection = File.ReadAllText(dbcontext).Trim();



            services.AddMongoDbContext<DefaultMongoDbContext>(options =>
            {
                options.ConnectionString = connection;
            });

        }
    }


    public class TestMongoDbContext : MongoDbContextBase
    {

        public TestMongoDbContext(MongoDbContextOptions options) : base(options)
        {

        }


    }





    [MongoDBTable("TestDB")]//
    public class TestDB : EntityBase<Guid>, IFullAuditedEntity<Guid>
    {
        public TestDB()
        {
            Id = Guid.NewGuid();
        }


        public string Name { get; set; }

        /// <summary>
        ///  获取或设置 最后修改用户
        /// </summary>
        [DisplayName("最后修改用户")]
        public virtual Guid? LastModifierUserId { get; set; }
        /// <summary>
        /// 获取或设置 最后修改时间
        /// </summary>
        [DisplayName("最后修改时间")]
        public virtual DateTime? LastModifierTime { get; set; }
        /// <summary>
        ///获取或设置 是否删除
        /// </summary>
        [DisplayName("是否删除")]
        public virtual bool IsDeleted { get; set; }
        /// <summary>
        ///获取或设置 创建用户ID
        /// </summary>
        [DisplayName("创建用户ID")]
        public virtual Guid? CreatorUserId { get; set; }
        /// <summary>
        ///获取或设置 创建时间
        /// </summary>
        [DisplayName("创建时间")]
        public virtual DateTime CreatedTime { get; set; }
    }
}
