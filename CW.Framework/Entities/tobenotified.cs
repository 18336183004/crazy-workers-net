
using System;
using System.Linq;
using System.Text;
using SqlSugar;


namespace CW.Framework.Entities
{
        ///<summary>
    ///待通知信息
    ///</summary>
    [SugarTable("tobenotified")]
    [Serializable]
    public partial class tobenotified:BaseEntity
    {
        public tobenotified()
        {
            this.CreateDate =DateTime.Now;

        }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>
        
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
        public long Id{get;set;}
           /// <summary>
           /// Desc:玩家id
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public long UserId{get;set;}
           /// <summary>
           /// Desc:状态 0失去 1获得
           /// Default:
           /// Nullable:True
           /// </summary>
        
        public byte? State{get;set;}
           /// <summary>
           /// Desc:类型 0职级 1装扮 2荣誉
           /// Default:
           /// Nullable:True
           /// </summary>
        
        public byte? Type{get;set;}
           /// <summary>
           /// Desc:是否已读 0未读 1已读
           /// Default:
           /// Nullable:True
           /// </summary>
        
        public byte? IsRead{get;set;}
           /// <summary>
           /// Desc:数据
           /// Default:
           /// Nullable:True
           /// </summary>
        
        public string Data{get;set;}
           /// <summary>
           /// Desc:创建时间
           /// Default:CURRENT_TIMESTAMP
           /// Nullable:False
           /// </summary>
        
        public DateTime CreateDate{get;set;}
    }
}
