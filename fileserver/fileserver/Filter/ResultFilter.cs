using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace  FileServer.Filter
{
    /// <summary>
    /// 执行结果过滤，匹配微服务结果
    /// </summary>
    public class ResultFilter : IResultFilter
    {
        /// <inheritdoc/>
        public void OnResultExecuting(ResultExecutingContext context)
        {
        }

        /// <inheritdoc/>
        public void OnResultExecuted(ResultExecutedContext context)
        {
            // Can't add to headers here because response has already begun.
        }
    }
}
