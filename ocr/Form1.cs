using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ocr
{
    public partial class Form1 : Form
    {
        IdentityOcr io = new IdentityOcr();

        public Form1()
        {
            InitializeComponent();
            io.picBox = picShow;
            //io.txtBox = taxtA;
            //io.msgBox = taxtB;
            io.lang = OcrLanguage.chi;
        }

        private void Callback() {
            Identity id = io.identityResult;
            if (id == null) {
                MessageBox.Show("请选择打开图片");
                return;
            }
            taxtA.Text = id.ToString();
            taxtB.Text = id.name;
            textBox1.Text = id.gender;
            textBox2.Text = id.nation;
            textBox3.Text = id.date;
            textBox4.Text = id.address;
            textBox5.Text = id.idNumber;
            textBox6.Text = id.companyName;
            textBox7.Text = id.companyAddress;
            textBox8.Text = id.name;
            textBox9.Text = id.position;
            textBox10.Text = id.tel;
            textBox11.Text = id.phoneNumber;
            textBox12.Text = id.email;
            textBox13.Text = id.sites;
            textBox14.Text = id.qq;

            if (id.idNumber != null && id.idNumber != "") {
                label9.BackColor = Color.Red;
            }
            else if (id.companyName != null && id.companyName != "") {
                label10.BackColor = Color.Red;
            }
            else {
                taxtA.Text = "/(ㄒoㄒ)/~~  \r\n未能做出很好的识别";
            }
            label19.Text = "width: " + io.img.Width + "   height: " + io.img.Height;
        }

        private void changeBtn_Click(object sender, EventArgs e)
        {
            label9.BackColor = Color.White;
            label10.BackColor = Color.White;
            if (io.img==null) {
                taxtA.Text = "请先选择要识别的照片";
                return;
            }
            io.receiveEvent += Callback;
            io.GetIdentity(io.img);
            label19.Text=taxtA.Text = "正在识别中……";
            

        }
        private void picShow_Click(object sender, EventArgs e)
        {
            Bitmap img=null;
            OpenFileDialog openf = new OpenFileDialog();
            openf.Title = "select a picture";
            openf.Filter = "Images(*.BMP;*.JPG;*.PNG;*.GIF;*.TIF;*.TIFF)|*.BMP;*.JPG;*.PNG;*.tif;*.tiff";
            if (openf.ShowDialog() == DialogResult.OK)
            {
                img = (Bitmap)Bitmap.FromFile(openf.FileName);
               // io.ExtractTextFromImage2(openf.FileName);
                if (img != null)
                {
                    picShow.BackgroundImage = img;
                    io.img = (Bitmap)img.Clone();   //防止发生位图锁定的错误
                }
            }
            if (io.img!=null) {
                label19.Text = "width: " + io.img.Width + "   height: " + io.img.Height;
            }
        }

        private void test_Click(object sender, EventArgs e)
        {
            io.receiveEvent += Callback;
            io.GetIdentity(io.img);
            label19.Text = taxtA.Text = "正在识别中……";
        }

        private void camOpen_Click(object sender, EventArgs e)
        {
            io.OpenCam();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            io.CloseCam();
        }
        private void Binaryzation_Click(object sender, EventArgs e) {
            PicTools b = new PicTools();
            picShow.BackgroundImage= b.Binaryzation((Bitmap)picShow.BackgroundImage);

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {

        }

        private void tableLayoutPanel1_SizeChanged(object sender, EventArgs e) {
            PictureBox a = picShow;
            a.Width = tableLayoutPanel1.Width;
            a.Height = tableLayoutPanel1.Height;
        }

        private void langEnglish_Click(object sender, EventArgs e) {
            io.lang = OcrLanguage.eng;
        }

        private void langChi_Click(object sender, EventArgs e) {
            io.lang = OcrLanguage.chi;
        }
    }
}
