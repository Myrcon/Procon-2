using System;

namespace Procon.Database.Shared.Builders.Modifiers {
    /// <summary>
    /// An explcit object is appended to any object that has been explitely added
    /// to the query chain and not generated by any builder code.
    /// </summary>
    [Serializable]
    public class Explicit : Modifier {
    }
}