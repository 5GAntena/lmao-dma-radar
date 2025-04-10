using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eft_dma_shared.Common.DMA;
using eft_dma_shared.Common.ESP;
using eft_dma_shared.Common.Features;
using eft_dma_shared.Common.Misc;
using eft_dma_shared.Common.Players;
using eft_dma_shared.Common.Ballistics;
using eft_dma_shared.Common.Unity;
using eft_dma_shared.Common.Unity.Collections;
using eft_dma_radar.Tarkov.GameWorld;
using eft_dma_radar.Tarkov.Features;
using eft_dma_shared.Common.DMA.ScatterAPI;
using System.Linq.Expressions;
using System.Runtime.Intrinsics.Arm;
using eft_dma_radar;
using eft_dma_radar.UI.Misc;
using static LonesEFTRadar.Tarkov.Features.MemoryWrites.AudioMuter;
using Microsoft.AspNetCore.Identity;

namespace LonesEFTRadar.Tarkov.Features.MemoryWrites
{
    public sealed class AudioMuter : MemWriteFeature<AudioMuter>
    {
        private static ulong AudioInstance => Memory.ReadPtr(MonoLib.Singleton.FindOne("BetterAudio") + 0x0, false);

        private static AudioMutedConfig Config { get; } = Program.Config.MemWrites.AudioMuter;

        private Dictionary<string, (bool lastState, float lastVolume)> _previousStates = new();

        private BetterAudio betterAudio;

        public override bool Enabled
        {
            get => Config.AudioCheck;
            set => Config.AudioCheck = value;
        }

        public override void TryApply(ScatterWriteHandle writes)
        {
            try
            {
                if (Enabled && Memory.InRaid)
                {
                    AudioInstance.ThrowIfInvalidVirtualAddress();

                    betterAudio ??= new BetterAudio(AudioInstance);

                    if (Config.UserFxOptions.Count == 0)
                    {
                        InitializeConfig();
                        LoneLogging.WriteLine($"Audio Strings read and set OK");
                        return;
                    }

                    if (!ApplyAudioSettings(writes))
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                LoneLogging.WriteLine($"ERROR configuring AudioMuter: {ex}");
            }
        }

        private void InitializeConfig()
        {
            foreach (var audioStr in betterAudio.AudioMixerDataS.GetAudioMixerStrings())
            {
                Config.UserFxOptions[audioStr] = new FxData(false, -80f);
            }
        }

        private bool ApplyAudioSettings(ScatterWriteHandle writes)
        {
            bool anyChangesMade = false;

            if (Config.UserFxOptions.Values.Any(fx => fx.FxState))
            {
                foreach (var kvp in Config.UserFxOptions)
                {
                    string fxKey = kvp.Key;
                    FxData current = kvp.Value;

                    if (!_previousStates.TryGetValue(fxKey, out var previous) || previous.lastState != current.FxState || Math.Abs(previous.lastVolume - current.FxVolume) > 0.0001f)
                    {
                        if (current.FxState)
                        {
                            betterAudio.Master.Internal.SetFloat(kvp.Key, kvp.Value.FxVolume, writes);
                            anyChangesMade = true;
                        }

                        _previousStates[fxKey] = (current.FxState, current.FxVolume);
                    }
                }

                if (anyChangesMade)
                {
                    writes.Callbacks += () =>
                    {
                        LoneLogging.WriteLine("Sound volumes Set");
                    };
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public class BetterAudio
        {
            public class AudioMixerData
            {
                private readonly ulong _address;

                public AudioMixerData(ulong address)
                {
                    _address = address;
                }

                public string GunsMixerVolume => GetFxKeyOrMemory("GunsVolume", 0x10);
                public string MainMixerVolume => GetFxKeyOrMemory("MainVolume", 0x18);
                public string InGameVolumeMixer => GetFxKeyOrMemory("InGame", 0x20);
                public string MusicVolumeMixer => GetFxKeyOrMemory("MusicVolume", 0x28);
                public string ReverbMixerVolume => GetFxKeyOrMemory("ReverbVolume", 0x30);
                public string AmbientOutDayMixerVolume => GetFxKeyOrMemory("AmbientOutDayVolume", 0x38);
                public string AmbientOutNightMixerVolume => GetFxKeyOrMemory("AmbientOutNightVolume", 0x40);
                public string AmbientOutMixerVolume => GetFxKeyOrMemory("AmbientOutVolume", 0x48);
                public string AmbientOutDayEffectsVolume => GetFxKeyOrMemory("AmbientOutDayEffectsVolume", 0x50);
                public string AmbientOutNightEffectsVolume => GetFxKeyOrMemory("AmbientOutNightEffectsVolume", 0x58);
                public string AmbientInMixerVolume => GetFxKeyOrMemory("AmbientInVolume", 0x60);
                public string AmbientInDayMixerVolume => GetFxKeyOrMemory("AmbientInDayVolume", 0x68);
                public string AmbientInNightMixerVolume => GetFxKeyOrMemory("AmbientInNightVolume", 0x70);
                public string WorldMixerVolume => GetFxKeyOrMemory("WorldVolume", 0x78);
                public string WorldMixerLowpassFreq => GetFxKeyOrMemory("WorldLowpassFreq", 0x80);
                public string WorldMixerReverbLevel => GetFxKeyOrMemory("WorldReverbLevel", 0x88);
                public string GunsMixerTinnitusSendLevel => GetFxKeyOrMemory("Tinnitus1", 0x90);
                public string MainMixerTinnitusSendLevel => GetFxKeyOrMemory("Tinnitus2", 0x98);
                public string AmbientMixerOcclusionSendLevel => GetFxKeyOrMemory("AmbientOccluded", 0xA0);
                public string AmbientOutNightEffectsSendLevel => GetFxKeyOrMemory("AmbientOutNightEffectsSend", 0xA8);
                public string AmbientOutDayEffectsSendLevel => GetFxKeyOrMemory("AmbientOutDayEffectsSend", 0xB0);
                public string AmbientOutReverbSendLevel => GetFxKeyOrMemory("AmbientOutReverbSend", 0xB8);
                public string AmbientOutDelaySendLevel => GetFxKeyOrMemory("AmbientOutDelaySend", 0xC0);
                public string OutEnvironmentVolume => GetFxKeyOrMemory("OutEnvironmentVolume", 0xC8);
                public string OutEnvironmentLowpassFreq => GetFxKeyOrMemory("BunkerLowpass", 0xD0);
                public string AmbientOutLowpassFreq => GetFxKeyOrMemory("AmbientOutLowpass", 0xD8);
                public string AmbientEffectsHighpassFreq => GetFxKeyOrMemory("AmbientOutHighpass", 0xE0);
                public string AmbientEffectsLowpassFreq => GetFxKeyOrMemory("AmbientEffectsLowpass", 0xE8);

                private string GetFxKeyOrMemory(string fxKey, uint offset)
                {
                    return Config.UserFxOptions.ContainsKey(fxKey)
                        ? fxKey
                        : Memory.ReadUnityString(Memory.ReadPtr(_address + offset, false));
                }

                public List<string> GetAudioMixerStrings()
                {
                    List<string> values = new List<string>();

                    var properties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

                    foreach (var property in properties)
                    {
                        if (property.PropertyType == typeof(string))
                        {
                            string value = (string)property.GetValue(this);
                            values.Add(value);
                        }
                    }

                    return values;
                }
            }

            public class Mixer
            {
                private readonly ulong _address;

                public Mixer(ulong address)
                {
                    _address = address;
                }

                public AudioMixer Internal => new AudioMixer(Memory.ReadPtr(_address + 0x10, false));
            }

            private readonly ulong _address;

            public BetterAudio(ulong address)
            {
                _address = address;
            }

            public Mixer Master => new Mixer(Memory.ReadPtr(_address + 0xb0, false));
            public AudioMixerData AudioMixerDataS => new AudioMixerData(Memory.ReadPtr(_address + 0x1f0, false));
        }

        public class AudioMixer
        {
            public class AudioMixerConstant
            {
                private readonly ulong _address;

                public AudioMixerConstant(ulong address)
                {
                    _address = address;
                }

                public uint ExposedParameterCount => Memory.ReadValue<uint>(_address + 0x80, false);
                public uint ExposedParameterNames => Memory.ReadValue<uint>(_address + 0x88, false);

                public uint GetExposedPropertyIndex(string name)
                {
                    Crc32.ProcessBlock(name);

                    var exposedParameterCount = this.ExposedParameterCount;

                    if (exposedParameterCount == 0)
                    {
                        return 0xFFFFFFFF;
                    }

                    var listBegin = _address + 0x88 + this.ExposedParameterNames;

                    for (uint i = 0; i < exposedParameterCount; i++)
                    {
                        var itemCrc32 = Memory.ReadValue<uint>(listBegin + (i * sizeof(uint)));

                        if (~Crc32.rem != itemCrc32)
                        {
                            continue;
                        }

                        return i;
                    }

                    return 0xFFFFFFFF;
                }
            }

            public class AudioMixerMemory
            {
                private readonly ulong _address;

                public AudioMixerMemory(ulong address)
                {
                    _address = address;
                }

                public ulong ExposedValues => Memory.ReadPtr(_address + 0x48, false);

                public float GetExposedProperty(uint index)
                {
                    return Memory.ReadValue<float>(this.ExposedValues + index * sizeof(float), false);
                }

                public void SetExposedProperty(uint index, float value, ScatterWriteHandle writes)
                {
                    writes.AddValueEntry(this.ExposedValues + index * sizeof(float), value);
                }
            }

            private readonly ulong _address;

            public AudioMixer(ulong address)
            {
                _address = address;
            }

            public AudioMixerConstant MixerConstant => new AudioMixerConstant(Memory.ReadPtr(_address + 0x68, false));

            public AudioMixerMemory MixerMemory => new AudioMixerMemory(Memory.ReadPtr(_address + 0x70, false));

            public float GetFloat(uint index)
            {
                return this.MixerMemory.GetExposedProperty(index);
            }

            public float GetFloat(string name)
            {
                var index = this.MixerConstant.GetExposedPropertyIndex(name);
                if (index == 0xFFFFFFFF)
                    return float.MinValue;

                return this.GetFloat(index);
            }

            public void SetFloat(uint index, float value, ScatterWriteHandle writes)
            {
                this.MixerMemory.SetExposedProperty(index, value, writes);
            }

            public void SetFloat(string name, float value, ScatterWriteHandle writes)
            {
                var index = this.MixerConstant.GetExposedPropertyIndex(name);

                if (index == 0xFFFFFFFF)
                    return;

                this.SetFloat(index, value, writes);
            }
        }

        private static class Crc32
        {
            private static readonly uint[] table = new uint[256];
            private static bool _isInitialized = false;
            public static uint rem;

            private static void InitializeTable()
            {
                byte v1 = 0;
                byte v5;
                do
                {
                    uint v2 = 0;
                    byte v3 = 0x80;
                    do
                    {
                        int v4 = (int)v2 ^ unchecked((int)0x80000000);
                        v5 = v3;
                        if ((v1 & v3) == 0)
                            v4 = (int)v2;

                        v2 = (uint)((2 * v4) ^ 0x4C11DB7);
                        if (v4 >= 0)
                            v2 = (uint)(2 * v4);

                        v3 >>= 1;
                    }
                    while (v5 >= 2);

                    uint v6 = ((2 * v2) ^ ((2 * v2) ^ (v2 >> 1)) & 0x55555555) >> 2;
                    int v7 = (int)(4 * ((2 * v2) ^ ((2 * v2) ^ (v2 >> 1)) & 0x55555555));
                    uint v8 = (16 * (uint)(v7 ^ (v7 ^ (int)v6) & 0x33333333)) ^ ((16 * (uint)(v7 ^ (v7 ^ (int)v6) & 0x33333333)) ^ ((uint)(v7 ^ (v7 ^ (int)v6) & 0x33333333) >> 4)) & 0xF0F0F0F;

                    int tableIndex = (byte)((65793 * ((32800 * v1) & 0x88440 | (2050 * v1) & 0x22110)) >> 16);
                    table[tableIndex] = __ROL4__((v8 << 8) ^ ((v8 << 8) ^ (v8 >> 8)) & 0xFF00FF, 16);

                    ++v1;
                }
                while (v1 != 0); // Loop until overflow back to 0

                _isInitialized = true;
            }

            public static void ProcessBlock(byte[] data, int beginIndex, int endIndex)
            {
                if (!_isInitialized)
                {
                    InitializeTable();
                }

                rem = 0xFFFFFFFF;

                int length = endIndex - beginIndex;
                int currentIndex = beginIndex;

                if (beginIndex > endIndex)
                    length = 0;

                if (length > 0)
                {
                    do
                    {
                        byte currentByte = data[currentIndex++];
                        byte index = (byte)((rem & 0xFF) ^ currentByte);
                        rem >>= 8;
                        rem ^= table[index];
                    }
                    while (currentIndex - beginIndex < length);
                }
            }

            public static void ProcessBlock(string text)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(text);
                ProcessBlock(bytes, 0, bytes.Length);
            }

            private static Dictionary<Type, object> RolByType = new Dictionary<Type, object>();

            private static T __ROL__<T>(T value, int count)
                where T : struct
            {
                // Lookup in cache first.
                Func<T, int, T> rol;
                object tmp;
                if (!RolByType.TryGetValue(typeof(T), out tmp))
                {
                    // Prepare variables and parameters.
                    var nbits = Marshal.SizeOf(typeof(T)) * 8;
                    var valParam = Expression.Parameter(typeof(T));
                    var countParam = Expression.Parameter(typeof(int));
                    var lowHighVar = Expression.Variable(typeof(T));

                    rol = Expression.Lambda<Func<T, int, T>>(
                        Expression.Block(new[] { lowHighVar },
                            // if (count > 0)
                            Expression.IfThenElse(Expression.GreaterThan(countParam, Expression.Constant(0)),
                                // {
                                Expression.Block(
                                    // count %= nbits;
                                    Expression.ModuloAssign(countParam, Expression.Constant(nbits)),
                                    // T high = value >> (nbits - count);
                                    Expression.Assign(lowHighVar,
                                        Expression.RightShift(valParam,
                                            Expression.Subtract(Expression.Constant(nbits), countParam))),
                                    // if ( T(-1) < 0 ) // signed value
                                    Expression.IfThen(Expression.LessThan(Expression.Convert(Expression.Constant(-1), typeof(T)), Expression.Constant(default(T))),
                                        // high &= ~((T(-1) << count));
                                        Expression.AndAssign(lowHighVar, Expression.Not(
                                            Expression.LeftShift(Expression.Convert(Expression.Constant(-1), typeof(T)), countParam)))),
                                    // value <<= count;
                                    Expression.LeftShiftAssign(valParam, countParam)
                                    ),
                                // }
                                // else
                                // {
                                Expression.Block(
                                    // count = -count % nbits;
                                    Expression.Assign(countParam, Expression.Modulo(Expression.Negate(countParam), Expression.Constant(nbits))),
                                    // T low = value << (nbits - count);
                                    Expression.Assign(lowHighVar, Expression.LeftShift(valParam, Expression.Subtract(Expression.Constant(nbits), countParam))),
                                    // value >>= count;
                                    Expression.RightShiftAssign(valParam, countParam)
                                    )
                                // }
                                ),
                                // return value | lowOrHigh;
                                Expression.Or(valParam, lowHighVar)
                            ), valParam, countParam).Compile();

                    RolByType.Add(typeof(T), rol);
                }
                else
                {
                    rol = (Func<T, int, T>)tmp;
                }

                return rol(value, count);
            }

            public static byte __ROL1__(byte value, int count) { return __ROL__(value, count); }
            public static ushort __ROL2__(ushort value, int count) { return __ROL__(value, count); }
            public static uint __ROL4__(uint value, int count) { return __ROL__(value, count); }
            public static ulong __ROL8__(ulong value, int count) { return __ROL__(value, count); }
            public static byte __ROR1__(byte value, int count) { return __ROL__(value, -count); }
            public static ushort __ROR2__(ushort value, int count) { return __ROL__(value, -count); }
            public static uint __ROR4__(uint value, int count) { return __ROL__(value, -count); }
            public static ulong __ROR8__(ulong value, int count) { return __ROL__(value, -count); }
        }
    }
}
