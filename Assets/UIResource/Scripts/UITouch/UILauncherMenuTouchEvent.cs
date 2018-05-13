/**
 * @file UILauncherMenuTouchEvent.cs
 * @brief ランチャー用のタッチエリアのイベント実行を管理
 * @author Ryota Shiroguchi
 * @date 2016-12-08
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

/**
 * @class UITouchEvent
 * @brief メニュー画面のスクリーンのタッチ処理の管理クラスの作業用
 * @todo MenuTouchAreaActionに反映したら削除する
 */
public class UILauncherMenuTouchEvent : MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IBeginDragHandler,IDragHandler,IEndDragHandler
{

    [SerializeField]
    private UILauncherMenuController m_launcherMenuController;     /**< ランチャーメニューコントローラー*/
    //フリック処理の開始位置を格納
    private Vector2 m_touchStartPos;
    //ドラッグ処理の開始位置を格納
    private Vector2 m_beginDragPos;
    //一つ前のドラッグ処理中の位置を格納
    private Vector2 m_previousDragPos;
    //ドラッグ処理中の位置を格納
    private Vector2 m_processInDragPos;
    //ドラッグの終了位置を格納
    private Vector2 m_endDragPos;
    // 画面のサイズ
    private float m_screenWidth;

    [SerializeField]
    private Sprite m_menuNormal; /**< 通常の時の画像*/
    [SerializeField]
    private Sprite m_menuActive; /**< 有効の時の画像*/
    [SerializeField]
    private List<Sprite> m_menuIconSpriteList = new List<Sprite>();  /**< アイコンの画像*/

    //ドラッグ中かどうか
    private bool m_inDrag = false;
    public bool InDrag
    {
        get
        {
            return m_inDrag;
        }
    }



    void Start() 
    {
        //初期化
        initialization();
    
    }


    /**
     * 初期化
     * @brief スクリーンサイズやタッチエリアの横幅の取得
     */
    private void initialization()
    {
        m_screenWidth = Screen.width;

    }

    //押した時
    public void OnPointerDown(PointerEventData eventData)
    {
        //フリックの開始位置を取得
        m_touchStartPos = eventData.position;

        //#LauncherMenu
        //ランチャーメニューの設定の初期化
        //TODO:複数配置に対応しているのでOnPointerDownで行う
        launcherMenuInitialization();

        m_launcherMenuController.OnPointerDownProcess(m_touchStartPos);

    }

    //押して離したら
    public void OnPointerUp(PointerEventData eventData)
    {
        //#LauncherMenu
         m_launcherMenuController.OnPointerUpProcess();

    }

    //ドラックが開始したとき呼ばれる
    public void OnBeginDrag(PointerEventData eventData)
    {
        //ドラッグの開始位置を取得
        m_beginDragPos = eventData.position;
        //一つ前のドラッグ処理中の位置を最初は存在しないので開始の座標を入れる
        m_previousDragPos = m_beginDragPos;
        
    }

    //ドラック中に呼ばれる.
    public void OnDrag(PointerEventData eventData)
    {
        m_inDrag = true;
        m_processInDragPos = eventData.position;

        processInDrag(m_previousDragPos.x, m_processInDragPos.x);
        //一つ前の座標の変数に現在の座標を入れ更新する
        m_previousDragPos = m_processInDragPos;

        //#LauncherMenu
        m_launcherMenuController.OnDragProcess(m_processInDragPos);

    }

    //ドラックが終了したとき呼ばれる
    public void OnEndDrag(PointerEventData eventData)
    {
        m_inDrag = false;
        //ドラック終了時の座標を取得
        m_endDragPos = eventData.position;
        //#LauncherMenu
        m_launcherMenuController.OnEndDragProcess();

    }


    /**
     * ドラック中の処理
     * @brief 画面をドラックしている間に実行する処理
     * @param beginPosX ドラックの開始位置（スクリーン座標）
     * @param processInPosX ドラックの中の位置（スクリーン座標）
     *
     */
    private void processInDrag(float beginPosX, float processInPosX)
    {
        //指の移動がない場合はドラッグされていないので処理を終了
        if (beginPosX == processInPosX)
        {
            return;
        }

        //移動量分移動する
        float distance = processInPosX - beginPosX;


    }

    /**
     * フリックした後の処理
     * @brief 指を離した後に実行する処理
     * @param startPosX タップの開始位置（スクリーン座標）
     * @param endPosX タップの終了位置（スクリーン座標）
     * @param swipeRate 移動量の計算に利用する分母の数
     * @todo 　ランチャーでやった手法でスクリーンと画面の誤差の補正に変更するか要検討
     */
    private void processAfterFlick(float startPosX, float endPosX, float swipeRate)
    {

        //移動量
        float distance = endPosX - startPosX;
        //移動量の絶対数
        float distanceAbs = Mathf.Abs(distance);
        //基準値を 画面サイズ * swipeRate に
        float baseValue = m_screenWidth * swipeRate;
        

        //移動量が基準以下 動きを戻す
        if(distanceAbs <= baseValue)
        {
            return;

        }

        //開始のx座標が終了のx座標より大きい 左から右に
        if (endPosX < startPosX)
        {
            return;

        }

        //開始のx座標が終了のx座標より小さい 右から左に
        else if (startPosX < endPosX)
        {
            return;

        }

    }


    /**
     * launcherMenuInitialization
     * @brief ランチャーメニューの設定をまとめている
     * @todo サンプルのために強引に作成
     */
    private void launcherMenuInitialization()
    {

        //Action
        List<UnityAction> setActionList = new List<UnityAction>();
        List<Sprite> setNormalSpriteList = new List<Sprite>();
        List<Sprite> setActiveSpriteList = new List<Sprite>();
        List<Sprite> setIconSpriteList = new List<Sprite>();

        //Action
        setActionList.Add(MenuActionTestA);
        setActionList.Add(MenuActionTestB);
        setActionList.Add(MenuActionTestC);

        //メニュー 通常時の背景画像
        setNormalSpriteList.Add(m_menuNormal);
        setNormalSpriteList.Add(m_menuNormal);
        setNormalSpriteList.Add(m_menuNormal);

        //メニュー 有効時の背景画像
        setActiveSpriteList.Add(m_menuActive);
        setActiveSpriteList.Add(m_menuActive);
        setActiveSpriteList.Add(m_menuActive);

        //アイコン
        setIconSpriteList.Add(m_menuIconSpriteList[0]);
        setIconSpriteList.Add(m_menuIconSpriteList[1]);
        setIconSpriteList.Add(m_menuIconSpriteList[2]);

        int count = setActionList.Count;
        //ランチャーの初期化
        m_launcherMenuController.Setup(0.5f, count, setActionList,  setNormalSpriteList, setActiveSpriteList, setIconSpriteList);
        
    }

    //ランチャーで呼び出すダミーのアクションの
    private void MenuActionTestA()
    {
        Debug.Log("アクションA");
    }

    //以下検証用のアクション
    private void MenuActionTestB()
    {
        Debug.Log("アクションB");
    }

    //以下検証用のアクション
    private void MenuActionTestC()
    {
        Debug.Log("アクションC");
    }


}
