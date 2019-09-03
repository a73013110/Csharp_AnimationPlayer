using AnimationPlayer.Models;
using System.Windows.Controls;

namespace AnimationPlayer.UserControls
{
    /// <summary>
    /// MessageDialogUserControl.xaml 的互動邏輯
    /// </summary>
    public partial class MessageDialogUserControl : UserControl
    {
        public MessageDialogUserControl()
        {
            InitializeComponent();

            this.MessageDialogModel = this.DataContext as MessageDialogModel;
        }

        public MessageDialogModel MessageDialogModel { get; set; }
    }
}
