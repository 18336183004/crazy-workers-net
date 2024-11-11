
using System;
using System.Linq;
using System.Text;
using SqlSugar;


namespace CW.Framework.Entities
{
        ///<summary>
    ///邀请记录
    ///</summary>
    [SugarTable("invitationrecord")]
    [Serializable]
    public partial class invitationrecord:BaseEntity
    {
        public invitationrecord()
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
           /// Desc:邀请Id
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public long InvitationId{get;set;}
           /// <summary>
           /// Desc:邀请时间
           /// Default:
           /// Nullable:False
           /// </summary>
        
        public DateTime InvitationDate{get;set;}
           /// <summary>
           /// Desc:是否使用 0未使用 1使用
           /// Default:
           /// Nullable:True
           /// </summary>
        
        public byte? IsUse{get;set;}
    }
}
