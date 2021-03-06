﻿using Spectrum.API;
using Spectrum.API.Interfaces.Plugins;
using Spectrum.API.Interfaces.Systems;
using System;
using System.Reflection;
using Harmony;

namespace Corecii.DiscordRPCEnabler
{
    public class Entry : IPlugin
    {
        public string FriendlyName => "Discord Rich Presence Enabler";
        public string Author => "Corecii";
        public string Contact => "SteamID: Corecii; Discord: Corecii#3019";
        public APILevel CompatibleAPILevel => APILevel.XRay;
        public static string PluginVersion = "Version C.1.1.0";

        public void Initialize(IManager manager)
        {
            try {
                var harmony = HarmonyInstance.Create("com.corecii.distance.discordRpcEnabler");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception e)
            {
                Console.WriteLine("Patching errors!\n" + e);
            }
        }

        [HarmonyPatch(typeof(DiscordController))]
        [HarmonyPatch("Start")]
        [HarmonyPriority(Priority.Last)]
        class PatchStart
        {
            static bool Prefix(DiscordController __instance)
            {
                try
                {
                    setPrivateField(__instance, "gameManager_", G.Sys.GameManager_);
                    if (BuildType.useAdventureFinal_)
                    {
                        UnityEngine.Object.Destroy(__instance);
                        return false;
                    }
                    DiscordRpc.UpdatePresence(ref __instance.presence);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Start Patch errors!\n" + e);
                }
                return false;
            }
        }

        [HarmonyPatch(typeof(DiscordController))]
        [HarmonyPatch("UpdateNetworkDiscordInfo")]
        class PatchUpdate
        {
            static void Postfix(DiscordController __instance)
            {
                try
                {
                    if (G.Sys.NetworkingManager_.IsOnline_)
                    {
                        __instance.presence.partyMax = Math.Max(G.Sys.PlayerManager_.TotalPlayerCount_, G.Sys.NetworkingManager_.maxPlayerCount_);
                        __instance.presence.partySize = G.Sys.PlayerManager_.TotalPlayerCount_;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("UpdateNetworkDiscordInfo Patch errors!\n" + e);
                }
            }
        }

        public void Shutdown() { }

        public static void setPrivateField(object obj, string fieldName, object value)
        {
            obj
                .GetType()
                .GetField(
                    fieldName,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static
                )
                .SetValue(obj, value);
        }
    }
}
