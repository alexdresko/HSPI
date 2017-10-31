using Hspi.HspiPlugin3;
using Hspi.HspiPlugin3.Events;

namespace HSPIPluginC.Sample
{
    public class EventContainer : EventContainerBase
    {
        
        public EventContainer(HspiBase3 root) : base(root)
        {
        }

        
        public override void HandleAudioEvent(AudioEventArgs audioEventArgs)
        {
        }

        
        public override void HandleCallerIdEvent(CallerIdEventArgs callerIdEventArgs)
        {
        }

        
        public override void HandleConfigChangeEvent(ConfigChangeArgs configChangeArgs)
        {
        }

        
        public override void HandleGenericEvent(GenericEventArgs genericEventArgs)
        {
        }

        
        public override void HandleLogEvent(LogEventArgs logEventArgs)
        {
        }

        
        public override void HandleSetupChangeEvent(SetupChangeEventArgs setupChangeEventArgs)
        {
        }

        
        public override void HandleSpeakerConnectEvent(SpeakerConnectEventArgs speakerConnectEventArgs)
        {
        }

        
        public override void HandleStringChangeEvent(StringChangeEventArgs stringChangeEventArgs)
        {
        }

        
        public override void HandleValueChangeEvent(ValueChangeEventArgs valueChangeEventArgs)
        {
        }

        
        public override void HandleValueSetEvent(ValueSetEventArgs valueSetEventArgs)
        {
        }

        
        public override void HandleVoiceRecordEvent(VoiceRecordEventArgs voiceRecordEventArgs)
        {
        }

        
        public override bool RegisterForAudioEvent()
        {
            return true;

        }

        
        public override bool RegisterForCallerIdEvent()
        {
            return true;
        }

        
        public override bool RegisterForConfigChangeEvent()
        {
            return true;

        }

        
        public override bool RegisterForGenericEvent()
        {
            return true;

        }

        
        public override bool RegisterForLogEvent()
        {
            return true;

        }

        
        public override bool RegisterForSetupChangeEvent()
        {
            return true;

        }

        
        public override bool RegisterForSpeakerConnectEvent()
        {
            return true;

        }

        
        public override bool RegisterForStringChangeEvent()
        {
            return true;

        }

        
        public override bool RegisterForValueChangedEvent()
        {
            return true;

        }

        
        public override bool RegisterForValueSetEvent()
        {
            return false;
        }

        
        public override bool RegisterForVoiceRecordEvent()
        {
            return true;

        }
    }
}