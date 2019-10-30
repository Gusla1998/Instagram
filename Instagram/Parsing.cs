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
    class Parsing : MainForm
    {
        private string first;
        const int GET_TIME = 2000;
        TextBox NickName;
        TextBox ParseCount;
        Label ParsLab;
        CheckBox IsOpened;
        CheckBox IsAvatar;
        CheckBox IsStory;
        Label PhotoLab;
        NumericUpDown LikesUpDown;
        CheckBox IsAv;
        CheckBox IsCom;
        CheckBox IsLike;
        NumericUpDown LikesOnCom;
        TextBox StopFile;
        TextBox FindFile;
        RadioButton stopword;
        RadioButton findword;
        NumericUpDown ComUpDown;

        public Parsing(TextBox nickname,
            TextBox parsecount, 
            Label parslab, 
            CheckBox isopened, 
            CheckBox isavatar, 
            CheckBox isstory,
            Label photolab,
            NumericUpDown likesupdown,
            CheckBox isav,
            CheckBox islike,
            CheckBox iscom,
            NumericUpDown likesoncom,
            TextBox stopfile,
            TextBox findfile,
            RadioButton STOPword,
            RadioButton FINDword,
            NumericUpDown comupdown,
            string FIRST
            )
        {
            NickName = nickname;
            ParseCount = parsecount;
            ParsLab = parslab;
            IsOpened = isopened;
            IsAvatar = isavatar;
            IsStory = isstory;
            PhotoLab = photolab;
            LikesUpDown = likesupdown;
            IsAv = isav;
            IsCom = iscom;
            IsLike = islike;
            LikesOnCom = likesoncom;
            StopFile = stopfile;
            FindFile = findfile;
            stopword = STOPword;
            findword = FINDword;
            ComUpDown = comupdown;
            first = FIRST;
        }
        public Parsing()
        {

        }
        public async void parsing(string Value, string pathNAME, string pathID)
        {
            HttpRequest http = new HttpRequest();
            string html="";

            //Инициализация
            Counter = 0;
            checker = 0;
            http.Cookies = Cookies();
            http.ClearAllHeaders();//Очищаем все заголовки => передаются только наши временные заговоки            
            http.AddHeader("Referer", " https://www.instagram.com/");//установить временный заголовок для одного запроса 
            string include_reel = "include_reel%22%3Atrue%2C%22";
            string fetch_mutual = "fetch_mutual%22%3Afalse%2C%22";

            int k = 50;

            bool f = false;
            while (f == false)
            {
                try
                {
                    html = http.Get(url).ToString();
                    Thread.Sleep(GET_TIME);
                    f = true;
                }
                catch (HttpException ex)
                {
                    ParsLab.Invoke(new Action<string>((s) => ParsLab.Text = s), "Ошибка : неправильный никнейм , отключен интернет или бан аккаунта пауза 1 минута.");
                    Thread.Sleep(60000);
                    continue;
                }
            }
            
            Regex regforid = new Regex("");
            string id = "";
            MatchCollection idis;

            //Выбор js для запроса.

    
            //Получение хэшей из js запроса
            Regex regforjs = new Regex("static/bundles/metro/Consumer.js/" + @"\S{15}");
            MatchCollection js = regforjs.Matches(html);
            
            f = false;
            while (f == false)
            {
                try
                {
                    html = http.Get(url + js[0].Value).ToString();
                    Thread.Sleep(GET_TIME);
                    f = true;
                }
                catch (HttpException ex)
                {
                    ParsLab.Invoke(new Action<string>((s) => ParsLab.Text = s), "Ошибка : неправильный никнейм , отключен интернет или бан аккаунта пауза 1 минута.");
                    Thread.Sleep(60000);
                    continue;
                }
            }
            Regex regforhash = new Regex(@"");
            string[] separator = new string[2];
            int end = 0;
            string query_hash = "";
            string[] words;
            MatchCollection hash;string x = "";

            if (Value == "followers")
            {
                regforhash = new Regex("FOLLOW_LIST_REQUEST_FAILED" + @".*" + "var t=\"" + @".*" + "\"");
                separator = new string[2] { "var t=\"", "\"" };
                hash = regforhash.Matches(html);
                words = hash[0].Value.Split(separator, StringSplitOptions.None);
                query_hash = words[1];
                string nick = NickName.Text;

                
                f = false;
                while (f == false)
                {
                    try
                    {
                        html = http.Get(url + NickName.Text).ToString();
                        Thread.Sleep(GET_TIME);
                        f = true;
                    }
                    catch (HttpException ex)
                    {
                        ParsLab.Invoke(new Action<string>((s) => ParsLab.Text = s), "Ошибка : неправильный никнейм , отключен интернет или бан аккаунта пауза 1 минута.");
                        Thread.Sleep(60000);
                        continue;
                    }
                }

                regforid = new Regex("\"owner\":{\"id\":\"" + @"\w*");
                idis = regforid.Matches(html);
                id = idis[0].Value.Substring(15);

                end = Convert.ToInt32(ParseCount.Text);
            }            
            else if (Value == "followings")
            {
                regforhash = new Regex("FOLLOW_LIST_REQUEST_FAILED" + @".*" + "n=\"" + @".*" + "\"");
                separator = new string[2] { "n=\"", "\"" };
                hash = regforhash.Matches(html);
                x = hash[0].Value;
                words = hash[0].Value.Split(separator, StringSplitOptions.None);
                query_hash = words[3];
                string nick = NickName.Text;
                
                f = false;
                while (f == false)
                {
                    try
                    {
                        html = http.Get(url + NickName.Text).ToString();
                        Thread.Sleep(GET_TIME);
                        f = true;
                    }
                    catch (HttpException ex)
                    {
                        ParsLab.Invoke(new Action<string>((s) => ParsLab.Text = s), "Ошибка : неправильный никнейм , отключен интернет или бан аккаунта пауза 1 минута.");
                        Thread.Sleep(60000);
                        continue;
                    }
                }

                regforid = new Regex("\"owner\":{\"id\":\"" + @"\w*");
                idis = regforid.Matches(html);
                id = idis[0].Value.Substring(15);

                end = Convert.ToInt32(ParseCount.Text);
            }            

            //Запрос с готовым хэшем и переменными.
            f = false;
            while (f == false)
            {
                try
                {
                    html = http.Get($"https://www.instagram.com/graphql/query/?query_hash={query_hash}&variables=%7B%22id%22%3A%22{id}%22%2C%22{include_reel}{fetch_mutual}first%22%3A{first}%7D").ToString();
                    Thread.Sleep(GET_TIME);
                    f = true;
                }
                catch (HttpException ex)
                {
                    ParsLab.Invoke(new Action<string>((s) => ParsLab.Text = s), "Ошибка : неправильный никнейм , отключен интернет или бан аккаунта пауза 1 минута.");
                    Thread.Sleep(60000);
                    continue;
                }
            }

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

            if (end > count) end = count - count % k;

            string path = pathID;
            using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default))
            {
                Regex regfornumber = new Regex("id\":\"" + @"\w*");
                MatchCollection numbers = regfornumber.Matches(html);

                Regex regforprivate = new Regex("is_private\":" + @"\w{4}");
                MatchCollection privates = regforprivate.Matches(html);

                Regex regforavatar = new Regex("profile_pic_url\":\"https://" + @"\S{25}"); //scontent-arn2-1.cdninstagram.com/vp
                MatchCollection avatars = regforavatar.Matches(html);

                Regex regforstory = new Regex("latest_reel_media\":" + @"\w{4}");//1549641310,null
                MatchCollection stories = regforstory.Matches(html);

                separator = new string[1] { "id\":\"" };
                string[] nums = new string[1];
                if (numbers.Count > 0)
                {
                    for (int i = 0; i < numbers.Count; i = i + 3)
                    {
                        Checker(() => {
                            nums = numbers[i].Value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                            sw.WriteLine(nums[0]);
                        }, privates[i / 3].Value, stories[i / 3].Value, avatars[(i * 2 / 3)].Value);
                        continue;
                    }
                }
                sw.Close();
            }

            path = pathNAME;
            using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default))
            {
                Regex regforname = new Regex("username\":\"" + @"(\w|\.|_)*");
                MatchCollection namess = regforname.Matches(html);

                int t = namess.Count;

                Regex regforprivate = new Regex("is_private\":" + @"\w{4}");
                MatchCollection privates = regforprivate.Matches(html);

                Regex regforavatar = new Regex("profile_pic_url\":\"https://" + @"\S{25}"); //scontent-arn2-1.cdninstagram.com/vp
                MatchCollection avatars = regforavatar.Matches(html);

                int y = avatars.Count;

                Regex regforstory = new Regex("latest_reel_media\":" + @"\w{4}");//1549641310,null
                MatchCollection stories = regforstory.Matches(html);

                int z = stories.Count;

                separator = new string[1] { "username\":\"" };
                string[] namesss = new string[1];
                if (namess.Count > 0)
                {

                    for (int i = 0; i < namess.Count; i = i + 2)
                    {
                        Checker(() => {
                            namesss = namess[i].Value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                            sw.WriteLine(namesss[0]);
                            Counter++;
                        }, privates[i / 2].Value, stories[i / 2].Value, avatars[i].Value);
                        checker++;
                        continue;
                    }
                }
                sw.Close();
            }
            while (end > k)
            {               
                ParsLab.Invoke(new Action<string>((s) => ParsLab.Text = s), "Собрано людей : " + Counter + " из " + checker + " .");
                //Запрос с готовым хэшем и переменными.
                f = false;
                while (f == false)
                {
                    try
                    {
                        html = http.Get($"https://www.instagram.com/graphql/query/?query_hash={query_hash}&variables=%7B%22id%22%3A%22{id}%22%2C%22{include_reel}{fetch_mutual}first%22%3A{first}%2C%22after%22%3A%22{after}%3D%3D%22%7D").ToString();
                        f = true;
                        Thread.Sleep(GET_TIME);
                    }
                    catch (HttpException ex)
                    {
                        ParsLab.Invoke(new Action<string>((s) => ParsLab.Text = s), "Ошибка : неправильный никнейм , отключен интернет или бан аккаунта пауза 1 минута.");
                        Thread.Sleep(60000);
                        continue;
                    }
                }
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

                path = pathID;
                using (StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default))
                {
                    Regex regfornumber = new Regex("id\":\"" + @"\w*");
                    MatchCollection numbers = regfornumber.Matches(html);

                    Regex regforprivate = new Regex("is_private\":" + @"\w{4}");
                    MatchCollection privates = regforprivate.Matches(html);

                    Regex regforavatar = new Regex("profile_pic_url\":\"https://" + @"\S{25}"); //scontent-arn2-1.cdninstagram.com/vp
                    MatchCollection avatars = regforavatar.Matches(html);

                    Regex regforstory = new Regex("latest_reel_media\":" + @"\w{4}");//1549641310,null
                    MatchCollection stories = regforstory.Matches(html);

                    separator = new string[1] { "id\":\"" };
                    string[] nums = new string[1];
                    if (numbers.Count > 0)
                    {
                        for (int i = 0; i < numbers.Count; i = i + 3)
                        {
                            Checker(() => {
                                nums = numbers[i].Value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                sw.WriteLine(nums[0]);
                            }, privates[i / 3].Value, stories[i / 3].Value, avatars[(i / 3 * 2)].Value);
                            continue;
                        }
                    }
                    sw.Close();
                }
                path = pathNAME;
                using (StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default))
                {
                    Regex regforname = new Regex("username\":\"" + @"(\w|\.|_)*");
                    MatchCollection namess = regforname.Matches(html);

                    Regex regforprivate = new Regex("is_private\":" + @"\w{4}");
                    MatchCollection privates = regforprivate.Matches(html);

                    Regex regforavatar = new Regex("profile_pic_url\":\"https://" + @"\S{25}"); //scontent-arn2-1.cdninstag
                    MatchCollection avatars = regforavatar.Matches(html);

                    Regex regforstory = new Regex("latest_reel_media\":" + @"\w{4}");//1549,null
                    MatchCollection stories = regforstory.Matches(html);

                    separator = new string[1] { "username\":\"" };
                    string[] namesss = new string[1];

                    if (namess.Count > 0)
                    {
                        for (int i = 0; i < namess.Count; i = i + 2)
                        {
                            Checker(() => {
                                namesss = namess[i].Value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                sw.WriteLine(namesss[0]);
                                Counter++;
                            }, privates[i / 2].Value, stories[i / 2].Value, avatars[i].Value);
                            checker++;
                            continue;

                        }
                     }
                    sw.Close();
                }
                k = k + Convert.ToInt32(first);
                if (Stopper)
                {
                    Stopper = false;
                    return;
                }
            }
            ParsLab.Invoke(new Action<string>((s) => ParsLab.Text = s), "Собрано людей : " + Counter + " из " + checker + " .");
        }

        public async void spec_parsing(string Value, string pathNAME, string pathID)
        {
            HttpRequest http = new HttpRequest();
            string html = "";

            //Cбор фоток
            Photos();
            int phcount = photos.Count;

            //Инициализация
            Counter = 0;
            checker = 0;
            http.Cookies = Cookies();
            http.ClearAllHeaders();//Очищаем все заголовки => передаются только наши временные заговоки            
            http.AddHeader("Referer", " https://www.instagram.com/");//установить временный заголовок для одного запроса 
            bool f = false;
            while (f == false)
            {
                try
                {
                    html = http.Get(url).ToString();
                    Thread.Sleep(GET_TIME);
                    f = true;
                }
                catch (HttpException ex)
                {
                    ParsLab.Invoke(new Action<string>((s) => ParsLab.Text = s), "Ошибка : неправильный никнейм , отключен интернет или бан аккаунта пауза 1 минута.");
                    Thread.Sleep(60000);
                    continue;
                }
            }
            
            Regex regforid = new Regex("");
            string id = "";            
            MatchCollection idis;
            Regex regforhash = new Regex(@"");
            string[] separator = new string[2];
            int end = 0;
            string query_hash = "";
            string[] words;
            MatchCollection hash;
            int j = 0;
            string path = pathID;
            using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default))
            {
                sw.Close();
            }
            path = pathNAME;
            using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default))
            {
                sw.Close();
            }

            //Variables для запроса
            string shortcode = "";
            string include_reel = "include_reel%22%3Atrue%2C%22";//Только для likes
            string after = "";
            string x = "";

           
            //Выбор js для запроса.
            if (Value == "Likers")
            {
                Regex regforjs = new Regex("static/bundles/metro/Consumer.js/" + @"\S{15}");
                MatchCollection js = regforjs.Matches(html);
                
                f = false;
                while (f == false)
                {
                    try
                    {
                        html = http.Get(url + js[0].Value).ToString();
                        Thread.Sleep(GET_TIME);
                        f = true;
                    }
                    catch (HttpException ex)
                    {
                        ParsLab.Invoke(new Action<string>((s) => ParsLab.Text = s), "Ошибка : неправильный никнейм , отключен интернет или бан аккаунта пауза 1 минута.");
                        Thread.Sleep(60000);
                        continue;
                    }
                }

                regforhash = new Regex("var t=\\\"" + @".*" + "\\\"");
                separator = new string[1] { "\"" };
                hash = regforhash.Matches(html);
                
                words = hash[0].Value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                query_hash = words[1];
                string nick = NickName.Text;
                
                f = false;
                while (f == false)
                {
                    try
                    {
                        html = http.Get(url + NickName.Text).ToString();
                        Thread.Sleep(GET_TIME);
                        f = true;
                    }
                    catch (HttpException ex)
                    {
                        ParsLab.Invoke(new Action<string>((s) => ParsLab.Text = s), "Ошибка : неправильный никнейм , отключен интернет или бан аккаунта пауза 1 минута.");
                        Thread.Sleep(60000);
                        continue;
                    }
                }

                regforid = new Regex("\"owner\":{\"id\":\"" + @"\w*");
                idis = regforid.Matches(html);
                id = idis[0].Value.Substring(15);

                end = Convert.ToInt32(ParseCount.Text);
                int k = 0;
                likerid.Clear();
                likername.Clear();

                while ((end > k) & (phcount>j))
                {
                    //Запрос с готовым хэшем и переменными.                    
                    shortcode = photos[j];
                    f = false;
                    while (f == false)
                    {
                        try
                        {
                            html = http.Get($"https://www.instagram.com/graphql/query/?query_hash={query_hash}&variables=%7B%22shortcode%22%3A%22{shortcode}%22%2C%22{include_reel}first%22%3A{first}%7D").ToString();
                            Thread.Sleep(GET_TIME);
                            f = true;
                        }
                        catch (HttpException ex)
                        {
                            ParsLab.Invoke(new Action<string>((s) => ParsLab.Text = s), "Ошибка : неправильный никнейм , отключен интернет или бан аккаунта пауза 1 минута.");
                            Thread.Sleep(60000);
                            continue;
                        }
                    }

                    Regex regforafter = new Regex("\"end_cursor\":\"" + @".*" + "==\"");
                    MatchCollection afters = regforafter.Matches(html);
                    separator = new string[2] { "\"end_cursor\":\"", "==\"" };
                    string[] afterwords;                    
                    if (afters.Count > 0)
                    {
                        afterwords = afters[0].Value.Split(separator, StringSplitOptions.None);
                        after = afterwords[1];
                    }

                    Regex regfornext = new Regex("\"has_next_page\":" + @"\w*" + ",");
                    MatchCollection next = regfornext.Matches(html);
                    separator = new string[2] { "\"has_next_page\":", "," };
                    string[] nextwords;
                    bool has_next_page;
                    if (next.Count > 0)
                    {
                        nextwords = next[0].Value.Split(separator, StringSplitOptions.None);
                        has_next_page = Convert.ToBoolean(nextwords[1]);
                    }

                    Regex regforcount = new Regex("\"count\":" + @"\w*" + ",");
                    MatchCollection counts = regforcount.Matches(html);
                    separator = new string[2] { "\"count\":", "," };
                    string[] countwords = counts[0].Value.Split(separator, StringSplitOptions.None);
                    int count = Convert.ToInt32(countwords[1]);

                    if (end > count) count = count - count % 25;
                    else count=end;

                    Regex regfornumber = new Regex("id\":\"" + @"\w*");
                    MatchCollection numbers = regfornumber.Matches(html);

                    Regex regforprivate = new Regex("is_private\":" + @"\w{4}");
                    MatchCollection privates = regforprivate.Matches(html);

                    Regex regforavatar = new Regex("profile_pic_url\":\"https://" + @"\S{25}"); //scontent-arn2-1.cdninstagram.com/vp
                    MatchCollection avatars = regforavatar.Matches(html);

                    Regex regforstory = new Regex("latest_reel_media\":" + @"\w{4}");//1549641310,null
                    MatchCollection stories = regforstory.Matches(html);

                    separator = new string[1] { "id\":\"" };
                    string[] nums = new string[1];
                    if (numbers.Count > 0)
                    {
                        for (int i = 1; i < numbers.Count; i = i + 3)
                        {
                            Checker(() =>
                            {
                                nums = numbers[i].Value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                likerid.Add(nums[0]);
                            }, privates[(i - 1) / 3].Value, stories[(i - 1) / 3].Value, avatars[((i - 1) * 2 / 3)].Value);
                            continue;
                        }
                    }

                    Regex regforname = new Regex("username\":\"" + @"(\w|\.|_)*");
                    MatchCollection namess = regforname.Matches(html);

                    regforprivate = new Regex("is_private\":" + @"\w{4}");
                    privates = regforprivate.Matches(html);

                    regforavatar = new Regex("profile_pic_url\":\"https://" + @"\S{25}"); //scontent-arn2-1.cdninstagram.com/vp
                    avatars = regforavatar.Matches(html);

                    regforstory = new Regex("latest_reel_media\":" + @"\w{4}");//1549641310,null
                    stories = regforstory.Matches(html);

                    separator = new string[1] { "username\":\"" };
                    string[] namesss = new string[1];
                    if (namess.Count > 0)
                    {
                        for (int i = 0; i < namess.Count; i = i + 2)
                        {
                            Checker(() =>
                            {
                                namesss = namess[i].Value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                likername.Add(namesss[0]);
                                Counter++;
                            }, privates[i / 2].Value, stories[i / 2].Value, avatars[i].Value);
                            checker++;
                            continue;

                        }
                    }

                    k = k + Convert.ToInt32(first);

                    while (count > k)
                    {
                        ParsLab.Invoke(new Action<string>((s) => ParsLab.Text = s), "Собрано людей : " + Counter + " из " + checker + "(с повторениями) .");

                        f = false;
                            while (f == false)
                            {
                                try
                                {
                                    html = http.Get($"https://www.instagram.com/graphql/query/?query_hash={query_hash}&variables=%7B%22shortcode%22%3A%22{shortcode}%22%2C%22{include_reel}first%22%3A{first}%2C%22after%22%3A%22{after}%3D%3D%22%7D").ToString();
                                    f = true;
                                    Thread.Sleep(GET_TIME);
                                }
                                catch (HttpException ex)
                                {
                                ParsLab.Invoke(new Action<string>((s) => ParsLab.Text = s), "Ошибка : неправильный никнейм , отключен интернет или бан аккаунта пауза 1 минута.");
                                Thread.Sleep(60000);
                                    continue;
                                }
                            }                        
                            regforafter = new Regex("\"end_cursor\":\"" + @".*" + "==\"");
                            afters = regforafter.Matches(html);
                            separator = new string[2] { "\"end_cursor\":\"", "==\"" };
                            if (afters.Count > 0)
                            {
                                afterwords = afters[0].Value.Split(separator, StringSplitOptions.None);
                                after = afterwords[1];
                            }

                            regfornext = new Regex("\"has_next_page\":" + @"\w*" + ",");
                            next = regfornext.Matches(html);
                            separator = new string[2] { "\"has_next_page\":", "," };
                            nextwords = next[0].Value.Split(separator, StringSplitOptions.None);
                            has_next_page = Convert.ToBoolean(nextwords[1]);
                            
                            regfornumber = new Regex("id\":\"" + @"\w*");
                            numbers = regfornumber.Matches(html);

                            regforprivate = new Regex("is_private\":" + @"\w{4}");
                            privates = regforprivate.Matches(html);

                            regforavatar = new Regex("profile_pic_url\":\"https://" + @"\S{25}");
                            avatars = regforavatar.Matches(html);

                            regforstory = new Regex("latest_reel_media\":" + @"\w{4}");
                            stories = regforstory.Matches(html);

                            separator = new string[1] { "id\":\"" };
                            nums = new string[1];
                            if (numbers.Count > 0)
                            {
                                for (int i = 1; i < numbers.Count; i = i + 3)
                                {
                                    Checker(() =>
                                    {
                                        nums = numbers[i].Value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                        likerid.Add(nums[0]);
                                    }, privates[(i - 1) / 3].Value, stories[(i - 1)/ 3].Value, avatars[((i - 1) / 3 * 2)].Value);
                                    continue;
                                }
                            }                           

                            regforname = new Regex("username\":\"" + @"(\w|\.|_)*");
                            namess = regforname.Matches(html);

                            regforprivate = new Regex("is_private\":" + @"\w{4}");
                            privates = regforprivate.Matches(html);

                            regforavatar = new Regex("profile_pic_url\":\"https://" + @"\S{25}");
                            avatars = regforavatar.Matches(html);

                            regforstory = new Regex("latest_reel_media\":" + @"\w{4}");
                            stories = regforstory.Matches(html);

                            separator = new string[1] { "username\":\"" };
                            namesss = new string[1];

                            if (namess.Count > 0)
                            {
                                for (int i = 0; i < namess.Count; i = i + 2)
                                {
                                    Checker(() =>
                                    {
                                        namesss = namess[i].Value.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                                        likername.Add(namesss[0]);
                                        Counter++;
                                    }, privates[i / 2].Value, stories[i / 2].Value, avatars[i].Value);
                                    checker++;
                                    continue;
                                }
                            }

                            k = k + Convert.ToInt32(first);
                        
                            if (Stopper)
                            {
                                Stopper = false;
                                return;
                            }

                    }
                    
                    end = end - k;
                    k = 0;
                    j++;
                    
                }
                ParsLab.Invoke(new Action<string>((s) => ParsLab.Text = s), "Собрано людей : " + Counter + " из " + checker + "(с повторениями) .");
                Sorting(pathNAME, pathID, Convert.ToInt32(LikesUpDown.Value),likerid,likername);
            }
            if (Value == "Commentators")
            {
                Regex regforjs = new Regex("static/bundles/metro/ProfilePageContainer.js/" + @"\S{15}");
                MatchCollection js = regforjs.Matches(html);
                
                f = false;
                while (f == false)
                {
                    try
                    {
                        html = http.Get(url + js[0].Value).ToString();
                        Thread.Sleep(GET_TIME);
                        f = true;
                    }
                    catch (HttpException ex)
                    {
                        ParsLab.Invoke(new Action<string>((s) => ParsLab.Text = s), "Ошибка : неправильный никнейм , отключен интернет или бан аккаунта пауза 1 минута.");
                        Thread.Sleep(60000);
                        continue;
                    }
                }

                regforhash = new Regex("t.comments" + @".*" + "queryId" + @".*" + "edge_media_to_comment");
                separator = new string[1] { "\"" };
                hash = regforhash.Matches(html);

                words = hash[0].Value.Split(separator, StringSplitOptions.None);
                query_hash = words[1];
                string nick = NickName.Text;
                x = query_hash;

                
                f = false;
                while (f == false)
                {
                    try
                    {
                        html = http.Get(url + NickName.Text).ToString();
                        Thread.Sleep(GET_TIME);
                        f = true;
                    }
                    catch (HttpException ex)
                    {
                        ParsLab.Invoke(new Action<string>((s) => ParsLab.Text = s), "Ошибка : неправильный никнейм , отключен интернет или бан аккаунта пауза 1 минута.");
                        Thread.Sleep(60000);
                        continue;
                    }
                }

                regforid = new Regex("\"owner\":{\"id\":\"" + @"\w*");
                idis = regforid.Matches(html);
                id = idis[0].Value.Substring(15);

                end = Convert.ToInt32(ParseCount.Text);
                
                int k = 0;
                comerid.Clear();
                comername.Clear();
                bool flag = true;
                if (stopword.Checked == true) flag = false;
                if (findword.Checked == true) flag = true;

                while ((end > k) & (phcount > j))
                {
                    //Запрос с готовым хэшем и переменными.                    
                    shortcode = photos[j];
                    f = false;
                    while (f == false)
                    {
                        try
                        {
                            html = http.Get($"https://www.instagram.com/graphql/query/?query_hash={query_hash}&variables=%7B%22shortcode%22%3A%22{shortcode}%22%2C%22first%22%3A{first}%7D").ToString();
                            Thread.Sleep(GET_TIME);
                            f = true;
                        }
                        catch (HttpException ex)
                        {
                            ParsLab.Invoke(new Action<string>((s) => ParsLab.Text = s), "Ошибка : неправильный никнейм , отключен интернет или бан аккаунта пауза 1 минута.");
                            Thread.Sleep(60000);
                            continue;
                        }
                    }

                    Regex regforafter = new Regex("\"end_cursor\":\"" + @".*" + "==\"");
                    MatchCollection afters = regforafter.Matches(html);
                    separator = new string[2] { "\"end_cursor\":\"", "==\"" };
                    string[] afterwords;
                    if (afters.Count > 0)
                    {
                        afterwords = afters[0].Value.Split(separator, StringSplitOptions.None);
                        after = afterwords[1];
                    }

                    Regex regfornext = new Regex("\"has_next_page\":" + @"\w*" + ",");
                    MatchCollection next = regfornext.Matches(html);
                    separator = new string[2] { "\"has_next_page\":", "," };
                    string[] nextwords;
                    bool has_next_page;
                    if (next.Count > 0)
                    {
                        nextwords = next[0].Value.Split(separator, StringSplitOptions.None);
                        has_next_page = Convert.ToBoolean(nextwords[1]);
                    }

                    Regex regforcount = new Regex("\"count\":" + @"\w*" + ",");
                    MatchCollection counts = regforcount.Matches(html);
                    separator = new string[2] { "\"count\":", "," };
                    string[] countwords = counts[0].Value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    int count = Convert.ToInt32(countwords[0]);

                    if (end > count) count = count - count % 10;
                    else count = end;

                    //Only avatar,text,likedcount
                    Regex regfornumber = new Regex("id\":\"" + @"\w*");
                    MatchCollection numbers = regfornumber.Matches(html);//по 2 со второго

                    Regex regforavatar = new Regex("profile_pic_url\":\"https://" + @"\S{25}"); //scontent-arn2-1.cdninstagram.com/vp
                    MatchCollection avatars = regforavatar.Matches(html);//по 1

                    regforcount = new Regex("\"count\":" + @"\w*");
                    counts = regforcount.Matches(html);//по 1 начиная со 2

                    regforcount = new Regex("\"count\":" + @"\w*");
                    counts = regforcount.Matches(html);
                    int[] Counts = new int[counts.Count];
                    separator = new string[1] { "\"count\":" };
                    string [] nums = new string[1];
                    for (int i = 0; i < counts.Count; i++)
                    {
                        nums = counts[i].Value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        Counts[i] = Convert.ToInt32(nums[0]);
                    }

                    Regex regfortext = new Regex("text\":\"" + @"[^,]*"); 
                    MatchCollection texts = regfortext.Matches(html);//по 1

                    separator = new string[1] { "id\":\"" };
                    nums = new string[1];
                    if (numbers.Count > 0)
                    {
                        for (int i = 0; i < numbers.Count; i = i + 2)
                        {
                            CheckerForCom(() =>
                            {
                                nums = numbers[i + 1].Value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                comerid.Add(nums[0]);
                            }, avatars[i / 2].Value, Counts[(i / 2) + 1], texts[i / 2].Value, flag);
                            continue;
                        }
                    }

                    Regex regforname = new Regex("username\":\"" + @"(\w|\.|_)*");
                    MatchCollection namess = regforname.Matches(html);

                    separator = new string[1] { "username\":\"" };
                    string[] namesss = new string[1];
                    if (namess.Count > 0)
                    {
                        for (int i = 0; i < namess.Count;i++)
                        {
                            CheckerForCom(() =>
                            {
                                namesss = namess[i].Value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                comername.Add(namesss[0]);
                                Counter++;
                            }, avatars[i].Value, Counts[i + 1], texts[i].Value, flag);
                            checker++;
                            continue;

                        }
                    }

                    k = k + Convert.ToInt32(first);

                    while (count > k)
                    {
                        ParsLab.Invoke(new Action<string>((s) => ParsLab.Text = s), "Собрано людей : " + Counter + " из " + checker + "(с повторениями) .");
                        f = false;
                            while (f == false)
                            {
                                try
                                {
                                    html = http.Get($"https://www.instagram.com/graphql/query/?query_hash={query_hash}&variables=%7B%22shortcode%22%3A%22{shortcode}%22%2C%22first%22%3A{first}%2C%22after%22%3A%22{after}%3D%3D%22%7D").ToString();
                                    f = true;
                                    Thread.Sleep(GET_TIME);
                                }
                                catch (HttpException ex)
                                {
                                ParsLab.Invoke(new Action<string>((s) => ParsLab.Text = s), "Ошибка : неправильный никнейм , отключен интернет или бан аккаунта пауза 1 минута.");
                                Thread.Sleep(60000);
                                    continue;
                                }
                            }
                            regforafter = new Regex("\"end_cursor\":\"" + @".*" + "==\"");
                            afters = regforafter.Matches(html);
                            separator = new string[2] { "\"end_cursor\":\"", "==\"" };
                            if (afters.Count > 0)
                            {
                                afterwords = afters[0].Value.Split(separator, StringSplitOptions.None);
                                after = afterwords[1];
                            }

                            regfornext = new Regex("\"has_next_page\":" + @"\w*" + ",");
                            next = regfornext.Matches(html);
                            separator = new string[2] { "\"has_next_page\":", "," };
                            nextwords = next[0].Value.Split(separator, StringSplitOptions.None);
                            has_next_page = Convert.ToBoolean(nextwords[1]);

                            //Only avatar,text,likedcount
                            regfornumber = new Regex("id\":\"" + @"\w*");
                            numbers = regfornumber.Matches(html);

                            regforavatar = new Regex("profile_pic_url\":\"https://" + @"\S{25}"); //scontent-arn2-1.cdninstagram.com/vp
                            avatars = regforavatar.Matches(html);

                            regforcount = new Regex("\"count\":" + @"\w*");
                            counts = regforcount.Matches(html);
                            Counts = new int[counts.Count];
                            separator = new string[1] { "\"count\":"};
                            nums = new string[1];
                            for (int i=0;i<counts.Count;i++)
                            {
                                nums = counts[i].Value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                Counts[i] = Convert.ToInt32(nums[0]);
                            }

                            regfortext = new Regex("text\":\"" + @"[^,]*");
                            texts = regfortext.Matches(html);//по 1

                            separator = new string[1] { "id\":\"" };
                            nums = new string[1];
                            if (numbers.Count > 0)
                            {
                                for (int i = 0; i < numbers.Count; i = i + 2)
                                {
                                    CheckerForCom(() =>
                                    {
                                        nums = numbers[i + 1].Value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                        comerid.Add(nums[0]);                                        
                                    }, avatars[i / 2].Value, Counts[(i/2)+1], texts[i/2].Value,flag);
                                    continue;                                    
                                }
                            }

                            regforname = new Regex("username\":\"" + @"(\w|\.|_)*");
                            namess = regforname.Matches(html);

                            separator = new string[1] { "username\":\"" };
                            namesss = new string[1];
                            if (namess.Count > 0)
                            {
                                for (int i = 0; i < namess.Count;i++)
                                {
                                    
                                    CheckerForCom(() =>
                                    {
                                        namesss = namess[i].Value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                        comername.Add(namesss[0]);
                                        Counter++;                                        
                                    }, avatars[i].Value, Counts[i + 1], texts[i].Value, flag);
                                    checker++;
                                    continue;
                                }
                            }
                            k = k + Convert.ToInt32(first);
                        if (Stopper)
                        {
                            Stopper = false;
                            return;
                        }
                    }
                    end = end - k;
                    k = 0;
                    j++;                    
                }
                ParsLab.Invoke(new Action<string>((s) => ParsLab.Text = s), "Собрано людей : " + Counter + " из " + checker + "(с повторениями) .");
                Sorting(pathNAME, pathID,Convert.ToInt32(ComUpDown.Value),comerid,comername);
            }
        }

        public async void likerpluscomer(string pathIDlike, string pathNAMElike,string pathIDcom,string pathNAMEcom, string pathID,string pathNAME)
        {
            string path;
            List<string> likeid = new List<string>();
            List<string> likename = new List<string>();
            List<string> comid = new List<string>();
            List<string> comname = new List<string>();

            path = pathIDlike;
            using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    likeid.Add(line);
                }
                sr.Close();
            }
            path = pathNAMElike;
            using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    likename.Add(line);
                }
                sr.Close();
            }
            path = pathIDcom;
            using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    comid.Add(line);
                }
                sr.Close();
            }
            path = pathNAMEcom;
            using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    comname.Add(line);
                }
                sr.Close();
            }

            var id = likeid.Intersect<string>(comid);
            var name = likename.Intersect<string>(comname);
            checker = likeid.Count<string>() + comid.Count<string>() - id.Count<string>();
            Counter = 0;

            path = pathID;
            using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default))
            {
                foreach (string man in id)
                {
                    sw.WriteLine(man);
                    Counter++;   
                }
                sw.Close();
            }

            path = pathNAME;
            using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default))
            {
                foreach (string man in name)
                {
                    sw.WriteLine(man);
                }
                sw.Close();
            }
            ParsLab.Invoke(new Action<string>((s) => ParsLab.Text = s), "Собрано людей : " + Counter + " из " + checker + " .(лайкающих или комментирующих)");

        }

        public void Stories(HttpRequest http,string html,string id,out List<string> story_id,out List<string> story_time)
        {
            http.ClearAllHeaders();//Очищаем все заголовки => передаются только наши временные заговоки            
            http.AddHeader("Referer", " https://www.instagram.com/");//установить временный заголовок для одного запроса 
            Request request = new Request();

            //(1 запрос)Получение хэшей из js запроса , обращаемся к инстаграму.
            html = request.Get_With_Ex(url, http);            
            Regex regforjs = new Regex("static/bundles/metro/Consumer.js/" + @"\S{15}");
            MatchCollection js = regforjs.Matches(html);

            //(2 запрос)Получение хэша для отправки запроса на id сторисов.
            html = request.Get_With_Ex(url + js[0].Value, http);
            Regex regforhash = new Regex("close_friends_story_ring_click_through" + @".*" + "h=\"" + @".*" + "\"");
            string[] separator = new string[2] { "h=\"", "\"" };
            MatchCollection hash = regforhash.Matches(html);
            string[] words = hash[0].Value.Split(separator, StringSplitOptions.None);
            string query_hash = words[1];

            //(3 запрос)Получение массива id историй.
            string symbols_for_request = 
                $"/graphql/query/?query_hash={query_hash}&variables=" +
                $"%7B%22reel_ids%22%3A%5B%22{id}%22%5D" +
                $"%2C%22tag_names%22%3A%5B%5D" +
                $"%2C%22location_ids%22%3A%5B%5D" +
                $"%2C%22highlight_reel_ids%22%3A%5B%5D" +
                $"%2C%22precomposed_overlay%22%3Afalse" +
                $"%2C%22show_story_viewer_list%22%3Atrue" +
                $"%2C%22story_viewer_fetch_count%22%3A50" +
                $"%2C%22story_viewer_cursor%22%3A%22%22" +
                $"%2C%22stories_video_dash_manifest%22%3Afalse%7D";
            html = request.Get_With_Ex(url+symbols_for_request, http);

            //(4 действие, не запрос)Выбираем информацию для просмотра сторисов
            story_id = new List<string>();
            story_time = new List<string>();
            Regex regex_id = new Regex("GraphStory"+@"\w*"+"\",\"id\":\"" + @"\d{19}");
            Regex regex_time = new Regex("taken_at_timestamp\":" + @"\d*");
            MatchCollection match_id = regex_id.Matches(html);
            MatchCollection match_time = regex_time.Matches(html);
            string[] separator_id = new string[1] { "id\":\"" };
            string[] separator_time = new string[1] { "taken_at_timestamp\":" };
            if (match_id.Count > 0)
            {
                for (int i = 0; i < match_id.Count; i++)
                {
                    string[] separating_id = match_id[i].Value.Split(separator_id, StringSplitOptions.RemoveEmptyEntries);
                    string[] separating_time = match_time[i].Value.Split(separator_time, StringSplitOptions.RemoveEmptyEntries);
                    story_id.Add(separating_id[1]);
                    story_time.Add(separating_time[0]);
                }
            }
        }

        public async void Photos()
        {
            HttpRequest http = new HttpRequest();
            string html = "";

            //Инициализация
            Counterph = 0;
            checkerph = 0;
            http.Cookies = Cookies();
            http.ClearAllHeaders();//Очищаем все заголовки => передаются только наши временные заговоки            
            http.AddHeader("Referer", " https://www.instagram.com/");//установить временный заголовок для одного запроса 
            
            bool f = false;
            while (f == false)
            {
                try
                {
                    html = http.Get(url).ToString();
                    Thread.Sleep(GET_TIME);
                    f = true;
                }
                catch (HttpException ex)
                {
                    PhotoLab.Invoke(new Action<string>((s) => PhotoLab.Text = s), "Ошибка : неправильный никнейм , отключен интернет или бан аккаунта пауза 1 минута.");
                    Thread.Sleep(60000);
                    continue;
                }
            }

            Regex regforid = new Regex("");
            photos.Clear();
            
            MatchCollection idis;
            Regex regforhash = new Regex(@"");
            string[] separator = new string[2];
            int end = 0;
            string query_hash = "";
            string[] words;
            MatchCollection hash;

            //Variables для запроса
            string id = "";
            string after = "";
            string x = "";

            Regex regforjs = new Regex("static/bundles/metro/ProfilePageContainer.js/" + @"\S{15}");
            MatchCollection js = regforjs.Matches(html);
            
            f = false;
            while (f == false)
            {
                try
                {
                    html = http.Get(url + js[0].Value).ToString();
                    Thread.Sleep(GET_TIME);
                    f = true;
                }
                catch (HttpException ex)
                {
                    PhotoLab.Invoke(new Action<string>((s) => PhotoLab.Text = s), "Ошибка : неправильный никнейм , отключен интернет или бан аккаунта пауза 1 минута.");
                    Thread.Sleep(60000);
                    continue;
                }
            }

            regforhash = new Regex("n.profilePosts" + @".*" + "queryId" + @".*" + "edge_owner_to_timeline_media");
            separator = new string[1] { "\"" };
            hash = regforhash.Matches(html);
            x = html;

            words = hash[0].Value.Split(separator, StringSplitOptions.None);
            query_hash = words[1];
            string nick = NickName.Text;
            

            
            f = false;
            while (f == false)
            {
                try
                {
                    html = http.Get(url + NickName.Text).ToString();
                    Thread.Sleep(GET_TIME);
                    f = true;
                }
                catch (HttpException ex)
                {
                    PhotoLab.Invoke(new Action<string>((s) => PhotoLab.Text = s), "Ошибка : неправильный никнейм , отключен интернет или бан аккаунта пауза 1 минута.");
                    Thread.Sleep(60000);
                    continue;
                }
            }

            regforid = new Regex("\"owner\":{\"id\":\"" + @"\w*");
            idis = regforid.Matches(html);
            id = idis[0].Value.Substring(15);

            end = 10000;

            //Запрос с готовым хэшем и переменными.
            f = false;
            while (f == false)
            {
                try
                {
                    html = http.Get($"https://www.instagram.com/graphql/query/?query_hash={query_hash}&variables=%7B%22id%22%3A%22{id}%22%2C%22first%22%3A{first}%7D").ToString();
                    Thread.Sleep(GET_TIME);
                    f = true;
                }
                catch (HttpException ex)
                {
                    PhotoLab.Invoke(new Action<string>((s) => PhotoLab.Text = s), "Ошибка : неправильный никнейм , отключен интернет или бан аккаунта пауза 1 минута.");
                    Thread.Sleep(60000);
                    continue;
                }
            }

            int k = 10;

            Regex regforafter = new Regex("\"end_cursor\":\"" + @".*" + "==\"");
            MatchCollection afters = regforafter.Matches(html);
            separator = new string[2] { "\"end_cursor\":\"", "==\"" };
            string[] afterwords = afters[0].Value.Split(separator, StringSplitOptions.None);
            after = afterwords[1];

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

            Regex regforcom = new Regex("\"comments_disabled\":" + @"\w*");
            MatchCollection coms = regforcom.Matches(html);
            string [] separator1 = new string[1] { "\"comments_disabled\":" };
            string[] comswords;
            string hasdis;

            if (end > count) end = count - count % 10;            
            
            Regex regforcode = new Regex("shortcode\":\"" + @".{100}");
            MatchCollection codes = regforcode.Matches(html);
            
            separator = new string[2] { "shortcode\":\"" , "\","};
            string[] code = new string[1];
            

            if (codes.Count > 0)
            {
                for (int i = 0; i < codes.Count; i = i + 1)
                {
                    comswords = coms[i].Value.Split(separator1, StringSplitOptions.RemoveEmptyEntries);
                    hasdis = comswords[0];

                    if (hasdis == "false")
                    {
                        code = codes[i].Value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        photos.Add(code[0]);
                        Counterph++;
                    }                   

                    
                    checkerph++;
                    continue;
                }
            }

            while (end > k)
            {
                PhotoLab.Invoke(new Action<string>((s) => PhotoLab.Text = s), "Собрано фотографий : " + Counterph + " из " + checkerph + " .");
                
                    //Запрос с готовым хэшем и переменными.
                    f = false;
                    while (f == false)
                    {
                        try
                        {
                            html = http.Get($"https://www.instagram.com/graphql/query/?query_hash={query_hash}&variables=%7B%22id%22%3A%22{id}%22%2C%22first%22%3A{first}%2C%22after%22%3A%22{after}%3D%3D%22%7D").ToString();
                            Thread.Sleep(GET_TIME);
                            f = true;
                        }
                        catch (HttpException ex)
                        {
                        PhotoLab.Invoke(new Action<string>((s) => PhotoLab.Text = s), "Ошибка : неправильный никнейм , отключен интернет или бан аккаунта пауза 1 минута.");
                        Thread.Sleep(60000);
                            continue;
                        }
                    }

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
                    
                    coms = regforcom.Matches(html);

                    regforcode = new Regex("shortcode\":\"" + @".{100}");
                    codes = regforcode.Matches(html);

                    separator = new string[2] { "shortcode\":\"", "\"," };
                    code = new string[1];

                    if (codes.Count > 0)
                    {
                        for (int i = 0; i < codes.Count; i = i + 1)
                        {
                            comswords = coms[i].Value.Split(separator1, StringSplitOptions.RemoveEmptyEntries);
                            hasdis = comswords[0];

                            if (hasdis == "false")
                            {
                                code = codes[i].Value.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                                photos.Add(code[0]);
                                Counterph++;
                            }

                            checkerph++;
                            continue;
                        }
                    }
                    k = k + Convert.ToInt32(first);
                if (Stopper)
                {
                    
                    Stopper = false;
                    return;
                }

            }
            PhotoLab.Invoke(new Action<string>((s) => PhotoLab.Text = s), "Собрано фотографий : " + Counterph + " из " + checkerph + " .");
        }

        public async void Checker(Action action, string open, string story, string avatar)
        {
            if ((IsOpened.Checked == true) && (IsStory.Checked == true) && (IsAvatar.Checked == true))
            {
                if ((avatar == "profile_pic_url\":\"https://scontent-arn2-1.cdninstag") && (open == "is_private\":fals") && (story != "latest_reel_media\":null")) action();
            }
            else if ((IsOpened.Checked == true) && (IsStory.Checked == true) && (IsAvatar.Checked == false))
            {
                if ((open == "is_private\":fals") && (story != "latest_reel_media\":null")) action();
            }
            else if ((IsOpened.Checked == true) && (IsStory.Checked == false) && (IsAvatar.Checked == true))
            {
                if ((open == "is_private\":fals") && (avatar == "profile_pic_url\":\"https://scontent-arn2-1.cdninstag")) action();
            }
            else if ((IsOpened.Checked == false) && (IsStory.Checked == true) && (IsAvatar.Checked == true))
            {
                if ((avatar == "profile_pic_url\":\"https://scontent-arn2-1.cdninstag") && (story != "latest_reel_media\":null")) action();
            }
            else if ((IsOpened.Checked == true) && (IsStory.Checked == false) && (IsAvatar.Checked == false))
            {
                if (open == "is_private\":fals") action();
            }
            else if ((IsOpened.Checked == false) && (IsStory.Checked == true) && (IsAvatar.Checked == false))
            {
                if (story != "latest_reel_media\":null") action();
            }
            else if ((IsOpened.Checked == false) && (IsStory.Checked == false) && (IsAvatar.Checked == true))
            {
                if (avatar == "profile_pic_url\":\"https://scontent-arn2-1.cdninstag") action();
            }
            else action();
        }

        public async void CheckerForCom(Action action,string avatar,int count,string text,bool option)
        {
            if ((IsLike.Checked == true) && (IsCom.Checked == true) && (IsAv.Checked == true))
            {
                if (option)
                {
                    if ((avatar == "profile_pic_url\":\"https://scontent-arn2-1.cdninstag") && (count >= LikesOnCom.Value) && (FindWord(FindFile.Text,text))) action();
                }
                else
                {
                    if ((avatar == "profile_pic_url\":\"https://scontent-arn2-1.cdninstag") && (count >= LikesOnCom.Value) && (StopWord(StopFile.Text,text))) action();
                }
            }
            else if ((IsLike.Checked == true) && (IsCom.Checked == true) && (IsAv.Checked == false))
            {
                if (option)
                {
                    if ((count >= LikesOnCom.Value) && (FindWord(FindFile.Text, text))) action();
                }
                else
                {
                    if ((count >= LikesOnCom.Value) && (StopWord(StopFile.Text, text))) action();
                }
            }
            else if ((IsLike.Checked == true) && (IsCom.Checked == false) && (IsAv.Checked == true))
            {
                if ((count >= LikesOnCom.Value) && (avatar == "profile_pic_url\":\"https://scontent-arn2-1.cdninstag")) action();
            }
            else if ((IsLike.Checked == false) && (IsCom.Checked == true) && (IsAv.Checked == true))
            {
                if (option)
                {
                    if ((avatar == "profile_pic_url\":\"https://scontent-arn2-1.cdninstag") && (FindWord(FindFile.Text, text))) action();
                }
                else
                {
                    if ((avatar == "profile_pic_url\":\"https://scontent-arn2-1.cdninstag") && (StopWord(StopFile.Text, text))) action();
                }
            }
            else if ((IsLike.Checked == true) && (IsCom.Checked == false) && (IsAv.Checked == false))
            {
                if (count >= LikesOnCom.Value) action();
            }
            else if ((IsLike.Checked == false) && (IsCom.Checked == true) && (IsAv.Checked == false))
            {
                if (option)
                {
                    if (FindWord(FindFile.Text, text)) action();
                }
                else
                {
                    if (StopWord(StopFile.Text, text)) action();
                }
            }
            else if ((IsLike.Checked == false) && (IsCom.Checked == false) && (IsAv.Checked == true))
            {
                if (avatar == "profile_pic_url\":\"https://scontent-arn2-1.cdninstag") action();
            }
            else action();
        }

        public bool StopWord(string file,string text)
        {
            using (StreamReader sr = new StreamReader(file, System.Text.Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    Regex find = new Regex(@".*" + line + @".*");
                    MatchCollection findintext = find.Matches(text);
                    if (findintext.Count > 0)
                    {
                        sr.Close();
                        return false;
                    }
                }
                sr.Close();
            }
            return true;
        }
        public bool FindWord(string file, string text)
        {
            using (StreamReader sr = new StreamReader(file, System.Text.Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    Regex find = new Regex(@".*" + line + @".*");
                    MatchCollection findintext = find.Matches(text);
                    if (findintext.Count > 0)
                    {
                        sr.Close();
                        return true;
                    }
                }
                sr.Close();
            }
            return false;
        }
        
        public  async void Sorting(string pathNAME, string pathID,int count,List<string> sortid,List<string> sortname)
        {
            //Инициализация
            IEnumerable<string> norepeat = sortid.Distinct<string>();
            IEnumerable<string> norepeat1 = sortname.Distinct<string>();
            int k;
            string path;
            path = pathNAME;

            checker = norepeat.Count<string>();
            Counter = 0;

            using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default))
            {                
                sw.Close();
            }
            path = pathID;
            using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default))
            {
                sw.Close();
            }

            //Сортировка1
            path = pathID;
            using (StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default))
            {
                foreach (string man in norepeat)
                {
                    k = 0;
                    for (int i = 0; i < sortid.Count; i++)
                    {
                        if (man == sortid[i]) k++;
                    }
                    if (k >= count)
                    {
                        sw.WriteLine(man);
                        Counter++;
                    }
                }
                sw.Close();
            }

            //Сортировка2
            path = pathNAME;
            using (StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default))
            {
                foreach (string man in norepeat1)
                {
                    k = 0;
                    for (int i = 0; i < sortname.Count; i++)
                    {
                        if (man == sortname[i]) k++;
                    }
                    if (k >= count)
                    {
                        sw.WriteLine(man);                        
                    }
                }
                sw.Close();
            }
            ParsLab.Invoke(new Action<string>((s) => ParsLab.Text = s), "Собрано людей : " + Counter + " из " + checker + " .(без повторений)");
        }
    }
}
