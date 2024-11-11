
using System;
using System.Linq;
using System.Text;
using SqlSugar;


namespace CW.Framework.Entities
{
        ///<summary>
    ///玩家道具
    ///</summary>
    [SugarTable("userprop")]
    [Serializable]
    public partial class userprop:BaseEntity
    {
        public userprop()
        {

        }
           /// <summary>
           /// Desc:
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
           /// Desc:道具1
           /// Default:
           /// Nullable:True
           /// </summary>
        
        public int? Prop1{get;set;}
           /// <summary>
           /// Desc:道具2
           /// Default:
           /// Nullable:True
           /// </summary>
        
        public int? Prop2{get;set;}
           /// <summary>
           /// Desc:道具3
           /// Default:
           /// Nullable:True
           /// </summary>
        
        public int? Prop3{get;set;}
    }
}
