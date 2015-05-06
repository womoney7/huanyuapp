using App.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using WebSocket4Net;

namespace App.Untity
{
    public delegate void ReceiveMessagedDelegate(object sender, ReceiveMessagedEventArgs args);

    public delegate void OnCompletion(Exception e);

    public delegate void OnCompletion<T>(T retValue, Exception e);

    public class ReceiveMessagedEventArgs
    {
        public ReceiveMessagedEventArgs(string message)
        {
            Message = message;
        }
        public string Message { get; set; }
    }

    public interface IService
    {
        void Invoke(string cmd, OnCompletion<object> result, Action networkerrAction = null);
        void InvokeNoResult(string cmd, OnCompletion result, Action networkerrAction = null);
        void InvokeWithPackage(string cmd, byte[] data, OnCompletion<object> result, Action networkerrAction = null);
        void InvokeWithParamter(string cmd, object paramter, OnCompletion<object> result, Action networkerrAction = null);
    }


    public class WebSocketProxy : IService, IDisposable
    {
        private object _lockobj = new object();
        private List<WebSocketSession> _caches;
        private Dictionary<string, WebSocketTask> _tasks;
        //private Timer _timer;

        public WebSocketProxy()
            : this(3)
        {

        }

        public WebSocketProxy(int max_connect_num = 3)
        {
            _caches = new List<WebSocketSession>();
            _tasks = new Dictionary<string, WebSocketTask>();
            //_timer = new Timer((n) =>
            //    {
            //        //清洗空闲的通道
            //        (n as WebSocketProxy).ClearSocketSessions();

            //    }, this, 60000, 1000);
            MaxConnectNum = max_connect_num;
        }

        public WebSocketSession DefaultWebSocketSession
        {
            get
            {
                return _caches.Where(x => x.IsDefault).FirstOrDefault();
            }
        }

        private static WebSocketProxy instance;
        public static WebSocketProxy Current
        {
            get
            {
                if (instance == null)
                    instance = new WebSocketProxy();
                return instance;
            }
        }
        /// <summary>
        /// 向系统发送消息
        /// </summary>
        /// <param name="message"></param>
        public void SendSystemMessage(string message)
        {
            var session = DefaultWebSocketSession;
            if (session != null)
            {
                session.Send(message);
            }
        }

        public static event ReceiveMessagedDelegate OnReceiveMessaged;

        public static Action<string, object> OnReceivedData;

        public static void RaiseReceiveMessaged(object sender, string msg)
        {
            if (OnReceiveMessaged != null)
            {
                var args = new ReceiveMessagedEventArgs(msg);
                OnReceiveMessaged(sender, args);
            }
        }

        internal WebSocketSession GetSocketItem()
        {
            lock (_lockobj)
            {
                WebSocketSession item = null;
                do
                {
                    var count = _caches.Count();
                    item = _caches.Where(x => x.IsBusy == false).FirstOrDefault();
                    if (item == null && count < MaxConnectNum)
                    {
                        item = new WebSocketSession(GetAddress());
                        if (_caches.Count == 0)
                            item.IsDefault = true;
                        item.Open();
                        _caches.Add(item);
                    }
                    else if (item != null)
                    {
                        if (item.Socket.State != WebSocketState.Open && item.Socket.State != WebSocketState.Connecting)
                            item.Open();
                        break;
                    }
                    else if (item == null)
                        Thread.Sleep(500);
                }
                while (true);
                return item;
            }
        }

        internal void ClearSocketSessions()
        {
            var list = _caches.Where(x => x.IsBusy == false
                && x.IsDefault == false
                && x.LastTime.AddSeconds(30) < DateTime.Now).ToList();
            lock (_lockobj)
            {
                foreach (var n in list)
                {
                    try
                    {
                        n.Close();
                    }
                    finally
                    {
                        _caches.Remove(n);

                    }
                }
            }
        }

        private string GetAddress()
        {
            string address = PropertiesUtil.Instance.GetProperty("websocketUrl");
            return address;
        }

        private CustomDataPackage GetParameters(string cmd, object parameter)
        {
            var pkg = new CustomDataPackage(parameter);
            pkg.Command = cmd;
            return pkg;
        }

        private void AddTask(WebSocketTask task)
        {
            if (!_tasks.ContainsKey(task.Id))
            {
                lock (_lockobj)
                {
                    _tasks.Add(task.Id, task);
                }
            }
        }

        internal void RemoveTask(string id)
        {
            lock (_lockobj)
            {
                _tasks.Remove(id);
            }
        }

        /// <summary>
        /// 主机地址
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 主机端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 最大连接数
        /// </summary>
        public int MaxConnectNum { get; set; }




        internal Dictionary<string, WebSocketTask> Tasks
        {
            get
            {
                return _tasks;
            }
        }

        #region IService

        public void InvokeNoResult(string cmd, OnCompletion result, Action networkerrAction = null)
        {
            var item = GetSocketItem();
            var param = GetParameters(cmd, null);
            item.Send(param.ToBytes());
            result(null);
        }

        public void Invoke(string cmd, OnCompletion<object> result, Action networkerrAction = null)
        {
            var param = GetParameters(cmd, null);
            param.Id = Guid.NewGuid().ToString("N");
            var task = new WebSocketTask()
            {
                Id = param.Id,
                OnCompletion = result,
                NetworkErrorAction = networkerrAction,
                Data = param.ToBytes()
            };
            task.Run();
            AddTask(task);
        }

        public void InvokeWithParamter(string cmd, object paramter, OnCompletion<object> result, Action networkerrAction = null)
        {
            var param = GetParameters(cmd, paramter);
            param.Id = Guid.NewGuid().ToString("N");
            var task = new WebSocketTask()
            {
                Id = param.Id,
                OnCompletion = result,
                NetworkErrorAction = networkerrAction,
                Data = param.ToBytes()
            };
            task.Run();
            AddTask(task);
        }


        public void InvokeWithPackage(string cmd, byte[] data, OnCompletion<object> result, Action networkerrAction = null)
        {
            var param = GetParameters(cmd, data);
            param.Id = Guid.NewGuid().ToString("N");
            var task = new WebSocketTask()
            {
                Id = param.Id,
                OnCompletion = result,
                NetworkErrorAction = networkerrAction,
                Data = param.ToBytes()
            };
            task.Run();
            AddTask(task);
        }

        #endregion

        public void Shutdown()
        {
            foreach (var session in _caches)
            {
                session.Close();
            }
        }

        public void Dispose()
        {
            Shutdown();
        }
    }

    public class WebSocketSession : IDisposable
    {
        public WebSocketSession(string address)
        {
            Socket = new WebSocket(address);
            Socket.Opened += Socket_Opened;
            Socket.Closed += Socket_Closed;
            Socket.MessageReceived += Socket_MessageReceived;
            Socket.Error += Socket_Error;
            Socket.DataReceived += Socket_DataReceived;
            LastTime = DateTime.Now;
        }

        void Socket_DataReceived(object sender, DataReceivedEventArgs e)
        {
            this.IsBusy = false;
            LastTime = DateTime.Now;
            ThreadPool.QueueUserWorkItem((s) =>
            {

                var data = e.Data;
                var obj = CustomDataPackage.ToDataPackage(data);
                if (obj != null && obj is CustomDataPackage)
                {
                    //解析自定义动态类型
                    var dataPackage = obj;
                    if (!string.IsNullOrEmpty(dataPackage.ErrorMessage))
                    {
                        //跳出服务器上出错信息
#if DEBUG
                        Android.Util.Log.Debug("Mono_WebSocketProxy", dataPackage.ErrorMessage);
#endif
                        return;
                    }
                    //解析自定义动态类型值 objValue
                    var id = obj.Id;
                    WebSocketTask task;
                    WebSocketProxy.Current.Tasks.TryGetValue(id, out task);
                    if (task != null)
                    {
                        WebSocketProxy.Current.RemoveTask(id);
                        task.OnCompletion(obj.Value, null);
                        task.Dispose();
                    }
                    else
                    {
                        if (WebSocketProxy.OnReceivedData != null)
                        {
                            WebSocketProxy.OnReceivedData(dataPackage.Command, dataPackage.Value);
                        }
                    }
                }
            });
        }

        void Socket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            Error = e.Exception.Message;
#if DEBUG
            Android.Util.Log.Debug("Mono_WebSocketProxy", Error);
#endif
            this.IsBusy = false;
            _isShutdown = true;
        }

        void Socket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            //显示消息
            WebSocketProxy.RaiseReceiveMessaged(this, e.Message);
            LastTime = DateTime.Now;
            this.IsBusy = false;
        }

        void Socket_Closed(object sender, EventArgs e)
        {
            if (!_isShutdown)
            {
                //如果非正常的关闭连接，则自动重新连接
                Thread.Sleep(1000);
                Open();
            }
        }

        void Socket_Opened(object sender, EventArgs e)
        {
            IsBusy = false;
        }

        public WebSocket Socket { get; set; }
        public bool IsBusy { get; set; }
        public string Error { get; set; }

        public WebSocketTask CurrentTask { get; set; }
        
        public bool IsReady
        {
            get
            {
                return (Socket != null && Socket.State == WebSocketState.Open);
            }
        }

        public bool IsDefault { get; set; }

        public int WaitTime { get; set; }

        public DateTime LastTime { get; set; }

        private bool _isShutdown;

        public void Send(string message)
        {
            while (!IsReady)
            {
                Thread.Sleep(200);
                WaitTime += 200;
                if (WaitTime > 10000)
                {  
                    var errorMsg = string.Format("网络连接超时\r\n可能服务器没启动或客户无信任权限等原因造成网络通讯异常！");
#if DEBUG
                    Android.Util.Log.Debug("Mono_WebSocketProxy", errorMsg);
#endif
                    WaitTime = 0;
                    IsBusy = false;
                    WebSocketTask task = this.CurrentTask;
                    if (task != null)
                    {
                        WebSocketProxy.Current.RemoveTask(task.Id);
                        if (task.NetworkErrorAction != null)
                        {
                            task.NetworkErrorAction();
                        }
                        task.Dispose();
                    }
                    return;
                }
            }
            IsBusy = true;
            Socket.Send(message);
        }

        public void Send(byte[] data)
        {
            while (!IsReady)
            {
                Thread.Sleep(200);
                WaitTime += 200;
                if (WaitTime > 10000)
                {
                    var errorMsg = string.Format("网络连接超时\r\n可能服务器没启动或客户无信任权限等原因造成网络通讯异常！");
#if DEBUG
                    Android.Util.Log.Debug("Mono_WebSocketProxy", errorMsg);
#endif
                    WaitTime = 0;
                    IsBusy = false;
                    WebSocketTask task =this.CurrentTask;
                    if (task != null)
                    {
                        WebSocketProxy.Current.RemoveTask(task.Id);
                        if (task.NetworkErrorAction != null)
                        {
                            task.NetworkErrorAction();
                        }
                        task.Dispose();
                    }
                    return;
                }
            }
            IsBusy = true;
            //try
            //{
            //    using (System.IO.FileStream fileStream = new System.IO.FileStream("/mnt/sdcard/customdata", System.IO.FileMode.CreateNew))
            //    {
            //        fileStream.Write(data, 0, data.Length);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Android.Util.Log.Debug("Mono_WebSocketProxy", ex.StackTrace); 
            //}

            Socket.Send(data, 0, data.Length);
        }



        public void Open()
        {
            if (!IsReady)
                Socket.Open();
        }

        public void Close()
        {
            _isShutdown = true;
            Socket.Close();
        }

        public void Dispose()
        {
            Socket = null;
        }

    }

    /// <summary>
    /// 通讯任务
    /// </summary>
    public class WebSocketTask : IDisposable
    {
        //任务ID
        public string Id { get; set; }
        //自动重发
        public bool IsAutoSend { get; set; }
        //通讯数据
        public byte[] Data { get; set; }
        //完成回调任务
        public OnCompletion<object> OnCompletion { get; set; }

        /// <summary>
        /// 网络错误回调
        /// </summary>
        public Action NetworkErrorAction { get; set; }

        //启动发送
        public void Run()
        {
            var item = WebSocketProxy.Current.GetSocketItem();
            item.CurrentTask = this;
            item.Send(Data);
        }
        //释放任务
        public void Dispose()
        {
            Data = null;
            OnCompletion = null;
            NetworkErrorAction = null;
        }
    }

}
