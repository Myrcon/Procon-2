﻿using NUnit.Framework;
using Procon.Core.Events;
using Procon.Core.Shared;
using Procon.Core.Shared.Events;

namespace Procon.Core.Test.Events {
    [TestFixture]
    public class TestEventsLog {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        /// <summary>
        /// Tests that attempting the command without any users in the security controller will
        /// result in insufficient permissions
        /// </summary>
        [Test]
        public void TestResultInsufficientPermissions() {
            EventsController events = new EventsController();

            ICommandResult result = events.Tunnel(CommandBuilder.EventsLog(new GenericEvent() {
                Name = "Nothing"
            }).SetOrigin(CommandOrigin.Remote).SetUsername("Phogue"));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
        }

        /// <summary>
        /// Tests that the packages can be fetched (or initiated a fetch) using the command with permissions.
        /// </summary>
        [Test]
        public void TestResultSuccess() {
            EventsController events = new EventsController();

            ICommandResult result = events.Tunnel(CommandBuilder.EventsLog(new GenericEvent() {
                Name = "Nothing"
            }).SetOrigin(CommandOrigin.Local));

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
        }
    }
}
