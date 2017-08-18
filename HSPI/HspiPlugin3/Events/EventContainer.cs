namespace Hspi.HspiPlugin3.Events
{
    public abstract class EventContainerBase
    {
        protected EventContainerBase(HspiBase3 root)
        {
            Root = root;
        }

        public abstract void HandleAudioEvent(AudioEventArgs audioEventArgs);

        public abstract void HandleCallerIdEvent(CallerIdEventArgs callerIdEventArgs);

        public abstract void HandleConfigChangeEvent(ConfigChangeArgs configChangeArgs);

        public abstract void HandleGenericEvent(GenericEventArgs genericEventArgs);

        public abstract void HandleLogEvent(LogEventArgs logEventArgs);

        public abstract void HandleSetupChangeEvent(SetupChangeEventArgs setupChangeEventArgs);

        public abstract void HandleSpeakerConnectEvent(SpeakerConnectEventArgs speakerConnectEventArgs);

        public abstract void HandleStringChangeEvent(StringChangeEventArgs stringChangeEventArgs);

        public abstract void HandleValueChangeEvent(ValueChangeEventArgs valueChangeEventArgs);

        public abstract void HandleValueSetEvent(ValueSetEventArgs valueSetEventArgs);

        public abstract void HandleVoiceRecordEvent(VoiceRecordEventArgs voiceRecordEventArgs);

        public abstract bool RegisterForAudioEvent();

        public abstract bool RegisterForCallerIdEvent();

        public abstract bool RegisterForConfigChangeEvent();

        public abstract bool RegisterForGenericEvent();

        public abstract bool RegisterForLogEvent();

        public abstract bool RegisterForSetupChangeEvent();

        public abstract bool RegisterForSpeakerConnectEvent();

        public abstract bool RegisterForStringChangeEvent();

        public abstract bool RegisterForValueChangedEvent();

        public abstract bool RegisterForValueSetEvent();

        public abstract bool RegisterForVoiceRecordEvent();

        public HspiBase3 Root { get; }
    }
}