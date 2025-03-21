using Ironwall.Dotnet.Framework.Models.Events;
using Ironwall.Libraries.Enums;

namespace Ironwall.Dotnet.Framework.Models.Communications.Events
{
    public interface IModeWindyRequestModel : IBaseMessageModel
    {
        EnumWindyMode ModeWindy { get; set; }
    }
}