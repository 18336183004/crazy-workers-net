
using System;
using System.Linq;
using System.Text;
using SqlSugar;


namespace CW.Framework.Entities
{
        ///<summary>
    ///收藏设置
    ///</summary>
    [SugarTable("collect")]
    [Serializable]
    public partial class collect:BaseEntity
    {
        public collect()
        {
            this.Status =Convert.ToByte("0");

        }
           /// <summary>
           /// Desc:收藏Id
           /// Default:
           /// Nullable:False
           /// </summary>
        
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
        public long Id{get;set;}
           /// <summary>
           /// Desc:职级Id
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public long PositionLevelId{get;set;}
           /// <summary>
           /// Desc:收藏名称
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public string Name{get;set;}
           /// <summary>
           /// Desc:图片
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public string Picture{get;set;}
           /// <summary>
           /// Desc:全身照
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public string FullBodyPhoto{get;set;}
           /// <summary>
           /// Desc:收藏描述
           /// Default:
           /// Nullable:True
           /// </summary>
        
        public string Description{get;set;}
           /// <summary>
           /// Desc:排序
           /// Default:
           /// Nullable:True
           /// </summary>
        
        public int? Sort{get;set;}
           /// <summary>
           /// Desc:状态 1上架 0下架
           /// Default:0
           /// Nullable:False
           /// </summary>
        
        public byte Status{get;set;}
    }
}
