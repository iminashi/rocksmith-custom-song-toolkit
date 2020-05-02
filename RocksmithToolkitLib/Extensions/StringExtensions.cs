using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using RocksmithToolkitLib.DLCPackage;


/*
Non-Sortable Artist, Title, Album Notes:
  Diacritics, Alpha, Numeric are allowed in any case combination
  Most special characters and puncuations are allowed with a few exceptions
  
Sortable Artist, Title, Album Notes:
  ( ) are always stripped
  / is replaced with a space
  - usage is inconsistent (so for consistency remove it)
  , is stripped (in titles)
  ' is not stripped
  . and ? usage are inconsistent (so for consistency leave these)
  Abbreviations/symbols like 'Mr.' and '&' are replaced with words
  Diacritics are replaced with their ASCII approximations if available

DLC Key, and Tone Key Notes:
  Limited to a maximum length of 30 charactures, minimum of 6 charactures for uniqueness
  Only Ascii Alpha and Numeric may be used
  No spaces, no special characters, no puncuation
  All alpha lower, upper, or mixed case are allowed
  All numeric is allowed
  
Reserved XML Characters:
  Double quotes usage must be escaped ==> &quot;
  Ampersand usage must be escaped ==> &amp;
  Dash usage must be escaped if not the first/last character ==> &#8211; or use "--"
*/

// "return value;" is used to aid with debugging validation methods

namespace RocksmithToolkitLib.Extensions
{
    public static class StringExtensions
    {
        #region Class Methods

        /// <summary>
        /// Capitalize the first character without changing the rest of the string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Capitalize(this string value)
        {
            if (String.IsNullOrEmpty(value))
                return String.Empty;

            value = string.Format("{0}{1}", value.Substring(0, 1).ToUpper(), value.Substring(1));
            return value;
        }

        public static string GetValidAcronym(this string value)
        {
            if (String.IsNullOrEmpty(value))
                return String.Empty;

            var v = Regex.Split(value, @"[\W\s]+").Where(r => !string.IsNullOrEmpty(r)).ToArray();
            if (v.Length > 1)
                return string.Join(string.Empty, v.Select(s => s[0])).ToUpper();

            value = value.ReplaceDiacritics();
            value = value.StripNonAlphaNumeric();
            return value;
        }

        public static string GetValidAppIdSixDigits(this string value)
        {            
            value = value.Trim();
            
            // social engineering code
            if (value.Equals("221680"))
                throw new InvalidDataException("<WARNING> Sentinel has detected futile human resistance ..." + Environment.NewLine +
                    "Buy Cherub Rock and you wont have to mess around changing AppId's.");

            // simple six digit number validation, eg. 248750
            // can be seven digits too eg. 1089163
            if (Regex.IsMatch(value, "^([0-9]{6}|[0-9]{7})$"))
                return value;

            return "";
        }

        public static string Filter(this string value, Predicate<char> predicate)
        {
            var sb = new StringBuilder(value.Length);

            foreach (char c in value)
            {
                if (predicate(c))
                    sb.Append(c);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets a valid Artist, Title, Album (ATA) name with spaces
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetValidAtaSpaceName(this string value)
        {
            if (String.IsNullOrEmpty(value))
                return String.Empty;

            // ODLC artist, title, album character use allows these
            // allow use of accents Über ñice \\p{L} diacritics
            // allow use of unicode punctuation \\p{P\\{S} not currently implimented
            // may need to be escaped \t\n\f\r#$()*+.?[\^{|  ... '-' needs to be escaped if not at the beginning or end of regex sequence
            // allow use of only these special characters \\-_ /&.:',!?()\"#
            // allow use of alphanumerics a-zA-Z0-9
            // tested and working ... Üuber!@#$%^&*()_+=-09{}][":';<>.,?/ñice 
            const string usableCharSet = @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~¡¢£¤¥¦§¨©ª«¬®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿĞğİıĲĳŁłŒœŞşŠšŸŽžƒ–—‘’‚“”„†‡•…‰‹›⁄€℗™□△○♭♯　、。々「」『』【】〒〓〔〕〝〟ぁあぃいぅうぇえぉおかがきぎくぐけげこごさざしじすずせぜそぞただちぢっつづてでとどなにぬねのはばぱひびぴふぶぷへべぺほぼぽまみむめもゃやゅゆょよらりるれろゎわゐゑをん゛゜ゝゞァアィイゥウェエォオカガキギクグケゲコゴサザシジスズセゼソゾタダチヂッツヅテデトドナニヌネノハバパヒビピフブプヘベペホボポマミムメモャヤュユョヨラリルレロヮワヰヱヲンヴヵヶ・ーヽヾ一丁七万丈三上下不与丑且世丘丙丞両並中串丸丹主乃久之乍乎乏乗乙九乞也乱乳乾亀了予争事二云互五井亘亙些亜亡交亥亦亨享京亭亮人什仁仇今介仏仔仕他付仙仝代令以仮仰仲件任企伊伍伎伏伐休会伝伯伴伶伸伺似伽佃但位低住佐佑体何余作佳併佼使侃例侍供依侠価侭侮侯侵侶便係促俄俊俗保信俣修俳俵俸俺倉個倍倒倖候借倣値倦倫倭倶倹偉偏停健偲側偵偶偽傍傑傘備催傭債傷傾僅働像僑僕僚僧僻儀億儒償優儲允元兄充兆兇先光克免兎児党兜入全八公六共兵其具典兼内円冊再冒冗写冠冥冨冬冴冶冷凄准凋凌凍凝几凡処凧凪凱凶凸凹出函刀刃分切刈刊刑列初判別利到制刷券刺刻剃則削前剖剛剣剤剥副剰割創劃劇劉力功加劣助努劫励労効劾勃勅勇勉動勘務勝募勢勤勧勲勺勾勿匁匂包化北匙匝匠匡匪匹区医匿十千升午半卑卒卓協南単博卜占卦卯印危即却卵卸卿厄厘厚原厨厩厭厳去参又叉及友双反収叔取受叙叛叡叢口古句叩只叫召可台叱史右叶号司吃各合吉吊吋同名后吏吐向君吟吠否含吸吹吻吾呂呆呈呉告呑周呪味呼命咋和咲咳咽哀品哉員哨哩哲唄唆唇唐唖唯唱唾啄商問啓善喉喋喚喜喝喧喪喫喬喰営嗣嘆嘉嘗嘘嘩嘱噂噌噛器噴噸噺嚇嚢囚四回因団困囲図固国圃圏園土圧在圭地坂均坊坐坑坤坦坪垂型垢垣埋城埜域埠埴執培基埼堀堂堅堆堕堤堪堰報場堵堺塀塁塊塑塔塗塘塙塚塞塩填塵塾境墓増墜墨墳墾壁壇壊壌壕士壬壮声壱売壷変夏夕外夙多夜夢大天太夫央失夷奄奇奈奉奏契奔套奥奨奪奮女奴好如妃妄妊妓妖妙妥妨妬妹妻妾姉始姐姑姓委姥姦姪姫姶姻姿威娃娘娠娩娯娼婁婆婚婦婿媒媛嫁嫉嫌嫡嬉嬢嬬嬰子孔字存孜孝孟季孤学孫宅宇守安宋完宍宏宕宗官宙定宛宜宝実客宣室宥宮宰害宴宵家容宿寂寄寅密富寒寓寛寝察寡寧審寮寵寸寺対寿封専射将尉尊尋導小少尖尚尤尭就尺尻尼尽尾尿局居屈届屋屍屑展属屠屡層履屯山岐岡岨岩岬岱岳岸峠峡峨峯峰島峻崇崎崖崩嵐嵩嵯嶋嶺巌川州巡巣工左巧巨差己巳巴巷巻巽巾市布帆希帖帝帥師席帯帰帳常帽幅幌幕幡幣干平年幸幹幻幼幽幾庁広庄庇床序底庖店庚府度座庫庭庵庶康庸廃廉廊廓廟廠延廷建廻廼廿弁弄弊式弐弓弔引弗弘弛弟弥弦弧弱張強弼弾彊当形彦彩彪彫彬彰影役彼往征径待律後徐徒従得御復循微徳徴徹徽心必忌忍志忘忙応忠快念忽怒怖怜思怠急性怨怪怯恋恐恒恕恢恥恨恩恭息恰恵悉悌悔悟悠患悦悩悪悲悶悼情惇惑惚惜惟惣惨惰想惹愁愈愉意愚愛感慈態慌慎慕慢慣慧慨慮慰慶慾憂憎憐憤憧憩憲憶憾懇懐懲懸戊戎成我戒或戚戟戦戯戴戸戻房所扇扉手才打払托扮扱扶批承技抄把抑投抗折抜択披抱抵抹押抽担拍拐拒拓拘拙招拝拠拡括拭拳拶拷拾持指按挑挙挟挨挫振挺挽挿捉捌捕捗捜捧捨据捲捷捺捻掃授掌排掘掛掠採探接控推掩措掬掲掴掻揃描提揖揚換握揮援揺損搬搭携搾摂摘摩摸摺撃撒撚撞撤撫播撮撰撲撹擁操擢擦擬擾支改攻放政故敏救敗教敢散敦敬数整敵敷文斉斌斎斐斑斗料斜斡斤斥斧斬断斯新方於施旅旋族旗既日旦旧旨早旬旭旺昂昆昇昌明昏易昔星映春昧昨昭是昼時晃晋晒晦晩普景晴晶智暁暇暑暖暗暢暦暫暮暴曇曙曜曝曲曳更書曹曽曾替最月有朋服朔朕朗望朝期木未末本札朱朴机朽杉李杏材村杓杖杜束条杢来杭杯東杵杷松板枇析枕林枚果枝枠枢枯架柁柄柊柏某柑染柔柘柚柱柳柴柵査柾柿栂栃栄栓栖栗校栢株栴核根格栽桁桂桃案桐桑桓桔桜桝桟桧桶梁梅梓梗梢梧梨梯械梱梶梼棄棉棋棒棚棟森棲棺椀椅椋植椎椙椛検椴椿楊楓楕楚楠楢業楯楳極楼楽概榊榎榔榛構槌槍様槙槻槽樋樗標樟模権横樫樵樹樺樽橋橘機橡橿檀檎櫓櫛櫨欄欝欠次欣欧欲欺欽款歌歎歓止正此武歩歪歯歳歴死殆殉殊残殖殴段殺殻殿毅母毎毒比毘毛氏民気水氷永氾汀汁求汎汐汗汚汝江池汰汲決汽沃沈沌沓沖沙没沢沫河沸油治沼沿況泉泊泌法泡波泣泥注泰泳洋洗洛洞津洩洪洲活派流浄浅浜浦浩浪浬浮浴海浸消涌涙涛涜涯液涼淀淋淑淘淡淫深淳淵混添清渇済渉渋渓渚減渠渡渥渦温測港湊湖湘湛湧湯湾湿満溌源準溜溝溢溶溺滅滋滑滝滞滴漁漂漆漉漏演漕漠漢漣漫漬漸潅潔潜潟潤潮潰澄澗澱激濁濃濠濡濫濯瀕瀞瀦瀧瀬灘火灯灰灸灼災炉炊炎炭点為烈烏烹焔焚無焦然焼煉煎煙煤照煩煮煽熊熔熟熱燃燈燐燕燥燦燭爆爪爵父爺爽爾片版牌牒牙牛牝牟牡牢牧物牲特牽犀犠犬犯状狂狐狗狙狛狩独狭狸狼狽猛猟猪猫献猶猷猿獄獅獣獲玄率玉王玖玩玲珂珊珍珠珪班現球理琉琢琳琴琵琶瑚瑛瑞瑠瑳璃璧環璽瓜瓢瓦瓶甑甘甚甜生産甥用甫田由甲申男町画界畏畑畔留畜畝畠畢略畦番異畳畷畿疋疎疏疑疫疲疹疾病症痔痕痘痛痢痩痴療癌癒癖発登白百的皆皇皐皮皿盃盆盈益盗盛盟監盤目盲直相盾省眉看県真眠眺眼着睡督睦瞥瞬瞭瞳矛矢知矧矩短矯石砂研砕砥砦砧砲破砺砿硝硫硬硯硲碁碇碍碑碓碕碗碧碩確磁磐磨磯礁礎示礼社祁祇祈祉祐祖祝神祢祥票祭祷禁禄禅禍禎福禦禰禽禾禿秀私秋科秒秘租秤秦秩称移稀程税稔稗稚稜種稲稼稽稿穀穂穆積穎穏穐穣穫穴究空穿突窃窄窒窓窟窪窮窯窺竃立竜章竣童竪端競竹竺竿笈笑笛笠笥符第笹筆筈等筋筏筑筒答策箆箇箔箕算管箪箭箱箸節範篇築篠篤篭簡簸簾簿籍米籾粁粂粉粋粍粒粕粗粘粛粟粥粧精糊糎糖糞糟糠糧糸系糾紀約紅紋納紐純紗紘紙級紛素紡索紫紬累細紳紹紺終絃組絆経結絞絡絢給統絵絶絹継続綜綬維綱網綴綺綻綾綿緊緋総緑緒線締編緩緬緯練緻縁縄縛縞縦縫縮績繁繊繋繍織繕繭繰纂纏缶罪罫置罰署罵罷羅羊美群羨義羽翁翌習翠翫翰翻翼耀老考者而耐耕耗耳耶耽聖聞聡聯聴職聾肇肉肋肌肖肘肝股肢肥肩肪肯肱育肴肺胃胆背胎胞胡胤胴胸能脂脅脆脇脈脊脚脱脳脹腎腐腔腕腫腰腸腹腺腿膏膚膜膝膨膳膿臆臓臣臥臨自臭至致臼興舌舎舗舘舛舜舞舟航般舵舶舷船艇艦艮良色艶芋芙芝芥芦芭芯花芳芸芹芽苅苑苓苔苗苛若苦苧苫英茂茄茅茎茜茨茶茸草荊荏荒荘荷荻莞莫莱菅菊菌菓菖菜菟菩華菰菱萄萌萎萩萱落葉葎著葛葡董葦葬葱葵葺蒋蒐蒔蒙蒜蒲蒸蒼蓄蓉蓋蓑蓬蓮蔀蔑蔓蔚蔦蔭蔵蔽蕃蕉蕊蕎蕗蕨蕩蕪薄薗薙薦薩薪薫薬薮薯藁藍藤藩藷藻蘇蘭虎虐虚虜虞虫虹虻蚊蚕蚤蛇蛋蛍蛎蛙蛤蛭蛮蛸蛾蜂蜘蜜蝉蝋蝕蝦蝶蝿融螺蟹蟻血衆行術街衛衝衡衣表衰衷衿袈袋袖被袴袷裁裂装裏裕補裟裡裳裸製裾複褐褒襖襟襲西要覆覇見規視覗覚覧親観角解触言訂計訊討訓託記訟訣訪設許訳訴診註証詐詑詔評詞詠詣試詩詫詮詰話該詳誇誉誌認誓誕誘語誠誤説読誰課誹誼調談請諌諏諒論諜諦諭諮諸諺諾謀謁謂謄謎謙講謝謡謬謹識譜警議譲護讃讐谷豆豊豚象豪豹貌貝貞負財貢貧貨販貪貫責貯貰貴買貸費貼貿賀賂賃賄資賊賎賑賓賛賜賞賠賢賦質賭購贈贋赤赦赫走赴起超越趣趨足距跡跨路跳践踊踏蹄蹟蹴躍身躯車軌軍軒軟転軸軽較載輔輝輩輪輯輸輿轄轍轟轡辛辞辰辱農辺辻込辿迂迄迅迎近返迦迩迫迭述迷追退送逃逆透逐逓途逗這通逝速造逢連逮週進逸逼遁遂遅遇遊運遍過道達違遜遠遡遣遥適遭遮遵遷選遺遼避還邑那邦邪邸郁郊郎郡部郭郵郷都鄭酉酋酌配酎酒酔酢酪酬酵酷酸醇醍醐醒醗醜醤醸釆采釈里重野量金釘釜針釣釦釧鈍鈎鈴鈷鉄鉛鉢鉦鉱鉾銀銃銅銑銘銚銭鋒鋤鋪鋭鋲鋳鋸鋼錆錐錘錠錦錨錫錬錯録鍋鍍鍔鍛鍬鍵鍾鎌鎖鎗鎚鎧鎮鏑鏡鐘鐙鐸鑑鑓長門閃閉開閏閑間関閣閤閥閲闇闘阜阪防阻阿陀附降限陛院陣除陥陪陰陳陵陶陸険陽隅隆隈隊階随隔隙際障隠隣隷隻隼雀雁雄雅集雇雌雑雛離難雨雪雫雰雲零雷電需震霊霜霞霧露青靖静非面革靭靴鞄鞍鞘鞠鞭韓韮音韻響頁頂頃項順須預頑頒頓頗領頚頬頭頴頻頼題額顎顔顕願顛類顧風飛食飢飯飲飴飼飽飾餅養餌餐餓館饗首香馨馬馳馴駁駄駅駆駈駐駒駕駿騎騒験騨騰驚骨骸髄高髪髭鬼魁魂魅魔魚魯鮎鮒鮪鮫鮭鮮鯉鯖鯛鯨鯵鰍鰐鰭鰯鰹鰻鱈鱒鱗鳥鳩鳳鳴鳶鴇鴎鴛鴨鴫鴬鴻鵜鵠鵡鵬鶏鶴鷲鷹鷺鹸鹿麓麗麟麦麹麺麻麿黄黍黒黙黛鼎鼓鼠鼻齢龍！＃＄％＆（）＊＋，－．／？｡｢｣､･ﾞﾟ￥";

            return value.Filter(c => usableCharSet.IndexOf(c) >= 0);
        }

        public static string GetValidToneName(this string value)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9_]");
            return rgx.Replace(value, "");
        }

        public static string GetValidFileName(this string fileName)
        {
            fileName = fileName.Replace(",", ""); // remove commas even though valid
            fileName = fileName.StripExcessWhiteSpace();
            fileName = String.Concat(fileName.Split(Path.GetInvalidFileNameChars()));
            return fileName;
        }

        public static string GetValidFilePath(this string value)
        {
            if (String.IsNullOrEmpty(value))
                return String.Empty;

            var fileName = Path.GetFileName(value);
            var pathName = Path.GetDirectoryName(value);
            fileName = fileName.GetValidFileName();
            pathName = pathName.GetValidPathName();
            value = Path.Combine(pathName, fileName);
            return value;
        }

        public static string GetValidInlayName(this string value, bool frets24 = false)
        {
            if (String.IsNullOrEmpty(value))
                return String.Empty;

            // remove all special characters, and leading numbers and replace spaces with underscore
            Regex rgx = new Regex("[^a-zA-Z0-9]_ ");
            value = rgx.Replace(value, "");
            value = value.StripLeadingNumbers();
            value = value.StripLeadingSpecialCharacters();

            // make sure (24) fret appears in the proper placement
            if (frets24)
            {
                if (value.Contains("24"))
                {
                    value = value.Replace("_24_", "_");
                    value = value.Replace("_24", "");
                    value = value.Replace("24_", "");
                    value = value.Replace(" 24 ", " ");
                    value = value.Replace("24 ", " ");
                    value = value.Replace(" 24", " ");
                    value = value.Replace("24", "");
                }
                value = value.Trim() + " 24";
            }

            value = value.ReplaceSpaceWith("_");
            return value;
        }

        /// <summary>
        /// Format string as valid DLCKey (aka SongKey), or ToneKey
        /// <para>CRITICAL: Provide 'songTitle' to prevent RS1 in-game hanging after tuning</para>
        /// </summary>
        /// <param name="value">DLCKey or ToneKey for verification</param>
        /// <param name="songTitle">optional SongTitle varification comparison for DLCKey </param>
        /// <param name="isTone">If set to <c>true</c> validate tone name and tone key</param>
        /// <returns>contains no spaces, no accents, or special characters but can begin with or be all numbers or all lower case</returns>
        public static string GetValidKey(this string value, string songTitle = "", bool isTone = false)
        {
            if (String.IsNullOrEmpty(value))
                return String.Empty;

            value = value.StripNonAlphaNumeric();

            // CRITICAL: prevents RS1 in game hanging after tuning
            // check if same, if so then add 'Song' to make key unique, skip check if isTone
            if (value == songTitle.StripNonAlphaNumeric() && !isTone)
                value = "Song" + value;

            // limit max Key length to 30
            value = value.Substring(0, Math.Min(30, value.Length)).Trim();

            // ensure min DLCKey length is 6, skip check if isTone
            if (value.Length < 7 && !isTone)
            {
                value = string.Concat(Enumerable.Repeat(value, 6));
                value = value.Substring(0, 6);
            }

            return value;
        }

        /// <summary>
        /// Validate lyric character usage
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetValidLyric(this string value)
        {
            // standard ODLC valid lyric character set, i.e., ã can not be used (confirmed by testing)
            //!"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_abcdefghijklmnopqrstuvwxyz{|}~¡¢¥¦§¨ª«°²³´•¸¹º»¼½¾¿ÀÁÂÄÅÆÇÈÉÊËÌÎÏÑÒÓÔÖØÙÚÛÜÞßàáâäåæçèéêëìíîïñòóôöøùúûüŒœŠšž„…€™␀★➨
            string validSpecialCharacters = " !\"#$%&'()*+,-./:;<=>?@[\\]^_{|}~¡¢¥¦§¨ª«°²³´•¸¹º»¼½¾¿ÀÁÂÄÅÆÇÈÉÊËÌÎÏÑÒÓÔÖØÙÚÛÜÞßàáâäåæçèéêëìíîïñòóôöøùúûüŒœŠšž€™␀★➨";
            string validAlphaNumerics = "a-zA-Z0-9";

            Regex rgx = new Regex("[^" + validAlphaNumerics + validSpecialCharacters + "]*");
            return rgx.Replace(value, "");
        }

        public static string GetValidPathName(this string pathName)
        {
            pathName = String.Concat(pathName.Split(Path.GetInvalidPathChars()));
            return pathName;
        }

        /// <summary>
        /// Standard short file name format for CDLC file names "{0}_{1}_{2}"
        /// </summary>
        /// <param name="stringFormat"></param>
        /// <param name="artist"></param>
        /// <param name="title"></param>
        /// <param name="version"></param>
        /// <param name="acronym">use artist acronym instead of full artist name</param>
        /// <returns></returns>
        public static string GetValidShortFileName(string artist, string title, string version, bool acronym = false)
        {
            if (String.IsNullOrEmpty(artist) || String.IsNullOrEmpty(title) || String.IsNullOrEmpty(version))
                throw new DataException("Artist, title, or version field is null or empty ...");

            // cleanup version numbering
            version = version.Replace(".", "_");

            string value;
            if (!acronym)
                value = String.Format("{0}_{1}_{2}", artist.GetValidAtaSpaceName(), title.GetValidAtaSpaceName(), version).Replace(" ", "-");
            else
                value = String.Format("{0}_{1}_{2}", artist.GetValidAcronym(), title.GetValidAtaSpaceName(), version).Replace(" ", "-");

            value = value.GetValidFileName().StripExcessWhiteSpace();
            return value;
        }

        public static string GetValidSortableName(this string value)
        {
            if (String.IsNullOrEmpty(value))
                return String.Empty;

            // processing order is important to achieve output like ODLC
            //value = value.ReplaceAbbreviations();
            //value = value.ReplaceDiacritics();
            //value = value.StripSpecialCharacters();
            //value = value.StripLeadingSpecialCharacters();
            Regex rgx = new Regex("^[^A-Za-z0-9]*");
            value = rgx.Replace(value, "");
            //value = value.ShortWordMover(); // "The Beatles" becomes "Beatles, The"
            //value = value.Capitalize(); // "blink-182" becomes "Blink 182"
            value = value.StripExcessWhiteSpace();

            return value;
        }

        public static string GetValidTempo(this string value)
        {
            float tempo = 0;
            float.TryParse(value.Trim(), out tempo);
            int bpm = (int)Math.Round(tempo);
            // check for valid tempo
            if (bpm > 0 && bpm < 999)  // allow insane tempo
                return bpm.ToString();

            // return "120"; // do not use a default tempo as this causes problems elsewhere
            // force user to make entry rather than defaulting
            return "";
        }

        public static string GetValidVersion(this string value)
        {
            if (String.IsNullOrEmpty(value))
                return String.Empty;

            Regex rgx = new Regex(@"^[\d\.]*");
            var match = rgx.Match(value);
            if (match.Success)
                return match.Value.Trim();

            // force user to make entry rather than defaulting
            return "";
        }

        public static bool IsVolumeValid(this float? value)
        {
            if (value == null)
                return false;

            // check for valid volume
            float volume = (float)Math.Round((double)value, 1);
            if (volume >= -45.0F && volume <= 45.0F)
                return true;

            return false;
        }

        public static float GetValidVolume(this float? value, float defaultVolume = -7.0F)
        {
            if (value == null)
                return defaultVolume;

            // check for valid volume
            float volume = (float)Math.Round((double)value, 1);
            if (volume >= -45.0F && volume <= 45.0F)
                return volume;

            // use default volume
            return defaultVolume;
        }

        public static string GetValidYear(this string value)
        {
            if (String.IsNullOrEmpty(value))
                return String.Empty;

            // check for valid four digit song year 
            if (!Regex.IsMatch(value, "^(15[0-9][0-9]|16[0-9][0-9]|17[0-9][0-9]|18[0-9][0-9]|19[0-9][0-9]|20[0-3][0-9])"))
                value = ""; // clear if not valid

            return value;
        }

        public static bool IsAppIdSixDigits(this string value)
        {
            if (String.IsNullOrEmpty(GetValidAppIdSixDigits(value)))
                return false;

            return true;
        }

        public static bool IsFilePathLengthValid(this string filePath)
        {
            if (Environment.OSVersion.Version.Major >= 6 && filePath.Length > 260)
                return false;

            if (Environment.OSVersion.Version.Major < 6 && filePath.Length > 215)
                return false;

            return true;
        }

        public static bool IsFilePathNameValid(this string filePath)
        {
            try
            {
                // check if filePath is null or empty
                if (String.IsNullOrEmpty(filePath))
                    return false;

                // check drive is valid
                var pathRoot = Path.GetPathRoot(filePath);
                if (!Directory.GetLogicalDrives().Contains(pathRoot))
                    return false;

                var fileName = Path.GetFileName(filePath);
                if (String.IsNullOrEmpty(fileName))
                    return false;

                var dirName = Path.GetDirectoryName(filePath);
                if (String.IsNullOrEmpty(dirName))
                    return false;

                if (dirName.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                    return false;

                if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                    return false;
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }

        public static bool IsFilePathValid(this string filePath)
        {
            if (filePath.IsFilePathLengthValid())
                if (filePath.IsFilePathNameValid())
                    return true;

            return false;
        }

        [Obsolete("Deprecated, please use appropriate StringExtension methods.", true)]
        public static string ObsoleteGetValidName(this string value, bool allowSpace = false, bool allowStartsWithNumber = false, bool underscoreSpace = false, bool frets24 = false)
        {
            // TODO: allow some additonal special charaters but not many

            // valid characters developed from actually reviewing ODLC artist, title, album names
            string name = String.Empty;

            if (!String.IsNullOrEmpty(value))
            {
                // ODLC artist, title, album character use allows these but not these
                // allow use of accents Über ñice \\p{L}
                // allow use of unicode punctuation \\p{P\\{S} not currently implimented
                // may need to be escaped \t\n\f\r#$()*+.?[\^{|  ... '-' needs to be escaped if not at the beginning or end of regex sequence
                // allow use of only these special characters \\-_ /&.:',!?()\"#
                // allow use of alphanumerics a-zA-Z0-9
                // tested and working ... Üuber!@#$%^&*()_+=-09{}][":';<>.,?/ñice

                Regex rgx = new Regex((allowSpace) ? "[^a-zA-Z0-9\\-_ /&.:',!?()\"#\\p{L}]" : "[^a-zA-Z0-9\\-_/&.:',!?()\"#\\p{L} ]");
                name = rgx.Replace(value, "");

                Regex rgx2 = new Regex(@"^[\d]*\s*");
                if (!allowStartsWithNumber)
                    name = rgx2.Replace(name, "");

                // prevent names from starting with special characters -_* etc
                Regex rgx3 = new Regex("^[^A-Za-z0-9]*");
                name = rgx3.Replace(name, "");

                if (frets24)
                {
                    if (name.Contains("24"))
                    {
                        name = name.Replace("_24_", "_");
                        name = name.Replace("_24", "");
                        name = name.Replace("24_", "");
                        name = name.Replace(" 24 ", " ");
                        name = name.Replace("24 ", " ");
                        name = name.Replace(" 24", " ");
                        name = name.Replace("24", "");
                    }
                    name = name.Trim() + " 24";
                }

                if (underscoreSpace)
                    name = name.Replace(" ", "_");
            }

            return name.Trim();
        }

        public static string ReplaceAbbreviations(this string value)
        {
            if (String.IsNullOrEmpty(value))
                return String.Empty;

            // this does a better job of replacing diacretics and special characters
            value = value.Replace(" & ", " and ");
            value = value.Replace("&", " and ");
            value = value.Replace("/", " ");
            value = value.Replace("-", " "); // inconsistent usage sometimes removed, sometimes replaced
            value = value.Replace(" + ", " plus ");
            value = value.Replace("+", " plus ");
            value = value.Replace(" @ ", " at ");
            value = value.Replace("@", " at ");
            value = value.Replace("Mr.", "Mister");
            value = value.Replace("Mrs.", "Misses");
            value = value.Replace("Ms.", "Miss");
            value = value.Replace("Jr.", "Junior");

            return value;
        }

        public static string ReplaceDiacritics(this string value)
        {
            if (String.IsNullOrEmpty(value))
                return String.Empty;

            value = Regex.Replace(value, "[ÀÁÂÃÅÄĀĂĄǍǺ]", "A");
            value = Regex.Replace(value, "[ǻǎàáâãäåąāă]", "a");
            value = Regex.Replace(value, "[ÇĆĈĊČ]", "C");
            value = Regex.Replace(value, "[çčćĉċ]", "c");
            value = Regex.Replace(value, "[ĎĐ]", "D");
            value = Regex.Replace(value, "[ďđ]", "d");
            value = Regex.Replace(value, "[ÈÉÊËĒĔĖĘĚ]", "E");
            value = Regex.Replace(value, "[ěèéêëēĕėę]", "e");
            value = Regex.Replace(value, "[ĜĞĠĢ]", "G");
            value = Regex.Replace(value, "[ģĝğġ]", "g");
            value = Regex.Replace(value, "[Ĥ]", "H");
            value = Regex.Replace(value, "[ĥ]", "h");
            value = Regex.Replace(value, "[ÌÍÎÏĨĪĬĮİǏ]", "I");
            value = Regex.Replace(value, "[ǐıįĭīĩìíîï]", "i");
            value = Regex.Replace(value, "[Ĵ]", "J");
            value = Regex.Replace(value, "[ĵ]", "j");
            value = Regex.Replace(value, "[Ķ]", "K");
            value = Regex.Replace(value, "[ķĸ]", "k");
            value = Regex.Replace(value, "[ĹĻĽĿŁ]", "L");
            value = Regex.Replace(value, "[ŀľļĺł]", "l");
            value = Regex.Replace(value, "[ÑŃŅŇŊ]", "N");
            value = Regex.Replace(value, "[ñńņňŉŋ]", "n");
            value = Regex.Replace(value, "[ÒÓÔÖÕŌŎŐƠǑǾ]", "O");
            value = Regex.Replace(value, "[ǿǒơòóôõöøōŏő]", "o");
            value = Regex.Replace(value, "[ŔŖŘ]", "R");
            value = Regex.Replace(value, "[ŗŕř]", "r");
            value = Regex.Replace(value, "[ŚŜŞŠ]", "S");
            value = Regex.Replace(value, "[şŝśš]", "s");
            value = Regex.Replace(value, "[ŢŤ]", "T");
            value = Regex.Replace(value, "[ťţ]", "t");
            value = Regex.Replace(value, "[ÙÚÛÜŨŪŬŮŰŲƯǓǕǗǙǛ]", "U");
            value = Regex.Replace(value, "[ǜǚǘǖǔưũùúûūŭůűų]", "u");
            value = Regex.Replace(value, "[Ŵ]", "W");
            value = Regex.Replace(value, "[ŵ]", "w");
            value = Regex.Replace(value, "[ÝŶŸ]", "Y");
            value = Regex.Replace(value, "[ýÿŷ]", "y");
            value = Regex.Replace(value, "[ŹŻŽ]", "Z");
            value = Regex.Replace(value, "[žźż]", "z");
            value = Regex.Replace(value, "[œ]", "oe");
            value = Regex.Replace(value, "[Œ]", "Oe");
            value = Regex.Replace(value, "[°]", "o");
            value = Regex.Replace(value, "[¡]", "!");
            value = Regex.Replace(value, "[¿]", "?");
            value = Regex.Replace(value, "[«»\u201C\u201D\u201E\u201F\u2033\u2036]", "\"");
            value = Regex.Replace(value, "[\u2026]", "...");

            return value;
        }

        public static string ReplaceDiacriticsFast(this string value)
        {
            // this does a good quick job of replacing diacretics
            // using "ISO-8859-8" gives better results than ""ISO-8859-1"
            byte[] byteOuput = Encoding.GetEncoding("ISO-8859-8").GetBytes(value);
            var result = Encoding.GetEncoding("ISO-8859-8").GetString(byteOuput);
            return result;
        }

        /// <summary>
        /// Replace white space with user choice of replacement or remove all together
        /// </summary>
        /// <param name="value"></param>
        /// <param name="replacementValue">Default is underscore</param>
        /// <returns></returns>
        public static string ReplaceSpaceWith(this string value, string replacementValue = "_")
        {
            var result = Regex.Replace(value.Trim(), @"[\s]", replacementValue);
            return result;
        }


        public static string ReplaceSpecialCharacters(this string value)
        {
            // tilde not used in ODLC
            var result = value.Replace('~', '-');
            return result;
        }

        public static string RestoreCRLF(this string value)
        {
            // replace single lf with crlf
            return Regex.Replace(value, @"\r\n?|\n", "\r\n");
        }

        /// <summary>
        /// Moves short words like "The " from the begining of a string to the end ", The" 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="undoIt">Use to undo ShortWordMover strings</param>
        /// <returns></returns>
        public static string ShortWordMover(this string value, bool undoIt = false)
        {
            if (String.IsNullOrEmpty(value))
                return String.Empty;

            // Artist Sort may begin with "A ", e.g. 'A Flock of Seaguls'
            var shortWord = new string[] { "The ", "THE ", "the " };
            var newEnding = new string[] { ", The", ", THE", ", the" };

            for (int i = 0; i < shortWord.Length; i++)
            {
                if (undoIt)
                {
                    if (value.EndsWith(newEnding[i], StringComparison.InvariantCulture))
                        value = String.Format("{0}{1}", shortWord[i], value.Substring(0, value.Length - newEnding[i].Length - 1)).Trim();
                }
                else
                {
                    if (value.StartsWith(shortWord[i], StringComparison.InvariantCulture))
                        value = String.Format("{0}{1}", value.Substring(shortWord[i].Length, value.Length - shortWord[i].Length), newEnding[i]).Trim();
                }
            }

            return value;
        }

        public static string StripCRLF(this string value, string replacement = "")
        {
            // replace single lf and/or crlf
            return Regex.Replace(value, @"\r\n?|\n", replacement);
        }

        public static string StripDiacritics(this string value)
        {
            // test string = "áéíóúç";
            var result = Regex.Replace(value.Normalize(NormalizationForm.FormD), "[^A-Za-z| ]", String.Empty);

            return result;
        }

        public static string StripExcessWhiteSpace(this string value)
        {
            Regex rgx = new Regex("[ ]{2,}", RegexOptions.None);
            var result = rgx.Replace(value, " ");

            return result;
        }

        /// <summary>
        /// Strips non-printable characters and returns valid UTF8 XML
        /// </summary>
        public static Stream StripIllegalXMLChars(this string value)
        {
            const string pattern = @"[\x01-\x08\x0B-\x0C\x0E-\x1F\x7F-\x84\x86-\x9F]"; // XML1.1
            value = Regex.Replace(value, pattern, "", RegexOptions.IgnoreCase);

            return new MemoryStream(new UTF8Encoding(false).GetBytes(value));
        }


        public static string StripLeadingNumbers(this string value)
        {
            Regex rgx = new Regex(@"^[\d]*\s*");
            var result = rgx.Replace(value, "");
            return result;
        }

        public static string StripLeadingSpecialCharacters(this string value)
        {
            if (String.IsNullOrEmpty(value))
                return String.Empty;

            Regex rgx = new Regex("^[^A-Za-z0-9(]*");
            var result = rgx.Replace(value, "");
            return result;
        }

        /// <summary>
        /// removes all non alphanumeric and all white space
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string StripNonAlphaNumeric(this string value)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9_]+");
            var result = rgx.Replace(value, "");
            return result;
        }

        public static string StripSpecialCharacters(this string value)
        {
            // TEST ()½!$€£Test$€£()½!  ()½!Test()½!
            // value = Regex.Replace(value, "[`~#\\$€£*',.;:!?()[]\"{}/]", "");
            Regex rgx = new Regex("[^a-zA-Z0-9 _#:'.]"); // only these are acceptable
            var result = rgx.Replace(value, "");
            return result;
        }

        public static string ToNullTerminatedAscii(this Byte[] bytes)
        {
            var result = Encoding.ASCII.GetString(bytes).TrimEnd('\0');
            return result;
        }

        public static string ToNullTerminatedUTF8(this Byte[] bytes)
        {
            var result = Encoding.UTF8.GetString(bytes).TrimEnd('\0');
            return result;
        }

        public static string GetStringInBetween(this string strSource, string strBegin, string strEnd)
        {
            string result = "";
            int iIndexOfBegin = strSource.IndexOf(strBegin);
            if (iIndexOfBegin != -1)
            {
                strSource = strSource.Substring(iIndexOfBegin + strBegin.Length);
                int iEnd = strSource.IndexOf(strEnd);
                if (iEnd != -1)
                {
                    result = strSource.Substring(0, iEnd);
                }
            }
            return result;
        }

        /// <summary>
        /// Splits a text string so that it wraps to specified line length
        /// </summary>
        /// <param name="inputText"></param>
        /// <param name="lineLength"></param>
        /// <param name="splitOnSpace"></param>
        /// <returns></returns>
        public static string SplitString(string inputText, int lineLength, bool splitOnSpace = true)
        {
            var finalString = String.Empty;

            if (splitOnSpace)
            {
                var delimiters = new[] { " " }; // , "\\" };
                var stringSplit = inputText.Split(delimiters, StringSplitOptions.None);
                var charCounter = 0;

                for (int i = 0; i < stringSplit.Length; i++)
                {
                    finalString += stringSplit[i] + " ";
                    charCounter += stringSplit[i].Length;

                    if (charCounter > lineLength)
                    {
                        finalString += Environment.NewLine;
                        charCounter = 0;
                    }
                }
            }
            else
            {
                for (int i = 0; i < inputText.Length; i += lineLength)
                {
                    if (i + lineLength > inputText.Length)
                        lineLength = inputText.Length - i;

                    finalString += inputText.Substring(i, lineLength) + Environment.NewLine;
                }
                finalString = finalString.TrimEnd(Environment.NewLine.ToCharArray());
            }

            return finalString;
        }

        /// <summary>
        /// Split contiguous string on caps and insert spaces
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string SplitCamelCase(string source)
        {
            string[] resultArray = Regex.Split(source, @"(?<!^)(?=[A-Z])");
            var result = String.Join(" ", resultArray).Trim();

            return result;
        }

        #endregion
    }
}
