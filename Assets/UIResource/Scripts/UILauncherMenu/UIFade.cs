using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class UIFade : MonoBehaviour
{
    //表示しているかどうか
    private bool m_isDisplayed;
    public bool IsDisplayed
    {
        get
        {
            return m_isDisplayed;
        }

    }

    public float FadeTime;
    // Use this for initialization
    void Start()
    {
        //初期化
        initialization();
    }


    //初期化
    private void initialization()
    {
        //非表示に
        m_isDisplayed = false;
        gameObject.GetComponent<CanvasGroup>().alpha = 0;
        
    }

    //表示のみ
    public void Display()
    {
        
        //表示させていたら処理を抜ける
        if (m_isDisplayed)
        {
            return;

        }
        
        //Flagを表示に
        m_isDisplayed = true;
        //不透明度を1に
        gameObject.GetComponent<CanvasGroup>().alpha = 1.0f;
    }

    //非表示のみ
    public void Invisible()
    {
        //表示させていなかったら処理を抜ける
        if (!m_isDisplayed)
        {
            return;

        }

        //Flagを非表示に
        m_isDisplayed = false;
        //不透明度を0に
        gameObject.GetComponent<CanvasGroup>().alpha = 0;
    }

    //フェードイン
    public void FadeIn(float fadeTime = 0)
    {
        //表示させていたら処理を抜ける
        if (m_isDisplayed)
        {
            return;

        }

        m_isDisplayed = true;

        if (fadeTime == 0)
        {
            fadeTime = FadeTime;

        }

        //フェードイン
        gameObject.GetComponent<CanvasGroup>().DOFade(1.0f, fadeTime);

    }

    //フェードアウト
    public void FadeOut (float fadeTime = 0)
    {
        //表示させていなければ処理を抜ける
        if (!m_isDisplayed)
        {
            return;

        }

        m_isDisplayed = false;

        if (fadeTime == 0)
        {
            fadeTime = FadeTime;

        }

        //フェードアウト
        gameObject.GetComponent<CanvasGroup>().DOFade(0, fadeTime);

    }

}
