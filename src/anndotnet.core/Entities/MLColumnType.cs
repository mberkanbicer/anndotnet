﻿//////////////////////////////////////////////////////////////////////////////////////////
// ANNdotNET - Deep Learning Tool on .NET Platform                                     //
// Copyright 2017-2020 Bahrudin Hrnjica                                                 //
//                                                                                      //
// This code is free software under the MIT License                                     //
// See license section of  https://github.com/bhrnjica/anndotnet/blob/master/LICENSE.md  //
//                                                                                      //
// Bahrudin Hrnjica                                                                     //
// bhrnjica@hotmail.com                                                                 //
// Bihac, Bosnia and Herzegovina                                                         //
// http://bhrnjica.net                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.Json.Serialization;
using Daany.Ext;

namespace Anndotnet.Core
{
    //Variable type
    public enum MLColumnType
    {
        [Description("None")]
        None, // ignore columns in modeling
        [Description("Feature")]
        Feature, //-treat column as input parameter or feature
        [Description("Label")]
        Label, // - treat column as output value or label

    }
}
