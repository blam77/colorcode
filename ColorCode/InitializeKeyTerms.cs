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

    public class InitializeKeyTerms
    {
        public HashSet<string> javaSet_type;
        public HashSet<string> javaSet_cond;
        string javaTerms;

        public InitializeKeyTerms()
        {
            javaSet_type = new HashSet<string>();
            javaSet_cond = new HashSet<string>();

            //loadFiles();
        }

        public async void loadFiles()
        {
            string path = @"Assets\JavaKeyTerms.txt";
            javaTerms = await ReadFromKeyTermsFile(path);
            loadHashes(javaTerms);
        }

        private void loadHashes(string terms)
        {
            if (terms != null)
            {
                string[] items = terms.Split(new string[] { "=" }, StringSplitOptions.None);
                string[] cond = items[0].Split(new string[] { ";", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                string[] type = items[1].Split(new string[] { ";", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string s in cond)
                {
                    javaSet_cond.Add(s);
                }

                foreach (string s in type)
                {
                    javaSet_type.Add(s);
                }
            }
        }

        private async Task<string> ReadFromKeyTermsFile(string path)
        {
            path = @"Assets\JavaKeyTerms.txt";
            StorageFolder assetFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            StorageFile file = null;
            while (file == null)
            {
                file = await assetFolder.GetFileAsync(path);
            }
            string contents = await FileIO.ReadTextAsync(file);

            if (file != null)
                return contents;
            else
                return "Did not work!";
        }
    }
}
