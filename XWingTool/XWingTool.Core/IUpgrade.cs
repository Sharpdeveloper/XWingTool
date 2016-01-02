using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XWingTool.Core
{
    public interface IUpgrade
    {
        int Count { get; set; }
        string Name { get; set; }
        string Gername { get; set; }
        int Id { get; set; }
        string UpgradeSlot { get; set; }
        List<string> Sources { get; set; }
        int Points { get; set; }
        bool Unique { get;  set; }
        List<string> GetText();
    }
}
