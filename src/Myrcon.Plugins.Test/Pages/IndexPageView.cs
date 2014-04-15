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

// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 10.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Myrcon.Plugins.Test.Pages
{
    using System;
    
    
    #line 1 "C:\Users\P\Documents\Projects\clients\myrcon\Potato\Potato-2\src\Myrcon.Plugins.Test\Pages\IndexPageView.tt"
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
    public partial class IndexPageView : IndexPageViewBase
    {
        public virtual string TransformText()
        {
            this.Write("<html>\r\n\t<head>\r\n\t\t<title>This is my title</title>\r\n\t</head>\r\n\t<body>\r\n\t\t<ul clas" +
                    "s=\"nav nav-tabs\" ng-controller=\"MenuController\">\r\n\t\t  <li ng-class=\"{ \'active\': " +
                    "isActive(\'/\') }\"><a href=\"#/\">Home</a></li>\r\n\t\t  <li ng-class=\"{ \'active\': isAct" +
                    "ive(\'/about\') }\"><a href=\"#/about\">About</a></li>\r\n\t\t  <li ng-class=\"{ \'active\':" +
                    " isActive(\'/widget/overview\') }\"><a href=\"#/widget/overview\">Widget/Overview</a>" +
                    "</li>\r\n\t\t  <li ng-class=\"{ \'active\': isActive(\'/widget/settings\') }\"><a href=\"#/" +
                    "widget/settings\">Widget/Settings</a></li>\r\n\t\t  <li ng-class=\"{ \'active\': isActiv" +
                    "e(\'/widget/player\') }\"><a href=\"#/widget/player\">Widget/Player</a></li>\r\n\t\t</ul>" +
                    "\r\n\t\t<div id=\"content\">\r\n            <div ng-view></div>\r\n\r\n            <script t" +
                    "ype=\"text/ng-template\" id=\"/index.html\">\r\n                <h2>Index</h2>\r\n\t\t\t\t<p" +
                    ">Here\'s a variable from the controller: {{ SomeData }}</p>\r\n\r\n\t\t\t\t<form class=\"s" +
                    "mart-form\" name=\"multiplication\" ng-submit=\"multiplication.$valid && Submit()\">\r" +
                    "\n\t\t\t\t\t<header>\r\n\t\t\t\t\t\t<h2>Enter a number to test command architecture</h2>\r\n\t\t\t\t" +
                    "\t</header>\r\n\t\t\t\t\t<fieldset>\r\n                        <section>\r\n                " +
                    "            <label class=\"input\">\r\n                                <input class=" +
                    "\"form-control\" type=\"input\" placeholder=\"Enter a number to multiply it by 2\" nam" +
                    "e=\"number\" ng-model=\"Mathematics.Number\" required>\r\n                            " +
                    "</label>\r\n                            <em ng-show=\"multiplication.number.$dirty " +
                    "&& multiplication.number.$invalid && multiplication.number.$error.required\" clas" +
                    "s=\"ng-hide\">Please enter a number to multiply</em>\r\n                        </se" +
                    "ction>\r\n                    </fieldset>\r\n\t\t\t\t\t<footer>\r\n                        " +
                    "<button class=\"btn btn-primary\" ng-disabled=\"MathematicsActions.Communicating ||" +
                    " multiplication.$invalid\" disabled=\"disabled\">\r\n                            <img" +
                    " ng-show=\"MathematicsActions.Communicating\" src=\"/assets/img/loading.gif\" class=" +
                    "\"ng-hide\"><i ng-hide=\"MathematicsActions.Communicating\" class=\"fa fa-arrow-right" +
                    "\"></i> Multiply\r\n                        </button>\r\n                    </footer" +
                    ">\r\n\t\t\t\t</form>\r\n\t\t\t\t\r\n\t\t\t\t<h3>Here\'s the result of the multiplication: {{ Mathem" +
                    "atics.Result }}</h3>\r\n\t\t\t\t<div ng-if=\"Mathematics.Result != 0\">\r\n\t\t\t\t\t<p>The pro" +
                    "cess this just went through:</p>\r\n\t\t\t\t\t<ul>\r\n\t\t\t\t\t\t<li>Angular.Submit()</li>\r\n\t\t" +
                    "\t\t\t\t<li>Through the sandbox between Potato.UI Window and this plugins iframe wit" +
                    "h postMessage</li>\r\n\t\t\t\t\t\t<li>Emitted via websocket to the Potato UI</li>\r\n\t\t\t\t\t" +
                    "\t<li>Credentials attached and the command, sent to a running Potato 2 C# Instanc" +
                    "e</li>\r\n\t\t\t\t\t\t<li>Deserialized by Potato 2, routed to a specific connection</li>" +
                    "\r\n\t\t\t\t\t\t<li>Crosses the sandbox into the Plugin AppDomain</li>\r\n\t\t\t\t\t\t<li>Routes" +
                    " the command \"TestPluginSimpleMultiplyByTwoCommand\" at https://github.com/Myrcon" +
                    "/Potato-2/blob/master/src/Myrcon.Plugins.Test/Tests/TestPluginsWebUi.cs </li>\r\n\t" +
                    "\t\t\t\t\t<li>Multiplies the number.</li>\r\n\t\t\t\t\t\t<li>Reverse of above.</li>\r\n\t\t\t\t\t</u" +
                    "l>\r\n\t\t\t\t</div>\r\n            </script>\r\n\r\n            <script type=\"text/ng-templ" +
                    "ate\" id=\"/about.html\">\r\n                <h2>About</h2>\r\n\t\t\t\t<p>This plugin is ma" +
                    "de by Geoff to demonstrate making a basic plugin UI.</p>\r\n            </script>\r" +
                    "\n\r\n            <script type=\"text/ng-template\" id=\"/widget/overview.html\">\r\n    " +
                    "            <h2>This is a plugin panel generated from the C# instance</h2>\r\n\t\t\t\t" +
                    "<p>You can see this text being pulled from Potato at https://github.com/Myrcon/P" +
                    "rocon-2/blob/master/src/Myrcon.Plugins.Test/Pages/IndexPageView.tt </p>\r\n\t\t\t\t<p>" +
                    "Panels like this will be displayed around Potato. It\'s entirely up to the plugin" +
                    " developer what content and functionality shows up in this little panel.</p>\r\n  " +
                    "          </script>\r\n\r\n            <script type=\"text/ng-template\" id=\"/widget/s" +
                    "ettings.html\">\r\n                <h2>This is a plugin panel generated from the C#" +
                    " instance</h2>\r\n\t\t\t\t<p>You can see this text being pulled from Potato at https:/" +
                    "/github.com/Myrcon/Potato-2/blob/master/src/Myrcon.Plugins.Test/Pages/IndexPageV" +
                    "iew.tt </p>\r\n\t\t\t\t<p>Potato 2 is very broad on its protocol. This panel could be " +
                    "used to display game-specific functionality of settings</p>\r\n            </scrip" +
                    "t>\r\n\r\n            <script type=\"text/ng-template\" id=\"/widget/player.html\">\r\n   " +
                    "             <h2>This is a plugin panel generated from the C# instance</h2>\r\n\t\t\t" +
                    "\t<p>You can see this text being pulled from Potato at https://github.com/Myrcon/" +
                    "Potato-2/blob/master/src/Myrcon.Plugins.Test/Pages/IndexPageView.tt </p>\r\n\t\t\t\t<p" +
                    ">This panel could be used to display additional infromation, statistics etc. abo" +
                    "ut this particular player.</p>\r\n            </script>\r\n\t\t</div>\r\n\t</body>\r\n</htm" +
                    "l>");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "10.0.0.0")]
    public class IndexPageViewBase
    {
        #region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
        #endregion
        #region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected System.Text.StringBuilder GenerationEnvironment
        {
            get
            {
                if ((this.generationEnvironmentField == null))
                {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set
            {
                this.generationEnvironmentField = value;
            }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors
        {
            get
            {
                if ((this.errorsField == null))
                {
                    this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errorsField;
            }
        }
        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private System.Collections.Generic.List<int> indentLengths
        {
            get
            {
                if ((this.indentLengthsField == null))
                {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent
        {
            get
            {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
            }
        }
        #endregion
        #region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0) 
                        || this.endsWithNewline))
            {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0))
            {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline)
            {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            }
            else
            {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if ((indent == null))
            {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if ((this.indentLengths.Count > 0))
            {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0))
                {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
        #endregion
        #region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public System.IFormatProvider FormatProvider
            {
                get
                {
                    return this.formatProviderField ;
                }
                set
                {
                    if ((value != null))
                    {
                        this.formatProviderField  = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if ((objectToConvert == null))
                {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                System.Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
                            typeof(System.IFormatProvider)});
                if ((method == null))
                {
                    return objectToConvert.ToString();
                }
                else
                {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                                this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        public ToStringInstanceHelper ToStringHelper
        {
            get
            {
                return this.toStringHelperField;
            }
        }
        #endregion
    }
    #endregion
}
