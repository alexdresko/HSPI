using HomeSeerAPI;

namespace Hspi.HspiPlugin3
{
    public abstract class Action
    {
        public int Uid { get; set; }

        public abstract string GetName();

        public abstract int GetId();

        public abstract string BuildUi(string uniqueControlId,
            IPlugInAPI.strTrigActInfo actionInfo,
            TreeNodeCollection<Action> action);

        public abstract bool ActionConfigured();

        public abstract string FormatUI();

        public abstract bool ReferencesDevice(int deviceId);
    }
}