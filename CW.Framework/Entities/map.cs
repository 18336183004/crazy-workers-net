
using System;
using System.Linq;
using System.Text;
using SqlSugar;


namespace CW.Framework.Entities
{
        ///<summary>
    ///地图表
    ///</summary>
    [SugarTable("map")]
    [Serializable]
    public partial class map:BaseEntity
    {
        public map()
        {
            this.Status =Convert.ToByte("0");

        }
           /// <summary>
           /// Desc:地图Id
           /// Default:
           /// Nullable:False
           /// </summary>
        
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
        public int Id{get;set;}
           /// <summary>
           /// Desc:难度等级 1低级 2中级 3高级 4困难 5地狱
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public byte DifficultyLevel{get;set;}
           /// <summary>
           /// Desc:地图类型 1(7*11) 2(4*7) 3(4*8) 4(5*8) 5(5*11) 6(6*10) 7(6 * 12)
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public byte MapDataBusTypes{get;set;}
           /// <summary>
           /// Desc:地图数据
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public string MapData{get;set;}
           /// <summary>
           /// Desc:乘客排序颜色
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public string MapDataRoloQueues{get;set;}
           /// <summary>
           /// Desc:可坐的乘客颜色
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public string MapDataGraySitDownColor{get;set;}
           /// <summary>
           /// Desc:通关时间
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public string MapDataCountDown{get;set;}
           /// <summary>
           /// Desc:状态 1上架 0下架
           /// Default:0
           /// Nullable:False
           /// </summary>
        
        public byte Status{get;set;}
    }
}
