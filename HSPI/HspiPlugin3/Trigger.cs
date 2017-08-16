using System.Collections.Generic;
using HomeSeerAPI;

namespace Hspi.HspiPlugin3
{
    public abstract class Trigger
    {
        public abstract int GetId();

        public abstract bool HasConditions();

        public abstract string GetName();

        public int Uid { get; set; }

        public bool IsCondition { get; set; }

        public abstract bool Test(IPlugInAPI.strTrigActInfo actionInfo, TreeNodeCollection<Trigger> trigger);
    }
}