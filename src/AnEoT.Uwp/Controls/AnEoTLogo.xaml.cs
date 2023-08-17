using System.Collections.Generic;
using System.Numerics;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Shapes;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace AnEoT.Uwp.Controls
{
    public sealed partial class AnEoTLogo : UserControl
    {
        public AnEoTLogo()
        {
            this.InitializeComponent();
        }

        public SolidColorBrush Fill
        {
            get => (SolidColorBrush)GetValue(FillProperty);
            set => SetValue(FillProperty, value);
        }

        // Using a DependencyProperty as the backing store for Fill.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(SolidColorBrush), typeof(AnEoTLogo), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public bool UseShort
        {
            get => (bool)GetValue(UseShortProperty);
            set => SetValue(UseShortProperty, value);
        }

        // Using a DependencyProperty as the backing store for UseShort.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UseShortProperty =
            DependencyProperty.Register("UseShort", typeof(bool), typeof(AnEoTLogo), new PropertyMetadata(false));
    }
}
