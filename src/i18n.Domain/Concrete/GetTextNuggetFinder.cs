using i18n.Domain.Abstract;
using i18n.Domain.Entities;
using i18n.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i18n.Domain.Concrete
{
    public class GetTextNuggetFinder : INuggetFinder
    {
        private Settings _settings;

        public GetTextNuggetFinder(Settings settings)
		{
			_settings = settings;
        }

        public IDictionary<string, TemplateItem> ParseAll()
        {
            IEnumerable<string> fileWhiteList = _settings.WhiteList;
            IEnumerable<string> directoriesToSearchRecursively = _settings.DirectoriesToScan;

            string currentFullPath;
            bool blacklistFound = false;

            var templateItems = new ConcurrentDictionary<string, TemplateItem>();
            // Collection of template items keyed by their id.

            var files = new List<string>();

            foreach (var directoryPath in directoriesToSearchRecursively)
            {
                foreach (string filePath in Directory.EnumerateFiles(directoryPath, "*.*", SearchOption.AllDirectories))
                {
                    blacklistFound = false;
                    currentFullPath = Path.GetDirectoryName(Path.GetFullPath(filePath));
                    foreach (var blackItem in _settings.BlackList)
                    {
                        if (currentFullPath == null || currentFullPath.StartsWith(blackItem, StringComparison.OrdinalIgnoreCase))
                        {
                            //this is a file that is under a blacklisted directory so we do not parse it.
                            blacklistFound = true;
                            break;
                        }
                    }
                    if (!blacklistFound)
                    {

                        //we check every filePath against our white list. if it's on there in at least one form we check it.
                        foreach (var whiteListItem in fileWhiteList)
                        {
                            //We have a catch all for a filetype
                            if (whiteListItem.StartsWith("*."))
                            {
                                if (Path.GetExtension(filePath) == whiteListItem.Substring(1))
                                {
                                    //we got a match
                                    files.Add(filePath);
                                    break;
                                }
                            }
                            else //a file, like myfile.js
                            {
                                if (Path.GetFileName(filePath) == whiteListItem)
                                {
                                    //we got a match
                                    files.Add(filePath);
                                    break;
                                }
                            }
                        }

                    }

                }
            }

            string manifest = null;
            try
            {
                manifest = Path.GetTempFileName();
                using (var writer = File.CreateText(manifest))
                {
                    foreach (var file in files)
                    {
                        writer.WriteLine(file);
                    }
                }

                var args = string.Format("-LC# -k_ --omit-header --from-code=UTF-8 -o - -f\"{0}\"", manifest);


                using (var reader = RunWithOutput(_settings.XGetTextPath, args))
                {
                    return ParseOutput(reader).GroupBy(t => t.Id).ToDictionary(g => g.Key, g => g.Last());
                }
            }
            finally
            {
                if (manifest.IsSet() && File.Exists(manifest))
                {
                    File.Delete(manifest);
                }
            }
        }

        private static StreamReader RunWithOutput(string filename, string args)
        {
            var info = new ProcessStartInfo(filename, args)
            {
                UseShellExecute = false,
                ErrorDialog = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            Console.WriteLine("{0} {1}", info.FileName, info.Arguments);
            var process = Process.Start(info);
            while (!process.StandardError.EndOfStream)
            {
                var line = process.StandardError.ReadLine();
                if (line == null)
                {
                    continue;
                }
                Console.WriteLine(line);
            }

            return process.StandardOutput;
        }

        private IEnumerable<TemplateItem> ParseOutput(StreamReader reader)
        {
            TemplateItem current = null;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (current == null)
                {
                    current = new TemplateItem();
                }

                if (line == string.Empty)
                {
                    if (current != null)
                    {
                        yield return current;
                        current = null;
                    }
                    continue;
                }

                if (line.StartsWith("#: "))
                {
                    if (current.References == null)
                    {
                        current.References = new List<string>();
                    }
                    ((List<string>)current.References).Add(line.Substring(3));
                }
                else if (line.StartsWith("#. ") || line.StartsWith("#  "))
                {
                    if (current.Comments == null)
                    {
                        current.Comments = new List<string>();
                    }
                    ((List<string>)current.Comments).Add(line.Substring(3));
                }
                else if (line.StartsWith("msgid "))
                {
                    current.Id = line.Substring(6);
                }
            }

            if (current != null)
            {
                yield return current;
            }
        }
    }
}
