using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using MailDaemon.Core;
using MailDaemon.Lib;
using Serilog;
using Serilog.Events;
using System.Diagnostics;

namespace MailDaemon.ConsoleApp
{
    internal class Program
    {
        private static SettingsInfo SettingsInfo;
        private static IMailDaemonService mailDaemonService;
        private static IMailProfileService mailProfileService;
        private static IMailMessageService mailMessageService;
        private static MailProfile mailProfile { get; set; }
        private static MailAgent mailAgent = new();
        private static bool DisplayHelp { get; set; }
        private static string PreviewsDirPath { get; set; }
		private static string ReportsDirPath { get; set; }
        //private static string AppDir { get; set; }
        private static string DiagMessage { get; set; }

        private static void Main(string[] args)
        {
            var configBuilder = new ConfigurationBuilder()
				.SetBasePath(AppContext.BaseDirectory)
				.AddJsonFile("appSettings.json", optional: false);

            var config = configBuilder.Build();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("logs\\log.txt", restrictedToMinimumLevel: LogEventLevel.Debug, rollingInterval: RollingInterval.Day)
                .MinimumLevel.Debug()
                .ReadFrom.Configuration(config)
                .CreateLogger();

            Log.Information("Start application.");

			SettingsInfo = new SettingsInfo();
			mailDaemonService = new MailDaemonService();
			mailProfileService = new JsonMailProfileService();
            mailMessageService = new MailMessageService();
            mailAgent = new MailAgent();
            
            SettingsInfo.AppDirectory = AppDomain.CurrentDomain.BaseDirectory;

            mailProfile = new MailProfile();
            try
            {
                // settings can be loaded from another source, e.g. database.
                SettingsInfo.MailProfileFileName = config["App:MailProfile"];
                //mailProfile.MailBodyTemplateFileName = SettingsInfo.MailProfile;
                DiagMessage = $"Mail profile: \"{SettingsInfo.MailProfileFileName}\"";
                Log.Information(DiagMessage);
                Console.WriteLine(DiagMessage);
                ResetDiagMessage();

                SettingsInfo.SmtpHost = config["MailServer:SmtpHost"];
                SettingsInfo.SmtpPort = Convert.ToInt32(config["MailServer:SmtpPort"]);
                SettingsInfo.SmtpUsername = config["MailServer:SmtpUsername"];
                SettingsInfo.SmtpPassword = config["MailServer:SmtpPassword"];
                SettingsInfo.SmtpEnableSSL = Convert.ToBoolean(config["MailServer:SmtpEnableSSL"]);

                if (mailProfile.Operator is not null && !string.IsNullOrEmpty(mailProfile.Operator.Address))
                {
                    SettingsInfo.Operator = new SenderInfo
                    {
                        Address = mailProfile.Operator.Address,
                        Name = mailProfile.Operator.Name
                    };
                }
                else
                {
                    SettingsInfo.Operator = new SenderInfo
                    {
                        Address = config["Operator:address"],
                        Name = config["Operator:name"]
                    };
                }

                // set SMTP server information
                mailAgent.SmtpHost = SettingsInfo.SmtpHost;
                mailAgent.SmtpPort = SettingsInfo.SmtpPort;
                mailAgent.SmtpUsername = SettingsInfo.SmtpUsername;
                mailAgent.SmtpPassword = SettingsInfo.SmtpPassword;
                mailAgent.SmtpEnableSSL = SettingsInfo.SmtpEnableSSL;
            }
            catch (Exception ex)
            {
                Log.Fatal(GenerateExceptionDetails(ex));
                DisplayErrorMessage(ex.Message);
                WaitForExit();
            }

            if (args.Length > 0)
			{
                Log.Information($"Command line args: mail-daemon {string.Join(" ", args)}");
				try
                {
                    var argIndex = 0;
                    foreach (var arg in args)
                    {
                        switch (arg.ToLower())
                        {
                            case "-v":
                                mailDaemonService.JustValidate = true;
                                break;
                            case "-d":
                                mailDaemonService.SendDemo = true;
                                break;
                            case "-gp":
                                mailDaemonService.GeneratePreview = true;
                                break;
                            case "-p":
                                SettingsInfo.MailProfileFileName = args[argIndex + 1];
                                //mailProfile.MailBodyTemplateFileName = args[argIndex + 1];
                                //mailProfile.MailBodyTemplateFullPath = Path.Combine(SettingsInfo.AppDirectory, SettingsInfo.MailProfilesDirectory, mailProfile.MailBodyTemplateFileName);
                                DiagMessage = $"Mail profile new name: \"{SettingsInfo.MailProfileFileName}\"";
                                Log.Information(DiagMessage);
                                Console.WriteLine(DiagMessage);
                                ResetDiagMessage();
                                break;
                            case "-h":
                                DisplayHelp = true;
                                break;
                        }

                        argIndex++;
                    }
                }
				catch (Exception ex)
				{
                    Log.Fatal(GenerateExceptionDetails(ex));
                    DisplayErrorMessage(ex.Message);
					return;
				}
			}

            if (DisplayHelp)
            {
                Console.WriteLine("Description:");
                Console.WriteLine("-v\t\tValidation mode to verify mail profile integrity. With this argument mails not sending to recipients.");
                Console.WriteLine("-d\t\tSend demo mail only to sender. With this argument mails not sending to recipients.");
                Console.WriteLine("-gp\t\tCreate files on disk with generated mails for each recipient.");
                Console.WriteLine("-p\t\tSet name of the mail profile.");
                WaitForExit();
                return;
            }

            if (!string.IsNullOrEmpty(mailDaemonService.MailProfileFilename) && !File.Exists(mailDaemonService.MailProfileFilename))
            {
                DiagMessage = $"Mail profile \"{mailDaemonService.MailProfileFilename}\" not exists.";
                DisplayErrorMessage(DiagMessage);
                Log.Fatal(DiagMessage);
                WaitForExit();
                return;
            }

            // TBD: Think about whether someone needs this information
            //Console.WriteLine("=== Mail Daemon 0.8 ===");
            //Console.WriteLine("Author:\t\tSergey Drozdov");
            //Console.WriteLine("Email:\t\tsergey.drozdov.0305@gmail.com");
            //Console.WriteLine("Website:\thttps://sd.blackball.lv/sergey-drozdov");
            //Console.Write(Environment.NewLine);

            if (mailDaemonService.JustValidate)
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
                DiagMessage = "--- Validation mode: do not send any mail. Just validate mail profile and recipients.";
                Log.Information(DiagMessage);
				Console.WriteLine(DiagMessage);
				Console.WriteLine("");
				Console.ResetColor();
                ResetDiagMessage();
			}

            DiagMessage = $"Application directory: {SettingsInfo.AppDirectory}";
            Console.WriteLine(DiagMessage);
            Log.Information(DiagMessage);
            ResetDiagMessage();

            SettingsInfo.MailProfileFullPath = Path.Combine(SettingsInfo.AppDirectory, SettingsInfo.MailProfilesDirectory, SettingsInfo.MailProfileFileName);
            try
            {
                //mailDaemonService.MailProfile = mailProfileService.ReadProfile();
                mailProfile = mailProfileService.ReadProfile(SettingsInfo.MailProfileFullPath);
            }
            catch (Exception ex)
            {
                Log.Fatal(GenerateExceptionDetails(ex));
                DisplayErrorMessage(ex.Message);
                WaitForExit();
                return;
            }

            mailProfile.MailBodyTemplateFullPath = Helper.GetMailBodyTemplateFullPath(SettingsInfo, mailProfile.MailBodyTemplateFileName);

            var profileValidation = mailProfileService.ValidateMailProfile(mailProfile);
            if (profileValidation.Count > 0)
            {
                // show errors
                if (profileValidation.Any(x => x.Level == ValidationLevel.Error))
                {
                    SetErrorMessagesStyle();
                    Console.WriteLine("");
                    Console.WriteLine("Errors:");
                    Log.Error("Errors:");
                    foreach (var item in profileValidation.Where(x => x.Level == ValidationLevel.Error))
                    {
                        Log.Error(item.Message);
                        DisplayErrorMessage(item.Message);
                    }
                }

                // show warnings
                if (profileValidation.Any(x => x.Level == ValidationLevel.Warning))
                {
                    SetWarningMessagesStyle();
                    Console.WriteLine("");
                    Console.WriteLine("Warnings:");
                    Log.Warning("Warnings:");
                    foreach (var item in profileValidation.Where(x => x.Level == ValidationLevel.Warning))
                    {
                        Log.Warning(item.Message);
                        DisplayWarningMessage(item.Message);
                    }
                }

                // if mail profile contains errors - stop execution
                if (mailDaemonService.Errors.Count > 0)
                {
                    WaitForExit();
                    return;
                }

                // if mail profile contains warnings - ask user to continue or not
                if (profileValidation.Any(x => x.Level == ValidationLevel.Warning))
                {
                    Console.WriteLine("");
                    Console.Write("Continue? [Y/N]");

                    var confirmed = false;
                    string key;
                    while (!confirmed)
                    {
                        key = Console.ReadLine().ToLower();

                        if (key == "y")
                        {
                            confirmed = true;
                        }
                        if (key == "n")
                        {
                            WaitForExit();
                            return;
                        }
                    }
                }
            }

            ReportsDirPath = Path.Combine(SettingsInfo.AppDirectory, "reports");
            if (!Directory.Exists(ReportsDirPath))
                Directory.CreateDirectory(ReportsDirPath);

            if (mailDaemonService.GeneratePreview)
            {
                try
                {
                    PreviewsDirPath = Path.Combine(SettingsInfo.AppDirectory, "previews", Path.GetFileName(SettingsInfo.MailProfileFileName));
                    if (!Directory.Exists(PreviewsDirPath))
                        Directory.CreateDirectory(PreviewsDirPath);
                    else
                    {
                        foreach (var filePath in Directory.EnumerateFiles(PreviewsDirPath))
                        {
                            File.Delete(filePath);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Fatal(GenerateExceptionDetails(ex));
                    DisplayErrorMessage(ex.Message);
                    WaitForExit();
                    return;
                }
            }

            try
            {
                mailProfile.MailBody = mailProfileService.ReadMailBodyTemplate(mailProfile.MailBodyTemplateFullPath);
            }
            catch (Exception ex)
            {
                Log.Fatal(GenerateExceptionDetails(ex));
                DisplayErrorMessage(ex.Message);
                WaitForExit();
                return;
            }

            // perform recipients
            var counter = 0;
			var recipientsReport = new StringBuilder();
			foreach (var recipient in mailProfile.Recipients.Where(x => !x.Skip.GetValueOrDefault()))
			{
				var recipientReportInfo = new StringBuilder();
				try
                {
                    if (string.IsNullOrEmpty(recipient.MailBodyTemplateFileName))
                    {
                        recipient.MailBodyTemplateFileName = mailProfile.MailBodyTemplateFileName;
                        recipient.MailBodyTemplateFullPath = mailProfile.MailBodyTemplateFullPath;
                        recipient.MailBody = mailProfile.MailBody;
                    }
                    else
                    {
                        // TBD: add support for HTML and plain text files
                        if (!string.IsNullOrEmpty(recipient.MailBodyTemplateFileName))
                        {
                            recipient.MailBodyTemplateFullPath = Helper.GetMailBodyTemplateFullPath(SettingsInfo, recipient.MailBodyTemplateFileName);
                            recipient.MailBody = mailProfileService.ReadMailBodyTemplate(recipient.MailBodyTemplateFullPath);
                        }
                    }

                    var mailMessage = mailMessageService.GenerateMailMessage(SettingsInfo.Operator, mailProfile, recipient);

                    // display mail sending process
                    counter++;
                    if (recipient.Skip.GetValueOrDefault())
                        Console.ForegroundColor = ConsoleColor.DarkGray;
					else
                        Console.ForegroundColor = ConsoleColor.White;
                    //Console.WriteLine($"({counter}) {recipient.Company?.ToUpper()} {recipient.Name}");
                    Console.WriteLine($"({counter}) {recipient.Name}");
                    Console.WriteLine($"Mail: {recipient.Address}");
					Console.WriteLine($"Subject: {mailMessage.Subject}");
					Console.WriteLine($"Template: {(!string.IsNullOrEmpty(recipient.MailBodyTemplateFileName) ? recipient.MailBodyTemplateFileName : mailDaemonService.MailProfile.MailBodyTemplateFileName)}");

                    if (recipient.Skip.GetValueOrDefault())
                        recipientReportInfo.AppendLine("<div style=\"color: #999\">");
                    //recipientReportInfo.AppendLine($"({counter}) {recipient.Company?.ToUpper()} {recipient.Name} <a href=\"mailto:{recipient.Address}\">{recipient.Address}</a>");
                    recipientReportInfo.AppendLine($"({counter}) {recipient.Name} <a href=\"mailto:{recipient.Address}\">{recipient.Address}</a>");
					recipientReportInfo.AppendLine($"<div>Subject: {mailMessage.Subject}</div>");
					if (!string.IsNullOrEmpty(recipient.MailBodyTemplateFileName) && recipient.MailBodyTemplateFileName != mailDaemonService.MailProfile.MailBodyTemplateFileName)
    					recipientReportInfo.AppendLine($"<div>Template: {recipient.MailBodyTemplateFileName}</div>");

                    // recipient related attachments use at first
                    if (recipient.Attachments != null)
                    {
                        foreach (var attachment in recipient.Attachments)
                        {
                            if (File.Exists(attachment.Path))
                            {
                                Console.WriteLine($"\tAttachment: \"{attachment.Path}\"");
                                recipientReportInfo.AppendLine($"<div style=\"padding-left: 40px\">Attachment: \"{attachment.Path}\"</div>");
                            }
                            else
                            {
                                DisplayWarningMessage($"\tAttachment: file \"{attachment.Path}\" not exists.");
                                recipientReportInfo.AppendLine($"<div style=\"padding-left: 40px; color: #aa0000\">Attachment: file \"{attachment.Path}\" not exists.</div>");
                            }
                        }
                    }

                    // attachments
                    if (mailDaemonService.MailProfile.Attachments != null)
					{
						foreach (var attachment in mailDaemonService.MailProfile.Attachments)
						{
							if (File.Exists(attachment.Path))
							{
								Console.WriteLine($"\tAttachment: \"{attachment.Path}\"");
								recipientReportInfo.AppendLine($"<div style=\"padding-left: 40px\">Attachment: \"{attachment.Path}\"</div>");
							}
							else
							{
								DisplayWarningMessage($"\tAttachment: file \"{attachment.Path}\" not exists.");
								recipientReportInfo.AppendLine($"<div style=\"padding-left: 40px\">Attachment: file \"{attachment.Path}\" not exists.</div>");
							}
						}
					}

                    if (recipient.Skip.GetValueOrDefault())
                    {
                        recipientReportInfo.AppendLine("--- Skipped ---");
                        recipientReportInfo.AppendLine("</div>");
                    }

                    if (mailDaemonService.SendDemo)
					{
						Console.ForegroundColor = ConsoleColor.Cyan;
						Console.WriteLine($"--- Send demo to sender address: {mailProfile.Sender.Address} ---");
						Console.ResetColor();
                    }

                    if (mailDaemonService.GeneratePreview)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"--- Create file \"{recipient.Address}.html\" with preview ---");
                        Console.ResetColor();
                        try
                        {
                            var fileNamePrefix = "";
							if (recipient.Skip.GetValueOrDefault())
                                fileNamePrefix = "(skipped)_";
                            var previewFilePath = Path.Combine(PreviewsDirPath, $"{fileNamePrefix}{recipient.Address}{Path.GetExtension(recipient.MailBodyTemplateFileName)}");
                            File.WriteAllText(previewFilePath, mailMessage.Body);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(GenerateExceptionDetails(ex));
                            DisplayErrorMessage(ex.Message);
                        }
                    }

                    if (!mailDaemonService.JustValidate)
					{
                        if (recipient.Skip.GetValueOrDefault())
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine("--- Skipped ---");
                            Console.ResetColor();
                            Console.WriteLine("");
                        }
                        else
                        {
                            var mailSendResult = mailAgent.Send(mailMessage);

						    if (!mailSendResult.Success)
						    {
							    DisplayErrorMessage(mailSendResult.Message);
						    }
						    else
						    {
							    Console.ForegroundColor = ConsoleColor.Green;
							    Console.WriteLine("--- Sent ---");
							    Console.ResetColor();
							    Console.WriteLine("");
						    }
                        }
					}
                    else
                    {
                        if (recipient.Skip.GetValueOrDefault())
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine("--- Skipped ---");
                            Console.ResetColor();
                        }
						Console.WriteLine("");
                    }
                }
				catch (Exception ex)
				{
                    Log.Error(GenerateExceptionDetails(ex));
                    DisplayErrorMessage(ex.Message);
					Console.WriteLine("--- Error ---");
					Console.WriteLine("");
				}

				recipientReportInfo.AppendLine("<br/>");
				recipientsReport.AppendLine(recipientReportInfo.ToString());
				Thread.Sleep(mailDaemonService.SendSleep);
			}

            var report = GenerateReport(mailDaemonService, mailProfile, recipientsReport);
            SaveReportFile(report);

            if (!mailDaemonService.JustValidate)
			{
				try
                {
                    DiagMessage = $"--- Send status report to sender: {mailProfile.Sender.Address} ---";
                    Log.Information(DiagMessage);
                    Console.WriteLine(DiagMessage);
                    ResetDiagMessage();

                    var mailMessage = new MailMessage();
                    mailMessage.To.Add(mailDaemonService.GetMailAddress(SettingsInfo.Operator.Address, SettingsInfo.Operator.Name));
                    mailMessage.From = mailDaemonService.GetMailAddress(mailProfile.Sender.Address, mailProfile.Sender.Name);
                    mailMessage.ReplyToList.Add(mailMessage.From);
                    mailMessage.Headers.Add("Reply-To", mailProfile.Sender.Address);
                    mailMessage.Subject = "Mail Daemon: mails has been sent";
                    mailMessage.SubjectEncoding = Encoding.UTF8;
                    mailMessage.IsBodyHtml = true;
                    mailMessage.BodyEncoding = Encoding.UTF8;
                    mailMessage.Body = report;

                    mailAgent.Send(mailMessage);

                    Console.ForegroundColor = ConsoleColor.Yellow;
					Console.WriteLine("--- Mails has been sent ---");
					Console.ForegroundColor = ConsoleColor.White;
				}
				catch (Exception ex)
				{
                    Log.Error(GenerateExceptionDetails(ex));
                    DisplayErrorMessage(ex.Message);
					Console.WriteLine("--- Error ---");
				}
				Thread.Sleep(5000);
			}

			if (mailDaemonService.JustValidate || mailDaemonService.SendDemo)
                WaitForExit();
		}

        private static string GenerateExceptionDetails(Exception ex)
        {
            var st = new StackTrace(ex, true);
            var frame = st.GetFrame(0);
            var fileName = frame.GetFileName();
            var methodName = frame.GetMethod().Name;
            var line = frame.GetFileLineNumber();
            var col = frame.GetFileColumnNumber();

            return $"File name: {fileName}{Environment.NewLine}Method: {methodName}{Environment.NewLine}Line: {line}{Environment.NewLine}Columns: {col}{Environment.NewLine}{ex}";
        }

        private static string GenerateReport(IMailDaemonService mailDaemonService, MailProfile mailProfile, StringBuilder recipientsReport)
        {
            var report = new StringBuilder();
            report.AppendLine("<!DOCTYPE html>");
            report.AppendLine("<html>");
            report.AppendLine("<head>");
            report.AppendLine("<meta charset=\"utf-8\" />");
            report.AppendLine("<title>Mail Daemon report</title>");
            report.AppendLine("</head>");
            report.AppendLine("<body>");
            report.AppendLine($"<div>{mailProfile.Recipients.Count} mails has been sent.</div>");
            report.AppendLine($"<div>Mail profile: \"{SettingsInfo.MailProfileFileName}\"</div>");
            report.AppendLine($"<div>Mail template: \"{mailProfile.MailBodyTemplateFileName}\"</div>");
            report.AppendLine("<br/>");
            report.AppendLine($"<div><strong>Recipients:</strong></div>");
            report.AppendLine($"<div>{recipientsReport}</div>");
            report.AppendLine("</body>");
            report.AppendLine("</html>");

            return report.ToString();
        }

        private static void SaveReportFile(string report)
        {
            try
            {
                var reportFilePath = Path.Combine(ReportsDirPath, $"report_{Path.GetFileName(SettingsInfo.MailProfileFileName)}_{DateTime.Now:dd.MM.yyyy_HH-mm}.html");
                File.WriteAllText(reportFilePath, report);
            }
            catch (Exception ex)
            {
                Log.Error(GenerateExceptionDetails(ex));
                DisplayWarningMessage(ex.Message);
                Console.Write("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private static void ResetDiagMessage()
        {
            DiagMessage = "";
        }

        private static void SetErrorMessagesStyle()
		{
			Console.ForegroundColor = ConsoleColor.Red;
		}

		private static void SetWarningMessagesStyle()
		{
			Console.ForegroundColor = ConsoleColor.Magenta;
		}

		private static void DisplayErrorMessage(string message)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(message);
			Console.ResetColor();
		}

		private static void DisplayWarningMessage(string message)
		{
			Console.ForegroundColor = ConsoleColor.Magenta;
			Console.WriteLine(message);
			Console.ResetColor();
		}

		private static void WaitForExit()
		{
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("");
			Console.Write("Press any key to exit...");
			Console.ReadKey();
		}
	}
}
