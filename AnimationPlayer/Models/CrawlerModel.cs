using AngleSharp;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using AnimationPlayer.Objects;
using AnimationPlayer.UserControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static AnimationPlayer.GlobalFunctions.AnimationObjectJson;

namespace AnimationPlayer.Models
{
    public partial class CrawlerModel
    {
        public CrawlerModel()
        {

        }

        // 熱門動畫的屬性(Property), 修改內容會直接反映在UI上
        public ObservableCollection<AnimationObject> Animations { get; set; } = new ObservableCollection<AnimationObject>();

        /// <summary>
        /// 取得搜尋動畫
        /// </summary>
        /// <returns></returns>
        public async Task GetSearchAnimations(string search)
        {
            Chrome chrome = new Chrome();  // 創建爬蟲工具
            #region 搜尋動畫方法(意外突破該網站15秒內不得重複搜尋的限制)
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.PB_Progress.Visibility = Visibility.Visible;
            mainWindow.SB_Hint.MessageQueue.Enqueue("清空動畫列表", "確認", () => mainWindow.SB_Hint.IsActive = false);
            this.Animations.Clear();    // 重置Animation
            mainWindow.SB_Hint.MessageQueue.Enqueue("初始化搜尋", "確認", () => mainWindow.SB_Hint.IsActive = false);
            await chrome.Initial();    // 確認chrome是否已經初始化
            mainWindow.SB_Hint.MessageQueue.Enqueue("正在搜尋動畫...", "確認", () => mainWindow.SB_Hint.IsActive = false);
            await chrome.Load("https://myself-bbs.com/search.php?mod=forum");  // 載入動畫搜尋網站
            mainWindow.SB_Hint.MessageQueue.Enqueue("正在取得搜尋動畫", "確認", () => mainWindow.SB_Hint.IsActive = false);
            string script = $"document.querySelector('#scform_srchtxt').value = '{search}';document.querySelector('#scform_submit').click();";
            await chrome.ExecuteScript(script);    // 透過JavaScript執行搜尋
            string source = chrome.GetSource();
            #endregion

            #region 取得動畫搜尋網站結果並Parser Html
            var Parser = new HtmlParser();
            var document = Parser.ParseDocument(source);
            #endregion

            #region 從搜尋的動畫網址取得適當的動畫連結
            bool noAnimation = true; // 紀錄是否有搜尋到的動畫
            var searchList = document.QuerySelector("#threadlist"); // 搜尋結果, 若為null表示沒搜尋到
            if (searchList != null)
            {
                var searchAnimations = searchList.QuerySelectorAll("li");    // 取得所有搜尋到的資料(可能包含心得等非動畫的資訊)
                foreach (var searchAnimation in searchAnimations)
                {
                    // 把非動畫資訊過濾掉
                    if (!searchAnimation.LastElementChild.LastElementChild.TextContent.Contains("動畫連載列表") &&
                        !searchAnimation.LastElementChild.LastElementChild.TextContent.Contains("完結動畫全集")) continue;
                    noAnimation = false;
                    // 取得動畫的名稱及連結, 並加入UserControl
                    _ = this.AddAnimation("https://myself-bbs.com/thread-" + searchAnimation.GetAttribute("id") + "-1-1.html");
                }
            }
            #endregion

            if (noAnimation)
            {
                ((MessageDialogUserControl)mainWindow.DH_Dialog.DialogContent).MessageDialogModel.Message = "未搜尋到相關動畫, 請重新搜尋";
                mainWindow.DH_Dialog.IsOpen = true;
            }
            else mainWindow.SB_Hint.MessageQueue.Enqueue("搜尋動畫取得完畢, 等待介面顯示...", "確認", () => mainWindow.SB_Hint.IsActive = false);
            mainWindow.PB_Progress.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 取得熱門動畫
        /// </summary>
        /// <returns></returns>
        public async Task GetPopularAnimations()
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.PB_Progress.Visibility = Visibility.Visible;
            mainWindow.SB_Hint.MessageQueue.Enqueue("清空動畫列表", "確認", () => mainWindow.SB_Hint.IsActive = false);
            this.Animations.Clear();    // 重置Animation
            mainWindow.SB_Hint.MessageQueue.Enqueue("正在取得熱門動畫", "確認", () => mainWindow.SB_Hint.IsActive = false);
            string URL = "https://myself-bbs.com/portal.php"; // 動畫網站
            var context = BrowsingContext.New(Configuration.Default.WithDefaultLoader());   // 產生瀏覽網頁的物件
            var document = await context.OpenAsync(URL);    // 取得網站內容
            var hotAnimations = document.QuerySelector("#portal_block_953_content").QuerySelectorAll("li"); // 從網站內取得熱門動畫
            // 取得動畫的名稱及連結, 並加入UserControl
            foreach (var hotAnimation in hotAnimations)
            {
                _ = this.AddAnimation(((IHtmlAnchorElement)hotAnimation.FirstElementChild).Href);
            }
            mainWindow.SB_Hint.MessageQueue.Enqueue("熱門動畫取得完畢, 等待介面顯示...", "確認", () => mainWindow.SB_Hint.IsActive = false);
            mainWindow.PB_Progress.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 取得我的最愛動畫
        /// </summary>
        /// <returns></returns>
        public void GetFavoriteAnimations()
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.PB_Progress.Visibility = Visibility.Visible;
            this.Animations.Clear();    // 重置Animation
            mainWindow.SB_Hint.MessageQueue.Enqueue("正在取得最愛的動畫", "確認", () => mainWindow.SB_Hint.IsActive = false);
            HashSet<AnimationObject> animations = GetAnimationObjectHashSetFromJson();
            if (animations.Count > 0)
            {
                foreach (AnimationObject animation in animations.Reverse()) // 反序添加Animation, animations的順序: 舊->新
                {
                    if (animation.IsFavaorite)
                    {
                        Animations.Add(animation);   // 只取得最愛的動畫
                        UpdateAnimation(animation);    // 更新動畫資訊
                    }
                }
            }

            if (Animations.Count == 0)
            {
                ((MessageDialogUserControl)mainWindow.DH_Dialog.DialogContent).MessageDialogModel.Message = "沒有最愛的動畫";
                mainWindow.DH_Dialog.IsOpen = true;
            }
            mainWindow.PB_Progress.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 取得最近觀看動畫
        /// </summary>
        /// <returns></returns>
        public void GetRecentAnimations()
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.PB_Progress.Visibility = Visibility.Visible;
            this.Animations.Clear();    // 重置Animation
            mainWindow.SB_Hint.MessageQueue.Enqueue("正在取得最近觀看動畫", "確認", () => mainWindow.SB_Hint.IsActive = false);
            HashSet<AnimationObject> animations = GetAnimationObjectHashSetFromJson();
            if (animations.Count > 0)
            {
                foreach (AnimationObject animation in animations.Reverse()) // 反序添加Animation
                {
                    if (animation.Recent_Watch_Index >= 0)
                    {
                        Animations.Add(animation);   // 只取得最近觀看過的動畫
                        UpdateAnimation(animation);    // 更新動畫資訊
                    }
                }
            }

            if (Animations.Count == 0)
            {
                ((MessageDialogUserControl)mainWindow.DH_Dialog.DialogContent).MessageDialogModel.Message = "近期無觀看動畫";
                mainWindow.DH_Dialog.IsOpen = true;
            }
            mainWindow.PB_Progress.Visibility = Visibility.Collapsed;
        }
    }
}
