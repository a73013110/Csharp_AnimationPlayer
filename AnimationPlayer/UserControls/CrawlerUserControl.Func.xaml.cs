using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace AnimationPlayer.UserControls
{
    public partial class CrawlerUserControl
    {
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
    }
}
