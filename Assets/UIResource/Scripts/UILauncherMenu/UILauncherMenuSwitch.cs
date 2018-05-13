
/**
 * @file UILauncherMenuSwitch.cs
 * @brief サブオプションのコンポーネントクラス
 * @author Ryota Shiroguchi
 * @date 2016-12-08
 */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;


/**
 * @class UILauncherMenuSwitch
 * @brief : 
 */
public class UILauncherMenuSwitch : MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IBeginDragHandler,IDragHandler,IEndDragHandler,IUpdateSelectedHandler
{

    private Vector2 m_touchStartPos;
    //ドラッグ処理の開始位置を格納
    private Vector2 m_beginDragPos;
    //一つ前のドラッグ処理中の位置を格納
    private Vector2 m_previousDragPos;
    //ドラッグ処理中の位置を格納
    private Vector2 m_processInDragPos;
    //ドラッグの終了位置を格納
    private Vector2 m_endDragPos;
    //ドラッグ中かどうか
    private bool m_inDrag = false;


    //[SerializeField]
    //private List<SubOption.MenuType> m_menuList = new List<SubOption.MenuType>();
    [SerializeField]
    private UnityEvent m_setEvent;
    //private SubOption.SetupArgs m_setSubOptionArgs = null;

    //private EffectButton m_effectButton = null;
    private Button m_button = null;

    void Start()
    {
        initialization();
        //subOptionArgsInitialization();
    }

    //選択中
    public void OnUpdateSelected(BaseEventData data)
    {
        

        /*
         
        //メニューが表示中
        if (SubOption.Instance.IslauncherMenuDisplaying)
        {
            //EffectButtonとButtonがある場合　無効化
            if (m_effectButton != null)
            {
                m_effectButton.enabled = false;

            }
            else if (m_button != null)
            {
                m_button.enabled = false;

            }

        }
        */
    }

    //押した時
    public void OnPointerDown(PointerEventData eventData)
    {
        //フリックの開始位置を取得
        m_touchStartPos = eventData.position;
        //#LauncherMenu
        //SubOption.Instance.DefaultMenuSetup(m_setSubOptionArgs);
        //SubOption.Instance.OnPointerDownProcess(m_touchStartPos);

    }

    //押して離したら
    public void OnPointerUp(PointerEventData eventData)
    {
        //SubOption.Instance.OnPointerUpProcess();

        /*

        //EffectButtonとButtonがある場合　無効化
        if (m_effectButton != null && m_effectButton.enabled == false)
        {
            m_effectButton.enabled = true;

        }
        else if (m_button != null && m_button.enabled == false)
        {
            m_button.enabled = true;

        }
        
        */

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

        //一つ前の座標の変数に現在の座標を入れ更新する
        m_previousDragPos = m_processInDragPos;
        //#LauncherMenu
        //SubOption.Instance.OnDragProcess(m_processInDragPos);

    }

    //ドラックが終了したとき呼ばれる
    public void OnEndDrag(PointerEventData eventData)
    {
        m_inDrag = false;
        
        //ドラック終了時の座標を取得
        m_endDragPos = eventData.position;
        //#LauncherMenu
        //SubOption.Instance.OnEndDragProcess();

    }

    /**
     * initialization
     * @brief 初期化の設定をまとめている
     */
    private void initialization()
    {
        //EffectButtonかButtonがあれば格納
        //m_effectButton = gameObject.GetComponent<EffectButton>();
        m_button = gameObject.GetComponent<Button>();

    }


    /**
     * SubOptionArgsInitialization
     * @brief サブオプションの設定をまとめている
     * @todo デザインが確定したら変更 仕様決まったら修正の可能性あり
     */
    private void subOptionArgsInitialization()
    {
        //UnityEventの情報かたUnityACtionを取り出し格納
        List<UnityAction> setActionList = new List<UnityAction>();
        //subOptionSetActionList(m_setEvent, setActionList);

        //メニュ数の制限
        //int SubOptionLimit = SubOption.Instance.LauncherMenuCount;
        //int count = m_menuList.Count;
        //int setActionCount = setActionList.Count;
        
        /*
        if (SubOptionLimit < count)
        {
            //Debug.LogError(gameObject.name + "に設定されたメニューの数が上限を超えています。 " + count + "/" + SubOptionLimit + "(メニューの数/メニューの上限)");
            return;

        }
        else if (SubOptionLimit < setActionCount)
        {
            //Debug.LogError(gameObject.name + "に設定されたActionの数が上限を超えています。 " + setActionCount + "/" + SubOptionLimit + "(Actionの数/Actionの数上限)");
            return;

        }

        else if (count != setActionCount)
        {
            //Debug.LogError(gameObject.name + "に設定されたメニューとActionの数が一致していません。 " + count + ":" + setActionCount + "(メニューの数:Actionの数)");
            return;
        }
        
        */

        /*

        //Menuに合わせたIconのSpriteを取得し格納
        List<Sprite> setIconSpriteList = new List<Sprite>();
        for (int i = 0; i < m_menuList.Count; ++i)
        {
            Sprite sprite = SubOption.Instance.GetMenuImage(m_menuList[i]);
            setIconSpriteList.Add(sprite);
            //Debug.logger.Log("setIconSpriteList" + i + ":" + setIconSpriteList[i]);
            
        }

        //設定
        m_setSubOptionArgs = new SubOption.SetupArgs(setActionList, setIconSpriteList);

        */

    }




    /**
     * subOptionSetActionList
     * @brief インスペークターで指定したUnityActionを取り出しListにセットする
     * @param[in] getEvent UnityActionが登録されているUnityEvent
     * @param[out] actionList UnityActionを格納するList
     */
    private void subOptionSetActionList(UnityEvent getEvent, List<UnityAction> actionList)
    {

        /*
        int count = getEvent.GetPersistentEventCount();

        for (int i = 0; i < count; ++i)
        {
            //CreateDelegateでUnityActionを作っている
            UnityAction action = (UnityAction)System.Delegate.CreateDelegate(typeof(UnityAction), getEvent.GetPersistentTarget(i), getEvent.GetPersistentMethodName(i));
            actionList.Add(action);
            //Debug.logger.Log("UnityAction:" + actionList[i].Method);

        }

        */
        
    }
    

}
