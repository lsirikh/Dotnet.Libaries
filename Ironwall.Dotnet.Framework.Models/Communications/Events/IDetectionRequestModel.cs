﻿using Ironwall.Dotnet.Framework.Models.Events;

namespace Ironwall.Dotnet.Framework.Models.Communications.Events
{
    public interface IDetectionRequestModel : IBaseMessageModel
    {
        DetectionEventModel Body { get; set; }

    }
}