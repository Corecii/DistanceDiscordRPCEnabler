using Spectrum.API;
using Spectrum.API.Interfaces.Plugins;
using Spectrum.API.Interfaces.Systems;
using System.Collections.Generic;
using System;
using Spectrum.API.Configuration;
using System.Reflection;
using UnityEngine;

namespace Corecii.LevelEditorSchemes
{
    public class Entry : IPlugin
    {
        public string FriendlyName => "Level Editor WASD";
        public string Author => "Corecii";
        public string Contact => "SteamID: Corecii; Discord: Corecii#3019";
        public APILevel CompatibleAPILevel => APILevel.XRay;
        public static string PluginVersion = "Version C.1.0.0";

        static string[] toolNames = new string[] {
            ToolInputCombos.unityToolInputCombosFileName_,
            ToolInputCombos.blenderToolInputCombosFileName_
        };

        public void Initialize(IManager manager)
        {
            foreach (string name in toolNames)
            {
                var toolInputBase = ToolInputCombos.Load(name);
                var combos = toolInputBase.GetComponent<ToolInputCombos>();
                combos.Save(name);
            }
        }

        object callPrivateMethod(Type tp, object obj, string methodName, params object[] args)
        {
            return tp.GetMethod(
                methodName,
                BindingFlags.NonPublic | BindingFlags.Instance
            ).Invoke(obj, args);
        }

        object callPrivateMethod(object obj, string methodName, params object[] args)
        {
            return obj.GetType().GetMethod(
                methodName,
                BindingFlags.NonPublic | BindingFlags.Instance
            ).Invoke(obj, args);
        }

        object getPrivateField(object obj, string fieldName)
        {
            return obj
                .GetType()
                .GetField(
                    fieldName,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static
                )
                .GetValue(obj);
        }

        public void Shutdown() { }
    }
}
