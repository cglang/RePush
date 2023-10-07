using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RePush
{
    public class Logger
    {
        StreamWriter write;
        public Logger(string filePath)
        {
            write = new StreamWriter(filePath, true);
        }

        public async Task WriteLineAsync(string logtext)
        {
            await write.WriteLineAsync(logtext);
            Console.WriteLine(logtext);
        }

        public static List<string> ReadAllLine(string filePath)
        {
            List<string> datas = new();

            using var stream = new StreamReader(filePath);
            string? text;
            while ((text = stream.ReadLine()) is not null)
            {
                datas.Add(text);
            }

            return datas;
        }

        public void Close()
        {
            write.Close();
        }
    }
}