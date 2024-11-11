using CW.Common.Logs;
using Newtonsoft.Json;
using Quartz;
using System.Net.WebSockets;
using System.Text;

namespace CW.Web.Ws
{
    /// <summary>
    /// websocket中间件
    /// </summary>
    public class WebsocketHandlerMiddleware : IMiddleware
    {
        private IScheduler scheduler;

        /// <summary>
        /// 构造方法
        /// </summary>
        public WebsocketHandlerMiddleware()
        {
            //scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
            //scheduler.Start();
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                try
                {
                    WebSocket websocket = await context.WebSockets.AcceptWebSocketAsync();
                    if (websocket != null)
                    {
                        var wsclient = new WebSocketClient
                        {
                            uid = context.Request.Path.ToString().Replace("/", ""),
                            webSocket = websocket
                        };
                        //await CreateJob(wsclient.uid);
                        await Handle(wsclient);
                    }
                }
                catch (Exception ex)
                {
                    await context.Response.WriteAsync(ex.Message);
                }
            }
            else
            {
                await next(context);
            }
        }

        /// <summary>
        /// 接受消息
        /// </summary>
        /// <param name="wsclient"></param>
        /// <returns></returns>
        private async Task Handle(WebSocketClient wsclient)
        {
            WebSocketClientCollection.AddClient(wsclient);
            //接受消息
            WebSocketReceiveResult clientData = null;
            do
            {
                var buffer = new byte[1024 * 10];
                clientData = await wsclient.webSocket.ReceiveAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), CancellationToken.None);
                if (clientData.CloseStatus.HasValue)
                {
                    //客户端断开处理
                    CloseAsync(wsclient.uid);
                    WebSocketClientCollection.RemoveClient(wsclient.uid);
                    break;
                }
                if (clientData.MessageType == WebSocketMessageType.Text)
                {
                    try
                    {
                        buffer = CutTail(buffer);
                        string data = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                        if (IsValidJson(data))
                        {
                            //WebSocket指令处理
                            MessageReq messageReq = JsonConvert.DeserializeObject<MessageReq>(data);
                            if (messageReq == null)
                            {
                                new log4net.($"Ws接收消息格式错误：{data}\n");

                                return;
                            }
                            var res = new RequstHandler().Run(messageReq);
                            if (res != null)
                            {
                                await SendMessageAsync(res);
                            }
                            else
                            {
                                new LogHelper().WriteErrorLog($"Ws发送{messageReq.messageId}消息失败\n");
                            }
                        }
                        else
                        {
                            new LogHelper().WriteErrorLog($"Ws接收消息不是json格式：{data}\n");
                        }
                    }
                    catch (Exception ex)
                    {
                        new LogHelper().WriteErrorLog($"Ws接收消息异常，{ex}");
                    }
                }
            } while (!clientData.CloseStatus.HasValue);
            WebSocketClientCollection.RemoveClient(wsclient.uid);
        }

        /// <summary>
        /// 发送文本消息
        /// </summary>
        /// <param name="to">目标uid</param>
        /// <param name="message">消息内容</param>
        /// <returns></returns>
        public async void SendMessageAsync(string to, string message)
        {
            if (to.ToLower().Equals("all"))
            {
                var receiverAll = WebSocketClientCollection.GetAllClient();
                if (receiverAll != null)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(message);
                    var buffer = new ArraySegment<byte>(bytes, 0, bytes.Length);
                    foreach (var receiver in receiverAll)
                    {
                        await receiver.webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
            }
            else
            {
                var receiver = WebSocketClientCollection.GetClient(to);
                if (receiver != null)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(message);
                    var buffer = new ArraySegment<byte>(bytes, 0, bytes.Length);
                    await receiver.webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        /// <summary>
        /// 发送JSON消息
        /// </summary>
        /// <param name="res"></param>
        /// <returns></returns>
        public async Task SendMessageAsync(MessageRes res)
        {
            var receiver = WebSocketClientCollection.GetClient(res.uid);
            if (receiver != null)
            {
                var resMsg = JsonConvert.SerializeObject(res);
                byte[] bytes = Encoding.UTF8.GetBytes(resMsg);
                var buffer = new ArraySegment<byte>(bytes, 0, bytes.Length);
                await receiver.webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        /// <summary>
        /// 客户端断线处理
        /// </summary>
        /// <param name="uid"></param>
        public async void CloseAsync(string uid)
        {
            //await DeleteJob(uid);
        }


        private async Task CreateJob(string uid)
        {
            var key = new JobKey(uid, "CapsuleToys");
            var existJob = scheduler.GetJobDetail(key).Result;
            if (existJob == null)
            {
                //创建job
                IJobDetail job = JobBuilder.Create<CapsuleToysRevenueJob>()
                    .WithIdentity(key)
                    .Build();
                job.JobDataMap.Put("uid", uid);
                // 创建触发器
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity(uid, "CapsuleToysTrigger")
                    .StartAt(DateTimeOffset.Now) // 设置开始时间
                    .WithSimpleSchedule(x => x.WithIntervalInSeconds(10).RepeatForever()) // 每1分钟执行一次
                    .Build();
                // 将作业和触发器加入调度器
                await scheduler.ScheduleJob(job, trigger);
            }
        }

        private async Task DeleteJob(string uid)
        {
            var key = new JobKey(uid, "CapsuleToys");
            await scheduler.DeleteJob(key);
        }


        private byte[] CutTail(byte[] byList)
        {
            int j = 0;
            byte[] tempb = null;
            for (int i = byList.Length - 1; i >= 0; i--)
            {
                if (byList[i] != 0x00 & j == 0)
                {
                    j = i;
                    if (tempb == null)
                    {
                        tempb = new byte[j + 1];
                    }

                    tempb[j] = byList[i];
                    j--;
                }
                else
                {
                    if (tempb != null)
                    {
                        tempb[j] = byList[i];
                        j--;
                    }
                }
            }
            return tempb;
        }

        private bool IsValidJson(string strInput)
        {
            try
            {
                var obj = JsonConvert.DeserializeObject(strInput);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}