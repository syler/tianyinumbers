using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace tianyi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<Obj> numbers = new ObservableCollection<Obj>();

        private readonly object locker = new object();

        private List<City> cities = new List<City>();
        private City selectedCity;
        private CollectionViewSource collectionViewSource;

        private int succeededCount = 0;
        private string header;
        private byte[] postBytes;

        public MainWindow()
        {
            InitializeComponent();
            this.collectionViewSource = this.FindResource("filteredNumbers") as CollectionViewSource;
            City defaultCity = new City("成都", "0128");
            this.cities.Add(defaultCity);
            this.cities.Add(new City("攀枝花", "0812"));
            this.cities.Add(new City("自贡", "0813"));
            this.cities.Add(new City("绵阳", "0816"));
            this.cities.Add(new City("南充", "0817"));
            this.cities.Add(new City("达州", "0818"));
            this.cities.Add(new City("遂宁", "0825"));
            this.cities.Add(new City("广安", "0826"));
            this.cities.Add(new City("巴中", "0827"));
            this.cities.Add(new City("泸州", "0830"));
            this.cities.Add(new City("宜宾", "0831"));
            this.cities.Add(new City("资阳", "0832"));
            this.cities.Add(new City("内江", "0132"));
            this.cities.Add(new City("乐山", "0833"));
            this.cities.Add(new City("眉山", "0133"));
            this.cities.Add(new City("凉山", "0834"));
            this.cities.Add(new City("雅安", "0835"));
            this.cities.Add(new City("甘孜", "0836"));
            this.cities.Add(new City("阿坝", "0837"));
            this.cities.Add(new City("德阳", "0838"));
            this.cities.Add(new City("广元", "0839"));

            this.SelectedCity = defaultCity;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Obj> Numbers
        {
            get
            {
                return numbers;
            }
        }

        public List<City> Cities
        {
            get
            {
                return this.cities;
            }
        }

        public City SelectedCity
        {
            get
            {
                return this.selectedCity;
            }

            set
            {
                this.selectedCity = value;
                this.RaisePropertyChanged("SelectedCity");
            }
        }

        public int SucceededCount
        {
            get
            {
                return this.succeededCount;
            }

            set
            {
                this.succeededCount = value;
                this.RaisePropertyChanged("SucceededCount");
            }
        }

        private void SearchNumberClick(object sender, RoutedEventArgs e)
        {
            string cityCode = this.SelectedCity.Code;
            string tail = string.Empty;
            string price = string.Empty;
            if (!string.IsNullOrEmpty(this.txtHeader.Text.Trim()))
            {
                this.header = this.txtHeader.Text.Trim();
            }

            this.postBytes = this.GetData();

            this.SucceededCount = 0;
            int maxCount = 1;
            int.TryParse(this.cbCount.Text.ToString(), out maxCount);

            for (int i = 0; i < maxCount; i++)
            {
                Task.Factory.StartNew(() =>
                {
                    HttpWebRequest request = HttpWebRequest.CreateHttp(new Uri("http://shop.sc.189.cn/searchNumOther", UriKind.Absolute));
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = this.postBytes.Length;
                    request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
                    request.Accept = "application/json, text/javascript, */*; q=0.01";
                    request.Headers.Add("Accept-Encoding", "gzip,deflate,sdch");
                    request.Headers.Add("Accept-Language", "en-US,en;q=0.8,zh-CN;q=0.6,zh;q=0.4,zh-TW;q=0.2");
                    request.Headers.Add("Origin", "http://shop.sc.189.cn");
                    request.Referer = "http://shop.sc.189.cn/searchNum?intid=wtdh-11";
                    request.Headers.Add("X-Requested-With", "XMLHttpRequest");

                    request.BeginGetRequestStream(this.BeginRequestStream, request);
                });
            }
        }

        private byte[] GetData()
        {
            string cityCode = this.SelectedCity.Code;
            string tail = string.Empty;
            string price = string.Empty;

            string postdata = string.Format("cityCode={0}&number={1}&price={2}&header={3}", cityCode, tail, price, this.header);
            byte[] postBytes = Encoding.UTF8.GetBytes(postdata);
            return postBytes;
        }

        private void BeginRequestStream(IAsyncResult result)
        {
            HttpWebRequest request = result.AsyncState as HttpWebRequest;
            using (Stream stream = request.EndGetRequestStream(result))
            {
                stream.Write(this.postBytes, 0, this.postBytes.Length);
                request.BeginGetResponse(this.BeginGetResponseStream, request);
            }
        }

        private void BeginGetResponseStream(IAsyncResult result)
        {
            StringBuilder sb = new StringBuilder();
            HttpWebRequest request = result.AsyncState as HttpWebRequest;

            using (Stream stream = request.EndGetResponse(result).GetResponseStream())
            {
                this.SucceededCount++;
                this.ParseStrem(stream);
            }
        }

        private void ParseStrem(Stream stream)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(RootObject));
            RootObject obj = serializer.ReadObject(stream) as RootObject;
            if (obj.isOk == "success")
            {
                foreach (var o in obj.obj)
                {
                    Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        lock (locker)
                        {
                            if (!this.numbers.Any(o2 => o2.number == o.number))
                            {
                                this.numbers.Add(o);

                                Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                                {
                                    this.Title = string.Format("完成次数：{0}，获得号码个数：{1}", this.SucceededCount, this.Numbers.Count);
                                }));
                            }
                        }
                    }));
                }
            }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            string keyword = this.txtKeyword.Text.Trim();
            if (string.IsNullOrEmpty(keyword))
            {
                e.Accepted = true;
            }
            else
            {
                Obj number = (Obj)e.Item;
                if (number.number.Contains(keyword))
                {
                    e.Accepted = true;
                }
                else
                {
                    e.Accepted = false;
                }
            }

        }

        private void KeywordChanged(object sender, TextChangedEventArgs e)
        {
            this.collectionViewSource.View.Refresh();
        }

        private void SaveResultClick(object sender, RoutedEventArgs e)
        {
            string filename = string.Format("{0}.syler", DateTime.Now.ToFileTime());
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Obj>));
            using (Stream stream = File.OpenWrite(filename))
            {
                serializer.WriteObject(stream, this.Numbers.Cast<Obj>().ToList());
            }
        }

        private void DropFiles(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                string firstSylerFile = null;
                foreach (string f in files)
                {
                    if (string.Compare(System.IO.Path.GetExtension(f), ".syler", true) == 0)
                    {
                        firstSylerFile = f;
                        break;
                    }
                }

                if (File.Exists(firstSylerFile))
                {
                    this.Numbers.Clear();
                    using (FileStream stream = File.OpenRead(firstSylerFile))
                    {
                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Obj>));
                        List<Obj> obj = serializer.ReadObject(stream) as List<Obj>;
                        foreach (Obj o in obj)
                        {
                            this.Numbers.Add(o);
                        }

                        this.Title = string.Format("完成次数：{0}，获得号码个数：{1}", this.SucceededCount, this.Numbers.Count);
                    }
                }
            }
        }

        private void ClearListClick(object sender, RoutedEventArgs e)
        {
            this.Numbers.Clear();
        }
    }
}
