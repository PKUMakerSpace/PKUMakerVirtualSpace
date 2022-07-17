using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace PKU.Readable
{
    public class ReadBubbleController : MonoBehaviour
    {
        private Camera mainCamera = null;

        [SerializeField]
        private RectTransform readBubble;

        [SerializeField]
        private TMP_Text bubbleTitle;

        [SerializeField]
        private TMP_Text bubbleContext;

        /// <summary>
        /// 气泡是否正在被显示（从气泡出现到气泡消失都算）
        /// </summary>
        private bool isBubbleShowing = false;

        private bool isAnimPlaying = false;

        private Coroutine readBubbleAnim;

        [SerializeField]
        private CanvasGroup fadeCanvasGroup;

        [Tooltip("淡入时间")]
        [SerializeField]
        private float fadeInTime;

        [Tooltip("展示时间")]
        [SerializeField]
        private float displayTime;

        [Tooltip("淡出时间")]
        [SerializeField]
        private float fadeOutTime;

        private void Start()
        {
            fadeCanvasGroup.alpha = 0;
        }

        private void Update()
        {
            if (mainCamera != null)
            {
                BubbleFaceCamera();
            }
            else
            {
                if (GameObject.FindGameObjectWithTag("PlayerCamera") != null)
                    mainCamera = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<Camera>();
            }

        }

        private void BubbleFaceCamera()
        {
            transform.LookAt(mainCamera.transform);
            transform.Rotate(Vector3.up, 180);
        }

        public void ShowReadBubble(string title, string context)
        {
            if (isBubbleShowing)
                return;

            bubbleTitle.SetText(title);
            bubbleContext.SetText(context);

            if (isAnimPlaying)
            {
                StopCoroutine(readBubbleAnim);
            }

            readBubbleAnim = StartCoroutine(ShowReadBubbleAnim());
        }

        public void HideReadBubble()
        {
            if (!isBubbleShowing)
                return;

            if (isAnimPlaying)
            {
                StopCoroutine(readBubbleAnim);
            }

            Debug.Log("Hide Read Bubble!");

            readBubbleAnim = StartCoroutine(HideReadBubbleAnim());
        }

        private IEnumerator ShowReadBubbleAnim()
        {
            isAnimPlaying = true;

            isBubbleShowing = true;

            fadeCanvasGroup.alpha = 0;

            LayoutRebuilder.ForceRebuildLayoutImmediate(readBubble.GetComponent<RectTransform>());

            // 淡入

            yield return fadeAnim(1.0f, fadeInTime);

            isAnimPlaying = false;

            yield return null;
        }

        private IEnumerator HideReadBubbleAnim()
        {
            isAnimPlaying = true;

            yield return fadeAnim(0f, fadeOutTime);

            isBubbleShowing = false;

            isAnimPlaying = false;

            yield return null;
        }

        private IEnumerator fadeAnim(float targetAlpha, float fadeTime)
        {
            float fadeSpeed = Mathf.Abs(fadeCanvasGroup.alpha - targetAlpha) / fadeTime;

            while (!Mathf.Approximately(fadeCanvasGroup.alpha, targetAlpha))
            {
                fadeCanvasGroup.alpha = Mathf.MoveTowards(fadeCanvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
                yield return null;
            }

        }
    }
}


