using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SudokuWPF.Model.Structures;

namespace SudokuWPF.Model
{
    public class MultiplayerManager
    {
        private readonly TcpListener _server;
        private readonly List<Player> _players;
        private readonly GameModel _gameModel;
        private bool _isHost;

        public event EventHandler<Player> PlayerJoined;
        public event EventHandler<Player> PlayerLeft;
        public event EventHandler<GameUpdate> GameUpdated;

        public MultiplayerManager(GameModel gameModel, int port = 5000)
        {
            _gameModel = gameModel;
            _players = new List<Player>();
            _server = new TcpListener(IPAddress.Any, port);
        }

        public async Task StartHostingAsync()
        {
            _isHost = true;
            _server.Start();
            await AcceptClientsAsync();
        }

        public async Task ConnectToHostAsync(string hostAddress, int port)
        {
            _isHost = false;
            using (var client = new TcpClient())
            {
                await client.ConnectAsync(hostAddress, port);
                await HandleClientConnectionAsync(client);
            }
        }

        private async Task AcceptClientsAsync()
        {
            while (true)
            {
                var client = await _server.AcceptTcpClientAsync();
                _ = HandleClientConnectionAsync(client);
            }
        }

        private async Task HandleClientConnectionAsync(TcpClient client)
        {
            using (var stream = client.GetStream())
            {
                var buffer = new byte[1024];
                while (true)
                {
                    var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    var gameUpdate = JsonConvert.DeserializeObject<GameUpdate>(message);

                    if (gameUpdate != null)
                    {
                        GameUpdated?.Invoke(this, gameUpdate);
                        if (_isHost)
                        {
                            await BroadcastUpdateAsync(gameUpdate);
                        }
                    }
                }
            }
        }

        private async Task BroadcastUpdateAsync(GameUpdate update)
        {
            var message = JsonConvert.SerializeObject(update);
            var data = Encoding.UTF8.GetBytes(message);

            foreach (var player in _players)
            {
                if (player.Client.Connected)
                {
                    await player.Client.GetStream().WriteAsync(data, 0, data.Length);
                }
            }
        }

        public async Task SendGameUpdateAsync(int col, int row, int value)
        {
            var update = new GameUpdate
            {
                Col = col,
                Row = row,
                Value = value,
                Timestamp = DateTime.Now
            };

            if (_isHost)
            {
                await BroadcastUpdateAsync(update);
            }
            else
            {
                // Відправляємо оновлення на сервер
                var message = JsonConvert.SerializeObject(update);
                var data = Encoding.UTF8.GetBytes(message);
                // TODO: Відправка на сервер
            }
        }
    }

    public class Player
    {
        public string Name { get; set; }
        public TcpClient Client { get; set; }
        public int Score { get; set; }
    }

    public class GameUpdate
    {
        public int Col { get; set; }
        public int Row { get; set; }
        public int Value { get; set; }
        public DateTime Timestamp { get; set; }
    }
} 