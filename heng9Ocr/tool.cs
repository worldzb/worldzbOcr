using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Collections;
using System.Threading;

using AForge;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Imaging;
using AForge.Imaging.Filters;

using ZXing.QrCode;
using ZXing;
using ZXing.Common;
using ZXing.Rendering;
using System.IO;
using System.Runtime.InteropServices;
using ZXing.Maxicode;
using ZXing.Multi.QrCode;
using ZXing.Multi.QrCode.Internal;
using System.Diagnostics;

using ocr;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;

namespace heng9Ocr
{
    /// <summary>
    /// 摄像机条形码识别
    /// </summary>
    class QRreader
    {
        public Bitmap img;        //导入的图像
        public Bitmap imgQR;       //用来识别的图像
        public Bitmap imgSave;      //要保存的图像
        public PictureBox picBox;   //显示图像控件
        public TextBox msgBox;      //消息显示文本框
        public int sleepTime;     //图像采集频率 1000 时为每秒一帧            
        private FilterInfoCollection videoDevice;    //摄像机设备
        public VideoCaptureDevice cam;       //摄像机
        public bool isFnh;                    //摄像机打开状态   
        public List<List<string>> qrCodeStr;                     //条形码识别后的字符串列表
        public string tempStr;                        //当前的识别字符串
        private ResultPoint[] resList;                //条形码坐标点
        private float width,height;                     //图片的宽高
        private float corsor=0;                      //未识别时间指针
        public delegate void resultManage();       //委托
        public event resultManage resultEvent;     //识别事件
        public event resultManage usedTime;
        public delegate void ProBar(int num);    //进度条显示委托
        public event ProBar proBarShowEvent;  //进度条值改变 
        public TimeSpan timers;           //用时时长 
        public int setX, setY, setWidth, setHeight;
        public int SetX {
            get {
                return setX;
            }
            set {
                if (value > 100) {
                    setX = 100;
                    MessageBox.Show("请输入0-100之间的数字");
                }
                else if(value <0) {
                    setX = 0;
                }
                else {
                    setX = value;
                }
            }
        }
        public int SetY {
            get
            {
                return setY;
            }
            set
            {
                if (value > 100) {
                    setY = 100;
                    MessageBox.Show("请输入0-100之间的数字");
                }
                else if (value < 0) {
                    setY = 0;
                }
                else {
                    setY= value;
                }
            }
        }                
        public int SetWidth {
            get {
                return setWidth;
            }
            set {
                if (value > 100) {
                    setWidth = 100;
                    MessageBox.Show("请输入0-100之间的数字");
                }
                else if (value < 0) {
                    setWidth = 0;
                }
                else {
                    setWidth = value;
                }
            }
        }
        public int SetHeight {
            get
            {
                return setWidth;
            }
            set
            {
                if (value > 100) {
                    setHeight = 100;
                    MessageBox.Show("请输入0-100之间的数字");
                }
                else if (value < 0) {
                    setHeight = 0;
                }
                else {
                    setWidth = value;
                }
            }
        }
        private int beginAngle;
        public List<List<string>> listPk;
        public List<string> maxList;
        private int sw, sh, sx, sy;
        public int resolution;


        //初始化
        public QRreader() {
            sleepTime = 3000;
            msgBox = null;
            cam = null;
            isFnh = true;
            qrCodeStr = new List<List<string>>();
            setX = 25;
            setY = 15;
            setWidth = 50;
            setHeight = 50;
            listPk = new List<List<string>>();
            resolution = 2;
        }

        //打开摄像机 并将摄像机内容显示在picturebox中
        public void OpenCam()
        {
            setHeight = 50;
            try
            {
                videoDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                //videoDevice 是一个filterInfo数组
               // MessageBox.Show("检测到"+videoDevice.Count.ToString()+"个摄像机设备，默认打开外置");
                if (videoDevice.Count > 0)
                {
                    cam = new VideoCaptureDevice(videoDevice[0].MonikerString);
                    cam.VideoResolution = cam.VideoCapabilities[resolution];
                    cam.Start();
                    //cam.newFrame 是一个newFrameEcentHandler 委托类型
                    //使用Lambda表达式 来写匿名方法   
                    //此方法用来控制图片刷新
                    cam.NewFrame += new NewFrameEventHandler((object obj, NewFrameEventArgs eventArg) =>
                    {
                        img = (Bitmap)eventArg.Frame.Clone();
                        imgSave = (Bitmap)img.Clone();
                        Bitmap imgShow = img;
                        //赋值图像大小
                        width = imgShow.Width;
                        height = imgShow.Height;
                        Bitmap cb = new Bitmap(img,(int)picBox.Width,(int)picBox.Height);
                        picBox.BackgroundImage = cb;
                        sx = (int)(setX * (width / 100.0f));
                        sy = (int)(setY * (width / 100.0f));
                        sw = (int)(setWidth * (width / 100.0f));
                        sh = (int)(setHeight * (height / 100.0f));
                        img = ImgCut(sx, sy, sw, sh, img);
                        imgQR = (Bitmap)img.Clone();
                        isFnh = true;
                        System.GC.Collect();
                    });
                }
                else{
                    MessageBox.Show("没有摄像机或者打开摄像机出现异常");
                    throw new Exception();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("打开摄像机发生异常，请检查摄像机设备" + e.Message);
            }
        }
        //图像截取并判别
        //一秒检测1帧
        private void PicCut()
        {
            while (true)
            {
                //Thread.Sleep(500);
                List<string> strlist;
                Bitmap inputImg = imgQR;
                Bitmap beSaveImg =(Bitmap) imgSave.Clone();
                strlist = Reader3(inputImg);
                /*****
                if (beginAngle != 3) {
                    listPk.Add(strlist);
                }
                else {
                    maxList = new List<string>();
                    foreach (var ml in listPk) {
                        foreach (var str in ml) {
                            bool isAdd = true;
                            foreach (var item in maxList) {
                                if (str==item) {
                                    isAdd = false;
                                    break;
                                }
                                else {
                                    isAdd = true;
                                }
                            }
                            if (isAdd) {
                                maxList.Add(str);
                            }
                        }
                    }
                    if (resultContrastEvent!=null) {
                        resultContrastEvent();
                    }
                    listPk = new List<List<string>>();
                }
                ****/
                corsor = 0;                     //将指针归零 
                if (strlist != null && strlist.Count>0)
                {
                    tempStr = "";
                    foreach (string s in strlist)
                    {
                        tempStr += (s + "\r\n");
                    }
                    if (msgBox!=null) {
                        msgBox.Text = tempStr;
                    }
                    if (qrCodeStr.Count>0)
                    {
                        bool isAddList = true;
                        List<string> tempL = new List<string>();
                        foreach (var str in strlist) {
                            bool isAdd = true;
                            foreach (var str2 in qrCodeStr[qrCodeStr.Count-1]) {
                                if (str == str2) {
                                    isAddList = false;
                                    isAdd = false;
                                    break;
                                }
                                else {
                                    isAdd = true;
                                }
                            }
                            if (isAdd) {
                                tempL.Add(str);
                            }
                        }
                        if (isAddList) {
                            qrCodeStr.Add(strlist);
                            AutoSavePic(beSaveImg);
                            resultEvent();   //触发事件
                        }
                        else {
                            foreach (var str3 in tempL) {
                                qrCodeStr[qrCodeStr.Count - 1].Add(str3);
                                resultEvent();   //触发事件
                            }
                        }
                    }
                    else {
                        qrCodeStr.Add(strlist);
                        AutoSavePic(beSaveImg); 
                        resultEvent();   //触发事件
                    }
                }
               // Thread.Sleep(sleepTime);
            }
        }
        //检测线程开启
        public List<List<string>> BeginReader()
        {
            if (cam != null)
            {
                if (img!=null) {
                    Thread t = new Thread(PicCut);
                    t.Start();
                }
            }
            else
            {
                /****************导入图片时调用
                List<string> strlist = QRMultiReader(img);
                if (strlist != null)
                {
                    qrCodeStr = strlist;
                    foreach (string s in strlist)
                    {
                        txtBox.Text += (s + "\r\n");
                    }
                }
                *************/
                MessageBox.Show("请先打开摄像机");
            }
            return qrCodeStr;
        }
        //关闭摄像机 释放资源
        public void CloseCam()
        {
            try
            {
                if (videoDevice != null)
                {
                    if (cam.IsRunning)
                    {
                        videoDevice.Clear();
                        cam.Stop();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("在关闭设备时发生错误" + e.Message);
            }
            //结束进程
            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);
            foreach (Process process in processes)
            {
                if (process.Id == current.Id)
                {
                    process.Kill();
                }
            }
        }
        //画框框子方法
        public void PaintKK() {
            Bitmap cb = new Bitmap(picBox.Width,picBox.Height);
            Graphics g = Graphics.FromImage(cb);
            Pen myPen = new Pen(Color.Red);
            myPen.Width = 2;
            g.DrawRectangle(myPen, (int)(setX * (width / 100.0f)) * ((float)picBox.Width) / width, (int)(setY * (width / 100.0f)) * ((float)picBox.Height) / height, (int)(setWidth * (width / 100.0f)) * ((float)picBox.Width) / width, (int)(setHeight * (height / 100.0f)) * ((float)picBox.Height) / height);
            g.Dispose();
            picBox.Image = cb;  
        }
        //单条形码识别
        private string QRcodeReader(Bitmap bmpQR)
        {
            string strQR = null;
            Result res=null;
            Result[] resArr = null;
            BarcodeReader barReader = new BarcodeReader();
            barReader.Options.CharacterSet = "UTF-8";
            barReader.AutoRotate = true;
            resArr= barReader.DecodeMultiple(bmpQR);
            res = barReader.Decode(bmpQR);
            if (res != null)
            {
                strQR = res.Text;
                resList = res.ResultPoints;
            }
            return strQR;
        }
        //多条形码识别 方法1 系统内置版本
        private List<string > MultiQRcodeReader(Bitmap bmpQR) {
            List<string> strQR = new List<string>();
            Result[] resArr = null;
            BarcodeReader barReader = new BarcodeReader();
            barReader.Options.CharacterSet = "UTF-8";
            //barReader.AutoRotate = true;
            try {
                if (bmpQR != null) {
                    resArr = barReader.DecodeMultiple(bmpQR);
                }
            }
            catch (Exception) {
            }
            
            if (resArr!=null) {
                foreach (var item in resArr) {
                    strQR.Add(item.Text);
                }
            }
            return strQR;
        }
        //多条形码识别 方法2    
        private List<string> QRMultiReader(Bitmap bmpMultiQR)
        {
            Bitmap bmpMultiQRUse = bmpMultiQR;
            bool isStop = true, isAdd = true; //是否循环  是否添加
            List<string> str = new List<string>();//返回的结果集
            string str1, str2;//临时 字符串
            float pointY, d = 5, h = 10;//填充用的坐标，填充速率，高度
            str1 = QRcodeReader(bmpMultiQRUse);
            if (str1 == null)
            {
                corsor++;
                if (msgBox!=null) {
                    msgBox.Text = "多条码识别尚未有结果" + corsor;
                }
                return null;
            }
            else {
                str.Add(str1);
                pointY = resList[0].Y; //填充起始点坐标
            }
            while (isStop)
            {
                if (pointY > 5 && h < bmpMultiQRUse.Height)
                {
                    AutoFillImage(pointY, h, bmpMultiQRUse);
                    //填充之后变换坐标，再次识别，填充。
                    pointY -= d;
                    h += (d + d);
                    str2 = QRcodeReader(bmpMultiQRUse);
                    if (str2 == null)
                    {
                        isStop = false;
                        break;
                    }
                    else
                    {
                        pointY = resList[0].Y;
                        for (int i = 0; i < str.Count; i++)
                        {
                            if (str[i] == str2)
                            {
                                isAdd = false;
                                break;
                            }
                            else
                            {
                                isAdd = true;
                            }
                        }
                    }
                    if (isAdd)
                    {
                        str.Add(str2);
                        str1 += "\r\n" + str2;
                    }
                }
                else
                {
                    isStop = false;
                    break;
                }
            }
            return str;
        }
        //多条形码识别 返回 数据对象
        private ReturnData QRMultiReaderAndReturnObj(Bitmap bmpMultiQR) {
            ReturnData rd = new ReturnData();
            Bitmap bmpMultiQRUse = bmpMultiQR;
            bool isStop = true, isAdd = true; //是否循环  是否添加
            List<string> str = new List<string>();//返回的结果集
            string str1, str2;//临时 字符串
            float pointY, d = 5, h = 10;//填充用的坐标，填充速率，高度
            //进度显示
            if (msgBox != null) {
                msgBox.Text = "正在识别 -----" + corsor + "%";
            }
            corsor += (100.0f / 18);
            str1 = QRcodeReader(bmpMultiQRUse);
            if (str1 == null) {
                return null;
            }
            else {
                str.Add(str1);
                pointY = resList[0].Y; //填充起始点坐标
            }
            while (isStop) {
                if (pointY > 5 && h < bmpMultiQRUse.Height) {
                    AutoFillImage(pointY, h, bmpMultiQRUse);
                    //填充之后变换坐标，再次识别，填充。
                    pointY -= d;
                    h += (d + d);
                    str2 = QRcodeReader(bmpMultiQRUse);
                    if (str2 == null){
                        isStop = false;
                        break;
                    }
                    else {
                        pointY = resList[0].Y;
                        for (int i = 0; i < str.Count; i++) {
                            if (str[i] == str2) {
                                isAdd = false;
                                break;
                            }
                            else {
                                isAdd = true;
                            }
                        }
                    }
                    if (isAdd) {
                        str.Add(str2);
                        str1 += "\r\n" + str2;
                    }
                }
                else {
                    isStop = false;
                    break;
                }
            }
            //排序输出

            rd.listStr = str;
            rd.bmp = bmpMultiQRUse;
            return rd;
        }
        //全方位识别
        private List<string> Reader(Bitmap bmp) {


            ReturnData rd = new ReturnData();
            List<string> str = new List<string>();//返回的结果集
            List<string> str2 = new List<string>();//临时结果集
            Bitmap rotateBmp;
            int angle=0;
            if (beginAngle == 4) beginAngle = 0;
            switch (beginAngle) {
                case 0:
                    angle = 0;
                    beginAngle += 1;
                    break;
                case 1:
                    angle = 5;
                    beginAngle += 1;
                    break;
                case 2:
                    angle = 10;
                    beginAngle +=1;
                    break;
                case 3:
                    angle = 15;
                    beginAngle +=1;
                    break;
                default:
                    break;
            }
            

            while (true){
                rotateBmp = RotateImg(bmp,angle);
                angle += 20;
                rd = QRMultiReaderAndReturnObj(rotateBmp);
                if (rd!=null)str2 = rd.listStr;
                if (angle > 360) {
                    angle = 0;
                    break;
                }
                else {
                    if (this.proBarShowEvent != null) {
                        proBarShowEvent(angle);
                    }
                }
                if (str2!=null) {
                    foreach (string sTemp in str2) {
                        bool isAdd=true;
                        foreach (var s in str) {
                            if (s==sTemp) {
                                isAdd = false;
                                break;
                            }
                            else {
                                isAdd = true;
                            }
                        }
                        if (isAdd) str.Add(sTemp);
                    }
                }
            }
            return str;
        }
        //全方位识别 3
        private List<string> Reader3(Bitmap bmp) {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            ReturnData rd = new ReturnData();
            List<string> str = new List<string>();//返回的结果集
            List<string> str2 = new List<string>();//临时结果集
            Bitmap rotateBmp;
            int angle = 0;
            /* 多方向不重复识别方案
            if (beginAngle == 3) beginAngle = 0;
            switch (beginAngle) {
                case 0:
                    angle = 0;
                    beginAngle += 1;
                    break;
                case 1:
                    angle = 5;
                    beginAngle += 1;
                    break;
                case 2:
                    angle = 10;
                    beginAngle += 1;
                    break;
                default:
                    break;
            }
            */
            while (true) {
                rotateBmp = RotateImg(bmp, angle);
                angle += 15;
                str2 = MultiQRcodeReader(rotateBmp);
                //str2 = QRMultiReader(rotateBmp);
                if (angle > 360) {
                    angle = 0;
                    break;
                }
                else {
                    if (this.proBarShowEvent != null) {
                        proBarShowEvent(angle);
                    }
                }
                if (str2 != null) {
                    foreach (string sTemp in str2) {
                        bool isAdd = true;
                        foreach (var s in str) {
                            if (s == sTemp) {
                                isAdd = false;
                                break;
                            }
                            else {
                                isAdd = true;
                            }
                        }
                        if (isAdd) str.Add(sTemp);
                    }
                }
            }
            sw.Stop();
            timers = sw.Elapsed;
            usedTime();
            return str;
        }
        //图片打开
        public string selectFile() {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Title = "请选择文件夹";
            if (sf.ShowDialog() == DialogResult.OK)
            {
                return sf.FileName;
            }
            else {
                return "";
            }
        }
        public void OpenFile()
        {
            OpenFileDialog openf = new OpenFileDialog();
            openf.Title = "select a picture";
            openf.Filter = "Images(*.BMP;*.JPG;*.PNG;*.GIF)|*.BMP;*.JPG;*.PNG";
            if (openf.ShowDialog() == DialogResult.OK)
            {
                img = (Bitmap)Bitmap.FromFile(openf.FileName);
                if (img != null)
                {
                    picBox.Image = img;
                }
            }
        }
        //保存图片 选择文件夹及设定文件名保存
        public bool SaveImg()
        {
            bool isReturn = false;
            try
            {
                SaveFileDialog saveFile = new SaveFileDialog();
                saveFile.Title = "保存图片";
                saveFile.Filter = "Images(*.JPG;*.PNG)|*.JPG;*.PNG";
                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    img.Save(saveFile.FileName, ImageFormat.Jpeg);
                    MessageBox.Show(saveFile.FileName);
                    isReturn = true;
                }
            }
            catch
            {
                isReturn = false;
            }
            return isReturn;
        }
        //自动保存图片  默认根目录
        public bool AutoSavePic(Bitmap saveImg) {
            try {
                Regex reg = new Regex(@"\S+[^(\r\n)]");    //提取条形码
                string str = reg.Match(tempStr).Value;
                //多条形码起名
                if (reg.Matches(tempStr).Count>1) {
                    str += "(识别出" + reg.Matches(tempStr).Count + "个条形码)";
                }
                string path = Directory.GetCurrentDirectory() +"\\"+str+ ".jpg";
                //保存图片的名称为识别的条形码
                saveImg.Save(path, ImageFormat.Jpeg);
                return true;
            } catch {
                return false;
            }
        }
        //自动保存图片2 自定义目录 需要输入
        public bool AutoSavePic(string path) {
            Bitmap savePic = img;
            try{
                path = path + "\\" + DateTime.Now.ToFileTime() + ".jpg";
                savePic.Save(path, ImageFormat.Jpeg);
                return true;
            }catch{
                return false;
            }
        }
        //自定义切图
        public Bitmap ImgCut(int x,int y,int width,int height,Bitmap bmp)
        {
            Crop crop = new Crop(new Rectangle(x, y, width, height ));
            Bitmap small = crop.Apply(bmp);
            return small;
        }
        //图像填充
        public Bitmap AutoFillImage(float pointY, float height,Bitmap bm)
        {
            Graphics g = Graphics.FromImage(bm);
            SolidBrush b1 = new SolidBrush(Color.Black);
            //HeightComp();
            g.FillRectangle(b1, resList[0].X, pointY, (resList[1].X - resList[0].X), height);
            g.Dispose();
            return bm;
        }
        
        public Bitmap RotateImg(System.Drawing.Image img, float angle) {
            //通过Png图片设置图片透明，修改旋转图片变黑问题。  
            int width = img.Width;
            int height = img.Height;
            //角度  
            Matrix mtrx = new Matrix();
            mtrx.RotateAt(angle, new PointF((width / 2), (height / 2)), MatrixOrder.Append);
            //得到旋转后的矩形  
            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(new RectangleF(0f, 0f, width, height));
            RectangleF rct = path.GetBounds(mtrx);
            //生成目标位图  
            Bitmap devImage = new Bitmap((int)(rct.Width), (int)(rct.Height));
            Graphics g = Graphics.FromImage(devImage);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //计算偏移量  
            System.Drawing.Point Offset = new System.Drawing.Point((int)(rct.Width - width) / 2, (int)(rct.Height - height) / 2);
            //构造图像显示区域：让图像的中心与窗口的中心点一致  
            Rectangle rect = new Rectangle(Offset.X, Offset.Y, (int)width, (int)height);
            System.Drawing.Point center = new System.Drawing.Point((int)(rect.X + rect.Width / 2), (int)(rect.Y + rect.Height / 2));
            g.TranslateTransform(center.X, center.Y);
            g.RotateTransform(angle);
            //恢复图像在水平和垂直方向的平移  
            g.TranslateTransform(-center.X, -center.Y);
            g.DrawImage(img, rect);
            //重至绘图的所有变换  
            g.ResetTransform();
            g.Save();
            g.Dispose();
            path.Dispose();
            return devImage as Bitmap;
        }
        
        //图像旋转2
       /*
        public Bitmap RotateImg(System.Drawing.Image img, float rotationAngle) {
            Bitmap bmp = new Bitmap(img.Width, img.Height);
            Graphics gfx = Graphics.FromImage(bmp);
            gfx.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);
            gfx.RotateTransform(rotationAngle);
            gfx.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);
            gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gfx.DrawImage(img, new System.Drawing.Point(0, 0));
            gfx.Dispose();
            return bmp as Bitmap;
        }
        /*********
         public Bitmap RotateImg(Bitmap bmp, float angle) {
             RotateBicubic filter = new RotateBicubic(angle, true);
             Bitmap newImage = filter.Apply(bmp);
             return newImage;
         }
         **********/
    }

    /// <summary>
    /// 返回值类型
    /// </summary>
    class ReturnData
    {
        public Bitmap bmp;
        public List<string> listStr;
        //初始化
        public ReturnData() {
            bmp = null;
            listStr = new List<string>();
        }
    }

}

