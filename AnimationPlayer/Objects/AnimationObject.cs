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

        public AnimationObject(string name, string image_source, string href)
        {
            this.Name = name;
            this.Image_source = image_source;
            this.Href = href;
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

        private string href = "https://myself-bbs.com/thread-44685-1-1.html";
        public string Href
        {
            get { return href; }
            set { SetProperty(ref href, value); }
        }

        /// <summary>
        /// 是否為紀錄最近觀看
        /// </summary>
        private int recent_watch_index = -1;
        public int Recent_Watch_Index
        {
            get { return recent_watch_index; }
            set { SetProperty(ref recent_watch_index, value); }
        }
    }
}
