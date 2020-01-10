using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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



namespace SocketChatClient

{

    /// <summary>

    /// Interaction logic for MainWindow.xaml

    /// </summary>

    public partial class MainWindow : Window
    {
        static int port = 3231;
        static string address = "127.0.0.1"; 

        public List<Message> Messages = new List<Message>();

        public MainWindow()
        {
            InitializeComponent();
            itemsControl.ItemsSource = Messages;            
        }

        private void SendMessageBtn(object sender, RoutedEventArgs e)
        {        
            SendMessage();
            RefreshData();
        }

        public void SendMessage()
        {         
            try
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ipPoint);

                var newMessage = new Message();
                newMessage.SendTime = DateTime.Now;
                newMessage.From = name_textBox.Text + ":";
                newMessage.Text = message_textBox.Text;

                var message = JsonConvert.SerializeObject(newMessage);

                byte[] data = Encoding.Unicode.GetBytes(message);
                socket.Send(data);

                data = new byte[256];
                StringBuilder builder = new StringBuilder();
                int bytes = 0;
                do
                {
                    bytes = socket.Receive(data, data.Length, 0);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (socket.Available > 0);

                var result = JsonConvert.DeserializeObject<List<Message>>(builder.ToString());
                Messages = result;

                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void RefreshData()
        {
            itemsControl.ItemsSource = null;
            itemsControl.ItemsSource = Messages;
        }
    }
}