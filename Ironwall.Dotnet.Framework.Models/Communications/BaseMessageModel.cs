using Ironwall.Dotnet.Framework.Enums;
using Ironwall.Dotnet.Framework.Models.Utils;
using Newtonsoft.Json;
using System;
using System.ComponentModel.Design;

namespace Ironwall.Dotnet.Framework.Models.Communications;

public class BaseMessageModel : IBaseMessageModel
{
    public BaseMessageModel()
    {
        Id = IdGenTool.GenIdCode();
        DataTime = DateTime.Now;
    }

    public BaseMessageModel(EnumCmdType cmd) : this()
    {
        Command = cmd;
    }

    public BaseMessageModel(IBaseMessageModel model) : this()
    {
        Id = model.Id;
        Command = model.Command;
    }

    public BaseMessageModel(string? id, EnumCmdType command = default, DateTime? dateTime = null) : this()
    {
        Id = id ?? IdGenTool.GenIdCode();
        Command = command;
        DataTime = dateTime ?? DateTime.Now;
    }
    
    [JsonProperty("id", Order = 0)]
    public string Id { get; set; }

    [JsonProperty("command", Order = 1)]
    public EnumCmdType Command { get; set; }

    [JsonProperty("time", Order = 99)]
    public DateTime? DataTime { get; set; }

}



