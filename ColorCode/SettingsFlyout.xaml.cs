using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI;
//using SDKTemplate;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ColorCode
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsFlyout : ColorCode.Common.LayoutAwarePage
    {
        // The guidelines recommend using 100px offset for the content animation.
        const int ContentAnimationOffset = 100;

        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        //MainPage rootPage = MainPage.Current;

        public SettingsFlyout()
        {
            this.InitializeComponent();

            ComboBoxItem light = new ComboBoxItem();
            light.Name = "Light";
            ComboBoxItem dark = new ComboBoxItem();
            dark.Name = "Dark";
            ComboBoxItem gators = new ComboBoxItem();
            gators.Name = "Gators";
            ThemeBox.Items.Add(light);
            ThemeBox.Items.Add(dark);
            ThemeBox.Items.Add(gators);

            FlyoutContent.Transitions = new TransitionCollection();
            FlyoutContent.Transitions.Add(new EntranceThemeTransition()
            {
                FromHorizontalOffset = (SettingsPane.Edge == SettingsEdgeLocation.Right) ? ContentAnimationOffset : (ContentAnimationOffset * -1)
            });

            ThemeBox = new ComboBox();
        }

        /// <summary>
        /// This is the click handler for the back button on the Flyout.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MySettingsBackClicked(object sender, RoutedEventArgs e)
        {
            // First close our Flyout.
            Popup parent = this.Parent as Popup;
            if (parent != null)
            {
                parent.IsOpen = false;
            }

            // If the app is not snapped, then the back button shows the Settings pane again.
            if (Windows.UI.ViewManagement.ApplicationView.Value != Windows.UI.ViewManagement.ApplicationViewState.Snapped)
            {
                SettingsPane.Show();
            }
        }

        /// <summary>
        /// This is the a common click handler for the buttons on the Flyout.  You would replace this with your own handler
        /// if you have a button or buttons on this page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FlyoutButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                //rootPage.NotifyUser("You selected the " + b.Content + " button", NotifyType.StatusMessage);
            }
        }

        private void ThemeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


            try
            {
                //List<ComboBoxItem> lst = new List<ComboBoxItem>();
                //lst.Add(Light);
                //lst.Add(Dark);
                //lst.Add(Gators);
                //ThemeBox.ItemsSource = lst;
                //ThemeBox.SelectedItem = lst.IndexOf(Light);
                var dis = ThemeBox.DisplayMemberPath;
                var obj = ThemeBox.SelectedItem;
                var not = ThemeBox.SelectedIndex;
                var val = ThemeBox.SelectedValue;
                if (ThemeBox.SelectedItem == "Light")
                {
                    CodeEditor.textColor = Colors.Black;
                    CodeEditor.color1 = Colors.Blue;
                    CodeEditor.color2 = Colors.Indigo;
                    CodeEditor.commentColor = Colors.Green;
                    CodeEditor.commentColor = Colors.Gray;
                }
                else if (ThemeBox.SelectedItem == "Dark")
                {

                    CodeEditor.textColor = Colors.White;
                    CodeEditor.color1 = Colors.Red;
                    CodeEditor.color2 = Colors.GreenYellow;
                    CodeEditor.commentColor = Colors.Goldenrod;
                    CodeEditor.commentColor = Colors.Ivory;
                }
            }
            catch (NullReferenceException err)
            {
                var obje = err;
            }

        }
    }
}
