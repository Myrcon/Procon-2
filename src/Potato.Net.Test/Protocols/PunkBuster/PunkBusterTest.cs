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
using NUnit.Framework;
using Potato.Net.Shared.Protocols.PunkBuster;
using Potato.Net.Shared.Protocols.PunkBuster.Packets;

namespace Potato.Net.Test.Protocols.PunkBuster {
    [TestFixture]
    public class PunkBusterTest {

        /// <summary>
        /// Tests a punkbuster player from a player list will match
        /// </summary>
        [Test]
        public void TestPunkBusterSerializerPlayer() {
            var punkBuster = PunkBusterSerializer.Deserialize("PunkBuster Server: 1  b88b60a36365592b1ae94fa04c5763ed(-) 111.222.0.1:3659 OK   1 3.0 0 (V) \"PhogueZero\"");

            Assert.IsInstanceOf<PunkBusterPlayer>(punkBuster);

            var player = punkBuster as PunkBusterPlayer;

            Assert.AreEqual(1, player.SlotId);
            Assert.AreEqual("b88b60a36365592b1ae94fa04c5763ed", player.Guid);
            Assert.AreEqual("111.222.0.1:3659", player.Ip);
            Assert.AreEqual("PhogueZero", player.Name);
        }

        /// <summary>
        /// Tests a punkbuster player list begin will match
        /// </summary>
        [Test]
        public void TestPunkBusterSerializerBeginPlayerList() {
            var punkBuster = PunkBusterSerializer.Deserialize("PunkBuster Server: Player List: [Slot #] [GUID] [Address] [Status] [Power] [Auth Rate] [Recent SS] [O/S] [Name]");

            Assert.IsInstanceOf<PunkBusterBeginPlayerList>(punkBuster);
        }

        /// <summary>
        /// Tests a punkbuster player list end will match
        /// </summary>
        [Test]
        public void TestPunkBusterSerializerEndPlayerList() {
            var punkBuster = PunkBusterSerializer.Deserialize("PunkBuster Server: End of Player List (1 Players)");

            Assert.IsInstanceOf<PunkBusterEndPlayerList>(punkBuster);

            var endPlayerList = punkBuster as PunkBusterEndPlayerList;

            Assert.AreEqual(1, endPlayerList.PlayerCount);
        }
    }
}
