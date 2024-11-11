using System.Net.WebSockets;

namespace CW.Web.Ws
{
    public class WebSocketClientCollection
    {
        private static List<WebSocketClient> _client = new List<WebSocketClient>();

        /// <summary>
        /// 添加客户端
        /// </summary>
        /// <param name="client"></param>
        public static void AddClient(WebSocketClient client)
        {
            var clientNow = _client.Where(c => c.uid == client.uid).FirstOrDefault();
            if (clientNow != null)
                _client.Remove(client);
            _client.Add(client);
        }

        /// <summary>
        /// 移除客户端
        /// </summary>
        /// <param name="clientId"></param>
        public static void RemoveClient(string clientId)
        {
            var clientNow = _client.Where(c => c.uid == clientId).FirstOrDefault();
            if (clientNow != null)
                _client.Remove(clientNow);
        }

        /// <summary>
        /// 获取客户端
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public static WebSocketClient GetClient(string clientId)
        {
            if (_client.Any(c => c.uid == clientId))
            {
                return _client.First(c => c.uid == clientId);
            }
            return null;
        }

        /// <summary>
        /// 获取全部客户端
        /// </summary>
        /// <returns></returns>
        public static List<WebSocketClient> GetAllClient()
        {
            return _client;
        }
    }

    public class WebSocketClient
    {
        public string uid { get; set; }
        /// <summary>
        /// 当时链接的socket
        /// </summary>
        public WebSocket webSocket { get; set; }
    }
}