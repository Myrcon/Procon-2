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
using Potato.Net.Shared.Models;

namespace Myrcon.Protocols.Frostbite.Generations.Second.Objects {
    [Serializable]
    public class SecondFrostbiteMapList {

        public static List<MapModel> Parse(List<string> words) {
            var maps = new List<MapModel>();

            if (words.Count >= 2) {
                int mapsCount = 0, wordsPerMap = 0;

                if (int.TryParse(words[0], out mapsCount) == true && int.TryParse(words[1], out wordsPerMap) == true) {

                    for (int mapOffset = 0, wordIndex = 2; mapOffset < mapsCount && wordIndex < words.Count; mapOffset++, wordIndex += wordsPerMap) {
                        var rounds = 0;
                        if (int.TryParse(words[wordIndex + 2], out rounds) == true) {
                            maps.Add(
                                new MapModel() {
                                    Index = mapOffset,
                                    Rounds = rounds == 0 ? 2 : rounds,
                                    Name = words[wordIndex],
                                    GameMode = new GameModeModel() {
                                        Name = words[wordIndex + 1]
                                    }
                                }
                            );
                        }
                    }
                }
            }

            return maps;
        }

    }
}
