using AnimationPlayer.Objects;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using static AnimationPlayer.GlobalFunctions.AnimationObjectJson;

namespace AnimationPlayer.UserControls
{
    public partial class CrawlerUserControl
    {
        /// <summary>
        /// 搜尋框上浮動畫
        /// </summary>
        private void FloatingSearchBoxInAnimation()
        {
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(10);
            double decrease = 0.07; // 初速
            double alpha = 0.952;   // 衰減率
            double beta = 0.00005;   // 最低速度
            timer.Tick += ((sender, e) =>
            {
                this.Grid_Layout.RowDefinitions[0].Height = new GridLength(this.Grid_Layout.RowDefinitions[0].Height.Value - decrease, GridUnitType.Star);
                this.Grid_Layout.RowDefinitions[2].Height = new GridLength(this.Grid_Layout.RowDefinitions[2].Height.Value + decrease, GridUnitType.Star);
                decrease = decrease * alpha + beta;
                //Console.WriteLine(decrease);
                if (this.Grid_Layout.RowDefinitions[0].Height.Value - decrease < 0)
                {
                    this.Grid_Layout.RowDefinitions[0].Height = new GridLength(0, GridUnitType.Star);
                    this.Grid_Layout.RowDefinitions[2].Height = new GridLength(3, GridUnitType.Star);
                    timer.Stop();
                }
            });
            timer.Start();
        }

        /// <summary>
        /// 新增AnimationPreviewUserControl
        /// </summary>
        /// <param name="animationObject"></param>
        private void AddAnimationPreviewUserControl(AnimationObject animationObject)
        {
            AnimationPreviewUserControl animationUserControl = null;
            // 根據模式顯示對應的AnimationPreviewUserControl
            switch (this.CurrentCrawlerMode)
            {
                case CrawlerMode.Recent:    // 近期觀看
                    animationUserControl = new AnimationPreviewUserControl(animationObject, (sender, eventArgs) =>
                    {
                        AnimationObject animationObj = ((Button)sender).DataContext as AnimationObject;
                        animationObj.Recent_Watch_Index = -1;   // 重設該動畫的最近觀看index
                        SetAnimationObjectToJson(animationObj); // 更新該動畫資訊到檔案
                        this.SP_AnimationPanel.Children.Remove(animationUserControl);   // 刪除UI
                    });
                    break;
                default:    // 其餘的模式
                    animationUserControl = new AnimationPreviewUserControl(animationObject);
                    break;
            }
            // 將animationUserControl的高度與其parent進行綁定
            Binding binding = new Binding("ActualHeight")
            {
                Source = this.SP_AnimationPanel // 設定欲Binding的ElementName
            };
            animationUserControl.SetBinding(UserControl.HeightProperty, binding);   // 設置Binding
            this.SP_AnimationPanel.Children.Add(animationUserControl);  // 添加UserControl到視窗
        }
    }
}
