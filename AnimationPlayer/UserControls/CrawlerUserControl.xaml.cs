﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using AnimationPlayer.Models;
using AnimationPlayer.Objects;
using MaterialDesignThemes.Wpf;
using System.Threading;

namespace AnimationPlayer.UserControls
{
    /// <summary>
    /// CrawlerUserControl.xaml 的互動邏輯
    /// </summary>
    public partial class CrawlerUserControl : UserControl
    {
        public CrawlerUserControl()
        {
            this.DataContext = CrawlerModel = new CrawlerModel();
            InitializeComponent();
            CrawlerModel.Animations.CollectionChanged += AnimationsCollectionChangedEventHandler;
        }

        private readonly CrawlerModel CrawlerModel;

        /// <summary>
        /// 用滑鼠滾輪滾動動畫Panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SV_AnimationViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollviewer = sender as ScrollViewer;
            if (e.Delta > 0)
                scrollviewer.LineLeft();
            else
                scrollviewer.LineRight();
            e.Handled = true;
        }

        /// <summary>
        /// 根據Model的Animations的變動, 於UI上進行更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AnimationsCollectionChangedEventHandler(object sender, NotifyCollectionChangedEventArgs e)
        {
            // 若新增動畫, 將其UserControl加入UI
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewStartingIndex == 0)    // 若有新元素, 則進行介面動畫
                {
                    if (this.Grid_Layout.RowDefinitions[0].Height != new GridLength(0, GridUnitType.Star)) this.FloatingSearchBoxInAnimation(); // 動畫將搜尋窗格往上移, 以於介面上顯示新動畫
                }
                else if (e.NewStartingIndex == 5)    // 若已有5個AnimationObject, 就可以來製作背景圖片囉 >_^
                {
                    for (int i = 0; i < 5; i++)
                    {
                        ((Image)this.Grid_Background.Children[i]).Source = new ImageSourceConverter().ConvertFromString(CrawlerModel.Animations[i].Image_source) as ImageSource;
                    }
                }
                AnimationPreviewUserControl animationUserControl = new AnimationPreviewUserControl((AnimationObject)e.NewItems[0]); // 產生UserControl
                Binding binding = new Binding("ActualHeight")   // 設置欲Binding的Property
                {
                    Source = this.SP_AnimationPanel // 設定欲Binding的ElementName
                };  
                animationUserControl.SetBinding(UserControl.HeightProperty, binding);   // 設置Binding
                this.SP_AnimationPanel.Children.Add(animationUserControl);  // 添加UserControl到視窗
            }
            // 若清除所有動畫, 將UI上的UserControl清除
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                this.SP_AnimationPanel.Children.Clear();
            }            
        }

        /// <summary>
        /// 顯示搜尋項目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Search_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(this.TB_Search.Text)) return; // 若搜尋的字串為空或只有單純空白
            _ = this.CrawlerModel.GetSearchAnimations(this.TB_Search.Text); // 搜尋動畫並顯示
        }

        /// <summary>
        /// 搜尋TextBox按下Enter, 觸發Btn_Search_Click事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TB_Search_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Btn_Search.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }

        /// <summary>
        /// 取得熱門動畫
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Btn_PopularAnimation_Click(object sender, RoutedEventArgs e)
        {
            _ = this.CrawlerModel.GetPopularAnimations();
        }

        /// <summary>
        /// 取得最愛的動畫
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Btn_FavoriteAnimation_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 取得近期觀看的動畫
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Btn_RecentAnimation_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
