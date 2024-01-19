using CefSharp;
using CefSharp.Wpf;
using LegendLauncher.Managers;
using System;
using System.Collections.Generic;
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

namespace LegendLauncher.View
{
    public partial class MapView : UserControl
    {
        private readonly ChromiumWebBrowser browser;

        public MapView()
        {
            InitializeComponent();
            browser = new ChromiumWebBrowser();
            Browser.Address = "legend.myftp.org:8123";
            // this.Height = height;
            // this.Width = width;
            // Browser.LayoutTransform = new ScaleTransform(scale, scale);
        }
    }
}
