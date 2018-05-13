/**
 * @file UIEventBase.cs
 * @brief  EventSystemsの処理の大枠をまとめた基盤クラス
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
 * @class UIEventBase
 * @brief EventSystemsの処理の大枠をまとめている
 */
public class UIEventBase : MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IPointerExitHandler,IBeginDragHandler,IDragHandler,IEndDragHandler
{

    protected bool m_isDragProcess = false;  /*< ドラッグ処理を行うかどうか*/
    [SerializeField]
    protected UnityEvent m_unityEvent;       /*< UnityEvent*/
    protected bool m_isUseUnityEvent = true; /*< UnityEventを利用するかどうか*/
    //押した時
    public void OnPointerDown(PointerEventData eventData)
    {
        //選択オブジェクト扱い
        EventSystem.current.SetSelectedGameObject(gameObject, eventData);

        //エフェクトの処理
        OnPointerDownOnEffect();
        //押した時の処理を継承先で上書き
        ProcessOnPointerDown(eventData.position);
    }

    //押して離したら
    public void OnPointerUp(PointerEventData eventData)
    {
        //選択解除
        EventSystem.current.SetSelectedGameObject(null, eventData);

        //エフェクトの処理
        OnPointerUpOnEffect();
        //押した時の処理を継承先で上書き
        ProcessOnPointerUp();

    }

    //範囲から出たら
    public void OnPointerExit(PointerEventData eventData)
    {
    
        ProcessOnPointerExit(eventData.position);
    }


    //ドラックが開始したとき呼ばれる
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!m_isDragProcess)
        {
            return;
        }

        ProcessOnBeginDrag(eventData.position);

    }

    //ドラック中に呼ばれる.
    public void OnDrag(PointerEventData eventData)
    {
        if (!m_isDragProcess)
        {
            return;
        }

        ProcessOnDrag(eventData.position);

    }

    //ドラックが終了したとき呼ばれる
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!m_isDragProcess)
        {
            return;
        }


        ProcessOnEndDrag(eventData.position);

    }


    /**
     * OnPointerUp時に実行
     * @brief OnPointerUp時に実行する処理をまとめるメソッド
     */
    public virtual void ProcessOnPointerDown(Vector2 eventData)
    {

    }


    /**
     * OnPointerExit時に実行
     * @brief OnPointerExit時に実行する処理をまとめるメソッド
     */
    public virtual void ProcessOnPointerExit(Vector2 eventData)
    {

    }


    /**
     * OnPointerUp時に実行
     * @brief OnPointerUp時に実行する処理をまとめるメソッド
     */
    public virtual void ProcessOnPointerUp()
    {

    }

    /**
     * OnBeginDrag時に実行
     * @brief OnBeginDrag時に実行する処理をまとめるメソッド
     */
    public virtual void ProcessOnBeginDrag(Vector2 beginDragPos)
    {

    }

    /**
     * OnDrag時に実行
     * @brief OnDrag時に実行に実行する処理をまとめるメソッド
     */
    public virtual void ProcessOnDrag(Vector2 processInDragPos)
    {

    }

    /**
    * OnEndDrag時に実行
    * @brief OnEndDrag時に実行に実行する処理をまとめるメソッド
    */
    public virtual void ProcessOnEndDrag(Vector2 processOnEndDragPos)
    {

    }

    /**
     * OnPointerDown時の効果処理の実行
     * @brief OnPointerDown時の効果処理をまとめるメソッド
     */
    public virtual void OnPointerUpOnEffect()
    {
        
    }

    /**
     * OnPointerDown時の効果処理の実行
     * @brief  OnPointerDown時の効果処理をまとめるメソッド
     */
    public virtual void OnPointerDownOnEffect()
    {

    }


}
