using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PamelloV7.Client.Components
{
    public partial class OptionComponent : ContentControl
    {
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.RegisterAttached("IsSelected", typeof(bool), typeof(OptionComponent), new PropertyMetadata(false));
        public bool IsSelected {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.RegisterAttached("Header", typeof(string), typeof(OptionComponent), new PropertyMetadata("Option"));
        public string Header {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty OptionsProperty = DependencyProperty.RegisterAttached("Options", typeof(object), typeof(OptionComponent), new PropertyMetadata(null));
        public object Options {
            get { return GetValue(OptionsProperty); }
            set { SetValue(OptionsProperty, value); }
        }

        public OptionComponent() {
        }
    }
}
