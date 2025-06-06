global using SKSvg = Svg.Skia.SKSvg;
global using SkiaSharp;
global using SkiaSharp.Views.Desktop;
global using System.ComponentModel;
global using System.Data;
global using System.Reflection;
global using System.Diagnostics;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Numerics;
global using System.Collections.Concurrent;
global using System.Net;
global using System.Runtime.CompilerServices;
global using System.Runtime.InteropServices;
global using System.Runtime.Intrinsics;
global using System.Runtime.Intrinsics.X86;
global using System.Net.Security;
global using System.Security.Cryptography;
global using System.Security.Cryptography.X509Certificates;
global using System.Collections;
global using System.Net.Http.Headers;
global using System.Buffers;
global using System.Buffers.Binary;
global using SDK;
global using eft_dma_shared;
global using eft_dma_shared.Misc;
global using eft_dma_shared.Common;
using System.Runtime.Versioning;
using arena_dma_radar;
using arena_dma_radar.UI.Radar;
using arena_dma_radar.UI.Misc;
using arena_dma_radar.Arena;
using arena_dma_radar.UI.ESP;
using eft_dma_shared.Common.Maps;
using arena_dma_radar.Arena.Features;
using eft_dma_shared.Common.Misc.Data;
using eft_dma_shared.Common.UI;

[assembly: AssemblyTitle(Program.Name)]
[assembly: AssemblyProduct(Program.Name)]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: SupportedOSPlatform("Windows")]

namespace arena_dma_radar
{
    internal static class Program
    {
        internal const string Name = "Arena DMA Radar";

        /// <summary>
        /// Global Program Configuration.
        /// </summary>
        public static Config Config { get; }

        /// <summary>
        /// Path to the Configuration Folder in %AppData%
        /// </summary>
        public static DirectoryInfo ConfigPath { get; } =
            new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "eft-dma-radar"));

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [Obfuscation(Feature = "Virtualization", Exclude = false)]
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                ConfigureProgram();
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), Program.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        #region Private Members

        static Program()
        {
            try
            {
                try
                {
                    string loneCfgPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Lones-Client");
                    if (Directory.Exists(loneCfgPath))
                    {
                        if (ConfigPath.Exists)
                            ConfigPath.Delete(true);
                        Directory.Move(loneCfgPath, ConfigPath.FullName);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR Importing Lone Config(s). Close down the radar, and try copy your config files manually from %AppData%\\LonesClient TO %AppData%\\eft-dma-radar\n\n" +
                        "Be sure to delete the Lones-Client folder when done.\n\n" +
                        $"ERROR: {ex}",
                        Program.Name,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                ConfigPath.Create();
                var config = Config.Load();
                eft_dma_shared.SharedProgram.Initialize(ConfigPath, config);
                Config = config;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), Program.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        /// <summary>
        /// Configure Program Startup.
        /// </summary>
        /// <param name="args">Args passed from entrypoint.</param>
        [Obfuscation(Feature = "Virtualization", Exclude = false)]
        [Obfuscation(Feature = "Virtualization", Exclude = false)]
        private static void ConfigureProgram()
        {
            ApplicationConfiguration.Initialize();
            using var loading = LoadingForm.Create();
            loading.UpdateStatus("Loading Tarkov.Dev Data...", 15);
            EftDataManager.ModuleInitAsync(loading).GetAwaiter().GetResult();
            loading.UpdateStatus("Loading Map Assets...", 35);
            LoneMapManager.ModuleInit();
            loading.UpdateStatus("Starting Memory Interface...", 50);
            MemoryInterface.ModuleInit();
            loading.UpdateStatus("Loading Features...", 75);
            FeatureManager.ModuleInit();
            ResourceJanitor.ModuleInit(new Action(CleanupWindowResources));
            loading.UpdateStatus("Loading Completed!", 100);
        }


        private static void CleanupWindowResources()
        {
            MainForm.Window?.PurgeSKResources();
            EspForm.Window?.PurgeSKResources();
        }

        #endregion
    }
}