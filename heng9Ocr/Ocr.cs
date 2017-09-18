using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ocr;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace heng9Ocr
{
    public partial class Ocr : Form
    {
        IAsyncResult ar;
        IAsyncResult arReader;
        Action a = null;
        QRreader qrReader = new QRreader();
        public bool isOpen;

        //初始化
        public Ocr() {
            InitializeComponent();
            qrReader.picBox = PicShow;
            qrReader.msgBox = MsgTxt;
            MsgTxt.Text = "正在打开摄像机，请稍后……";
            a = qrReader.OpenCam;
            isOpen = true;
            ar = a.BeginInvoke((ar) => { MsgTxt.Text = "已打开摄像机";}, null);
            Action autoOPenQR = AutoOPenReader;
            arReader =autoOPenQR.BeginInvoke((arReader)=> {MsgTxt.Text = "条形码检测开始";},null);
            ShowPosition();
        }
        //显示识别框位置
        private void ShowPosition() {
            x.Text = qrReader.setX.ToString() + "%";
            y.Text = qrReader.setY.ToString() + "%";
            w.Text = qrReader.setWidth.ToString() + "%";
            h.Text = qrReader.setHeight.ToString() + "%";
        }
        //窗口关闭，资源释放
        private void Ocr_FormClosed(object sender, FormClosedEventArgs e) {
            QRreader camClose = new QRreader();
            camClose.CloseCam();
            if (a != null) {
                a.EndInvoke(ar);
            }
        }
        //点击开启检测
        private void QRcode_Click(object sender, EventArgs e) {
            if (isOpen == true && qrReader.img != null) {
                qrReader.BeginReader();
                isOpen = false;
                qrReader.resultEvent += ResultShow;
                qrReader.proBarShowEvent += Barshow;
            }
            else if (isOpen == false) {
                MessageBox.Show("已经开始检测");
            }
            else if (isOpen == true && qrReader.img == null) {
                MessageBox.Show("请稍等片刻，摄像机正在打开中");
            }
        }
        //进度条
        private void Barshow(int angle) {
            PB.Value = angle;
        }
        //结果显示
        public void ResultShow() {
            string str = "";
            foreach (var listItem in qrReader.qrCodeStr) {
                str += "-------- result -------\r\n";
                foreach (var item in listItem) {
                    str += item + "\r\n";
                }
            }
            TxtShow.Text = str ;
            resultCount.Text = qrReader.qrCodeStr.Count+"";
            //labelTime.Text = "用时："+qrReader.timers.TotalMilliseconds/1000+"s";
        }
        //过程运行时间显示
        public void ResultShow2() {
            labelTime.Text = "用时：" + qrReader.timers.TotalMilliseconds / 1000 + "s";
        }
        //退出菜单
        private void InputPic_Click(object sender, EventArgs e) {
            qrReader.OpenFile();
        }
        //关闭摄像机
        private void closeCam_Click(object sender, EventArgs e) {
            qrReader.CloseCam();
        }
        //图片自适应
        private void tableLayoutPanel1_SizeChanged(object sender, EventArgs e) {
            PicShow.Width = tableLayoutPanel1.Width;
            PicShow.Height = tableLayoutPanel1.Height;
        }
        //文本框自适应
        private void panel1_SizeChanged(object sender, EventArgs e) {
            panel1.Width = 189;
            TxtShow.Height = panel1.Height;
            TxtShow.Width = panel1.Width;
        }
        //提示栏自适应
        private void tableLayoutPanel2_SizeChanged(object sender, EventArgs e) {
            MsgTxt.Width = tableLayoutPanel2.Width;
            MsgTxt.Height = tableLayoutPanel2.Height;
        }
        //文本框清除
        private void clearText_Click(object sender, EventArgs e) {
            TxtShow.Text = "";
        }
        //检测开启
        private void Open() {
            if (isOpen == true && qrReader.img != null) {
                qrReader.BeginReader();
                isOpen = false;
                qrReader.resultEvent += ResultShow;
                qrReader.proBarShowEvent += Barshow;
                qrReader.usedTime += ResultShow2;
            }
            else if (isOpen == false) {
                MessageBox.Show("已经开始检测");
            }
            else if (isOpen == true && qrReader.img == null) {
                MessageBox.Show("请稍等片刻，摄像机正在打开中");
            }
        }
        //自动开启 检测，在之前检查图像时候到位，防止出错处理
        private void AutoOPenReader() {
            while (true) {
                if (qrReader.img!=null && qrReader.imgQR!=null) {
                    qrReader.PaintKK();
                    this.Open();
                    break;
                }
            }
        }
        //识别框设置 确定 设置区域，并刷新
        private void button1_Click(object sender, EventArgs e) {
            if (x.Text!=null&&x.Text!=""&& y.Text != null && y.Text != ""&& w.Text != null && w.Text != ""&& h.Text != null && h.Text != "") {
                Regex reg = new Regex(@"\d+");
                qrReader.setX = Int32.Parse(reg.Match(x.Text).Value);
                qrReader.setY = Int32.Parse(reg.Match(y.Text).Value);
                qrReader.setWidth = Int32.Parse(reg.Match(w.Text).Value);
                qrReader.setHeight = Int32.Parse(reg.Match(h.Text).Value);
                ShowPosition();
            }
            else {
                MessageBox.Show("设置的值不能为空");
                qrReader.setX = 20;
                qrReader.setY = 20;
                qrReader.setWidth = 50;
                qrReader.setHeight = 50;
            }
            qrReader.PaintKK();
        }
        private void Ocr_Load(object sender, EventArgs e) {
        }
        //图像框大小改变时，识别框自适应
        private void PicShow_SizeChanged(object sender, EventArgs e) {
            qrReader.PaintKK();
        }
        //重置
        private void reset_Click(object sender, EventArgs e) {
            qrReader.setX = 25;
            qrReader.setY = 15;
            qrReader.setWidth = 50;
            qrReader.setHeight = 50;
            qrReader.PaintKK();
            ShowPosition();
        }
        //清除缓存
        private void reReader_Click(object sender, EventArgs e) {
            qrReader.qrCodeStr = new List<List<string>>();
            resultCount.Text = "0";
        }
        //重启摄像机
        private void ReStratCam() {
            MsgTxt.Text = "正在重启摄像机";
            a = qrReader.OpenCam;
            isOpen = true;
            ar = a.BeginInvoke((ar) => { MsgTxt.Text = "已重启摄像机"; }, null);
        }
        //最大分辨率 0
        private void Max_Click(object sender, EventArgs e) {
            qrReader.cam.Stop();
            qrReader.resolution = 0;
            ReStratCam();
        }
        //分辨率1
        private void largish_Click(object sender, EventArgs e) {
            qrReader.cam.Stop();
            qrReader.resolution = 1;
            ReStratCam();
        }
        //分辨率2
        private void medel_Click(object sender, EventArgs e) {
            qrReader.cam.Stop();
            qrReader.resolution = 2;
            ReStratCam();
        }
        //分辨率3
        private void aLittle_Click(object sender, EventArgs e) {
            qrReader.cam.Stop();
            qrReader.resolution = 3;
            ReStratCam();
        }
        //分辨率4
        private void small_Click(object sender, EventArgs e) {
            qrReader.cam.Stop();
            qrReader.resolution = 4;
            ReStratCam();
        }
    }     
}
