using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using MailDaemon.Lib;
using System;

namespace MailDaemon.Core
{
    public class JsonMailProfileService : IMailProfileService
    {
        public MailProfile ReadProfile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("File path cannot be empty.");
            }

            try
            {
                var mailProfile = JsonConvert.DeserializeObject<MailProfile>(File.ReadAllText(filePath));
                return mailProfile;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string ReadMailBodyTemplate(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("Mail body template file path cannot be empty.");
            }

            try
            {
                if (File.Exists(filePath))
                {
                    using (var sr = new StreamReader(filePath))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return "";
        }

        public List<ValidationInfo> ValidateMailProfile(MailProfile mailProfile)
        {
            var result = new List<ValidationInfo>();

            // validate sender
            if (mailProfile.Sender == null)
            {
                result.Add(new ValidationInfo()
                {
                    Level = ValidationLevel.Error,
                    Message = $"Mail \"sender\" property in \"{mailProfile.FileName}\" not exists."
                });
            }
            else if (string.IsNullOrEmpty(mailProfile.Sender.Address))
            {
                result.Add(new ValidationInfo()
                {
                    Level = ValidationLevel.Error,
                    Message = $"Mail \"sender\" address value in \"{mailProfile.FileName}\" is empty."
                });
            }
            else if (!mailProfile.Sender.Address.ValidateEmail())
            {
                result.Add(new ValidationInfo()
                {
                    Level = ValidationLevel.Error,
                    Message = $"Mail \"sender\" address \"{mailProfile.Sender.Address}\" not valid."
                });
            }

            if (string.IsNullOrEmpty(mailProfile.Subject))
            {
                result.Add(new ValidationInfo()
                {
                    Level = ValidationLevel.Error,
                    Message = $"Mail \"subject\" value in \"{mailProfile.FileName}\" is empty."
                });
            }

            // validate recipients
            if (mailProfile.Recipients == null)
            {
                result.Add(new ValidationInfo()
                {
                    Level = ValidationLevel.Error,
                    Message = $"Mail \"recipients\" property in \"{mailProfile.FileName}\" not exists."
                });
            }
            else if (mailProfile.Recipients.Count == 0)
            {
                result.Add(new ValidationInfo()
                {
                    Level = ValidationLevel.Error,
                    Message = "No mail recipients found."
                });
            }
            else
            {
                foreach (var recipient in mailProfile.Recipients)
                {
                    if (!recipient.Address.ValidateEmail())
                    {
                        result.Add(new ValidationInfo()
                        {
                            Level = ValidationLevel.Error,
                            Message = $"Mail \"recipient\" address \"{recipient.Address}\" not valid."
                        });
                    }

                    if (!recipient.Skip.GetValueOrDefault() && recipient.Attachments != null)
                    {
                        foreach (var attachment in recipient.Attachments)
                        {
                            if (string.IsNullOrEmpty(attachment.Path))
                            {
                                result.Add(new ValidationInfo()
                                {
                                    Level = ValidationLevel.Warning,
                                    Message = $"Attachment file path for recipient \"{recipient.Address}\" is empty."
                                });
                                continue;
                            }

                            if (!File.Exists(attachment.Path))
                            {
                                result.Add(new ValidationInfo()
                                {
                                    Level = ValidationLevel.Warning,
                                    Message = $"Attachment \"{attachment.Path}\" for recipient \"{recipient.Address}\" not found."
                                });
                            }
                        }
                    }
                }
            }

            // validate mail template
            if (string.IsNullOrEmpty(mailProfile.MailBodyTemplateFileName))
            {
                result.Add(new ValidationInfo()
                {
                    Level = ValidationLevel.Error,
                    Message = $"Mail \"template\" property in \"{mailProfile.FileName}\" is empty."
                });
            }
            else
            {
                if (!File.Exists(mailProfile.MailBodyTemplateFileName))
                {
                    result.Add(new ValidationInfo()
                    {
                        Level = ValidationLevel.Error,
                        Message = $"Mail body template file \"{mailProfile.MailBodyTemplateFullPath}\" not exists."
                    });
                }
            }

            // validate attachments
            if (mailProfile.Attachments != null)
            {
                foreach (var attachment in mailProfile.Attachments)
                {
                    if (string.IsNullOrEmpty(attachment.Path))
                    {
                        result.Add(new ValidationInfo()
                        {
                            Level = ValidationLevel.Warning,
                            Message = $"Attachment file path for mail profile \"{mailProfile.FileName}\" is empty."
                        });
                        continue;
                    }

                    if (!File.Exists(attachment.Path))
                    {
                        result.Add(new ValidationInfo()
                        {
                            Level = ValidationLevel.Error,
                            Message = $"Attachment \"{attachment.Path}\" for mail profile \"{mailProfile.FileName}\" not exists."
                        });
                    }
                }
            }

            return result;
        }
    }
}
