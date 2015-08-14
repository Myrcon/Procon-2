﻿#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
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
using NuGet;
using Potato.Service.Shared.Packages;

namespace Potato.Service.Shared.Test.TestServicePackages.Mocks {
    public class MockPackageManagerDispatch : IPackageManagerDispatch {
        public bool DispatchedInstallPackage { get; set; }
        public bool DispatchedUpdatePackage { get; set; }
        public bool DispatchedUninstallPackage { get; set; }

        public MockPackageManagerDispatch() {
            DispatchedInstallPackage = false;
            DispatchedUpdatePackage = false;
            DispatchedUninstallPackage = false;
        }

        public void InstallPackage(IPackageManager manager, IPackage package) {
            DispatchedInstallPackage = true;
        }

        public void UpdatePackage(IPackageManager manager, IPackage package) {
            DispatchedUpdatePackage = true;
        }

        public void UninstallPackage(IPackageManager manager, IPackage package) {
            DispatchedUninstallPackage = true;
        }
    }
}
