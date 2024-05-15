﻿using System;
using System.Data;
using DiscordRPC;
using DiscordRPC.Logging;
using MHURPorting.Views.Extensions;
using Serilog;

namespace MHURPorting.Services;

public static class DiscordService
{
    private const string ID = "1225146168447733893";
    
    private static DiscordRpcClient? Client;
    
    private static readonly Assets Assets = new() { LargeImageKey = "icon",SmallImageKey = "icon" , LargeImageText = "MHUR Porting"};
    
    private static readonly Timestamps Timestamp = new() { Start = DateTime.UtcNow };

    private static readonly RichPresence DefaultPresence = new()
    {
        State = "Idle",
        Timestamps = Timestamp,
        Assets = Assets,
        Buttons = new[]
        {
            new Button
            {
                Label = "Join Porteria!",
                Url = Globals.DISCORD_URL
            },
            new Button() 
            {
                Label = "Download MHUR Porting",
                Url = Globals.GITHUB_URL
            }
        }
    };
    
    public static void Initialize()
    {
        if (Client is not null && !Client.IsDisposed) return;
        
        Client = new DiscordRpcClient(ID);
        Client.OnReady += (_, args) => Log.Information("Discord Service Started for {0}", args.User.Username);
        Client.OnError += (_, args) => Log.Information("Discord Service Error {0}: {1}", args.Type.ToString(), args.Message);

        Client.Initialize();
        Client.SetPresence(DefaultPresence);
    }
    
    public static void DeInitialize()
    {
        var user = Client?.CurrentUser;
        Log.Information("Discord Service Stopped for {0}#{1}", user?.Username, user?.Discriminator);
        Client?.Deinitialize();
        Client?.Dispose();
    }

    public static void Update(EAssetType assetType)
    {
        Client?.UpdateState($"Browsing {assetType.GetDescription()}");
        Client?.UpdateSmallAsset(assetType.ToString().ToLower(), assetType.GetDescription());
    }

}