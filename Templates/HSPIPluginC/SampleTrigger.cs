using System;
using HomeSeerAPI;
using Hspi.HspiPlugin3;

namespace HSPIPluginB.Dev
{
    public class SampleTrigger : Trigger
    {
        public override int GetId()
        {
            throw new NotImplementedException();
        }

        public override bool HasConditions()
        {
            throw new NotImplementedException();
        }

        public override string GetName()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override bool Test(IPlugInAPI.strTrigActInfo actionInfo, TreeNodeCollection<Trigger> trigger)
        {
            throw new NotImplementedException();
        }
    }
}