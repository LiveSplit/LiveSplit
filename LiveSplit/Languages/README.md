# Multiple Language Support

Copy the entire **`Languages` folder** to the directory where **LiveSplit.exe** is located.

Run LiveSplit.exe, right click on the window and set the language in `Settings` -> `Languages`.

## Add Other Languages

If LiveSplit does not support your language, you can add your language to it.

1. Create a new file ending in .cfg in the Languages folder with a name that is an abbreviation for your language. For example, `zh-CN.cfg`.
2. Copy all the contents of the `en.cfg` file into the newly created file.
3. Change the contents of the DisplayName tag to the name of your language, which will be displayed in the "Settings" -> "Language" option. For example, `<DisplayName>English</DisplayName>`.
4. Translate all content in xml tags to your language based on English text.

After completing the above, you can switch to your language in the "Settings" -> "Language" of the software.
