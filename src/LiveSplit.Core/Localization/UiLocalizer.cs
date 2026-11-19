using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace LiveSplit.Localization;

public static class UiLocalizer
{
    private static readonly ConditionalWeakTable<ListControl, ListControlLocalizationState> formattedListControls = new();

    public static string Translate(string source)
    {
        return Translate(source, LanguageResolver.ResolveCurrentCultureLanguage());
    }

    public static string Translate(string source, AppLanguage language)
    {
        if (string.IsNullOrEmpty(source) || language == null || !language.RequiresLocalization)
        {
            return source;
        }

        return UiTextCatalog.TryGetTranslation(source, language, out string translated)
            ? translated
            : source;
    }

    public static string TranslateKey(string key, string fallback)
    {
        return TranslateKey(key, fallback, LanguageResolver.ResolveCurrentCultureLanguage());
    }

    public static string TranslateKey(string key, string fallback, AppLanguage language)
    {
        if (UiTextCatalog.TryGetKeyTranslation(language, key, out string translated))
        {
            return translated;
        }

        return Translate(fallback, language);
    }

    public static void Apply(Form form)
    {
        Apply(form, LanguageResolver.ResolveCurrentCultureLanguage());
    }

    public static void Apply(Form form, AppLanguage language)
    {
        if (form == null || language == null || !language.RequiresLocalization)
        {
            return;
        }

        ApplyControl(form, language);
    }

    public static void ApplyOpenForms(AppLanguage language)
    {
        if (language == null || !language.RequiresLocalization)
        {
            return;
        }

        foreach (Form form in Application.OpenForms)
        {
            Apply(form, language);
        }
    }

    public static void ApplyCurrentCultureToOpenForms()
    {
        ApplyOpenForms(LanguageResolver.ResolveCurrentCultureLanguage());
    }

    private static void ApplyControl(Control control, AppLanguage language)
    {
        if (control == null)
        {
            return;
        }

        if (CanTranslateControlText(control))
        {
            control.Text = Translate(control.Text, language);
        }

        if (control is Form or UserControl)
        {
            ApplyOwnedToolStrips(control, language);
        }

        if (control is ToolStrip strip)
        {
            ApplyToolStripItems(strip.Items, language);
        }

        if (control is ListControl listControl)
        {
            EnsureListControlFormatting(listControl, language);
        }

        if (control.ContextMenuStrip != null)
        {
            ApplyToolStripItems(control.ContextMenuStrip.Items, language);
        }

        if (control is DataGridView dataGridView)
        {
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.HeaderText = Translate(column.HeaderText, language);
            }
        }

        if (control is ListView listView)
        {
            foreach (ColumnHeader column in listView.Columns)
            {
                column.Text = Translate(column.Text, language);
            }
        }

        if (control is TreeView treeView)
        {
            ApplyTreeNodes(treeView.Nodes, language);
        }

        if (control is Form form)
        {
            if (form.MainMenuStrip != null)
            {
                ApplyToolStripItems(form.MainMenuStrip.Items, language);
            }

            if (form.ContextMenuStrip != null)
            {
                ApplyToolStripItems(form.ContextMenuStrip.Items, language);
            }
        }

        foreach (Control child in control.Controls)
        {
            ApplyControl(child, language);
        }
    }

    private static bool CanTranslateControlText(Control control)
    {
        if (control is TextBoxBase or ComboBox or ListBox or NumericUpDown or DomainUpDown or DateTimePicker)
        {
            return false;
        }

        return !string.IsNullOrWhiteSpace(control.Text);
    }

    private static void ApplyToolStripItems(ToolStripItemCollection items, AppLanguage language)
    {
        if (items == null)
        {
            return;
        }

        foreach (ToolStripItem item in items)
        {
            if (item is not ToolStripTextBox && item is not ToolStripComboBox &&
                !string.IsNullOrWhiteSpace(item.Text))
            {
                item.Text = Translate(item.Text, language);
            }

            if (!string.IsNullOrWhiteSpace(item.ToolTipText))
            {
                item.ToolTipText = Translate(item.ToolTipText, language);
            }

            if (item is ToolStripComboBox comboBox)
            {
                EnsureListControlFormatting(comboBox.ComboBox, language);
            }

            if (item is ToolStripDropDownItem dropDownItem)
            {
                ApplyToolStripItems(dropDownItem.DropDownItems, language);
            }
        }
    }

    private static void EnsureListControlFormatting(ListControl listControl, AppLanguage language)
    {
        if (listControl == null)
        {
            return;
        }

        if (formattedListControls.TryGetValue(listControl, out ListControlLocalizationState state))
        {
            state.Language = language;
            listControl.Refresh();
            return;
        }

        state = new ListControlLocalizationState { Language = language };
        formattedListControls.Add(listControl, state);
        listControl.Format += ListControl_Format;
        listControl.FormattingEnabled = false;
        listControl.FormattingEnabled = true;
        listControl.Refresh();
    }

    private static void ListControl_Format(object sender, ListControlConvertEventArgs e)
    {
        if (sender is not ListControl listControl
            || !formattedListControls.TryGetValue(listControl, out ListControlLocalizationState state))
        {
            return;
        }

        string source = GetListControlSourceText(listControl, e);
        if (source != null)
        {
            e.Value = Translate(source, state.Language);
        }
    }

    private static string GetListControlSourceText(ListControl listControl, ListControlConvertEventArgs e)
    {
        if (e.ListItem != null && !string.IsNullOrWhiteSpace(listControl.DisplayMember))
        {
            object displayValue = GetMemberValue(e.ListItem, listControl.DisplayMember);
            if (displayValue is string displayText)
            {
                return displayText;
            }
        }

        if (e.ListItem is string listItemText)
        {
            return listItemText;
        }

        return e.Value as string;
    }

    private static object GetMemberValue(object item, string memberPath)
    {
        object value = item;
        foreach (string memberName in memberPath.Split('.'))
        {
            if (value == null)
            {
                return null;
            }

            PropertyDescriptor property = TypeDescriptor.GetProperties(value).Find(memberName, true);
            if (property == null)
            {
                return null;
            }

            value = property.GetValue(value);
        }

        return value;
    }

    private static void ApplyTreeNodes(TreeNodeCollection nodes, AppLanguage language)
    {
        if (nodes == null)
        {
            return;
        }

        foreach (TreeNode node in nodes)
        {
            if (!string.IsNullOrWhiteSpace(node.Text))
            {
                node.Text = Translate(node.Text, language);
            }

            if (!string.IsNullOrWhiteSpace(node.ToolTipText))
            {
                node.ToolTipText = Translate(node.ToolTipText, language);
            }

            ApplyTreeNodes(node.Nodes, language);
        }
    }

    private static void ApplyOwnedToolStrips(object owner, AppLanguage language)
    {
        foreach (FieldInfo field in EnumerateInstanceFields(owner.GetType()))
        {
            object value;
            try
            {
                value = field.GetValue(owner);
            }
            catch
            {
                continue;
            }

            switch (value)
            {
                case ContextMenuStrip contextMenuStrip:
                    ApplyToolStripItems(contextMenuStrip.Items, language);
                    break;
                case MenuStrip menuStrip:
                    ApplyToolStripItems(menuStrip.Items, language);
                    break;
                case ToolStrip toolStrip:
                    ApplyToolStripItems(toolStrip.Items, language);
                    break;
            }
        }
    }

    private sealed class ListControlLocalizationState
    {
        public AppLanguage Language { get; set; }
    }

    private static System.Collections.Generic.IEnumerable<FieldInfo> EnumerateInstanceFields(Type type)
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

        while (type != null && type != typeof(object))
        {
            foreach (FieldInfo field in type.GetFields(flags))
            {
                yield return field;
            }

            type = type.BaseType;
        }
    }
}
