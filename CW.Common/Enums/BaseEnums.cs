using System;
using System.Collections.Generic;
using System.Text;

namespace CW.Common.Enums
{
    public class BaseEnums
    {
        /// <summary>
        /// 是否有效 1有效 0无效
        /// </summary>
        public enum IsValid { Valid = 1, InValid = 0 };
        /// <summary>
        /// 是否通过 1通过 2不通过
        /// </summary>
        public enum IsApprove { Approved = 1, NotApprove };
        /// <summary>
        /// 是否可操作 1可操作 2不可操作
        /// </summary>
        public enum IsOperation { Operation = 1, NonOperation }
        /// <summary>
        /// 是否必须 1必须 2不必须
        /// </summary>
        public enum IsMust { Must = 1, NotMust }
        /// <summary>
        /// 是否为空(用于查询IS操作符) 1不为空 2为空
        /// </summary>
        public enum IsNull { NotNull = 1, Null }
        /// <summary>
        /// 是否显示 1是 2否
        /// </summary>
        public enum IsView { View = 1, UnView = 2 }
        /// <summary>
        /// 是否默认 1是 2否
        /// </summary>
        public enum IsDefault { Default = 1, UnDefault = 2 }
        /// <summary>
        /// 是否包含上级 1包含 2不包含
        /// </summary>
        public enum IsIncludeSuperior { Include = 1, Exclude }
        /// <summary>
        /// 操作类型 1新增 2编辑 3删除
        /// </summary>
        public enum OperationType { Add = 1, Edit = 2, Del = 3 }
        /// <summary>
        /// 是否正确TOF 1true 2false
        /// </summary>
        public enum TrueOrFalse { True = 1, False = 2 }
        /// <summary>
        /// 是否审核 1是 2否
        /// </summary>
        public enum IsCheck { Check = 1, NoCheck = 2 }
        /// <summary>
        /// 是否正确 1是 2否
        /// </summary>
        public enum IsCorrect { Correct = 1, InCorrect = 2 }
        /// <summary>
        /// 是否勾选 1是 2否
        /// </summary>
        public enum IsSelect { Select = 1, UnSelect = 2 }
        /// <summary>
        /// 是否确认 1是 2否
        /// </summary>
        public enum IsConfirm { Confirmed = 1, Unconfirmed = 2 }
        /// <summary>
        /// 是否成功 1是 2否
        /// </summary>
        public enum IsSuccess
        {
            /// <summary>
            /// 成功
            /// </summary>
            Success = 1,
            /// <summary>
            /// 失败
            /// </summary>
            Fail = 2
        }
        /// <summary>
        /// 是否实名 1是 2否 3未知
        /// </summary>
        public enum IsRealName
        {
            /// <summary>
            /// 实名
            /// </summary>
            RealName = 1,
            /// <summary>
            /// 未实名
            /// </summary>
            NoRealName = 2,
            /// <summary>
            /// 未知
            /// </summary>
            Unknown = 3
        }
    }
}
