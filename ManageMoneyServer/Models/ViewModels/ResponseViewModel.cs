namespace ManageMoneyServer.Models.ViewModels
{
    public enum NotificationType
    {
        Undefined,
        Success,
        Notification,
        Error
    }
    public class ResponseViewModel
    {
        public NotificationType NotificationType { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
