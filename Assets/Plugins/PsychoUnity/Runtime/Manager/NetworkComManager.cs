using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PsychoUnity.Manager
{
    /// <summary>
    /// Network communication manager now support the management of four different network
    /// entities : `TCPServer`, `TCPClient`, `UDPServer`, and `UDPClient`
    /// </summary>
    public class NetworkComManager : Singleton<NetworkComManager>
    {
        private readonly Dictionary<string, NetworkComBase> _networkDic = new Dictionary<string, NetworkComBase>();

        /// <summary>
        /// Create a network entity
        /// </summary>
        /// <param name="name">name of the entity</param>
        /// <param name="hostName">hostname</param>
        /// <param name="port">port</param>
        /// <param name="type">the type of the entity that you want to create</param>
        /// <param name="mode"></param>
        /// <exception cref="InvalidOperationException">The network entity named {the passed name} already exists</exception>
        /// <exception cref="ArgumentOutOfRangeException">The passed type does`t exists</exception>
        public void Create(string name, string hostName, int port, NetWorkType type, WorkMode mode = WorkMode.Auto)
        {
            if (_networkDic.ContainsKey(name))
            {
                throw new InvalidOperationException($"A network with the name '{name}' already exists.");
            }

            switch (type)
            {
                case NetWorkType.TcpServer:
                    _networkDic.Add(name, new NetworkServer(hostName, port, NetWorkType.TcpServer, name, mode));
                    break;
                case NetWorkType.TcpClient:
                    _networkDic.Add(name, new NetworkClient(hostName, port, NetWorkType.TcpClient, name, mode));
                    break;
                case NetWorkType.UdpServer:
                    _networkDic.Add(name, new NetworkServer(hostName, port, NetWorkType.UdpServer, name, mode));
                    break;
                case NetWorkType.UdpClient:
                    _networkDic.Add(name, new NetworkClient(hostName, port, NetWorkType.UdpClient, name, mode));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public bool CheckConnect(string name)
        {
            return Verify(name) && _networkDic[name].Check();
        }

        /// <summary>
        /// Init connection
        /// </summary>
        /// <param name="name"></param>
        public async Task InitAsync(string name)
        {
            if (Verify(name))
            {
                await _networkDic[name].InitAsync();
            }
        }

        /// <summary>
        /// Read data
        /// </summary>
        /// <param name="name">the name of the network entity that needs to be controlled</param>
        /// <param name="buffer">caller need to pass a buffer to receive data</param>
        public void Read(string name, byte[] buffer)
        {
            if (Verify(name))
            {
                _networkDic[name].Read(buffer);
            }
        }

        /// <summary>
        /// Read data and return it to the caller
        /// </summary>
        /// <param name="name">the name of network entity that needs to be controlled</param>
        /// <returns>data</returns>
        public string ReadLine(string name)
        {
            return Verify(name) ? _networkDic[name].ReadLine() : string.Empty;
        }

        /// <summary>
        /// Asynchronous data reading
        /// </summary>
        /// <param name="name">the name of network entity that needs to be controlled</param>
        /// <param name="buffer">caller need to pass a buffer to receive data</param>
        public async Task ReadAsync(string name, byte[] buffer)
        {
            if (Verify(name))
            {
                await _networkDic[name].ReadAsync(buffer);
            }
        }

        /// <summary>
        /// Asynchronous data reading and return it to caller
        /// </summary>
        /// <param name="name">the name of network entity than needs to be controlled</param>
        /// <returns>data</returns>
        public async Task<string> ReadLineAsync(string name)
        {
            if (Verify(name))
            {
                return await _networkDic[name].ReadLineAsync();
            }

            return string.Empty;
        }

        /// <summary>
        /// Send data
        /// </summary>
        /// <param name="name">the name of network entity that needs to be controlled</param>
        /// <param name="buffer">data to be sent</param>
        public void Send(string name, byte[] buffer)
        {
            if (Verify(name))
            {
                _networkDic[name].Send(buffer);
            }
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="name">the name of network entity that needs to be controlled</param>
        /// <param name="msg">message to be sent</param>
        public void SendLine(string name, string msg)
        {
            if (Verify(name))
            {
                _networkDic[name].SendLine(msg);
            }
        }

        /// <summary>
        /// Asynchronous data sending
        /// </summary>
        /// <param name="name">the name of network entity that needs to be controlled</param>
        /// <param name="buffer">data to be sent</param>
        public async Task SendAsync(string name, byte[] buffer)
        {
            if (Verify(name))
            {
                await _networkDic[name].SendAsync(buffer);
            }
        }

        /// <summary>
        /// Asynchronous message sending
        /// </summary>
        /// <param name="name">the name of the network entity that needs to be controlled</param>
        /// <param name="msg">message to be sent</param>
        public async Task SendMsgAsync(string name, string msg)
        {
            if (Verify(name))
            {
                await _networkDic[name].SendLineAsync(msg);
            }
        }

        /// <summary>
        /// Stop a network entity
        /// </summary>
        /// <param name="name">the name of the network entity that you want to stop</param>
        public void Stop(string name)
        {
            if (Verify(name))
            {
                _networkDic[name].Stop();
            }
        }

        /// <summary>
        /// Clear all network entities
        /// </summary>
        public void Clear()
        {
            _networkDic?.Clear();
        }

        private bool Verify(string name)
        {
            if (_networkDic.ContainsKey(name))
            {
                return true;
            }

            throw new InvalidOperationException($"Network {name} does`t exist");
        }
    }

    internal abstract class NetworkComBase
    {
        internal abstract Task InitAsync();

        internal abstract int Read(byte[] buffer);

        internal abstract string ReadLine();

        internal abstract Task<int> ReadAsync(byte[] buffer);

        internal abstract Task<string> ReadLineAsync();

        internal abstract void Send(byte[] buffer);

        internal abstract void SendLine(string msg);

        internal abstract Task SendAsync(byte[] buffer);

        internal abstract Task SendLineAsync(string msg);

        internal abstract bool Check();

        internal abstract void Stop();
        internal abstract Task ListeningAsync();

        internal abstract bool HeartBeat();
        internal abstract Task<bool> HeartBeatAsync();
    }

    internal class NetworkServer : NetworkComBase
    {
        private IPEndPoint IpEndPoint { get; }
        private Socket Listener { get; set; }

        private readonly string _entityName;
        private readonly WorkMode _workMode;
        private readonly NetWorkType _netWorkType;

        // The connection with client
        private Socket Handler { get; set; }

        private bool Connected { get; set; }

        // Init
        internal NetworkServer(string hostName, int port, NetWorkType type, string entityName,
            WorkMode mode = WorkMode.Auto)
        {
            IpEndPoint = new IPEndPoint(IPAddress.Parse(hostName), port);
            _entityName = entityName;
            _workMode = mode;
            _netWorkType = type;

        }

        internal override async Task InitAsync()
        {
            try
            {
                // Create listener
                Listener = _netWorkType switch
                {
                    NetWorkType.TcpServer => new Socket(IpEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp),
                    NetWorkType.TcpClient => throw new ArgumentOutOfRangeException(nameof(_netWorkType), _netWorkType, null),
                    NetWorkType.UdpServer => new Socket(IpEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Udp),
                    NetWorkType.UdpClient => throw new ArgumentOutOfRangeException(nameof(_netWorkType), _netWorkType, null),
                    _ => throw new ArgumentOutOfRangeException(nameof(_netWorkType), _netWorkType, null)
                };

                Listener.Bind(IpEndPoint);
                Listener.Listen(100);
                
                // Create communication stream
                Debug.Log("Waiting for connection...");
                Handler = await Listener.AcceptAsync();
                // Set connected state true
                Connected = true;
                Debug.Log("Connected");
                
                // Determine whether to enable Auto mode
                if (_workMode == WorkMode.Auto)
                {
                    var _ = this.ListeningAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        internal override int Read(byte[] buffer)
        {
            try
            {
                if (Verify() && HeartBeat())
                {
                    return Handler.Receive(buffer);
                }

                return 0;
            }
            catch (Exception)
            {
                Handler.Close();
                throw;
            }
        }

        internal override string ReadLine()
        {
            var buffer = new byte[1024];
            return Read(buffer) > 0 ? Encoding.UTF8.GetString(buffer) : string.Empty;
        }

        internal override async Task<int> ReadAsync(byte[] buffer)
        {
            try
            {
                if (Verify() && await HeartBeatAsync())
                {
                    return await Handler.ReceiveAsync(buffer, SocketFlags.None);
                }

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        internal override async Task<string> ReadLineAsync()
        {
            var buffer = new byte[1024];
            return await ReadAsync(buffer) > 0 ? Encoding.UTF8.GetString(buffer) : string.Empty;
        }

        internal override void Send(byte[] buffer)
        {
            try
            {
                if (Verify())
                {
                    Handler.Send(buffer, SocketFlags.None);
                }
            }
            catch (Exception)
            {
                Handler.Close();
                throw;
            }
        }

        internal override void SendLine(string msg)
        {
            Send(Encoding.UTF8.GetBytes(msg));
        }

        internal override async Task SendAsync(byte[] buffer)
        {
            try
            {
                if (Verify())
                {
                    await Handler.SendAsync(buffer, SocketFlags.None);
                }
            }
            catch (Exception)
            {
                Handler.Close();
                throw;
            }
        }

        internal override async Task SendLineAsync(string msg)
        {
            await SendAsync(Encoding.UTF8.GetBytes(msg));
        }

        internal override bool Check()
        {
            return Connected;
        }

        internal override void Stop()
        {
            Connected = false;
            Handler?.Shutdown(SocketShutdown.Both);
            Handler?.Close();
            Listener.Close();
        }

        internal override async Task ListeningAsync()
        {
            try
            {
                while (Handler.Connected && await HeartBeatAsync())
                {
                    var buffer = new byte[1024];
                    await ReadAsync(buffer);
                    EventManager.Instance.EventTrigger($"{_entityName}", buffer);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Handler.Close();
                throw;
            }
        }

        internal override bool HeartBeat()
        {
            try
            {
                Send(new byte[1]);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        internal override async Task<bool> HeartBeatAsync()
        {
            try
            {
                await SendAsync(new byte[1]);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private bool Verify()
        {
            if (Handler != null)
            {
                return true;
            }

            throw new InvalidOperationException("Connection does`t exists...");
        }
    }

    internal class NetworkClient : NetworkComBase
    {
        private IPEndPoint IpEndPoint { get; }

        private Socket Client { get; set; }

        private readonly string _entityName;
        private readonly WorkMode _workMode;
        private readonly NetWorkType _netWorkType;

        private bool Connected { get; set; }

        internal NetworkClient(string hostName, int port, NetWorkType type, string entityName,
            WorkMode mode = WorkMode.Auto)
        {
            IpEndPoint = new IPEndPoint(IPAddress.Parse(hostName), port);
            _entityName = entityName;
            _workMode = mode;
            _netWorkType = type;

        }

        internal override async Task InitAsync()
        {
            try
            {
                // Create client socket
                Client = _netWorkType switch
                {
                    NetWorkType.TcpClient => new Socket(IpEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp),
                    NetWorkType.UdpClient => new Socket(IpEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Udp),
                    NetWorkType.TcpServer => throw new ArgumentOutOfRangeException(nameof(_netWorkType), _netWorkType, null),
                    NetWorkType.UdpServer => throw new ArgumentOutOfRangeException(nameof(_netWorkType), _netWorkType, null),
                    _ => throw new ArgumentOutOfRangeException(nameof(_netWorkType), _netWorkType, null)
                };
                
                // Create communication stream with server
                Debug.Log("Waiting for connection...");
                await Client.ConnectAsync(IpEndPoint);
                // Set connect state to true
                Connected = true;
                Debug.Log("Connected");
                
                // Determine werther to enable Auto mode
                if (_workMode == WorkMode.Auto)
                {
                    await ListeningAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        internal override int Read(byte[] buffer)
        {
            try
            {
                if (Client.Connected && HeartBeat())
                {
                    return Client.Receive(buffer, SocketFlags.None);
                }

                return 0;
            }
            catch (Exception)
            {
                Client.Close();
                throw;
            }
        }

        internal override string ReadLine()
        {
            var buffer = new byte[1024];
            return Read(buffer) > 0 ? Encoding.UTF8.GetString(buffer) : string.Empty;
        }

        internal override async Task<int> ReadAsync(byte[] buffer)
        {
            try
            {
                if (Client.Connected && await HeartBeatAsync())
                {
                    return await Client.ReceiveAsync(buffer, SocketFlags.None);
                }

                return 0;
            }
            catch (Exception)
            {
                Client.Close();
                throw;
            }
        }

        internal override async Task<string> ReadLineAsync()
        {
            var buffer = new byte[1024];

            return await ReadAsync(buffer) > 0 ? Encoding.UTF8.GetString(buffer) : string.Empty;
        }

        internal override void Send(byte[] buffer)
        {
            try
            {
                if (Client.Connected)
                {
                    Client.Send(buffer, SocketFlags.None);
                }
            }
            catch (Exception)
            {
                Client.Close();
                throw;
            }
        }

        internal override void SendLine(string msg)
        {
            Send(Encoding.UTF8.GetBytes(msg));
        }

        internal override async Task SendAsync(byte[] buffer)
        {
            try
            {
                if (Client.Connected)
                {
                    await Client.SendAsync(buffer, SocketFlags.None);
                }
            }
            catch (Exception)
            {
                Client.Close();
                throw;
            }
        }

        internal override async Task SendLineAsync(string msg)
        {
            await SendAsync(Encoding.UTF8.GetBytes(msg));
        }

        internal override bool Check()
        {
            return Connected;
        }

        internal override void Stop()
        {
            Connected = false;
            Client.Close();
        }

        internal override async Task ListeningAsync()
        {
            try
            {
                while (Client.Connected && await HeartBeatAsync())
                {
                    var buffer = new byte[1024];
                    await ReadAsync(buffer);
                    EventManager.Instance.EventTrigger($"{_entityName}", buffer);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Client.Close();
                throw;
            }
        }

        internal override bool HeartBeat()
        {
            try
            {
                Send(new byte[1]);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        internal override async Task<bool> HeartBeatAsync()
        {
            try
            {
                await SendAsync(new byte[1]);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    /// <summary>
    /// Choose the type of network entity to be create
    /// </summary>
    public enum NetWorkType
    {
        TcpServer,
        TcpClient,
        UdpServer,
        UdpClient
    }

    /// <summary>
    /// Choose the work mode of network entity
    /// </summary>
    public enum WorkMode
    {
        Manual,
        Auto
    }
}