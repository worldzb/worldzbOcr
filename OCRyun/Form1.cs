using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;

using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Xml;

namespace OCRyun
{
    public partial class Form1 : Form
    {

        string m_ocrdatapath = "";//ocrdata路径
        string m_ocrconfigpath = "";//config路径
        const string OCRDATA = "\\ocr_data";
        const string OCRCONFIG = "\\ScanBcr.cfg";
        string m_tmpOcrImagepath = "";
        string m_UploadImagePath = "";
        // private List<string> m_filenamelist;//文件夹下的图片路径
        private int m_cardId = 4;//识别种类 默认身份证{0:身份证 1:车牌 2:驾驶证 3:行驶证 4:名片 5:护照 6:银行卡 7:文档}
        private string[] filetypeList = new string[] { "*.jpg", "*.jpeg", "*.png", "*.bmp", "*.tif" };
        private bool m_IsOCRRight = true;//控制上传识别变量，上传未返回结果，操作暂时停止

        // IntPtr m_intPScan0Former = IntPtr.Zero;
        // StringBuilder m_sbresult = new StringBuilder(8291);//识别结果。

        static string m_api = "http://www.yunmaiocr.com/SrvXMLAPI";
        static string m_t = "PC";
        static string m_key = "";
        private string m_action = "namecard.scan";
        static string m_time = "";
        static string m_loginid = "账号";
        static string m_loginpwd = "密码";
        static string m_ocrLang = "2";
        static string m_keyLang = "0";

        //压缩图片
        public static bool GetPicThumbnail(string sFile, string outPath, int flag) {
            System.Drawing.Image iSource = System.Drawing.Image.FromFile(sFile);
            ImageFormat tFormat = iSource.RawFormat;
            //以下代码为保存图片时，设置压缩质量  
            EncoderParameters ep = new EncoderParameters();
            long[] qy = new long[1];
            qy[0] = flag;//设置压缩的比例1-100  
            EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
            ep.Param[0] = eParam;
            try {
                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo jpegICIinfo = null;
                for (int x = 0; x < arrayICI.Length; x++) {
                    if (arrayICI[x].FormatDescription.Equals("JPEG")) {
                        jpegICIinfo = arrayICI[x];
                        break;
                    }
                }
                if (jpegICIinfo != null) {
                    iSource.Save(outPath, jpegICIinfo, ep);//dFile是压缩后的新路径  
                }
                else {
                    iSource.Save(outPath, tFormat);
                }
                return true;
            }
            catch {
                return false;
            }
            finally {
                iSource.Dispose();
                iSource.Dispose();
            }
        }


        public void PostImage(string filepath) {
            m_IsOCRRight = false;
            WebClient webclient = new WebClient();
            webclient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            webclient.UploadDataCompleted += new UploadDataCompletedEventHandler(webclient_UploadDataCompleted);
            DateTime oldTime = new DateTime(1970, 1, 1);
            TimeSpan span = DateTime.Now.Subtract(oldTime);
            long milliSecondsTime = (long)span.TotalMilliseconds;
            m_time = milliSecondsTime.ToString();
            m_key = Guid.NewGuid().ToString();
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] md5buf = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(m_action + m_loginid + m_key + m_time + m_loginpwd));
            string verifystr = BitConverter.ToString(md5buf);//System.Text.Encoding.UTF8.GetString(md5buf);
            verifystr = verifystr.Replace("-", "");
            byte[] md5pwd = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(m_loginpwd));
            string password = BitConverter.ToString(md5pwd);
            password = password.Replace("-", "");
            string filename = Guid.NewGuid().ToString() + ".jpg";

            FileStream pFileStream = null;
            byte[] pfilebuf = new byte[0];
            try {
                pFileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
                BinaryReader r = new BinaryReader(pFileStream);
                r.BaseStream.Seek(0, SeekOrigin.Begin);    //将文件指针设置到文件开
                pfilebuf = r.ReadBytes((int)r.BaseStream.Length);
            }
            catch {
            }
            finally {
                if (pFileStream != null)
                    pFileStream.Close();
            }
            int index = 0;
            string poststr = "";
            poststr = String.Format("<action>{0}</action><client>{1}</client><system>{2}</system><password>{3}</password><key>{4}</key><time>{5}</time><verify>{6}</verify><ocrlang>{7}</ocrlang><keylang>{8}</keylang><file>",
                m_action, m_loginid, m_t, password, m_key, m_time, verifystr, m_ocrLang, m_keyLang, filename);
            byte[] firstbuf = System.Text.Encoding.UTF8.GetBytes(poststr);

            byte[] endbuf = System.Text.Encoding.UTF8.GetBytes("</file><ext>jpg</ext>");

            byte[] postbuf = new byte[firstbuf.Length + pfilebuf.Length + endbuf.Length + 1];

            firstbuf.CopyTo(postbuf, index);
            index += firstbuf.Length;
            pfilebuf.CopyTo(postbuf, index);
            index += pfilebuf.Length;
            endbuf.CopyTo(postbuf, index);
            webclient.UploadDataAsync(new Uri(m_api), "POST", postbuf);
        }


        void ResolveXml(string xmlstring, ref string outbuf, int cardId) {
            XmlDocument m_xml = new XmlDocument();
            m_xml.LoadXml(xmlstring);
            XmlNode m_xmlNode = null;
            if (m_cardId == 6)
                m_xmlNode = m_xml.SelectSingleNode("result");
            else
                m_xmlNode = m_xml.SelectSingleNode("data");
            XmlNodeList m_nodeList = m_xmlNode.ChildNodes;

            if (m_xmlNode != null) {
                foreach (XmlNode xnf1 in m_nodeList) {
                    if (xnf1.Name.IndexOf("er") == 0 || xnf1.Name.IndexOf("dw") == 0) { continue; }
                    if (xnf1.InnerText != "") {
                        outbuf += xnf1.Name + "   :";
                        outbuf += xnf1.InnerText;
                        outbuf += "\r\n";
                    }
                }
            }
            Console.Write(outbuf);
        }
        void webclient_UploadDataCompleted(object sender, UploadDataCompletedEventArgs e) {
            string returnMessage = "";
            string result = "";
            string outbuf = "";
            int ret = 0;
            if (e.Error == null) {
                returnMessage = Encoding.UTF8.GetString(e.Result);
                if (m_cardId == 6) {
                    result = returnMessage.Substring(16, 1);
                }
                else
                    result = returnMessage.Substring(8, 1);

                textBox1.Text = returnMessage;
                Console.WriteLine(returnMessage);


                m_IsOCRRight = true;
                return;
                ret = Int32.Parse(result);
                if (1 == ret) {
                    if (m_cardId == 6) {
                        result = returnMessage.Remove(8, 18);
                        result = result.Replace("\0", "  ");
                    }
                    else if (m_cardId == 4) {
                        result = returnMessage.Substring(18);
                        result = result.Replace("<f>", "");
                        result = result.Replace("</f>", "");
                        result = result.Replace("<n>", "");
                        result = result.Replace("</n>", ": ");
                        result = result.Replace("<v>", "");
                        result = result.Replace("</v>", "\r\n");
                    }
                    else
                        result = returnMessage.Substring(18);

                    if (m_cardId == 7 || m_cardId == 4) {
                        if (result != "") {
                            result = result.Replace("<data>", "");
                            result = result.Replace("</data>", "");
                            outbuf = result;
                        }
                    }
                    else
                        ResolveXml(result, ref outbuf, m_cardId);

                    if (outbuf != "")
                        textBox1.Text = outbuf;
                    else
                        textBox1.Text = "识别异常";
                }
                else {
                    textBox1.Text = "识别失败";
                }
            }
            else {
                textBox1.Text = "网络错误,请查看网络是否连接正常";
                Console.WriteLine(e.Error);
            }
            m_IsOCRRight = true;
        }

        public Form1() {
            InitializeComponent();
            m_ocrdatapath = System.Windows.Forms.Application.StartupPath.ToString() + OCRDATA;//取目录下
            m_ocrconfigpath = System.Windows.Forms.Application.StartupPath.ToString() + OCRCONFIG;//取目录下
            m_tmpOcrImagepath = System.Windows.Forms.Application.StartupPath.ToString() + "\\temp.jpg";

            // m_filenamelist =new List<string>();
            m_UploadImagePath = System.Windows.Forms.Application.StartupPath.ToString() + "\\uploadimage\\";
            if (!Directory.Exists(m_UploadImagePath))//如果不存在就创建file文件夹
            {
                Directory.CreateDirectory(m_UploadImagePath);//创建该文件夹
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            System.Environment.Exit(0);
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }


        private void pictureBox1_Click(object sender, EventArgs e) {
            OpenFileDialog openfile = new OpenFileDialog();
            openfile.Filter = "jpg图片(*.jpg)|*.jpg|bmp图片(*.bmp)|*.bmp|tif图片(*.tiff)|*.tiff|png图片(*.png)|*.png";
            openfile.RestoreDirectory = true;
            openfile.FilterIndex = 1;

            if (openfile.ShowDialog() == DialogResult.OK) {
                if (!m_IsOCRRight) {
                    this.textBox1.Text = "正在识别中，请稍后识别下一张！";
                    return;
                }
                this.textBox1.Text = "正在上传识别，请稍等...";


                this.pictureBox1.Load(openfile.FileName);

                try {
                    m_UploadImagePath = System.Windows.Forms.Application.StartupPath.ToString() + "\\uploadimage\\";
                    m_UploadImagePath += DateTime.Now.ToString("yyyy-MMdd-hh-mm-ss");
                    m_UploadImagePath += ".jpg";
                    GetPicThumbnail(openfile.FileName, m_UploadImagePath, 30);
                    PostImage(m_UploadImagePath);
                }
                catch { }
            }
        }
    }
}



