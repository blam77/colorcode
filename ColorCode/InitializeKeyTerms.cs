using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading.Tasks;
using System.IO;
using Windows.Storage;

namespace ColorCode
{

    class InitializeKeyTerms
    {
        public HashSet<string> javaSet_type;
        public HashSet<string> javaSet_cond;
        string javaTerms;

        public InitializeKeyTerms()
        {
            javaSet_type = new HashSet<string>();
            javaSet_cond = new HashSet<string>();

            string[] items = javaTerms.Split(new string[] { "=" }, StringSplitOptions.None);
            string[] cond = items[0].Split(new string[] { ";" }, StringSplitOptions.None);
            string[] type = items[1].Split(new string[] { ";" }, StringSplitOptions.None);
            
            foreach (string s in cond)
            {
                javaSet_cond.Add(s);
            }

            foreach (string s in type)
            {
                javaSet_type.Add(s);
            }
        }

        private async void loadFiles()
        {
            javaTerms = await ReadFromKeyTermsFile("Assets/JavaKeyTerms.txt");
        }

        private async Task<string> ReadFromKeyTermsFile(string path)
        {
            Windows.Storage.StorageFile javaData = null;
            Windows.ApplicationModel.Package pack = Windows.ApplicationModel.Package.Current;
            Windows.Storage.StorageFolder folder = pack.InstalledLocation;
            javaData = await folder.GetFileAsync(path);
            if(javaData != null)
            {
                string fileContent = await FileIO.ReadTextAsync(javaData);
                return fileContent;
            }
            else
            {
                return "File not found";
            }
        }
    }
}
