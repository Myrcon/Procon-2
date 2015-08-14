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
using System.Net;
using NUnit.Framework;
using Potato.Net.Shared.Utils;

namespace Potato.Net.Shared.Test.Utils {
    [TestFixture]
    public class NetworkTest {
        [Test]
        public void TestGetExternalIpAddressPingLocalhost() {
            var ip = Network.GetExternalIpAddress("localhost");

            Assert.IsTrue(ip.ToString().Equals("127.0.0.1") || ip.ToString().Equals("::1"));
        }
    }
}
