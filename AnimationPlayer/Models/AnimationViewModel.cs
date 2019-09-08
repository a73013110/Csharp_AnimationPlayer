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
            var document = await context.OpenAsync(this.Animation.Href);    
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
                var hrefs = vod.LastElementChild.QuerySelectorAll("li");
                foreach (var href in hrefs)
                {
                    if (href.FirstElementChild.TextContent == "站內")
                    {
                        string dataHref = href.FirstElementChild.GetAttribute("data-href").Replace("\n", string.Empty);
                        string[] data = dataHref.Split('/');    // 動畫ID: data[data.Length - 2], 動畫集數: data[data.Length - 1]
                        // 儲存動畫ID及集數
                        this.VodList.Add(new AnimationVodObject(vod.FirstElementChild.TextContent, data[data.Length - 2] + "/" +  data[data.Length - 1]));
                        break;
                    }
                }
            }
            // 動畫資訊及列表取得完畢事件
            GetAnimationInfoAndVodListCompleted(this, new EventArgs());
        }
    }
}
