using ManageMoneyServer.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ManageMoneyServer.Results
{
    public class JsonResponse : JsonResult
    {
        public JsonResponse() : this(null) { }
        public JsonResponse(object value) 
            : this(string.Empty, value) { }
        public JsonResponse(string message) : this(message, null) { }
        public JsonResponse(string message, object value) 
            : this(HttpStatusCode.OK, message, value) { }
        public JsonResponse(NotificationType notificationType, string message, object value) 
            : this(HttpStatusCode.OK, notificationType, message, value) { }
        public JsonResponse(HttpStatusCode statusCode, string message, object value)
            : this(statusCode, NotificationType.Undefined, message, value) { }
        public JsonResponse(HttpStatusCode statusCode, NotificationType notificationType, string message, object value, object serializerSettings = null) 
            : base(ResponseObject(notificationType, message, value), serializerSettings)
        {
            StatusCode = (int)statusCode;
        }
        private static ResponseViewModel ResponseObject(NotificationType notificationType, string message, object data)
        {
            return new ResponseViewModel
            {
                NotificationType = notificationType,
                Message = message,
                Data = data
            };
        }
    }
}
