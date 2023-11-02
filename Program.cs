using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
//I couldn't get Newtonsoft.Json to work due to my versioning; I wasn't prepared for a 2 hour web app take home assignment
//Other wise this would have been a bit neater with a proepr UI.
namespace ErlyAssignment
{

    class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<Author> Authors { get; set; }
        public string Description { get; set; }
        public string CoverImageUrl { get; set; }
    }
    class Program
    {

        static List<Author> authors = new List<Author>();
        static List<Book> books = new List<Book>();
        static int nextAuthorId = 1;
        static int nextBookId = 1;


        //ran out of time to debug aspects related to saving to the json file.
        static void Main(string[] args)
        {
            LoadAuthorsFromFile();
            LoadBooksFromFile();

            Console.WriteLine("Hi Welcome to Karl's Erly Assignment Task (I hope it's a satisfactory experience).");
            while (true)
            {
                Console.WriteLine("Please Select an option from below using it's corresponding digit (1-4):");
                Console.WriteLine("1: Add a New Author.");
                Console.WriteLine("2: Add a New Book with a Author(s).");
                Console.WriteLine("3: Search for a Book by Title or Author.");
                Console.WriteLine("4: Exit.");
                Console.Write("Enter your choice: ");

                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            AddAuthor();
                            Console.WriteLine("Author Added. Press Enter to continue.");
                            Console.ReadLine();
                            break;
                        case 2:
                            AddBook();
                            Console.WriteLine("Book added. Press Enter to continue.");
                            Console.ReadLine();
                            break;
                        case 3:
                        //currently not fully working as intended but I ran out of time.
                        //needed to also fix the list all books function.
                            Console.WriteLine("Enter search query (author or book title) or press Enter to list all books: ");
                            string query = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(query))
                            {
                                SearchBooks(query);
                            }
                            else
                            {
                                ListAllBooks();
                            }
                            Console.ReadLine();
                            break;
                        case 4:
                            Console.WriteLine("Exiting the program. Press Enter to exit.");
                            SaveAuthorsToFile();
                            SaveBooksToFile();
                            Console.ReadLine();
                            return;
                        default:
                            Console.WriteLine("Invalid choice. Press Enter to try again.");
                            Console.ReadLine();
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Press Enter to try again.");
                    Console.ReadLine();
                }
            }

            /*
            Console.WriteLine("What is your name?");
            var name = Console.ReadLine();
            var currentDate = DateTime.Now;
            Console.WriteLine($"{Environment.NewLine}Hello, {name}, on {currentDate:d} at {currentDate:t}!");
            Console.Write($"{Environment.NewLine}Press any key to exit...");
            Console.ReadKey(true);
            */

            static void LoadAuthorsFromFile()
            {
                if (File.Exists("Authors.json"))
                {
                    string[] lines = File.ReadAllLines("Authors.json");
                    authors = new List<Author>();
                    bool inAuthorsArray = false;
                    foreach (string line in lines)
                    {
                        if (inAuthorsArray)
                        {
                            if (line.Contains("}"))
                            {
                                inAuthorsArray = false;
                            }
                            else
                            {
                                string[] parts = line.Trim().TrimEnd(',').Split(':');
                                if (parts.Length == 2)
                                {
                                    string key = parts[0].Trim().Trim('"');
                                    string value = parts[1].Trim().Trim('"', ',', ' ', '}');
                                    if (key == "id" && int.TryParse(value, out int id))
                                    {
                                        var author = new Author { Id = id };
                                        authors.Add(author);
                                    }
                                    else if (key == "name")
                                    {
                                        authors.Last().Name = value;
                                    }
                                }
                            }
                        }
                        if (line.Contains("\"authors\": ["))
                        {
                            inAuthorsArray = true;
                        }
                    }
                }
            }

            static void LoadBooksFromFile()
            {
                if (File.Exists("Books.json"))
                {
                    string[] lines = File.ReadAllLines("Books.json");
                    books = new List<Book>();
                    bool inBooksArray = false;
                    foreach (string line in lines)
                    {
                        if (inBooksArray)
                        {
                            if (line.Contains("}"))
                            {
                                inBooksArray = false;
                            }
                            else
                            {
                                string[] parts = line.Trim().TrimEnd(',').Split(':');
                                if (parts.Length == 2)
                                {
                                    string key = parts[0].Trim().Trim('"');
                                    string value = parts[1].Trim().Trim('"', ',', ' ', '}');
                                    if (key == "id" && int.TryParse(value, out int id))
                                    {
                                        var book = new Book { Id = id };
                                        books.Add(book);
                                    }
                                    else if (key == "title")
                                    {
                                        books.Last().Title = value;
                                    }
                                    else if (key == "description")
                                    {
                                        books.Last().Description = value;
                                    }
                                    else if (key == "coverImageUrl")
                                    {
                                        books.Last().CoverImageUrl = value;
                                    }
                                }
                            }
                        }
                        if (line.Contains("\"books\": ["))
                        {
                            inBooksArray = true;
                        }
                        else if (line.Contains("\"authors\": ["))
                        {
                            var authorIds = new List<int>();
                            inBooksArray = true;
                            foreach (string subLine in lines)
                            {
                                if (subLine.Contains("}"))
                                {
                                    inBooksArray = false;
                                }
                                else
                                {
                                    string[] subParts = subLine.Trim().TrimEnd(',').Split(':');
                                    if (subParts.Length == 2)
                                    {
                                        string subKey = subParts[0].Trim().Trim('"');
                                        string subValue = subParts[1].Trim().Trim('"', ',', ' ', '}');
                                        if (subKey == "id" && int.TryParse(subValue, out int authorId))
                                        {
                                            authorIds.Add(authorId);
                                        }
                                    }
                                }
                            }
                            books.Last().Authors = authors.Where(author => authorIds.Contains(author.Id)).ToList();
                        }
                    }
                }
            }

            static void AddAuthor()
            {
                Console.Write("Enter the author's name: ");
                string authorName = Console.ReadLine();

                var author = new Author
                {
                    Id = nextAuthorId++,
                    Name = authorName,
                };

                authors.Add(author);

                SaveAuthorsToFile();
            }

            static void SaveAuthorsToFile()
            {
                using (StreamWriter file = File.CreateText("Authors.json"))
                {
                    file.WriteLine("{");
                    file.WriteLine("  \"authors\": [");
                    foreach (var author in authors)
                    {
                        string authorJson = $"    {{\"id\": {author.Id}, \"name\": \"{author.Name}\"}}";
                        file.WriteLine(authorJson);
                    }
                    file.WriteLine("  ]");
                    file.WriteLine("}");
                }
            }

            static void AddBook()
            {
                if (authors.Count == 0)
                {
                    Console.WriteLine("No authors available. Please add authors first.");
                    return;
                }

                Console.Write("Enter the book title: ");
                string bookTitle = Console.ReadLine();

                Console.Write("Enter the book description: ");
                string bookDescription = Console.ReadLine();

                Console.Write("Enter the cover image URL: ");
                string coverImageUrl = Console.ReadLine();

                Console.WriteLine("Select author(s) for the book (comma-separated list of author IDs):");
                List<Author> selectedAuthors = SelectAuthors();

                var book = new Book
                {
                    Id = nextBookId++,
                    Title = bookTitle,
                    Authors = selectedAuthors,
                    Description = bookDescription,
                    CoverImageUrl = coverImageUrl
                };

                books.Add(book);
            }
            static void SaveBooksToFile()
            {
                using (StreamWriter file = File.CreateText("Books.json"))
                {
                    file.WriteLine("{");
                    file.WriteLine("  \"books\": [");
                    foreach (var book in books)
                    {
                        file.WriteLine("    {");
                        file.WriteLine($"      \"id\": {book.Id},");
                        file.WriteLine($"      \"title\": \"{book.Title}\",");
                        file.WriteLine("      \"authors\": [");
                        foreach (var author in book.Authors)
                        {
                            file.WriteLine($"        {{\"id\": {author.Id}, \"name\": \"{author.Name}\"}},");
                        }
                        file.WriteLine("      ],");
                        file.WriteLine($"      \"description\": \"{book.Description}\",");
                        file.WriteLine($"      \"coverImageUrl\": \"{book.CoverImageUrl}\"");
                        file.WriteLine("    },");
                    }
                    file.WriteLine("  ]");
                    file.WriteLine("}");
                }
            }


            static List<Author> SelectAuthors()
            {
                List<Author> selectedAuthors = new List<Author>();
                foreach (var author in authors)
                {
                    Console.WriteLine($"ID: {author.Id}, Name: {author.Name}");
                }
                Console.Write("Enter the author IDs separated by commas: ");
                string[] authorIds = Console.ReadLine().Split(',');
                foreach (string authorId in authorIds)
                {
                    if (int.TryParse(authorId, out int id))
                    {
                        Author selectedAuthor = authors.FirstOrDefault(author => author.Id == id);
                        if (selectedAuthor != null)
                        {
                            selectedAuthors.Add(selectedAuthor);
                        }
                    }
                }
                return selectedAuthors;
            }

            static void SearchBooks(string query)
            {
                var matchingBooks = books
                    .Where(book => book.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                                   book.Authors.Any(author => author.Name.Contains(query, StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                if (matchingBooks.Count == 0)
                {
                    Console.WriteLine("No matching books found.");
                }
                else
                {
                    Console.WriteLine("Matching Books:");
                    foreach (var book in matchingBooks)
                    {
                        Console.WriteLine($"ID: {book.Id}");
                        Console.WriteLine($"Title: {book.Title}");
                        Console.WriteLine($"Authors: {string.Join(", ", book.Authors.Select(author => author.Name))}");
                        Console.WriteLine();
                    }
                }
            }

            static void ListBooks()
            {
                Console.WriteLine("1. List all books");
                Console.WriteLine("2. Search books by author or title");
                Console.Write("Enter your choice: ");

                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            ListAllBooks();
                            break;
                        case 2:
                            Console.Write("Enter search query (author or book title): ");
                            string query = Console.ReadLine();
                            SearchBooks(query);
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Press Enter to try again.");
                            Console.ReadLine();
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Press Enter to try again.");
                    Console.ReadLine();
                }
            }
            static void ListAllBooks()
            {
                if (books.Count == 0)
                {
                    Console.WriteLine("No books available.");
                }
                else
                {
                    Console.WriteLine("All Books:");
                    foreach (var book in books)
                    {
                        Console.WriteLine($"ID: {book.Id}");
                        Console.WriteLine($"Title: {book.Title}");
                        Console.WriteLine($"Authors: {string.Join(", ", book.Authors.Select(author => author.Name))}");
                        Console.WriteLine();
                    }
                }
            }
        }
    }
}