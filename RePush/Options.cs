using System;
using System.Collections.Generic;

namespace RePush
{
    public class Options
    {
        public Options(int threadCount, string? method, string url, string? input)
        {
            ThreadCount = threadCount;
            Method = method ?? "GET";
            Url = url;
            Input = input ?? "data.txt";
        }

        public int ThreadCount { get; set; }

        public string Method { get; set; }

        public string Url { get; set; }

        public string Input { get; set; }

        public static Options Build(string[] args)
        {
            var ps = ParseCommandLineArguments(args);

            int number = 1;
            string? mehtod = null;
            string? url = null;
            string? input = null;

            foreach (var p in ps)
            {
                if (p.Key is "thread" or "th")
                {
                    if (!int.TryParse(p.Value, out number) || number <= 0)
                        throw new Exception("--thread 或 -th 参数必须为大于0的数字");
                }
                else if (p.Key is "method" or "m")
                {
                    if (p.Value.ToUpper() is not ("GET" or "POST"))
                        throw new Exception("--method 或 -m 只允许 GET 或 POST");

                    mehtod = p.Value.ToUpper();
                }
                else if (p.Key is "url" or "u")
                {
                    url = p.Value;
                }
                else if (p.Key is "input" or "in")
                {
                    input = p.Value;
                }
            }

            if (number <= 0)
                throw new Exception("--thread 或 -th 参数必须为大于0的数字");

            if (string.IsNullOrWhiteSpace(url))
                throw new Exception("--url 或 -u 为必填项");

            var options = new Options(number, mehtod, url, input);

            return options;
        }

        static Dictionary<string, string> ParseCommandLineArguments(string[] args)
        {
            var parsedArgs = new Dictionary<string, string>();

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];

                // 处理长选项 "--option"
                if (arg.StartsWith("--"))
                {
                    string key = arg.Substring(2);

                    if (i + 1 < args.Length && !args[i + 1].StartsWith("-"))
                    {
                        string value = args[i + 1];
                        parsedArgs[key] = value;
                        i++; // 跳过当前值，因为已经处理了
                    }
                    else
                    {
                        parsedArgs[key] = string.Empty; // 如果没有提供值，则将该选项标记为 Empty
                    }
                }
                // 处理短选项 "-o"
                else if (arg.StartsWith("-"))
                {
                    string key = arg.Substring(1);

                    if (i + 1 < args.Length && !args[i + 1].StartsWith("-"))
                    {
                        string value = args[i + 1];
                        parsedArgs[key] = value;
                        i++; // 跳过当前值，因为已经处理了
                    }
                    else
                    {
                        parsedArgs[key] = string.Empty;
                    }
                }
            }

            return parsedArgs;
        }
    }
}