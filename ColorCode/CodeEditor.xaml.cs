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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ColorCode
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class CodeEditor : Page
    {
        string textFromCodePad;
        StorageFile globalFile;
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
                    if (this.Frame != null)
                    {
                        this.Frame.Navigate(typeof(CodeEditor), file);
                    }

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
                if (file == null)
                {
                    //insert warning
                }
                else
                {
                    string _Content;
                    RichCodePad.Document.GetText(Windows.UI.Text.TextGetOptions.UseCrlf, out _Content);
                    await FileIO.WriteTextAsync(file, _Content);
                    if (this.Frame != null)
                    {
                        this.Frame.Navigate(typeof(CodeEditor), file);
                    }
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
    }
}
