
using System;
using System.Linq;
using System.Text;
using SqlSugar;


namespace CW.Framework.Entities
{
        ///<summary>
    ///玩家记录
    ///</summary>
    [SugarTable("loginrecord")]
    [Serializable]
    public partial class loginrecord:BaseEntity
    {
        public loginrecord()
        {

        }
           /// <summary>
           /// Desc:记录Id
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
           /// Desc:游玩时间
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public DateTime PlayDate{get;set;}
           /// <summary>
           /// Desc:json数据
           /// Default:
           /// Nullable:True
           /// </summary>
        
        public string Data{get;set;}
           /// <summary>
           /// Desc:是否上班成功 0未成功 1成功
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public byte IsSuccess{get;set;}
           /// <summary>
           /// Desc:开始时间
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public DateTime SatrtDate{get;set;}
           /// <summary>
           /// Desc:是否加班 0否 1是
           /// Default:
           /// Nullable:True
           /// </summary>
        
        public byte? IsWorkOvertime{get;set;}
           /// <summary>
           /// Desc:是否加班上班成功 0未成功 1成功
           /// Default:
           /// Nullable:True
           /// </summary>
        
        public byte? IsWorkOvertimeSuccess{get;set;}
    }
}
