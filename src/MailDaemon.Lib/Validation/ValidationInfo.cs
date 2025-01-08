namespace MailDaemon.Lib
{
    public class ValidationInfo
    {
        public ValidationLevel Level { get; set; } = ValidationLevel.None;
        public string Message { get; set; }
    }
}
