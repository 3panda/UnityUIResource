/**
 * @file UILauncherMenu.cs
 * @brief ランチャーメニューの各メニューの動きを設定
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
using DG.Tweening;


/**
 * @class UILauncherMenu
 * @brief ランチャーメニューの各メニューの動きを設定
 */
public class UILauncherMenu : MonoBehaviour
{


    //================================================================================
    //初期設定関連
    //================================================================================
    private float m_moveDuration = 0.2f; /*< アニメーションの継続時間 */
    private float m_fadeDuration = 0.25f;/*< ファードの継続時間 */
    private RectTransform m_selfRt;      /*< 自身のRectTransform */
    private UIFade m_selfFade;           /*< 自信のFadeクラス */
    //TODO:シングルトンになりどのページでも使われるので実行するメソッドをどうするか検討中
    private UnityEvent m_launchEvent = new UnityEvent();

    //================================================================================
    //画像関連
    //================================================================================
    //TODO:背景とアイコンを分けているのは仮 仕様が固まったら合わせる
    [SerializeField]
    private Image m_backgroundImage; /*< 背景Imageコンポーネント*/
    public Sprite NormalSprite;      /*< 通常時のSprite*/
    public Sprite ActiveSprite;      /*< 有効状態のSprite*/

    [SerializeField]
    private Image m_iconImage;       /*< アイコンのImageコンポーネント*/
    public Sprite IconSprite;        /*< 設定するアイコンのSprite*/

    //================================================================================
    //座標と角
    //================================================================================
    private Vector2 m_initialPos;   /*< 初期の座標 */
    private Vector2 m_moveStartPos; /*< 表示開始時の座標 */
    private Vector2 m_moveEndPos;   /*< 最大で動く距離 */
    private double m_selfDegree;   /*< 自身の度 */
    public double SelfDegree
    {
        get
        {
            return m_selfDegree;
        }

    }

    //================================================================================
    //状態の管理
    //================================================================================
    private bool m_isActive = false; /*< 有効な状態かどうか*/
    public bool IsActive
    {
        get
        {
            return m_isActive;
        }
    }

    
    void Start()
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

        m_selfRt = gameObject.GetComponent<RectTransform>();
        m_selfFade = gameObject.GetComponent<UIFade>();
        m_initialPos = m_selfRt.anchoredPosition;
        m_backgroundImage.sprite = NormalSprite;
        m_iconImage.sprite = IconSprite;

    }


    /**
     * メニューのセットアップ
     * @brief 指定された座標に移動する
     * @param movePos 移動させる座標
     */
    public void Setup(Vector2 setStartPos, Vector2 setEndPos, double setDegree)
    {
        
        m_selfFade.FadeIn(m_fadeDuration);
        m_moveStartPos = setStartPos;
        m_moveEndPos = setEndPos;
        m_selfDegree = setDegree;
        m_selfRt.DOAnchorPos(setStartPos, m_moveDuration).SetEase(Ease.OutQuart);
        Debug.Log(gameObject.name + "開始座標:" + m_moveStartPos);
        Debug.Log(gameObject.name + "終了座標:" + m_moveEndPos);
        Debug.Log(gameObject.name + "角度:" + setStartPos);
        
    }


    /**
     * SetEventAction
     * @brief このメニューで実行するアクションを外部から設定する
     * @param[in] setAction このメニューで実行するAction
     */
    public void SetEventAction(UnityAction setAction)
    {
        //初期化
        m_launchEvent.RemoveAllListeners();
        //Actionの追加
        m_launchEvent.AddListener(setAction);

    }


    /**
     * SetImage
     * @brief ON OFF画像　アイコン画像のセット
     * @param[in] normalSprite 通常時のSprite
     * @param[in] activeSprite 有効状態のSprite
     * @param[in] iconSprite   アイコンのSprite

     */
    public void SetImage(Sprite normalSprite, Sprite activeSprite, Sprite iconSprite)
    {
        m_backgroundImage.sprite = normalSprite;
        ActiveSprite = activeSprite;
        m_iconImage.sprite = iconSprite;

    }


    
    /**
     * 個別の処理の実行可能な状態にする
     * @brief メソッドの実行を可能にする メニューを指定位置に移動させる
     */
    public void DoActiveState()
    {
        if (m_isActive)
        {
            return;
        }

        m_isActive = true;
        m_backgroundImage.sprite = ActiveSprite;
        m_selfRt.DOAnchorPos(m_moveEndPos, m_moveDuration).SetEase(Ease.OutQuart);

    }
    

    /**
     * 個別の処理の実行不可能な状態に戻す
     * @brief メソッドの実行を不可に戻す,メニューを表示後の最初の位置に戻る
     */
    public void DoInvalidState()
    {
        if (!m_isActive)
        {
            return;
        }

        m_isActive = false;
        m_backgroundImage.sprite = NormalSprite;
        m_selfRt.DOAnchorPos(m_moveStartPos, m_moveDuration).SetEase(Ease.OutQuart);

    }

    /**
     * メニューに割り当てられた命令を実行する
     * @brief メニューに割り当てられた命令を 有効な状態でドラッグを解除した時に実行する
     * @todo 利用する場面に応じてメソッドを定義するようにしてこれに渡すようにする
     */
    public void InvokeCommand ()
    {
        if(!m_isActive)
        {
            Debug.Log(gameObject.name + "はアクティブで無いので命令は実行されませんでした");
            return;
        }

        //作成中
        Debug.Log(gameObject.name + "がアクティブメソッドを実行します");
        m_launchEvent.Invoke();
        
    }

    /**
     * メニューを元に戻す
     * @brief 最初座標にメニューを移動する
     */
    public void ResetMenu()
    {
        m_selfRt.anchoredPosition = m_initialPos;

    }

    public void FadeIn()
    {
        m_selfFade.FadeIn(m_fadeDuration);
        
    }

    public void FadeOut()
    {
        m_selfFade.FadeOut(m_fadeDuration);

    }


}
