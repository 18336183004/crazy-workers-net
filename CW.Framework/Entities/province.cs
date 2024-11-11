
using System;
using System.Linq;
using System.Text;
using SqlSugar;


namespace CW.Framework.Entities
{
        ///<summary>
    ///省份
    ///</summary>
    [SugarTable("province")]
    [Serializable]
    public partial class province:BaseEntity
    {
        public province()
        {

        }
           /// <summary>
           /// Desc:省份Id
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
    }
}
