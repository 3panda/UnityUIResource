/**
 * @file UITouchEvent.cs
 * @brief タッチエリアのイベント実行を管理
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
public class UITouchEvent : MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    [SerializeField]
    private UISlideshow m_slideshow; /**< スライドショー*/
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

    }

    //押して離したら
    public void OnPointerUp(PointerEventData eventData)
    {
 
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

    }

    //ドラックが終了したとき呼ばれる
    public void OnEndDrag(PointerEventData eventData)
    {
        m_inDrag = false;
        //ドラック終了時の座標を取得
        m_endDragPos = eventData.position;

        processAfterFlick(m_beginDragPos.x, m_endDragPos.x, 0.15f);


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

        m_slideshow.AllMoveValue(distance);

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
            m_slideshow.MoveAllBackContents();
            return;

        }

        //開始のx座標が終了のx座標より大きい 左から右に
        if (endPosX < startPosX)
        {
            m_slideshow.MoveRightAllContents();
            return;

        }

        //開始のx座標が終了のx座標より小さい 右から左に
        else if (startPosX < endPosX)
        {
            m_slideshow.MoveLeftAllContents();
            return;

        }

    }


}
