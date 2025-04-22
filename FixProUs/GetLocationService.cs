
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices.Sensors;
using System;
using FixProUs.Models;
using FixPro.Services.Data;

namespace FixProUs
{

    public class GetLocationService
    {
        private readonly bool stopping = false;
        private static int Idincerment = 0;
        private readonly HubConnection _hubConnection;
        public event Action<DataMapsModel> OnMessageReceivedLocation;

        public GetLocationService()
        {
            _hubConnection = new HubConnectionBuilder()
               .WithUrl("https://api.fixprous.com/ChatHub") // Update with your hub URL
               .WithAutomaticReconnect()
               .Build();

            _hubConnection.On<DataMapsModel>("ReceiveLocation", (locationData) =>
            {
                OnMessageReceivedLocation?.Invoke(locationData);
            });
            
        }

        public async Task StartAsync(CancellationToken token)
        {
            try
            {
                await _hubConnection.StartAsync();

                await Task.Run(async () =>
                {
                    while (!stopping)
                    {
                        token.ThrowIfCancellationRequested();
                        try
                        {
                            var location = await MainThread.InvokeOnMainThreadAsync<Location>(() =>
                            {
                                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                                return Geolocation.GetLocationAsync(request);
                            });

                            if (location != null)
                            {
                                Idincerment += 1;

                                var locationData = new DataMapsModel
                                {
                                    Id = Idincerment,
                                    BranchId = int.Parse(Helpers.Settings.BranchId),
                                    EmployeeId = int.Parse(Helpers.Settings.UserId),
                                    Lat = location.Latitude.ToString(),
                                    Long = location.Longitude.ToString(),
                                    Time = location.Timestamp.ToString(),
                                    CreateDate = DateTime.Now.ToShortDateString(),
                                    MPosition = new Location(location.Latitude, location.Longitude),
                                };

                                // Send location data via SignalR
                                if (_hubConnection.State == HubConnectionState.Connected)
                                {
                                    await _hubConnection.SendAsync("UpdateLocation", locationData);
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }

                        await Task.Delay(2000); // Reduce CPU usage
                    }
                }, token);
            }
            catch (Exception ex)
            {
                Console.WriteLine("SignalR Connection Error: " + ex.Message);
            }
        }

        public async Task StopAsync()
        {
            if (_hubConnection.State == HubConnectionState.Connected)
            {
                await _hubConnection.StopAsync();
            }
        }

    }

    //public class GetLocationService
    //{
    //    readonly bool stopping = false;

    //    static int Idincerment = 0;
    //    public GetLocationService()
    //    {
    //    }


    //    public async Task Run(CancellationToken token)
    //    {

    //        await Task.Run(async () =>
    //        {
    //            while (!stopping)
    //            {
    //                token.ThrowIfCancellationRequested();
    //                try
    //                {
    //                    //await Task.Delay(2000);

    //                    //var request = new GeolocationRequest(GeolocationAccuracy.High);
    //                    //var location = await Geolocation.GetLocationAsync(request);
    //                    var location = await MainThread.InvokeOnMainThreadAsync<Location>(() =>
    //                    {
    //                        var request = new GeolocationRequest(GeolocationAccuracy.Medium);
    //                        return Geolocation.GetLocationAsync(request);
    //                    });

    //                    if (location != null)
    //                    {
    //                        var message = new LocationMessage
    //                        {
    //                            Latitude = location.Latitude,
    //                            Longitude = location.Longitude
    //                        };


    //                        List<DataMapsModel> Listmap = new List<DataMapsModel>();
    //                        Idincerment += 1;

    //                        Listmap.Add(new DataMapsModel
    //                        {
    //                            Id = Idincerment,
    //                            BranchId = int.Parse(Helpers.Settings.BranchId),
    //                            EmployeeId = int.Parse(Helpers.Settings.UserId),
    //                            Lat = location.Latitude.ToString(),
    //                            Long = location.Longitude.ToString(),
    //                            Time = location.Timestamp.ToString(),
    //                            CreateDate = DateTime.Now.ToShortDateString(),
    //                            MPosition = new Location(location.Latitude, location.Longitude),
    //                        });

    //                        await Helpers.Utility.PostData("api/UploadXML/PostXmlFile", JsonConvert.SerializeObject(Listmap, Newtonsoft.Json.Formatting.None,
    //                                    new JsonSerializerSettings()
    //                                    {
    //                                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    //                                    }));

    //                        Device.BeginInvokeOnMainThread(() =>
    //                        {
    //                            MessagingCenter.Send(message, "Location");
    //                        });
    //                    }

    //                }
    //                catch (Exception ex)
    //                {
    //                    Device.BeginInvokeOnMainThread(() =>
    //                    {
    //                        var errormessage = new LocationErrorMessage();
    //                        MessagingCenter.Send(errormessage, "LocationError");
    //                    });
    //                }
    //            }
    //            return;
    //        }, token);
    //    }
    //}
}
