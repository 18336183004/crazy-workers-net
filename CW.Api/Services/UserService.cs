using CW.Api.IServices;
using CW.Api.Models;
using CW.Api.Services.BASE;
using CW.Common.Attributes;
using CW.Common.BASE;
using CW.Common.Enums;
using CW.Framework.Entities;
using CW.Framework.Extensions;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SqlSugar;

namespace CW.Api.Services
{
    public class UserService : BaseService, IUserService
    {
        public UserService(IHttpContextAccessor httpContext) : base(httpContext)
        {

        }

        /// <summary>
        /// 用户授权
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseModel> Authorisation(LoginUserModel model)
        {
            var user = await _dbContext.Queryable<user>().Where(c => c.OpenId == model.OpenId).FirstAsync();
            if (user != null)
            {
                return await _dbContext.Updateable<user>()
               .SetColumns(c => new user
               {
                   Name = model.Name,
                   Tel = model.Tel,
                   Picture = model.Picture,
               }).Where(c => c.OpenId == model.OpenId)
               .ExecuteAsync();
            }
            throw new UserOperationException("用户登录失败!");
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseModel> GetUserInfo()
        {
            ResponseModel response = new();
            var result = await _dbContext.Queryable<user>().Where(c => c.OpenId == OpenId && c.Id == UserId)
                .Select(c => new UserInfoModel
                {
                    Name = c.Name,
                    Tel = c.Tel,
                    Picture = c.Picture,
                    CollectId = SqlFunc.Subqueryable<usercollect>().Where(a => a.UserId == UserId && a.IsWear == 1).Select(a => a.CollectId),
                    Collect = SqlFunc.Subqueryable<collect>().Where(c => c.Id == SqlFunc.Subqueryable<usercollect>().Where(a => a.UserId == UserId && a.IsWear == 1).Select(c => c.CollectId)).Select(c => c.Picture),
                    FullBodyPhoto = SqlFunc.Subqueryable<collect>().Where(c => c.Id == SqlFunc.Subqueryable<usercollect>().Where(a => a.UserId == UserId && a.IsWear == 1).Select(c => c.CollectId)).Select(c => c.FullBodyPhoto),
                    PositionLevelId = c.PositionLevelId,
                    PositionLevelName = SqlFunc.Subqueryable<positionlevel>().Where(b => b.Id == c.PositionLevelId).Select(b => b.Name),
                    Area = SqlFunc.Subqueryable<province>().Where(b => b.Id == c.ProvinceId).Select(b => b.Name),
                    Day = (int)c.WorkingDays
                }).FirstAsync();
            response.Data = result;
            return response;
        }

        /// <summary>
        /// 获取玩家收藏列表
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseModel> GetUserCollectList()
        {
            ResponseModel response = new();

            UserCollectData userCollectDate = new();
            //查询收藏设置表
            var collects = await _dbContext.Queryable<collect>().Where(c => c.Status == (byte)BaseEnums.IsValid.Valid).OrderBy(c => c.Sort).ToListAsync();
            if (collects?.Count > 0)
            {
                List<UserCollectModel> userCollectModels = new List<UserCollectModel>();
                //查询玩家获得的收藏 
                var userCollects = await _dbContext.Queryable<usercollect>().Where(c => c.UserId == UserId).ToListAsync();
                var positionlevels = await _dbContext.Queryable<positionlevel>().Where(c => c.Status == (byte)BaseEnums.IsValid.Valid).ToListAsync();
                foreach (var item in collects)
                {
                    UserCollectModel model = new()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Picture = item.Picture,
                        FullBodyPhoto = item.FullBodyPhoto,
                        PositionLevelId = item.PositionLevelId,
                        PositionLevelIName = positionlevels?.Where(c => c.Id == item.PositionLevelId).Select(c => c.Name).FirstOrDefault(),

                        Description = item.Description,
                        IsHold = (byte)userCollects.Count(a => a.CollectId == item.Id),
                        IsWear = (byte)userCollects.Count(a => a.CollectId == item.Id && a.IsWear == 1),
                    };
                    model.Condition = $"升级为[{model.PositionLevelIName}]获得此装扮";
                    userCollectModels.Add(model);
                }
                userCollectDate.UserCollects = userCollectModels;
                userCollectDate.HoldNum = userCollects.Count;
                userCollectDate.Total = collects.Count;
            }
            response.Data = userCollectDate;
            return response;
        }

        /// <summary>
        /// 玩家穿戴收藏装扮
        /// </summary>
        /// <param name="collectId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UserWearCollect(int collectId)
        {
            //查询收藏设置表
            var collect = await _dbContext.Queryable<collect>().Where(c => c.Id == collectId).FirstAsync();
            if (collect != null)
            {
                var userCollects = await _dbContext.Queryable<usercollect>().Where(c => c.UserId == UserId).ToListAsync();
                if (userCollects?.Count > 0)
                {
                    //查询被选中的装扮
                    var userCollect = userCollects?.Where(c => c.CollectId == collectId).FirstOrDefault();
                    if (userCollect != null)
                    {
                        //开启事务
                        return await _dbContext.TransactionAsync(async () =>
                        {
                            ResponseModel result = ResponseModel.Success();
                            //查询是否有穿戴的装扮
                            var wearCollect = userCollects?.Where(c => c.IsWear == 1).FirstOrDefault();
                            if (wearCollect != null)
                            {
                                //穿戴的装扮改为未穿戴
                                result = await _dbContext.Updateable<usercollect>()
                                                                                .SetColumns(c => new usercollect
                                                                                {
                                                                                    IsWear = 0
                                                                                }).Where(c => c.Id == wearCollect.Id)
                                                                                .ExecuteAsync();
                            }
                            if (result.Code == ResponseCode.Success)
                            {
                                //重新进行穿戴新选择的装扮
                                result = await _dbContext.Updateable<usercollect>()
                                                         .SetColumns(c => new usercollect
                                                         {
                                                             IsWear = 1
                                                         }).Where(c => c.Id == userCollect.Id)
                                                         .ExecuteAsync();
                            }
                            return result;
                        });
                    }
                }
                throw new UserOperationException("玩家没有当前装扮!");
            }
            throw new UserOperationException("收藏装扮配置中没有当前装扮!");
        }

        /// <summary>
        /// 获取玩家荣誉列表
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseModel> GetUserHonorList()
        {
            ResponseModel response = new();

            UserHonorData userHonorData = new();
            //查询收藏设置表
            var honors = await _dbContext.Queryable<honor>().Where(c => c.Status == (byte)BaseEnums.IsValid.Valid).ToListAsync();
            if (honors?.Count > 0)
            {
                List<UserHonorModel> userCollectModels1 = new List<UserHonorModel>();
                List<UserHonorModel> userCollectModels2 = new List<UserHonorModel>();
                List<UserHonorModel> userCollectModels3 = new List<UserHonorModel>();
                //查询玩家获得的收藏 
                var userhonors = await _dbContext.Queryable<userhonor>().Where(c => c.UserId == UserId).ToListAsync();
                var list = honors.Where(c => c.Type == 1).OrderBy(c => c.Sort).ToList();
                var list2 = honors.Where(c => c.Type == 2).OrderBy(c => c.Sort).ToList();
                var list3 = honors.Where(c => c.Type == 3).OrderBy(c => c.Sort).ToList();
                foreach (var item in list)
                {
                    userCollectModels1.Add(new UserHonorModel()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Picture = item.Picture,
                        Description = item.Description,
                        IsHold = (byte)userhonors.Count(a => a.HonorId == item.Id),
                        IsWear = (byte)userhonors.Count(a => a.HonorId == item.Id && a.IsWear == 1),
                    });
                }
                foreach (var item in list2)
                {
                    userCollectModels2.Add(new UserHonorModel()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Picture = item.Picture,
                        Description = item.Description,
                        IsHold = (byte)userhonors.Count(a => a.HonorId == item.Id),
                        IsWear = (byte)userhonors.Count(a => a.HonorId == item.Id && a.IsWear == 1),
                    });
                }
                foreach (var item in list3)
                {
                    userCollectModels3.Add(new UserHonorModel()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Picture = item.Picture,
                        Description = item.Description,
                        IsHold = (byte)userhonors.Count(a => a.HonorId == item.Id),
                        IsWear = (byte)userhonors.Count(a => a.HonorId == item.Id && a.IsWear == 1),
                    });
                }
                userHonorData.List1 = userCollectModels1;
                userHonorData.List2 = userCollectModels2;
                userHonorData.List3 = userCollectModels3;

                if (userCollectModels1.Any(c => c.IsWear == 1))
                {
                    userHonorData.UserHonor = userCollectModels1?.FirstOrDefault(c => c.IsWear == 1);
                }
                if (userCollectModels2.Any(c => c.IsWear == 1))
                {
                    userHonorData.UserHonor = userCollectModels2?.FirstOrDefault(c => c.IsWear == 1);
                }
                if (userCollectModels3.Any(c => c.IsWear == 1))
                {
                    userHonorData.UserHonor = userCollectModels3?.FirstOrDefault(c => c.IsWear == 1);
                }
            }
            response.Data = userHonorData;
            return response;
        }

        /// <summary>
        /// 玩家穿戴荣誉
        /// </summary>
        /// <param name="honorId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UserWearHonor(int honorId)
        {
            //查询荣誉设置表
            var honor = await _dbContext.Queryable<honor>().Where(c => c.Id == honorId).FirstAsync();
            if (honor != null)
            {
                var userHonors = await _dbContext.Queryable<userhonor>().Where(c => c.UserId == UserId).ToListAsync();
                if (userHonors?.Count > 0)
                {
                    //查询被选中的荣誉
                    var userHonor = userHonors?.Where(c => c.HonorId == honorId).FirstOrDefault();
                    if (userHonor != null)
                    {
                        //开启事务
                        return await _dbContext.TransactionAsync(async () =>
                        {
                            ResponseModel result = ResponseModel.Success();
                            //查询是否有穿戴的荣誉
                            var wearHonor = userHonors?.Where(c => c.IsWear == 1).FirstOrDefault();
                            if (wearHonor != null)
                            {
                                //穿戴的荣誉改为未穿戴
                                result = await _dbContext.Updateable<userhonor>()
                                                                                .SetColumns(c => new userhonor
                                                                                {
                                                                                    IsWear = 0
                                                                                }).Where(c => c.Id == wearHonor.Id)
                                                                                .ExecuteAsync();
                            }
                            if (result?.Code == ResponseCode.Success)
                            {
                                //重新进行穿戴新选择的荣誉
                                result = await _dbContext.Updateable<userhonor>()
                                                             .SetColumns(c => new userhonor
                                                             {
                                                                 IsWear = 1
                                                             }).Where(c => c.Id == userHonor.Id)
                                                             .ExecuteAsync();
                            }
                            return result;
                        });
                    }
                }
                throw new UserOperationException("玩家没有当前荣誉!");
            }
            throw new UserOperationException("荣誉配置中没有当前荣誉!");
        }

        /// <summary>
        /// 添加游玩记录
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseModel> AddRecord()
        {
            ResponseModel response = new();
            if (!await _dbContext.Queryable<loginrecord>().AnyAsync(c => c.UserId == UserId && c.PlayDate == _dbContext.GetDate().Date))
            {
                //创建记录
                loginrecord record = new()
                {
                    UserId = (int)UserId,
                    PlayDate = _dbContext.GetDate().Date,
                    IsSuccess = 0,
                    SatrtDate = _dbContext.GetDate()
                };
                return await _dbContext.Insertable(record).ExecuteAsync();
            }
            return response;
        }

        /// <summary>
        /// 结束本局游戏
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseModel> EndThisGame(EndThisGameModel model)
        {
            ResponseModel response = new();
            var info = await _dbContext.Queryable<loginrecord>()
                                     .Where(c => c.UserId == UserId && c.PlayDate == _dbContext.GetDate().Date)
                                     .FirstAsync();
            if (info != null)
            {
                //正常关卡
                if (model.Barrier == 0)
                {
                    string? json = null;
                    List<RecordModel> records = new();
                    if (string.IsNullOrEmpty(info.Data))
                    {
                        RecordModel record = new()
                        {
                            Id = 1,
                            EndTime = _dbContext.GetDate(),
                            IsSuccess = model.Type
                        };
                        records.Add(record);
                        json = JsonConvert.SerializeObject(records);
                    }
                    else
                    {
                        records = JsonConvert.DeserializeObject<List<RecordModel>>(info.Data);
                        RecordModel record = new()
                        {
                            Id = 1,
                            EndTime = _dbContext.GetDate(),
                            IsSuccess = model.Type
                        };
                        records.Add(record);
                        json = JsonConvert.SerializeObject(records);
                    }
                    //开启事务
                    response = await _dbContext.TransactionAsync(async () =>
                    {
                        var result = await _dbContext.Updateable<loginrecord>()
                                         .SetColumns(c => c.Data == json)
                                         .SetColumnsIF(info.IsSuccess != 1, c => c.IsSuccess == model.Type)
                                         .Where(c => c.UserId == UserId && c.PlayDate == _dbContext.GetDate().Date)
                                         .ExecuteAsync();
                        if (result.Code == ResponseCode.Success)
                        {
                            if (await _dbContext.Queryable<loginrecord>().AnyAsync(c => c.UserId == UserId && c.PlayDate == _dbContext.GetDate().Date && c.IsSuccess == 1))
                            {
                                result = await _dbContext.Updateable<user>()
                                       .SetColumns(c => c.WorkingDays == c.WorkingDays + 1)
                                       .Where(c => c.Id == UserId && c.OpenId == OpenId)
                                       .ExecuteAsync();
                            }
                        }
                        //职级Id
                        long? positionlevelId = null;
                        //装扮
                        if (result.Code == ResponseCode.Success)
                        {
                            //职级，装扮变动
                            if ((info.IsSuccess == 1 && model.Type == 0) || (info.IsSuccess == 0 && model.Type == 1))
                            {
                                var user = await _dbContext.Queryable<user>().Where(c => c.Id == UserId).FirstAsync();
                                if (user != null)
                                {
                                    var positionlevelInfo = await _dbContext.Queryable<positionlevel>().Where(c => c.Id == user.PositionLevelId + 1).FirstAsync();
                                    if (positionlevelInfo != null)
                                    {
                                        PositionLevelModel positionLevelModel = new()
                                        {
                                            Id = positionlevelInfo.Id,
                                            Name = positionlevelInfo.Name,
                                            Picture = positionlevelInfo.Picture,
                                        };
                                        result = await AddToBeNotified(UserId, 0, 1, positionLevelModel);
                                        if (result?.Code != ResponseCode.Success)
                                        {
                                            throw new UserOperationException("职级待通知消息报错!");
                                        }
                                        positionlevelId = positionlevelInfo.Id;
                                        //查询装扮
                                        var collect = await _dbContext.Queryable<collect>().Where(c => c.PositionLevelId == positionlevelInfo.Id).FirstAsync();
                                        if (collect != null && !await _dbContext.Queryable<usercollect>().AnyAsync(c => c.UserId == UserId && c.CollectId == collect.Id))
                                        {
                                            CollectModel collectModel = new()
                                            {
                                                Id = collect.Id,
                                                Name = collect.Name,
                                                Picture = collect.Picture,
                                                FullBodyPhoto = collect.FullBodyPhoto
                                            };
                                            result = await AddToBeNotified(UserId, 1, 1, collectModel);
                                            if (result?.Code != ResponseCode.Success)
                                            {
                                                throw new UserOperationException("装扮待通知消息报错!");
                                            }
                                            usercollect usercollect = new()
                                            {
                                                UserId = UserId,
                                                CollectId = collect.Id,
                                                Ispermanent = 0,
                                                IsWear = 0,
                                                IsFixed = 0,
                                                ObtainingDate = DateTime.Now,
                                            };
                                            result = await _dbContext.Insertable(usercollect).ExecuteAsync();
                                            if (result?.Code != ResponseCode.Success)
                                            {
                                                throw new UserOperationException("新增装扮报错!");
                                            }
                                        }

                                        //升级为董事长的时候，全部已有装扮变为永久
                                        if (positionlevelId == 14)
                                        {
                                            result = await _dbContext.Updateable<usercollect>().SetColumns(c => c.Ispermanent == 1).Where(c => c.UserId == UserId && c.Ispermanent == 0).ExecuteAsync();
                                        }
                                    }
                                }
                            }
                        }
                        //荣誉
                        if (result.Code == ResponseCode.Success)
                        {
                            var honorList = await _dbContext.Queryable<honor>().ToListAsync();
                            //荣誉变动
                            //查询上班记录
                            var list = await _dbContext.Queryable<loginrecord>().Where(c => c.UserId == UserId).ToListAsync();
                            if (list?.Count > 0)
                            {
                                //查询迟到天数荣誉
                                // 排序日期列表（确保日期是排序的）  
                                var dates = list.Where(c => c.IsSuccess == 0).Select(c => c.PlayDate).OrderBy(c => c).ToList();

                                List<TimePeriod> continuousPeriods = GetContinuousPeriods(dates);
                                int bay = continuousPeriods.OrderByDescending(c => c.DurationDays).Select(c => c.DurationDays).FirstOrDefault();
                                if (bay != 0)
                                {
                                    List<long> NewHonorIds = new();
                                    var userhonors = await _dbContext.Queryable<userhonor>().Where(c => c.UserId == UserId).ToListAsync();
                                    if (bay >= 1 && bay < 3 && !userhonors.Any(c => c.HonorId == 301))
                                    {
                                        NewHonorIds.Add(301);
                                    }
                                    else if (bay >= 3 && bay < 7 && !userhonors.Any(c => c.HonorId == 302))
                                    {
                                        NewHonorIds.Add(302);
                                    }
                                    else
                                    {
                                        if (!userhonors.Any(c => c.HonorId == 303))
                                        {
                                            NewHonorIds.Add(303);
                                        }
                                    }
                                    //职级到CEO和董事长，添加游艇荣誉
                                    if (positionlevelId != null && (positionlevelId == 13 || positionlevelId == 14) && !userhonors.Any(c => c.HonorId == 605))
                                    {
                                        NewHonorIds.Add(605);
                                    }
                                    if (NewHonorIds?.Count > 0)
                                    {
                                        List<tobenotified> tobenotifieds = new List<tobenotified>();
                                        List<HonorModel> honors = new List<HonorModel>();
                                        foreach (var item in NewHonorIds)
                                        {

                                            var info1 = honorList?.Where(c => c.Id == item).FirstOrDefault();
                                            if (info1 != null)
                                            {
                                                tobenotified tobenotified = new()
                                                {
                                                    UserId = UserId,
                                                    State = 1,
                                                    Type = 2,
                                                    IsRead = 0,
                                                    CreateDate = _dbContext.GetDate()
                                                };

                                                HonorModel honorModel = new()
                                                {
                                                    Id = info1.Id,
                                                    Name = info1.Name,
                                                    Picture = info1.Picture,
                                                };
                                                tobenotified.Data = JsonConvert.SerializeObject(honorModel);
                                                tobenotifieds.Add(tobenotified);
                                                honors.Add(honorModel);
                                            }
                                        }

                                        if (honors?.Count > 0)
                                        {
                                            result = await _dbContext.Insertable(tobenotifieds).ExecuteAsync();
                                            if (result?.Code != ResponseCode.Success)
                                            {
                                                throw new UserOperationException("荣誉新增报错!");
                                            }
                                            List<userhonor> userhonorList = new List<userhonor>();
                                            foreach (var honor in honors)
                                            {
                                                userhonor userhonor = new()
                                                {
                                                    UserId = UserId,
                                                    HonorId = (long)honor.Id,
                                                    IsWear = 0,
                                                    ObtainingDate = DateTime.Now,
                                                };
                                                userhonorList.Add(userhonor);
                                            }
                                            if (userhonorList?.Count > 0)
                                            {
                                                result = await _dbContext.Insertable(userhonorList).ExecuteAsync();
                                                if (result?.Code != ResponseCode.Success)
                                                {
                                                    throw new UserOperationException("新增玩家荣誉报错!");
                                                }
                                            }
                                        }
                                    }
                                }

                                //查询14天通关
                                var dates1 = list.Where(c => c.IsSuccess == 1).Select(c => c.PlayDate).OrderBy(c => c).ToList();
                                List<TimePeriod> continuousPeriods1 = GetContinuousPeriods(dates1);
                                int bay1 = continuousPeriods1.OrderByDescending(c => c.DurationDays).Select(c => c.DurationDays).FirstOrDefault();
                                if (bay1 != 0 && bay1 >= 14)
                                {
                                    var info1 = honorList?.Where(c => c.Id == 5).FirstOrDefault();
                                    if (info1 != null)
                                    {
                                        tobenotified entity = new()
                                        {
                                            UserId = UserId,
                                            State = 1,
                                            Type = 2,
                                            IsRead = 0,
                                            CreateDate = _dbContext.GetDate()
                                        };
                                        HonorModel honorModel = new()
                                        {
                                            Id = info1.Id,
                                            Name = info1.Name,
                                            Picture = info1.Picture,
                                        };
                                        entity.Data = JsonConvert.SerializeObject(honorModel);

                                        result = await _dbContext.Insertable(entity).ExecuteAsync();
                                        if (result?.Code != ResponseCode.Success)
                                        {
                                            throw new UserOperationException("新增待通知消息报错!");
                                        }
                                        userhonor userhonor = new()
                                        {
                                            UserId = UserId,
                                            HonorId = (long)info1.Id,
                                            IsWear = 0,
                                            ObtainingDate = DateTime.Now,
                                        };

                                        result = await _dbContext.Insertable(userhonor).ExecuteAsync();
                                        if (result?.Code != ResponseCode.Success)
                                        {
                                            throw new UserOperationException("新增玩家荣誉报错!");
                                        }
                                    }
                                }

                                //查询加班通关处理
                                var dates2 = list.Where(c => c.IsWorkOvertime == 1 && c.IsWorkOvertimeSuccess == 1).Select(c => c.PlayDate).OrderBy(c => c).ToList();
                                List<TimePeriod> continuousPeriods2 = GetContinuousPeriods(dates2);
                                int bay2 = continuousPeriods1.OrderByDescending(c => c.DurationDays).Select(c => c.DurationDays).FirstOrDefault();
                                if (bay2 != 0)
                                {
                                    List<long> NewHonorIds = new();
                                    var userhonors = await _dbContext.Queryable<userhonor>().Where(c => c.UserId == UserId).ToListAsync();
                                    if (bay2 >= 3 && bay2 < 7 && !userhonors.Any(c => c.HonorId == 6))
                                    {
                                        NewHonorIds.Add(6);
                                    }
                                    else
                                    {
                                        if (!userhonors.Any(c => c.HonorId == 7))
                                        {
                                            NewHonorIds.Add(7);
                                        }
                                    }
                                    if (NewHonorIds?.Count > 0)
                                    {
                                        List<tobenotified> tobenotifieds = new List<tobenotified>();
                                        List<HonorModel> honors = new List<HonorModel>();
                                        foreach (var item in NewHonorIds)
                                        {

                                            var info1 = honorList?.Where(c => c.Id == item).FirstOrDefault();
                                            if (info1 != null)
                                            {
                                                tobenotified tobenotified = new()
                                                {
                                                    UserId = UserId,
                                                    State = 1,
                                                    Type = 2,
                                                    IsRead = 0,
                                                    CreateDate = _dbContext.GetDate()
                                                };

                                                HonorModel honorModel = new()
                                                {
                                                    Id = info1.Id,
                                                    Name = info1.Name,
                                                    Picture = info1.Picture,
                                                };
                                                tobenotified.Data = JsonConvert.SerializeObject(honorModel);
                                                tobenotifieds.Add(tobenotified);
                                                honors.Add(honorModel);
                                            }
                                        }

                                        if (honors?.Count > 0)
                                        {
                                            result = await _dbContext.Insertable(tobenotifieds).ExecuteAsync();
                                            if (result?.Code != ResponseCode.Success)
                                            {
                                                throw new UserOperationException("荣誉新增报错!");
                                            }
                                            List<userhonor> userhonorList = new List<userhonor>();
                                            foreach (var honor in honors)
                                            {
                                                userhonor userhonor = new()
                                                {
                                                    UserId = UserId,
                                                    HonorId = honor.Id,
                                                    IsWear = 0,
                                                    ObtainingDate = DateTime.Now,
                                                };
                                                userhonorList.Add(userhonor);
                                            }
                                            if (userhonorList?.Count > 0)
                                            {
                                                result = await _dbContext.Insertable(userhonorList).ExecuteAsync();
                                                if (result?.Code != ResponseCode.Success)
                                                {
                                                    throw new UserOperationException("新增玩家荣誉报错!");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        return result;
                    });
                }
                //加班关卡
                else
                {

                }
            }
            response.Data = await _dbContext.Queryable<user>()
                                      .Where(c => c.Id == UserId && c.OpenId == OpenId)
                                      .Select(c => new UserInfo
                                      {
                                          Id = c.PositionLevelId,
                                          Name = SqlFunc.Subqueryable<positionlevel>().Where(a => a.Id == c.PositionLevelId && a.Status == 1).Select(a => a.Name),
                                          WorkingDays = c.WorkingDays
                                      })
                                      .FirstAsync();
            return response;
        }

        /// <summary>
        /// 获取省份排行
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseModel> GetProvinceRanking()
        {
            ResponseModel response = new();

            ProvinceRankingData data = new();

            var result = await _dbContext.Queryable<province>()
                                                          .Select(p => new ProvinceRankingModel
                                                          {
                                                              Id = p.Id,
                                                              Name = p.Name,
                                                              TotalNumber = SqlFunc.Subqueryable<user>().LeftJoin<loginrecord>((u, l) => u.Id == l.UserId).Where((u, l) => _dbContext.GetDate().Month == l.PlayDate.Month && u.ProvinceId == p.Id).Count(),
                                                              Attendance = (double)SqlFunc.Subqueryable<user>().LeftJoin<loginrecord>((u, l) => u.Id == l.UserId).Where((u, l) => _dbContext.GetDate().Month == l.PlayDate.Month && l.IsSuccess == 1 && u.ProvinceId == p.Id).Count() /
                                                                                     (double)SqlFunc.Subqueryable<user>().LeftJoin<loginrecord>((u, l) => u.Id == l.UserId).Where((u, l) => _dbContext.GetDate().Month == l.PlayDate.Month && u.ProvinceId == p.Id).Count()
                                                          })
                                                          .MergeTable()
                                                          .OrderByDescending(p => p.TotalNumber)
                                                          .ToListAsync();
            //var result = await _dbContext.Queryable<province>()
            //                                               .LeftJoin<user>((p, u) => p.Id == u.ProvinceId)
            //                                               .LeftJoin<loginrecord>((p, u, l) => u.Id == l.UserId)
            //                                               .Where((p, u, l) => _dbContext.GetDate().Month == l.PlayDate.Month)//查询本月记录
            //                                               .Select((p, u, l) => new ProvinceRankingModel
            //                                               {
            //                                                   Name = p.Name,
            //                                                   TotalNumber = SqlFunc.AggregateCount(l.UserId),
            //                                                   Attendance = $"{(SqlFunc.AggregateCount(l.IsSuccess == 1) / SqlFunc.AggregateCount(l.UserId)) * 100}%",
            //                                               })
            //                                               .MergeTable()
            //                                               .OrderBy(c => c.TotalNumber)
            //                                               .ToListAsync();
            data.ProvinceRankings = result;
            data.UserProvinceId = await _dbContext.Queryable<user>()
                                                           .Where(u => u.Id == UserId && u.OpenId == OpenId)
                                                           .Select(c => c.ProvinceId)
                                                           .FirstAsync();

            response.Data = data;
            return response;

        }

        /// <summary>
        /// 查询邀请申请
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseModel> InvitationRecord()
        {
            ResponseModel response = new();

            var result = await _dbContext.Queryable<invitationrecord>()
                .LeftJoin<loginrecord>((i, l) => i.InvitationId == l.UserId && i.InvitationDate == _dbContext.GetDate().Date)
                .Where((i, l) => i.UserId == UserId && i.IsUse == 0).ToListAsync();
            int bay = 0;
            if (result?.Count >= 3)
            {
                bay = 3;
            }
            else
            {
                bay = result.Count();
            }
            RewardModel reward = new()
            {
                Reward = await _dbContext.Queryable<usercollect>().AnyAsync(c => c.UserId == UserId && c.IsFixed == 1) ? 1 : 0,
                Reward1 = 2,
                Num = bay
            };
            response.Data = reward;
            return response;
        }

        /// <summary>
        /// 领取邀请奖励
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseModel> ReceiveInvitation()
        {
            ResponseModel response = new();
            var result = await _dbContext.Queryable<invitationrecord>()
              .LeftJoin<loginrecord>((i, l) => i.InvitationId == l.UserId && i.InvitationDate == _dbContext.GetDate().Date)
              .Where((i, l) => i.UserId == UserId && i.IsUse == 0).ToListAsync();
            if (result?.Count >= 3)
            {
                //发放收藏
                if (!await _dbContext.Queryable<usercollect>().AnyAsync(c => c.CollectId == 1 && c.UserId == UserId))
                {
                    var collects = await _dbContext.Queryable<collect>().Where(c => c.Id > 14).Select(c => c.Id).ToListAsync();
                    if (collects?.Count > 0)
                    {
                        // 实例化Random类，用于生成随机数
                        Random random = new Random();
                        // 生成一个介于0和numbers.Count - 1之间的随机索引
                        int randomIndex = random.Next(collects.Count);
                        // 通过随机索引从List中选取元素
                        long id = collects[randomIndex];
                        usercollect usercollect = new()
                        {
                            UserId = UserId,
                            CollectId = id,//临时数据
                            Ispermanent = 1,
                            IsWear = 0,
                            IsFixed = 1,
                            ObtainingDate = _dbContext.GetDate()
                        };
                        var collect = await _dbContext.Queryable<collect>().Where(c => c.Id == 1).FirstAsync();
                        //添加待通知职级信息
                        CollectModel collectModel = new()
                        {
                            Id = id,
                            Name = collect.Name,
                            Picture = collect.Picture,
                            FullBodyPhoto = collect.FullBodyPhoto
                        };
                        tobenotified tobenotified = new()
                        {
                            UserId = UserId,
                            State = 1,
                            Type = 1,
                            IsRead = 0,
                            Data = JsonConvert.SerializeObject(collectModel),
                            CreateDate = _dbContext.GetDate()
                        };
                        //开启事务
                        await _dbContext.TransactionAsync(async () =>
                        {
                            var res = await _dbContext.Insertable(usercollect).ExecuteAsync();
                            if (res.Code != ResponseCode.Success)
                            {
                                return;
                            }
                            res = await _dbContext.Insertable(tobenotified).ExecuteAsync();
                            if (res.Code != ResponseCode.Success)
                            {
                                return;
                            }
                        });
                    }
                }

                //修改当天邀请记录为已邀请
                var res = await _dbContext.Updateable<invitationrecord>().SetColumns(c => c.IsUse == 1).Where(c => result.Take(3).Select(a => a.Id).Contains(c.Id)).ExecuteAsync();
                if (res.Code != ResponseCode.Success)
                {
                    return res;
                }
                //插入道具奖励记录
                if (await _dbContext.Queryable<userprop>().AnyAsync(c => c.UserId == UserId))
                {
                    res = await _dbContext.Updateable<userprop>()
                                                               .SetColumns(c => c.Prop1 == c.Prop1 + 1)
                                                               .SetColumns(c => c.Prop2 == c.Prop2 + 1)
                                                               .SetColumns(c => c.Prop3 == c.Prop3 + 1)
                                                               .Where(c => c.UserId == UserId)
                                                               .ExecuteAsync();
                }
                else
                {
                    userprop userprop = new userprop
                    {
                        UserId = UserId,
                        Prop1 = 1,
                        Prop2 = 2,
                        Prop3 = 3
                    };
                    res = await _dbContext.Insertable(userprop).ExecuteAsync();
                }
                if (res.Code != ResponseCode.Success)
                {
                    return res;
                }
            }
            return response;
        }

        /// <summary>
        /// 查询职级排行
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseModel> GetPositionLevelRanking()
        {
            ResponseModel response = new();
            var result = await _dbContext.Queryable<positionlevel>()
                .Where(c => c.Status == (byte)BaseEnums.IsValid.Valid)
                .Select(c => new PositionLevelRankingModel
                {
                    Id = c.Id,
                    Name = c.Name,
                })
                .OrderByDescending(c => c.Id)
                .ToListAsync();
            //查询玩家
            var users = await _dbContext.Queryable<user>().ToListAsync();
            //查询玩家已穿戴的装扮
            var ucs = await _dbContext.Queryable<usercollect>()
                                                      .OrderBy(u => u.IsWear == 1)
                                                      .LeftJoin<collect>((u, c) => u.CollectId == c.Id)
                                                      .Select((u, c) => new
                                                      {
                                                          UserId = u.UserId,
                                                          Picture = SqlFunc.Subqueryable<user>().Where(a => a.Id == u.UserId).Select(a => a.Picture),
                                                          CollectId = u.CollectId,
                                                          CollectPicture = c.Picture,
                                                          FullBodyPhoto = c.FullBodyPhoto
                                                      })
                                                      .ToListAsync();
            var list = ucs.GroupBy(c => c.UserId)
                .Select(c => new PositionLevelUserModel
                {
                    UserId = c.Select(a => a.UserId).FirstOrDefault(),
                    Picture = c.Select(a => a.Picture).FirstOrDefault(),
                    CollectId = c.OrderByDescending(a => a.CollectId).Select(a => a.CollectId).FirstOrDefault(),
                    CollectPicture = c.OrderByDescending(a => a.CollectId).Select(a => a.CollectPicture).FirstOrDefault(),
                    FullBodyPhoto = c.OrderByDescending(a => a.CollectId).Select(a => a.FullBodyPhoto).FirstOrDefault(),
                }).ToList();
            foreach (var item in result)
            {
                item.TotalNumber = users?.Count(c => c.PositionLevelId == item.Id);
                if (ucs?.Count > 0)
                {
                    var ids = users.Where(c => c.PositionLevelId == item.Id).OrderBy(a => a.InviterId != null).Select(a => a.Id).ToList();
                    List<PositionLevelUserModel> lists = new();
                    foreach (var id in ids)
                    {
                        lists.Add(list.Where(c => id == c.UserId && c.CollectId == item.Id)
                               .Select(c => new PositionLevelUserModel
                               {
                                   UserId = c.UserId,
                                   Picture = c.Picture,
                                   CollectId = c.CollectId,
                                   CollectPicture = c.CollectPicture,
                                   FullBodyPhoto = c.FullBodyPhoto
                               }).FirstOrDefault());
                        if (lists.Count >= 10)
                        {
                            break;
                        }
                    }
                    item.List = lists;
                }
            }
            response.Data = result;
            return response;
        }

        /// <summary>
        /// 消息通知
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseModel> GetToBenotifiedList()
        {
            ResponseModel response = new();
            var result = await _dbContext.Queryable<tobenotified>()
                .Where(c => c.UserId == UserId && c.IsRead == 0)
                .Select(c => new ToBenotifiedModel
                {
                    Id = c.Id,
                    State = (byte)c.State,
                    Type = (byte)c.Type,
                    Data = c.Data
                })
                .OrderBy(c => c.Id)
                .ToListAsync();
            response.Data = result;
            return response;
        }

        /// <summary>
        /// 修改消息通知为已读
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseModel> EditToBenotified(EditToBenotifiedModel model)
        {
            return await _dbContext.Updateable<tobenotified>()
                .Where(c => c.Id == model.Id)
                .SetColumns(c => c.IsRead == 1).ExecuteAsync();
        }

        /// <summary>
        /// 新增消息通知
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddToBenotified(AddToBenotifiedModel model)
        {
            ResponseModel result = new();
            //开启事务
            return await _dbContext.TransactionAsync(async () =>
            {
                if (model.State == 0)
                {
                    if (model.Type == 0)
                    {

                    }
                    else if (model.Type == 1)
                    {

                    }
                    else if (model.Type == 2)
                    {

                    }
                    else
                    {
                        throw new UserOperationException("类型错误!");
                    }
                }
                else
                {
                    if (model.Type == 0)
                    {

                    }
                    else if (model.Type == 1)
                    {

                    }
                    else if (model.Type == 2)
                    {
                        if (!await _dbContext.Queryable<userhonor>().AnyAsync(c => c.HonorId == model.Id && c.UserId == UserId))
                        {
                            var honor = await _dbContext.Queryable<honor>().Where(c => c.Id == model.Id).FirstAsync();
                            HonorModel honorModel = new()
                            {
                                Id = honor.Id,
                                Name = honor.Name,
                                Picture = honor.Picture,
                            };
                            result = await AddToBeNotified(UserId, 2, 1, honorModel);
                            if (result?.Code != ResponseCode.Success)
                            {
                                throw new UserOperationException("荣誉待通知消息报错!");
                            }
                            userhonor userhonor = new()
                            {
                                UserId = UserId,
                                HonorId = model.Id,
                                IsWear = 0,
                                ObtainingDate = DateTime.Now,
                            };
                            result = await _dbContext.Insertable(userhonor).ExecuteAsync();
                            if (result?.Code != ResponseCode.Success)
                            {
                                throw new UserOperationException("新增荣誉报错!");
                            }
                        }
                    }
                    else
                    {
                        throw new UserOperationException("类型错误!");
                    }
                }
            });
        }

        /// <summary>
        /// 获取玩家道具
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseModel> GetUserProp()
        {
            ResponseModel response = new();
            var result = await _dbContext.Queryable<userprop>()
                .Where(c => c.UserId == UserId)
                .Select(c => new UserPropModel
                {
                    Prop1 = c.Prop1 == null ? 0 : (int)c.Prop1,
                    Prop2 = c.Prop2 == null ? 0 : (int)c.Prop2,
                    Prop3 = c.Prop3 == null ? 0 : (int)c.Prop3
                })
                .ToListAsync();
            await _dbContext.Deleteable<userprop>().Where(c => c.UserId == UserId).ExecuteCommandAsync();
            response.Data = result;
            return response;
        }

        #region 内部方法
        /// <summary>
        /// 新增待通知信息
        /// </summary>
        /// <param name="Id">玩家</param>
        /// <param name="type">类型 0职级 1装扮 2荣誉</param>
        /// <param name="state">0 失去 1获得</param>
        /// <param name="model">数据</param>
        /// <returns></returns>
        private async Task<ResponseModel> AddToBeNotified(long Id, byte type, byte state, object model)
        {
            tobenotified tobenotified = new()
            {
                UserId = Id,
                State = state,
                Type = type,
                IsRead = 0,
                Data = JsonConvert.SerializeObject(model),
                CreateDate = _dbContext.GetDate()
            };
            var result = await _dbContext.Insertable(tobenotified).ExecuteAsync();
            return result;
        }
        public class TimePeriod
        {
            public DateTime StartDate { get; set; }
            public int DurationDays { get; set; }
        }
        private static List<TimePeriod> GetContinuousPeriods(List<DateTime> dates)
        {
            List<TimePeriod> periods = new List<TimePeriod>();

            if (dates.Count == 0)
            {
                return periods;
            }

            TimePeriod currentPeriod = new TimePeriod
            {
                StartDate = dates[0],
                DurationDays = 1
            };

            for (int i = 1; i < dates.Count; i++)
            {
                if (dates[i].Date == dates[i - 1].Date.AddDays(1))
                {
                    // 当前日期与前一个日期连续，增加持续天数  
                    currentPeriod.DurationDays++;
                }
                else
                {
                    // 当前日期与前一个日期不连续，保存当前时间段并开始新的时间段  
                    periods.Add(currentPeriod);
                    currentPeriod = new TimePeriod
                    {
                        StartDate = dates[i],
                        DurationDays = 1
                    };
                }
            }

            // 添加最后一个时间段  
            periods.Add(currentPeriod);

            return periods;
        }
        #endregion
    }
}
