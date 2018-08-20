# QnA Maker と Cognitive Services

## QnA Maker
- [QnA Maker（マイクロソフト公式サイト）](https://azure.microsoft.com/ja-jp/services/cognitive-services/qna-maker/)
- [QnA Maker ドキュメント（マイクロソフト公式サイト）](https://docs.microsoft.com/ja-jp/azure/cognitive-services/QnAMaker/)

Azure の Coginitive Services のひとつ。  
- 既存のコンテンツを使って、自然な会話形式でユーザーに回答する単純な Q&A ができる
- FAQ の URL、構造化されたドキュメント、製品マニュアルを取り込み、考えられるあらゆる質問とその回答のペアをコンテンツから抽出する
- ボットのテストとトレーニングは、使い慣れたチャット インターフェイスを使用して簡単に行うことができる
- QnA Maker を他の API (Language Understanding サービス、Speech API など) とシームレスに統合することにより、さまざまな方法でユーザーからの質問を解釈して回答することができる

価格
|レベル|機能|料金|
|--|--|--|
|Free|3 managed documents; up to 1 MB each|¥0|
|Standars|No limit on the number of managed documents|¥1,120/月|

上記の価格設定とは別に、以下のリソースに対しても支払いが必要となる。QnA Maker リソースを作成すると、自分のAzureサブスクリプションでデータとランタイムをホストすることになるが、これらは Azure Search と App Service によって提供される。
|リソース|機能|料金|
|--|--|--|
|Azure App Service (for the runtime)|[More information](https://azure.microsoft.com/ja-jp/services/app-service/)|[Pricing](https://azure.microsoft.com/ja-jp/pricing/details/app-service/)|
|Azure Search (for the data)|[More information](https://azure.microsoft.com/ja-jp/services/search/)|[Pricing](https://azure.microsoft.com/ja-jp/pricing/details/search/)|

### 作成
ダッシュボードから「リソースの作成」で「QnA Maker」で検索する。

