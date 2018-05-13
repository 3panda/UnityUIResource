/**
 * @file UITouchEvent.cs
 * @brief スライドショー全体を管理
 * @author Ryota Shiroguchi
 * @date 2016-12-08
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;


/**
 * @class UISlideshow
 * @brief スライドショー全体を管理
 */
public class UISlideshow : MonoBehaviour
{

    //全てのコンテンツが動いているかどうか
    private bool m_inAllContentsMoving = false;
    //中央に配置配置するコンテンツのindex番号
    private int m_setContentsCenterNumber;
    //コンテンツにセットするフェードの期間
    private float m_contentsDuration = 1.0f;
    //スライドさせるコンテンツのGameObjectを格納するList
    private List<GameObject> m_contentObjsList = new List<GameObject>();
    //スライドさせるコンテンツ
    private List<UISlideshowContents> m_slideshowContentsList = new List<UISlideshowContents>();
    //インスペクターで設定するコンテンツのGameObject
    public List<GameObject> SetContentObjsList;
    public List<GameObject> ContentObjsList
    {
        get
        {
            return m_contentObjsList;
        }
    }
    //各indexの座標を保持するList
    private List<Vector2> m_contentsPosList = new List<Vector2>();
    public List<Vector2> ContentsPosList
    {
        get
        {
            return m_contentsPosList;
        }
    }
    //初期のリストで画面の中央に配置されたコンテンツのindex番号
    private int m_contentsCenterNumber;
    public int ContentsCenterNumber
    {
        get
        {
            return m_contentsCenterNumber;
        }
    }
    //コンテンツ一つのサイズ
    private float m_contentsWidth;
    public float ContentsWidth
    {
        get
        {
            return m_contentsWidth;
        }
    }
    //Listの数
    private int m_contentsCount;
    public int ContentsCount
    {
        get
        {
            return m_contentsCount;
        }
    }
    //Listのindexの最後の数
    private int m_contentsLastIndex;
    public int ContentsLastIndex
    {
        get
        {
            return m_contentsLastIndex;
        }
    }


    void Awake()
    {
        setup();

    }

    /**
     * セットアップ
     * @brief シーン開始の前に取得が必要なフィールドの値を取得
     */
    private void setup()
    {
        m_contentObjsList = SetContentObjsList;
        m_contentsWidth = m_contentObjsList[0].GetComponent<RectTransform>().sizeDelta.x;
        m_contentsCount = m_contentObjsList.Count;
        m_contentsLastIndex = m_contentsCount -1;

        //コンテンツのList から スライドさせるコンテンツと座標を取得しListに格納
        foreach (GameObject obj in m_contentObjsList)
        {
            Vector2 objPos = obj.GetComponent<RectTransform>().anchoredPosition;
            UISlideshowContents objContents = obj.GetComponent<UISlideshowContents>();
            m_contentsPosList.Add(objPos);
            m_slideshowContentsList.Add(objContents);

        }

    }

    /**
     * セットアップ (外部アクセス用)
     * @brief シーン開始の前に取得が必要なフィールドの値を取得
     * @todo シーンの作業を単独で行う為に必要になるので最終的に不要になる
     */
    public void Setup()
    {
        setup();
        
    }


    /**
     * 初期化
     * @brief 配置番号とアニメーションの期間を受け取り シーンの読み込み時の初期化を行う
     * @param setNumber 配置番号
     * @param setDuration アニメーションの期間
     *
     */
    public void Initialization(int setNumber, float setDuration)
    {
        //中心になるコンテンツのX座標を求める
        //中心になるコンテンツのX座標 = (親である自身のサイズ/2) - (コンテンツ1つ分/2)
        float objWidht =  gameObject.GetComponent<RectTransform>().sizeDelta.x;
        float setPosIndex = (objWidht/2) - (m_contentsWidth/2);
        //Ex) 3200/2 - 640/2 = 1280

        //画面の中央に位置するコンテンツのindex番号を取得
        for (int i = 0; i < m_contentsCount; i ++)
        {
            if(m_contentsPosList[i].x == setPosIndex)
            {
                m_contentsCenterNumber = i;
            }
        }

        m_contentsDuration = setDuration;
        allSetContents(setNumber);

    }


    /**
     * 全てのコンテンツを設定と配置
     * @brief 指定したindex番号のカテゴリを中心に全てのコンテンツを配置を行う
     * @param center 中心に移動させるカテゴリのindex番号
     * @returns 
     */
    private void allSetContents(int center)
    {
        //移動する値 = [画面中央のindx番号] - [中央に移動するindex番号]　の差分
        int addIndexNum = m_contentsCenterNumber - center;

        //コンテンツの並び替え
        for (int j = 0; j < m_contentsCount; j ++)
        {
            int posNum = setContentsNum(j, addIndexNum);
            m_contentObjsList[j].GetComponent<RectTransform>().anchoredPosition = m_contentsPosList[posNum];
            //移動先のindex番号を設定
            m_slideshowContentsList[j].SetPosNumber(posNum, true);
            //フェードの時間を指定
            m_slideshowContentsList[j].SetDuration(m_contentsDuration);
        }

    }


    /**
     * 移動先のindex番号を取得
     * @brief 指定したコンテンツの移動先のindex番号を取得
     * @param moveIndex 移動させるコンテンツのindex番号
     * @param addIndexNum 移動する量(加算するindex番号)
     * @returns  移動位置（座標のListのindex番号）
     */
    private int setContentsNum(int moveIndex, int addIndexNum)
    {

        //コンテンツindex番号と 移動する値　の合計
        int sumNum = moveIndex + addIndexNum;
        int posIndex;

        //sumNum が 0以下 の時はListの数を加算
        if (sumNum < 0)
        {
            posIndex = sumNum + m_contentsCount;
        }
        //sumNum　が Listのindex数以上の時はListの数を減算
        else if (sumNum > m_contentsLastIndex)
        {
            posIndex = sumNum - m_contentsCount;
        }
        //それ以外
        else 
        {
            posIndex = sumNum;
        }

        return posIndex;

    }


    /**
     * 全てのコンテンツを現在の位置番号の座標に戻す
     * @brief 全てのコンテンツをのMoveBackContentsメソッドを実行
     */
    public void MoveAllBackContents()
    {
        //すでに動いている場合は動作を停止し誤作動を防止
        m_inAllContentsMoving = CheckAllContentsMovingFlag();
        if (m_inAllContentsMoving)
        {
            return;
        }

        m_inAllContentsMoving = true;

        foreach (UISlideshowContents usc in m_slideshowContentsList)
        {
            usc.MoveBackContents();
        }
    }


    /**
     * 全てのコンテンツを左に移動
     * @brief 全てのコンテンツをのMoveLeftメソッドを実行
     */
    public void MoveLeftAllContents()
    {
        //すでに動いている場合は動作を停止し誤作動を防止
        m_inAllContentsMoving = CheckAllContentsMovingFlag();
        if (m_inAllContentsMoving)
        {
            return;
        }
        

        m_inAllContentsMoving = true;

        foreach (UISlideshowContents usc in m_slideshowContentsList)
        {
            usc.MoveLeft();
        }

    }


    /**
     * 全てのコンテンツを右に移動
     * @brief 全てのコンテンツのMoveRightメソッドを実行
     */
    public void MoveRightAllContents()
    {
        //すでに動いている場合は動作を停止し誤作動を防止
        m_inAllContentsMoving = CheckAllContentsMovingFlag();
        if (m_inAllContentsMoving)
        {
            return;
        }

        m_inAllContentsMoving = true;

        foreach (UISlideshowContents usc in m_slideshowContentsList)
        {
            usc.MoveRight();
        }

    }



    /**
     * 全てのコンテンツが停止状態を確認
     * @brief 全てのコンテンツのInMovingフィールドを確認
     * @returns 全てのコンテンツが停止状態かどうか
     */
    public bool CheckAllContentsMovingFlag()
    {
        bool moving = true;
        for (int k = 0; k < m_contentsCount; k ++)
        {
            if (m_slideshowContentsList[k].InMoving == true)
            {
                return moving;
            }
        }

        moving = false;
        return moving;

    }


    /**
     * 全てのコンテンツを指定された値だけ座標を移動
     * @brief 全てのコンテンツのMoveValueメソッドを実行
     * @param setValue 移動する値
     */
    public void AllMoveValue(float setValue)
    {
        //すでに動いている場合は動作を停止し誤作動を防止
        m_inAllContentsMoving = CheckAllContentsMovingFlag();
        if (m_inAllContentsMoving)
        {
            return;
        }
        
        m_inAllContentsMoving = true;

        foreach (UISlideshowContents usc in m_slideshowContentsList)
        {
            usc.MoveValue(setValue);
        }
    }


    /**
     * 全てのコンテンツを指定されたカテゴリに移動
     * @brief 移動量を計算し全てのコンテンツのDirectMoveメソッドを実行する
     * @param moveCategory 移動先のカテゴリ番号
     */
    public void AllDirectMove(int moveCategory)
    {
        //すでに動いている場合は動作を停止し誤作動を防止
        m_inAllContentsMoving = CheckAllContentsMovingFlag();
        if (m_inAllContentsMoving)
        {
            return;
        }

        m_inAllContentsMoving = true;

        //移動量を計算
        int movePosNum = checkCategoryToMovePos(moveCategory);
        int addNum;
        addNum = m_contentsCenterNumber - movePosNum;

        //中心の位置と移動するコンテンツが同じ(addNum == 0)場合は移動が不要なので処理を終了
        if (addNum == 0)
        {
            return;
        }

        foreach (UISlideshowContents usc in m_slideshowContentsList)
        {
            usc.DirectMove(addNum);
        }

    }


    /**
     * 指定されたカテゴリの移動先を確認
     * @brief 指定されたカテゴリ番号からコンテンツのPosNumberを確認する
     * @param moveCategory カテゴリ番号
     * @returns カテゴリ番号の座標Listのindex番号
     */
    private int checkCategoryToMovePos(int moveCategory)
    {
        int number = 0;

        foreach (UISlideshowContents usc in m_slideshowContentsList)
        {
            //移動するカテゴリ番号のコンテンツの場合
            if (usc.CategoryNumber == moveCategory)
            {
                //コンテンツの配置番号(PosNumber)を返す
                number = usc.PosNumber;
            }
        }

        return number;

    }


}
