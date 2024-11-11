using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using CW.Api.IServices;
using CW.Api.Models;
using CW.Api.Services.BASE;
using CW.Common.Attributes;
using CW.Common.BASE;
using CW.Common.Enums;
using CW.Common.JwtAuth;
using CW.Framework.Entities;
using CW.Framework.Extensions;
using Newtonsoft.Json;
using SqlSugar;

namespace CW.Api.Services
{
    public class LoginService : BaseService, ILoginService
    {
        public LoginService() : base()
        {

        }

        /// <summary>
        /// 获取玩家信息
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public async Task<user> GetUserinfo(string openId)
        {
            return await _dbContext.Queryable<user>().Where(c => c.OpenId == openId).FirstAsync();
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="UserOperationException"></exception>
        public async Task<ResponseModel> Login(LoginModel model, user user)
        {
            ResponseModel result = new();
            int day = 0;
            int positionLevel = (int)user.PositionLevelId;
            //开启事务
            return await _dbContext.TransactionAsync(async () =>
            {
                //比较当前日期的日期是否等于数据库玩家储存的日期，用来修改玩家登录时间
                if (user.GoOnlineDate.Date != DateTime.Today)
                {
                    //查询用户几天没有进行登录，对超过一天没有登录的玩家进行职级，装扮，荣誉的添加和删除
                    day = (DateTime.Now.Date - user.GoOnlineDate.Date).Days - 1;
                    if (day >= 1)
                    {
                        //修改职级为最新的职级
                        positionLevel = (int)user.PositionLevelId - (int)day <= 1 ? 1 : (int)user.PositionLevelId - (int)day;
                        //查询当前职级职级
                        var positionLevelInfo = await GetPositionLevelName(positionLevel);
                        if (positionLevelInfo == null)
                        {
                            throw new UserOperationException("职级查询为空!");
                        }
                        //新职级和查询出的用户职级不一致时，添加职级通知消息记录
                        if (positionLevel != (int)user.PositionLevelId)
                        {
                            //添加待通知职级信息
                            PositionLevelModel positionLevelModel = new()
                            {
                                Id = positionLevel,
                                Name = positionLevelInfo.Name,
                                Picture = positionLevelInfo.Picture,
                            };
                            //添加记录
                            result = await AddToBeNotified(user.Id, 0, 0, positionLevelModel);
                            if (result?.Code != ResponseCode.Success)
                            {
                                throw new UserOperationException("职级待通知消息报错!");
                            }
                        }
                        //查询可删除的未永久获取的对应职级的装扮
                        var list = await _dbContext.Queryable<usercollect>()
                                                                    .LeftJoin<collect>((u, c) => u.CollectId == c.Id)
                                                                    .Where((u, c) => u.UserId == user.Id)
                                                                    .Select((u, c) => new
                                                                    {
                                                                        UserCollectId = u.Id,
                                                                        UserId = u.Id,
                                                                        IsPermanent = u.Ispermanent,
                                                                        c.Id,
                                                                        c.PositionLevelId,
                                                                        c.Name,
                                                                        c.Picture,
                                                                        c.FullBodyPhoto
                                                                    })
                                                                    .ToListAsync();
                        //查询需要删除的装扮
                        var ids = list?.Where(c => c.IsPermanent == 0 && c.PositionLevelId > positionLevel)
                                                                    .Select(c => c.UserCollectId)
                                                                    .ToList();
                        //装扮处理
                        if (ids?.Count > 0)
                        {
                            List<tobenotified> tobenotifieds = new List<tobenotified>();
                            foreach (var item in ids)
                            {
                                var info = list?.Where(c => c.UserCollectId == item).FirstOrDefault();
                                if (info != null)
                                {
                                    tobenotified tobenotified = new()
                                    {
                                        UserId = user.Id,
                                        State = 1,
                                        Type = 0,
                                        IsRead = 0,
                                        CreateDate = _dbContext.GetDate()
                                    };
                                    //添加待通知职级信息
                                    CollectModel collectModel = new()
                                    {
                                        Id = info.Id,
                                        Name = info.Name,
                                        Picture = info.Picture,
                                        FullBodyPhoto = info.FullBodyPhoto
                                    };
                                    tobenotified.Data = JsonConvert.SerializeObject(collectModel);
                                    tobenotifieds.Add(tobenotified);
                                }
                            }
                            if (tobenotifieds?.Count > 0)
                            {
                                var result = await _dbContext.Insertable(tobenotifieds).ExecuteAsync();
                                if (result?.Code != ResponseCode.Success)
                                {
                                    throw new UserOperationException("装扮待通知消息报错!");
                                }
                            }
                            //删除超过当前职级的装扮
                            var id = await _dbContext.Deleteable<usercollect>().Where(c => ids.Contains(c.Id)).ExecuteCommandAsync();
                            if (id <= 0)
                            {
                                throw new UserOperationException("删除超过当前职级的装扮报错!");
                            }
                            //查询删除之后最高的职级装扮
                            var userCollectId = list?.Where(c => c.PositionLevelId <= positionLevel)
                                                                  .OrderByDescending(c => c.PositionLevelId)
                                                                  .Select(c => c.UserCollectId)
                                                                  .FirstOrDefault();
                            if (userCollectId != null)
                            {
                                //穿戴的装扮改为穿戴
                                result = await _dbContext.Updateable<usercollect>()
                                                                                .SetColumns(c => new usercollect
                                                                                {
                                                                                    IsWear = 1
                                                                                }).Where(c => c.Id == userCollectId)
                                                                                .ExecuteAsync();
                            }
                            if (result?.Code != ResponseCode.Success)
                            {
                                throw new UserOperationException("修改穿戴装扮报错!");
                            }
                        }

                        List<long> honorIds = new List<long> { 304, 305, 306, 307 };
                        //查询全部未获得荣誉
                        var UserHonorList = await _dbContext.Queryable<userhonor>()
                                                                    .Where(u => u.UserId == user.Id && honorIds.Contains(u.HonorId))
                                                                    .ToListAsync();
                        var honorList = await _dbContext.Queryable<honor>()
                                                                  .Where(c => honorIds.Contains(c.Id))
                                                                  .ToListAsync();
                        List<long> NewHonorIds = new();

                        if (day >= 1 && day < 3)
                        {
                            NewHonorIds.Add(304);
                        }
                        else if (day >= 3 && day < 7)
                        {
                            if (UserHonorList.Any(c => c.HonorId != 304))
                            {
                                NewHonorIds.Add(304);
                                NewHonorIds.Add(305);
                            }
                            else
                            {
                                NewHonorIds.Add(305);
                            }
                        }
                        else if (day >= 7 && day < 14)
                        {
                            if (UserHonorList.Any(c => c.HonorId != 305))
                            {
                                NewHonorIds.Add(304);
                                NewHonorIds.Add(305);
                                NewHonorIds.Add(306);
                            }
                            else
                            {
                                NewHonorIds.Add(306);
                            }
                        }
                        else if (day >= 14)
                        {
                            if (UserHonorList.Any(c => c.HonorId != 306))
                            {
                                NewHonorIds.Add(304);
                                NewHonorIds.Add(305);
                                NewHonorIds.Add(306);
                                NewHonorIds.Add(307);
                            }
                            else
                            {
                                NewHonorIds.Add(307);
                            }
                        }

                        if (NewHonorIds?.Count > 0)
                        {
                            List<tobenotified> tobenotifieds = new List<tobenotified>();
                            List<HonorModel> honors = new List<HonorModel>();
                            foreach (var item in NewHonorIds)
                            {
                                if (!UserHonorList.Any(c => c.HonorId == item))
                                {
                                    var info1 = honorList?.Where(c => c.Id == item).FirstOrDefault();
                                    if (info1 != null)
                                    {
                                        tobenotified tobenotified = new()
                                        {
                                            UserId = user.Id,
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
                            }

                            if (honors?.Count > 0)
                            {
                                result = await _dbContext.Insertable(tobenotifieds).ExecuteAsync();
                                if (result?.Code != ResponseCode.Success)
                                {
                                    throw new UserOperationException("荣誉获取报错!");
                                }
                                List<userhonor> userhonorList = new List<userhonor>();
                                foreach (var honor in honors)
                                {
                                    userhonor userhonor = new()
                                    {
                                        UserId = user.Id,
                                        HonorId = (long)honor.Id,
                                        IsWear = 0,
                                        ObtainingDate = DateTime.Now,
                                    };
                                    userhonorList.Add(userhonor);
                                }
                                if (userhonorList?.Count > 0)
                                {
                                    var result = await _dbContext.Insertable(userhonorList).ExecuteAsync();
                                    if (result?.Code != ResponseCode.Success)
                                    {
                                        throw new UserOperationException("修改穿戴装扮报错!");
                                    }
                                }
                            }
                        }
                    }
                    //修改用户登录时间
                    result = await EditAccount(model.OpenId, positionLevel);
                    if (result?.Code != ResponseCode.Success)
                    {
                        throw new UserOperationException("修改用户信息报错!");
                    }
                }
                result.Data = day;
                return result;
            });
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseModel> LoginAccount(LoginModel model)
        {
            //创建新用户
            user user = new()
            {
                OpenId = model.OpenId,
                PositionLevelId = 1,//默认实习生
                WorkingDays = 0,
                CreateDate = _dbContext.GetDate(),
                GoOnlineDate = _dbContext.GetDate(),
            };

            if (!string.IsNullOrEmpty(model.InviterId))
            {
                user.InviterId = await _dbContext.Queryable<user>().Where(c => c.OpenId == model.InviterId).Select(c => c.Id).FirstAsync();
            }
            return await _dbContext.TransactionAsync(async () =>
            {
                ResponseModel result = new();
                var id = await _dbContext.Insertable(user).ExecuteReturnIdentityAsync();
                if (id > 0)
                {
                    if (!string.IsNullOrEmpty(model.InviterId) && !await _dbContext.Queryable<invitationrecord>().AnyAsync(c => c.UserId == id && c.InvitationId == user.InviterId && c.InvitationDate == _dbContext.GetDate().Date))
                    {
                        invitationrecord invitationrecord = new()
                        {
                            UserId = (long)user.InviterId,
                            InvitationId = id,
                            InvitationDate = _dbContext.GetDate().Date,
                            IsUse = 0
                        };
                        result = await _dbContext.Insertable(invitationrecord).ExecuteAsync();
                    }
                    //新玩家添加默认装扮
                    if (result.Code == ResponseCode.Success)
                    {
                        usercollect usercollect = new()
                        {
                            UserId = id,
                            CollectId = 1,
                            Ispermanent = 0,
                            IsWear = 1,//默认为穿戴
                            IsFixed = 0,
                            ObtainingDate = DateTime.Now,
                        };
                        result = await _dbContext.Insertable(usercollect).ExecuteAsync();
                    }
                }
                else
                {
                    result.Code = ResponseCode.Error;
                }
                return result;
            });
        }

        //修改用户登录时间
        public async Task<ResponseModel> EditAccount(string openId, int bay)
        {
            var result = await _dbContext.Updateable<user>()
                .SetColumns(c => new user()
                {
                    PositionLevelId = bay == 0 ? 1 : bay,
                    GoOnlineDate = SqlFunc.GetDate()
                }).Where(c => c.OpenId == openId)
                .ExecuteAsync();
            return result;
        }

        /// <summary>
        /// 地图
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseModel> Map()
        {
            ResponseModel response = new ResponseModel();

            var data = await _dbContext.Queryable<map>().Where(c => c.Status == (byte)BaseEnums.IsValid.Valid).ToListAsync();
            if (data?.Count > 0)
            {
                // 实例化Random类，用于生成随机数
                Random random = new();

                List<map> list = new();

                var list1 = data.Where(c => c.DifficultyLevel == 1).ToList();
                list.Add(list1[random.Next(list1.Count)]);

                var list2 = data.Where(c => c.DifficultyLevel == 2).ToList();
                list.Add(list2[random.Next(list2.Count)]);

                var list3 = data.Where(c => c.DifficultyLevel == 3).ToList();
                list.Add(list3[random.Next(list3.Count)]);

                var list4 = data.Where(c => c.DifficultyLevel == 4).ToList();
                list.Add(list4[random.Next(list4.Count)]);

                var list5 = data.Where(c => c.DifficultyLevel == 5).ToList();
                list.Add(list5[random.Next(list5.Count)]);

                //var list6 = data.Where(c => c.DifficultyLevel == 6).ToList();
                //list.Add(list6[random.Next(list6.Count)]);

                if (list?.Count > 0)
                {
                    DateModel info = new();
                    info.MapDataBusTypes = $"[{string.Join(",", list.Select(c => c.MapDataBusTypes))}]";
                    info.MapData = $"[{string.Join(",", list.Select(c => c.MapData)).Replace(" ", "")}]";
                    info.MapDataRoloQueues = $"[{string.Join(",", list.Select(c => c.MapDataRoloQueues))}]";
                    info.MapDataGraySitDownColor = $"[{string.Join(",", list.Select(c => c.MapDataGraySitDownColor))}]";
                    info.MapDataCountDown = $"[{string.Join(",", list.Select(c => c.MapDataCountDown))}]";
                    response.Data = info;
                }
            }
            return response;
        }

        /// <summary>
        /// 查询职级名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PositionLevelModel> GetPositionLevelName(int id)
        {
            return await _dbContext.Queryable<positionlevel>().Where(c => c.Id == id && c.Status == (byte)BaseEnums.IsValid.Valid).Select(c => new PositionLevelModel
            {
                Id = c.Id,
                Name = c.Name,
                Picture = c.Picture,
            }).FirstAsync();
        }

        /// <summary>
        /// 新增待通知信息
        /// </summary>
        /// <param name="Id">玩家</param>
        /// <param name="type">类型  0职级 1装扮 2荣誉</param>
        /// <param name="state">0 失去 1获得</param>
        /// <param name="model">数据</param>
        /// <returns></returns>
        public async Task<ResponseModel> AddToBeNotified(long Id, byte type, byte state, object model)
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

        /// <summary>
        /// 每日总数记录
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseModel> DailyRecord()
        {
            ResponseModel response = new();
            DateTime now = DateTime.Today;
            DailyRecordModel model = new DailyRecordModel
            {
                Total = await _dbContext.Queryable<loginrecord>().CountAsync(c => c.PlayDate == now),
                CompletedQuantity = await _dbContext.Queryable<loginrecord>().CountAsync(c => c.PlayDate == now && c.IsSuccess == 1)
            };
            response.Data = model;
            return response;
        }

    }
}
