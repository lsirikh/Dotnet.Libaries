﻿using Ironwall.Dotnet.Framework.Models.Devices;
using System.Collections.Generic;

namespace Ironwall.Dotnet.Framework.Models.Communications.Devices
{
    public interface ICameraMappingSaveResponseModel : IResponseModel
    {
        List<CameraMappingModel> Body { get; set; }
    }
}