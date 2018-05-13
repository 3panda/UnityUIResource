/**
 * @file UITouchEventBase.cs
 * @brief EventSystemsのメソッド毎の処理を設定する為の基盤クラス UIEventBaseを継承
 * @author Ryota Shiroguchi
 * @date 2016-12-08
 */


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;


/**
 * @class UITouchEventBase
 * @brief EventSystemsのメソッド毎の処理を設定する為の基盤クラス UIEventBaseを継承
 * @note サブオプションの使用を考慮している
 */
public class UITouchEventBase : UIEventBase
{
    
    //================================================================================
    //初期設定関連
    //================================================================================
    protected Vector2 m_touchStartPos;        /*< フリック処理の開始位置を格納*/
    protected Vector2 m_beginDragPos;         /*< ドラッグ処理の開始位置を格納*/
    protected Vector2 m_previousDragPos;      /*< 一つ前のドラッグ処理中の位置を格納*/
    protected Vector2 m_processInDragPos;     /*< ドラッグ処理中の位置を格納*/
    protected Vector2 m_endDragPos;           /*< ドラッグの終了位置を格納*/
    protected Vector2 m_onPointerExit;        /*< イベントの範囲から外れた時の位置を格納*/
    protected float m_screenWidth;            /*< 画面のサイズ*/
    protected bool m_inDrag = false;          /*< ドラッグ中かどうか*/
    protected bool m_isPointer = false;       /*< イベントの範囲内に入ったかどうか*/


    //================================================================================
    //サブオプションの設定関連
    //================================================================================
    protected bool m_isUseSubOption = false;        /*< サブオプションを使用するかどうか*/
    //protected SubOptionArgs m_subOptionArgs = null; /*< サブオプションの設定*/

    void Awake()
    {
        //Dragを有効に
        m_isDragProcess = true;
        
    }


    void Start() 
    {
        initialization();
        StartSetup();
    }


    /**
     * 初期化処理
     * @brief 初期化時に実行する処理をまとめるメソッド
     */
    protected void initialization()
    {
        m_screenWidth = Screen.width;
    }


    /**
     * OnPointerUp時に実行
     * @brief OnPointerUp時に実行する処理をまとめるメソッド
     */
    public override void ProcessOnPointerDown(Vector2 touchStartPos)
    {
        //押下中
        m_isPointer = true;

        //フリックの開始位置を取得
        m_touchStartPos = touchStartPos;

        //サブオプションを使用
        //if (m_isUseSubOption)
        //{
            //サブオプションのメニューを定義
            //SubOption.Instance.Setup(m_subOptionArgs);

            //SubOption.Instance.OnPointerDownProcess(m_touchStartPos);
            
        //}

    }

    /**
     * OnPointerUp時に実行
     * @brief OnPointerUp時に実行する処理をまとめるメソッド
     */
    public override void ProcessOnPointerUp()
    {
        //押下中断
        m_isPointer = false;

/*
        if (m_isUseSubOption)
        {


            bool isDisplaying = SubOption.Instance.IslauncherMenuDisplaying;
            bool isProcess = SubOption.Instance.IslauncherMenuOtherProcess;

            if(!isDisplaying && isProcess == true)
            {
                //SubOptionの発動カウントダウンを停止する
                SubOption.Instance.StopCountDown();

                //Unity Eventを使用する場合実行
                if (m_isUseUnityEvent)
                {
                    Invoke(m_unityEvent);

                }

            }
            else
            {
                SubOption.Instance.OnPointerUpProcess();
                
            }

        }
        else
        {

            
            //Unity Eventを使用する
            if (m_isUseUnityEvent)
            {
                Invoke(m_unityEvent);

            }

        }
*/
        
    }




    /**
     * OnPointerExit時に実行
     * @brief OnPointerExit時に実行する処理をまとめるメソッド
     */
    public override void ProcessOnPointerExit(Vector2 eventData)
    {
        m_isPointer = false;
        m_onPointerExit = eventData;
        
    }



    /**
     * OnBeginDrag時に実行
     * @brief OnBeginDrag時に実行する処理をまとめるメソッド
     */
    public override void ProcessOnBeginDrag(Vector2 beginDragPos)
    {
        //ドラッグの開始位置を取得
        m_beginDragPos = beginDragPos;
        //一つ前のドラッグ処理中の位置を最初は存在しないので開始の座標を入れる
        m_previousDragPos = m_beginDragPos;

        //m_isExecutableAction = false;
    }

    /**
     * OnDrag時に実行
     * @brief OnDrag時に実行する処理をまとめるメソッド
     */
    public override void ProcessOnDrag(Vector2 processInDragPos)
    {
        m_inDrag = true;
        m_processInDragPos = processInDragPos;


        //サブオプションが表示されているか
        bool isDisplaying = false;

        //サブオプションを使う場合
        //if (m_isUseSubOption)
        //{
            //isDisplaying = SubOption.Instance.IslauncherMenuDisplaying;
        //}

        //サブオプションが表示されてい無い時に実行する
        if(!isDisplaying)
        {

            //動きが無い場合は処理を抜ける　ProcessInDragを実行しない
            if (m_previousDragPos.x == m_processInDragPos.x)
            {
                return;

            }

            ProcessInDrag(m_previousDragPos.x, m_processInDragPos.x);
            
        }

        //一つ前の座標の変数に現在の座標を入れ更新する
        m_previousDragPos = m_processInDragPos;

        //サブオプションを使用
        //if (m_isUseSubOption)
        //{
            //SubOption.Instance.OnDragProcess(m_processInDragPos);
            
        //}

    }



    /**
     * OnEndDrag時に実行
     * @brief OnEndDrag時に実行する処理をまとめるメソッド
     */
    public override void ProcessOnEndDrag(Vector2 processOnEndDragPos)
    {
        m_inDrag = false;
        //ドラック終了時の座標を取得
        m_endDragPos = processOnEndDragPos;

        //ランチャーメニューが表示していない & 処理に影響しない状態
        bool isDisplaying = false;
        bool isProcess = true;

        //サブオプションを使用
        if (m_isUseSubOption)
        {
            //isDisplaying = SubOption.Instance.IslauncherMenuDisplaying;
            //isProcess = SubOption.Instance.IslauncherMenuOtherProcess;
        }

        
        if(!isDisplaying && isProcess == true)
        {
            ProcessAfterFlick(m_beginDragPos.x, m_endDragPos.x, 0.15f);
        }

        //サブオプションを使用
        if (m_isUseSubOption)
        {
            //SubOption.Instance.OnEndDragProcess();
            
        }

        
    }



    /**
     * StartSetup
     * @brief 初期化後に設定などを行うメソッド
     */
    public virtual void StartSetup()
    {

    }

    /**
     * ProcessInDrag
     * @brief ProcessOnDrag内で実行する個別の処理をまとめるメソッド 継承先で利用
     */
    public virtual void ProcessInDrag(float beginPosX, float processInPosX)
    {
    
    }

    /**
     * ProcessAfterFlick
     * @brief ProcessOnEndDrag内で実行するDrag終了後の個別の処理をまとめるメソッド 継承先で利用
     */
    public virtual void ProcessAfterFlick(float startPosX, float endPosX, float swipeRate)
    {
    
    }

    /**
     * OnInvoke
     * @brief 実行
     */
    public virtual void Invoke(UnityEvent ue)
    {
        ue.Invoke();
    }

}
