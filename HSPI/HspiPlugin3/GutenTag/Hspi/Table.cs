using GutenTag;
using Scheduler;

namespace Hspi.HspiPlugin3.GutenTag.Hspi
{
    public class Table : Tag
    {
        public Table(TableClass tableClass) : base("table")
        {
            Add("class", tableClass.Description());
        }
    }
}