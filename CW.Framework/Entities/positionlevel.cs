
using System;
using System.Linq;
using System.Text;
using SqlSugar;


namespace CW.Framework.Entities
{
        ///<summary>
    ///职位级别
    ///</summary>
    [SugarTable("positionlevel")]
    [Serializable]
    public partial class positionlevel:BaseEntity
    {
        public positionlevel()
        {
            this.Status =Convert.ToByte("0");

        }
           /// <summary>
           /// Desc:职级Id
           /// Default:
           /// Nullable:False
           /// </summary>
        
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
        public long Id{get;set;}
           /// <summary>
           /// Desc:名称
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public string Name{get;set;}
           /// <summary>
           /// Desc:排序
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public int Sort{get;set;}
           /// <summary>
           /// Desc:图片
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public string Picture{get;set;}
           /// <summary>
           /// Desc:状态 1上架 0下架
           /// Default:0
           /// Nullable:False
           /// </summary>
        
        public byte Status{get;set;}
    }
}
