using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;//Пространство имен для работы с файловой системой и папками
using xNet;
using System.Text.RegularExpressions;
using System.Threading;

namespace Instagram
{
    class Filter
    {
        //Путь до файла
        private string file_for_names;
        private string file_for_id;

        private Label FilterLab;
        private Button Start;
        private Button Stop;

        private const int GET_TIME = 100;
        private const int WAIT_TIME = 30000;

        //Конструктор обьявления
        public Filter(string FILE_NAME,string FILE_ID,Label FILTERLAB,Button START, Button STOP)
        {
            file_for_names = FILE_NAME;
            file_for_id = FILE_ID;
            FilterLab = FILTERLAB;
            Start = START;
            Stop = STOP;
        }

        //Сохраняем список имен из файлов в лист для удобства доступа
        private List<string> NickInput(string path)
        {
            List<string> names = new List<string>();
            using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    names.Add(line);
                }
                sr.Close();
            }
            return names;
        }

        //Сохранение списка результатов в файл
        private void NickOutput(List<string> listforsave,string path)
        {
            using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default))
            {
                for (int i=0;i<listforsave.Count;i++)
                {
                    sw.WriteLine(listforsave[i]);
                }
                sw.Close();
            }
        }

        //Проверка для Checkers есть ли хотя бы 1 фильтр
        public bool Empty_Checkers(bool [] Checkers)
        {
            for (int i=0;i<Checkers.Length;i++)
            {
                if (Checkers[i] == true) return true;
            }
            MessageBox.Show(
                        "Укажите хотя бы 1 фильтр.",
                        "Ошибка настройки фильтров.",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
            return false;
        }

        //Проверка для Digits что все циферные поля правильны
        private bool Wrong_Digits(string [] digits)
        {
            bool right = true;
            int [] result = new int [digits.Length];
            for (int i=0;i < digits.Length;i++)
            {
                right = false;
                if (Int32.TryParse(digits[i], out result[i]))
                    if (result[i] >= 0)
                        right=true;
                if (!right)
                {
                    MessageBox.Show(
                        "В числовом поле буквы, отрицательное число, пустота.",
                        "Ошибка настройки фильтров.",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                    return false;
                }
            }
            for (int i = 0; i < result.Length; i = i + 2)
            {
                if (result[i] > result[i + 1])
                {
                    MessageBox.Show(
                        "Число \"От\" должно быть меньше числа \"До" +
                        "\".",
                        "Ошибка настройки фильтров.",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                    return false;
                }
            }
            return true;
        }

        //Проверка для Слов что ничего лишнего не занесено, сделал но нужно подумать как перенести это сюда
        private bool Wrong_Words(string[] words)
        {
            return true;
        }

        //Методы фильтрации
        public void filtering(bool [] Checkers, string [] digits,string [] words)
        {
            //Проверка входных данных
            if (!Empty_Checkers(Checkers)) return;
            if (!Wrong_Digits(digits)) return;
            if (!Wrong_Words(words)) return;

            Start.Invoke(new Action<bool>((button) => Start.Enabled = button), false);
            Stop.Invoke(new Action<bool>((button) => Stop.Enabled = button), true);

            List<string> names =  NickInput(file_for_names);//Список на входе
            List<string> id = NickInput(file_for_id);

            string html = "";

            HttpRequest http = new HttpRequest();
            string address = "https://www.instagram.com/";

            List<string> final_names = new List<string>();//Список для сохранения
            List<string> final_ids = new List<string>();

            bool Is_Satisfy;
            bool f;//Для запросов
            int SkolkoPerebral = 0;
            int SkolkoSohranil = 0;
            Request request = new Request();

            //тут начинаем фильтровать
            for (int i=0;i<names.Count;i++)
            {
                if (MainForm.Stopper == true)
                {
                    MainForm.Stopper = false;

                    FilterLab.Invoke(new Action<string>((text) => FilterLab.Text = text), "Фильтрация остановлена...");
                    Start.Invoke(new Action<bool>((button) => Start.Enabled = button), true);
                    Stop.Invoke(new Action<bool>((button) => Stop.Enabled = button), false);

                    return;
                }

                //Проверку в true и запрос на 1 человека
                Is_Satisfy = true;

                html = request.Get_With_Ex(address + names[i], http);
                if (html == "") continue;

                SkolkoPerebral++;
                FilterLab.Invoke(new Action<string>((text) => FilterLab.Text = text), "Отфильтрованно аккаунтов : " + SkolkoPerebral + " из : " + names.Count + ", сохранено : " + SkolkoSohranil);
                
                //Проверка
                Is_Satisfy = Biography();
                if (!Is_Satisfy) continue;
                Is_Satisfy = External_Url(Checkers[0], words[0], html);
                if (!Is_Satisfy) continue;
                Is_Satisfy = Edge_Followed_By(Checkers[1], digits[0],digits[1], html);
                if (!Is_Satisfy) continue;
                Is_Satisfy = Edge_Follow(Checkers[2], digits[2], digits[3], html);
                if (!Is_Satisfy) continue;
                Is_Satisfy = Full_Name();
                if (!Is_Satisfy) continue;
                Is_Satisfy = Has_Channel(Checkers[3], words[1], html);
                if (!Is_Satisfy) continue;
                Is_Satisfy = Highlight_Reel_Count(Checkers[4], digits[4], digits[5], html);
                if (!Is_Satisfy) continue;
                Is_Satisfy = Is_Business_Account(Checkers[5], words[2], html);
                if (!Is_Satisfy) continue;
                Is_Satisfy = Is_Joined_Recently(Checkers[6], words[3], html);
                if (!Is_Satisfy) continue;
                Is_Satisfy = Business_Category_Name();
                if (!Is_Satisfy) continue;
                Is_Satisfy = Connected_Fb_Page(Checkers[7], words[4], html);
                if (!Is_Satisfy) continue;
                Is_Satisfy = Felix_Video_Count(Checkers[8], digits[6], digits[7], html);
                if (!Is_Satisfy) continue;
                Is_Satisfy = Media_Count(Checkers[9], digits[8], digits[9], html);
                if (!Is_Satisfy) continue;
                Is_Satisfy = Saved_Media_Count(Checkers[10], digits[10], digits[11], html);
                if (!Is_Satisfy) continue;

                //Если дошел до конца то записываем
                final_names.Add(names[i]);
                final_ids.Add(id[i]);

                SkolkoSohranil++;
            }
            //Сохранение результатов в тот же файл
            NickOutput(final_names, file_for_names);
            NickOutput(final_ids, file_for_id);

            Start.Invoke(new Action<bool>((button) => Start.Enabled = button), true);
            Stop.Invoke(new Action<bool>((button) => Stop.Enabled = button), false);
        }

        //Биография
        private bool Biography()
        {
            return true;
        }
        //ссылка в профиле
        private bool External_Url(bool BoxIsCheck,string link,string HTML)
        {
            if (BoxIsCheck)
            {
                Regex exp = new Regex("\"external_url\":null");
                MatchCollection findexp = exp.Matches(HTML);
                if (findexp.Count>0)
                {
                    if (link=="да")
                        return false;
                }
                else if (findexp.Count < 0)
                {
                    if (link == "нет")
                        return false;
                }
            }
            return true;
        }
        //Количество подписчиков
        private bool Edge_Followed_By(bool BoxIsCheck, string Min_Value,string Max_Value, string HTML)
        {
            if (BoxIsCheck)
            {
                Regex exp = new Regex("\"edge_followed_by\":{\"count\":" + @"(\w*)");
                MatchCollection findexp = exp.Matches(HTML);

                if (findexp.Count > 0)
                {
                    int min = Convert.ToInt32(Min_Value);
                    int max = Convert.ToInt32(Max_Value);

                    string[] separator = new string[1] { "\"edge_followed_by\":{\"count\":" };
                    string[] helpsep = findexp[0].Value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    int followers = Convert.ToInt32(helpsep[0]);

                    if ((followers < min) || (followers > max))
                        return false;
                }
            }
            return true;
        }
        //Количество подписок
        private bool Edge_Follow(bool BoxIsCheck, string Min_Value, string Max_Value, string HTML)
        {
            if (BoxIsCheck)
            {
                Regex exp = new Regex("\"edge_follow\":{\"count\":" + @"(\w*)");
                MatchCollection findexp = exp.Matches(HTML);
                if (findexp.Count > 0)
                {
                    int min = Convert.ToInt32(Min_Value);
                    int max = Convert.ToInt32(Max_Value);

                    string[] separator = new string[1] { "\"edge_follow\":{\"count\":" };
                    string[] helpsep = findexp[0].Value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    int followings = Convert.ToInt32(helpsep[0]);

                    if ((followings < min) || (followings > max))
                        return false;
                }
            }
            return true;
        }
        //полное имя
        private bool Full_Name()
        {
            return true;
        }
        //есть ли канал
        private bool Has_Channel(bool BoxIsCheck, string chanel, string HTML)
        {
            if (BoxIsCheck)
            {
                Regex exp = new Regex("\"has_channel\":false");
                MatchCollection findexp = exp.Matches(HTML);
                if (findexp.Count > 0)
                {
                    if (chanel == "да")
                        return false;
                }
                else if (findexp.Count < 0)
                {
                    if (chanel == "нет")
                        return false;
                }
            }
            return true;
        }
        //Количество сториков сохраненных
        private bool Highlight_Reel_Count(bool BoxIsCheck, string Min_Value, string Max_Value, string HTML)
        {
            if (BoxIsCheck)
            {
                Regex exp = new Regex("\"highlight_reel_count\":" + @"(\w*)");
                MatchCollection findexp = exp.Matches(HTML);
                if (findexp.Count > 0)
                {
                    int min = Convert.ToInt32(Min_Value);
                    int max = Convert.ToInt32(Max_Value);

                    string[] separator = new string[1] { "\"highlight_reel_count\":" };
                    string[] helpsep = findexp[0].Value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    int stories = Convert.ToInt32(helpsep[0]);

                    if ((stories < min) || (stories > max))
                        return false;
                }
            }
            return true;
        }
        //Является ли бизнесс аккаунтом
        private bool Is_Business_Account(bool BoxIsCheck, string buisness, string HTML)
        {
            if (BoxIsCheck)
            {
                Regex exp = new Regex("\"is_business_account\":false");
                MatchCollection findexp = exp.Matches(HTML);
                if (findexp.Count > 0)
                {
                    if (buisness == "да")
                        return false;
                }
                else if (findexp.Count < 0)
                {
                    if (buisness == "нет")
                        return false;
                }
            }
            return true;
        }
        //Появился ли недавно
        private bool Is_Joined_Recently(bool BoxIsCheck, string recently, string HTML)
        {
            if (BoxIsCheck)
            {
                Regex exp = new Regex("\"is_joined_recently\":false");
                MatchCollection findexp = exp.Matches(HTML);
                if (findexp.Count > 0)
                {
                    if (recently == "да")
                        return false;
                }
                else if (findexp.Count < 0)
                {
                    if (recently == "нет")
                        return false;
                }
            }
            return true;
        }
        //Категория бизнес аккаунта
        private bool Business_Category_Name()
        {
            return true;
        }
        //Подключена ли страница в фб
        private bool Connected_Fb_Page(bool BoxIsCheck, string connected, string HTML)
        {
            if (BoxIsCheck)
            {
                Regex exp = new Regex("\"connected_fb_page\":null");
                MatchCollection findexp = exp.Matches(HTML);
                if (findexp.Count > 0)
                {
                    if (connected == "да")
                        return false;
                }
                else if (findexp.Count < 0)
                {
                    if (connected == "нет")
                        return false;
                }
            }
            return true;
        }
        //Количество игтв публикаций
        private bool Felix_Video_Count(bool BoxIsCheck, string Min_Value, string Max_Value, string HTML)
        {
            if (BoxIsCheck)
            {
                Regex exp = new Regex("\"edge_felix_video_timeline\":{\"count\":" + @"(\w*)");
                MatchCollection findexp = exp.Matches(HTML);
                if (findexp.Count > 0)
                {
                    int min = Convert.ToInt32(Min_Value);
                    int max = Convert.ToInt32(Max_Value);

                    string[] separator = new string[1] { "\"edge_felix_video_timeline\":{\"count\":" };
                    string[] helpsep = findexp[0].Value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    int felix = Convert.ToInt32(helpsep[0]);

                    if ((felix < min) || (felix > max))
                        return false;
                }
            }
            return true;
        }
        //Фильтры по игтв публикациям
        //Количество публикаций
        private bool Media_Count(bool BoxIsCheck, string Min_Value, string Max_Value, string HTML)
        {
            if (BoxIsCheck)
            {
                Regex exp = new Regex("\"edge_owner_to_timeline_media\":{\"count\":" + @"(\w*)");
                MatchCollection findexp = exp.Matches(HTML);
                if (findexp.Count > 0)
                {
                    int min = Convert.ToInt32(Min_Value);
                    int max = Convert.ToInt32(Max_Value);

                    string[] separator = new string[1] { "\"edge_owner_to_timeline_media\":{\"count\":" };
                    string[] helpsep = findexp[0].Value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    int media = Convert.ToInt32(helpsep[0]);

                    if ((media < min) || (media > max))
                        return false;
                }
            }
            return true;
        }
        //Фильтры по публикациям
        //Количество сохраненных фоток
        private bool Saved_Media_Count(bool BoxIsCheck, string Min_Value, string Max_Value, string HTML)
        {
            if (BoxIsCheck)
            {
                Regex exp = new Regex("\"edge_saved_media\":{\"count\":" + @"(\w*)");
                MatchCollection findexp = exp.Matches(HTML);
                if (findexp.Count > 0)
                {
                    int min = Convert.ToInt32(Min_Value);
                    int max = Convert.ToInt32(Max_Value);

                    string[] separator = new string[1] { "\"edge_saved_media\":{\"count\":" };
                    string[] helpsep = findexp[0].Value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    int saved = Convert.ToInt32(helpsep[0]);

                    if ((saved < min) || (saved > max))
                        return false;
                }
            }
            return true;
        }
        //Фильтры по схраненным публикациям
    }
}
