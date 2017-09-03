using System.ComponentModel;

namespace Hspi.HspiPlugin3.GutenTag.Hspi
{
    public enum TdClass
    {
        [Description("tablecolumn")]
        TableColumn,
        [Description("tablecell")]
        TableCell,
        [Description("tableheader")]
        TableHeader
    }
}