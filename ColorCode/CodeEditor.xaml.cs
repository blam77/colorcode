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
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.System;
using System.Text.RegularExpressions;
using Windows.UI.Text;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ColorCode
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class CodeEditor : Page
    {
        //string textFromCodePad;
        StorageFile globalFile;
        Boolean isCtrlKeyPressed;
        List<int> highlights;
        List<string> wordsToHighlight;
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
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
              StorageFile file = e.Parameter as StorageFile;
              CodePad.Text = "";
              if (file != null)
              {
                  var _Content = await Windows.Storage.FileIO.ReadTextAsync(file);
                  var _Path = file.Path;
                  CodePad.Text = _Content;
                  RichCodePad.Document.SetText(Windows.UI.Text.TextSetOptions.None, _Content);
                  
              }
              globalFile = file;
              //Windows.UI.Text.LineSpacingRule aRule = (Windows.UI.Text.LineSpacingRule)3;
              //aRule.Double = 1;
              ITextParagraphFormat format = RichCodePad.Document.GetDefaultParagraphFormat();
                  format.SetLineSpacing(LineSpacingRule.Exactly, (float)12.3);
                  RichCodePad.Document.SetDefaultParagraphFormat(format);
              
      //      CodePad.Text.setSource(e.stream);
        }
         


        private void RichPad_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Control) isCtrlKeyPressed = false;


            /*string str;
            RichCodePad.Document.GetText(Windows.UI.Text.TextGetOptions.None, out str);
            List<int> intz = new List<int>();
            List<string> words = new List<string>();
            List<int> intz_visited = new List<int>();
            foreach (Match match in Regex.Matches(str, @"\s" + "int" + @"\b|\bint\s|\bint\\;?"))
            {
                    intz.Add(match.Index);
                    words.Add(match.ToString());
            }
            if (e.Key == Windows.System.VirtualKey.Space || e.Key == VirtualKey.Enter)
            {
                var selectTwo = RichCodePad.Document.Selection.StartPosition;

                var select = RichCodePad.Document.Selection;

                //color the textbox black before you recolor blue
                RichCodePad.Document.GetRange(0, str.Length).CharacterFormat.ForegroundColor = Windows.UI.Colors.Black;

                for (int i = 0; i < intz.Count; i++)
                {
                    if (intz_visited.Contains(intz[i]) == false)
                    {
                        select.StartPosition = intz[i];
                        select.EndPosition = intz[i] + words[i].Length;
                        RichCodePad.Document.GetRange(select.StartPosition, select.EndPosition).CharacterFormat.ForegroundColor = Windows.UI.Colors.Blue;
                        intz_visited.Add(i);
                    }
                }

                select.StartPosition = selectTwo;
            }*/
            if (e.Key == Windows.System.VirtualKey.Space || e.Key == VirtualKey.Enter || e.Key == VirtualKey.Back)
            {
                
                syntax_highlight();
                check_lineNumbers();
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
                RichCodePad.Document.GetText(Windows.UI.Text.TextGetOptions.None, out body);


                tempPos = select.StartPosition;
                //First half of the document
                var firstRange = RichCodePad.Document.GetRange(0, tempPos);
                String firstString;
                firstRange.GetText(Windows.UI.Text.TextGetOptions.None, out firstString);

                //Second half of the document
                var secondRange = RichCodePad.Document.GetRange(tempPos, body.Length);
                String secondString;
                secondRange.GetText(Windows.UI.Text.TextGetOptions.None, out secondString);
                int bodyLength = body.Length;
                if (tempPos == bodyLength - 1)
                    secondString = "";

                //Add firstString together with a 'tab'
                body = firstString + "\t";

                //add the second half of the text back
                body += secondString;

                //save cursor to the after tab spot
                RichCodePad.Document.SetText(Windows.UI.Text.TextSetOptions.None, body);
                RichCodePad.Document.Selection.StartPosition = firstString.Length + 1;

                //trying to fix highlighting with tab
                syntax_highlight();
            }
            if (e.Key == Windows.System.VirtualKey.Control) isCtrlKeyPressed = true;
            else if (isCtrlKeyPressed)
            {

                if (e.Key == VirtualKey.O)
                {
                    oButton_Click(this, e);
                }
                if (e.Key == VirtualKey.S)
                {
                    sButton_Click(this, e);
                }


            }
        }

        private void check_lineNumbers()
        {
            string s;
            int count = 0;
            RichCodePad.Document.GetText(Windows.UI.Text.TextGetOptions.None, out s);
           
            foreach (Match m in Regex.Matches(s, @"\r(\w+)|(\w+)\r|\r")) 
            {
                count++;
            }
            LineNumbers.Text = "";
            for (int i = 1; i < count+1; i++)
            {
                LineNumbers.Text += i;
                LineNumbers.Text += "\r";
            }
            
        }

        private void syntax_highlight()
        {
            string str;
            highlights = new List<int>();
            List<int> highlights2 = new List<int>();
            wordsToHighlight = new List<string>();
            List<string> wordsToHighlight2 = new List<string>();
            RichCodePad.Document.GetText(Windows.UI.Text.TextGetOptions.None, out str);
            var selectTwo = RichCodePad.Document.Selection.StartPosition;
            var select1 = RichCodePad.Document.Selection;

                foreach (string s in App.terms.javaSet_type)
                {
                    foreach (Match match in Regex.Matches(str, @"\s" + s + @"\b|\b" + s + @"\s|\b" + s + @"\\;?"))
                    {
                        highlights.Add(match.Index);
                        wordsToHighlight.Add(match.ToString());
                    }
                }

                foreach (string s in App.terms.javaSet_cond)
                {
                    foreach (Match match in Regex.Matches(str, @"\s" + s + @"\b|\b" + s + @"\s|\b" + s + @"\\;?|\b"+s+@"\\{?"))
                    {
                        highlights2.Add(match.Index);
                        wordsToHighlight2.Add(match.ToString());
                    }
                }

                //color the textbox black before you recolor blue
                RichCodePad.Document.GetRange(0, str.Length).CharacterFormat.ForegroundColor = Windows.UI.Colors.Black;

                for (int i = 0; i < highlights.Count; i++)
                {
                    select1.StartPosition = highlights[i];
                    select1.EndPosition = highlights[i] + wordsToHighlight[i].Length;
                    RichCodePad.Document.GetRange(select1.StartPosition, select1.EndPosition).CharacterFormat.ForegroundColor = Windows.UI.Colors.Blue;
                }
                for (int h = 0; h < highlights2.Count; h++)
                {
                    select1.StartPosition = highlights2[h];
                    select1.EndPosition = highlights2[h] + wordsToHighlight2[h].Length;
                    RichCodePad.Document.GetRange(select1.StartPosition, select1.EndPosition).CharacterFormat.ForegroundColor = Windows.UI.Colors.Indigo;
                }


            //still working here!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
               foreach (Match match in Regex.Matches(str, @"\/{2}?([^\r]*)\r"))
                {
                    int first = match.Index;
                    string end = match.ToString();
                    int len = end.Length; 
                    RichCodePad.Document.GetRange(first, first+len).CharacterFormat.ForegroundColor = Windows.UI.Colors.Green;
                }

               foreach (Match match in Regex.Matches(str, @"\b\\?\\/?(\w+)\r\b"))
               {
                   //testing string quotations
               }

            select1.StartPosition = selectTwo;
            select1.EndPosition = selectTwo;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private async void oButton_Click(object sender, RoutedEventArgs e)
        {
            //    string x = nameInput.Text;

            //   StorageFolder storageFolder = KnownFolders.DocumentsLibrary;
            //    StorageFile sampleFile = await storageFolder.CreateFileAsync(x);

            if (EnsureUnsnapped())
            {
                FileOpenPicker openPicker = new FileOpenPicker();
                openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                // Dropdown of file types the user can save the file as
                openPicker.ViewMode = PickerViewMode.List;
                openPicker.FileTypeFilter.Add(".txt");
                //openPicker.FileTypeFilter.Add(".java");

                IAsyncOperation<StorageFile> asyncOp = openPicker.PickSingleFileAsync();
                StorageFile file = await asyncOp;

                if (file != null)
                {


                    //var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);

                    //await new Windows.UI.Popups.MessageDialog(_Content).ShowAsync();

                    if (this.Frame != null)
                    {
                        this.Frame.Navigate(typeof(CodeEditor), file);
                    }
                }
            }

        }

        private async void saButton_Click(object sender, RoutedEventArgs e)
        {
            //    string x = nameInput.Text;

            //   StorageFolder storageFolder = KnownFolders.DocumentsLibrary;
            //    StorageFile sampleFile = await storageFolder.CreateFileAsync(x);

            if (EnsureUnsnapped())
            {
                FileSavePicker savePicker = new FileSavePicker();
                savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                // Dropdown of file types the user can save the file as
                savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
                // Default file name if the user does not type one in or select a file to replace
                savePicker.SuggestedFileName = "New Document";

                IAsyncOperation<StorageFile> asyncOp = savePicker.PickSaveFileAsync();
                StorageFile file = await asyncOp;
                if (file != null)
                {
                    string _Content;
                    RichCodePad.Document.GetText(Windows.UI.Text.TextGetOptions.UseCrlf, out _Content);
                    await FileIO.WriteTextAsync(file, _Content);
             /*       if (this.Frame != null)
                    {
                        this.Frame.Navigate(typeof(CodeEditor), file);
                    }
                    */
                }
                
            }
        }

        private async void sButton_Click(object sender, RoutedEventArgs e)
        {
            //    string x = nameInput.Text;

            //   StorageFolder storageFolder = KnownFolders.DocumentsLibrary;
            //    StorageFile sampleFile = await storageFolder.CreateFileAsync(x);

            if (EnsureUnsnapped())
            {

                
                StorageFile file = globalFile;
                var select = RichCodePad.Document.Selection;

                if (file == null)
                {
                    //insert warning
                }
                else
                {
                    string _Content;
                    RichCodePad.Document.GetText(Windows.UI.Text.TextGetOptions.UseCrlf, out _Content);
                    await FileIO.WriteTextAsync(file, _Content);
                /*    if (this.Frame != null)
                    {
                        CodePad.Text = "";
                        if (file != null)
                        {
                            var _Content1 = await Windows.Storage.FileIO.ReadTextAsync(file);
                            var _Path = file.Path;
                            CodePad.Text = _Content1;
                            RichCodePad.Document.SetText(Windows.UI.Text.TextSetOptions.None, _Content);
                            RichCodePad.Document.Selection.StartPosition = select.StartPosition;

                        }
                    }  */
                }
            }
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            if (this.Frame != null)
            {
                this.Frame.Navigate(typeof(MainPage));
            }
        }

        internal bool EnsureUnsnapped()
        {
            // FilePicker APIs will not work if the application is in a snapped state.
            // If an app wants to show a FilePicker while snapped, it must attempt to unsnap first
            bool unsnapped = ((ApplicationView.Value != ApplicationViewState.Snapped) || ApplicationView.TryUnsnap());
            if (!unsnapped)
            {

            }

            return unsnapped;
        }

        private void RichPad_SelectionChanged(object sender, RoutedEventArgs e)
        {
        }
    }
}
