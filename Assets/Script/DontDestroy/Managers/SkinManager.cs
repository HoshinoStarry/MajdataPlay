﻿using MajdataPlay.Types;
using MajSimaiDecode;
using System.IO;
using UnityEngine;

public class SkinManager : MonoBehaviour
{
    public static SkinManager Instance { get; private set; }

    public Sprite SubDisplay;

    public Sprite Tap;
    public Sprite Tap_Each;
    public Sprite Tap_Break;
    public Sprite Tap_Ex;

    public Sprite Slide;
    public Sprite Slide_Each;
    public Sprite Slide_Break;
    public Sprite[] Wifi = new Sprite[11];
    public Sprite[] Wifi_Each = new Sprite[11];
    public Sprite[] Wifi_Break = new Sprite[11];

    public Sprite Star;
    public Sprite Star_Double;
    public Sprite Star_Each;
    public Sprite Star_Each_Double;
    public Sprite Star_Break;
    public Sprite Star_Break_Double;
    public Sprite Star_Ex;
    public Sprite Star_Ex_Double;

    public Sprite Hold;
    public Sprite Hold_On;
    public Sprite Hold_Off;
    public Sprite Hold_Each;
    public Sprite Hold_Each_On;
    public Sprite Hold_Ex;
    public Sprite Hold_Break;
    public Sprite Hold_Break_On;
    public Sprite HoldEnd;
    public Sprite HoldEachEnd;
    public Sprite HoldBreakEnd;


    public Sprite[] Just = new Sprite[54];
    public Sprite[] JudgeText = new Sprite[5];
    public Sprite CriticalPerfect_Break;
    public Sprite Perfect_Break;
    public Sprite FastText;
    public Sprite LateText;

    public Sprite Touch;
    public Sprite Touch_Each;
    public Sprite TouchPoint;
    public Sprite TouchPoint_Each;
    public Sprite TouchJust;
    public Sprite[] TouchBorder = new Sprite[2];
    public Sprite[] TouchBorder_Each = new Sprite[2];

    public Sprite[] TouchHold = new Sprite[5];
    public Sprite TouchHold_Off;

    public Sprite Outline;

    public Texture2D test;

    public Sprite[] TapLines;
    public Sprite[] StarLines;
    public Material BreakMaterial;
    public RuntimeAnimatorController JustBreak;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        var path = GameManager.SkinPath;
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        print(path);

        Outline = SpriteLoader.LoadSpriteFromFile(path + "/outline.png");
        SubDisplay = SpriteLoader.LoadSpriteFromFile(path + "/SubBackgourd.png");

        Tap = SpriteLoader.LoadSpriteFromFile(path + "/tap.png");
        Tap_Each = SpriteLoader.LoadSpriteFromFile(path + "/tap_each.png");
        Tap_Break = SpriteLoader.LoadSpriteFromFile(path + "/tap_break.png");
        Tap_Ex = SpriteLoader.LoadSpriteFromFile(path + "/tap_ex.png");

        Slide = SpriteLoader.LoadSpriteFromFile(path + "/slide.png");
        Slide_Each = SpriteLoader.LoadSpriteFromFile(path + "/slide_each.png");
        Slide_Break = SpriteLoader.LoadSpriteFromFile(path + "/slide_break.png");
        for (var i = 0; i < 11; i++)
        {
            Wifi[i] = SpriteLoader.LoadSpriteFromFile(path + "/wifi_" + i + ".png");
            Wifi_Each[i] = SpriteLoader.LoadSpriteFromFile(path + "/wifi_each_" + i + ".png");
            Wifi_Break[i] = SpriteLoader.LoadSpriteFromFile(path + "/wifi_break_" + i + ".png");
        }

        Star = SpriteLoader.LoadSpriteFromFile(path + "/star.png");
        Star_Double = SpriteLoader.LoadSpriteFromFile(path + "/star_double.png");
        Star_Each = SpriteLoader.LoadSpriteFromFile(path + "/star_each.png");
        Star_Each_Double = SpriteLoader.LoadSpriteFromFile(path + "/star_each_double.png");
        Star_Break = SpriteLoader.LoadSpriteFromFile(path + "/star_break.png");
        Star_Break_Double = SpriteLoader.LoadSpriteFromFile(path + "/star_break_double.png");
        Star_Ex = SpriteLoader.LoadSpriteFromFile(path + "/star_ex.png");
        Star_Ex_Double = SpriteLoader.LoadSpriteFromFile(path + "/star_ex_double.png");

        var border = new Vector4(0, 58, 0, 58);
        Hold = SpriteLoader.LoadSpriteFromFile(path + "/hold.png", border);
        Hold_Each = SpriteLoader.LoadSpriteFromFile(path + "/hold_each.png", border);
        Hold_Each_On = SpriteLoader.LoadSpriteFromFile(path + "/hold_each_on.png", border);
        Hold_Ex = SpriteLoader.LoadSpriteFromFile(path + "/hold_ex.png", border);
        Hold_Break = SpriteLoader.LoadSpriteFromFile(path + "/hold_break.png", border);
        Hold_Break_On = SpriteLoader.LoadSpriteFromFile(path + "/hold_break_on.png", border);

        if (File.Exists(Path.Combine(path, "hold_on.png")))
            Hold_On = SpriteLoader.LoadSpriteFromFile(path + "/hold_on.png", border);
        else
            Hold_On = Hold;
        Hold_Off = SpriteLoader.LoadSpriteFromFile(path + "/hold_off.png", border);
        if (File.Exists(Path.Combine(path, "hold_each_on.png")))
            Hold_Each_On = SpriteLoader.LoadSpriteFromFile(path + "/hold_each_on.png", border);
        else
            Hold_Each_On = Hold_Each;

        if (File.Exists(Path.Combine(path, "hold_break_on.png")))
            Hold_Break_On = SpriteLoader.LoadSpriteFromFile(path + "/hold_break_on.png", border);
        else
            Hold_Break_On = Hold_Break;

        // Critical Perfect

        Just[0] = SpriteLoader.LoadSpriteFromFile(path + "/just_curv_r.png");
        Just[1] = SpriteLoader.LoadSpriteFromFile(path + "/just_str_r.png");
        Just[2] = SpriteLoader.LoadSpriteFromFile(path + "/just_wifi_u.png");
        Just[3] = SpriteLoader.LoadSpriteFromFile(path + "/just_curv_l.png");
        Just[4] = SpriteLoader.LoadSpriteFromFile(path + "/just_str_l.png");
        Just[5] = SpriteLoader.LoadSpriteFromFile(path + "/just_wifi_d.png");

        // Perfect

        Just[6] = SpriteLoader.LoadSpriteFromFile(path + "/just_curv_r_p.png");
        Just[7] = SpriteLoader.LoadSpriteFromFile(path + "/just_str_r_p.png");
        Just[8] = SpriteLoader.LoadSpriteFromFile(path + "/just_wifi_u_p.png");
        Just[9] = SpriteLoader.LoadSpriteFromFile(path + "/just_curv_l_p.png");
        Just[10] = SpriteLoader.LoadSpriteFromFile(path + "/just_str_l_p.png");
        Just[11] = SpriteLoader.LoadSpriteFromFile(path + "/just_wifi_d_p.png");

        // Fast Perfect

        Just[12] = SpriteLoader.LoadSpriteFromFile(path + "/just_curv_r_fast_p.png");
        Just[13] = SpriteLoader.LoadSpriteFromFile(path + "/just_str_r_fast_p.png");
        Just[14] = SpriteLoader.LoadSpriteFromFile(path + "/just_wifi_u_fast_p.png");
        Just[15] = SpriteLoader.LoadSpriteFromFile(path + "/just_curv_l_fast_p.png");
        Just[16] = SpriteLoader.LoadSpriteFromFile(path + "/just_str_l_fast_p.png");
        Just[17] = SpriteLoader.LoadSpriteFromFile(path + "/just_wifi_d_fast_p.png");

        // Fast Great

        Just[18] = SpriteLoader.LoadSpriteFromFile(path + "/just_curv_r_fast_gr.png");
        Just[19] = SpriteLoader.LoadSpriteFromFile(path + "/just_str_r_fast_gr.png");
        Just[20] = SpriteLoader.LoadSpriteFromFile(path + "/just_wifi_u_fast_gr.png");
        Just[21] = SpriteLoader.LoadSpriteFromFile(path + "/just_curv_l_fast_gr.png");
        Just[22] = SpriteLoader.LoadSpriteFromFile(path + "/just_str_l_fast_gr.png");
        Just[23] = SpriteLoader.LoadSpriteFromFile(path + "/just_wifi_d_fast_gr.png");

        // Fast Good

        Just[24] = SpriteLoader.LoadSpriteFromFile(path + "/just_curv_r_fast_gd.png");
        Just[25] = SpriteLoader.LoadSpriteFromFile(path + "/just_str_r_fast_gd.png");
        Just[26] = SpriteLoader.LoadSpriteFromFile(path + "/just_wifi_u_fast_gd.png");
        Just[27] = SpriteLoader.LoadSpriteFromFile(path + "/just_curv_l_fast_gd.png");
        Just[28] = SpriteLoader.LoadSpriteFromFile(path + "/just_str_l_fast_gd.png");
        Just[29] = SpriteLoader.LoadSpriteFromFile(path + "/just_wifi_d_fast_gd.png");

        // Late Perfect

        Just[30] = SpriteLoader.LoadSpriteFromFile(path + "/just_curv_r_late_p.png");
        Just[31] = SpriteLoader.LoadSpriteFromFile(path + "/just_str_r_late_p.png");
        Just[32] = SpriteLoader.LoadSpriteFromFile(path + "/just_wifi_u_late_p.png");
        Just[33] = SpriteLoader.LoadSpriteFromFile(path + "/just_curv_l_late_p.png");
        Just[34] = SpriteLoader.LoadSpriteFromFile(path + "/just_str_l_late_p.png");
        Just[35] = SpriteLoader.LoadSpriteFromFile(path + "/just_wifi_d_late_p.png");
    
        // Late Great

        Just[36] = SpriteLoader.LoadSpriteFromFile(path + "/just_curv_r_late_gr.png");
        Just[37] = SpriteLoader.LoadSpriteFromFile(path + "/just_str_r_late_gr.png");
        Just[38] = SpriteLoader.LoadSpriteFromFile(path + "/just_wifi_u_late_gr.png");
        Just[39] = SpriteLoader.LoadSpriteFromFile(path + "/just_curv_l_late_gr.png");
        Just[40] = SpriteLoader.LoadSpriteFromFile(path + "/just_str_l_late_gr.png");
        Just[41] = SpriteLoader.LoadSpriteFromFile(path + "/just_wifi_d_late_gr.png");

        // Late Good

        Just[42] = SpriteLoader.LoadSpriteFromFile(path + "/just_curv_r_late_gd.png");
        Just[43] = SpriteLoader.LoadSpriteFromFile(path + "/just_str_r_late_gd.png");
        Just[44] = SpriteLoader.LoadSpriteFromFile(path + "/just_wifi_u_late_gd.png");
        Just[45] = SpriteLoader.LoadSpriteFromFile(path + "/just_curv_l_late_gd.png");
        Just[46] = SpriteLoader.LoadSpriteFromFile(path + "/just_str_l_late_gd.png");
        Just[47] = SpriteLoader.LoadSpriteFromFile(path + "/just_wifi_d_late_gd.png");
       
        // Miss

        Just[48] = SpriteLoader.LoadSpriteFromFile(path + "/miss_curv_r.png");
        Just[49] = SpriteLoader.LoadSpriteFromFile(path + "/miss_str_r.png");
        Just[50] = SpriteLoader.LoadSpriteFromFile(path + "/miss_wifi_u.png");
        Just[51] = SpriteLoader.LoadSpriteFromFile(path + "/miss_curv_l.png");
        Just[52] = SpriteLoader.LoadSpriteFromFile(path + "/miss_str_l.png");
        Just[53] = SpriteLoader.LoadSpriteFromFile(path + "/miss_wifi_d.png");

        JudgeText[0] = SpriteLoader.LoadSpriteFromFile(path + "/judge_text_miss.png");
        JudgeText[1] = SpriteLoader.LoadSpriteFromFile(path + "/judge_text_good.png");
        JudgeText[2] = SpriteLoader.LoadSpriteFromFile(path + "/judge_text_great.png");
        JudgeText[3] = SpriteLoader.LoadSpriteFromFile(path + "/judge_text_perfect.png");
        JudgeText[4] = SpriteLoader.LoadSpriteFromFile(path + "/judge_text_cPerfect.png");
        CriticalPerfect_Break = SpriteLoader.LoadSpriteFromFile(path + "/judge_text_break.png");
        Perfect_Break = SpriteLoader.LoadSpriteFromFile(path + "/judge_text_perfect_break.png");

        FastText = SpriteLoader.LoadSpriteFromFile(path + "/fast.png");
        LateText = SpriteLoader.LoadSpriteFromFile(path + "/late.png");

        Touch = SpriteLoader.LoadSpriteFromFile(path + "/touch.png");
        Touch_Each = SpriteLoader.LoadSpriteFromFile(path + "/touch_each.png");
        TouchPoint = SpriteLoader.LoadSpriteFromFile(path + "/touch_point.png");
        TouchPoint_Each = SpriteLoader.LoadSpriteFromFile(path + "/touch_point_each.png");

        TouchJust = SpriteLoader.LoadSpriteFromFile(path + "/touch_just.png");

        TouchBorder[0] = SpriteLoader.LoadSpriteFromFile(path + "/touch_border_2.png");
        TouchBorder[1] = SpriteLoader.LoadSpriteFromFile(path + "/touch_border_3.png");
        TouchBorder_Each[0] = SpriteLoader.LoadSpriteFromFile(path + "/touch_border_2_each.png");
        TouchBorder_Each[1] = SpriteLoader.LoadSpriteFromFile(path + "/touch_border_3_each.png");

        for (var i = 0; i < 4; i++) TouchHold[i] = SpriteLoader.LoadSpriteFromFile(path + "/touchhold_" + i + ".png");
        TouchHold[4] = SpriteLoader.LoadSpriteFromFile(path + "/touchhold_border.png");
        TouchHold_Off = SpriteLoader.LoadSpriteFromFile(path + "/touchhold_off.png");

        Debug.Log(test);
    }
    public JudgeTextSkin GetJudgeTextSkin()
    {
        return new()
        {
            CP_Break = CriticalPerfect_Break,
            P_Break = Perfect_Break,
            CriticalPerfect = JudgeText[4],
            Perfect = JudgeText[3],
            Great = JudgeText[2],
            Good = JudgeText[1],
            Miss = JudgeText[0],

            Fast = FastText,
            Late = LateText
        };
    }
    public TapSkin GetTapSkin()
    {
        return new()
        {
            Normal = Tap,
            Each = Tap_Each,
            Break = Tap_Break,
            Ex = Tap_Ex,

            BreakMaterial = BreakMaterial,
            NoteLines = TapLines,
            ExEffects = new Color[]
            {
                new Color(255 / 255f,172 / 255f,225 / 255f), // Pink
                new Color(255 / 255f,254 / 255f,119 / 255f), // Yellow
                new Color(255 / 255f,254 / 255f,119 / 255f), // Yellow
            }
        };
    }
    public StarSkin GetStarSkin()
    {
        return new()
        {
            Normal = Star,
            Double = Star_Double,
            Each = Star_Each,
            EachDouble = Star_Each_Double,
            Break = Star_Break,
            BreakDouble = Star_Break_Double,
            Ex = Star_Ex,
            ExDouble = Star_Ex_Double,

            BreakMaterial = BreakMaterial,
            NoteLines = StarLines,
            ExEffects = new Color[]
            {
                new Color(255 / 255f,172 / 255f,225 / 255f), // Pink
                new Color(255 / 255f,254 / 255f,119 / 255f), // Yellow
                new Color(255 / 255f,254 / 255f,119 / 255f), // Yellow
            }
        };
    }
    public HoldSkin GetHoldSkin()
    {
        return new()
        {
            Normal = Hold,
            Off = Hold_Off,
            Normal_On = Hold_On,
            Each = Hold_Each,
            Each_On = Hold_Each_On,
            Break = Hold_Break,
            Break_On = Hold_Break_On,
            Ex = Hold_Ex,

            BreakMaterial = BreakMaterial,
            NoteLines = TapLines,
            Ends = new Sprite[3]
            {
                HoldEnd,
                HoldEachEnd,
                HoldBreakEnd
            },
            ExEffects = new Color[]
            {
                new Color(255 / 255f,172 / 255f,225 / 255f), // Pink
                new Color(255 / 255f,254 / 255f,119 / 255f), // Yellow
                new Color(255 / 255f,254 / 255f,119 / 255f), // Yellow
            }
        };
    }
    public SlideSkin GetSlideSkin()
    {
        return new SlideSkin()
        {
            Star = GetStarSkin(),
            Normal = Slide,
            Each = Slide_Each,
            Break = Slide_Break,
            BreakMaterial = BreakMaterial
        };
    }
    public WifiSkin GetWifiSkin()
    {
        return new WifiSkin()
        {
            Star = GetStarSkin(),
            Normal = Wifi,
            Each = Wifi_Each,
            Break = Wifi_Break,
            BreakMaterial = BreakMaterial
        };
    }
    public TouchHoldSkin GetTouchHoldSkin()
    {
        return new TouchHoldSkin()
        {
            Fans = new Sprite[4]
            {
                TouchHold[0],
                TouchHold[1],
                TouchHold[2],
                TouchHold[3],
            },
            Boader = TouchHold[4],
            Point = TouchPoint,
            Off = TouchHold_Off
        };
    }
    public TouchSkin GetTouchSkin()
    {
        return new TouchSkin()
        {
            Normal = Touch,
            Each = Touch_Each,
            Point_Normal = TouchPoint,
            Point_Each = TouchPoint_Each,
            Border_Each = TouchBorder_Each,
            Border_Normal = TouchBorder,
            JustBorder = TouchJust
        };
    }

}