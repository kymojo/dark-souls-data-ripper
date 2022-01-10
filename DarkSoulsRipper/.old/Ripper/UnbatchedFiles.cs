using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DarkSoulsRipper.Ripper
{
    class UnbatchedFiles<F>
    {
        public Dictionary<String, F> Files { get; set; }

        public UnbatchedFiles() {
            Files = new Dictionary<String, F>();
        }

        public UnbatchedFiles(Dictionary<String, F> fileDictionary) {
            Files = fileDictionary;
        }

        public bool IsEmpty() {
            return (Files.Count == 0);
        }

        public BatchedFiles<F> AsBatchedFiles() {
            Dictionary<String, List<F>> batchDictionary = new Dictionary<String, List<F>>();
            foreach (KeyValuePair<String, F> kvp in Files) {
                List<F> batch = new List<F>();
                batch.Add(kvp.Value);
                batchDictionary.Add(kvp.Key, batch);
            }
            return new BatchedFiles<F>(batchDictionary);
        }

        public void Add(String name, F file) {
            if (Files.ContainsKey(name)) {
                Console.WriteLine($"UnbatchedFiles object already contains entry for '{name}'. Skipping add...");
                return;
            }
            Files.Add(name, file);
        }

        public void Add(UnbatchedFiles<F> otherFiles) {
            foreach (KeyValuePair<String, F> kvp in otherFiles.Files)
                Files.Add(kvp.Key, kvp.Value);
        }

        public void RegexReplaceNames(String nameRegex, String regexFind, String regexReplace) {
            UnbatchedFiles<F> newUnbatched = new UnbatchedFiles<F>();
            foreach (KeyValuePair<String, F> kvp in Files) {
                if (Regex.IsMatch(kvp.Key, nameRegex)) {
                    String newName = Regex.Replace(kvp.Key, regexFind, regexReplace);
                    newUnbatched.Add(newName, kvp.Value);
                    Files.Remove(kvp.Key);
                }
            }
            Add(newUnbatched);
        }
    }
}
