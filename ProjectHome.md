## Creating .epub files from your csharp or dotNet application ##

What library does is get a list of html/xhtml files, convert them to xhtml, download all referenced images and crate an .epub archive.

So if you want to create a book from pdf, from msword or txt files you should convert the contents to a list of html files and then use the library on them.

### Usage example: ###
```
using(var f = File.Create("test.epub"))
using(var book = new Book(f){Title = "The sample book"})
{
    book.Authors = new List<string>(){{"The Book Author"}};
    book.AddChapter(new StreamReader(File.OpenRead("2004-06-13-10-56.html")), "2004-06-13-10-56.html",OnImageLoading);
    book.AddChapter(new StreamReader(File.OpenRead("section39.xhtml")), "section39.xhtml", OnImageLoading);
}
```

where you should have method OnImageLoading defined. You might want images to be taken from your local harddrive, then the method looks like that:

```
private static Stream OnImageLoading(string fileName)
{
    return File.OpenRead(fileName);
}
```