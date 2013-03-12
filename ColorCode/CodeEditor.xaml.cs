using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ComponentModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ColorCode
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class CodeEditor : Page
    {
        string textFromCodePad;
        //string textFromRichPad;

        public CodeEditor()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
              String stream = e.Parameter as String;
              CodePad.Text = "";
              CodePad.Text = stream;
              RichCodePad.Document.SetText(Windows.UI.Text.TextSetOptions.None, stream);
      //      CodePad.Text.setSource(e.stream);
        }

        private void CodePad_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Tab)
            {
                //Testing tab funcationality
                int tempPos = CodePad.SelectionStart;

                //First half of the document
                CodePad.Select(0, tempPos);
                String firstString = CodePad.SelectedText;

                //Second half of the document
                CodePad.Select(tempPos, CodePad.Text.Length);
                String secondString = CodePad.SelectedText;

                //Add firstString together with a 'tab'
                CodePad.Text = firstString + "\t";

                //save cursor to the after tab spot
                int tempPos2 = firstString.Length + 1;

                //add the second half of the text back
                CodePad.Text += secondString;

                //set cursor back to tab spot
                CodePad.SelectionStart = tempPos2;

                textFromCodePad = CodePad.Text;
                //return CodePad.Text;
            }
        }

        private void RichPad_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            
            if (e.Key == Windows.System.VirtualKey.Tab)
            {
                //Testing tab funcationality
                int tempPos;
                var select = RichCodePad.Document.Selection;
                string body;
                RichCodePad.Document.GetText(Windows.UI.Text.TextGetOptions.UseCrlf, out body);


                tempPos = select.StartPosition;
                //First half of the document
                var firstRange = RichCodePad.Document.GetRange(0, tempPos);
                String firstString;
                firstRange.GetText(Windows.UI.Text.TextGetOptions.None, out firstString); ;

                //Second half of the document
                var secondRange = RichCodePad.Document.GetRange(tempPos, body.Length);
                String secondString;
                secondRange.GetText(Windows.UI.Text.TextGetOptions.None, out secondString);

                //Add firstString together with a 'tab'
                body = firstString + "\t";

                //add the second half of the text back
                body += secondString;

                //save cursor to the after tab spot
                RichCodePad.Document.SetText(Windows.UI.Text.TextSetOptions.None, body);
                RichCodePad.Document.Selection.StartPosition = firstString.Length + 1;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            if (this.Frame != null)
            {
                this.Frame.Navigate(typeof(MainPage));
            }
        }
    }
}
