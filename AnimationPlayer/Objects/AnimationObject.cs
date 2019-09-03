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

        /// <summary>
        /// 動畫名稱
        /// </summary>
        private string name = "東京喰種 第四季/東京喰種:re 最終章【全 12 集】 ...";
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }
        /// <summary>
        /// 動畫圖片網址
        /// </summary>
        private string image_source = "https://myself-bbs.com/data/attachment/forum/201809/24/1451116dz9i5o5jz6r9wbl.jpg";
        public string Image_source
        {
            get { return image_source; }
            set { SetProperty(ref image_source, value); }
        }
        /// <summary>
        /// 動畫網址
        /// </summary>
        private string href = "https://myself-bbs.com/thread-44685-1-1.html";
        public string Href
        {
            get { return href; }
            set { SetProperty(ref href, value); }
        }
        /// <summary>
        /// 紀錄最近觀看的集數
        /// </summary>
        private int recent_watch_index = -1;
        public int Recent_Watch_Index
        {
            get { return recent_watch_index; }
            set { SetProperty(ref recent_watch_index, value); }
        }
        /// <summary>
        /// 紀錄是否為最愛
        /// </summary>
        private bool isFavaorite = false;
        public bool IsFavaorite
        {
            get { return isFavaorite; }
            set { SetProperty(ref isFavaorite, value); }
        }
    }
}
