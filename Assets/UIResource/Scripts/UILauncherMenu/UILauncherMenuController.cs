/**
 * @file UILauncherMenuController.cs
 * @brief ランチャーメニューの各メニューをコントロール
 * @author Ryota Shiroguchi
 * @date 2016-12-08
 */

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;


/**
 * @class UILauncherMenuController
 * @brief ランチャーメニューの各メニューをコントロール
 * @todo 状態管理の列挙型を使って行うように改修する
 */
public class UILauncherMenuController : MonoBehaviour
{


    //================================================================================
    //初期設定関連
    //================================================================================
    [SerializeField]
    private Canvas m_launcherMenuCanvas;                                          /**< ランチャーメニューのCanvas */
    [SerializeField]
    private GameObject m_menuParentOG;                                            /**< メニューの親オブジェクトを格納 */
    [SerializeField]
    private List<UILauncherMenu> m_launcherMenuList = new List<UILauncherMenu>(); /**< メニューのUILauncherMenuクラスを格納するList */
    [SerializeField]
    private GameObject m_overlayGO;                                               /**< オーバーレイのGameObjectの格納 */

    /*< 30.0d, 70.0d, 110.0d, 150.0d */
    private const double m_menuBaseDegree =  30.0d;                               /**< メニューの基準角度 */
    private const double m_menuBaseAddDegree =  40.0d;                            /**< メニュー作成で基準角度に加算する角度 */
    private List<double> m_menuObjDegreeList = new List<double>();                /**< メニューを配置するための角度 パターンのリスト */
    private List<double> m_menuReverseObjDegreeList = new List<double>();         /**< メニューを配置するための角度 パターンのリスト */
    [SerializeField]
    private float m_menuRadius = 150.0f;                                          /**< メニューの弧の円周の半径 */
    [SerializeField]
    private float m_menuStartWaitingTime = 1.0f;                                  /**< メニューの起動までの待機時間 */


    //================================================================================
    //メニューを配置する座標のリスト
    //================================================================================
    private List<Vector2> m_currentPosList = new List<Vector2>();                /**< 利用する配置リスト */
    //TODO:辞書にできないか？
    private List<Vector2> m_menuObjPosListLeft = new List<Vector2>();            /**< 通常左 */
    private List<Vector2> m_menuObjPosListCenter = new List<Vector2>();          /**< 通常中央 */
    private List<Vector2> m_menuObjPosListRight = new List<Vector2>();           /**< 通常右 */
    private List<Vector2> m_menuObjPosListReverseLeft = new List<Vector2>();     /**< 反転左 */
    private List<Vector2> m_menuObjPosListRnversionCenter = new List<Vector2>(); /**< 反転中央 */
    private List<Vector2> m_menuObjPosListRnversionRight = new List<Vector2>();  /**< 反転右 */


    //================================================================================
    //メニュー配置用に加算する角度の定数
    //================================================================================
    private const double addDegreeLeft = -20.0d;            /**< 左端 */
    private const double addDegreeRight = 20.0d;            /**< 右端 */
    private const double addDegreeReverseLeft = -120.0d;    /**< 左端 反転 */
    private const double addDegreeRnversionCenter = 180.0d; /**< 中央 反転 */
    private const double addRnversionRight = 120.0d;        /**< 右端 反転 */

    //================================================================================
    //ランチャー 座標関連
    //================================================================================
    private Vector2 m_screenBasePos;        /**< ランチャーを表示する基点のスクリーン座標 */
    private Vector2 m_menuBasePos;          /**< ランチャーを表示する基点のメニューの座標 */
    private Vector2 m_launcherMenuMaximum;  /**< ランチャーメニューの表示領域のサイズ */
    private Vector2 m_launcherMenuSize;     /**< メニューのサイズ */
    private Vector2 m_previousDragPos;      /**< ランチャー実行中の前回の座標 */
    private Vector2 m_menuParentDefaultPos; /**< ランチャーの実行前の座標 */

    //================================================================================
    //ランチャーの状態関連
    //================================================================================

    private bool m_isExecutable = true;     /**< ランチャーを実行可能かどうか 通常はTrue*/
    public bool IsExecutable
    {
        get
        {
            return m_isExecutable;
        }
    }

    private bool m_isDisplaying = false;     /**< ランチャーが表示中かどうか */
    public bool IsDisplaying
    {
        get
        {
            return m_isDisplaying;
        }
    }

    private bool m_isInWaiting = false;      /**< ランチャーが待機中かどうか */
    public bool IsInWaiting
    {
        get
        {
            return m_isInWaiting;
        }
    }


    private bool m_isOtherProcess = true;    /**< タッチイベントで他の処理と被る場合に他を許可するか */
    public bool IsOtherProcess
    {
        get
        {
            return m_isOtherProcess;
        }
    }

    private bool m_isDegreesMatched = false; /**< 個別のメニューと角が一致しているか */

    //================================================================================
    //ランチャー起動時の個別メニュー関連
    //================================================================================
    private float m_menuBaseToMaxDistance;     /**< メニューの基点からの最大距離 */
    private float m_menuStartDistance;         /**< メニューの開始時の移動量*/
    private float m_menuActiveStartDistance;   /**< メニューが有効になる開始の距離 */
    private float m_menuActiveEndDistance;     /**< メニューが有効になる終了の距離 */
    private int m_menuActiveNunmber;           /**< 有効になってるメニューのindex番号 */
    private float m_addDistance;


    [SerializeField]
    private float m_menuAddDistance = 30.0f;   /**< メニューの距離に加算する量 */
    private float m_degreeScope = 10.0f;       /**<メニューを有効にすドラッグの度の範囲*/

    //================================================================================
    //その他
    //================================================================================
    private int m_menuCount;         /**< メニューの数 */
    public int MenuCount
    {
        get
        {
            return m_menuCount;
        }
    }
    private float m_screenDiameter;  /**< スクリーンサイズに対する倍率 */
    private IEnumerator m_coroutine; /**< コールチンを設定 */
    private launcherMenuState m_launcherMenuState = launcherMenuState.BeforeCountDown; /**< ランチャーのメニューの状態 */
    private UIFade m_overlayFade;    /**< オーバーレイのFadeのクラスを格納 TODO:フェードがシェーダーに変わるので不要になる予定*/

    /**
     * @enum launcherMenuState
     * @brief ランチャーのメニューの状態の管理に利用する列挙型
     */
    private enum launcherMenuState
    {
        BeforeCountDown,
        CountDown,
        Executing
    }


    void Awake()
    {
        initialization();
        
    }

    /**
     * 初期化
     * @brief フィールドの初期化
     *
     */
    private void initialization()
    {

        float canvasWidth = m_launcherMenuCanvas.GetComponent<CanvasScaler>().referenceResolution.x;
        float canvasHeight = m_launcherMenuCanvas.GetComponent<CanvasScaler>().referenceResolution.y;
    
        float menuWidth = m_launcherMenuList[0].GetComponent<RectTransform>().sizeDelta.x;
        float menuHeight = m_launcherMenuList[0].GetComponent<RectTransform>().sizeDelta.y;

        //ランチャーメニューの表示領域 = Canvasの横幅と縦幅
        m_launcherMenuMaximum = new Vector2 (canvasWidth, canvasHeight);
        //メニューの個別の横幅 縦幅
        m_launcherMenuSize = new Vector2 (menuWidth, menuHeight);
        //表示の比率を指定 Canvasの横幅 / スクリーンの横幅
        m_screenDiameter = m_launcherMenuMaximum.x / Screen.width;
        Debug.Log("m_screenDiameter:" + m_screenDiameter);
        //メニューの数を取得
        m_menuCount = m_launcherMenuList.Count;
        //オーバーレイのFadeのクラスを格納
        m_overlayFade = m_overlayGO.GetComponent<UIFade>();
        //メニューの初期の位置を取得
        m_menuParentDefaultPos = m_menuParentOG.GetComponent<RectTransform>().anchoredPosition;


    }


    /**
     *  Setup
     * @brief ランチャーの各メニューの設定処理
     * @param[in] menuCount 表示するメニューの数
     * @param[in] setMenuActionList 表示するメニューのアクションのList
     * @param[in] setNormalSpriteList 表示するメニューの通常状態の画像のList
     * @param[in] setActiveSpriteList 表示するメニューの有効状態の画像のList
     * @param[in] setIconSpriteList 表示するメニューのアイコンの画像のList
     */
    public void Setup(float menuStartWaitingTime, int menuCount, List<UnityAction> setMenuActionList,  List<Sprite> setNormalSpriteList, List<Sprite> setActiveSpriteList, List<Sprite> setIconSpriteList)
    {
        //メニューの表示までの待機時間
        m_menuStartWaitingTime = menuStartWaitingTime;
        //リストの初期化
        m_menuObjDegreeList.Clear();
        m_menuReverseObjDegreeList.Clear();
        m_menuObjPosListLeft.Clear();
        m_menuObjPosListCenter.Clear();
        m_menuObjPosListRight.Clear();
        m_menuObjPosListReverseLeft.Clear();
        m_menuObjPosListRnversionCenter.Clear();
        m_menuObjPosListRnversionRight.Clear();

        Debug.Log("menuCount:" + menuCount);
        //一度全てのメニューをフェードアウト
        foreach (UILauncherMenu menu in m_launcherMenuList)
        {
           menu.FadeOut();
        }

        //メニューの角度のリストを表示数分作成
        createMenuObjDegreeList(menuCount, m_menuObjDegreeList);
        //実際に配置する数とメニューの最大数の差
        int countDiff = m_menuCount - menuCount;
        //反転用の角度パターン作成
        foreach (double degree in m_menuObjDegreeList)
        {
            m_menuReverseObjDegreeList.Add(degree);
        }
        m_menuReverseObjDegreeList.Reverse();

        //配置パターンを作成
        //右端（上部用も）はメニューの配置を反転しているため、メニューの数が最大(現状4)より少ない場合を調整する
        createMenuObjPosList(m_menuObjPosListLeft, m_menuObjDegreeList, addDegreeLeft);
        createMenuObjPosList(m_menuObjPosListCenter, m_menuObjDegreeList);
        createMenuObjPosList(m_menuObjPosListRight, m_menuReverseObjDegreeList, addDegreeRight + m_menuBaseAddDegree * countDiff);
        createMenuObjPosList(m_menuObjPosListReverseLeft, m_menuObjDegreeList, addDegreeReverseLeft);
        createMenuObjPosList(m_menuObjPosListRnversionCenter, m_menuReverseObjDegreeList, addDegreeRnversionCenter);
        createMenuObjPosList(m_menuObjPosListRnversionRight, m_menuReverseObjDegreeList, addRnversionRight + m_menuBaseAddDegree * countDiff);

        Debug.Log("m_screenDiameter:" + m_screenDiameter);
        //個別メニューの基点(x:0.0 y:0.0)からの最大距離
        m_menuStartDistance = getDistance(m_menuObjPosListCenter[0], Vector2.zero);
        m_menuBaseToMaxDistance = m_menuStartDistance + m_menuAddDistance;

        //メニューのサイズ分を有効範囲にする
        m_addDistance = m_launcherMenuSize.x;
        m_menuActiveStartDistance = m_menuStartDistance - m_addDistance;
        m_menuActiveEndDistance = m_menuBaseToMaxDistance + m_addDistance;

        //必要な数を表示
        for (int i = 0; i < menuCount; i++)
        {
            m_launcherMenuList[i].FadeIn();
        }

        //メニューにメソッドを設定する
        setMenuAction(menuCount, setMenuActionList, m_launcherMenuList);
        //画像関連を設定する
        setMenuImages(menuCount, setNormalSpriteList, setActiveSpriteList, setIconSpriteList, m_launcherMenuList);
        


         Debug.Log("m_menuObjPosListLeft0:" + m_menuObjPosListLeft[0]);
         Debug.Log("m_menuObjPosListLeft1:" + m_menuObjPosListLeft[1]);
         Debug.Log("m_menuObjPosListLeft2:" + m_menuObjPosListLeft[2]);




    }




    /**
     * ランチャーメニューの状態を確認する
     * @brief launcherMenuStateを更新し返す
     * @returns launcherMenuStateの状態
     */
    private launcherMenuState checkLauncherMenuState()
    {
        launcherMenuState state = launcherMenuState.BeforeCountDown;
        //ランチャーの実行前
        if (m_isDisplaying == false)
        {
            // ランチャーの待機前
            if (m_isInWaiting == false)
            {
                state = launcherMenuState.BeforeCountDown;

            }

            // ランチャーの待機中
            if (m_isInWaiting == true)
            {
                state = launcherMenuState.CountDown;

            }

        }
        //ランチャーの実行中
        else
        {
            state = launcherMenuState.Executing;
        }

        return state;
        
    }


    /**
     * createMenuObjDegreeList
     * @brief 指定した数のメニューの角度を指定したリストに追加していく
     * @param[in] menuCount メニューの数
     * @param[out] menuObjDegreeList 角度を格納するリスト
     */
    private void createMenuObjDegreeList(int menuCount, List<double> menuObjDegreeList)
    {
        
        int count = menuCount;
        for (int i = 0; i < count; i++)
        {
            double addDegree = m_menuBaseAddDegree;
            double setDegree = m_menuBaseDegree + (addDegree * i);
            menuObjDegreeList.Add(setDegree);
        }

    }

    //
    
    /**
     * setMenuAction
     * @brief メニューにアクションをに追加していく
     * @param[in] menuCount メニューの数
     * @param[in] setMenuActionList　メニューに指定するActionを格納したList
     * @param[out] launcherMenuList  Actionを設定するメニューのList
     */
    private void setMenuAction(int menuCount, List<UnityAction> setMenuActionList, List<UILauncherMenu> launcherMenuList)
    {
        int count = menuCount;
        for (int i = 0; i < count; i++)
        {
            m_launcherMenuList[i].SetEventAction(setMenuActionList[i]);
        }

    }


    /**
     * setMenuImages
     * @brief メニューに画像をに追加していく
     * @param[in] menuCount メニューの数
     * @param[in] setNormalSpriteList　メニューに指定する通常画像を格納したList
     * @param[in] setActiveSpriteList　メニューに指定する通常画像を格納したList
     * @param[in] setIconSpriteList　メニューに指定する通常画像を格納したList
     * @param[out] launcherMenuList　画像を指定すメニューのList
     */
    private void setMenuImages(int menuCount, List<Sprite> setNormalSpriteList, List<Sprite> setActiveSpriteList, List<Sprite> setIconSpriteList, List<UILauncherMenu> launcherMenuList)
    {
        
        int count = menuCount;
        for (int i = 0; i < count; i++)
        {
            launcherMenuList[i].SetImage(setNormalSpriteList[i], setActiveSpriteList[i], setIconSpriteList[i]);
        }

    }


    /**
     * メニューの配置用Listを作成
     * @brief 角度のパターンリストとメニューの半径 加算する角度から配置する角度を計算しListに格納
     * @param[in] setPosList 配置用の座標List
     * @param[in] setDegreeList 角度のパターンList
     * @param[in] addDegree 加算する角度
     */
    private void createMenuObjPosList(List<Vector2> setPosList, List<double> setDegreeList, double addDegree = 0.0d)
    {

        foreach (double degree in setDegreeList)
        {
            Vector2 setPos;
            setPos = coordinateOnCircumference(m_menuRadius, degree, addDegree);
            setPosList.Add(setPos);
            
        }

    }


    /**
     * 円周上の座標を 半径と角度を求める
     * @brief 角度と距離から座標を出す式から計算
     * @param[in] radius 半径
     * @param[in] degree 角度
     * @param[in] addDegree 回転させるための調整の値
     * @returns 角度
     */
    private Vector2 coordinateOnCircumference(float radius, double degree, double addDegree = 0.0d)
    {
        Vector2 pos;

        //ラジアンを求める addDegreeは回転させるための調整用
        double radian = (degree + addDegree) * Mathf.PI / 180;
        //ラジアンがdoubleなのでキャスト
        //TODO:そもそもfloatだと問題あるか未調査
        float radianF = (float)radian;
        float posX = Mathf.Cos(radianF) * radius;
        float posY = Mathf.Sin(radianF) * radius;

        pos = new Vector2 (posX, posY);
        return pos;
    }



    /**
     * 画面を押した時のランチャーメニューの処理を実行
     * @brief 画面を押した時のランチャー実行前、実行中 それぞれで実行する処理
     * @param[in] basePointPos 画面を押した始めのスクリーン座標
     */
    public void OnPointerDownProcess(Vector2 basePointPos)
    {

        m_launcherMenuState = checkLauncherMenuState();

        switch (m_launcherMenuState)
        {
            case launcherMenuState.BeforeCountDown:

                m_screenBasePos = new Vector2(basePointPos.x, basePointPos.y);
                m_menuBasePos = new Vector2(m_screenBasePos.x * m_screenDiameter, m_screenBasePos.y * m_screenDiameter);
                startDelayedLauncherMenu(m_menuStartWaitingTime);
                break;

            case launcherMenuState.CountDown:
                break;

            case launcherMenuState.Executing:
                break;

            default:
                break;
                
        }

    }


    /**
     * 画面を押して離した時のランチャーメニューの処理
     * @brief 画面を押して離した時に実行 ランチャー実行前、実行中 それぞれの処理を実行
     */
    public void OnPointerUpProcess()
    {

        m_launcherMenuState = checkLauncherMenuState();

        switch (m_launcherMenuState)
        {
            case launcherMenuState.BeforeCountDown:
                break;

            case launcherMenuState.CountDown:
                stopDelayedLauncherMenu(m_coroutine);
                break;

            case launcherMenuState.Executing:
                stopLauncherMenu();
                break;

            default:
                break;
        }

    }

    
    /**
     * ドラックが開始時のランチャーメニューの処理
     * @brief  ドラックが開始したとき時に実行 ランチャー実行前、実行中 それぞれの処理を実行
     */
    public void OnBeginDragProcess()
    {

        m_launcherMenuState = checkLauncherMenuState();

        switch (m_launcherMenuState)
        {
            case launcherMenuState.BeforeCountDown:
                break;

            case launcherMenuState.CountDown:
                stopDelayedLauncherMenu(m_coroutine);
                break;

            case launcherMenuState.Executing:
                break;

            default:
                break;
        }

        
    }


    /**
     * ドラッグが実行中のランチャーメニューの処理
     * @brief ドラッグが実行中に実行 ランチャー実行前、実行中 それぞれの処理を実行
     * @param[in] processInDragPos ドラッグ中の座標
     */
    public void OnDragProcess(Vector2 processInDragPos)
    {

        m_launcherMenuState = checkLauncherMenuState();
        
        switch (m_launcherMenuState)
        {
            case launcherMenuState.BeforeCountDown:
                break;

            case launcherMenuState.CountDown:
                stopDelayedLauncherMenu(m_coroutine);
                break;

            case launcherMenuState.Executing:

                //今回のメニューの座標を計算しドラッグと起点からの距離 角度を求める
                Vector2 menuProcessInDragPos = new Vector2(processInDragPos.x * m_screenDiameter, processInDragPos.y * m_screenDiameter);
                float processInDistance = getDistance(m_menuBasePos, menuProcessInDragPos);
                double setDegree = getDegree(menuProcessInDragPos.x, menuProcessInDragPos.y, m_menuBasePos.x, m_menuBasePos.y);

                //メニューの処理
                menuProcess(processInDistance, setDegree, m_launcherMenuList);

                break;

            default:
                break;
                
        }

    }


    /**
     * 個別メニューの処理
     * @brief 個別メニューの処理
     * @param[in] setDistance 移動距離
     * @param[in] setDegree 指定する度
     * @param[in] menuList メニューを管理するList
     * @todo  ifの入れ子が深いので列挙などで整理したいが理解しにくくなりそうなので要検討
     */
    private void menuProcess(float setDistance, double setDegree, List<UILauncherMenu> menuList)
    {

        int count = menuList.Count;

        if (m_isDegreesMatched)
        {

            double menuDegree = m_launcherMenuList[m_menuActiveNunmber].SelfDegree;
            double degreeDifference = setDegree - menuDegree;
            float abseDifference = Mathf.Abs((float)degreeDifference);

            //角度が許容範囲かどうか
            if (abseDifference <= m_degreeScope)
            {

                //メニューが有効になる範囲かどうか
                if (m_menuActiveStartDistance <= setDistance && setDistance <= m_menuActiveEndDistance)
                {

                    if (!m_launcherMenuList[m_menuActiveNunmber].IsActive)
                    {

                        m_launcherMenuList[m_menuActiveNunmber].DoActiveState();
                        return;

                    }

                }
                else
                {
                    m_launcherMenuList[m_menuActiveNunmber].DoInvalidState();
                
                }


            }
            else
            {
                m_isDegreesMatched = false;

                if (m_launcherMenuList[m_menuActiveNunmber].IsActive)
                {
                    m_launcherMenuList[m_menuActiveNunmber].DoInvalidState();

                }
                
            }

        }
        else
        {

            if (m_launcherMenuList[m_menuActiveNunmber].IsActive)
            {
                m_launcherMenuList[m_menuActiveNunmber].DoInvalidState();
                
            }

            //ドラッグの角と一致する度がメニューに無いか探索
            for (int i = 0; i < count; ++i)
            {
                double menuDegree = m_launcherMenuList[i].SelfDegree;
                double degreeDifference = setDegree - menuDegree;
                float abseDifference = Mathf.Abs((float)degreeDifference);

                if (abseDifference <= m_degreeScope)
                {
                    m_menuActiveNunmber = i;
                    m_isDegreesMatched = true;
                    return;

                }

            }
        
        }

    }




    /**
     * ドラックが終了した時のランチャーメニューの処理
     * @brief  ドラックが実行中に実行 ランチャー実行前、実行中 それぞれの処理を実行
     */
    public void OnEndDragProcess()
    {

        m_launcherMenuState = checkLauncherMenuState();

        switch (m_launcherMenuState)
        {
            case launcherMenuState.BeforeCountDown:
                break;

            case launcherMenuState.CountDown:
                stopDelayedLauncherMenu(m_coroutine);
                break;

            case launcherMenuState.Executing:
                stopLauncherMenu();

                break;

            default:
                break;
                
        }

    }


    /**
     * 終了と開始の二点間の角度を計算
     * @brief 終了と開始の座標(x,y)のシータ角から計算
     * @param[in] bX 終了のx座標
     * @param[in] bY 終了のy座標
     * @param[in] aX 開始のx座標
     * @param[in] aY 開始のy座標
     * @returns 二点間の角度
     * @note 第三,四引数は指定しなければ(0.0, 0.0)になる
     */
    private double getDegree(float bX, float bY, float aX = 0.0f, float aY = 0.0f)
    {
        float radian = Mathf.Atan2(bY - aY, bX - aX);
        double degree = radian * 180 / Mathf.PI;

        return degree;

    }


    /**
     * 指定した距離から座標を計算
     * @brief 三角関数を利用し距離を計算
     * @param[in] distance 指定する距離
     * @param[in] setDegree 自身の度
     * @returns 指定した距離から座標
     */
    private Vector2 getMoveCoordinate(float distance, double setDegree)
    {

        Vector2 pos;
        double degree = setDegree;
        //距離（半径） 移動量と初期の半径
        float radius = m_menuRadius + distance;
        //角度と距離から座標を出す式
        //ラジアンを求める
        double radian = (degree) * Mathf.PI / 180;
        //ラジアンがdoubleなのでキャスト
        //TODO:そもそもfloatだと問題あるか未調査
        float radianF = (float)radian;
        float posX = Mathf.Cos(radianF) * radius;
        float posY = Mathf.Sin(radianF) * radius;

        pos = new Vector2 (posX, posY);

        return pos;

    }

    /**
     * 座標から2点間の距離を出す
     * @brief 開始の座標 と 終了の座標 の距離から２点間の距離
     * @param[in] basePos 開始の座標
     * @param[in] endPos 終了の座標
     * @returns 2点間の距離
     */
    private float getDistance(Vector2 basePos, Vector2 endPos)
    {
        double distance = Mathf.Sqrt((endPos.x - basePos.x) * (endPos.x - basePos.x) + (endPos.y - basePos.y) * (endPos.y - basePos.y));
        return (float) distance;
    }


    /**
     * ランチャーメニューのカウントダウンの為の遅延処理の実行
     * @brief ランチャーメニューを指定した時間だけ遅延させて開始
     * @param[in] delayTime 遅延させる時間
     */
    private void startDelayedLauncherMenu(float delayTime)
    {
        //遅延処理実行中なので誤作動防止の為に処理を抜ける
        if (m_isInWaiting)
        {
            return;

        }
        


        //TODO:外部から強制的に実行させないようしている場合ここと通る
        if (!m_isExecutable)
        {
            Debug.Log("ランチャーの実行が許可されていないのでカウントダウンを終了します。");
            return;
        }

        m_isInWaiting = true;
        m_coroutine = delayedProcess(startLauncherMenu, delayTime);
        StartCoroutine(m_coroutine);

    }


    /**
     * 遅延処理
     * @brief secondで指定した秒数後にコールバックを実行
     * @param[in] clallBackMethod コールバックで実行させるメソッド
     * @param[in] delayTime 遅延させる時間
     */
    private IEnumerator delayedProcess(Action clallBackMethod, float delayTime)
    {
            
        yield return new WaitForSeconds(delayTime);

        clallBackMethod();
        
        yield break;

    }


    /**
     * ランチャーメニューを開始
     * @brief ランチャーメニューの開始時の処理
     * @todo 遅延させて実行されるので直接はよばれない
     */
    private void startLauncherMenu()
    {

        //ランチャー実行不可の場合は処理しない
        //delayの影響で、待っている間に不許可になる可能性があるため
        if (!m_isExecutable)
        {
            Debug.Log("ランチャーの実行が許可されていないので終了します。");
            return;

        }

        //ランチャーが実行中は処理を停止し誤作動を防止
        if (m_isDisplaying)
        {
            return;
            
        }

        Vector2 setPos = m_menuBasePos;

        switchOverlayFade();
        menuParentObjShow(setPos);
        
        //メニューの中心が画面のどこにあるかで利用する座標のListを調べセットする
        m_currentPosList = searchMenuListPos(setPos);
        allMenuSetup(m_currentPosList);
        m_isDisplaying = true;

        //実行後 遅延処理で実行した場合は停止に戻す
        if (m_isInWaiting)
        {
            m_isInWaiting = false;

        }
    }


    /**
     * 指定した座標から表示するメニューの座標の一覧を検索
     * @brief 画面のどこにあるかで利用する座標のListを調べる
     * @param[in] setPos 座標
     * @returns 利用する座標のList
     */
    private List<Vector2> searchMenuListPos(Vector2 setPos)
    {
        List<Vector2> list = m_menuObjPosListCenter;
        
        //縦反転の境界 ランチャーメニューの表示領域の高さ - (半径 + メニューの大きさ)
        float verticalityReverseBoundary = m_launcherMenuMaximum.y - (m_menuRadius + m_launcherMenuSize.y);

        //表示切り替えの基準 ランチャーメニューの表示領域の幅 /3 
        float areaBase = m_launcherMenuMaximum.x / 3;
        //左 中央 右の境界
        float leftReversalBoundary = areaBase;
        float centerReversalBoundary = areaBase * 2;
        float rightReversalBoundary = m_launcherMenuMaximum.x;


        //座標が反転の領域を越えなければ上に表示
        if (setPos.y < verticalityReverseBoundary)
        {
            //x座標が左の領域の場合
            if (setPos.x < leftReversalBoundary)
            {
                list = m_menuObjPosListLeft;
            }
            //x座標が 中央の領域の場合
            else if (setPos.x > leftReversalBoundary && setPos.x < centerReversalBoundary)
            {
                list = m_menuObjPosListCenter;
            }
            //x座標が 右の領域の場合
            else if (setPos.x > centerReversalBoundary && setPos.x < rightReversalBoundary)
            {
                list = m_menuObjPosListRight;
            }

        }
        //座標が反転の領域を越えたら下にメニューを表示
        else
        {
            //x座標が左の領域の場合
            if (setPos.x < leftReversalBoundary)
            {
                list = m_menuObjPosListReverseLeft;
            }
            //x座標が 中央の領域の場合
            else if (setPos.x > leftReversalBoundary && setPos.x < centerReversalBoundary)
            {
                list = m_menuObjPosListRnversionCenter;
            }
            //x座標が 右の領域の場合
            else if (setPos.x > centerReversalBoundary && setPos.x < rightReversalBoundary)
            {
                list = m_menuObjPosListRnversionRight;
            }

        }

        return list;
        
    }

    /**
     * 全てのメニューを指定した座標に移動
     * @brief 全てのメニューを指定した座標に移動　度も更新
     * @param[in] posList 移動させる座標のList
     */
    private void allMenuSetup(List<Vector2> posList)
    {

        int l = posList.Count;

        for (int i = 0; i < l; ++i)
        {
            //初期の座標
            Vector2 setPos = posList[i];

            //度も求めて更新
            double setDegree = getDegree (posList[i].x, posList[i].y);

            //移動する最大の距離の座標
            Vector2 setMaxMovePos = getMoveCoordinate(m_menuAddDistance, setDegree);

            m_launcherMenuList[i].Setup(setPos, setMaxMovePos, setDegree);

        }

    }


    /**
     * ランチャーメニューの遅延処理を停止
     * @brief ランチャーメニューのコールチンを停止
     * @param[in] stopCoroutine 停止するコールチンを設定
     * @note ランチャーの終了とは違う
     */
    private void stopDelayedLauncherMenu(IEnumerator stopCoroutine)
    {
        if (!m_isInWaiting)
        {
            return;
            
        }

        StopCoroutine(stopCoroutine);

        m_isInWaiting = false;

    }


    /**
     * StopCountDown
     * @brief stopDelayedLauncherMenuを外部から実行
     */
    public void StopCountDown()
    {
        stopDelayedLauncherMenu(m_coroutine);
    }

    /**
     *ランチャーメニューを閉じる
     * @brief ランチャーメニューのフェードアウトし閉じる
     * @note 表示しているのでコールチンの停止処理ではない
     */
    private void stopLauncherMenu()
    {
        //ランチャーが停止していたらランチャーメニューがすでに非表なので処理を抜ける
        if (!m_isDisplaying)
        {
            return;

        }

        //待機中の場合は開始していないので処理を停止
        if (m_isInWaiting)
        {
            return;
        }

        switchOverlayFade();
        menuHidden();
        m_launcherMenuList[m_menuActiveNunmber].InvokeCommand();
        m_launcherMenuList[m_menuActiveNunmber].DoInvalidState();
        allMenuReset();
        m_isDisplaying = false;

    }


    /**
     * 全てのメニューを元の位置に戻す
     * @brief ランチャーメニューのすべてのメニューのResetMenuメソッドを実行
     */
    private void allMenuReset()
    {

        int count = m_menuCount;
        for (int i = 0; i < count; ++i)
        {
            m_launcherMenuList[i].ResetMenu();
        }
        
    }


    /**
     * オーバーレイのレイヤーの表示非表示の切り替え
     * @brief オーバーレイのレイヤーの表示非表示の切り替え
     * @todo m_overlayFadeが仮なので後々変更
     */
    private void switchOverlayFade()
    {
        bool displayed = m_overlayFade.IsDisplayed;

        float fadeTime = 0.25f;

        //表示されている時
        if (displayed)
        {
            
            m_overlayFade.FadeOut(fadeTime);
            Debug.Log("フェードアウト");

            //他のプロセスを許可をフェードの実行後にずらして行う
            IEnumerator coroutine = delayedProcess(changeIsOtherProcess, fadeTime);
            StartCoroutine(coroutine);
            
        }
        //表示されていない時
        else
        {
            //TODO:フェードの板は仮なので後々変更
            m_overlayFade.FadeIn(fadeTime);
            m_isOtherProcess = false;

        }
        
    }


    /**
     * 他の処理の実行を許可するかを切り替える
     * @brief タッチイベントで他の処理と被る場合に他を許可するかの状態を変更
     */
    private void changeIsOtherProcess()
    {
        m_isOtherProcess = !m_isOtherProcess;
    }


    /**
     * メニューの中心点を表示する
     * @brief メニューの親の要素を指定した座標に表示
     * @param[in] menuPos 表示する座標
     */
    private void menuParentObjShow(Vector2 menuPos)
    {
        m_menuParentOG.GetComponent<RectTransform>().anchoredPosition = menuPos;
        UIFade menuFade = m_menuParentOG.GetComponent<UIFade>();
        menuFade.FadeIn();

    }


    /**
     * メニューの中心点を非表示にする
     * @brief メニューの親の要素を非表示
     */
    private void menuHidden()
    {
        m_menuParentOG.GetComponent<RectTransform>().anchoredPosition = m_menuParentDefaultPos;
        UIFade menuFade = m_menuParentOG.GetComponent<UIFade>();
        menuFade.Invisible();

    }



    /**外部からランチャーの実行を許可しない
     * @brief 外部からランチャーの実行を許可しない
     */
    public void DisallowExecution()
    {
        if (!m_isExecutable)
        {
            Debug.Log("ランチャーの実行はすでに不許可です。");
            return;

        }

        Debug.Log("ランチャーの実行を不許可。");
        m_isExecutable = false;

    }

    /**外部からランチャーの実行を許可する
     * @brief 外部からランチャーの実行を許可する
     */
    public void AllowExecution()
    {
        if (m_isExecutable)
        {
            Debug.Log("ランチャーの実行はすでに許可されています。");
            return;

        }

        Debug.Log("ランチャーの実行を許可。");
        m_isExecutable = true;

    }

}
