using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DarkSoulsRipper.Ripper
{
    class BatchedFiles<F>
    {
        public Dictionary<String, List<F>> Batches { get; set; }

        public BatchedFiles() {
            Batches = new Dictionary<String, List<F>>();
        }

        public BatchedFiles(Dictionary<String, List<F>> batchDictionary) {
            Batches = batchDictionary;
        }

        public bool IsEmpty() {
            return (Batches.Count == 0);
        }

        public void Batch(String batchMatchRegex, String batchNewNameRegex) {
            BatchedFiles<F> newBatches = new BatchedFiles<F>();
            
            foreach (KeyValuePair<String, List<F>> kvp in Batches) {

                if (Regex.IsMatch(kvp.Key, batchMatchRegex)) {

                    List<F> oldBatch = kvp.Value;
                    String oldBatchName = kvp.Key;
                    Batches.Remove(oldBatchName);

                    String newBatchName = Regex.Replace(oldBatchName, batchMatchRegex, batchNewNameRegex);

                    List<F> newBatch;
                    bool existingNewBatch = newBatches.Batches.TryGetValue(newBatchName, out newBatch);
                    
                    if (!existingNewBatch)
                        newBatch = new List<F>();
                    else
                        newBatches.Batches.Remove(newBatchName);

                    newBatch.AddRange(oldBatch);
                    newBatches.Batches.Add(newBatchName, newBatch);
                }
            }
            if (newBatches.Batches.Count > 0)
                Add(newBatches);
        }

        public void Add(String name, List<F> batch) {
            if (Batches.ContainsKey(name)) {
                Console.WriteLine($"BatchedFiles object already contains entry for '{name}'. Skipping add...");
                return;
            }
            Batches.Add(name, batch);
        }

        public void Add(BatchedFiles<F> otherFiles) {
            foreach (KeyValuePair<String, List<F>> kvp in otherFiles.Batches)
                Batches.Add(kvp.Key, kvp.Value);
        }

        public void RegexReplaceNames(String nameRegex, String regexFind, String regexReplace) {
            BatchedFiles<F> newBatched = new BatchedFiles<F>();
            foreach (KeyValuePair<String, List<F>> kvp in Batches) {
                if (Regex.IsMatch(kvp.Key, nameRegex)) {
                    String newName = Regex.Replace(kvp.Key, regexFind, regexReplace);
                    newBatched.Add(newName, kvp.Value);
                    Batches.Remove(kvp.Key);
                }
            }
            Add(newBatched);
        }
    }
}
