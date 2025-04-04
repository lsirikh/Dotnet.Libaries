﻿using Ironwall.Dotnet.Framework.Models.Events;
using Ironwall.Libraries.Enums;
using Ironwall.Redis.Message.Framework;
using Newtonsoft.Json;
using System;

namespace Ironwall.Dotnet.Framework.Models.Communications.Events
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 6/25/2024 10:36:49 AM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class ContactOffRequestModel : BaseEventMessageModel, IContactOffRequestModel
    {
        public ContactOffRequestModel()
        {
        }

        public ContactOffRequestModel(IContactEventModel model)
            : base(EnumCmdType.EVENT_CONTACT_OFF_REQUEST)
        {
            Body = model as ContactEventModel;
        }

        [JsonProperty("body", Order = 6)]
        public ContactEventModel Body { get; set; }
    }
}