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


namespace ColorCode
{
    /// <summary>
    /// The code editor of the ColorCode application
    /// </summary>

    public sealed partial class CodeEditor : Page
    {
        //string textFromCodePad;
        StorageFile globalFile;
        Boolean isCtrlKeyPressed;
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

                  syntax_highlight();
                  check_lineNumbers();

                  RichCodePad.Document.Selection.StartPosition = 0;
                  RichCodePad.Document.Selection.EndPosition = 0;



              
      //      CodePad.Text.setSource(e.stream);
        }
         


        private void RichPad_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Control) isCtrlKeyPressed = false;

            string s;
            RichCodePad.Document.GetText(Windows.UI.Text.TextGetOptions.None, out s);

            if (e.Key == Windows.System.VirtualKey.Space || e.Key == VirtualKey.Enter || e.Key == VirtualKey.Back)
            {
                syntax_highlight();
                check_lineNumbers(s);
            }

            int keyCode = (int)e.Key;
            if (keyCode == 222)
            {
                string_Highlighting(s);
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

        private void check_lineNumbers(string s)
        {
            int count = 0;
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
            int first;
            string end;
            int len;
            RichCodePad.Document.GetText(Windows.UI.Text.TextGetOptions.None, out str);
            var selectTwo = RichCodePad.Document.Selection.StartPosition;
            var select1 = RichCodePad.Document.Selection;

            //color the textbox black before you recolor blue
            RichCodePad.Document.GetRange(0, str.Length).CharacterFormat.ForegroundColor = Windows.UI.Colors.Black;
            foreach (string s in App.terms.javaSet_type)
            {
                foreach (Match match in Regex.Matches(str, @"\s" + s + @"\b|\b" + s + @"\s|\b" + s + @"\\;?"))
                {
                    first = match.Index;
                    end = match.ToString();
                    len = end.Length;
                    RichCodePad.Document.GetRange(first, first + len).CharacterFormat.ForegroundColor = Windows.UI.Colors.Blue;
                }
            }

            foreach (string s in App.terms.javaSet_cond)
            {
                foreach (Match match in Regex.Matches(str, @"\s" + s + @"\b|\b" + s + @"\s|\b" + s + @"\\;?|\b"+s+@"\\{?"))
                {
                    first = match.Index;
                    end = match.ToString();
                    len = end.Length;
                    RichCodePad.Document.GetRange(first, first + len).CharacterFormat.ForegroundColor = Windows.UI.Colors.Indigo;
                }
            }

            string_Highlighting(str);
            comment_Highlighting(str);

               
            select1.StartPosition = selectTwo;
            select1.EndPosition = selectTwo;
        }

        private void string_Highlighting(string str)
        {
            //string str;
            //RichCodePad.Document.GetText(Windows.UI.Text.TextGetOptions.None, out str);
            //Regex r = new Regex(@"""[^""\\]*(?:\\.[^""\\]*)*""");
            foreach (Match match in Regex.Matches(str, @"""[^""\\]*(?:\\.[^""\\]*)*"""))
            {
                int first = match.Index;
                string end = match.ToString();
                int len = end.Length;
                RichCodePad.Document.GetRange(first, first + len).CharacterFormat.ForegroundColor = Windows.UI.Colors.Gray;
                //testing string quotations
            }
        }

        private void comment_Highlighting(string str)
        {
            foreach (Match match in Regex.Matches(str, @"\/{2}?([^\r]*)\r"))
            {
                int first = match.Index;
                string end = match.ToString();
                int len = end.Length;
                RichCodePad.Document.GetRange(first, first + len).CharacterFormat.ForegroundColor = Windows.UI.Colors.Green;
            }
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
                openPicker.FileTypeFilter.Add(".java");
                openPicker.FileTypeFilter.Add(".cpp");
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
                savePicker.FileTypeChoices.Add("Java Source File", new List<string>() { ".java" });
                savePicker.FileTypeChoices.Add("C++ Source File", new List<string>() { ".cpp" });
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
            var select = RichCodePad.Document.Selection;
            string str;
            int cursor = select.StartPosition;
            Stack<int> brackets = new Stack<int>();
            RichCodePad.Document.GetText(TextGetOptions.None, out str);
            char[] characters = str.ToCharArray();
            char curr = characters[select.StartPosition];

            //determine whether brackets or parenthesis
            if (curr == '{' || (cursor - 1 >= 0 && characters[cursor - 1] == '}'))
                brackParen_Matching(str, characters, curr, '{', '}');
            else if (curr == '(' || (cursor - 1 >= 0 && characters[cursor-1] == ')'))
                brackParen_Matching(str, characters, curr, '(', ')');
            else
                RichCodePad.Document.GetRange(0, str.Length).CharacterFormat.BackgroundColor = Windows.UI.Colors.Transparent;
        }

        private void brackParen_Matching(string body, char[] bodyChars, char curr, char start, char end)
        {
            //code for highlighting matching brackets/parenthesis
            var select = RichCodePad.Document.Selection;
            int cursor = select.StartPosition;
            Stack<int> brackets = new Stack<int>();

            //clear the background first
            RichCodePad.Document.GetRange(0, body.Length).CharacterFormat.BackgroundColor = Windows.UI.Colors.Transparent;

            if (curr == start)
            {
                int endBracket = -1;
                for (int i = select.StartPosition + 1; i < body.Length; i++)
                {
                    if (bodyChars[i] == end)
                    {
                        if (brackets.Count == 0)
                        {
                            endBracket = i;
                            break;
                        }
                        else
                        {
                            brackets.Pop();
                        }
                    }
                    else if (bodyChars[i] == start)
                    {
                        brackets.Push(i);
                    } 
                }
                if (endBracket != -1)
                {
                    RichCodePad.Document.GetRange(cursor, cursor + 1).CharacterFormat.BackgroundColor = Windows.UI.Colors.LightGray;
                    RichCodePad.Document.GetRange(endBracket, endBracket + 1).CharacterFormat.BackgroundColor = Windows.UI.Colors.LightGray;
                }
            }
            else if (bodyChars[cursor - 1] == end)
            {
                int startBracket = -1;
                for (int i = select.StartPosition - 2; i >= 0; i--)
                {
                    if (bodyChars[i] == start)
                    {
                        if (brackets.Count == 0)
                        {
                            startBracket = i;
                            break;
                        }
                        else
                        {
                            brackets.Pop();
                        }
                    }
                    else if (bodyChars[i] == end)
                    {
                        brackets.Push(i);
                    }
                }
                if (startBracket != -1)
                {
                    RichCodePad.Document.GetRange(cursor - 1, cursor).CharacterFormat.BackgroundColor = Windows.UI.Colors.LightGray;
                    RichCodePad.Document.GetRange(startBracket, startBracket + 1).CharacterFormat.BackgroundColor = Windows.UI.Colors.LightGray;
                }
            }
        }
    }
}
