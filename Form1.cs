using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Web.Script.Serialization;
using System.Runtime.Serialization.Json;
using System.Net;
using Newtonsoft.Json;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.Json;





namespace Astrometry
{
   

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
        }
        //public class Session
        //{
        //    public string session { get; set; }
        //}
        public class TodoItem2
        {
            public int Id { get; set; }

          public string Text { get; set; }

         // public bool Complete { get; set; }
        }    

        private string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
        //timer 1 is sample interval fixed at 55 sec

        private void Log(string logtext)
        {
            string fullpath = path + "\\astrometrylog.txt";
            StreamWriter log;
            if (!File.Exists(fullpath))
            {
                log = new StreamWriter(fullpath);
            }
            else
            {
                log = File.AppendText(fullpath);
            }

            //  log.WriteLine(DateTime.Now);
            //  log.WriteLine("scopefocus - Error");
            log.WriteLine(logtext);
            log.Close();
            return;
        }


        public class jsonData
        {
            public List<Elements> data { get; set; }

    
        }
        public class Elements
        {
            public string status { get; set; }
        //    public string message { get; set; }
            public string session { get; set; }

        }

        public static List<jsonData> Convert(string json)
        {
            return JsonConvert.DeserializeObject<List<jsonData>>(json);
        }

        // try this....
        private T UnpackJson<T>(object data)
        {
            var serializer = new JavaScriptSerializer();
            string rawJSON = serializer.Serialize(data);

            var result = serializer.Deserialize<T>(rawJSON);
            return result;
        }

        //static JsonObject getLoginJSON(String s)
        //{
        //    JavaScriptSerializer js = new JavaScriptSerializer();
        //    JsonObject obj = new JsonObject();
        //    obj.Serialize("apikey", s);
        //    obj.put("apikey", s);
        //    return obj;
        //}
        internal static JsonObject getLoginJSON(string s)
        {
            JsonObject obj = new JsonObject();
            //	obj.put("apikey", s);
            obj["apikey"] = s;
            return obj;
        }

        internal static JsonObject createJson(string id, string value)
        {
            JsonObject obj = new JsonObject();
            obj[id] = value;
            return obj;
        }
        string apikey = "ckylhfaafccqhumf";
        string session;  //rem'd s odont have to use getSession, when done remove session below w/ value
        private async void GetSession (string apikey)
        {
             //eventaully add to settings
            string URI = "http://nova.astrometry.net/api/login";
            // string myParameters = "request-json=%7B%22apikey%22%3A+%22ckylhfaafccqhumf%22%7D";
            JsonObject obj = new JsonObject();
            JsonObject jsonObject = getLoginJSON(apikey);
            String input = "&request-json=" + jsonObject.ToString();
            //using (WebClient wc = new WebClient())
            //{
            //    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            //    //   string HtmlResult = wc.UploadString(URI, myParameters);
            //    string HtmlResult = wc.UploadString(URI, input);
            //    //   richTextBox1.Text = HtmlResult;
            //    var serializer = new JavaScriptSerializer();
            //    //   var result = serializer.DeserializeObject(HtmlResult);
            //    var result2 = serializer.Deserialize<Dictionary<string, string>>(HtmlResult);  //e.g. http://procbits.com/2011/04/21/quick-json-serializationdeserialization-in-c
            //    string session = result2["session"];
            //    return session;
            //}

            string baseAddress = "http://nova.astrometry.net/api/login";
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpContent contentPost = new StringContent(input, Encoding.UTF8, "application/x-www-form-urlencoded");
                using (var response = await httpClient.PostAsync(baseAddress, contentPost))
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    var serializer = new JavaScriptSerializer();
                    var result2 = serializer.Deserialize<Dictionary<string, string>>(responseData);  //e.g. http://procbits.com/2011/04/21/quick-json-serializationdeserialization-in-c
                    session = result2["session"];
                    richTextBox1.Text = session;
                }
            }


            }

        internal static JsonObject getUploadJson(string session)
        {
            JsonObject obj = new JsonObject();
            obj["publicly_visible"] = "y";
            obj["allow_modifications"] ="d";
            obj["session"] = session;
            obj["allow_commercial_use"] = "d";
            return obj;
        }

     //   string session = "u7wk3z21yz40ku9yg6hnlqx67sx8pdgm";
        string subid;
        private async void PostFile(string session, string filename)
        {
            string URI = "http://nova.astrometry.net/api/upload";
            JsonObject obj = new JsonObject();
            JsonObject jsonObject = getUploadJson(session);
            String input =  jsonObject.ToString();  // was &json-request...
            string baseAddress = "http://nova.astrometry.net/api/upload";
            using (var httpClient = new HttpClient())
            {
                using (var content = new MultipartFormDataContent())
                {

                    var fileContent = new ByteArrayContent(File.ReadAllBytes(filename));
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                 //   httpClient.DefaultRequestHeaders.TryAddWithoutValidation("file", "\"file\"");
                    content.Add(new StringContent(jsonObject.ToString(), Encoding.UTF8, "text/plain"),  "&request-json");//wasd   "application/x-www-form-urlencoded"
                    var contentFile = new StringContent("filename=" + filename, Encoding.UTF8, "application/octet-strem"); content.Add(contentFile, "file" );

                    using (var response = await httpClient.PostAsync(baseAddress, content))
                        {
                        richTextBox1.Text = response.ToString();
                        if (response.IsSuccessStatusCode)
                            richTextBox1.AppendText("sucess!");
                        string responseData = await response.Content.ReadAsStringAsync();
                        richTextBox1.AppendText(responseData.ToString());
                    }
                }
                    
               
            }






            //String input = "&request-json=" + jsonObject.ToString();
            //using (WebClient wc = new WebClient())
            //{
            //    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            //    //   string HtmlResult = wc.UploadString(URI, myParameters);
            //    string HtmlResult = wc.UploadString(URI, input);
            //    //   richTextBox1.Text = HtmlResult;
            //    var serializer = new JavaScriptSerializer();
            //    //   var result = serializer.DeserializeObject(HtmlResult);
            //    var result = serializer.Deserialize<Dictionary<string, string>>(HtmlResult);

            //    return subid;
            //}
        }


        private async void button1_Click(object sender, EventArgs e)
        {
            GetSession(apikey);
            richTextBox1.Text = session;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            PostFile(session, "M81M82.fit");
        }

        //private void button2_Click(object sender, EventArgs e)
        //{
        //    ScriptEngine engine = Python.CreateEngine();
        //    engine.ExecuteFile(@"C:\Users\ksipp_000\Documents\GitHub\Astrometry\astrometry.net\net\client\client.py");



        //    //var ipy = Python.CreateRuntime();

        //    //dynamic test = ipy.UseFile("client.py");
        //    //test.Client();
        //    //test.login("ckylhfaafccqhumf");
        //    ////   test.url_upload('http://apod.nasa.gov/apod/image/1408/lagooncenter_hstschmidt_960.jpg');
        //}

    }
        
}
