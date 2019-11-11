using NewLife.Log;
using NewLife.RocketMQ;
using NewLife.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace rocketMqDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                Thread t = new Thread(testConsumer);
                t.Start();
                
                
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void testConsumer()
        {
            try
            {
                var consumer = new Consumer
                {


                    Topic = txtTopic.Text,
                    Group = txtGroup.Text,
                    NameServerAddress = txtIP.Text,

                    FromLastOffset = false,
                    BatchSize = 20,
                    Log = XTrace.Log,
                };

                consumer.OnConsume = (q, ms) =>
                {
                    log(string.Format("[{0}@{1}]收到消息[{2}]", q.BrokerName, q.QueueId, ms.Length));

                    foreach (var item in ms.ToList())
                    {
                        log(string.Format($"消息：主键【{item.Keys}】，产生时间【{item.BornTimestamp.ToDateTime()}】，") + "内容【" + item.Body.ToStr() + "】");
                    }
                    return true;
                };

                consumer.Start();
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void testProduct()
        {
            try
            {
                var mq = new Producer
                {


                    Topic = txtTopic.Text,
                    NameServerAddress = txtIP.Text,
                    Group = txtGroup.Text,
                    Log = XTrace.Log,
                };


                mq.Start();


                var msg = new Msg();
                msg.id = 1234;
                msg.type = "order";
                msg.content = txtMsg.Text;
                mq.Publish(txtMsg.Text, "tag1");
                mq.Dispose();
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void log(string msg)
        {
            this.Invoke(new Action(() => {
                this.richTextBox1.AppendText(string.Format("时间:{0}\r\n内容:{1}\r\n", 
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), msg));
            }));
        }
        public class Msg
        {
            public int id { get; set; }
            public string type { get; set; }
            public string content { get; set; }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            try
            {
                Thread t = new Thread(testProduct);
                t.Start();
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
