using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Patagames.Ocr;
using Patagames.Ocr.Enums;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using AForge;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Imaging;
using AForge.Imaging.Filters;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

using System.Threading;
using System.IO;
using System.Runtime.InteropServices;

namespace ocr
{
    public enum OcrLanguage {
         chi,
         eng,
    };
    /// <summary>
    /// 提供ocr识别的功能；
    /// 以及摄像机开启/关闭； 
    /// 识别后的字符串信息提取处理；
    /// </summary>
    public sealed class IdentityOcr
    {
        public Bitmap img;        //导入的图像
        public PictureBox picBox;        //图像显示框
        //public TextBox txtBox;           //input txtbox
        //public TextBox msgBox;           //result show box
        public string ocrStr;            //识别的结果字符串
        public OcrLanguage lang;         //语言设置
        private FilterInfoCollection videoDevice;    //摄像机设备
        private VideoCaptureDevice cam;       //摄像机
        public bool isFnh;                    //摄像机打开状态   
        public int width, height;             //图片的宽高

        public delegate void Receive();      //创建观察者委托
        public event Receive receiveEvent;   //观察者事件
        public Identity identityResult;      //识别后的对象
        private bool isRunning;

        //初始化
        public IdentityOcr() {
            identityResult = new Identity();
            isRunning = false;
            isFnh = false;
        }

        //销毁对象方法
        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        //----------------------------------------------------
        //打开摄像机 并将摄像机内容显示在picturebox中
        public void OpenCam()
        {
            try
            {
                videoDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                // MessageBox.Show("检测到"+videoDevice.Count.ToString()+"个摄像机设备，默认打开外置");
                if (videoDevice.Count > 0)
                {
                    cam = new VideoCaptureDevice(videoDevice[0].MonikerString);
                    cam.VideoResolution = cam.VideoCapabilities[0];
                    cam.Start();

                    //cam.newFrame 是一个newFrameEcentHandler 委托类型
                    //使用Lambda表达式 来写匿名方法   
                    //此方法用来控制图片刷新
                    cam.NewFrame += new NewFrameEventHandler((object obj, NewFrameEventArgs eventArg) =>
                    {
                        img = (Bitmap)eventArg.Frame.Clone();
                        Bitmap imgShow = img;
                        width = imgShow.Width;
                        height = imgShow.Height;
                        Graphics g = picBox.CreateGraphics();
                        g.InterpolationMode = InterpolationMode.NearestNeighbor;
                        //图像大小转换
                        if (width>1280||height>720) {
                            g.DrawImage(imgShow, new Rectangle(0, 0, picBox.Width, picBox.Height), new Rectangle((width - 1280) / 2, (height - 720) / 2, 1280, 720), GraphicsUnit.Pixel);
                        }
                        else {
                            g.DrawImage(imgShow, new Rectangle(0, 0, picBox.Width, picBox.Height), new Rectangle(0,0,width,height), GraphicsUnit.Pixel);
                        }
                        //图像剪切
                        img = ImgCut((width - 1280) / 2, (height - 720) / 2, 1280, 720, img);
                        g.Dispose();
                        isFnh = true;
                    });
                }
                else
                {
                    MessageBox.Show("没有摄像机或者打开摄像机出现异常");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("打开摄像机发生异常，请检查摄像机设备" + e.Message);
            }
        }
        //关闭摄像机
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
        //自定义切图
        public Bitmap ImgCut(int x, int y, int width, int height, Bitmap bmp)
        {
            Crop crop = new Crop(new Rectangle(x, y, width, height));
            Bitmap small = crop.Apply(bmp);
            return small;
        }
//-------------------------------------------------------------------
        //ocr 文字识别
        //从图片中获取文字    图片为字段img
        private string ExtractTextFromImage()
        {
            if (img.Width>500||img.Height>500) {
                int width = img.Width;
                int height = img.Height;
                img =ImgCut((width - 480) / 2, (height - 480) / 2, 480, 480, img);
            }
            using (OcrApi api = OcrApi.Create())
            {
                if (lang == OcrLanguage.chi)
                {
                    string[] configs = { "config.cfg" };
                    api.Init(Languages.ChineseSimplified);
                }
                else
                {
                    string[] configs = { "config.cfg" };
                    api.Init(Languages.English);
                }
                try
                {
                    //ocrStr = api.GetTextFromImage(img);
                    ocrStr = api.GetTextFromImage(img, 0, 0,img.Width,img.Height);
                    ocrStr = api.GetUtf8Text();
                   // txtBox.Text = ocrStr;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            return ocrStr;
        }
        //从目标图片中获取文字
        public string ExtractTextFromImage(Bitmap bmp)
        {
            if (img.Width > 500 || img.Height > 500)
            {
                int width = img.Width;
                int height = img.Height;
                bmp = ImgCut((width - 480) / 2, (height - 480) / 2, 480, 480, bmp);
            }
            using (OcrApi api = OcrApi.Create())
            {
                if (lang == OcrLanguage.chi)
                {
                    api.Init(Languages.ChineseSimplified);
                }
                else
                {
                    api.Init(Languages.English);
                }
                try
                {
                    ocrStr = api.GetTextFromImage(bmp);
                    ocrStr = api.GetUtf8Text();
                    //txtBox.Text = IdentityString(ocrStr);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message+"图像过大，请将图片限制在500*500 px 之内");
                }
            }
            return ocrStr;
        }
        public void ExtractTextFromImage2(string fileName) {
            string str = null;

            using (var api = OcrApi.Create()) {
                api.Init(Languages.English);
                //Create the renderer to PDF file output. The extension will be added automatically
                using (var renderer = OcrPdfRenderer.Create("output_pdf_file", Directory.GetCurrentDirectory())) {
                    renderer.BeginDocument("Title");
                    api.ProcessPages(fileName, null, 0, renderer);
                    renderer.EndDocument();
                }
            }
        }

        //-----------------------------------------------------------------------
        //字符串处理 身份证 字符串处理
        public Identity IdentityCodeString(string ocrString)
        {
            Identity iC = new Identity();
            //去头去尾
            ocrString = ocrString.Trim();
            ocrString = ocrString.Replace(' ', '-');
            //分割
            string[] str = ocrString.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < str.Length; i++)
            {
                string[] arr = str[i].Split('-');
                str[i] = string.Concat(arr);
            }
            //MessageBox.Show(ocrString);
            for (int i = 0; i < str.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        int indexBegin = str[i].IndexOf("");
                        iC.name = str[i].Substring(2);
                        break;
                    case 1:
                        indexBegin = str[i].IndexOf("性");
                        iC.gender = str[i].Substring(2, 1);
                        indexBegin = str[i].IndexOf("");
                        iC.nation = str[i].Substring(5);
                        break;
                    case 2:
                        indexBegin = str[i].IndexOf("");
                        iC.date = str[i].Substring(2);
                        break;
                    case 3:
                        indexBegin = str[i].IndexOf("");
                        iC.address = str[i].Substring(2);
                        break;
                    case 4:
                        if (str.Length <= 5)
                        {
                            indexBegin = str[i].IndexOf("");
                            iC.idNumber = str[i].Substring(7);
                        }
                        else
                        {
                            iC.address += str[i];
                        }
                        break;
                    default:
                        indexBegin = str[i].IndexOf("");
                        if (str[i].Length > 6)
                        {
                            iC.idNumber = str[i].Substring(6);
                        }
                        break;
                }
            }
            return iC;
        }
        //换行符的处理
        public string IdentityString(string ocrString)
        {
            string res = null;
            //去头去尾
            if (ocrString==null||ocrString=="") {
                return res;
            }
            ocrString = ocrString.Trim();
            //ocrString = ocrString.Replace(' ', '-');
            //分割
            string[] str = ocrString.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < str.Length; i++)
            {
                string[] arr = str[i].Split(' ');
                str[i] = string.Concat(arr);
                res += str[i] + "\r\n";
            }
            
            return res;
        }
        //除去字符串中的标点符号
        public string RemoveBd(string str) {
            Regex reg = new Regex(@"\p{P}");  //正则当中 \p{P} 代表所有的标点符号
            if (str!=""&&str!=null) {
                str = reg.Replace(str, "");
            }
            return str;
        }
        //去除空白
        public string RemoveNull(string str) {
            Regex reg = new Regex(@"\s");  //正则当中 \p{P} 代表所有的标点符号
            if (str != "" && str != null)
            {
                str = reg.Replace(str, "");
            }
            return str;
        }
        //将易混淆的字母字符转成数字
        public string LetterToNumber(params string[] s) {
            string str = s[0];
            string regPlus = null;
            if (s.Length == 2) 
            {
                regPlus = s[1];
            }
            if (str == "" || str == null)
            {
                return "";
            }
            //容错处理 错误修改
            string listReg = @"(i|l|z|Z|o|O|b|A|D|s|S|B|m|三|〇|\?|g)";
            Regex reg = new Regex(listReg);
            int icount = reg.Matches(str).Count;
            if (icount != 0)
            {
                for (int i = 0; i < icount; i++)
                {
                    //校正列表
                    switch (reg.Match(str).Value)
                    {
                        case "i":
                            str = Regex.Replace(str, @"i", "1");
                            break;
                        case "l":
                            str = Regex.Replace(str, @"l", "1");
                            break;
                        case "z":
                            str = Regex.Replace(str, @"z", "2");
                            break;
                        case "Z":
                            str = Regex.Replace(str, @"Z", "2");
                            break;
                        case "o":
                            str = Regex.Replace(str, @"o", "0");
                            break;
                        case "O":
                            str = Regex.Replace(str, @"O", "0");
                            break;
                        case "b":
                            str = Regex.Replace(str, @"b", "6");
                            break;
                        case "A":
                            str = Regex.Replace(str, @"A", "4");
                            break;
                        case "D":
                            str = Regex.Replace(str, @"D", "0");
                            break;
                        case "s":
                            str = Regex.Replace(str, @"s", "5");
                            break;
                        case "S":
                            str = Regex.Replace(str, @"S", "5");
                            break;
                        case "B":
                            str = Regex.Replace(str, @"B", "8");
                            break;
                        case "m":
                            str = Regex.Replace(str, @"m", "3");
                            break;
                        case "三":
                            str = Regex.Replace(str, @"三", "3");
                            break;
                        case "〇":
                            str = Regex.Replace(str, @"〇", "0");
                            break;
                        case "?":
                            str = Regex.Replace(str, @"?", "7");
                            break;
                        case "g":
                            str = Regex.Replace(str, @"g", "9");
                            break;
                    }
                }
            }
            return str;
        }

        //email获取
        private string GetEmail(string str){
            string email = null;
            Regex reg = new Regex(@"\S*@\S*\.\S*");
            email = reg.Match(str).Value;
            return email;
        }
        //手机
        private string GetCallPhoneNumber(string str) {
            string phoneNumber=null;
            Regex reg = new Regex(@"1\d{10}");
            Regex reg2 = new Regex(@"\d{11}");
            phoneNumber = reg.Match(str).Groups[0].Value;
            if (phoneNumber==null ||phoneNumber=="") {
                phoneNumber = reg2.Match(str).Value;
            }
            return phoneNumber;
        }
        //座机号获取
        private string GetTelephoneNumber(string str)
        {
            string phoneNumber = null;
            Regex reg = new Regex(@"\d{2,4}(_|-|——|一|__|--)\d{6,8}");
            phoneNumber = reg.Match(str).Groups[0].Value;
            return phoneNumber;
        }
        //身份证号码获取
        private string GetIdentityCodeNumber(string str)
        {
            string identityNumber = null;
            Regex reg = new Regex(@"\d{18}");
            Regex reg2 = new Regex(@"\d{17}\w");
            Regex reg3 = new Regex(@"(公民身份号码|身份号码|份号码|(公\S身\S号码)|(\S民\S+号码)|(公\S+码)|(\S{2}身份\S{2})|(\S+份证号\S))\S+");
            Regex reg4 = new Regex(@"(\d\S{10,})");
            identityNumber = RemoveNull(str);
            identityNumber = reg.Match(str).Groups[0].Value;
            if (identityNumber==null||identityNumber=="") {
                identityNumber = reg2.Match(str).Value;
                if (identityNumber == null || identityNumber == "") {
                    identityNumber = reg3.Match(str).Value;
                    identityNumber = RemoveNull(identityNumber);
                    identityNumber = reg4.Match(identityNumber).Value;
                    identityNumber = LetterToNumber(identityNumber);
                }
            }
            
            return identityNumber;
        }
        
        //名字获取
        private string GetName(string str) {
            string name = null;
            string familyName = "赵|钱|孙|李|周|吴|郑|王|冯|陈|楮|卫|蒋|沈|韩|杨|朱|秦|尤|许|何|吕|施|张|孔|曹|严|华|金|魏|陶|姜|戚|谢|邹|喻|柏|水|窦|章|云|苏|潘|葛|奚|范|彭|郎|鲁|韦|昌|马|苗|凤|花|方|俞|任|袁|柳|酆|鲍|史|唐|费|廉|岑|薛|雷|贺|倪|汤|滕|殷|罗|毕|郝|邬|安|常|乐|于|时|傅|皮|卞|齐|康||伍|余|元|卜|顾|孟|平|黄|和|穆|萧|尹|姚|邵|湛|汪|祁|毛|禹|狄|米|贝|明|臧|计|伏|成|戴|谈|宋|茅|庞|熊|纪|舒|屈|项|祝|董|梁|杜|阮|蓝|闽|席|季|麻|强|贾|路|娄|危|江|童|颜|郭|梅|盛|林|刁|锺|徐|丘|骆|高|夏|蔡|田|樊|胡|凌|霍|虞|万|支|柯|昝|管|卢|莫|经|房|裘|缪|干|解|应|宗|丁|宣|贲|邓|郁|单|杭|洪|包|诸|左|石|崔|吉|钮|龚|程|嵇|邢|滑|裴|陆|荣|翁|荀|羊|於|惠|甄|麹|家|封|芮|羿|储|靳|汲|邴|糜|松|井|段|富|巫|乌|焦|巴|弓|牧|隗|山|谷|车|侯|宓|蓬|全|郗|班|仰|秋|仲|伊|宫|宁|仇|栾|暴|甘|斜|厉|戎|祖|武|符|刘|景|詹|束|龙|叶|幸|司|韶|郜|黎|蓟|薄|印|宿|白|怀|蒲|邰|从|鄂|索|咸|籍|赖|卓|蔺|屠|蒙|池|乔|阴|郁|胥|能|苍|双|闻|莘|党|翟|谭|贡|劳|逄|姬|申|扶|堵|冉|宰|郦|雍|郤|璩|桑|桂|濮|牛|寿|通|边|扈|燕|冀|郏|浦|尚|农|温|别|庄|晏|柴|瞿|阎|充|慕|连|茹|习|宦|艾|鱼|容|向|古|易|慎|戈|廖|庾|终|暨|居|衡|步|都|耿|满|弘|匡|国|文|寇|广|禄|阙|东|欧|殳|沃|利|蔚|越|夔|隆|师|巩|厍|聂|晁|勾|敖|融|冷|訾|辛|阚|那|简|饶|空|曾|毋|沙|乜|养|鞠|须|丰|巢|关|蒯|相|查|后|荆|红|游|竺|权|逑|盖|益|桓|公||万俟|司马|上官|欧阳|夏侯|诸葛|闻人|东方|赫连|皇甫|尉迟|公羊|澹台|公冶|宗政|濮阳|淳于|单于|太叔|申屠|公孙|仲孙|轩辕|令狐|锺离|宇文|长孙|慕容|鲜于|闾丘|司徒|司空|丌官|司寇|仉|督|子车|颛孙|端木|巫马|公西|漆雕|乐正|壤驷|公良|拓拔|夹谷|宰父|谷梁|晋|楚|阎|法|汝|鄢|涂|钦|段干|百里|东郭|南门|呼延|归|海|羊舌|微生|岳|帅|缑|亢|况|后|有|琴|梁丘|左丘|东门|西门|商|牟|佘|佴|伯|赏|南宫|墨|哈|谯|笪|年|爱|阳|佟|第五|言|福";
            Regex reg = new Regex(@"(姓名|名)+[\u4e00-\u9fa5]{1,3}");   //[\u4e00-\u9fa5]+   
            Regex reg2 = new Regex(@"[^(姓名|名)]+[\u4e00-\u9fa5]{2,4}");
            Regex reg3 = new Regex(@"("+ familyName + ")+[\u4e00-\u9fa5]{2,4}");
            name = reg.Match(str).Value;
            name = reg2.Match(name).Value;
            if (name==null||name=="") {
                name = reg3.Match(str).Value;
            }
            return name;
        }
        //换行获取
        private string GetEnter(string str) {
            string enter = null;
            Regex reg = new Regex(@"\r\n");
            enter = reg.Match(str).Value;
            return enter;
        }
        //性别获取
        private string GetGender(string str) {
            string gender = null;
            Regex reg = new Regex(@"[男女]");
            gender = reg.Match(str).Value;
            return gender;
        }
        //公司名称
        private string GetCompanyName(string str) {
            string companyName = null;
            string companyAttribute = null;
            Regex reg = new Regex(@"[\u4e00-\u9fa5]+(公司|公|司|集团|集|团|工作室|事务所|部)");
            Regex reg2 = new Regex(@"[^(公司|公|司|集团|集|团|工作室|事务所|部)][\u4e00-\u9fa5]+[^(公司|公|司|集团|集|团|工作室|事务所|部)]");
            Regex reg3 = new Regex(@"(公司|公|司|集团|集|团|工作室|事务所|部)");
            companyName = reg.Match(str).Value;
            companyAttribute = reg3.Match(companyName).Value;
            if (companyAttribute!=null) {
                if (companyAttribute == "公" || companyAttribute == "司" || companyAttribute == "公司")
                {
                    companyAttribute = "公司";
                }
                else if (companyAttribute == "集" || companyAttribute == "团" || companyAttribute == "集团")
                {
                    companyAttribute = "集团";
                }
            }
            companyName = reg2.Match(companyName).Value+companyAttribute;
            return companyName;
        }
        //出生年月
        private string GetAgeDate(string str)
        {
            string ageDate = null;
            Regex reg = new Regex(@"\d{4}[^\d]{1,3}\d{2}[^\d]{1,3}\d{2}");        //1998 年 10 月 10 号
            Regex reg2 = new Regex(@"(出生|出生年月|年月|出生日期|月|出|生)\S+");
            Regex reg3 = new Regex(@"[^(出生|出生年月|年月|出生日期|月)]\S+");
            Regex reg4 = new Regex(@"(i|l|z|Z|o|O|b|A|D|s|S|B|m)");

            ageDate = reg.Match(str).Value;
            if (ageDate==""||ageDate==null)ageDate = reg2.Match(str).Value;
            int icount = reg4.Matches(ageDate).Count;
            if (icount!=0) {
                ageDate = LetterToNumber(ageDate);
            }
            if (ageDate!=null||ageDate!="") {
                ageDate = reg3.Match(ageDate).Value;
                ageDate = Regex.Replace(ageDate, @"[\u4e00-\u9fa5]", ".");
            }
            return ageDate;
        }
        //职务获取
        private string GetJobName(string str) {
            string jobName = null;
            Regex reg = new Regex(@"[\u4e00-\u9fa5]{0,6}(经理|助理|主管|员|职员|工程师|师|主任|秘书|长|顾问|翻译|代表|人|会计|家|编辑)");
            Regex reg2 = new Regex(@"[^(职务|部门|务|门)][\u4e00-\u9fa5]+");
            //Regex reg3 = new Regex(@"[\u4e00-\u9fa5]{0,6}(员||人)");
            jobName = reg.Match(str).Value;
            jobName = reg2.Match(jobName).Value;
            return jobName;
        }
        //民族
        private string GetNation(string str)
        {
            string nation = null;
            Regex reg = new Regex(@"((汉|壮|满|回|苗|维吾尔|土家|彝|蒙古|藏|布依|侗|瑶|朝鲜|白|哈尼|哈萨克黎|傣|畲|傈僳|仡佬|东乡|高山|拉祜|水|佤| 纳西|羌|土|仫佬|锡伯|柯尔克孜|达斡尔|景颇|毛南|撒拉|布朗|塔吉克|阿昌|普米|鄂温克|怒|京|基诺|德昂|保安|俄罗斯|裕固|乌孜别克|门巴|鄂伦春|独龙|塔塔尔|赫哲|珞巴)族|汉)");
            Regex reg2 = new Regex(@"(民族|族)[\u4e00-\u9fa5]{0,4}族");
            Regex reg3 = new Regex(@"[^(民族|族)][\u4e00-\u9fa5]{0,4}族");
            Regex reg4 = new Regex(@"");
            nation = reg.Match(str).Value;
            if (nation==null||nation=="") {
                nation = reg2.Match(str).Value;
                nation = reg3.Match(nation).Value;
                //MessageBox.Show(reg2.Matches(str).Count.ToString());
            }
            return nation;
        }
        //年龄
        private string GetAges(string str) {
            string getAges = null, comAges=null;
            DateTime year =new DateTime();
            year = System.DateTime.Now;
            Regex reg = new Regex(@"(年龄)[^\d]{0,3}\d{2}");
            Regex reg1 = new Regex(@"\d{2}");
            Regex reg2=new Regex(@"\d{4}");
            getAges = reg.Match(str).Value;
            getAges = reg1.Match(getAges).Value;
            comAges = reg2.Match(this.GetAgeDate(str)).Value;
            if (getAges==" ") {
                int icomAges = Math.Abs(Convert.ToInt16(comAges)-year.Year);
                getAges = icomAges+"";
            } 
            return getAges;
        }
        //住址
        private string GetAddress(string str)
        {
            string address = null;
            string other = null;
            Regex reg = new Regex(@"(住址|地点|地址|位置|址|住)[\u4e00-\u9fa5]*\r\n[\u4e00-\u9fa5]*");
            Regex reg_= new Regex(@"(住址|地点|地址|位置|址|住)[\u4e00-\u9fa5]*");
            Regex reg2 = new Regex(@"(住址|地点|地址|位置|址|住)\S+\r\n\S+");
            Regex reg2_ = new Regex(@"(住址|地点|地址|位置|址|住)\S+");
            Regex reg3 = new Regex(@"[^(住址|地点|地址|位置|址|住)][\u4e00-\u9fa5]*\r\n\S+");
            Regex reg3_ = new Regex(@"[^(住址|地点|地址|位置|址|住)][\u4e00-\u9fa5]*");
            Regex reg4 = new Regex(@"\r\n");
            Regex reg5 = new Regex(@"(公民身份号码|身份号码|份号码|(公\S身\S号码)|(\S民\S+号码))");
            address = RemoveNull(str);
            address = reg.Match(address).Value;
            if (address == null || address == ""||address.Length<=4) {
                address= reg2.Match(str).Value;
            }
            address =reg3.Match(address).Value;
            address = Regex.Replace(address, @"\r\n", "");
            other = reg5.Match(address).Value;
            if (other == null || other == "")
            {
                return address;
            }
            else {
                address = reg_.Match(str).Value;
                if (address == null || address == "" || address.Length <= 4)
                {
                    address = reg2_.Match(str).Value;
                }
                address = reg3_.Match(address).Value;
                return address;
            }
            
        }
        private string GetCompanyAddress(string str)
        {
            string address = null;
            string other = null;
            Regex reg = new Regex(@"(住址|地点|地址|位置|址|住)[\u4e00-\u9fa5]*");
            Regex reg2 = new Regex(@"(住址|地点|地址|位置|址|住)\S+");
            Regex reg3= new Regex(@"[^(住址|地点|地址|位置|址|住)][\u4e00-\u9fa5]*");
            Regex reg4 = new Regex(@"\r\n");
            address = RemoveNull(str);
            address = reg.Match(str).Value;
            if (address == null || address == "" || address.Length <= 4)
            {
                address = reg2.Match(str).Value;
            }
            address = reg3.Match(address).Value;
            return address;
        }
        //网站
        private string GetSites(string str) {
            string sites=null;
            Regex reg = new Regex(@"(www|ww\S|w\S\S).\S+.(com|cn|net|cc|org|sites)");
            sites = reg.Match(str).Value;
            return sites;
        }
        //QQ
        private string GetQQ(string str) {
            string qq = null;
            Regex reg = new Regex(@"(QQ|qq)\S+");
            Regex reg2 = new Regex(@"[^(QQ|qq)]\d+");
            qq = reg.Match(str).Value;
            qq = reg2.Match(qq).Value;
            return qq;
        }
        //fax 传真
        private string GetFax(string str) {
            string fax = null;
            Regex reg = new Regex(@"\d{3,5}(_|-|——|一|__|--)\d{6,8}");
            fax = reg.Match(str).Value;
            return fax;
        }

        //对img 进行识别并且返回对象
        public void GetIdentity() {
            string ocrGetString = null;
            IAsyncResult ar=null;
            Func<string> funcOcr;
            if (img != null) {
                funcOcr = ExtractTextFromImage;     //中文识别 开启新线程
                ar = funcOcr.BeginInvoke((IAsyncResult a)=>{
                    ocrGetString = funcOcr.EndInvoke(a);
                    if (ocrGetString!=null && ocrGetString!="") {
                        ocrGetString = IdentityString(ocrGetString);      //换行符处理
                        ocrGetString = RemoveBd(ocrGetString);            //去标点处理
                        identityResult.name = GetName(ocrGetString);
                        identityResult.gender = GetGender(ocrGetString);
                        identityResult.nation = GetNation(ocrGetString);
                        identityResult.age = GetAges(ocrGetString);
                        identityResult.date = GetAgeDate(ocrGetString);
                        identityResult.address = GetAddress(ocrGetString);
                        identityResult.idNumber = GetIdentityCodeNumber(ocrGetString);
                        identityResult.companyName = GetCompanyName(ocrGetString);
                        identityResult.position = GetJobName(ocrGetString);
                        identityResult.companyAddress = GetCompanyAddress(ocrGetString);
                        identityResult.tel = GetTelephoneNumber(ocrGetString);
                        identityResult.email = GetEmail(ocrGetString);
                        identityResult.phoneNumber = GetCallPhoneNumber(ocrGetString);
                        identityResult.sites = GetSites(ocrGetString);
                        identityResult.qq = GetQQ(ocrGetString);
                        identityResult.fax = GetFax(ocrGetString);
                        //当对identityResult赋值完成后触发事件
                        receiveEvent();
                    } 
                },ar);
            }
        }
        //对传入的图片识别后提取出识别对象
        public void GetIdentity(Bitmap a)
        {
            //如果正在运行识别，则直接退出
            if (isRunning) {
                MessageBox.Show("识别程序正在运行，请稍等！");
                return;
            }
            isRunning = true;  
            Identity id = new Identity();
            string ocrGetString = null;
            IAsyncResult ar = null;
            Func<Bitmap,string> funcOcr;
            if (a != null) {
                //ocrGetString = ExtractTextFromImage(a);    //中文识别
                funcOcr = ExtractTextFromImage;     //中文识别 开启新线程
                ar = funcOcr.BeginInvoke(a,(IAsyncResult b) =>{
                    ocrGetString = funcOcr.EndInvoke(b);
                    if (ocrGetString != null && ocrGetString != "") {
                        ocrGetString = IdentityString(ocrGetString);      //换行符处理
                        ocrGetString = RemoveBd(ocrGetString);            //去标点处理
                        identityResult.name = GetName(ocrGetString);
                        identityResult.gender = GetGender(ocrGetString);
                        identityResult.nation = GetNation(ocrGetString);
                        identityResult.age = GetAges(ocrGetString);
                        identityResult.date = GetAgeDate(ocrGetString);
                        identityResult.address = GetAddress(ocrGetString);
                        identityResult.idNumber = GetIdentityCodeNumber(ocrGetString);
                        identityResult.companyName = GetCompanyName(ocrGetString);
                        identityResult.position = GetJobName(ocrGetString);
                        identityResult.companyAddress = GetCompanyAddress(ocrGetString);
                        identityResult.tel = GetTelephoneNumber(ocrGetString);
                        identityResult.email = GetEmail(ocrGetString);
                        identityResult.phoneNumber = GetCallPhoneNumber(ocrGetString);
                        identityResult.sites = GetSites(ocrGetString);
                        identityResult.qq = GetQQ(ocrGetString);
                        identityResult.fax = GetFax(ocrGetString);
                        receiveEvent();
                        isRunning = false;//识别程序结束
                    }
                }, ar);
            }
        }
    }

    /// <summary>
    /// 名片；
    /// 姓名、性别、年龄、地址、手机、电话、工作单位、邮箱等信息；
    /// 联络资料(中英文地址、电话、行动电话、呼叫器、传真号码)
    /// </summary>
    public class Identity
    {
        public string name { get; set; }             //姓名
        public string gender { get; set; }           //性别
        public string nation { get; set; }           //民族
        public string age { get; set; }              //年龄
        public string date { get; set; }             //出生日期
        public string address { get; set; }          //居住地址
        public string idNumber { get; set; }         //身份证号码
        public string companyName { get; set; }      //公司名称
        public string position { get; set; }         //职位
        public string companyAddress { get; set; }   //公司地址
        public string tel { get; set; }              //电话
        public string email { get; set; }            //邮箱
        public string phoneNumber { get; set; }      //手机号码
        public string sites { get; set; }            //网站
        public string qq { get; set; }               //qq
        public string fax { get; set; }              //传真

        public override string ToString() {
            string outResult = null;
            outResult += name + "\r\n";
            outResult += gender + "\r\n";
            outResult += nation + "\r\n";
            outResult += age + "\r\n";
            outResult += date + "\r\n";
            outResult += address + "\r\n";
            outResult += idNumber + "\r\n";
            outResult += companyName + "\r\n";
            outResult += position + "\r\n";
            outResult += companyAddress + "\r\n";
            outResult += tel + "\r\n";
            outResult += email + "\r\n";
            outResult += phoneNumber + "\r\n";
            outResult += sites + "\r\n";
            outResult += fax + "\r\n";
            return outResult;
        }
        //对象转成字符--名片 
        public string BusinessCard() {
            string str = "";
            str += name + "\r\n";
            str += companyName + "\r\n";
            str += position + "\r\n";
            str += companyAddress + "\r\n";
            str += tel + "\r\n";
            str += email + "\r\n";
            str += phoneNumber + "\r\n";
            str += sites + "\r\n";
            str += fax + "\r\n";
            return str;
        }
        //身份证 对象转换成字符
        public string IdentituCard() {
            string str = "";
            str += name + "\r\n";
            str += gender + "\r\n";
            str += nation + "\r\n";
            str += date + "\r\n";
            str += address + "\r\n";
            str += idNumber + "\r\n";
            return str;
        }
    }

    /// <summary>
    /// 图像处理工具集
    /// </summary>
    public class PicTools
    {
        //图像二值化
        public static Bitmap staBmp;
        public Bitmap Binaryzation(Bitmap img) {
            //格式转化
            if (img != null)
            {
                if (img != staBmp) {
                    var bnew = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
                    Graphics g = Graphics.FromImage(bnew);
                    g.DrawImage(img, 0, 0);
                    g.Dispose();
                    //灰度化
                    img = new Grayscale(0.3, 0.3, 0.3).Apply(img);
                    //img = new Grayscale(0.2125, 0.7154, 0.0721).Apply(img);
                    //二值化
                    img = new Threshold(150).Apply(img);
                    img = new BlobsFiltering(1, 1, img.Width, img.Height).Apply(img);
                    staBmp = img;
                    return img;
                }
                else {
                    MessageBox.Show("图片已经二值化");
                    return img;
                }
            }
            else
            {
                MessageBox.Show("请先导入一张照片或者打开摄像机");
                return null;
            }
        }
    }

}
