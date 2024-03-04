using System;
using System.Collections.Generic;
using System.Windows.Markup;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;


namespace EgorovProject
{

    public partial class MainWindow : Window
    {

        string dictionaryFilePath = "dictionary.txt";


        private List<Term> dictionary;
        

        public MainWindow()
        {
            {
                InitializeComponent();

                // Получение пути к файлу словаря внутри проекта
                dictionaryFilePath = "dictionary.txt";

                // Загрузка словаря из файла
                dictionary = LoadDictionary();

                // Заполнение ListBox содержимым словаря
                dictionaryListBox.ItemsSource = dictionary;

                // Привязка события к TextBox для обновления списка при вводе
                Search.TextChanged += SearchTextBox_TextChanged;
            }

        }


        public class Term
        {
            public string Word { get; set; }
            public string Description { get; set; }

            public Term(string word, string description)
            {
                Word = word;
                Description = description;
            }
        }

        private Term TermFromString(string str)
        {
            string[] parts = str.Split(';');
            if (parts.Length >= 2)
            {
                string word = parts[0].Trim();
                string description = string.Join(";", parts.Skip(1)).Trim();
                return new Term(word, description);
            }

            // Возможно, вернуть значение по умолчанию или сгенерировать исключение в зависимости от вашей логики
            return new Term("", "");
        }

        private List<Term> LoadDictionary()
        {
            List<Term> loadedDictionary = new List<Term>();

            try
            {
                // Просто читаем строки из файла
                if (File.Exists(dictionaryFilePath))
                {
                    string[] lines = File.ReadAllLines(dictionaryFilePath);

                    // Каждая строка должна содержать слово и описание, разделенные символом ";"
                    foreach (string line in lines)
                    {
                        loadedDictionary.Add(TermFromString(line));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке словаря: {ex.Message}", "Ошибка");
            }

            return loadedDictionary;
        }

        private void SaveDictionary()
        {
            try
            {
                // Просто записываем текст в файл
                File.WriteAllLines(dictionaryFilePath, dictionary.Select(term => $"{term.Word}; {term.Description}"));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении словаря: {ex.Message}", "Ошибка");
            }
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = Search.Text.Trim();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                List<Term> searchResults = dictionary
                    .Where(term => term.Word.ToLower().Contains(searchTerm.ToLower()))
                    .ToList();

                // Обновление ListBox с результатами поиска
                dictionaryListBox.ItemsSource = searchResults;
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Выполнить поиск при изменении текста в TextBox
            SearchBtn_Click(sender, e);
        }

        private void addWordBtn_Click(object sender, RoutedEventArgs e)
        {
            // Получение данных из TextBox
            string newWord = addWord.Text.Trim();
            string newDescription = descriptionWord.Text.Trim();

            if (!string.IsNullOrEmpty(newWord) && !string.IsNullOrEmpty(newDescription))
            {
                // Создание нового термина и добавление в список
                Term newTerm = new Term(newWord, newDescription);
                dictionary.Add(newTerm);

                // Очистка TextBox после добавления термина
                addWord.Clear();
                descriptionWord.Clear();

                // Обновление ListBox
                dictionaryListBox.ItemsSource = null;
                dictionaryListBox.ItemsSource = dictionary;

                // Сохранение изменений в файл
                SaveDictionary();
            }
            else
            {
                MessageBox.Show("Введите слово и его описание перед добавлением.", "Предупреждение");
            }
        }
    }
}

