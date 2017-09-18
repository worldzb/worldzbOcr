using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ZXing;
using ZXing.Common;
using ZXing.Multi.QrCode;

namespace test
{
    public partial class Form1 : Form
    {
        public Bitmap bt;
        public Form1() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            
        }

        private void pictureBox1_Click(object sender, EventArgs e) {
            OpenFileDialog ofg = new OpenFileDialog();
            ofg.Filter = "Image(*.JPG;*.PNG;*.GIF)|*.JPG;*.PNG;*.GIF";
            if (ofg.ShowDialog()==DialogResult.OK) {
                pictureBox1.Image = Bitmap.FromFile(ofg.FileName);
                bt=(Bitmap) Bitmap.FromFile(ofg.FileName);
            }
        }
        

        private void button2_Click(object sender, EventArgs e) {

            //SusanCornersDetector scd = new SusanCornersDetector();
            /***************
            List<IntPoint> corners = new List<IntPoint>();
            corners.Add(new IntPoint(99, 99));
            corners.Add(new IntPoint(156, 79));
            corners.Add(new IntPoint(184, 126));
            corners.Add(new IntPoint(122, 150)); 

            // create filter
            QuadrilateralTransformation filter =new QuadrilateralTransformation(corners, 200, 200);
            *************/
            // OilPainting filter = new OilPainting(30);
            //RotateBilinear filter = new RotateBilinear(30, true);
            // apply the filter
            RotateBicubic filter = new RotateBicubic(30, true);
            pictureBox1.Image= filter.Apply(bt);

        }

        private BinaryBitmap BitmapToByteArray(Bitmap image) {
            MemoryStream ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            byte[] imgByte = ms.GetBuffer();
            ms.Close();
            LuminanceSource ls = new RGBLuminanceSource(imgByte, image.Width, image.Height);
            BinaryBitmap bb = new BinaryBitmap(new HybridBinarizer(ls));
            return bb;
        }
        public List<string> QRcodeMultiReader(Bitmap img) {
            List<string> strQR = null;
            var multiReader = new QRCodeMultiReader();
            //Hashtable hints = new Hashtable();
            Result[] res = multiReader.decodeMultiple(BitmapToByteArray(img));


            if (res != null) {
                foreach (Result str in res) {
                    strQR.Add(str.Text);
                    textBox2.Text += ("//" + str.Text);
                }
            }
            else {
                MessageBox.Show("获取失败");
            }
            return strQR;
        }
    }
}
