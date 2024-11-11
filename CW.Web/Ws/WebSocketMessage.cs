using System.Net.WebSockets;

namespace CW.Web.Ws
{
    public class WebSocketMessage
    {
        public string from { get; set; }
  
        public string to { get; set; }

        public int groupId { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public int action { get; set; }

        public string message { get; set; }
    }

    public class MessageReq
    {
        //项目Id
        public string projectId { get; set; }
        //消息Id
        public string messageId { get; set; }
        //json对象
        public string data { get; set; }
        //用户id
        public string uid { get; set; }
        //用户名
        public string name { get; set; }
    }

    public class MessageRes
    {
        //错误码
        public int code { get; set; }
        //错误内容
        public string msg { get; set; }
        //项目Id
        public string projectId { get; set; }
        //消息Id
        public string messageId { get; set; }
        //用户id
        public string uid { get; set; }
        //json对象
        public string data { get; set; }
    }

    public enum ErrorCode
    {
        Success = 0,
        Error = 1,
    }
}
