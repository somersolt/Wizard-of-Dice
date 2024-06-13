using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel : MonoBehaviour
{
    public Mediator mediator;


    public virtual void Init()
    {
        mediator = FindObjectOfType<Mediator>();
    }


    public virtual void SlideOpenPanel()
    {
        gameObject.SetActive(true);
        mediator.ui.backGroundPanel.gameObject.SetActive(true);
        StartCoroutine(PanelSlide(this));
    }
    public virtual void OpenPanel()
    {
        gameObject.SetActive(true);
        mediator.ui.backGroundPanel.gameObject.SetActive(true);
    }

    protected virtual void ClosePanel()
    {
        gameObject.SetActive(false);
        mediator.ui.backGroundPanel.gameObject.SetActive(false);
    }
    protected void RewardSound(int i)
    {
        mediator.ui.audioSource.PlayOneShot(mediator.ui.uiAudioClips[i]);
    }

    protected IEnumerator PanelSlide(Panel panel)
    {
        float duration = 0.3f;
        float time = 0f;
        Vector3 startpos = panel.transform.position;

        panel.transform.position = new Vector3(1000, startpos.y, startpos.z);

        while (time < duration)
        {
            panel.transform.position = Vector3.Lerp(panel.transform.position, startpos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        panel.transform.position = startpos;
    }
}
