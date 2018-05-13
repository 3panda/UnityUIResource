/**
 * @file UISlideshowContents.cs
 * @brief スライドショーの各コンテンツの動作を設定
 * @author Ryota Shiroguchi
 * @date 2016-12-08
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

/**
 * @class UISlideshowContents
 * @brief スライドショーの各コンテンツの動作を設定
 */
public class UISlideshowContents : MonoBehaviour
{
    //全てのコンテンツの数
    private int m_allContentsCount;
    //コンテンツの最後のindex番号(右端)
    private int m_allContentsLast;
    //アニメーションの期間
    private float m_duration = 1.0f;
    //自身も含めたコンテンツのList
    private List<GameObject> m_allContentsList = new List<GameObject>();
    //移動用の座標List
    private List<Vector2> m_allContentsPosList = new List<Vector2>();
    //スライドショーの本体を取得
    public UISlideshow Slideshow;
    //自身の横幅
    private float m_contentsWidth;
    //自身の配置番号
    private int m_posNumber;
    public int PosNumber
    {
        get
        {
            return m_posNumber;
        }
    }
    //自身のカテゴリ番号 = 初期の配置番号
    private int m_categoryNumber;
    public int CategoryNumber
    {
        get 
        {
            return m_categoryNumber;
        }
    }
    //動いているかどうか
    private bool m_inMoving = false;
    public bool InMoving
    {
        get 
        {
            return m_inMoving;
        }
    }


    void Start ()
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
        m_allContentsList = Slideshow.ContentObjsList;
        m_allContentsPosList = Slideshow.ContentsPosList;
        m_allContentsCount = Slideshow.ContentsCount;
        m_allContentsLast = Slideshow.ContentsLastIndex;
        m_contentsWidth = Slideshow.ContentsWidth;
        m_categoryNumber = getCategoryNumber();

    }

    /**
     * 配置番号と座標を更新
     * @brief 指定した番号に自身の配置番号を更新し座標も変更
     * @param number 変更後の番号
     * @param isBeforeInitialization 初期化処理の前かどうか
     */
    private void setPosNumber(int number, bool isBeforeInitialization = false)
    {

        int count;

        //初期化前
        if(isBeforeInitialization)
        {
            count = Slideshow.ContentsCount;
        }
        //初期化後
        else
        {
            count = m_allContentsCount;
        }


        //適切で無い数値が入った場合
        if(number < 0 || number >= count)
        {
            //Debug.LogError("カテゴリ数の最大数を超えている、または最小数を下回っています。");
            return;
        }

        //配置番号の更新
        m_posNumber = number;

    }

    /**
     * 配置番号と座標を更新 (外部アクセス用)
     * @brief 外部から配置番号と座標を更新
     * @param number 変更後の番号
     * @param isBeforeInitialization 初期化処理の前かどうか
     */
    public void SetPosNumber(int number, bool isBeforeInitialization = false)
    {
        setPosNumber(number,isBeforeInitialization);
    }


    /**
     * 指定した値にアニメーションの期間を設定
     * @brief アニメーションの期間を外部からセットする
     * @param duration アニメーションの期間
     */
    public void SetDuration(float duration)
    {
         m_duration = duration;
    }


    /**
     * カテゴリ番号を取得
     * @brief Listの並び順から自身のカテゴリ番号を取得する
     * @returns カテゴリ番号
     */
    private int getCategoryNumber()
    {
        int number;
        number = m_allContentsList.IndexOf(gameObject);
        return number;
    }


    /**
     * 指定した配置番号の座標に移動
     * @brief 引数で指定した配置番号の座標に移動する
     * @param index 移動先の配置Listの番号
     *
     */
    private void moveContents(int index)
    {
        //すでに動いている場合は動作を停止し誤作動を防止
        if(m_inMoving)
        {
            return;
        }

        m_inMoving = true;

        //左端を超えた場合は右端へ
        if (index < 0)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = m_allContentsPosList[m_allContentsLast];
            setPosNumber(m_allContentsLast);
            StartCoroutine(noFadeMoveCallBack());

        }
        //右端を超えた場合は左端へ
        else if(index > m_allContentsLast)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = m_allContentsPosList[0];
            setPosNumber(0);
            StartCoroutine(noFadeMoveCallBack());

        }
        //通常
        else
        {
            Vector2 objPos = m_allContentsPosList[index];
            gameObject.GetComponent<RectTransform>().DOAnchorPosX(objPos.x, m_duration).SetEase(Ease.OutQuart).OnComplete(fadeMoveCallBack);
            setPosNumber(index);
        }
    }


    /**
     * 現在の位置番号の座標に戻す
     * @brief フリック処理などで座標が移動した座標を現在の配置番号の座標に戻す
     */
    public void MoveBackContents()
    {
        moveContents(m_posNumber);
    }


    /**
     * 座標を移動した後のコールバック処理
     * @brief 座標を移動した後にイージングの補正と移動状態を完了に
     */
    private void fadeMoveCallBack()
    {
        gameObject.GetComponent<RectTransform>().anchoredPosition = m_allContentsPosList[m_posNumber];
        m_inMoving = false;
    }


    /**
     * Tweenを行わずに移動する場合のコールバック
     * @brief イージングの時間分遅延して後に実行
     */
    private IEnumerator noFadeMoveCallBack()
    {
        //他のフェードとタイミングを合わせる
        yield return new WaitForSeconds(m_duration);
        m_inMoving = false;
    }

    /**
     * 現在の座標から左から右へ1つ移動する
     * @brief 現在の配置番号に1を加算し座標を移動
     *
     */
    public void MoveLeft()
    {
        moveContents(m_posNumber + 1);
    }


    /**
     * 現在の座標から左から右へ1つ移動する
     * @brief 現在の配置番号に1を減算し座標を移動
     *
     */
    public void MoveRight()
    {
        moveContents(m_posNumber - 1);
    }


    /**
     * 指定された値だけ座標を移動
     * @brief 現在の座標に 指定した値を加算(減算)する処理
     * @param setValue 移動する値
     */
    public void MoveValue(float setValue)
    {
        //すでに動いている場合は動作を停止し誤作動を防止
        if(m_inMoving) 
        {
            return;
        }


        //現在の座標の更新
        Vector2 currentPos =  gameObject.GetComponent<RectTransform>().anchoredPosition;
        //反映する座標の初期化
        Vector2 setPos = new Vector2(currentPos.x, currentPos.y);

        //値を絶対値に変換
        float absoluteValue = Mathf.Abs(setValue);
        //コンテンツとスクリーンの比率
        float screenRate = m_contentsWidth / Screen.width;
        //加算する値
        float addValue = absoluteValue * screenRate;

        m_inMoving = true;
        
        //左右の判定
        if (setValue > 0)
        {
            setPos = new Vector2(currentPos.x + addValue, currentPos.y);

        }
        else if (setValue < 0)
        {
            setPos = new Vector2(currentPos.x - addValue, currentPos.y);
        }

        gameObject.GetComponent<RectTransform>().anchoredPosition = setPos;
        m_inMoving = false;

    }


    /**
     * 指定した座標に一気に移動する
     * @brief 現在の配置番号に移動量を加算して一気に座標を移動する
     * @param addNum 加算する移動量
     */
    public void DirectMove(int addNum)
    {
        //すでに動いている場合は動作を停止し誤作動を防止
        if(m_inMoving)
        {
            return;
        }

        m_inMoving = true;
        int setPos = m_posNumber + addNum;

        //移動量と自身の配置番号の合計が右端(Listの最後)より大きい場合
        if (setPos > m_allContentsLast)
        {
            setPos = setPos - m_allContentsCount;
            gameObject.GetComponent<RectTransform>().anchoredPosition = m_allContentsPosList[setPos];
            setPosNumber(setPos);
            StartCoroutine(noFadeMoveCallBack());

        }
        //移動量と自身の配置番号の合計が左端(0)より小さい場合
        else if (setPos < 0)
        {
            setPos = setPos + m_allContentsCount;
            gameObject.GetComponent<RectTransform>().anchoredPosition = m_allContentsPosList[setPos];
            setPosNumber(setPos);
            StartCoroutine(noFadeMoveCallBack());

        }
        //通常
        else
        {
            Vector2 objPos = m_allContentsPosList[setPos];
            gameObject.GetComponent<RectTransform>().DOAnchorPosX(objPos.x, m_duration).SetEase(Ease.OutQuart).OnComplete(fadeMoveCallBack);
            setPosNumber(setPos);

        }

    }


}
