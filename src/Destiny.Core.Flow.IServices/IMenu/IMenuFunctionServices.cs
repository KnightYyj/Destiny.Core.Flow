﻿using Destiny.Core.Flow.Dependency;
using Destiny.Core.Flow.Dtos.MenuFunction;
using Destiny.Core.Flow.Filter;
using Destiny.Core.Flow.Filter.Abstract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Destiny.Core.Flow.IServices.IMenu
{
    public interface IMenuFunctionServices : IScopedDependency
    {

        Task<IPagedResult<MenuFunctionOutPageListDto>> GetMenuFunctionListAsync(Guid menuId);
    }
}
