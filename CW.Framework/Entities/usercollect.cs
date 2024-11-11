
using System;
using System.Linq;
using System.Text;
using SqlSugar;


namespace CW.Framework.Entities
{
        ///<summary>
    ///玩家收藏
    ///</summary>
    [SugarTable("usercollect")]
    [Serializable]
    public partial class usercollect:BaseEntity
    {
        public usercollect()
        {

        }
           /// <summary>
           /// Desc:玩家收藏Id
           /// Default:
           /// Nullable:False
           /// </summary>
        
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
        public long Id{get;set;}
           /// <summary>
           /// Desc:玩家Id
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public long UserId{get;set;}
           /// <summary>
           /// Desc:收藏Id
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public long CollectId{get;set;}
           /// <summary>
           /// Desc:是否永久 0否 1是
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public byte Ispermanent{get;set;}
           /// <summary>
           /// Desc:是否穿戴 0未穿戴 1穿戴
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public byte IsWear{get;set;}
           /// <summary>
           /// Desc:获得时间
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public DateTime ObtainingDate{get;set;}
           /// <summary>
           /// Desc:是否固定 0否 1是
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public byte IsFixed{get;set;}
    }
}
