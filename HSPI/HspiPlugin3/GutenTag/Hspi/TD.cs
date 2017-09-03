using GutenTag;
using Scheduler;

namespace Hspi.HspiPlugin3.GutenTag.Hspi
{
    public class TD : Tag
    {
        public TD(TdClass tdClass, string value = null) : base("td")
        {
            Attributes["class"] = new TagProperty(tdClass.Description());

            if (tdClass == TdClass.TableColumn)
            {
                Attributes["align"] = new TagProperty("center");    
            }

            if (value != null)
            {
                Add(value);
            }


        }
    }
}