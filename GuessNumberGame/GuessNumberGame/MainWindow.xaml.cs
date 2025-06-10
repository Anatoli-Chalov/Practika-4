using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Xml.Serialization;

namespace GuessNumberGame
{
    public partial class MainWindow : Window
    {
        private int secretNumber;
        private int attempts;
        private const string ResultsFileName = "results.txt";

        public MainWindow()
        {
            InitializeComponent();
            StartNewGame();
            LoadResults();
        }

        private void StartNewGame()
        {
            Random random = new Random();
            secretNumber = random.Next(1, 101);
            attempts = 0;
            HintText.Text = "";
            AttemptsText.Text = "";
            GuessTextBox.Text = "";
            ResultPanel.Visibility = Visibility.Collapsed;
            GuessTextBox.IsEnabled = true;
        }

        private void CheckGuess_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(GuessTextBox.Text, out int guess))
            {
                attempts++;
                AttemptsText.Text = $"Попыток: {attempts}";

                if (guess == secretNumber)
                {
                    HintText.Text = "Поздравляем! Вы угадали число!";
                    ResultPanel.Visibility = Visibility.Visible;
                    GuessTextBox.IsEnabled = false;
                }
                else if (guess < secretNumber)
                {
                    HintText.Text = "Загаданное число больше!";
                }
                else
                {
                    HintText.Text = "Загаданное число меньше!";
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите корректное число!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            GuessTextBox.Text = "";
            GuessTextBox.Focus();
        }

        private void SaveResult_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PlayerNameTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, введите ваше имя!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = new GameResult
            {
                PlayerName = PlayerNameTextBox.Text,
                Attempts = attempts,
                Date = DateTime.Now
            };

            SaveResultToFile(result);
            LoadResults();
            StartNewGame();
        }

        private void SaveResultToFile(GameResult result)
        {
            List<GameResult> results = new List<GameResult>();

            // Чтение существующих результатов
            if (File.Exists(ResultsFileName))
            {
                using (StreamReader reader = new StreamReader(ResultsFileName))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<GameResult>));
                    results = (List<GameResult>)serializer.Deserialize(reader);
                }
            }

            // Добавление нового результата
            results.Add(result);

            // Сохранение всех результатов
            using (StreamWriter writer = new StreamWriter(ResultsFileName))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<GameResult>));
                serializer.Serialize(writer, results);
            }
        }

        private void LoadResults()
        {
            if (File.Exists(ResultsFileName))
            {
                try
                {
                    using (StreamReader reader = new StreamReader(ResultsFileName))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(List<GameResult>));
                        var results = (List<GameResult>)serializer.Deserialize(reader);
                        ResultsListView.ItemsSource = results;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке результатов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void StartNewGame_Click(object sender, RoutedEventArgs e)
        {
            StartNewGame();
        }
    }

    public class GameResult
    {
        public string PlayerName { get; set; }
        public int Attempts { get; set; }
        public DateTime Date { get; set; }
    }
}