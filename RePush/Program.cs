using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace RePush
{
    internal class Program
    {
        private static readonly HttpClient httpClient = new();
        static Logger? logger;

        internal static async Task Main(string[] args)
        {
#if DEBUG
            args = new string[] { "-m", "get", "-u", "https://www.baidu.com" };
#endif
            Options option;
            try
            {
                option = Options.Build(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            if (!Directory.Exists("logs"))
                Directory.CreateDirectory("logs");
            logger = new(Path.Combine("logs", $"{DateTime.Now:yyyy-MM-dd}.log"));

            List<string> datas = Logger.ReadAllLine(option.Input);

            ParallelOptions parallelOptions = new() { MaxDegreeOfParallelism = option.ThreadCount };

            int index = 0;
            await Parallel.ForEachAsync(datas, parallelOptions, async (data, _) =>
            {
                string responseBody = string.Empty;
                try
                {
                    responseBody = await SendRequest(option.Method, option.Url, data);
                }
                catch (Exception ex)
                {
                    await logger.WriteLineAsync($"[ERROR]\t[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]:{data}\t{ex.Message}\t{ex}");
                }
                finally
                {
                    await logger.WriteLineAsync($"[INFO]\t[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]:{++index:000000}\t{data}\t{responseBody}");
                }
            });

            if (!Directory.Exists("finish"))
                Directory.CreateDirectory("finish");
            File.Move(option.Input, Path.Combine("finish", $"data.{DateTime.Now:MMdd-HHmmss}.txt"));

            Console.WriteLine("全部已完成");

            Console.ReadKey();
        }

        async static Task<string> SendRequest(string method, string url, string data)
        {
            string responseBody;
            if (method == "POST")
            {
                HttpContent content = new StringContent(data);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                responseBody = await response.Content.ReadAsStringAsync();
            }
            else if (method == "GET")
            {
                HttpResponseMessage response = await httpClient.GetAsync($"{url}?{data}");
                response.EnsureSuccessStatusCode();
                responseBody = await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new Exception("method 错误");
            }

            return responseBody;
        }
    }
}