using System;
using System.Windows.Forms;
using System.Runtime.CompilerServices;

namespace LiveSplit.Localization;

public static class UiLocalizer
{
    private static readonly ConditionalWeakTable<ListControl, object> formattedListControls = new();

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

        if (control is ToolStrip strip)
        {
            ApplyToolStripItems(strip.Items, language);
        }

        if (control is ListControl listControl)
        {
            EnsureListControlFormatting(listControl);
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
                EnsureListControlFormatting(comboBox.ComboBox);
            }

            if (item is ToolStripDropDownItem dropDownItem)
            {
                ApplyToolStripItems(dropDownItem.DropDownItems, language);
            }
        }
    }

    private static void EnsureListControlFormatting(ListControl listControl)
    {
        if (listControl == null)
        {
            return;
        }

        try
        {
            formattedListControls.Add(listControl, new object());
        }
        catch (ArgumentException)
        {
            // Already registered.
            return;
        }

        listControl.FormattingEnabled = true;
        listControl.Format += ListControl_Format;
        listControl.Refresh();
    }

    private static void ListControl_Format(object sender, ListControlConvertEventArgs e)
    {
        if (e.Value is string formattedValue)
        {
            e.Value = Translate(formattedValue);
        }
        else if (e.ListItem is string source)
        {
            e.Value = Translate(source);
        }
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
}
