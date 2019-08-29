using AngleSharp;
using AngleSharp.Html.Dom;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using AnimationPlayer.Objects;
using MaterialDesignThemes.Wpf;
using MahApps.Metro.Controls;
using AnimationPlayer.UserControls;
using AngleSharp.Html.Parser;
using CefSharp;
using CefSharp.OffScreen;
using System.Threading;
using static AnimationPlayer.GlobalFunctions.AnimationObjectJson;
using System.Collections.Generic;
using System.Windows.Threading;

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
        private readonly CefBrowser Cef = new CefBrowser();
        public async Task GetSearchAnimations(string search)
        {
            #region 使用CefBrowser搜尋動畫(意外突破該網站15秒內不得重複搜尋的限制)
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.PB_Progress.Visibility = Visibility.Visible;
            mainWindow.SB_Hint.MessageQueue.Enqueue("清空動畫列表", "確認", () => mainWindow.SB_Hint.IsActive = false);
            this.Animations.Clear();    // 重置Animation
            mainWindow.SB_Hint.MessageQueue.Enqueue("初始化搜尋", "確認", () => mainWindow.SB_Hint.IsActive = false);
            bool loadEnd = false;   // 標記CefBrowser是否已經載入完畢
            void FrameLoadEndEvent(object s, FrameLoadEndEventArgs e) => loadEnd = true;    // CefBrowser載入完畢事件
            Cef.Browser.FrameLoadEnd += FrameLoadEndEvent;  // 註冊CefBrowser載入完畢事件
            Cef.Browser.Load("https://myself-bbs.com/search.php?mod=forum");    // 載入動畫搜尋網站
            await Task.Run(() => SpinWait.SpinUntil(() => loadEnd, 3000));  // 等待CefBrowser載入完畢
            loadEnd = false;    // 重設標記
            mainWindow.SB_Hint.MessageQueue.Enqueue("正在搜尋動畫...", "確認", () => mainWindow.SB_Hint.IsActive = false);
            string script = $"document.querySelector('#scform_srchtxt').value = '{search}';document.querySelector('#scform_submit').click();";
            Cef.Browser.GetMainFrame().ExecuteJavaScriptAsync(script);  // 透過JavaScript執行搜尋
            await Task.Run(() => SpinWait.SpinUntil(() => loadEnd, 3000));  // 等待CefBrowser載入完畢
            Cef.Browser.FrameLoadEnd -= FrameLoadEndEvent;  // 註銷CefBrowser載入完畢事件
            #endregion
            
            #region 取得動畫搜尋網站結果並Parser Html
            string source = await Cef.Browser.GetSourceAsync();
            var Parser = new HtmlParser();
            var document = Parser.ParseDocument(source);  
            #endregion

            #region [已棄用]舊版(透過HTTP傳輸資料)搜尋動畫方法
            //#region 取得搜尋動畫時post所需的資料(formhash和searchsubmit)
            //string URL = "https://myself-bbs.com/search.php?mod=forum"; // 動畫搜尋網站
            //var context = BrowsingContext.New(Configuration.Default.WithDefaultLoader());   // 產生瀏覽網頁的物件
            //var document = await context.OpenAsync(URL);    // 取得網站內容
            //// 取得2個Post要用到的資訊
            //var formhash = document.QuerySelector("input[name='formhash']") as IHtmlInputElement;
            //var searchsubmit = document.QuerySelector("input[name='searchsubmit']") as IHtmlInputElement;
            //#endregion

            //#region 透過post的方式取得搜尋的動畫網址
            //mainWindow.SB_Hint.MessageQueue.Enqueue("正在搜尋動畫...", "確認", () => mainWindow.SB_Hint.IsActive = false);
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);    // 設定Request物件及網址
            //request.Method = "POST";    // 設定Request方法
            //request.ContentType = "application/x-www-form-urlencoded";
            //var sb = new StringBuilder($"formhash={formhash.Value}&srchtxt={search}&searchsubmit={searchsubmit.Value}");    // 設定Post資訊
            //var byteArray = Encoding.UTF8.GetBytes(sb.ToString());  // 使用UTF8編碼(否則中文會變亂碼)轉成byteArray(指定的Post資訊格式)
            //using (Stream reqStream = request.GetRequestStream())   // 開始Post
            //{
            //    reqStream.Write(byteArray, 0, byteArray.Length);
            //}
            //WebResponse response = await request.GetResponseAsync();
            //string searchAnimationsHref = response.ResponseUri.AbsoluteUri;    // 取得搜尋的動畫網址
            //#endregion 
            #endregion

            #region 從搜尋的動畫網址取得適當的動畫連結
            mainWindow.SB_Hint.MessageQueue.Enqueue("正在取得搜尋動畫", "確認", () => mainWindow.SB_Hint.IsActive = false);
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
            HashSet<AnimationObject> recentWatch = GetAnimationObjectHashSetFromJson();
            if (recentWatch.Count > 0)
            {
                foreach (AnimationObject animation in recentWatch)
                {
                    if (animation.IsFavaorite) Animations.Add(animation);   // 只取得最近觀看過的
                }
                mainWindow.SB_Hint.MessageQueue.Enqueue("最愛的動畫取得完畢, 等待介面顯示...", "確認", () => mainWindow.SB_Hint.IsActive = false);
            }

            if (Animations.Count == 0)
            {
                ((MessageDialogUserControl)mainWindow.DH_Dialog.DialogContent).MessageDialogModel.Message = "近期無觀看動畫";
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
            HashSet<AnimationObject> recentWatch = GetAnimationObjectHashSetFromJson();
            if (recentWatch.Count > 0)
            {
                foreach (AnimationObject animation in recentWatch)
                {
                    if (animation.Recent_Watch_Index >= 0) Animations.Add(animation);   // 只取得最近觀看過的
                }
                mainWindow.SB_Hint.MessageQueue.Enqueue("最近觀看動畫取得完畢, 等待介面顯示...", "確認", () => mainWindow.SB_Hint.IsActive = false);
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
