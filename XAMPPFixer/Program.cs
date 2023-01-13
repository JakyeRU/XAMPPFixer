using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using Spectre.Console;

namespace XAMPPFixer
{
    class Program
    {
        static string driveLetter = Path.GetPathRoot(Environment.CurrentDirectory);
        static string bootLogo = @"[red]
            ██╗░░██╗░█████╗░███╗░░░███╗██████╗░██████╗░  ███████╗██╗██╗░░██╗███████╗██████╗░
            ╚██╗██╔╝██╔══██╗████╗░████║██╔══██╗██╔══██╗  ██╔════╝██║╚██╗██╔╝██╔════╝██╔══██╗
            ░╚███╔╝░███████║██╔████╔██║██████╔╝██████╔╝  █████╗░░██║░╚███╔╝░█████╗░░██████╔╝
            ░██╔██╗░██╔══██║██║╚██╔╝██║██╔═══╝░██╔═══╝░  ██╔══╝░░██║░██╔██╗░██╔══╝░░██╔══██╗
            ██╔╝╚██╗██║░░██║██║░╚═╝░██║██║░░░░░██║░░░░░  ██║░░░░░██║██╔╝╚██╗███████╗██║░░██║
            ╚═╝░░╚═╝╚═╝░░╚═╝╚═╝░░░░░╚═╝╚═╝░░░░░╚═╝░░░░░  ╚═╝░░░░░╚═╝╚═╝░░╚═╝╚══════╝╚═╝░░╚═╝[/]";
        static string finishLogo = @"[green]
            ▀▄▀ ▄▀█ █▀▄▀█ █▀█ █▀█   █ █▀   █▀▀ █ ▀▄▀ █▀▀ █▀▄
            █░█ █▀█ █░▀░█ █▀▀ █▀▀   █ ▄█   █▀░ █ █░█ ██▄ █▄▀[/]";
        static string xampp_path = $"{driveLetter}xampp";
        static string mysql_path = $"{driveLetter}xampp\\mysql";
        static string[] mysql_excluded_files = {"mysql", "performance_schema", "phpmyadmin", "test"};

        static void Main(string[] args)
        {
            AnsiConsole.MarkupLine(bootLogo);

            if (!Directory.Exists(xampp_path))
            {
                AnsiConsole.MarkupLine("[red][[ERROR]][/] XAMPP is not installed on this device. Exiting...");
                System.Environment.Exit(1);
            } else
            {
                AnsiConsole.MarkupLine($"[green][[INFO]][/] XAMPP installation found at [green]{xampp_path}[/]");
            }

            // Making sure the mysqld.exe process is not running.
            if (Process.GetProcessesByName("mysqld").Length > 0)
            {
                AnsiConsole.MarkupLine("[red][[ERROR]][/] Process [red]mysqld.exe[/] should not be running!");

                if (Prompt("Should the process [red]mysqld.exe[/] be stopped?"))
                {
                    foreach (var process in Process.GetProcessesByName("mysqld"))
                    {
                        AnsiConsole.MarkupLine($"[yellow][[INFO]][/] Attempting to kill process [red]mysqld.exe[/]...");
                        process.Kill();
                    }
                }
                else
                {
                    System.Environment.Exit(1);
                }

                AnsiConsole.MarkupLine($"[green][[INFO]][/] Process [red]mysqld.exe[/] killed. Resuming...");
            }

            bool shouldBackupExistingData = Prompt("Should the existing data be backed up in case of failure?");

            // Making a backup of the data if shouldBackupExistingData is true
            if (shouldBackupExistingData)
            {
                AnsiConsole.MarkupLine("[yellow][[INFO]][/] Started backup of existing data...");
                // Creating the "data-backups" folder if it doesn't exist
                Int64 unixTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                Directory.CreateDirectory($"{mysql_path}\\data-backups");
                Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory($"{mysql_path}\\data", $"{mysql_path}\\data-backups\\data-{unixTime}");
                AnsiConsole.MarkupLine($"[green][[INFO]][/] Backed up data at [green]{mysql_path}\\data-backups\\data-{unixTime}[/]");
            }

            // Renaming the directory "data" to "data-temp"
            Directory.Move($"{mysql_path}\\data", $"{mysql_path}\\data-temp");
            AnsiConsole.MarkupLine($"[green][[INFO]][/] Renamed [green]{mysql_path}\\data[/] to [green]{mysql_path}\\data-temp[/]");

            // Copying the "backup" folder to "data"
            Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory($"{mysql_path}\\backup", $"{mysql_path}\\data");
            AnsiConsole.MarkupLine($"[green][[INFO]][/] Copied [green]{mysql_path}\\backup[/] to [green]{mysql_path}\\data[/]");

            // Looping through "data-temp" and moving necessary files to "data"
            foreach (var directory in Directory.GetDirectories($"{mysql_path}\\data-temp"))
            {
                // Making sure the file is not excluded
                if (!mysql_excluded_files.Contains(new DirectoryInfo(directory).Name))
                {
                    Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(directory, $"{mysql_path}\\data\\{new DirectoryInfo(directory).Name}", true);
                }
            }
            AnsiConsole.MarkupLine($"[green][[INFO]][/] Copied database files from [green]{mysql_path}\\data-temp[/] to [green]{mysql_path}\\data[/]");

            // Copying the "ibdata1" from "data-temp" to "data"
            File.Copy($"{mysql_path}\\data-temp\\ibdata1", $"{mysql_path}\\data\\ibdata1", true);
            AnsiConsole.MarkupLine($"[green][[INFO]][/] Copied the [yellow]ibdata1[/] file from [green]{mysql_path}\\data-temp[/] to [green]{mysql_path}\\data[/]");

            // Cleaning up the temporary directory "data-temp"
            Directory.Delete($"{mysql_path}\\data-temp", true);
            AnsiConsole.MarkupLine($"[green][[INFO]][/] Deleted [green]{mysql_path}\\data-temp[/]");

            AnsiConsole.MarkupLine(finishLogo);

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static bool Prompt(string prompt)
        {
            AnsiConsole.Markup($"[yellow][[PROMPT]][/] {prompt} [[y/n]]: ");

            var response = Console.ReadLine();

            if (!response.Equals("Y") && !response.Equals("y") && !response.Equals("N") && !response.Equals("n"))
            {
                AnsiConsole.Markup($"[red][[ERROR]][/] Invalid response. Valid responses are: [green]Y[/], [green]y[/], [red]N[/], [red]n[/]");
                Environment.Exit(1);
            }

            return response.Equals("y") || response.Equals("Y");
        }
    }
}