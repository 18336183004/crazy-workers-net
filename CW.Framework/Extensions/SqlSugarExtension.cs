using CW.Common.Attributes;
using CW.Common.BASE;
using CW.Common.Logs;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CW.Framework.Extensions
{
    public static class SqlSugarExtension
    {
        #region CRUD通用方法
        /// <summary>
        /// 查询 直接返回通用消息类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable">SqlSugar查询条件</param>
        /// <param name="index">页数</param>
        /// <param name="size">行数</param>
        /// <returns></returns>
        public static async Task<ResponseModel> ToResultAsync<T>(this ISugarQueryable<T> queryable, int? index = null, int? size = null)
        {
            try
            {
                if (index != null && size != null)
                {
                    RefAsync<int> totalCount = 0;
                    RefAsync<int> totalPage = 0;
                    var result = await queryable.ToPageListAsync((int)index, (int)size, totalCount, totalPage);
                    ResponseModel response = new ResponseModel(result)
                    {
                        DataCount = totalCount,
                        PageCount = totalPage
                    };
                    return response;
                }
                else
                {
                    var result = await queryable.ToListAsync();
                    return ResponseModel.Success(result);

                }
            }
            catch (Exception ex)
            {
                return ResponseModel.Error(ex.Message);
            }
        }
        /// <summary>
        /// 自定义新增 直接返回通用消息类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db">SqlSugar插入条件</param>
        /// <param name="errorMessage">错误信息</param>
        /// <returns></returns>
        public static async Task<ResponseModel> ExecuteAsync<T>(this IInsertable<T> db, string errorMessage = null)
            where T : class, new()
        {
            try
            {
                var res = await db.ExecuteReturnBigIdentityAsync();
                if (res > 0)
                    return ResponseModel.Success(res);
                else
                    throw new UserOperationException("插入失败!");
            }
            catch (Exception ex)
            {
                return ResponseModel.Error(!string.IsNullOrEmpty(errorMessage) ? errorMessage : ex.Message);
            }
        }
        /// <summary>
        /// 更新 直接返回通用消息类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db">SqlSugar更新条件</param>
        /// <param name="isCheckAffectedRows">是否校验受影响行数大于0(默认是)</param>
        /// <param name="errorMessage">错误信息</param>
        /// <returns></returns>
        public static async Task<ResponseModel> ExecuteAsync<T>(this IUpdateable<T> db, bool isCheckAffectedRows = true, string errorMessage = null) where T : class, new()
        {
            try
            {
                var res = await db.ExecuteCommandAsync();
                if (res > 0 || !isCheckAffectedRows)
                    return ResponseModel.Success();
                else
                    throw new UserOperationException("更新失败!");
            }
            catch (Exception ex)
            {
                return ResponseModel.Error(!string.IsNullOrEmpty(errorMessage) ? errorMessage : ex.Message);
            }
        }
        /// <summary>
        /// 软删除 直接返回通用消息类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="updateColumns">更新列</param>
        /// <param name="isCheckAffectedRows">是否校验受影响行数大于0(默认是)</param>
        /// <param name="errorMessage">错误信息</param>
        /// <returns></returns>
        public static async Task<ResponseModel> SoftDeleteAsync<T>(this IDeleteable<T> db, Expression<Func<T, T>> updateColumns, bool isCheckAffectedRows = true, string errorMessage = null)
            where T : class, new()
        {
            //获取provider，provider用于获取update更新
            var provider = db.IsLogic();

            //获取where条件和参数
            //var where = provider.DeleteBuilder.GetWhereString[5..]; //去除where
            var pars = provider.DeleteBuilder.Parameters;

            //获取IUpdateable<T>
            IUpdateable<T> updateTable = provider.Deleteable.Context.Updateable<T>();

            //参数赋值
            if (pars != null)
            {
                updateTable.UpdateBuilder.Parameters.AddRange(pars);
            }

            //校验更新列
            if (updateColumns == null)
            {
                throw new Exception("Update columns can not be null!");
            }

            //设置更新列
            updateTable.SetColumns(updateColumns);

            try
            {
                var res = await updateTable.ExecuteCommandAsync();
                if (res > 0 || !isCheckAffectedRows)
                    return ResponseModel.Success();
                else
                    throw new UserOperationException("删除失败!");
            }
            catch (Exception ex)
            {
                return ResponseModel.Error(!string.IsNullOrEmpty(errorMessage) ? errorMessage : ex.Message);
            }
        }
        /// <summary>
        /// 事务(无返回值) 直接返回通用消息类
        /// </summary>
        /// <param name="db">SqlSugarScope</param>
        /// <param name="func">委托方法</param>
        /// <param name="errorMessage">错误信息</param>
        /// <param name="errorCallBack">错误回调</param>
        /// <returns></returns>
        public static async Task<ResponseModel> TransactionAsync(this SqlSugarScope db, Func<Task> func, string errorMessage = null, Action<Exception> errorCallBack = null)
        {
            var result = await db.UseTranAsync(func, errorCallBack);
            if (result.IsSuccess)
                return ResponseModel.Success();
            else
                return ResponseModel.Error(!string.IsNullOrEmpty(errorMessage) ? errorMessage : result.ErrorMessage);
        }
        /// <summary>
        /// 事务(有返回值) 直接返回通用消息类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db">SqlSugarScope</param>
        /// <param name="func">委托方法</param>
        /// <param name="errorMessage">错误信息</param>
        /// <param name="errorCallBack">错误回调</param>
        /// <returns></returns>
        public static async Task<ResponseModel> TransactionAsync<T>(this SqlSugarScope db, Func<Task<T>> func, string errorMessage = null, Action<Exception> errorCallBack = null)
        {
            var result = await db.UseTranAsync(func, errorCallBack);
            if (result.IsSuccess)
                return ResponseModel.Success(result.Data);
            else
                return ResponseModel.Error(!string.IsNullOrEmpty(errorMessage) ? errorMessage : result.ErrorMessage);
        }
        /// <summary>
        /// 事务 直接返回通用消息类
        /// </summary>
        /// <param name="db"></param>
        /// <param name="func"></param>
        /// <param name="errorMessage"></param>
        /// <param name="errorCallBack"></param>
        /// <returns></returns>
        public static async Task<ResponseModel> TransactionAsync(this SqlSugarScope db, Func<Task<ResponseModel>> func, string errorMessage = null, Action<Exception> errorCallBack = null)
        {
            ResponseModel result = ResponseModel.Error(errorMessage);
            try
            {
                db.BeginTran();

                if (func != null)
                {
                    result = await func();
                }
                if (result?.Code == ResponseCode.Success)
                {
                    db.CommitTran();
                }
                else
                {
                    db.RollbackTran();
                }
                return result;
            }
            catch (UserOperationException ex)
            {
                result.Message = !string.IsNullOrEmpty(errorMessage) ? errorMessage : ex.Message;
                db.RollbackTran();
                errorCallBack?.Invoke(ex);
                return result;
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    result.Message = errorMessage;
                }
                else
                {
                    //if (EnvironmentHelper.IsFat)
                    {
                        result.Message = "操作失败!";
                        NLogHelper.ErrorLog($"执行事务出错，{ex.Message}", ex);
                    }
                    //else
                    //{
                    //    result.Message = ex.Message;
                    //}
                }

                db.RollbackTran();

                errorCallBack?.Invoke(ex);
                return result;
            }
        }
        #endregion

        #region 扩展
        /// <summary>
        /// SqlSugar自定义扩展
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static List<SqlFuncExternal> CustomSqlFunc()
        {
            List<SqlFuncExternal> resultList = new List<SqlFuncExternal>
            {
                //GROUP_CONCAT
                new SqlFuncExternal
                {
                    UniqueMethodName = "GroupConcat",
                    MethodValue = (expInfo, dbType, expContext) =>
                    {
                        if (dbType == DbType.MySql)
                        {
                            return $@"GROUP_CONCAT( {expInfo.Args[0].MemberName} SEPARATOR '{expInfo.Args[1].MemberValue}' ) ";
                        }
                        else
                        {
                            throw new NotSupportedException("Not Support this database");
                        }
                    }
                }
            };

            return resultList;
        }
        /// <summary>
        /// GROUP_CONCAT
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column">字段</param>
        /// <param name="separator">分隔符</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static string GroupConcat<T>(T column, string separator)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        #endregion
    }
}
