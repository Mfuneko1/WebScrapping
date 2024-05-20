using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using HtmlAgilityPack;

namespace WebScrapping
{
    class Book
    {
        public string Title { get; set; }

        public string Price { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var bookLinks = GetBookLinks("https://books.toscrape.com/catalogue/category/books/mystery_3/index.html");
            Console.WriteLine(format: "Found {0} links", bookLinks.Count);

            var books = GetBookDetails(bookLinks);
            exportToCsv(books);
        }

        static void exportToCsv(List<Book> books)
        {
            using (var writer = new StreamWriter("./books.csv"))
            using(var csv = new CsvWriter(writer, CultureInfo.InvariantCulture)){
                csv.WriteRecords(books);
            }
        }

        static List<Book> GetBookDetails(List<string> urls)
        {
            var books = new List<Book>();

            foreach (var item in urls)
            {
                HtmlDocument document = GetDocument(item);
                var titleXPath = "//h1";
                var pricePath = "//div[contains(@class,\"product_main\")]/p[@class=\"price_color\"]";
                var book = new Book();

                book.Title = document.DocumentNode.SelectSingleNode(titleXPath).InnerText;
                book.Price = document.DocumentNode.SelectSingleNode(pricePath).InnerText;

                books.Add(book);
            }

            return books;
        }

        static HtmlDocument GetDocument(string url)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            return doc;
        }

        static List<string> GetBookLinks(string url)
        {
            var booklist = new List<string>();

            HtmlDocument doc = GetDocument(url);
            HtmlNodeCollection linkNodes = doc.DocumentNode.SelectNodes(xpath: "//h3/a");
            var baseUri = new Uri(url);

            foreach (var item in linkNodes)
            {
                string href = item.Attributes[name: "href"].Value;
                booklist.Add(item: new Uri(baseUri, href).AbsoluteUri);

            }

            return booklist;
        }
    }
}
