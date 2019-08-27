using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using AnimationPlayer.Reflections;

namespace AnimationPlayer.Objects
{
    public class AnimationObject : NotifyPropertyChangedEx
    {
        public AnimationObject()
        {
        }

        public AnimationObject(string name, string image_source, string link)
        {
            this.Name = name;
            this.Image_source = image_source;
            this.Link = link;
        }

        //public string Name { get; set; }
        private string name = "東京喰種 第四季/東京喰種:re 最終章【全 12 集】 ...";
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private string image_source = "https://myself-bbs.com/data/attachment/forum/201809/24/1451116dz9i5o5jz6r9wbl.jpg";
        public string Image_source
        {
            get { return image_source; }
            set { SetProperty(ref image_source, value); }
        }

        private string link = "https://myself-bbs.com/thread-44685-1-1.html";
        public string Link
        {
            get { return link; }
            set { SetProperty(ref link, value); }
        }
    }
}
