using STTNote.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace STTNote.Helpers
{
    public class ConfigHelper
    {
        public static ConfigHelper INI
        {
            get { return new ConfigHelper(); }
        }

        public List<INISectionItem> GetFromFile(string iniPath)
        {
            var result = new List<INISectionItem>();
            if (!File.Exists(iniPath))
            {
                return result;
            }

            var currentSection = string.Empty;

            var allConfigContents = new List<string>();

            try
            {
                allConfigContents = File.ReadAllLines(iniPath).ToList();
            }
            catch (Exception ex)
            {
                return result;
            }

            foreach (var line in allConfigContents)
            {
                var trimmedLine = line.Trim();

                //Skip empty line
                if (string.IsNullOrEmpty(trimmedLine))
                {
                    continue;
                }

                if (trimmedLine.StartsWith(";"))
                {
                    var newSectionItem = new INISectionItem()
                    {
                        Section = currentSection,
                        Value = trimmedLine,
                        IsComment = true
                    };
                    result.Add(newSectionItem);
                    continue;
                }

                if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                {
                    currentSection = trimmedLine.Substring(1, trimmedLine.Length - 2);
                }
                else if (trimmedLine.Contains("="))
                {
                    var key = trimmedLine.Substring(0, trimmedLine.IndexOf('='))?.Trim() ?? string.Empty;
                    var value = trimmedLine.Substring(trimmedLine.IndexOf('=') + 1).Trim() ?? string.Empty;

                    if (!string.IsNullOrEmpty(key))
                    {
                        var newSectionItem = new INISectionItem()
                        {
                            Section = currentSection,
                            Key = key,
                            Value = value,
                            IsComment = false
                        };
                        result.Add(newSectionItem);
                    }
                }
            }

            return result;
        }

        public void SaveToFile(string iniPath, List<INISectionItem> sections)
        {
            var dirPath = Path.GetDirectoryName(iniPath);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            var tempFile = $"{dirPath}_{Guid.NewGuid()}.ini";
            using (var writer = new StreamWriter(tempFile))
            {
                var sectionGroup = sections.GroupBy(sec => sec.Section);

                foreach (var group in sectionGroup)
                {
                    writer.WriteLine($"[{group.Key}]");

                    foreach (var item in group)
                    {
                        if (item.IsComment)
                        {
                            writer.WriteLine(item.Value);
                        }
                        else
                        {
                            writer.WriteLine($"{item.Key}={item.Value}");
                        }
                    }
                    writer.WriteLine();
                }
                writer.Flush();
            }

            try
            {
                File.Delete(iniPath);
                File.Move(tempFile, iniPath);
            }
            catch
            {
                //
            }
        }
    }
}
