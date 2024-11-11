
using System;
using System.Linq;
using System.Text;
using SqlSugar;


namespace CW.Framework.Entities
{
        ///<summary>
    ///用户表
    ///</summary>
    [SugarTable("user")]
    [Serializable]
    public partial class user:BaseEntity
    {
        public user()
        {
            this.CreateDate =DateTime.Now;

        }
           /// <summary>
           /// Desc:用户Id
           /// Default:
           /// Nullable:False
           /// </summary>
        
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
        public long Id{get;set;}
           /// <summary>
           /// Desc:微信openId
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public string OpenId{get;set;}
           /// <summary>
           /// Desc:省份Id
           /// Default:
           /// Nullable:True
           /// </summary>
        
        public long? ProvinceId{get;set;}
           /// <summary>
           /// Desc:职级Id
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public long PositionLevelId{get;set;}
           /// <summary>
           /// Desc:名称
           /// Default:
           /// Nullable:True
           /// </summary>
        
        public string Name{get;set;}
           /// <summary>
           /// Desc:手机号
           /// Default:
           /// Nullable:True
           /// </summary>
        
        public string Tel{get;set;}
           /// <summary>
           /// Desc:头像
           /// Default:
           /// Nullable:True
           /// </summary>
        
        public string Picture{get;set;}
           /// <summary>
           /// Desc:上班天数
           /// Default:
           /// Nullable:True
           /// </summary>
        
        public int? WorkingDays{get;set;}
           /// <summary>
           /// Desc:创建时间 默认为当前时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:True
           /// </summary>
        
        public DateTime? CreateDate{get;set;}
           /// <summary>
           /// Desc:上线时间
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public DateTime GoOnlineDate{get;set;}
           /// <summary>
           /// Desc:邀请人UserId
           /// Default:
           /// Nullable:True
           /// </summary>
        
        public long? InviterId{get;set;}
    }
}
