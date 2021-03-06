﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;

// NOTE: Newtonsoft.Json NuGet パッケージをインストールしてください.
using Newtonsoft.Json;

namespace QnAMaker
{
    class Program
    {
        /// <summary>
        /// エントリーポイント
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // CreateKBメソッドを呼び出してナレッジベースを作成し、
            // ナレッジベースが作成されるまでQnA Maker操作のステータスを定期的にチェック.
            CreateQnA.CreateKB();

            // UpdateKBメソッドを呼び出してナレッジベースを更新し、
            // ナレッジベースが更新されるまでQnA Maker操作のステータスを定期的にチェック.
            UpdateQnA.UpdateKB();

            // PublishKBメソッドを呼び出してナレッジベースを公開する
            PublishQnA.PublishKB();

            Console.ReadLine();
        }
    }
}