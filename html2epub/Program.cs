using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using epub;
using libEpub;

namespace html2epub
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> parameters = args.Where(i => !string.IsNullOrEmpty(i) && i.StartsWith("-")).ToList();
            var title =
                parameters.Where(i => i.StartsWith("-title")).DefaultIfEmpty("-title=\"unknown\"").First().Split('=').
                    Skip(1).DefaultIfEmpty("\"unknown\"").First();

            var author =
                parameters.Where(i => i.StartsWith("-author")).DefaultIfEmpty("-title=\"unknown\"").First().Split('=').
                    Skip(1).DefaultIfEmpty("\"unknown\"").First();

            var encoding =
                parameters.Where(i => i.StartsWith("-encoding")).DefaultIfEmpty("-encoding=").First().Split('=').
                    Skip(1).DefaultIfEmpty(null).First();

            using(var f = File.Create(string.Format("{0}.zip", title)))
            {
                using (var book = new Book(f) { Title = title })
                {
                    book.Authors = new List<string>() { { author } };
                    args.Where(i => !string.IsNullOrEmpty(i) && !i.StartsWith("-")).ToList().
                    ForEach(
                        chapter =>
                            book.AddChapter(
                                new StreamReader(File.OpenRead(chapter), string.IsNullOrEmpty(encoding) ? Encoding.Default : Encoding.GetEncoding(encoding)),
                                chapter,
                                OnImageLoading));
                }
            }
            
            
/*
            if(args.Length != 2)
            {
                Console.WriteLine("USAGE: html2epub <directory with contents> <output directory>");
                return;
            }

            var inDir = args[0];
            var outDir = args[1];

            if(Directory.Exists(outDir))
            {
                Console.WriteLine("Directory {0} already exists",outDir);
                return;
            }

            Directory.CreateDirectory(outDir);

            using(var mimeFile = new StreamWriter(Path.Combine(outDir,"mimetype")))
            {
                mimeFile.WriteLine("application/epub+zip");
            }

            var ncx = new epub.ncx()
                          {
                              docTitle = new ncxDocTitle(){text = "Gennady Alexandrov. Livejournal archive"},
                              version = "2005-1",
                              head = new []
                              { 
                                  new ncxMeta() { content = "FB2BookID", name = "dtb:uid" },
                                  new ncxMeta() { content = "2", name = "dtb:depth" },
                                  new ncxMeta() { content = "0", name = "dtb:totalPageCount" },
                                  new ncxMeta() { content = "0", name = "dtb:maxPageNumber" },
                              },
                          };

            int navPointCounter = 1;

            var ncxNavPoints = new List<ncxNavPoint>();


            foreach (var fileName in Directory.GetFiles(inDir, "*.*htm*", SearchOption.TopDirectoryOnly))
            {
                XmlDocument xmlDocument;
                using(var inHtmlFile = new StreamReader(fileName))
                {
                    var sgmlReader = new Sgml.SgmlReader
                    {
                        DocType = "HTML",
                        WhitespaceHandling = WhitespaceHandling.All,
                        CaseFolding = Sgml.CaseFolding.ToLower,
                        InputStream = inHtmlFile,
                    };
                    xmlDocument = new XmlDocument { PreserveWhitespace = true, XmlResolver = null };
                    xmlDocument.Load(sgmlReader);

                    var htmlElement = xmlDocument.GetElementsByTagName("html")[0];
                    var headElement = xmlDocument.CreateElement("head");
                    htmlElement.InnerXml = string.Format("<head><link href=\"stylesheet.css\" type=\"text/css\" rel=\"stylesheet\" /></head>{0}", htmlElement.InnerXml);

                    var outFileName = Path.GetFileNameWithoutExtension(fileName) + ".xhtml";

                    using (var outFile = new XmlTextWriter(Path.Combine(outDir,outFileName),Encoding.UTF8))
                    {
                        xmlDocument.Save(outFile);
                    }

                    string title = navPointCounter.ToString();

                    var navPoint = new ncxNavPoint()
                                          {
                                              content = new ncxNavPointContent(){src = outFileName},
                                              playOrder = navPointCounter.ToString(),
                                              id = string.Format("NavPoint-{0}",navPointCounter++),
                                              navLabel = new ncxNavPointNavLabel(){text = title},
                                          };
                    ncxNavPoints.Add(navPoint);
                    

                }
            }
            ncx.navMap = ncxNavPoints.ToArray();

            var xmlSerializer = new XmlSerializer(typeof(ncx));
            using(var ncxFile = new StreamWriter(Path.Combine(outDir,"toc.ncx")))
            {
                xmlSerializer.Serialize(ncxFile,ncx);
            }
 */
        }

        private static Stream OnImageLoading(string fileName)
        {
            return File.OpenRead(fileName);
        }
    }
}
