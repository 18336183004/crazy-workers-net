
using System;
using System.Linq;
using System.Text;
using SqlSugar;


namespace CW.Framework.Entities
{
        ///<summary>
    ///荣誉设置
    ///</summary>
    [SugarTable("honor")]
    [Serializable]
    public partial class honor:BaseEntity
    {
        public honor()
        {
            this.Status =Convert.ToByte("0");

        }
           /// <summary>
           /// Desc:荣誉Id
           /// Default:
           /// Nullable:False
           /// </summary>
        
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
        public long Id{get;set;}
           /// <summary>
           /// Desc:荣誉名称
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
           /// Desc:荣誉描述
           /// Default:
           /// Nullable:True
           /// </summary>
        
        public string Description{get;set;}
           /// <summary>
           /// Desc:类型 1上班&加班成功系列 2迟到&旷工系列 3局内相关
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public byte Type{get;set;}
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
