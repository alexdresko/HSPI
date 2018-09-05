using System;
using System.Collections.Generic;
using System.Linq;
using HomeSeerAPI;
using Scheduler.Classes;

namespace Hspi.HspiPlugin3.Events
{
    public class EventProcessor
    {
        private readonly EventContainerBase _eventContainerBase;

        private readonly IAppCallbackAPI _callback;

        private readonly string _name;

        public EventProcessor(EventContainerBase eventContainerBase, IAppCallbackAPI callback, string name)
        {
            _eventContainerBase = eventContainerBase;
            _callback = callback;
            _name = name;
        }

        public void Handle(Enums.HSEvent eventType, object[] parameters)
        {
            var strings = parameters.Select(p => p.ToString()).ToList();
            switch (eventType)
            {
                case Enums.HSEvent.AUDIO:
                    _eventContainerBase.HandleAudioEvent(ParseAudioEvent(strings));
                    break;
                case Enums.HSEvent.LOG:
                    _eventContainerBase.HandleLogEvent(ParseLogEvent(strings));
                    break;
                case Enums.HSEvent.CONFIG_CHANGE:
                    _eventContainerBase.HandleConfigChangeEvent(ParseConfigChangeEvent(strings));
                    break;
                case Enums.HSEvent.STRING_CHANGE:
                    _eventContainerBase.HandleStringChangeEvent(ParseStringChangeEvent(strings));
                    break;
                case Enums.HSEvent.SPEAKER_CONNECT:
                    _eventContainerBase.HandleSpeakerConnectEvent(ParseSpeakerConnectEvent(strings));
                    break;
                case Enums.HSEvent.CALLER_ID:
                    _eventContainerBase.HandleCallerIdEvent(ParseCallerIdEvent(strings));
                    break;
                case Enums.HSEvent.VALUE_CHANGE:
                    _eventContainerBase.HandleValueChangeEvent(ParseValueChangeEvent(strings));
                    break;
                case Enums.HSEvent.VALUE_SET:
                    _eventContainerBase.HandleValueSetEvent(ParseValueSetEvent(strings));
                    break;
                case Enums.HSEvent.VOICE_REC:
                    _eventContainerBase.HandleVoiceRecordEvent(ParseVoiceRecordEvent(strings));
                    break;
                case Enums.HSEvent.SETUP_CHANGE:
                    _eventContainerBase.HandleSetupChangeEvent(ParseSetupChangeEvent(strings));
                    break;
                case Enums.HSEvent.GENERIC:
                    _eventContainerBase.HandleGenericEvent(ParseGenericEvent(strings));
                    break;
                case Enums.HSEvent.RUN_SCRIPT_SPECIAL:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
            }
        }

        private static GenericEventArgs ParseGenericEvent(List<string> strings)
        {
            return new GenericEventArgs
            {
                Event = strings[1],
                Sender = strings[2],
                Parameters = strings.Skip(2).ToList()
            };
        }

        private static SetupChangeEventArgs ParseSetupChangeEvent(List<string> strings)
        {
            throw new NotImplementedException(
                $"Investigate ParseSetupChangeEvent parameter: {string.Join("|", strings)}");
        }

        private static VoiceRecordEventArgs ParseVoiceRecordEvent(List<string> strings)
        {
            throw new NotImplementedException(
                $"Investigate ParseVoiceRecordEvent parameter: {string.Join("|", strings)}");
        }

        private static ValueSetEventArgs ParseValueSetEvent(List<string> strings)
        {
            throw new NotImplementedException(
                $"Investigate ParseValueSetEvent parameter: {string.Join("|", strings)}");
        }

        private static ValueChangeEventArgs ParseValueChangeEvent(List<string> strings)
        {
            return new ValueChangeEventArgs
            {
                Address = strings[1],
                NewValue = double.Parse(strings[2]),
                OldValue = double.Parse(strings[3]),
                DeviceReferenceNumber = int.Parse(strings[4])
            };
        }

        private static CallerIdEventArgs ParseCallerIdEvent(List<string> strings)
        {
            return new CallerIdEventArgs
            {
                Number = strings[1],
                PhoneCompanyName = strings[2],
                AddressBookLastName = strings[3],
                AddressBookFirstName = strings[4],
                AddressBookCompany = strings[5],
                Line = strings[6]
            };
        }

        private static SpeakerConnectEventArgs ParseSpeakerConnectEvent(List<string> strings)
        {
            return new SpeakerConnectEventArgs
            {
                Host = strings[1],
                Instance = strings[2],
                ConnectionStatus = strings[3] == "0" ? SpeakerConnectionStatus.Disconnect : SpeakerConnectionStatus.Connect,
                IpAddress = strings[4]
            };
        }

        private static StringChangeEventArgs ParseStringChangeEvent(List<string> strings)
        {
            throw new NotImplementedException(
                $"Investigate ParseStringChangeEvent parameter: {string.Join("|", strings)}");
        }

        private static ConfigChangeArgs ParseConfigChangeEvent(List<string> strings)
        {
            return new ConfigChangeArgs
            {
                Type = ParseConfigChangeType(strings[1]),
                Id = strings[2],
                DeviceReferenceNumber = int.Parse(strings[3]),
                Dac = ParseDac(strings[4]),
                WhatChanged = strings[5]
            };
        }

        private static Dac ParseDac(string value)
        {
            switch (value)
            {
                case "0": return Dac.NotKnown;
                case "1": return Dac.Added;
                case "2": return Dac.Deleted;
                case "3": return Dac.Changed;
                default:
                    return Dac.None;
            }
        }

        private static ConfigChangeType ParseConfigChangeType(string value)
        {
            switch (value)
            {
                case "0":
                    return ConfigChangeType.change_device;
                case "1":
                    return ConfigChangeType.change_event;
                case "2":
                    return ConfigChangeType.change_event_group;
                default:
                    return ConfigChangeType.change_setup_item;
            }
        }

        private static LogEventArgs ParseLogEvent(List<string> strings)
        {
            return new LogEventArgs
            {
                DateTime = DateTime.Parse(strings[1]),
                MessageClass = strings[2],
                Message = strings[3],
                Color = strings[4],
                Priority = strings[5],
                From = strings[6],
                ErrorCode = strings[7],
                Date = DateTime.Parse(strings[8])
            };
        }

        private static AudioEventArgs ParseAudioEvent(List<string> strings)
        {
            return new AudioEventArgs
            {
                Started = ParseBoolOrIntAsBool(strings[1]),
                Devices = ParseBoolOrIntAsInt(strings[1]),
                AudioType = strings[2] == "0" ? AudioType.TTS : AudioType.WAV,
                Host = strings[3],
                Instance = strings[4]
            };
        }

        private static int ParseBoolOrIntAsInt(string value)
        {
            var lowered = value.ToLower();

            return new[] { "true", "false" }.Contains(lowered) ? 0 : int.Parse(lowered);
        }

        private static bool ParseBoolOrIntAsBool(string value)
        {
            var lowered = value.ToLower();

            return !new[] { "true", "false" }.Contains(lowered) || bool.Parse(lowered);
        }

        public void Configure()
        {
            if (_eventContainerBase.RegisterForValueChangedEvent())
            {
                _callback.RegisterEventCB(Enums.HSEvent.VALUE_CHANGE, _name, "");
            }

            if (_eventContainerBase.RegisterForAudioEvent())
            {
                _callback.RegisterEventCB(Enums.HSEvent.AUDIO, _name, "");
            }

            if (_eventContainerBase.RegisterForCallerIdEvent())
            {
                _callback.RegisterEventCB(Enums.HSEvent.CALLER_ID, _name, "");
            }

            if (_eventContainerBase.RegisterForConfigChangeEvent())
            {
                _callback.RegisterEventCB(Enums.HSEvent.CONFIG_CHANGE, _name, "");
            }

            if (_eventContainerBase.RegisterForGenericEvent())
            {
                _callback.RegisterEventCB(Enums.HSEvent.GENERIC, _name, "");
            }

            if (_eventContainerBase.RegisterForLogEvent())
            {
                _callback.RegisterEventCB(Enums.HSEvent.LOG, _name, "");
            }

            if (_eventContainerBase.RegisterForSetupChangeEvent())
            {
                _callback.RegisterEventCB(Enums.HSEvent.SETUP_CHANGE, _name, "");
            }

            if (_eventContainerBase.RegisterForSpeakerConnectEvent())
            {
                _callback.RegisterEventCB(Enums.HSEvent.SPEAKER_CONNECT, _name, "");
            }

            if (_eventContainerBase.RegisterForStringChangeEvent())
            {
                _callback.RegisterEventCB(Enums.HSEvent.STRING_CHANGE, _name, "");
            }

            if (_eventContainerBase.RegisterForValueSetEvent())
            {
                _callback.RegisterEventCB(Enums.HSEvent.VALUE_SET, _name, "");
            }

            if (_eventContainerBase.RegisterForVoiceRecordEvent())
            {
                _callback.RegisterEventCB(Enums.HSEvent.VOICE_REC, _name, "");
            }
        }
    }
}