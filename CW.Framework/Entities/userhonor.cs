
using System;
using System.Linq;
using System.Text;
using SqlSugar;


namespace CW.Framework.Entities
{
        ///<summary>
    ///玩家荣誉
    ///</summary>
    [SugarTable("userhonor")]
    [Serializable]
    public partial class userhonor:BaseEntity
    {
        public userhonor()
        {

        }
           /// <summary>
           /// Desc:玩家荣誉Id
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
           /// Desc:荣誉Id
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public long HonorId{get;set;}
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
    }
}
