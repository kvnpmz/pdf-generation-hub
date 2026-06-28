var builder = new Builder();
await builder.ExecuteAsync("kidney_foodlist_card", 0);

using var lua = new NLua.Lua();
lua.State.Encoding = System.Text.Encoding.UTF8; 

var myString = (string)lua.DoString("return '😀 ○ café'")[0];
Console.WriteLine(myString);
