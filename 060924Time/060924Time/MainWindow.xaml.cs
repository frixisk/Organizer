using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace _060924Time
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer countTimer, checkerAClock;
        private List<string> notes = new List<string>();
        private TimeSpan timeLeft;
        SoundPlayer play = new SoundPlayer();
        private List<DateTime> aClocks = new List<DateTime>();
        private bool isTimerRunning = false, isTimerInitialized = false;

        public MainWindow()
        {
            InitializeComponent();

            //Таймер
            countTimer = new DispatcherTimer();
            countTimer.Interval = TimeSpan.FromSeconds(1);
            countTimer.Tick += Timer_Tick;

            //Чекер будильника
            checkerAClock = new DispatcherTimer();
            checkerAClock.Interval = TimeSpan.FromSeconds(1);
            checkerAClock.Tick += AlarmChecker_Tick;
            checkerAClock.Start();
        }

        private void StartTimerButton_Click(object sender, RoutedEventArgs e)
        {
            //Проверка таймера
            if (!isTimerInitialized)
            {
                try
                {
                    int hours = int.Parse(this.hours.Text);
                    int minutes = int.Parse(this.minutes.Text);
                    int seconds = int.Parse(this.seconds.Text);

                    timeLeft = new TimeSpan(hours, minutes, seconds);
                    TimerDisplay.Text = timeLeft.ToString(@"hh\:mm\:ss");

                    isTimerInitialized = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка ввода данных: {ex.Message}");
                    return;
                }
            }

            if (!isTimerRunning)
            {
                countTimer.Start();
                isTimerRunning = true;
            }
        }

        private void StopTimerButton_Click(object sender, RoutedEventArgs e)
        {
            //Таймер на паузе
            countTimer.Stop();
            isTimerRunning = false; 
        }

        private void ResetTimerButton_Click(object sender, RoutedEventArgs e)
        {
            countTimer.Stop();
            timeLeft = TimeSpan.Zero;
            TimerDisplay.Text = "00:00:00";

            //Сброс флагов
            isTimerRunning = false;
            isTimerInitialized = false;

            StopAlarm();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (timeLeft.TotalSeconds > 0)
            {
                timeLeft = timeLeft.Subtract(TimeSpan.FromSeconds(1));
                TimerDisplay.Text = timeLeft.ToString(@"hh\:mm\:ss");
            }
            else
            {
                countTimer.Stop();
                TimerDisplay.Text = "00:00:00";
                PlayAlarm();
                MessageBox.Show("Таймер завершен!", "Выполнено");

                isTimerRunning = false;
                isTimerInitialized = false;
            }
        }

        private void AlarmChecker_Tick(object sender, EventArgs e)
        {
            CheckAlarms();
        }

        private void AddAlarmButton_Click(object sender, RoutedEventArgs e)
        {
            if (AlarmDatePicker.SelectedDate.HasValue && TimeSpan.TryParse(AlarmTimeInput.Text, out TimeSpan alarmTimeSpan))
            {
                DateTime alarmTime = AlarmDatePicker.SelectedDate.Value.Date + alarmTimeSpan;
                aClocks.Add(alarmTime);
                AlarmsList.Items.Add($"Будильник на {alarmTime:dd.MM.yyyy HH:mm}.");
            }
            else
            {
                MessageBox.Show("Выберите будильник для добавления.", "Ошибка");
            }
        }

        private void CheckAlarms()
        {
            for (int i = 0; i < aClocks.Count; i++)
            {
                if (DateTime.Now >= aClocks[i])
                {
                    PlayAlarm();
                    MessageBox.Show($"Будильник на {aClocks[i]}.");
                    aClocks.RemoveAt(i);
                    AlarmsList.Items.RemoveAt(i);
                    i--;
                }
            }
        }
        private void AddNoteButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(text.Text))
            {
                notes.Add(text.Text);

                text.Clear();

                UpdateNoteList();
            }
            else
            {
                MessageBox.Show("Введите текст заметки!", "Ошибка");
            }
        }
        
        private void DeleteNoteButton_Click(object sender, RoutedEventArgs e)
        {
            if (textList.SelectedIndex >= 0 && textList.SelectedIndex < notes.Count)
            {
                notes.RemoveAt(textList.SelectedIndex);

                UpdateNoteList();
            }
            else
            {
                MessageBox.Show("Выберите заметку для удаления!","Ошибка");
            }
        }
        private void UpdateNoteList()
        {
            textList.Items.Clear();

            for (int i = 0; i < notes.Count; i++)
            {
                textList.Items.Add($"{i + 1}. {notes[i]}");
            }
        }
        private void PlayAlarm()
        {
            play.SoundLocation = "alarm.wav";
            play.Play();
        }

        private void DeleteAlarmButton_Click(object sender, RoutedEventArgs e)
        {
            if (AlarmsList.SelectedItem != null)
            {
                int selectedIndex = AlarmsList.SelectedIndex;
                if (selectedIndex >= 0 && selectedIndex < aClocks.Count)
                {
                    aClocks.RemoveAt(selectedIndex);
                }

                AlarmsList.Items.RemoveAt(selectedIndex);
            }
            else
            {
                MessageBox.Show("Выберите будильник для удаления.", "Ошибка");
            }
        }
        private void StopAlarm()
        {
            play.Stop();
        }
    }
}
