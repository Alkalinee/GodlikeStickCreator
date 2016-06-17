﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using GodlikeStickCreator.Core;
using GodlikeStickCreator.Core.Applications;
using GodlikeStickCreator.ViewModelBase;

namespace GodlikeStickCreator.ViewModels
{
    public class ApplicationsViewModel : View
    {
        private RelayCommand _deselectAllCommand;
        private RelayCommand _selectAllCommand;

        public ApplicationsViewModel(UsbStickSettings usbStickSettings) : base(usbStickSettings)
        {
            Applications = new List<ApplicationInfo>(new[]
            {
                new ApplicationInfo
                {
                    Name = "ILSpy",
                    DownloadUrl =
                        new Lazy<string>(
                            () => GetNewestReleaseDownloadUrl("https://github.com/icsharpcode/ILSpy/releases")),
                    ApplicationCategory = ApplicationCategory.Decompiler,
                    Description = "ILSpy is the open-source .NET assembly browser and decompiler."
                },
                new ApplicationInfo
                {
                    Name = "FFmpeg 32 Bit",
                    DownloadUrl = new Lazy<string>(() => GetFFmpegDownloadUrl(true)),
                    ApplicationCategory = ApplicationCategory.Multimedia,
                    Description =
                        "FFmpeg is a complete, cross-platform solution to record, convert and stream audio and video."
                },
                new ApplicationInfo
                {
                    Name = "FFmpeg 64 Bit",
                    DownloadUrl = new Lazy<string>(() => GetFFmpegDownloadUrl(false)),
                    ApplicationCategory = ApplicationCategory.Multimedia,
                    Description =
                        "FFmpeg is a complete, cross-platform solution to record, convert and stream audio and video."
                },
                new ApplicationInfo
                {
                    Name = "Process Explorer",
                    DownloadUrl = new Lazy<string>(() => "https://download.sysinternals.com/files/ProcessExplorer.zip"),
                    ApplicationCategory = ApplicationCategory.SystemTools,
                    Description =
                        "Process Explorer is a freeware task manager and system monitor."
                },
                new ApplicationInfo
                {
                    Name = "Autoruns",
                    DownloadUrl = new Lazy<string>(() => "https://download.sysinternals.com/files/Autoruns.zip"),
                    ApplicationCategory = ApplicationCategory.SystemTools,
                    Description =
                        "Autoruns is an app that shows you what apps are configured to run during your system bootup or login."
                },
                new ApplicationInfo
                {
                    Name = "HxD",
                    DownloadUrl = new Lazy<string>(() => "http://mh-nexus.de/downloads/HxDen.zip"),
                    ApplicationCategory = ApplicationCategory.Editors,
                    Description =
                        "HxD is a carefully designed and fast hex editor which, additionally to raw disk editing and modifying of main memory (RAM), handles files of any size."
                },
                new ApplicationInfo
                {
                    Name = "Notepad++",
                    DownloadUrl = new Lazy<string>(GetNotepadPlusPlusDownloadLink),
                    ApplicationCategory = ApplicationCategory.Editors,
                    Description =
                        "Notepad++ is a free source code editor and Notepad replacement that supports several languages."
                },
                new ApplicationInfo
                {
                    Name = "MultiHasher",
                    DownloadUrl = new Lazy<string>(() => "http://hostsman.it-mate.co.uk/MultiHasher_2.8.2_win.zip"),
                    ApplicationCategory = ApplicationCategory.FileTools,
                    Description = "MultiHasher is a freeware file hash calculator."
                },
                new ApplicationInfo
                {
                    Name = "7-Zip (console)",
                    DownloadUrl = new Lazy<string>(Get7ZipDownloadUrl),
                    ApplicationCategory = ApplicationCategory.FileTools,
                    Description = "7-Zip is a file archiver with a high compression ratio."
                },
                new ApplicationInfo
                {
                    Name ="PEStudio",
                    DownloadUrl = new Lazy<string>(() => "https://www.winitor.com/tools/pestudio/current/pestudio.zip"),
                    ApplicationCategory = ApplicationCategory.FileTools,
                    Description = "pestudio is a tool that is used in many Cyber Emergency Response Teams (CERT) worldwide in order to perform malware initial assessment."
                },
                new ApplicationInfo
                {
                    Name ="CPU-Z",
                    DownloadUrl = new Lazy<string>(() => "http://download.cpuid.com/cpu-z/cpu-z_1.76-en.zip"),
                    ApplicationCategory = ApplicationCategory.InformationTools,
                    Description = "CPU-Z is a freeware that gathers information on some of the main devices of your system."
                },
                new ApplicationInfo
                {
                    Name ="LaZagne",
                    DownloadUrl = new Lazy<string>(() => GetNewestReleaseDownloadUrl("https://github.com/AlessandroZ/LaZagne/releases")),
                    ApplicationCategory = ApplicationCategory.InformationTools,
                    Description = "The LaZagne project is used to retrieve lots of passwords stored on a local computer from about 22 programs."
                },
                new ApplicationInfo
                {
                    Name ="NotMyFault",
                    DownloadUrl = new Lazy<string>(() => "https://live.sysinternals.com/files/NotMyFault.zip"),
                    ApplicationCategory = ApplicationCategory.InformationTools,
                    Description = "The NotMyFault tool is a great way to crash Windows systems in a controlled manner to test various tools and analyze crashes."
                },
                new ApplicationInfo
                {
                    Name ="Prime95 32 Bit",
                    DownloadUrl = new Lazy<string>(() => "http://www.mersenne.org/ftp_root/gimps/p95v289.win32.zip"),
                    ApplicationCategory = ApplicationCategory.InformationTools,
                    Description = "Prime95 is a small and easy to use application that allows you to find Mersenne Prime numbers designed for overclockers."
                },
                new ApplicationInfo
                {
                    Name ="Prime95 64 Bit",
                    DownloadUrl = new Lazy<string>(() => "http://www.mersenne.org/ftp_root/gimps/p95v289.win64.zip"),
                    ApplicationCategory = ApplicationCategory.InformationTools,
                    Description = "Prime95 is a small and easy to use application that allows you to find Mersenne Prime numbers designed for overclockers."
                },
                new ApplicationInfo
                {
                    Name = "CoreTemp 32 Bit",
                    DownloadUrl = new Lazy<string>(() => "http://www.alcpu.com/CoreTemp/php/download.php?id=2"),
                    ApplicationCategory = ApplicationCategory.InformationTools,
                    Extension = "zip",
                    Description = "Core Temp is a compact, no fuss, small footprint, yet powerful program to monitor processor temperature and other vital information."
                },
                new ApplicationInfo
                {
                    Name = "CoreTemp 64 Bit",
                    DownloadUrl = new Lazy<string>(() => "http://www.alcpu.com/CoreTemp/php/download.php?id=3"),
                    ApplicationCategory = ApplicationCategory.InformationTools,
                    Extension = "zip",
                    Description = "Core Temp is a compact, no fuss, small footprint, yet powerful program to monitor processor temperature and other vital information."
                },
                new ApplicationInfo
                {
                    Name = "ImgBurn",
                    DownloadUrl = new Lazy<string>(() => "http://filessjc01.dddload.net/static/Portable_ImgBurn_2.5.8.0.exe"),
                    ApplicationCategory = ApplicationCategory.InformationTools,
                    Extension = "7z",
                    Description = "ImgBurn is a lightweight CD / DVD / HD DVD / Blu-ray burning application that everyone should have in their toolkit!"
                },
                new ApplicationInfo
                {
                    Name = "FileZilla",
                    DownloadUrl = new Lazy<string>(() => "http://vorboss.dl.sourceforge.net/project/filezilla/FileZilla_Client/3.18.0/FileZilla_3.18.0_win32.zip"),
                    ApplicationCategory = ApplicationCategory.Network,
                    Description = "FileZilla is a cross-platform graphical FTP, SFTP, and FTPS file management tool."
                },
                new ApplicationInfo
                {
                    Name = "Putty",
                    DownloadUrl = new Lazy<string>(() => "https://the.earth.li/~sgtatham/putty/latest/x86/putty.zip"),
                    ApplicationCategory = ApplicationCategory.Network,
                    Description = "PuTTY is a free implementation of SSH and Telnet."
                },
                new ApplicationInfo
                {
                    Name = "WinSCP",
                    DownloadUrl = new Lazy<string>(GetWinSCPDownloadUrl),
                    ApplicationCategory = ApplicationCategory.Network,
                    Description = "WinSCP is an open source free SFTP client and FTP client for Windows."
                },
                new ApplicationInfo
                {
                    Name = "PsTools",
                    DownloadUrl = new Lazy<string>(() => "https://download.sysinternals.com/files/PSTools.zip"),
                    ApplicationCategory = ApplicationCategory.SystemTools,
                    Description = "he PsTools suite includes command-line utilities that help you administer your Windows NT/2K systems."
                },
                new ApplicationInfo
                {
                    Name = "SharpDevelop",
                    DownloadUrl = new Lazy<string>(() => "http://netix.dl.sourceforge.net/project/sharpdevelop/SharpDevelop%205.x/5.1/SharpDevelop_5.1.0.5216_Xcopyable.zip"),
                    ApplicationCategory = ApplicationCategory.Editors,
                    Description = "#develop (short for SharpDevelop) is a free IDE for C# projects on Microsoft's .NET platform"
                }
            }.OrderBy(x => x.Name));
            foreach (var applicationInfo in Applications)
                applicationInfo.Add = true;

            CanGoForward = true;
            UsbStickSettings.ApplicationInfo = Applications;
        }

        public List<ApplicationInfo> Applications { get; set; }

        public RelayCommand SelectAllCommand
        {
            get
            {
                return _selectAllCommand ?? (_selectAllCommand = new RelayCommand(parameter =>
                {
                    foreach (var applicationInfo in Applications)
                        applicationInfo.Add = true;
                }));
            }
        }

        public RelayCommand DeselectAllCommand
        {
            get
            {
                return _deselectAllCommand ?? (_deselectAllCommand = new RelayCommand(parameter =>
                {
                    foreach (var applicationInfo in Applications)
                        applicationInfo.Add = false;
                }));
            }
        }

        private static string GetNewestReleaseDownloadUrl(string githubReleaseUrl)
        {
            var websiteSource = new WebClient().DownloadString(githubReleaseUrl);
            var url =
                Regex.Match(websiteSource, @"<ul class=""release-downloads"">\s*<li>\s*<a href=""(?<url>(.+?))""")
                    .Groups
                    ["url"].Value;
            return "https://www.github.com" + url;
        }

        private static string GetFFmpegDownloadUrl(bool bit32)
        {
            var source = new WebClient().DownloadString("https://ffmpeg.zeranoe.com/builds/");
            return $"https://ffmpeg.zeranoe.com/builds/win{(bit32 ? "32" : "64")}/static/" +
                   Regex.Match(source,
                       $@"<a class=""latest"" href=""\./win{(bit32 ? "32" : "64")}/static/(?<filename>(.*?))""").Groups[
                           "filename"].Value;
        }

        private static string GetNotepadPlusPlusDownloadLink()
        {
            var source = new WebClient().DownloadString("https://notepad-plus-plus.org/download");
            return "https://notepad-plus-plus.org" +
                   Regex.Match(source, @"<a href=""(?<url>(.*?))"">Notepad\+\+ zip package").Groups["url"].Value;
        }

        private static string Get7ZipDownloadUrl()
        {
            var source = new WebClient().DownloadString("http://www.7-zip.org/download.html");
            return "http://www.7-zip.org/" +
                   Regex.Match(source, @"<A href=""(?<url>(.*?))-extra\.7z"">Download<\/A>").Groups["url"].Value +
                   "-extra.7z";
        }

        private static string GetWinSCPDownloadUrl()
        {
            using (var webClient = new WebClient())
            {
                var source = webClient.DownloadString("https://winscp.net/eng/download.php");
                source =
                    webClient.DownloadString("https://winscp.net" +
                                             Regex.Match(source, @"<a href=""\.\.(?<downloadUrl>(.*?))\.zip""").Groups[
                                                 "downloadUrl"].Value + ".zip");
                return Regex.Match(source, @"href='(?<url>(.*?))'>\[Direct download\]<\/a>").Groups["url"].Value;
            }
        }
    }
}