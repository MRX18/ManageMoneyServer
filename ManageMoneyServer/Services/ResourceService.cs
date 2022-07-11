using Microsoft.Extensions.Localization;
using System.Collections.Generic;

namespace ManageMoneyServer.Services
{
    public class ResourceService
    {
        public readonly IStringLocalizer<Messages> Messages;
        public readonly IStringLocalizer<Fields> Fields;
        public ResourceService(IStringLocalizer<Messages> messages,
            IStringLocalizer<Fields> fields)
        {
            Messages = messages;
            Fields = fields;
        }
        public object Json()
        {
            var json = new 
            { 
                Messages = JsonObject(Messages?.GetAllStrings(true)),
                Fields = JsonObject(Fields?.GetAllStrings(true))
            };

            return json;
        }

        private Dictionary<string, string> JsonObject(IEnumerable<LocalizedString> localizedStrings)
        {
            Dictionary<string, string> json = new Dictionary<string, string>();

            foreach(LocalizedString ls in localizedStrings)
            {
                json.Add(ls.Name, ls.Value);
            }

            return json;
        }
    }
}
