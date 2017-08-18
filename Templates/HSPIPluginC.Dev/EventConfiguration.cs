using Hspi.HspiPlugin3;
using Hspi.HspiPlugin3.Events;

namespace HSPIPluginC.Dev
{
    public class EventContainer : EventContainerBase
    {
        /// <inheritdoc />
        public EventContainer(HspiBase3 root) : base(root)
        {
        }

        /// <inheritdoc />
        public override void HandleAudioEvent(AudioEventArgs audioEventArgs)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public override void HandleCallerIdEvent(CallerIdEventArgs callerIdEventArgs)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public override void HandleConfigChangeEvent(ConfigChangeArgs configChangeArgs)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public override void HandleGenericEvent(GenericEventArgs genericEventArgs)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public override void HandleLogEvent(LogEventArgs logEventArgs)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public override void HandleSetupChangeEvent(SetupChangeEventArgs setupChangeEventArgs)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public override void HandleSpeakerConnectEvent(SpeakerConnectEventArgs speakerConnectEventArgs)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public override void HandleStringChangeEvent(StringChangeEventArgs stringChangeEventArgs)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public override void HandleValueChangeEvent(ValueChangeEventArgs valueChangeEventArgs)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public override void HandleValueSetEvent(ValueSetEventArgs valueSetEventArgs)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public override void HandleVoiceRecordEvent(VoiceRecordEventArgs voiceRecordEventArgs)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public override bool RegisterForAudioEvent()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public override bool RegisterForCallerIdEvent()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public override bool RegisterForConfigChangeEvent()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public override bool RegisterForGenericEvent()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public override bool RegisterForLogEvent()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public override bool RegisterForSetupChangeEvent()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public override bool RegisterForSpeakerConnectEvent()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public override bool RegisterForStringChangeEvent()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public override bool RegisterForValueChangedEvent()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public override bool RegisterForValueSetEvent()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public override bool RegisterForVoiceRecordEvent()
        {
            throw new System.NotImplementedException();
        }
    }
}