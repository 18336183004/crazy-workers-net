
using System;
using System.Linq;
using System.Text;
using SqlSugar;


namespace CW.Framework.Entities
{
        ///<summary>
    ///账号
    ///</summary>
    [SugarTable("account")]
    [Serializable]
    public partial class account:BaseEntity
    {
        public account()
        {

        }
           /// <summary>
           /// Desc:账号Id
           /// Default:
           /// Nullable:False
           /// </summary>
        
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
        public long Id{get;set;}
           /// <summary>
           /// Desc:账号
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public string UserName{get;set;}
           /// <summary>
           /// Desc:密码
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public string PassWord{get;set;}
    }
}
