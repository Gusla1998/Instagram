using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using xNet;
using System.Text.RegularExpressions;//Пространство имен для работы с регулярными выражениями
using System.IO;//Пространство имен для работы с файловой системой и папками
using System.Diagnostics;//Пространство имен для работы с процессами
using System.Threading;//Работа с многопоточностью
using System.Net;

namespace Instagram
{
    public partial class MainForm : Form
    {
        public BackgroundWorker bw;
        public MainForm()
        {
            InitializeComponent();            
        }
        
        

       ///Задаваемые поля      
        public static string url = "https://www.instagram.com/";//Забиваем адрес домена нужного        
        
        public static List<string> names = new List<string>();// список с кем работаем
        public static List<string> likername = new List<string>();//Вспомогательный список имен для лайкеров и комментаторов
        public static List<string> likerid = new List<string>();//Вспомогательный список айди для лайкеров и комментаторов
        public static List<string> comername = new List<string>();//Вспомогательный список имен для лайкеров и комментаторов
        public static List<string> comerid = new List<string>();//Вспомогательный список айди для лайкеров и комментаторов

        public static List<string> idforfollow = new List<string>();// список id for follow
        public static List<string> idforunfollow = new List<string>();// список id for unfollow
        public static List<string> photos = new List<string>();//Запоминаем photos
        public static List<string> words = new List<string>();//Запоминаем стоп слова для парсинга
        public static string yourid = "";

        public static bool Stopper = false;
        public static bool Continuer = true;

        public int schetchik = 0;
        public int target = 0;
        public int vsego = 0;

        public int Counter = 0;
        public int checker = 0;
        public int Counterph = 0;
        public int checkerph = 0;

        private void Form1_Load(object sender, EventArgs e)
        {         
            Is_Site_Value.SelectedItem = "нет";
            Is_Igtv_Value.SelectedItem = "нет";
            Is_Buisness_Value.SelectedItem = "нет";
            Is_Recently_Value.SelectedItem = "нет";
            Is_Facebook_Value.SelectedItem = "нет";
        }

        //Вывод счетчика из файла
        public static int Count_Read()
        {            
            string path = "Counter.txt";
            int x = 0;
            using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
            {
                x = Convert.ToInt32(sr.ReadLine());
                sr.Close();
            }
            return x;
        }

        //Ввод счетчика в файл
        public void Count_Write(int x)
        {
            string path = "Counter.txt";
            using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default))
            {
                sw.WriteLine(x);
                sw.Close();
            }
        }        

        //Считываем id фоток для лайков и комментов из файла и заносим в лист ids
        public void Media(string pathUsersId,List<string> list)
        {
            string path = pathUsersId;
            list.Clear();
            using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    list.Add(line);
                }
                sr.Close();
            }
        }

        //Записываем кукисы в текстовый файл для хранения
        public void Cookies_Text(string html)
        {            
                string path = "Cookies.txt";
                using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default))
                {
                    sw.WriteLine(html + ";  ");
                    sw.Close();                
                }
            
        }

        //Извлекаем кукисы и создаем готовый словарь из них
        public CookieDictionary Cookies()
        {
            string path = "Cookies.txt";
            List<string> string_cookies = new List<string>();

            //Считываем куки в лист из файла
            using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
            {
                string line;
                string helpline;
                while ((line = sr.ReadLine()) != null)
                {
                    helpline = "";
                    for (int i = 0; i < line.Length - 1; i++)
                    {
                        if ((line[i] == '='))
                        {
                            string_cookies.Add(helpline);
                            helpline = "";
                            i++;
                        }
                        else if (((line[i] == ';') & (line[i + 1] == ' ')))
                        {
                            if ((string_cookies[string_cookies.Count - 1] == "ds_user_id")) yourid = helpline;
                            string_cookies.Add(helpline);
                            helpline = "";
                            i = i + 2;
                        }
                        helpline += line[i];
                    }

                }
                sr.Close();
            }

            //Записываем в кукисловарь
            CookieDictionary cookies = new CookieDictionary();
            for (int i = 0; i < string_cookies.Count - 1; i = i + 2)
            {
                if (string_cookies[i] == "csrftoken") continue;
                cookies.Add(string_cookies[i], string_cookies[i + 1]);
            }
            cookies.IsLocked = true;
            return cookies;
        }

        ///Autorization
        private void button1_Click(object sender, EventArgs e)
        {
            string login = txtLogin.Text;//Сохраняем логин из текстбокса для логина
            string Pass = txtPass.Text;//Сохраняем пароль из текстбокса для пароля
            string html;
            string token;

            HttpRequest http = new HttpRequest();
            http.Cookies = new CookieDictionary();

            html = http.Get(url).ToString();// Отправляем запрос и принимаем тело сообщения в виде строки.
            token = Regex.Match(html, "{\"csrf_token\":\"(.*?)\"").Groups[1].Value;

            http.ClearAllHeaders();//Очищаем все заголовки => передаются только наши временные заговоки
            http.AddHeader("X-CSRFToken", token);
            http.AddHeader("X-Requested-With", "XMLHttpRequest");//установить временный заголовок для одного запроса
            http.AddHeader("Referer", url);//установить временный заголовок для одного запроса
            html = http.Post("https://www.instagram.com/accounts/login/ajax/", "username=" + login + "&password=" + Pass, "application/x-www-form-urlencoded").ToString();

            Cookies_Text(http.Cookies.ToString());
            if (http.Cookies.Count > 5)
            {
                AutorizLabel.Text = $"Вы вошли в instagram под аккаунтом: {login} .";
                AutorizLabel.ForeColor = Color.Green;
            }
        }

        ///Likes
        public async void Liking()
        {           
            Media("parsNAME.txt", names);

            string html;

            int start_count = Count_Read();//счетчик с какого человека в списке начинать
            int finish_count = Convert.ToInt32(likes.Text) + start_count;
            if (finish_count > names.Count) finish_count = names.Count;
            int save = start_count;

            schetchik = 1;
            vsego = names.Count - start_count;
            target = Convert.ToInt32(likes.Text);

            int delay = Convert.ToInt32(delaylike.Text) * 1000;

            Request request = new Request();
            HttpRequest http = new HttpRequest();            
            http.Cookies = Cookies();            

            for (int i = start_count; i < finish_count; i++)
            {
                LikeLabel.Invoke(new Action<string>((s) => LikeLabel.Text = s), "Поставили лайк : " + schetchik + " из " + target + " , всего " + vsego + " .");

                html = request.Get_With_Ex($"https://www.instagram.com/{names[i]}/", http);

                Regex regex = new Regex("\"GraphImage\",\"id\":\"" + @"\w{19}");
                MatchCollection matches = regex.Matches(html);

                if (matches.Count > 0)
                {
                    string photo = matches[0].Value.Substring(19);
                    string token = Regex.Match(html, "{\"csrf_token\":\"(.*?)\"").Groups[1].Value;
                    html = request.Like_With_Ex(http, html,photo,token, delay, LikeLabel);
                }                

                Count_Write(i);
                save = i + 1;
                schetchik++;
                if (Stopper)
                {
                    LikeLabel.Invoke(new Action<string>((s) => LikeLabel.Text = s), "Проставление лайков приостановленно...");
                    Stopper = false;
                    Continuer = false;
                    break;
                }
            }
            Count_Write(save);
        }

        ///Comments
        public async void Commenting()
        {
            Media("parsNAME.txt", names);

            string html;

            int start_count = Count_Read();//счетчик с какого человека в списке начинать
            int finish_count = Convert.ToInt32(commentscount.Text) + start_count;
            if (finish_count > names.Count) finish_count = names.Count;
            int save = start_count;

            schetchik = 1;
            vsego = names.Count - start_count;
            target = Convert.ToInt32(commentscount.Text);

            int delay = Convert.ToInt32(commentsdelay.Text) * 1000;

            Request request = new Request();
            HttpRequest http = new HttpRequest();
            http.Cookies = Cookies();

            for (int i = start_count; i < finish_count; i++)
            {
                ComLabel.Invoke(new Action<string>((s) => ComLabel.Text = s), "Поставили комментарий : " + schetchik + " из " + target + " , всего " + vsego + " .");

                html = request.Get_With_Ex($"https://www.instagram.com/{names[i]}/", http);

                Regex regex = new Regex("\"GraphImage\",\"id\":\"" + @"\w{19}");
                MatchCollection matches = regex.Matches(html);

                Regex disable = new Regex("comments_disabled\":" + @"\w{4}");
                MatchCollection dises = disable.Matches(html);

                if (matches.Count > 0)
                {
                    if (dises[0].Value == "comments_disabled\":true")
                    {
                        continue;
                    }
                    else
                    {
                        string comment = matches[0].Value.Substring(19);
                        string token = Regex.Match(html, "{\"csrf_token\":\"(.*?)\"").Groups[1].Value;

                        html = request.Comment_With_Ex(http, html, comment, token, delay, ComLabel, phrase.Text);
                    }
                }
                Count_Write(i);
                save = i + 1;
                schetchik++;
                if (Stopper)
                {
                    ComLabel.Invoke(new Action<string>((s) => ComLabel.Text = s), "Пауза...");
                    Stopper = false;
                    Continuer = false;
                    break; ;
                }
            }
            Count_Write(save);
        }

        public async void Story_Watch(int count)
        {
            string html="";
            string token = "";
            
            HttpRequest http = new HttpRequest();
            Parsing parsing = new Parsing();
            Request request = new Request();

            html = http.Get(url).ToString();
            token = Regex.Match(html, "{\"csrf_token\":\"(.*?)\"").Groups[1].Value;

            Media("parsID.txt", idforfollow);
            http.Cookies = Cookies();
            int statr_index = Count_Read();

            for (int i=0;i<count;i++)
            {
                //получили список id и time историй для одного пользователя под номером i
                parsing.Stories(http, html, idforfollow[i], out List<string> story_id, out List<string> story_time);

                //просмотрели его сторисы
                request.Story_With_Ex(http,token,story_id,story_time,idforfollow[i],1000,StoryLabel);
            }
            StoryLabel.Text = "vse";
        }
        ///UnFollows
        ///
        public async void Unfollowing()
        {
            List<string> unfoling = new List<string>();
            string html = "";
            HttpRequest http = new HttpRequest();

            //Инициализация
            Counter = 0;
            checker = 0;
            http.Cookies = Cookies();
            http.ClearAllHeaders();//Очищаем все заголовки => передаются только наши временные заговоки            
            http.AddHeader("Referer", " https://www.instagram.com/");//установить временный заголовок для одного запроса 
            string include_reel = "include_reel%22%3Atrue%2C%22";
            string fetch_mutual = "fetch_mutual%22%3Afalse%2C%22";
            string first = "5";
            string NICK = "";
            Request request = new Request();

            int k = 5;

            html = request.Get_With_Ex(url, http);

            Regex regfornic = new Regex("\"username\":\"" + @"(\w*)" + "\",\"edge_web_feed_timeline\"");
            MatchCollection nicks = regfornic.Matches(html);
            string[] sepnick = new string[2] { "username\":\"", "\",\"edge_web_feed_timeline\"" };
            string[] nick = nicks[0].Value.Split(sepnick, StringSplitOptions.RemoveEmptyEntries);
            NICK = nick[1];

            Regex regforid = new Regex("");
            string id = "";
            MatchCollection idis;

            //Выбор js для запроса.


            //Получение хэшей из js запроса
            Regex regforjs = new Regex("static/bundles/metro/Consumer.js/" + @"\S{15}");
            MatchCollection js = regforjs.Matches(html);

            html = request.Get_With_Ex(url + js[0].Value, http);

            Regex regforhash = new Regex(@"");
            string[] separator = new string[2];
            int end = 0;
            string query_hash = "";
            string[] words;
            MatchCollection hash; string x = "";

            regforhash = new Regex("FOLLOW_LIST_REQUEST_FAILED" + @".*" + "n=\"" + @".*" + "\"");
            separator = new string[2] { "n=\"", "\"" };
            id = yourid;

            hash = regforhash.Matches(html);
            words = hash[0].Value.Split(separator, StringSplitOptions.None);
            query_hash = words[3];
            first = "5";
            k = 5;

            end = 7500;

            //Запрос с готовым хэшем и переменными.
            html = request.Get_With_Ex($"https://www.instagram.com/graphql/query/?query_hash={query_hash}&variables=%7B%22id%22%3A%22{id}%22%2C%22{include_reel}{fetch_mutual}first%22%3A{first}%7D", http);

            Regex regforafter = new Regex("\"end_cursor\":\"" + @".*" + "==\"");
            MatchCollection afters = regforafter.Matches(html);
            separator = new string[2] { "\"end_cursor\":\"", "==\"" };
            string[] afterwords = afters[0].Value.Split(separator, StringSplitOptions.None);
            string after = afterwords[1];

            Regex regfornext = new Regex("\"has_next_page\":" + @"\w*" + ",");
            MatchCollection next = regfornext.Matches(html);
            separator = new string[2] { "\"has_next_page\":", "," };
            string[] nextwords = next[0].Value.Split(separator, StringSplitOptions.None);
            bool has_next_page = Convert.ToBoolean(nextwords[1]);

            Regex regforcount = new Regex("\"count\":" + @"\w*" + ",");
            MatchCollection counts = regforcount.Matches(html);
            separator = new string[2] { "\"count\":", "," };
            string[] countwords = counts[0].Value.Split(separator, StringSplitOptions.None);
            int count = Convert.ToInt32(countwords[1]);
            int helpcount = count;
            int countincycle = count;

            Regex regfornumber = new Regex("id\":\"" + @"\w*");
            MatchCollection numbers = regfornumber.Matches(html);

            separator = new string[1] { "id\":\"" };
            string[] nums = new string[1];
            if (numbers.Count > 0)
            {
                for (int i = 0; i < numbers.Count; i = i + 3)
                {
                    nums = numbers[i].Value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    unfoling.Add(nums[0]);
                }
            }


            int countunfollow = Convert.ToInt32(UnFolCount.Text);
            int delay = Convert.ToInt32(UnFolDelay.Text) * 1000;
            if (countunfollow > count) countunfollow = count;
            http.Cookies = Cookies();

            schetchik = 1;
            vsego = count;
            target = Convert.ToInt32(countunfollow);

            html = request.Get_With_Ex(url, http);

             string token = Regex.Match(html, "{\"csrf_token\":\"(.*?)\"").Groups[1].Value;

            for (int i = 0; i < countunfollow; i++)
            {
                metka:;
                
                UnfolLabel.Invoke(new Action<string>((s) => UnfolLabel.Text = s), "Отписались от: " + schetchik + " из " + target + " , всего " + vsego + " .");

                html = request.Unfollowing_With_Ex(http, html, token, delay, UnfolLabel, unfoling[i]);

                //тут снова гет запрос на количество если неизменилось то заново;
                html = request.Get_With_Ex(url + NICK, http);

                regforcount = new Regex("edge_follow\":{\"count\":" + @"\w*" + "},");
                counts = regforcount.Matches(html);
                separator = new string[2] { "edge_follow\":{\"count\":", "}," };
                countwords = counts[0].Value.Split(separator, StringSplitOptions.None);
                countincycle = Convert.ToInt32(countwords[1]);

                token = Regex.Match(html, "{\"csrf_token\":\"(.*?)\"").Groups[1].Value;

                if (countincycle == helpcount)
                {
                    Thread.Sleep(300000);
                    goto metka;
                }
                else helpcount = countincycle;
                schetchik++;

                if (Stopper)
                {                    
                    UnfolLabel.Invoke(new Action<string>((s) => UnfolLabel.Text = s), "Пауза...");
                    Stopper = false;
                    Continuer = false;
                    break;
                }

                if ((i == (unfoling.Count - 1)) && (i != 0))
                {
                    if (((count - 1) - i) < 5)
                    {
                        k = count - i - 1;
                        first = $"{k}";
                    }
                    //Запрос с готовым хэшем и переменными.
                    html = request.Get_With_Ex($"https://www.instagram.com/graphql/query/?query_hash={query_hash}&variables=%7B%22id%22%3A%22{id}%22%2C%22{include_reel}{fetch_mutual}first%22%3A{first}%2C%22after%22%3A%22{after}%3D%3D%22%7D", http);

                    regforafter = new Regex("\"end_cursor\":\"" + @".*" + "==\"");
                    afters = regforafter.Matches(html);
                    separator = new string[2] { "\"end_cursor\":\"", "==\"" };
                    afterwords = afters[0].Value.Split(separator, StringSplitOptions.None);
                    after = afterwords[1];

                    regfornext = new Regex("\"has_next_page\":" + @"\w*" + ",");
                    next = regfornext.Matches(html);
                    separator = new string[2] { "\"has_next_page\":", "," };
                    nextwords = next[0].Value.Split(separator, StringSplitOptions.None);
                    has_next_page = Convert.ToBoolean(nextwords[1]);

                    regfornumber = new Regex("id\":\"" + @"\w*");
                    numbers = regfornumber.Matches(html);

                    separator = new string[1] { "id\":\"" };
                    nums = new string[1];
                    if (numbers.Count > 0)
                    {
                        for (int j = 0; j < numbers.Count; j = j + 3)
                        {
                            nums = numbers[j].Value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                            unfoling.Add(nums[0]);
                        }
                    }
                    k = k + Convert.ToInt32(first);
                }
            }
        }
        
        ///Follows  
        public async void Following(string parameter)
        {
            string html = "";
            HttpRequest http = new HttpRequest();

            Media("parsID.txt", idforfollow);
            Media("parsNAME.txt", names);

            schetchik = 1;

            Request request = new Request();

            int count = Count_Read();//счетчик с какого человека в списке начинать
            int countfollow = 0;
            vsego = names.Count - count;
            int delay = 0;
            if (parameter == "Special")
            {
                countfollow = Convert.ToInt32(SpecialFolCount.Text) + count;
                delay = Convert.ToInt32(FollDelay.Text) * 1000;
                target = Convert.ToInt32(SpecialFolCount.Text);
            }
            else
            {
                countfollow = Convert.ToInt32(FollowCount.Text) + count;
                delay = Convert.ToInt32(FolDelay.Text) * 1000;
                target = Convert.ToInt32(FollowCount.Text);
            }

            if (countfollow > idforfollow.Count) countfollow = idforfollow.Count;

            http.Cookies = Cookies();
            int save = count;

            html = request.Get_With_Ex(url, http);

            string token = Regex.Match(html, "{\"csrf_token\":\"(.*?)\"").Groups[1].Value;

            for (int i = count; i < countfollow; i++)
            {
                Follabel.Invoke(new Action<string>((s) => Follabel.Text = s), "Подписались на  : " + schetchik + " из " + target + " , всего " + vsego + " .");

                html = request.Get_With_Ex($"https://www.instagram.com/{names[i]}/", http);
                if (html == "") continue;

                Regex regex = new Regex("\"GraphImage\",\"id\":\"" + @"\w{19}");
                MatchCollection matches = regex.Matches(html);

                Regex disable = new Regex("comments_disabled\":" + @"\w{4}");
                MatchCollection dises = disable.Matches(html);

                token = Regex.Match(html, "{\"csrf_token\":\"(.*?)\"").Groups[1].Value;
                if (token == "") continue;

                html = request.Follow_With_Ex(http, html, token, delay, Follabel, idforfollow[i]);

                int lik = Convert.ToInt32(FollowLikes.Value);
                int com = Convert.ToInt32(FollowComments.Value);

                if (matches.Count < lik) lik = matches.Count;
                if (matches.Count < com) com = matches.Count;

                if (parameter == "Special")
                {
                    if (lik > 0)
                    {
                        string like = "";
                        if (matches.Count > 0)
                        {
                            for (int j = 0; j < lik; j++)
                            {
                                like = matches[j].Value.Substring(19); 
                                
                                html = request.Like_With_Ex(http, html,like,token, delay, LikeLabel);
                            }
                        }
                    }

                    if (com > 0)
                    {
                        string comment = "";
                        if (matches.Count > 0)
                        {
                            if (dises[0].Value == "comments_disabled\":true")
                            {
                                int l = 0;
                            }
                            else
                            {
                                for (int k = 0; k < com; k++)
                                {
                                    Thread.Sleep(delay);

                                    comment = matches[k].Value.Substring(19);

                                    html = request.Comment_With_Ex(http, html, comment, token, delay, ComLabel, FollowPhrase.Text);
                                }
                            }
                        }
                    }
                }
                Count_Write(i);
                save = i + 1;
                schetchik++;

                if (Stopper)
                {
                    Follabel.Invoke(new Action<string>((s) => Follabel.Text = s), "Пауза...");
                    Stopper = false;
                    Continuer = false;
                    break;
                }
            }
            Count_Write(save);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Continuer = true;
            bw = new BackgroundWorker();
            bw.WorkerSupportsCancellation = true;
            
            bw.DoWork += (obj, ea) =>
            {
                Count_Write(0);
                Parsing parsing = new Parsing(
                    NickName,
                    ParseCount,
                    ParsLab,
                    IsOpened,
                    IsAvatar,
                    IsStory,
                    photoLab,
                    LikesUpDown,
                    IsAv,
                    IsLike,
                    IsCom,
                    LikesOnCom,
                    stopfile,
                    findfile,
                    stopword,
                    findword,
                    ComUpDown,
                    ParsSpeed.Value.ToString()                  
                    );
                if (radiofollowers.Checked == true) parsing.parsing("followers", "parsNAME.txt", "parsID.txt");
                else if (radiofollowings.Checked == true) parsing.parsing("followings", "parsNAME.txt", "parsID.txt");
                else if (radiolike.Checked == true) parsing.spec_parsing("Likers", "parsNAME.txt", "parsID.txt");
                else if (radiocomment.Checked == true) parsing.spec_parsing("Commentators", "parsNAME.txt", "parsID.txt");
                else if (radiodouble.Checked == true)
                {
                    parsing.spec_parsing("Likers", "helpdir/likename.txt", "helpdir/likeid.txt");
                    if (!Continuer) return;
                    parsing.spec_parsing("Commentators", "helpdir/comname.txt", "helpdir/comid.txt");
                    if (!Continuer) return;
                    parsing.likerpluscomer("helpdir/likeid.txt", "helpdir/likename.txt", "helpdir/comid.txt", "helpdir/comname.txt","parsID.txt", "parsNAME.txt" );
                }
            };           
            bw.RunWorkerAsync();
        }      

        private void button2_Click_1(object sender, EventArgs e)
        {
            Continuer = true;
            if (radioButton2.Checked==true)
            {
                bw = new BackgroundWorker();
                bw.WorkerSupportsCancellation = true;
                bw.DoWork += (obj, ea) =>
                {
                    Following("Special");
                };
                bw.RunWorkerAsync();
                
            }
            else if (radioButton1.Checked == true)
            {
                bw = new BackgroundWorker();
                bw.WorkerSupportsCancellation = true;
                bw.DoWork += (obj, ea) =>
                {
                    Following("Simple");
                    if (!Continuer) return;
                    Thread.Sleep(30000);
                    Liking();
                    if (!Continuer) return;
                    Thread.Sleep(30000);
                    Commenting();
                    if (!Continuer) return;
                    Thread.Sleep(30000);
                    Unfollowing();
                    if (!Continuer) return;
                };
                bw.RunWorkerAsync();
            }
            
        }
        

        
        private void инструкцияToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Status.Visible = true;
            Aupanel.Visible = false;
            Parspanel.Visible = false;
            Filpanel.Visible = false;
            Actpanel.Visible = false;
        }
        private void парсингToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Parspanel.Visible = true;
            Aupanel.Visible = false;            
            Status.Visible = false;
            Filpanel.Visible = false;
            Actpanel.Visible = false;
        }
        private void авторизацияToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            HttpRequest http = new HttpRequest();

            Aupanel.Visible = true;
            Status.Visible = false;
            Parspanel.Visible = false;
            Filpanel.Visible = false;
            Actpanel.Visible = false;
            http.Cookies = Cookies();
            if (http.Cookies.Count > 5)
            {
                Thread.Sleep(1000);
                string html = http.Get(url).ToString();
                Regex regfornic = new Regex("\"username\":\"" + @"(\w*)" + "\",\"edge_web_feed_timeline\"");
                MatchCollection nicks = regfornic.Matches(html);
                string[] separator = new string[2] { "username\":\"", "\",\"edge_web_feed_timeline\"" };
                string[] nick = nicks[0].Value.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                AutorizLabel.Text = $"Вы авторизированны под аккаунтом: {nick[1]} .";
                AutorizLabel.ForeColor = Color.Green;
                return;
            }
        }

        private void фильтрацияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filpanel.Visible = true;
            Aupanel.Visible = false;
            Status.Visible = false;
            Parspanel.Visible = false;
            Actpanel.Visible = false;
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            //Создаем массив чекеров
            bool[] Checkers = new bool[11] 
            {
                Is_Site.Checked, Is_Followers.Checked, Is_Followings.Checked, Is_Igtv.Checked, Is_Stories.Checked, Is_Buisness.Checked,
                Is_Recently.Checked, Is_Facebook.Checked, Is_Felix.Checked, Is_Media.Checked, Is_Saved.Checked
            };
            //Создаем массив для числовых данных
            string[] Digits = new string[12] 
            {
                Is_Followers_Min.Text, Is_Followers_Max.Text,
                Is_Followings_Min.Text, Is_Followings_Max.Text,
                Is_Stories_Min.Text, Is_Stories_Max.Text, 
                Is_Felix_Min.Text, Is_Felix_Max.Text,
                Is_Media_Min.Text, Is_Media_Max.Text,
                Is_Saved_Min.Text, Is_Saved_Max.Text
            };
            //Создаем и проверяем массив для строковых данных
            string[] Words = new string[5]; ;
            try
            {
                Words = new string [5]
                {
                Is_Site_Value.SelectedItem.ToString(),Is_Igtv_Value.SelectedItem.ToString(),Is_Buisness_Value.SelectedItem.ToString(),
                Is_Recently_Value.SelectedItem.ToString(),Is_Facebook_Value.SelectedItem.ToString()
                };
            }
            catch(Exception)
            {
                MessageBox.Show(
                        "В списке нужно выбирать один из двух вариантов(да или нет), а не писать там самостоятельно.",
                        "Ошибка настройки фильтров.",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation
                        );
                return;
            }

            //Создаем и проверям массив для путевых данных            
            Filter filter = new Filter("parsNAME.txt","parsID.txt",FilterLab,FilterButton,Stop_Filter);           

            //Запускаем фильтрацию и параллельно визуалицизацию.
            bw = new BackgroundWorker();
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += (obj, ea) =>
            {
                filter.filtering(Checkers, Digits, Words);
            };
            bw.RunWorkerAsync();
        }

        private void действияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Actpanel.Visible = true;
            Filpanel.Visible = false;
            Aupanel.Visible = false;
            Status.Visible = false;
            Parspanel.Visible = false;
        }
        private void Stoppars_Click(object sender, EventArgs e)
        {
            Stopper = true;
            Continuer = false;
        }
        private void Stop_Filter_Click(object sender, EventArgs e)
        {
            Stopper = true;
        }

        private void StopActions_Click(object sender, EventArgs e)
        {
            Stopper = true;
        }
        private void radioButton1_Click(object sender, EventArgs e)
        {
            radioButton1.Checked = true;
            radioButton2.Checked = false;
        }
        private void radioButton2_Click(object sender, EventArgs e)
        {
            radioButton2.Checked = true;
            radioButton1.Checked = false;
        }
        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void radioButton4_CheckedChanged_1(object sender, EventArgs e)
        {

        }
        private void radiodouble_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void label3_Click(object sender, EventArgs e)
        {

        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void radiolike_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click_1(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Story_Watch(10);
        }
    }
}
