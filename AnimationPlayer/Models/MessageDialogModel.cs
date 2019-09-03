using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimationPlayer.Reflections;

namespace AnimationPlayer.Models
{
    public class MessageDialogModel : NotifyPropertyChangedEx
    {
        private string icon = "WarningOutline";
        public string Icon
        {
            get { return icon; }
            set { SetProperty(ref icon, value); }
        }

        private string title = "提示：";
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        private string message = "測試訊息";
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        private string btnText = "確定";
        public string BtnText
        {
            get { return btnText; }
            set { SetProperty(ref btnText, value); }
        }
    }
}
