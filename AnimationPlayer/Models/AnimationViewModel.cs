using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimationPlayer.Objects;
using AngleSharp;
using AngleSharp.Html.Dom;

namespace AnimationPlayer.Models
{
    public class AnimationViewModel
    {
        public AnimationViewModel(AnimationObject animation)
        {
            this.Animation = animation;
            _ = GetAnimationInfoAndVodList();            
        }

        public AnimationObject Animation { get; set; }  // 這個UserControl所顯示的動畫
        public event EventHandler GetAnimationInfoAndVodListCompleted;  // 動畫資訊及列表取得完畢事件

        public List<string> Infos = new List<string>();
        public List<AnimationVodObject> VodList = new List<AnimationVodObject>();

        /// <summary>
        /// 取得動畫資訊及動畫列表
        /// </summary>
        /// <returns></returns>
        public async Task GetAnimationInfoAndVodList()
        {
            // 取得網站內容
            var context = BrowsingContext.New(Configuration.Default.WithDefaultLoader());
            var document = await context.OpenAsync(this.Animation.Link);    
            // 取得動畫資訊
            var infos = document.QuerySelector("div.info_info").QuerySelectorAll("li");
            foreach (var info in infos)
            {
                this.Infos.Add(info.TextContent);
            }
            // 取得劇情摘要
            var intro = document.QuerySelector("#info_introduction");
            this.Infos.Add(intro.FirstElementChild.TextContent + ": " + intro.LastElementChild.TextContent);
            // 取得動畫列表
            var vodList = document.QuerySelector("div.fr.vodlist_index").QuerySelector(".main_list").Children;
            foreach (var vod in vodList)
            {
                var links = vod.LastElementChild.QuerySelectorAll("li");
                foreach (var link in links)
                {
                    if (link.FirstElementChild.TextContent == "站內")
                    {
                        string dataHref = link.FirstElementChild.GetAttribute("data-href").Replace("\n", string.Empty);
                        string[] data = dataHref.Split('/');
                        this.VodList.Add(new AnimationVodObject(vod.FirstElementChild.TextContent, "https://vpx.myself-bbs.com/" + data[data.Length - 2] + "/" +  data[data.Length - 1] + "/720p.m3u8"));
                        break;
                    }
                }
            }
            // 動畫資訊及列表取得完畢事件
            GetAnimationInfoAndVodListCompleted(this, new EventArgs());
        }
    }
}
