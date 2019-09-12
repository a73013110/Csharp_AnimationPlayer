using AngleSharp;
using AngleSharp.Html.Dom;
using System.Threading.Tasks;
using AnimationPlayer.Objects;
using static AnimationPlayer.GlobalFunctions.AnimationObjectJson;
using System;

namespace AnimationPlayer.Models
{
    public partial class CrawlerModel
    {
        /// <summary>
        /// 輸入動畫連結即可取得動畫名稱及圖片
        /// 結果會立即反映於UI
        /// </summary>
        /// <param name="name">動畫名稱</param>
        /// <param name="href">動畫連結</param>
        /// <returns></returns>
        private async Task AddAnimation(string href)
        {
            // 先檢查先前是否有儲存
            AnimationObject animation = GetAnimationObjectFromJson(href);
            if (animation != null)
            {
                Animations.Add(animation);
                UpdateAnimation(animation);    // 檢查是否要更新動畫資訊
            }
            else    // 若都沒有儲存才至網頁搜尋
            {
                var context = BrowsingContext.New(Configuration.Default.WithDefaultLoader());   // 產生瀏覽網頁的物件
                var document = await context.OpenAsync(href);    // 異步取得網站內容
                var meta = document.QuerySelector("*[name='keywords']") as IHtmlMetaElement;    // 從Meta Tag取得動畫名稱
                var image = document.QuerySelector("div.info_img_box.fl").FirstElementChild as IHtmlImageElement;  // 從網站內取得特定id tag裡面的li
                Animations.Add(new AnimationObject(meta.Content, image.Source, href));
            }
        }
        /// <summary>
        /// 背景更新經儲存的動畫資訊
        /// </summary>
        /// <param name="AnimationObject"></param>
        private void UpdateAnimation(AnimationObject animationObject)
        {
            Task.Run(async () =>
            {
                var context = BrowsingContext.New(Configuration.Default.WithDefaultLoader());   // 產生瀏覽網頁的物件
                var document = await context.OpenAsync(animationObject.Href);    // 異步取得網站內容
                var meta = document.QuerySelector("*[name='keywords']") as IHtmlMetaElement;    // 從Meta Tag取得動畫名稱
                var image = document.QuerySelector("div.info_img_box.fl").FirstElementChild as IHtmlImageElement;  // 從網站內取得特定id tag裡面的li
                animationObject.Name = meta.Content;
                animationObject.Image_source = image.Source;
                SetAnimationObjectToJson(animationObject); // 更新該動畫資訊到檔案
            });
        }
    }
}
