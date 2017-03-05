// Copyright (C) 2016 SRG Technology, LLC
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Collections.Specialized;
using Scheduler;

namespace Hspi
{
    /// <summary>
    ///     This class adds some common support functions for creating the web pages used by HomeSeer plugins.
    ///     <para />
    ///     For each control there are three functions:
    ///     <list type="bullet">
    ///         <item>
    ///             <description><c>Build:</c> Used to initially create the control in the web page.</description>
    ///         </item>
    ///         <item>
    ///             <description><c>Update:</c> Used to modify the control in an existing web page.</description>
    ///         </item>
    ///         <item>
    ///             <description><c>Form:</c> Not normally call externally but could be useful in special circumstances.</description>
    ///         </item>
    ///     </list>
    /// </summary>
    /// <seealso cref="Scheduler.PageBuilderAndMenu.clsPageBuilder" />
    public class PageBuilder : PageBuilderAndMenu.clsPageBuilder
    {
        /// <summary>
        /// </summary>
        /// <param name="pagename">The name used by HomeSeer when referencing this particular page.</param>
        public PageBuilder(string pagename) : base(pagename)
        {
        }

        /// <summary>
        ///     Build a button for a web page.
        /// </summary>
        /// <param name="text">The text on the button.</param>
        /// <param name="name">The name used to create the references for the button.</param>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        /// <returns>The text to insert in the web page to create the button.</returns>
        protected string BuildButton(string text, string name, bool enabled = true)
        {
            return "<div id='" + name + "_div'>" + FormButton(name, text, enabled: enabled) + "</div>";
        }

        /// <summary>
        ///     Update a button on a web page that was created with a DIV tag.
        /// </summary>
        /// <param name="text">The text on the button.</param>
        /// <param name="name">The name used to create the references for the button.</param>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        protected void UpdateButton(string text, string name, bool enabled = true)
        {
            divToUpdate.Add(name + "_div", FormButton(name, text, enabled: enabled));
        }

        /// <summary>
        ///     Return the string required to create a web page button.
        /// </summary>
        protected string FormButton(string name, string label = "Submit", bool submitForm = true,
            string imagePathNormal = "", string imagePathPressed = "", string toolTip = "",
            bool enabled = true, string style = "")
        {
            var b = new clsJQuery.jqButton(name, label, PageName, submitForm)
            {
                id = "o" + name,
                imagePathNormal = imagePathNormal
            };
            b.imagePathPressed = imagePathPressed == "" ? b.imagePathNormal : imagePathPressed;
            b.toolTip = toolTip;
            b.enabled = enabled;
            b.style = style;

            var button = b.Build();
            button = button.Replace("</button>\r\n", "</button>").Trim();
            return button;
        }

        /// <summary>
        ///     Build a label for a web page.
        /// </summary>
        /// <param name="msg">The text for the label.</param>
        /// <param name="name">The name used to create the references for the label.</param>
        /// <returns>The text to insert in the web page to create the label.</returns>
        protected string BuildLabel(string name, string msg = "")
        {
            return "<div id='" + name + "_div'>" + FormLabel(name, msg) + "</div>";
        }

        /// <summary>
        ///     Update a label on a web page that was created with a DIV tag.
        /// </summary>
        /// <param name="msg">The text for the label.</param>
        /// <param name="name">The name used to create the references for the label.</param>
        protected void UpdateLabel(string name, string msg = "")
        {
            divToUpdate.Add(name + "_div", FormLabel(name, msg));
        }

        /// <summary>
        ///     Return the string required to create a web page label.
        /// </summary>
        protected string FormLabel(string name, string message = "", bool visible = true)
        {
            string content;
            if (visible)
                content = message + "<input id='" + name + "' Name='" + name + "' Type='hidden'>";
            else
                content = "<input id='" + name + "' Name='" + name + "' Type='hidden' value='" + message + "'>";
            return content;
        }

        /// <summary>
        ///     Build a text entry box for a web page.
        /// </summary>
        /// <param name="text">The default text for the text box.</param>
        /// <param name="name">The name used to create the references for the text box.</param>
        /// <param name="allowEdit">if set to <c>true</c> allow the text to be edited.</param>
        /// <returns>The text to insert in the web page to create the text box.</returns>
        protected string BuildTextBox(string name, string text = "", bool allowEdit = true)
        {
            return "<div id='" + name + "_div'>" + HtmlTextBox(name, text, 20, allowEdit) + "</div>";
        }

        /// <summary>
        ///     Update a text box on a web page that was created with a DIV tag.
        /// </summary>
        /// <param name="text">The text for the text box.</param>
        /// <param name="name">The name used to create the references for the text box.</param>
        /// <param name="allowEdit">if set to <c>true</c> allow the text to be edited.</param>
        protected void UpdateTextBox(string name, string text = "", bool allowEdit = true)
        {
            divToUpdate.Add(name + "_div", HtmlTextBox(name, text, 20, allowEdit));
        }

        /// <summary>
        ///     Return the string required to create a web page text box.
        /// </summary>
        protected string HtmlTextBox(string name, string defaultText, int size, bool allowEdit = true)
        {
            var style = "";
            var sReadOnly = "";

            if (!allowEdit)
            {
                style = "color:#F5F5F5; background-color:#C0C0C0;";
                sReadOnly = "readonly='readonly'";
            }

            return "<input type='text' id='o" + name + "' style='" + style + "' size='" + size + "' name='" + name +
                   "' " + sReadOnly + " value='" + defaultText + "'>";
        }

        /// <summary>
        ///     Build a check box for a web page.
        /// </summary>
        /// <param name="name">The name used to create the references for the text box.</param>
        /// <param name="Checked">if set to <c>true</c> [checked].</param>
        /// <param name="AutoPostBack">if set to <c>true</c></param>
        /// <param name="SubmitForm">if set to <c>true</c></param>
        /// <returns>The text to insert in the web page to create the check box.</returns>
        protected string BuildCheckBox(string Name, bool Checked = false, bool AutoPostBack = true, bool SubmitForm = true)
        {
            return "<div id='" + Name + "_div'>" + FormCheckBox(Name, Checked, AutoPostBack, SubmitForm) + "</div>";
        }

        /// <summary>
        ///     Update a check box on a web page that was created with a DIV tag.
        /// </summary>
        /// <param name="name">The name used to create the references for the text box.</param>
        /// <param name="Checked">if set to <c>true</c> [checked].</param>
        /// <param name="AutoPostBack">if set to <c>true</c></param>
        /// <param name="SubmitForm">if set to <c>true</c></param>
        protected void UpdateCheckBox(string Name, bool Checked = false, bool AutoPostBack = true, bool SubmitForm = true)
        {
            divToUpdate.Add(Name + "_div", FormCheckBox(Name, Checked, AutoPostBack, SubmitForm));
        }

        /// <summary>
        ///     Return the string required to create a web page check box.
        /// </summary>
        protected string FormCheckBox(string name, bool Checked = false, bool autoPostBack = true,
            bool submitForm = true)
        {
            var cb = new clsJQuery.jqCheckBox(name, "", PageName, autoPostBack, submitForm)
            {
                id = "o" + name,
                @checked = Checked
            };
            return cb.Build();
        }

        /// <summary>
        ///     Build a list box for a web page.
        /// </summary>
        /// <param name="name">The name used to create the references for the list box.</param>
        /// <param name="options">Data value pairs used to populate the list box.</param>
        /// <param name="selected">Index of the item to be selected.</param>
        /// <param name="selectedValue">Name of the value to be selected.</param>
        /// <param name="width">Width of the list box</param>
        /// <param name="enabled">if set to <c>true</c> [enabled].  Doesn't seem to work.</param>
        /// <returns>The text to insert in the web page to create the list box.</returns>
        protected string BuildListBox(string name, ref NameValueCollection options, int selected = -1,
            string selectedValue = "", int width = 150, bool enabled = true)
        {
            return "<div id='" + name + "_div'>" +
                   FormListBox(name, ref options, selected, selectedValue, width, enabled) + "</div>";
        }

        /// <summary>
        ///     Update a list box for a web page that was created with a DIV tag.
        /// </summary>
        /// <param name="name">The name used to create the references for the list box.</param>
        /// <param name="options">Data value pairs used to populate the list box.</param>
        /// <param name="selected">Index of the item to be selected.</param>
        /// <param name="selectedValue">Name of the value to be selected.</param>
        /// <param name="width">Width of the list box</param>
        /// <param name="enabled">if set to <c>true</c> [enabled].  Doesn't seem to work.</param>
        protected void UpdateListBox(string name, ref NameValueCollection options, int selected = -1,
            string selectedValue = "", int width = 150, bool enabled = true)
        {
            divToUpdate.Add(name + "_div", FormListBox(name, ref options, selected, selectedValue, width, enabled));
        }

        /// <summary>
        ///     Return the string required to create a web page list box.
        /// </summary>
        protected string FormListBox(string name, ref NameValueCollection options, int selected = -1,
            string selectedValue = "", int width = 150, bool enabled = true)
        {
            var lb = new clsJQuery.jqListBox(name, PageName);

            lb.items.Clear();
            lb.id = "o" + name;
            lb.style = "width: " + width + "px;";
            lb.enabled = enabled;

            if (options != null)
            {
                for (var i = 0; i < options.Count; i++)
                {
                    if (selected == -1 && selectedValue == options.GetKey(i))
                        selected = i;
                    lb.items.Add(options.GetKey(i));
                }
                if (selected >= 0)
                    lb.SelectedValue = options.GetKey(selected);
            }

            return lb.Build();
        }

        /// <summary>
        ///     Build a drop list for a web page.
        /// </summary>
        /// <param name="name">The name used to create the references for the list box.</param>
        /// <param name="options">Data value pairs used to populate the list box.</param>
        /// <param name="selected">Index of the item to be selected.</param>
        /// <param name="selectedValue">Name of the value to be selected.</param>
        /// <returns>The text to insert in the web page to create the drop list.</returns>
        protected string BuildDropList(string name, ref NameValueCollection options, int selected = -1,
            string selectedValue = "")
        {
            return "<div id='" + name + "_div'>" +
                   FormDropDown(name, ref options, selected, selectedValue: selectedValue) + "</div>";
        }

        /// <summary>
        ///     Update a drop list for a web page that was created with a DIV tag.
        /// </summary>
        /// <param name="name">The name used to create the references for the list box.</param>
        /// <param name="options">Data value pairs used to populate the list box.</param>
        /// <param name="selected">Index of the item to be selected.</param>
        /// <param name="selectedValue">Name of the value to be selected.</param>
        protected void UpdateDropList(string name, ref NameValueCollection options, int selected = -1,
            string selectedValue = "")
        {
            divToUpdate.Add(name + "_div", FormDropDown(name, ref options, selected, selectedValue: selectedValue));
        }

        /// <summary>
        ///     Return the string required to create a web page drop list.
        /// </summary>
        protected string FormDropDown(string name, ref NameValueCollection options, int selected, int width = 150,
            bool submitForm = true, bool addBlankRow = false,
            bool autoPostback = true, string tooltip = "", bool enabled = true, string ddMsg = "",
            string selectedValue = "")
        {
            var dd = new clsJQuery.jqDropList(name, PageName, submitForm)
            {
                selectedItemIndex = -1,
                id = "o" + name,
                autoPostBack = autoPostback,
                toolTip = tooltip,
                style = "width: " + width + "px;",
                enabled = enabled
            };


            //Add a blank area to the top of the list
            if (addBlankRow)
                dd.AddItem(ddMsg, "", false);

            if (options != null)
                for (var i = 0; i < options.Count; i++)
                {
                    var sel = i == selected || options.Get(i) == selectedValue;

                    dd.AddItem(options.GetKey(i), options.Get(i), sel);
                }

            return dd.Build();
        }
    }
}