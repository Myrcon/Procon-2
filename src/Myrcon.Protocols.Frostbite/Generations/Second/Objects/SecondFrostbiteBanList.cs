﻿#region Copyright
// Copyright 2015 Geoff Green.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using Myrcon.Protocols.Frostbite.Objects;
using Potato.Net.Shared.Models;

namespace Myrcon.Protocols.Frostbite.Generations.Second.Objects {
    [Serializable]
    public class SecondFrostbiteBanList {

        public static List<BanModel> Parse(List<string> words) {
            var bans = new List<BanModel>();

            for (var i = 0; i < words.Count; i += 6) {
                var banWords = words.GetRange(i, 6);
                banWords.RemoveAt(4);
                bans.Add(FrostbiteBan.ParseBanListItem(banWords));
            }

            return bans;
        }
    }
}
