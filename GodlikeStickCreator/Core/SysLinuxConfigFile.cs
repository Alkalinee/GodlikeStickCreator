﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media;
using GodlikeStickCreator.Annotations;
using GodlikeStickCreator.Core.Config;
using GodlikeStickCreator.Core.System;
using GodlikeStickCreator.Utilities;

namespace GodlikeStickCreator.Core
{
    public class SysLinuxConfigFile
    {
        private readonly string _filename;
        private string _content;

        private SysLinuxConfigFile(string filename, SysLinuxAppearance sysLinuxAppearance)
        {
            _filename = filename;
            SysLinuxAppearance = sysLinuxAppearance;
            Entries = new ObservableCollection<SystemEntry>();
        }

        public ObservableCollection<SystemEntry> Entries { get; }
        public SysLinuxAppearance SysLinuxAppearance { get; private set; }

        private string GetDefaultHeader()
        {
            return
                $@"UI vesamenu.c32
TIMEOUT 300
MENU RESOLUTION 640 480
MENU TITLE {SysLinuxAppearance.ScreenTitle}
MENU BACKGROUND background.png
MENU TABMSG {SysLinuxAppearance.ScreenMessage}
MENU CLEAR

MENU WIDTH 72
MENU MARGIN 10
MENU VSHIFT 3
MENU HSHIFT 6
MENU ROWS 15
MENU TABMSGROW 20
MENU TIMEOUTROW 22

MENU COLOR title        1;36;44 {ColorToHex(SysLinuxAppearance.TitleForegroundColor)} {ColorToHex(SysLinuxAppearance.TitleBackgroundColor)} none
MENU COLOR hotsel       30;47   #2980b9 #DDDDDDDD
MENU COLOR sel          30;47   {ColorToHex(SysLinuxAppearance.SelectedForegroundColor)} {ColorToHex(SysLinuxAppearance.SelectedBackgroundColor)}
MENU COLOR border       30;44	{ColorToHex(SysLinuxAppearance.BorderForegroundColor)} {ColorToHex( SysLinuxAppearance.BorderBackgroundColor)} std
MENU COLOR scrollbar    30;44   {ColorToHex(SysLinuxAppearance.ScrollBarForegroundColor)} {ColorToHex( SysLinuxAppearance.ScrollBarBackgroundColor)} none
MENU COLOR tabmsg       31;40   {ColorToHex(SysLinuxAppearance.TabMsgForegroundColor)} #00000000 std
MENU COLOR unsel	    37;44   {ColorToHex(SysLinuxAppearance.UnselectedForegroundColor)} {ColorToHex(SysLinuxAppearance.UnselectedBackgroundColor)} std";
        }

        private void CreateNew()
        {
            var configString =
                $@"# <Header>
{GetDefaultHeader()}
# </Header>

LABEL Boot from first Hard Drive
MENU LABEL Continue to Boot from ^First HD (default)
KERNEL chain.c32
APPEND hd1
MENU DEFAULT";
            File.WriteAllText(_filename, configString);
            Load(configString);
        }

        private string ColorToHex(Color c)
        {
            // ReSharper disable once UseStringInterpolation
            return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", c.A, c.R, c.G, c.B);
        }

        private void Load(string content)
        {
            var contentHeader =
                Regex.Match(content, @"# <Header>\s*(?<header>(.*?))\s*# <\/Header>", RegexOptions.Singleline).Groups[
                    "header"].Value;
            var defaultHeader = GetDefaultHeader();
            if (contentHeader != defaultHeader)
                content = content.Replace(contentHeader, defaultHeader);

            _content = content;
            ParseEntries();
        }

        private bool LoadConfig(string content, string filename)
        {
            string pattern;
            var match = Regex.Match(content, pattern =
                RegexReplaceNewLine(@"UI vesamenu\.c32
TIMEOUT 300
MENU RESOLUTION 640 480
MENU TITLE (?<title>(.*?))
MENU BACKGROUND background\.png
MENU TABMSG (?<tabmsg>(.*?))
MENU CLEAR

MENU WIDTH 72
MENU MARGIN 10
MENU VSHIFT 3
MENU HSHIFT 6
MENU ROWS 15
MENU TABMSGROW 20
MENU TIMEOUTROW 22

MENU COLOR title        1;36;44 (?<titleForeground>(#.*?)) (?<titleBackground>(#.*?)) none
MENU COLOR hotsel       30;47   #2980b9 #DDDDDDDD
MENU COLOR sel          30;47   (?<selectedForeground>(#.*?)) (?<selectedBackground>(#.*?))
MENU COLOR border       30;44	(?<borderForeground>(#.*?)) (?<borderBackground>(#.*?)) std
MENU COLOR scrollbar    30;44   (?<scrollbarForeground>(#.*?)) (?<scrollbarBackground>(#.*?)) none
MENU COLOR tabmsg       31;40   (?<tabmsgForeground>(#.*?)) #00000000 std
MENU COLOR unsel	    37;44   (?<unselectedForeground>(#.*?)) (?<unselectedBackground>(#.*?)) std"));
            if (!match.Success)
                return false;

            SysLinuxAppearance = new SysLinuxAppearance
            {
                ScreenTitle = match.Groups["title"].Value,
                ScreenMessage = match.Groups["tabmsg"].Value,
                TitleForegroundColor = (Color) ColorConverter.ConvertFromString(match.Groups["titleForeground"].Value),
                TitleBackgroundColor = (Color) ColorConverter.ConvertFromString(match.Groups["titleBackground"].Value),
                SelectedForegroundColor =
                    (Color) ColorConverter.ConvertFromString(match.Groups["selectedForeground"].Value),
                SelectedBackgroundColor =
                    (Color) ColorConverter.ConvertFromString(match.Groups["selectedBackground"].Value),
                BorderForegroundColor = (Color) ColorConverter.ConvertFromString(match.Groups["borderForeground"].Value),
                BorderBackgroundColor = (Color) ColorConverter.ConvertFromString(match.Groups["borderBackground"].Value),
                ScrollBarForegroundColor =
                    (Color) ColorConverter.ConvertFromString(match.Groups["scrollbarForeground"].Value),
                ScrollBarBackgroundColor =
                    (Color) ColorConverter.ConvertFromString(match.Groups["scrollbarBackground"].Value),
                TabMsgForegroundColor = (Color) ColorConverter.ConvertFromString(match.Groups["tabmsgForeground"].Value),
                UnselectedForegroundColor =
                    (Color) ColorConverter.ConvertFromString(match.Groups["unselectedForeground"].Value),
                UnselectedBackgroundColor =
                    (Color) ColorConverter.ConvertFromString(match.Groups["unselectedBackground"].Value),
                ScreenBackground = Path.Combine(Path.GetDirectoryName(filename), "background.png")
            };

            _content = content;
            ParseEntries();
            return true;
        }

        private static string RegexReplaceNewLine([RegexPattern] string regexPattern)
        {
            return regexPattern.Replace(Environment.NewLine, @"(\n|\r\n?)");
        }

        public void AppendCategoryIfNotExists(Category category)
        {
            if (!_content.Contains($"MENU BEGIN {category}"))
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"MENU BEGIN {category}");
                stringBuilder.AppendLine($"MENU TITLE {EnumUtilities.GetDescription(category)}");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine($"\t# {category}:nextEntry");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("\tMENU SEPARATOR");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine($"\tLABEL exit_{category}");
                stringBuilder.AppendLine($"\tMENU LABEL {SysLinuxAppearance.ReturnToMainMenuText}");
                stringBuilder.AppendLine("\tMENU EXIT");
                stringBuilder.AppendLine("MENU END");

                _content += "\r\n\r\n" + stringBuilder;
            }
        }

        public bool ContainsSystem(SystemInfo systemInfo)
        {
            return _content.Contains($"<{systemInfo.Name.Replace(" ", null)} ");
        }

        public string GetSystemDirectory(SystemInfo systemInfo)
        {
            return
                Regex.Match(_content, $@"# <{systemInfo.Name.Replace(" ", null)} name="".*"" directory=""(?<directoryName>(.*?))"">")
                    .Groups["directoryName"].Value;
        }

        public void AddSystem(SystemInfo systemInfo, MenuItemInfo menuItemInfo, string directoryName)
        {
            AppendCategoryIfNotExists(systemInfo.Category);
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"\t# <{systemInfo.Name.Replace(" ", null)} name=\"{systemInfo.Name}\" directory=\"{directoryName}\">");
            if (!menuItemInfo.Raw)
            {
                stringBuilder.AppendLine($"\t\tLABEL {systemInfo.Name}");
                stringBuilder.AppendLine($"\t\tMENU LABEL {systemInfo.Name}");
                stringBuilder.AppendLine("\t\tMENU INDENT 1");
            }

            foreach (var line in menuItemInfo.Text.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries))
                stringBuilder.AppendLine("\t\t" + line);

            stringBuilder.AppendLine($"\t# </{systemInfo.Name.Replace(" ", null)}>");
            stringBuilder.AppendLine();

            var nextItemEntry = $"\t# {systemInfo.Category}:nextEntry";
            stringBuilder.Append(nextItemEntry);

            _content = _content.Replace(nextItemEntry, stringBuilder.ToString());
            Entries.Add(new SystemEntry
            {
                Name = systemInfo.Name,
                Category = systemInfo.Category,
                Directory = directoryName
            });
        }

        public void RemoveSystem(SystemEntry systemEntry)
        {
            RemoveSystem(systemEntry.Name);
        }

        public void RemoveSystem(SystemInfo systemInfo)
        {
            RemoveSystem(systemInfo.Name);
        }

        private void RemoveSystem(string name)
        {
            var openTag = $"\t# <{name.Replace(" ", null)} ";
            var closeTag = $"\t# </{name.Replace(" ", null)}>";

            var beginIndex = _content.IndexOf(openTag, StringComparison.InvariantCulture);
            var endIndex = _content.IndexOf(closeTag, StringComparison.InvariantCulture) + closeTag.Length;
            _content = _content.Remove(beginIndex, endIndex - beginIndex);

            var entry = Entries.FirstOrDefault(x => x.Name == name);
            if (entry != null)
                Entries.Remove(entry);
        }

        private void ParseEntries()
        {
            Entries.Clear();
            var lines = _content.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
            var entryOpened = false;
            var regex = new Regex(@"# <(?<system>(.*?)) name=""(?<name>(.*?))"" directory=""(?<directoryName>(.*?))"">");
            var currentCategory = Category.Other;
            foreach (var line in lines)
            {
                var currentLine = line.TrimStart();

                if (currentLine.StartsWith("MENU BEGIN "))
                {
                    var categoryStr = currentLine.Substring("MENU BEGIN ".Length);
                    Enum.TryParse(categoryStr, out currentCategory);
                }

                if (currentLine.StartsWith("# <") && !entryOpened)
                {
                    var match = regex.Match(currentLine);
                    if (!match.Success)
                        continue;

                    entryOpened = true;
                    Entries.Add(new SystemEntry {Name = match.Groups["name"].Value, Directory = match.Groups["directoryName"].Value, Category = currentCategory});
                }

                if (currentLine.StartsWith("# </"))
                    entryOpened = false;
            }
        }

        public void Save()
        {
            File.WriteAllText(_filename, _content);
        }

        public static SysLinuxConfigFile Create(string filename, SysLinuxAppearance sysLinuxAppearance)
        {
            var configFile = new SysLinuxConfigFile(filename, sysLinuxAppearance);
            configFile.CreateNew();
            return configFile;
        }

        public static SysLinuxConfigFile OpenFile(string filename, SysLinuxAppearance sysLinuxAppearance)
        {
            var configFile = new SysLinuxConfigFile(filename, sysLinuxAppearance);
            configFile.Load(File.ReadAllText(filename));
            return configFile;
        }

        public static bool TryParse(string filename, out SysLinuxConfigFile sysLinuxConfigFile)
        {
            var sysLinuxConfig = new SysLinuxConfigFile(filename, null);
            if (sysLinuxConfig.LoadConfig(File.ReadAllText(filename), filename))
            {
                sysLinuxConfigFile = sysLinuxConfig;
                return true;
            }

            sysLinuxConfigFile = null;
            return false;
        }
    }

    public class SystemEntry
    {
        public string Name { get; set; }
        public string Directory { get; set; }
        public Category Category { get; set; }
    }

    public class MenuItemInfo
    {
        public MenuItemInfo(string text)
        {
            Text = text;
        }

        public MenuItemInfo(string text, bool raw)
        {
            Text = text;
            Raw = raw;
        }

        public string Text { get; }
        public bool Raw { get; set; }
    }
}