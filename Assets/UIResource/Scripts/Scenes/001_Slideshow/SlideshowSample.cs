using UnityEngine;
using System.Collections;

public class SlideshowSample : MonoBehaviour
{
    [SerializeField]
    private UISlideshow m_slideshow;
    [SerializeField]
    private int m_setNumber = 2;
    [SerializeField]
    private float m_setDuration = 0.5f;
    
    void Start()
    {
        m_slideshow.Initialization(m_setNumber, m_setDuration);
    }

}
