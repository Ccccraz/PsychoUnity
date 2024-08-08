using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

namespace PsychoUnity.Manager
{
    public class TcpServerManager : Singleton<TcpServerManager>
    {
        private readonly Dictionary<string, TcpServer> _tcpServerDic = new();
        
        public void Create(string name, string hostname, int port)
        {
            if (_tcpServerDic.ContainsKey(name))
            {
                Debug.LogWarning($"tcp server {name} already exist");
            }
            else
            {
                _tcpServerDic.Add(name, new TcpServer(hostname, port));
            }
        }

        public async Task StartAsync(string name)
        {
            var server = Verify(name);

            if (server != null)
            {
                await server.ListenAsync();
            }
        }

        public async Task Close(string name)
        {
            var server = Verify(name);

            if (server != null)
            {
                await server.StopAsync();
            }
        }

        public void AddEvent(string name, string eventName)
        {
            var server = Verify(name);

            server?.AddEvent(eventName);
        }

        private TcpServer Verify(string name)
        {
            return _tcpServerDic.GetValueOrDefault(name);
        }
    }

    internal class TcpServer
    {
        // Configuration of tcp server
        private IPEndPoint IpEndPoint { get; }
        private TcpListener Server { get; set; }
        
        // Container of client connect
        private readonly ConcurrentBag<Task> _clientTasks = new();

        // List of event type
        private readonly List<string> _eventList = new();

        // Whether this server running
        private bool _isRunning;

        public TcpServer(string ip, int port)
        {
            IpEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            Server = new TcpListener(IpEndPoint);
        }

        public void AddEvent(string eventName)
        {
            _eventList.Add(eventName);
        }

        public async Task ListenAsync()
        {
            Server.Start();
            _isRunning = true;
            
            while (_isRunning)
            {
                try
                {
                    var client = await Server.AcceptTcpClientAsync();
                    var clientTask = ProcessClientAsync(client);
                    _clientTasks.Add(clientTask);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"{e} occured");
                }
            }

        }

        public async Task StopAsync()
        {
            _isRunning = false;
            var tasks = _clientTasks.ToArray();
            await Task.WhenAll(tasks);
            Server.Stop();
        }

        private async Task ProcessClientAsync(TcpClient client)
        {
            var stream = client.GetStream();

            try
            {
                var buf = new byte[1024];
                while ((await stream.ReadAsync(buf, 0, buf.Length)) != 0)
                {
                    var data = System.Text.Encoding.UTF8.GetString(buf);

                    foreach (var variable in _eventList)
                    {
                        switch (data)
                        {
                            case var _ when data == variable:
                                EventManager.Instance.EventTrigger(data, client);
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"{e} occured");
            }
            finally
            {
                client.Close();
            }
        }
    }
}