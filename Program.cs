/* C#, lesson_44  20.05.2023																																						
 * 																																						
Task № 1:																																						
Створіть програму для роботи з інформацією про музичний альбом, яка зберігатиме таку інформацію: 																																						
1. Назва альбому. 																																						
2. Назва виконавця. 																																						
3. Рік випуску. 																																						
4. Тривалість. 																																						
5. Студія звукозапису. 																																						
Програма має бути з такою функціональністю: 																																						
1. Введення інформації про альбом. 																																						
2. Виведення інформації про альбом. 																																						
3. Серіалізація альбому. 																																						
4. Збереження серіалізованого альбому у файл. 																																						
5. Завантаження серіалізованого альбому з файлу. 																																						
6. Збережіть дані про альбом у xml файлу																																						
7. Додайте логування, використовуйте NLog																																						
																																						
*/
//--------------------------------------------------------------------------------------------------------																																						
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using NLog;

namespace Lesson_44
{
//--------------------------------------------------------------------------------------------------------																																						
    [Serializable]
    class Album
    {
        public string TITLE { get; set; }
        public string SINGER { get; set; }
        public int YEAR { get; set; }
        public int LENGTH { get; set; }
        public string STUDIO { get; set; }

        public override string ToString()
        {
            return $"\n===============================================\n" +
                $"Album name: {TITLE}\nSinger: {SINGER}\nAlbum release year: {YEAR}\n" +
                $"Album length (min): {LENGTH}\nAlbum studio: {STUDIO}\n" +
                $"=============================================== ";
        }
    }
//--------------------------------------------------------------------------------------------------------																																						
    class Program
    {
        enum LOGS { ERROR = 1, WARN, INFO }

        public static void EnterAlbum(List<Album> albums)       // метод - введення інформації про альбоми
        {
            int size;
            do
            {
                Console.Write("\nEnter the count of album(s) ( > 0 ): ");
                size = Convert.ToInt32(Console.ReadLine());
                if (size < 1)
                    Console.WriteLine("Incorrect! Try again\n");
            } while (size < 1);

            for (int index = 0; index < size; index++)
            {
                Album temp = new Album();
                Console.Write("\nEnter title of the album # {0}: ", (albums.Count + 1));
                temp.TITLE = Console.ReadLine();
                Console.Write("Enter the singer: ");
                temp.SINGER = Console.ReadLine();
                Console.Write("Enter album release year: ");
                temp.YEAR = Convert.ToInt32(Console.ReadLine());
                Console.Write("Enter the album length (min): ");
                temp.LENGTH = Convert.ToInt32(Console.ReadLine());
                Console.Write("Enter album release studio: ");
                temp.STUDIO = Console.ReadLine();

                albums.Add(temp);
                SaveLogFile((int)LOGS.INFO, $"The album # {albums.Count} was written");
            }
        }

        public static void ReceiveAlbum(List<Album> albums)             // метод - виведення інформації про альбом в консоль
        {
            if(albums.Count == 0)
            {
                SaveLogFile((int)LOGS.ERROR, "Album list does not exist, printing error.");
                Console.WriteLine("Album list does not exist, printing error.");
            }

            else
            {
                foreach (var item in albums)
                {
                    Console.WriteLine(item.ToString());
                }
            }
        }

        public static void SerializeAlbum(string path, List<Album> albums)  // метод - серіалізація об'єкту і збереження його в бінарний файл
        {
            if (albums.Count == 0)
            {
                SaveLogFile((int)LOGS.ERROR, "Album list doesn't exist, serialization error.");
                Console.WriteLine("Album list does not exist, serialization error.");
            }
            else
            {
                using (FileStream fileAlbums = new FileStream(path, FileMode.Create))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fileAlbums, albums);
                }
                Console.WriteLine("An album(s) was saved in a binary file.");
                SaveLogFile((int)LOGS.WARN, $"The list of album(s) was saved in binary file.");
            }
        }

        public static void DownloadAlbumFromFile(string path)  // метод - завантаження серіалізованого об'єкту з бінарного файлу і виведення інфо в консоль
        {
            if (!File.Exists(path))
            {
                SaveLogFile((int)LOGS.ERROR, "The binary file with the album list doesn't exist, downloading error.");
                Console.WriteLine("The binary file with the album list doesn't exist, downloading error");
            }
            else
            {
                List<Album> albums = new List<Album>();
                using (FileStream fileAlbum = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    albums = (List<Album>)formatter.Deserialize(fileAlbum);
                }
                ReceiveAlbum(albums);
                SaveLogFile((int)LOGS.INFO, $"The list of album(s) was download.");
            }
        }

        public static void SaveAlbumXmlFile(string pathXml, List<Album> albums)     // метод - збереження об'єкту в xml-файл
        {
            if (albums.Count == 0)
            {
                SaveLogFile((int)LOGS.ERROR, "Album list does not exist, saving xml-file error.");
                Console.WriteLine("Album list does not exist, saving xml-file error.");
            }

            else
            {
                XmlDocument xmlDoc = new XmlDocument();

                XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                xmlDoc.AppendChild(xmlDeclaration);

                XmlElement rootElement = xmlDoc.CreateElement("albums");
                xmlDoc.AppendChild(rootElement);

                for (int index = 0; index < albums.Count; index++)
                {
                    XmlElement albumElement = xmlDoc.CreateElement("album");
                    XmlAttribute idAttr = xmlDoc.CreateAttribute("id");
                    idAttr.Value = (index + 1).ToString();
                    albumElement.SetAttributeNode(idAttr);

                    XmlElement titleElement = xmlDoc.CreateElement("title");
                    titleElement.InnerText = albums[index].TITLE;
                    albumElement.AppendChild(titleElement);

                    XmlElement singerElement = xmlDoc.CreateElement("singer");
                    singerElement.InnerText = albums[index].SINGER;
                    albumElement.AppendChild(singerElement);

                    XmlElement yearElement = xmlDoc.CreateElement("year");
                    yearElement.InnerText = (albums[index].YEAR).ToString();
                    albumElement.AppendChild(yearElement);

                    XmlElement lengthElement = xmlDoc.CreateElement("length");
                    lengthElement.InnerText = (albums[index].LENGTH).ToString();
                    albumElement.AppendChild(lengthElement);

                    XmlElement studioElement = xmlDoc.CreateElement("studio");
                    studioElement.InnerText = albums[index].STUDIO;
                    albumElement.AppendChild(studioElement);

                    rootElement.AppendChild(albumElement);
                }
                xmlDoc.Save(pathXml);
                SaveLogFile((int)LOGS.WARN, $"The list of album(s) was saved in xml file, not binary.");
                Console.WriteLine("The list of album(s) was saved in xml file, not binary.");
            }
        }

        public static void SaveLogFile(int codeLog, string message)         // метод - логування інформації про діяльність користувача
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            switch (codeLog)
            {
                case (int)LOGS.ERROR:
                    logger.Error(message);
                    break;
                case (int)LOGS.WARN:
                    logger.Warn(message);
                    break;
                case (int)LOGS.INFO:
                    logger.Info(message);
                    break;
            }
        }
//-------------------------------------------------------------------------------------------------------------------------        																																						
        static void Main(string[] args)
        {
            Console.WriteLine("Program \"C# lesson_44 \"Task 1, Music album(s)\"\n");
            int index;
            string path = "albums.bin";
            string pathXml = "albums.xml";
            List<Album> albums = new List<Album>();

            do
            {
                do
                {
                    Console.WriteLine("\n\n  1 - Enter information about the album\n" +
                                    "  2 - Output information about the album\n" +
                                    "  3 - Saving a serialized album to a file\n" +
                                    "  4 - Downloading a serialized album from a file\n" +
                                    "  5 - Save the album to an xml file\n" +
                                    "  6 - Exit\n");
                    Console.Write("Make your choice (1 - 6): ");
                    index = Convert.ToInt32(Console.ReadLine());
                    if (index < 1 || index > 6)
                        Console.WriteLine("Incorrect! Try again.\n\n");

                } while (index < 1 || index > 6);

                switch (index)
                {
                    case 1:
                        EnterAlbum(albums);                 // заповнення списку альбомів
                        break;
                    case 2:
                        ReceiveAlbum(albums);               // виведення інформації про альбоми в консоль
                        break;
                    case 3:
                        SerializeAlbum(path, albums);       // серіалізація списку альбомів і збереження у файл
                        break;
                    case 4:
                        DownloadAlbumFromFile(path);        // завантаження з файлу списку альбомів і виведення в консоль
                        break;
                    case 5:
                        SaveAlbumXmlFile(pathXml, albums);  // збереження списку альбомів в xml-файл
                        break;
                }
            } while (index != 6);

            Console.WriteLine("\n\nThe music album(s) has been listened to ...\n\n");
            SaveLogFile((int)LOGS.INFO, $"The music album(s) has been deleted from the disc.");
        }
    }
}
//-------------------------------------------------------------------------------------------------------------------------