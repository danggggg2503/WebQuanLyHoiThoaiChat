using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VChatCore.Dto
{
    public class MailClass
    {
        public string EmailFrom { get; set; } = "dangbinz25@gmail.com";
        public string EmailPassword { get; set; } = "0964406792";
        public List<string> ToEmailIds { get; set; } = new List<string>();
        public string Subject { get; set; } = "";
        public string Body { get; set; } = "";
        public bool IsBodyHtml { get; set; } = true;

        public List<string> Attachments { get; set; } = new List<string>();
    }
}
