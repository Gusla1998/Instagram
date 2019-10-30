using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xNet;
using System.Threading;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Net;

namespace Instagram
{
    class Request
    {
        private const int GET_TIME = 1000;
        private const int GET_WAIT_TIME = 30000;
        private const int POST_WAIT_TIME = 300000;
        private const int COMMENT_WAIT_TIME = 900000;

        public string Get_With_Ex(string Request_String,HttpRequest http)
        {            
            string html = "";

            bool check = false;
            while(check == false)
            {
                try
                {
                    html = http.Get(Request_String).ToString();
                    Thread.Sleep(GET_TIME);
                    check = true;
                }
                catch(HttpException ex)
                {
                    Regex exp = new Regex(@"(\d{3})");
                    MatchCollection status = exp.Matches(ex.Message);

                    if (status.Count > 0)
                    {
                        switch (status[0].Value)
                        {
                            case ("404"):
                                {
                                    html = "";
                                    check = true;
                                    continue;
                                }
                            case ("429"):
                                {
                                    Thread.Sleep(GET_WAIT_TIME);
                                    continue;
                                }
                            case ("200"):
                                {
                                    continue;
                                }
                        };
                    }
                    else
                    {
                        Thread.Sleep(GET_WAIT_TIME);
                    }
                }
            }
            return html;
        }

        public string Like_With_Ex(HttpRequest http,string html,string photo,string token,int delay,Label LikeLabel)
        {
            string HTML = html;

            bool check = false;
            while (check == false)
            {
                try
                {
                    http.ClearAllHeaders();//Очищаем все заголовки => передаются только наши временные заговоки
                    http.AddHeader("X-CSRFToken", token);//установить временный заголовок для одного запроса
                    http.AddHeader("X-Requested-With", "XMLHttpRequest");//установить временный заголовок для одного запроса
                    http.AddHeader("Referer", " https://www.instagram.com/");//установить временный заголовок для одного запроса                     

                    byte[] bytes = new byte[8];

                    html = http.Post($"https://www.instagram.com/web/likes/{photo}/like/", bytes, "application/x-www-form-urlencoded").ToString();
                    Thread.Sleep(delay);
                    check = true;
                }
                catch (HttpException)
                {
                    LikeLabel.Invoke(new Action<string>((s) => LikeLabel.Text = s), "Пауза из-за активных действий 5 минут...");
                    Thread.Sleep(POST_WAIT_TIME);

                    continue;
                }
            }

            return HTML;
        }

        public string Comment_With_Ex(HttpRequest http, string html,string comment,string token, int delay, Label ComLabel,string phrase)
        {
            string HTML = "";

            bool check = false;
            while (check == false)
            {
                try
                {
                    http.ClearAllHeaders();//Очищаем все заголовки => передаются только наши временные заговоки
                    http.AddHeader("X-CSRFToken", token);//установить временный заголовок для одного запроса
                    http.AddHeader("X-Requested-With", "XMLHttpRequest");//установить временный заголовок для одного запроса
                    string str = $"comment_text={phrase}&replied_to_comment_id=";

                    html = http.Post($"https://www.instagram.com/web/comments/{comment}/add/", str, "application/x-www-form-urlencoded").ToString();
                    Thread.Sleep(delay);
                    check = true;
                }
                catch (HttpException)
                {
                    ComLabel.Invoke(new Action<string>((s) => ComLabel.Text = s), "Пауза из-за активных действий 15 минут...");
                    Thread.Sleep(COMMENT_WAIT_TIME);

                    continue;
                }

            }

            return HTML;
        }

        public string Unfollowing_With_Ex(HttpRequest http, string html, string token, int delay, Label UnfolLabel,string human)
        {
            string HTML = "";

            bool check = false;
            while (check == false)
            {
                try
                {
                    http.ClearAllHeaders();//Очищаем все заголовки => передаются только наши временные заговоки
                    http.AddHeader("X-CSRFToken", token);//установить временный заголовок для одного запроса
                    http.AddHeader("X-Requested-With", "XMLHttpRequest");//установить временный заголовок для одного запроса
                    http.AddHeader("Referer", "https://www.instagram.com/");//установить временный заголовок для одного запроса 
                    byte[] bytes = new byte[8];

                    HTML = http.Post($"https://www.instagram.com/web/friendships/{human}/unfollow/", bytes, "application/x-www-form-urlencoded").ToString();
                    Thread.Sleep(delay);
                    check = true;
                }
                catch (HttpException)
                {
                    UnfolLabel.Invoke(new Action<string>((s) => UnfolLabel.Text = s), "Пауза из-за активных действий 5-10 минут...");
                    Thread.Sleep(POST_WAIT_TIME);

                    continue;
                }
            }

            return HTML;
        }

        public string Follow_With_Ex(HttpRequest http, string html, string token, int delay, Label Follabel, string human)
        {
            bool check = false;
            while (check == false)
            {
                try
                {
                    http.ClearAllHeaders();//Очищаем все заголовки => передаются только наши временные заговоки
                    http.AddHeader("X-CSRFToken", token);//установить временный заголовок для одного запроса
                    http.AddHeader("X-Requested-With", "XMLHttpRequest");//установить временный заголовок для одного запроса
                    http.AddHeader("Referer", "https://www.instagram.com");//установить временный заголовок для одного запроса 
                    byte[] bytes = new byte[8];

                    html = http.Post($"https://www.instagram.com/web/friendships/{human}/follow/", bytes, "application/x-www-form-urlencoded").ToString();
                    Thread.Sleep(delay);
                    check = true;
                }
                catch (HttpException)
                {
                    Follabel.Invoke(new Action<string>((s) => Follabel.Text = s), "Пауза 5 минут...");
                    Thread.Sleep(POST_WAIT_TIME);
                    continue;
                }
            }

            return html;
        }

        public string Story_With_Ex(HttpRequest http,string token, List<string> story_id,List<string> story_time,string id, int delay,Label StoryLabel)
        {
            string html="";

            if (story_id.Count > 0)
            {
                bool check = false;
                while (check == false)
                {
                    try
                    {
                        http.ClearAllHeaders();
                        http.AddHeader("X-CSRFToken", token);
                        http.AddHeader("X-Requested-With", "XMLHttpRequest");

                        int last_story = story_id.Count - 1;
                        string request_string = 
                            $"reelMediaId={story_id[last_story]}&" +
                            $"reelMediaOwnerId={id}&" +
                            $"reelId={id}&" +
                            $"reelMediaTakenAt={story_time[last_story]}&" +
                            $"viewSeenAt={story_time[last_story]}";

                        html = http.Post($"https://www.instagram.com/stories/reel/seen", request_string, "application/x-www-form-urlencoded").ToString();
                        Thread.Sleep(delay);
                        check = true;
                    }
                    catch (HttpException)
                    {
                        StoryLabel.Invoke(new Action<string>((s) => StoryLabel.Text = s), "Пауза из-за активных действий 5 минут...");
                        Thread.Sleep(POST_WAIT_TIME);

                        continue;
                    }
                }
            }
            return html;
        }
    }
}
