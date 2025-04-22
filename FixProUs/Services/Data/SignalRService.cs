
//using Microsoft.AspNet.SignalR.Client;
using FixProUs.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Twilio.TwiML.Messaging;

namespace FixPro.Services.Data
{
    public class SignalRService
    {
        private readonly HubConnection _hubConnection;

        public event Action<string, string, string, string> OnMessageReceived;
        public event Action<string, string, string, string> OnMessageReceivedUserData;
        public event Action<DataMapsModel> OnMessageReceivedLocation;

        public SignalRService()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://fixproapi.engprosoft.net/ChatHub") // Update with your hub URL
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<string, string, string, string>("ReceiveMessage", (user, message, userFrom, userTo) =>
            {
                OnMessageReceived?.Invoke(user, message, userFrom, userTo);
            });

            _hubConnection.On<string, string, string, string>("ChangeUserData", (user, message, userFrom, userTo) =>
            {
                OnMessageReceivedUserData?.Invoke(user, message, userFrom, userTo);
            });

            _hubConnection.On<DataMapsModel>("ReceiveLocation", (locationData) =>
            {
                OnMessageReceivedLocation?.Invoke(locationData);
            });
        }

        public async Task StartAsync()
        {
            try
            {
                await _hubConnection.StartAsync();
                Console.WriteLine("SignalR connected.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR connection failed: {ex.Message}");
            }
        }

        public async Task Disconnect()
        {
            try
            {
                await _hubConnection.StopAsync();
                Console.WriteLine("SignalR disconnected.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR disconnection failed: {ex.Message}");
            }
        }


        public async Task SendLocation(DataMapsModel locationData)
        {
            try
            {
                if (_hubConnection.State != HubConnectionState.Connected)
                {
                    await StartAsync();
                }

                if (_hubConnection.State == HubConnectionState.Connected)
                {
                    await _hubConnection.InvokeAsync("UpdateLocation", locationData);
                }
                else
                {
                    Console.WriteLine("Failed to send location: SignalR connection is still not active.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SendLocation failed: {ex.Message}");
            }
        }

        public async Task SendMessage(string user, string message)
        {
            try
            {
                await _hubConnection.InvokeAsync("SendMessage", user, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SendMessage failed: {ex.Message}");
            }
        }
    }

    //public class SignalRService
    //{
    //    private readonly HubConnection _hubConnection;
    //    private readonly IHubProxy _hubProxy;
    //    public event Action<string, string, string, string> OnMessageReceived;

    //    public SignalRService()
    //    {

    //        _hubConnection = new HubConnection("https://fixproapi.engprosoft.net/");
    //        _hubConnection.TraceLevel = TraceLevels.All;
    //        _hubConnection.TraceWriter = Console.Out;
    //        _hubProxy = _hubConnection.CreateHubProxy("ChatHub");

    //        _hubProxy.On<string, string, string, string>("ReceiveMessage", (user, message, userFrom, userTo) =>
    //        {
    //            // Handle received message
    //            OnMessageReceived?.Invoke(user, message, userFrom, userTo);
    //        });
    //    }

    //    public async Task StartAsync()
    //    {
    //        try
    //        {
    //            await _hubConnection.Start();
    //            Console.WriteLine("SignalR connected.");
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine($"SignalR connection failed: {ex.Message}");
    //        }
    //    }

    //    public async Task Disconnect()
    //    {
    //        try
    //        {
    //            _hubConnection.Stop();
    //            Console.WriteLine("SignalR disconnected.");
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine($"SignalR disconnection failed: {ex.Message}");
    //        }
    //    }

    //    public async Task SendMessage(string user, string message)
    //    {
    //        try
    //        {
    //            await _hubProxy.Invoke("SendMessage", user, message);
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine($"SendMessage failed: {ex.Message}");
    //        }
    //    }
    //}
}
